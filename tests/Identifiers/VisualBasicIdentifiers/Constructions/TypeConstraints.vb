Public Class TypeConstraints1(Of T As IEnumerable(Of IEnumerable(Of Integer)))
End Class

Public Class TypeConstraints2(Of T As {IEnumerable(Of IEnumerable(Of Integer)), IDisposable})
End Class

Public Class TypeConstraints3(Of T As System.Exception)
End Class

Public Class TypeConstraints4(Of T As {System.Exception})
End Class

Public Class TypeConstraints5(Of T As Exception)
End Class