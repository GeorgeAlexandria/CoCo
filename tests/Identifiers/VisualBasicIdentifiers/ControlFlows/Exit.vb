Public Class [Exit]

  Sub Create()
    Exit Sub
  End Sub

  Function Create(arg%) As Integer

    Try
      Exit Try
    Catch ex As System.Exception
      Exit Try
    End Try

    Exit Function
  End Function

  Public Property Value() As String
    Get
      Exit Property
    End Get
    Set(ByVal value As String)
      Exit Property
    End Set
  End Property

End Class