Public Class [With]

  Private Class Temp
    Public field As Integer
  End Class

  Public Sub Create()
    With argument
    End With

    Dim value = 5
    With value
      .CompareTo(2)
    End With

    Dim variable = New Temp With {.field = 25}
  End Sub

End Class