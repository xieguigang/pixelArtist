---
name: fix-box-model-and-gravity
overview: 修复三维盒子线框模型的边连接错误、修复墙壁碰撞边界的坐标系不一致、将重力方向固定为始终指向局部坐标系 -Y 方向（窗口底部）。
todos:
  - id: fix-box-edges
    content: 修正 Renderer.vb 中 edges 数组，将底面和顶面的对角线连接改为正确的相邻边连接
    status: completed
  - id: fix-wall-boundary
    content: 修正 Simulation.vb 中 ResolveWalls 边界从 [-half,+half] 改为 [0, BoxSize]
    status: completed
  - id: fix-gravity-direction
    content: 简化 Simulation.vb 中 ComputeGravityLocal 为固定方向 (0, -gravityStrength, 0)，移除摄像机旋转依赖
    status: completed
  - id: update-hint-text
    content: 更新 MainForm.vb 中交互提示文字，移除重力随旋转变化的误导描述
    status: completed
    dependencies:
      - fix-gravity-direction
---

## 用户需求

修复三维小球碰撞模拟项目中的两个问题：

1. **三维盒子模型不正确**：渲染时盒子的线框边有对角线错误（底面和顶面的边连接到了对角顶点而非相邻顶点）
2. **重力受力方向不正确**：当前重力方向随摄像机旋转而变化，但用户要求重力始终指向窗口底部（即局部坐标系中固定为 `(0, -gravityStrength, 0)`），不随摄像机角度改变

## 核心修复

- 修正盒子12条边的顶点连接顺序，消除对角线
- 修正墙壁碰撞边界从 `[-half, +half]` 改为 `[0, BoxSize]`，与盒子模型坐标一致
- 移除重力方向对摄像机角度的依赖，改为固定方向
- 更新交互提示文字以反映新行为

## 技术栈

- 语言：VB.NET (Windows Forms)
- 渲染：GDI+ 自绘
- 物理引擎：半隐式欧拉积分 + 冲量法碰撞
- 坐标参考：`Microsoft.VisualBasic.Imaging.Drawing3D.Camera` / `Microsoft.VisualBasic.Imaging.Physics.Vector3`

## 问题分析与修复方案

### 问题1：盒子边连接错误 (Renderer.vb)

**根因**：`edges` 数组中，底面和顶面的顶点连接顺序错误，将非相邻顶点相连形成对角线。

当前顶点布局（8个角点，范围 `[0, BoxSize]³`）：

- 底面：0:(0,0,0), 1:(s,0,0), 2:(s,s,0), 3:(0,s,0)
- 顶面：4:(0,0,s), 5:(s,0,s), 6:(s,s,s), 7:(0,s,s)

底面正确边应为矩形四条边 `{0,1},{1,2},{2,3},{3,0}`，但当前 `{1,3}` 和 `{2,0}` 是对角线。顶面同理 `{5,7}` 和 `{6,4}` 需修正为 `{5,6}` 和 `{7,4}`。

**修复**：将 edges 数组修正为标准立方体线框的12条边。

### 问题2：墙壁碰撞边界坐标系不一致 (Simulation.vb)

**根因**：`Ball.position` 的范围是 `[0, BoxSize]`（见 Ball.vb 注释和 RandomBall 生成逻辑），但 `ResolveWalls()` 使用 `half = BoxSize/2`，边界为 `[-half, +half]`，导致球被限制在盒子中心的小区域内。

**修复**：将 `ResolveWalls()` 的边界改为 `lo = ballRadiusMin()`, `hi = BoxSize - ballRadiusMin()`，使墙壁与渲染的盒子线框对齐。

### 问题3：重力方向 (Simulation.vb)

**根因**：`ComputeGravityLocal()` 将世界向下向量 `(0,-1,0)` 通过摄像机逆旋转变换到局部坐标系，使重力随摄像机旋转而变化。用户要求重力始终指向窗口底部。

**修复**：将 `ComputeGravityLocal()` 简化为直接返回 `New Vector3(0, -gravityStrength, 0)`，移除所有摄像机旋转逻辑。同时可将该方法简化为内联赋值。

## 修改文件清单

### Renderer.vb — 修正盒子边定义

仅修改 `edges` 数组（第20-24行），修正底面和顶面的边连接。

### Simulation.vb — 两处修复

1. `ResolveWalls()`（第142-201行）：将边界从 `[-half, +half]` 改为 `[0, BoxSize]`
2. `ComputeGravityLocal()`（第78-89行）：简化为固定方向，移除摄像机参数依赖
3. `Step()` 方法（第118-140行）：移除 `ComputeGravityLocal(camera)` 调用，改为直接赋值

### MainForm.vb — 更新提示文字

更新 `lblHint.Text`（第168-175行）中关于"旋转盒子会改变重力相对盒面的方向"的描述。