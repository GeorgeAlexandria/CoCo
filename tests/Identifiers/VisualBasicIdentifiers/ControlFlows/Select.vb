Public Class [Select]

  Sub Create(arg%)

    Select Case arg
      Case 1 To 5
        Exit Select
      Case 2, 7, 8
      Case Is > 10
      Case Else
    End Select

  End Sub

End Class
