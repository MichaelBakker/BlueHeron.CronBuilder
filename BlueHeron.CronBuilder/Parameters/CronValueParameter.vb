
''' <summary>
''' Parameter that consists of a single value.
''' </summary>
Public NotInheritable Class CronValueParameter
	Implements ICronParameter

#Region " Objects and variables "

	Private mValues As List(Of Integer)

#End Region

#Region " Properties "

	''' <summary>
	''' The <see cref="ParameterType"/> of this parameter
	''' </summary>
	Public ReadOnly Property ParameterType As ParameterType Implements ICronParameter.ParameterType

	''' <summary>
	''' The assigned value.
	''' </summary>
	Public ReadOnly Property Value As Object

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType Implements ICronParameter.ValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="ICronParameter.ToList()" />
	Public Function ToList() As List(Of Integer) Implements ICronParameter.ToList

		If mValues Is Nothing Then
			mValues = New List(Of Integer) From {ToInteger(Value)}
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String Implements ICronParameter.ToString

		Return Value.ToString

	End Function

	''' <inheritdoc cref="ICronParameter.Validate()" />
	Public Function Validate() As Boolean Implements ICronParameter.Validate

		If Not ParameterType.Validate(ValueType, Value) Then
			Return False
		End If

		Return True

	End Function

	''' <inheritdoc cref="ICronParameter.Validate(ByRef String)" />
	Public Function Validate(ByRef errorMessage As String) As Boolean Implements ICronParameter.Validate
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

		ParameterType = paramType
		Me.ValueType = valueType
		Me.Value = value

	End Sub

#End Region

End Class