
''' <summary>
''' Parameter that consists of a single value.
''' </summary>
Public NotInheritable Class CronValueParameter
	Inherits CronParameter

#Region " Objects and variables "

	Private ReadOnly mValue As Object

#End Region

#Region " Properties "

	''' <summary>
	''' The assigned value.
	''' </summary>
	Public ReadOnly Property Value As Object

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return Value.ToString

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		If Not ParameterType.Validate(ValueType, Value) Then
			Return False
		End If

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean
		Dim blValid As Boolean = True

		If Not ParameterType.Validate(ValueType, Value) Then
			blValid = False
			errorMessage = String.Format(My.Resources.errParameter, Value)
		End If

		Return blValid

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronValueParameter that should contain a value of the given <see cref="ParameterValueType"/>.
	''' </summary>
	''' <param name="valueType">The expected <see cref="ParameterValueType"/></param>
	''' <param name="value">The assigned value</param>
	Friend Sub New(paramType As ParameterType, valueType As ParameterValueType, value As Object)

		MyBase.New(paramType)
		Me.ValueType = valueType
		mValue = value

	End Sub

#End Region

End Class