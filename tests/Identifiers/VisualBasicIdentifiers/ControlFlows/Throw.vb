Namespace ControlFlow

  Public Class [Throw]

    Sub Create()

      Throw New System.Exception()
      Try
      Catch ex As System.Exception
        Throw
      End Try
    End Sub

  End Class

End Namespace