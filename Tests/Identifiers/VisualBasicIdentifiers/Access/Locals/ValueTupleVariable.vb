Namespace Access.Locals

  Public Class ValueTupleVariable

    Public Sub Create()
      Dim arg = (1, 2)
      Console.WriteLine(arg.Item1 + arg.Item2)
    End Sub

  End Class

End Namespace