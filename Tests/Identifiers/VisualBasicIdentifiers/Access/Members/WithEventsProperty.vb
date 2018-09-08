Namespace Access.Locals

  Public Class WithEventsProperty

    Private WithEvents watcher As IO.FileSystemWatcher

    Public Sub Create() Handles watcher.Changed

    End Sub

  End Class

End Namespace