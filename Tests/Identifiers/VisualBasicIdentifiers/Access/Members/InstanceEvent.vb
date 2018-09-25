Imports System.Collections.ObjectModel

Namespace Access.Members

  Public Class InstanceEvent

    Public Sub Create()
      Dim handler As EventHandler = AddressOf Create
      AddHandler AppDomain.CurrentDomain.DomainUnload, handler
      RemoveHandler AppDomain.CurrentDomain.DomainUnload, handler

      AddHandler AppDomain.CurrentDomain.DomainUnload, AddressOf Create
      RemoveHandler AppDomain.CurrentDomain.DomainUnload, AddressOf Create
    End Sub

  End Class

  Public Class MyCollection
    Inherits ObservableCollection(Of Integer)

    Public Sub Create() Handles MyBase.CollectionChanged

    End Sub

  End Class

End Namespace