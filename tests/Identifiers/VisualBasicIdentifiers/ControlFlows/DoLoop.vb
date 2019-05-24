Public Class DoLoop

  Sub Create(arg%)

    Do While arg < 10
    Loop

    Do While arg < 10
      Continue Do
    Loop

    Do While arg < 10
      Exit Do
    Loop

    Do
    Loop While arg < 10

    Do Until arg < 10
    Loop

    Do
    Loop Until arg < 10

  End Sub

End Class