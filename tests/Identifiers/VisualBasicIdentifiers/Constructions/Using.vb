Public Class [Using]

  Public Sub Create()
    Using resource

    End Using

    Using stream As New IO.FileStream("", IO.FileMode.Append)

    End Using

  End Sub

End Class