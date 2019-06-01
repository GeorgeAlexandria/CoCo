Public Class Iterator

  Iterator Function GetValues() As IEnumerable(Of Integer)
    Yield 5
    Yield 7
    Yield 9
  End Function
End Class
