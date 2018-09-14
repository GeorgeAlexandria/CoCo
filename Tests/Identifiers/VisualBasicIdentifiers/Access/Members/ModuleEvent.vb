Namespace Access.Members

  Module TempModule

    Public Event Closed As EventHandler

  End Module

  Public Class ModuleEvent

    Public Sub Create()
      Dim handler As EventHandler = AddressOf Create

      AddHandler TempModule.Closed, handler
      RemoveHandler TempModule.Closed, handler

      AddHandler TempModule.Closed, AddressOf Create
      RemoveHandler TempModule.Closed, AddressOf Create
    End Sub

  End Class

End Namespace