Imports Namespaces = VisualBasicIdentifiers.Access.Namespaces

Namespace Access.Namespaces

  Public Class CustomNamespace

    Public Sub Create()
      ' Try to access by FQN and less, do not remove them
      Dim value = New VisualBasicIdentifiers.Access.Namespaces.CustomNamespace()
      value = New Access.Namespaces.CustomNamespace()
      ' "Namespace" in this case is not alias
      value = New Namespaces.CustomNamespace()
    End Sub

  End Class

End Namespace