
''' <summary>
''' Parameter that consists of a start value and an increment.
''' </summary>
Public NotInheritable Class CronStepParameter
	Inherits CronParameterBase

#Region " Public methods and functions "

	''' <inheritdoc cref="ICronParameter.AsEnumerable()" />
	Public Overrides Function AsEnumerable() As IEnumerable(Of Integer)

		If mValues Is Nothing Then
			Dim incr As Integer = Value.Values(1).Value
			Dim maxVal As Integer = MaximumValues(ParameterType)
			Dim minVal As Integer = MinimumValues(ParameterType)
			Dim val As Integer = Math.Min(maxVal, Math.Max(Value.Values(0).Value, minVal))

			mValues = New List(Of Integer)
			If val <= (maxVal - incr) Then
				For i As Integer = val To maxVal Step incr
					mValues.Add(i)
				Next
			End If
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Format(fmtStep, Value.Values(0), Value.Values(1))

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronStepParameter.
	''' </summary>
	''' <param name="startValue">The assigned start value</param>
	''' <param name="increment">The assigned increment value</param>
	Friend Sub New(paramType As ParameterType, startValue As ParameterValue, increment As ParameterValue)

		MyBase.New(paramType, ParameterValue.Step(startValue, increment))

	End Sub

#End Region

End Class