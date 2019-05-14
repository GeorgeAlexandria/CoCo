Namespace Identifiers

  Public Class Example

    Public Class Range

      Public ReadOnly Property Start As Integer

      Public ReadOnly Property Length As Integer

      Public ReadOnly Property [End] As Integer
        Get
          Return Start + Length()
        End Get
      End Property

      Public Sub New(start%, length%)
        Me.Start = start
        Me.Length = length
      End Sub

      ''' <remarks>
      ''' If <paramref name="other"/> start position greater then current end => return current instance
      ''' </remarks>
      Public Function TryUnionWith(other As Range) As Range
        Return If(other.Start > [End], Me, New Range(Start, other.End - Start))
      End Function

    End Class

    Public Class Text

      Private ReadOnly _ranges As List(Of Range)
      Private ReadOnly _underlyingText As String

      Private Sub New(text As String, ranges As IEnumerable(Of Range))
        _underlyingText = text
        _ranges = New List(Of Range)(ranges)
      End Sub

      Public Sub New(text$)
        _underlyingText = text
        _ranges = New List(Of Range) From {New Range(0, text.Length)}
      End Sub

      Public Function DivideBy(position As Integer) As Text
        Dim list = New List(Of Range)
        Dim textWasDivided As Boolean
        For Each range In _ranges
          If range.Start < position And position < range.End Then
            list.Add(New Range(range.Start, position - range.Start))
            list.Add(New Range(position, range.End - position))
            textWasDivided = True
          Else
            list.Add(range)
          End If
        Next

        Return If(textWasDivided, New Text(_underlyingText, list), Me)
      End Function

      Public ReadOnly Property RangesCount As Integer
        Get
          Return _ranges.Count
        End Get
      End Property

    End Class

    Public Function CreateRangedText(inputText As String) As Text

      Dim text As Text
      Dim initialStep = 1
      While text Is Nothing OrElse (text.RangesCount < 26 And initialStep < 20)
        text = New Text(inputText)
        For index As Integer = 0 To inputText.Length Step initialStep + index
          text = text.DivideBy(index)
        Next
        initialStep += 1
      End While

      Return If(initialStep < 20, text, New Text(inputText))

    End Function

  End Class

End Namespace