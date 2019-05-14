Namespace Access.Parameters

  Public Class DelegateParameter

    Public Sub Create()

      Dim action = Sub(arg As Integer) Console.Write(arg)
      Dim [function] = Function(arg As Integer) arg + arg * 2

    End Sub

  End Class

End Namespace