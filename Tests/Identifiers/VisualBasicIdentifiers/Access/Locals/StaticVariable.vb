Namespace Access.Locals

  Public Class StaticVariable

    Public Sub Create()
      Static variable As Integer = 5
      Static variable2 As Integer = variable
    End Sub

  End Class

End Namespace