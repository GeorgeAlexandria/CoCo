Public Class RangeVariable

  Public Sub Create()

    Dim arg = From item In {5, 6}
              Let value = item * 10
              Select value * 2

  End Sub

End Class