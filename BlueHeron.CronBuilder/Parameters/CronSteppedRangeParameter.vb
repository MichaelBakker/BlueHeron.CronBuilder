
''' <summary>
''' Parameter that consists of a range of values and an increment value.
''' E.g.: 1-6/2 -> 1, 2, 3, 4, 5, 6, 8, 10, 12, 14, ...
''' </summary>
Public NotInheritable Class CronSteppedRangeParameter
	Inherits CronParameterBase

#Region " Public methods and functions "

	''' <inheritdoc cref="ICronParameter.AsEnumerable()" />
	Public Overrides Function AsEnumerable() As IEnumerable(Of Integer)

		If mValues Is Nothing Then
			Dim fromVal As Integer = Value.Values(0).Value
			Dim toVal As Integer = Value.Values(1).Value
			Dim incr As Integer = Value.Values(2).Value
			Dim maxVal As Integer = MaximumValues(ParameterType)
			Dim minVal As Integer = MinimumValues(ParameterType)

			mValues = New List(Of Integer)
			For i As Integer = Math.Max(minVal, fromVal) To Math.Min(maxVal, toVal)
				mValues.Add(i)
			Next
			If toVal <= (maxVal - incr) Then
				For i As Integer = toVal To maxVal Step incr
					mValues.Add(i)
				Next
			End If
		End If

		Return mValues.Distinct

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Format(fmtSteppedRange, Value.Values(0), Value.Values(1), Value.Values(2))

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronSteppedRangeParameter.
	''' </summary>
	''' <param name="fromValue">The assigned start value</param>
	''' <param name="toValue">The assigned end value</param>
	''' <param name="incrementValue">The assigned increment value</param>
	Friend Sub New(paramType As ParameterType, fromValue As ParameterValue, toValue As ParameterValue, incrementValue As ParameterValue)

		MyBase.New(paramType, ParameterValue.SteppedRange(fromValue, toValue, incrementValue))

	End Sub

#End Region

End Class