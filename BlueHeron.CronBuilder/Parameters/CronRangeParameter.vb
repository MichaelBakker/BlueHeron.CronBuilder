
''' <summary>
''' Parameter that consists of a range of values.
''' </summary>
Public NotInheritable Class CronRangeParameter
	Inherits CronParameter

#Region " Properties "

	''' <summary>
	''' The assigned start value.
	''' </summary>
	Public ReadOnly Property From As Object

	''' <summary>
	''' The assigned end value.
	''' </summary>
	Public ReadOnly Property [To] As Object

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Format(fmtRange, From, [To])

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		If Not (ParameterType.Validate(ValueType, From) AndAlso ParameterType.Validate(ValueType, [To])) Then
			Return False
		End If

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean
		Dim blValid As Boolean = True

		If Not ParameterType.Validate(ValueType, From) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(My.Resources.errParameter, From)
		End If
		If Not ParameterType.Validate(ValueType, [To]) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(My.Resources.errParameter, [To])
		End If

		Return blValid

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronRangeParameter that should contain two values of the given <see cref="ParameterValueType"/>.
	''' </summary>
	''' <param name="valueType">The expected <see cref="ParameterValueType"/></param>
	''' <param name="fromValue">The assigned start value</param>
	''' <param name="toValue">The assigned start value</param>
	Friend Sub New(paramType As ParameterType, valueType As ParameterValueType, fromValue As Object, toValue As Object)

		MyBase.New(paramType)
		Me.ValueType = valueType
		From = fromValue
		[To] = toValue

	End Sub

#End Region

End Class