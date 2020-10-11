
''' <summary>
''' Parameter that consists of a comma-separated list of values.
''' </summary>
Public NotInheritable Class CronListParameter
	Inherits CronParameterBase

#Region " Public methods and functions "

	''' <inheritdoc cref="ICronParameter.AsEnumerable()" />
	Public Overrides Function AsEnumerable() As IEnumerable(Of Integer)

		If mValues Is Nothing Then
			mValues = New List(Of Integer)
			For Each v As ParameterValue In Value.Values
				If v.ValueType.IsSingleValueType Then
					mValues.Add(v.Value)
				Else
					If v.ValueType = ValueType.Range Then
						mValues.AddRange(New CronRangeParameter(ParameterType, v.Values(0), v.Values(1)).AsEnumerable)
					ElseIf v.ValueType = ValueType.Step Then
						mValues.AddRange(New CronStepParameter(ParameterType, v.Values(0), v.Values(1)).AsEnumerable)
					ElseIf v.ValueType = ValueType.SteppedRange Then
						mValues.AddRange(New CronSteppedRangeParameter(ParameterType, v.Values(0), v.Values(1), v.Values(2)).AsEnumerable)
					End If
				End If
			Next
		End If

		Return mValues.Distinct

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Join(Comma, Value.Values)

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronListParameter.
	''' </summary>
	''' <param name="values">The assigned values</param>
	Friend Sub New(paramType As ParameterType, values As IEnumerable(Of ParameterValue))

		MyBase.New(paramType, ParameterValue.List(values))

	End Sub

#End Region

End Class