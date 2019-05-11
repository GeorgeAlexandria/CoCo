' Check the same alias name as the last level namespace for the non global imported namespaces
Imports Design = System.ComponentModel.Design
Imports Serialization = System.ComponentModel.Design.Serialization

' Check the alias for the first level namespace
Imports Sys = System

Public Class AliasNamespace

  Public Sub Create()
    Dim obj = New Design.ServiceContainer()
    Dim obj2 = New Serialization.ContextStack()
    Dim obj3 = New Design.Serialization.ContextStack()
    Dim [error] = New Sys.ArgumentOutOfRangeException()
    Sys.ArgumentOutOfRangeException.ReferenceEquals("", "")
  End Sub

End Class