' Check the same alias name as the last level namespace
Imports Collections = System.Collections
Imports Generic = System.Collections.Generic

' Check the alias for the first level namespace
Imports Sys = System

Public Class AliasNamespace

  Public Sub Create()
    Dim list = New Generic.List(Of Integer)()
    Dim list2 = New Collections.Generic.List(Of Integer)()
    Dim [error] = New Sys.ArgumentOutOfRangeException()
    Sys.ArgumentOutOfRangeException.ReferenceEquals("", "")
  End Sub

End Class