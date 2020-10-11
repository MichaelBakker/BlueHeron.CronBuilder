
''' <summary>
''' Parameter that consists of a single value.
''' </summary>
Public NotInheritable Class CronValueParameter
	Inherits CronParameterBase

#Region " Public methods and functions "

	''' <inheritdoc cref="ICronParameter.AsEnumerable()" />
	<DebuggerStepThrough()>
	Public Overrides Function AsEnumerable() As IEnumerable(Of Integer)

		If mValues Is Nothing Then
			Dim maxVal As Integer = MaximumValues(ParameterType)
			Dim minVal As Integer = MinimumValues(ParameterType)

			mValues = New List(Of Integer) From {Math.Min(maxVal, Math.Max(Value.Value, minVal))}
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	<DebuggerStepThrough()>
	Public Overrides Function ToString() As String

		Return Value.ToString

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronValueParameter.
	''' </summary>
	''' <param name="value">The assigned value</param>
	<DebuggerStepThrough()>
	Friend Sub New(paramType As ParameterType, value As ParameterValue)

		MyBase.New(paramType, value)

	End Sub

#End Region

End Class