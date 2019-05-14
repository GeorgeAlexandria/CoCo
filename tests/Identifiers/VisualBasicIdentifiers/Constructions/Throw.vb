Public Class [Throw]

  Public Sub Create()
    Dim argument = New ArgumentException()
    Throw argument
  End Sub

End Class