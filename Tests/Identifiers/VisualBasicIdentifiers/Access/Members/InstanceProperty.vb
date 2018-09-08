Namespace Access.Locals

  Public Class InstanceProperty

    Public Sub Create()
      Dim info = New Long?
      If info.HasValue Then Console.WriteLine(info.Value)
    End Sub

  End Class

End Namespace