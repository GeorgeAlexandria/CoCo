Namespace Access.Members

  Public Class TypeEvent

    Private Class Temp

      Public Shared Event Closed As EventHandler

    End Class

    Public Sub Create()

      Dim handler As EventHandler = AddressOf Create

      AddHandler Temp.Closed, handler
      RemoveHandler Temp.Closed, handler

      AddHandler Temp.Closed, AddressOf Create
      RemoveHandler Temp.Closed, AddressOf Create
    End Sub

  End Class

End Namespace