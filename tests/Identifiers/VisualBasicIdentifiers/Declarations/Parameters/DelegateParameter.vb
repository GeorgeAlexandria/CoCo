Public Class DelegateParameter

  Private action As Action(Of Integer) = Sub(arg As Integer) Console.Write(arg)

  Private func As Func(Of Integer, Boolean) = Function(arg As Integer) arg > 0

End Class