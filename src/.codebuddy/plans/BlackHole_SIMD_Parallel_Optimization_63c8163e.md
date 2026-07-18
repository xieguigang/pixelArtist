---
name: BlackHole_SIMD_Parallel_Optimization
overview: 对 BlackHole 黑洞模拟器进行并行化与 SIMD 优化。核心是引入基于 System.Numerics.Vector(Of Double) 的"光子包(Packet)追踪"重写测地线 RK4 积分器（Schwarzschild 与 Kerr 双路径），使一条 SIMD 向量寄存器同时积分 4~8 条光子，对占 90%+ 耗时的 RK4 热循环获得约向量宽度倍速的提升；同时将渲染主循环改为按包(Packet)在 Parallel.For 中并行，向量化星空点积循环、引入 BlackBody 查表(LUT)消除热路径超越函数，并对共享的 GaussianBlur/PixelBuffer 做像素级并行。保留原标量 Trace 作为正确性对照与 A/B 开关。
todos:
  - id: packet-schwarzschild
    content: 在 GeodesicsPacket.vb 实现 Schwarzschild 向量化 RK4 与 TracePacket（含 active 掩码终止与盘穿越）
    status: completed
  - id: packet-kerr
    content: 在 GeodesicsPacket.vb 实现 Kerr 向量化 KerrDeriv/RK4/TracePacket（Vector.Sin/Cos + turning-point 反射）
    status: completed
    dependencies:
      - packet-schwarzschild
  - id: renderer-packet-parallel
    content: 改写 BlackHoleRenderer 按包并行渲染并调用 TracePacket，保留标量 Trace 与 UsePacketTracing 开关
    status: completed
    dependencies:
      - packet-schwarzschild
      - packet-kerr
  - id: blackbody-lut
    content: 在 AccretionDisk.vb 增加 BlackBody 查表 LUT 替代超越函数
    status: completed
  - id: starfield-simd
    content: 将 Starfield 星数据改为并行数组并以 Vector 并行点积累积
    status: completed
  - id: shared-parallel
    content: 为共享 GaussianBlur 与 PixelBuffer 的像素循环添加 Parallel.For
    status: completed
  - id: ab-verify
    content: 使用 [subagent:code-explorer] 交叉核对标量/包追踪逐像素 RGB 容差与渲染性能基准
    status: completed
    dependencies:
      - renderer-packet-parallel
      - blackbody-lut
      - starfield-simd
      - shared-parallel
---

## 用户需求

针对 BlackHole（VB.NET 黑洞模拟器）项目，对核心计算代码实施「并行化」与「SIMD」性能优化，在不改变视觉输出的前提下显著降低单帧渲染耗时。

## 产品概述

保持原有黑洞/吸积盘/星空渲染效果不变，内部将最耗时的逐光子测地线 RK4 积分改为「光子包(Packet)追踪」——一条 SIMD 向量寄存器同时积分 4~8 条光子；并完善渲染与后处理的并行度，引入 BlackBody 查表消除热路径超越函数。

## 核心特性

- **Packet Tracing（SIMD 测地线积分）**：用 `System.Numerics.Vector(Of Double)` 重写 Schwarzschild 与 Kerr 两条测地线 RK4 积分器，对 RK4 最热循环获得约向量宽度（AVX2=4 / AVX-512=8）倍速。
- **按包并行渲染**：`BlackHoleRenderer.Render` 以 LANES 个连续像素为包，在 `Parallel.For` 中并行调用 `TracePacket`，保持写缓冲缓存局部性。
- **BlackBody LUT**：用预计算温度→RGB 查表+线性插值替代 `Math.Pow/Log` 超越函数，覆盖吸积盘与星空热路径。
- **Starfield SIMD**：星空星数据改为并行数组，点积累积循环用向量并行。
- **共享类并行**：`GaussianBlur`、`PixelBuffer` 的像素独立循环加 `Parallel.For`。
- **正确性对照**：保留原标量 `Trace` 与新 `UsePacketTracing` 开关，支持 A/B 逐像素 RGB 容差比对。

## 技术栈选择

- 语言/框架：VB.NET，`net10.0-windows`，目标 `x64`（BlackHole.vbproj 第 5、9 行）。
- SIMD 基元：`System.Numerics.Vector(Of Double)`；`Vector(Of Double).Count` 自适应运行机指令集（AVX2=4 / AVX-512=8）。net10 提供 `Vector.Sin/Cos/Sqrt`（Kerr 路径所需）。
- 并行基元：`System.Threading.Tasks.Parallel`、`System.Threading.Tasks.Dataflow` 或自带 `Partitioner`（沿用现有 `Parallel.For` 风格）。
- 复用现有类型：`PhotonResult`/`DiskHit`（`Geodesics.vb`）、`vec3 = Point3D`、共享 `PixelBuffer`/`GaussianBlur`。新增 SIMD 逻辑不引入新的外部依赖。

## 实现方案

**策略**：将「逐光子串行 RK4」改为「N 条光子打包、按分量对齐的 SIMD 锁步积分」。一条光子无法在自身时间轴上向量化（步间强依赖），但 N 条光子的 RK4 每一步对同一公式独立，天然可分通道。包内维持 `active` 掩码，逐通道独立终止（捕获/逃逸/盘穿越/turning-point 反射）。

**关键技术决策**：

1. **Packet 状态用 `Vector(Of Double)` 而非 `Point3D`**：`Point3D` 为标量 `Double` 结构体，逐分量 `Add/Sqrt` 会生成大量标量指令；SIMD 直接持有 `px,py,pz,vx,vy,vz`（Schwarzschild）与 `r,th,ph,L,Q,signR,signTh`（Kerr），全部为 `Vector(Of Double)`，最后再转回 `PhotonResult`/`DiskHit`，避免逐分量 `Point3D` 调用开销。
2. **掩码终止而非分支**：每步后据比较结果用 `Vector.ConditionalSelect` / `And` 更新 `active`；终止通道的 p/v 置 0 不再变化，结果写入输出 `PhotonResult()` 数组。盘穿越逐通道算 `t/hit/rad`，命中向该通道独立 `List(Of DiskHit)` 追加。
3. **初值标量、循环向量**：`SolveConserved`/`CartesianVelToBL`/`EmbeddingToBL` 等初值计算逐光子标量完成，再彼此打包进向量——这些只算一次、占比极小，不向量化更安全。
4. **Kerr 必须向量化 `Sin/Cos`**：`KerrDeriv` 含 `Sin(th)/Cos(th)` 与 `Sqrt(Delta/Sigma)`；net10 `Vector.Sin/Cos/Sqrt` 可直接向量化，避免回退标量。
5. **保留标量 `Trace` 作对照**：新增静态开关 `Geodesics.UsePacketTracing`（默认 True）。渲染器可切回标量做逐像素 RGB 容差比对，确保浮点结果在视觉容差内一致。
6. **BlackBody LUT**：静态预计算温度∈[500,40000]K（对应 `AccretionDisk` 中 clamp 区间，含 Starfield 区间）、步进 ~50K 的 (r,g,b) 表；`Color` 改为查表+线性插值，与 Tanner Helland 公式视觉一致。

**性能与可靠性**：RK4 热循环为 `O(maxSteps × 每步常量)`，SIMD 将其降为约 `O(maxSteps / LANES)` 的向量步数（指令数降为 1/LANES）。瓶颈从「标量算术吞吐」转为「内存/掩码混合」，需保证 `active` 掩码用 `Vector` 运算而非逐通道 `If`（避免 AVX 通道间的标量回退）。包大小=LANES 随硬件自适应，无 AVX-512 也能跑（AVX2=4）。

## 实现要点（防回归）

- 复用现有 `Parallel.For` 习惯；新增并行循环仅限像素相互独立处（GaussianBlur/PixelBuffer/星空/渲染），绝不并行改写共享可变状态。
- `TracePacket` 输出数组长度须 = LANES；尾部不满一包的像素用零方向填充 `active=0`，渲染侧按真实像素数截断。
- `ComputePixel` 改为接收已追踪的 `PhotonResult`（签名 `ComputePixel(result, model, sky, u, v)`），保持 `Render` 对外签名（`width,height,CancellationToken`）不变以兼容 `FormMain`。
- 启动期打印 `Vector(Of Double).Count` 便于确认向量宽度；LUT 在首次访问惰性构建并缓存。
- 保留浮点容差测试（≤1/255）作为验收，避免 SIMD 重排序导致的肉眼差异。

## 架构设计

```
Render(Parallel.For over packets)
   └─ 生成 LANES 条 ray (连续像素, NormCoords)
   └─ Geodesics.TracePacket(origins(), dirs(), model) As PhotonResult()
         ├─ Schwarzschild 包 RK4 (Vector)  [Spin < 0.001]
         └─ Kerr 包 RK4 (Vector + Vector.Sin/Cos)  [Spin > 0]
   └─ 逐通道 ComputePixel(result(lane), model, sky, u, v) → PixelBuffer
Bloom (GaussianBlur, 并行两遍) → ToBitmap (并行)
Starfield.GetColor (SIMD 点积)  // 逃逸像素背景
```

标量 `Trace` 仍保留为 `UsePacketTracing=False` 的对照实现，结构上为双实现分发，不影响现有调用方。

## 目录结构

```
src/BlackHole/
├── Geodesics.vb          # [MODIFY] 保留原标量 Trace/TraceSchwarzschild/TraceKerr 作为对照；新增
│                         #   UsePacketTracing 静态开关与 TracePacket 分发（按 Spin 选择包实现）。
├── GeodesicsPacket.vb    # [NEW] 光子包 SIMD 积分器。定义 PacketState(Schwarzschild/Kerr) 与
│                         #   Vector(Of Double) 状态；实现 VectorSchwarzschildAccel、RK4SchwarzschildPacket、
│                         #   TraceSchwarzschildPacket；以及 VectorKerrDeriv、RK4KerrPacket、TraceKerrPacket；
│                         #   逐通道 active 掩码、捕获/逃逸/盘穿越检测、Kerr turning-point 反射；
│                         #   导出 TracePacket(origins() As vec3, dirs() As vec3, model) As PhotonResult()。
├── BlackHoleRenderer.vb  # [MODIFY] Render 改为 Parallel.For 按包迭代（packetCount=Ceiling(W*H/LANES)），
│                         #   每包生成 LANES 条 ray、调用 Geodesics.TracePacket、逐通道 ComputePixel 写 buffer；
│                         #   ComputePixel 改为接收 PhotonResult 的签名；NormCoords 保持不变。
├── AccretionDisk.vb      # [MODIFY] BlackBody 增加静态 LUT（温度→RGB，线性插值），Color 改为查表实现；
│                         #   ComputeEmission 逻辑不变，仅受益 LUT 提速。
└── Starfield.vb          # [MODIFY] 星数据改为并行数组 dirX/dirY/dirZ/magR/magG/magB；GetColor 用
│                           Vector(Of Double) 每 LANES 颗并行点积+阈值累加。
src/Astrophysics/raytracing/pixeldata/
├── GaussianBlur.vb       # [MODIFY] blurHorizontally/blurVertically 外层循环改为 Parallel.For（像素独立）。
└── PixelBuffer.vb        # [MODIFY] add/multiply/filterByEmission/clone 像素循环改为 Parallel.For。
```

## 关键代码结构

```
' GeodesicsPacket.vb —— 包积分对外接口（接口级定义）
Public Shared Function TracePacket(
        origins() As vec3, dirs() As vec3, model As BlackHoleModel) As PhotonResult()
    ' origins/dirs 长度 = LANES (= Vector(Of Double).Count)；返回等长 PhotonResult()
End Function

' Schwarzschild 包状态：每个字段均为 Vector(Of Double)，active 用 1.0/0.0 表示
Structure PacketStateS
    Public px, py, pz, vx, vy, vz, h2, active As Vector(Of Double)
End Structure

' Kerr 包状态：BL 坐标 + 守恒量 + 方向符号，全部 Vector(Of Double)
Structure PacketStateK
    Public r, th, ph, L, Q, signR, signTh, active As Vector(Of Double)
    Public aGeom As Double   ' 几何自旋，对所有通道相同
End Structure
```