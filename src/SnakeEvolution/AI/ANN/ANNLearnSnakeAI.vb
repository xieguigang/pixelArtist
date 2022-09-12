Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.QLearning

Public Class ANNLearnSnakeAI

    Dim core As Network

    Sub New()
        core = New Network(
            inputSize:=inputSize,
            hiddenSize:=hiddenSize,
            outputSize:=outputSize,
            learnRate:=learnRate,
            momentum:=momentum,
            active:=active,
            weightInit:=weightInit
        )
    End Sub

End Class
