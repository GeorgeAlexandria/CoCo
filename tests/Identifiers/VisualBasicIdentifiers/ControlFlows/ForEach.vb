Public Class ForEach

  Sub Create(arg%)

    For Each elem% In {5, 2, 8}
    Next

    For Each elem% In {5, 2, 8}
      Continue For
    Next

    For Each elem% In {5, 2, 8}
      Exit For
    Next

  End Sub

End Class
