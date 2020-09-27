
''' <summary>
''' Parameter that consists of a start value and an increment.
''' </summary>
Public NotInheritable Class CronStepParameter
	Inherits CronParameter

#Region " Objects and variables "

	Private ReadOnly mStartValue As Object

	Private ReadOnly mIncrement As Object

#End Region

#Region " Properties "

	''' <summary>
	''' The assigned start value.
	''' </summary>
	Public ReadOnly Property Value As Object

	''' <summary>
	''' The assigned increment value.
	''' </summary>
	Public ReadOnly Property Increment As Object

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Format(fmtStep, mStartValue, mIncrement)

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		If Not (ParameterType.Validate(ValueType, mStartValue) AndAlso ParameterType.Validate(ValueType, mIncrement)) Then
			Return False
		End If

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean
		Dim blValid As Boolean = True

		If Not ParameterType.Validate(ValueType, mStartValue) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(My.Resources.errParameter, mStartValue)
		End If
		If Not ParameterType.Validate(ValueType, mIncrement) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(My.Resources.errParameter, mIncrement)
		End If

		Return blValid

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronStepParameter that should contain two values of the given <see cref="ParameterValueType"/>.
	''' </summary>
	''' <param name="valueType">The expected <see cref="ParameterValueType"/></param>
	''' <param name="startValue">The assigned start value</param>
	''' <param name="increment">The assigned start value</param>
	Friend Sub New(paramType As ParameterType, valueType As ParameterValueType, startValue As Object, increment As Object)

		MyBase.New(paramType)
		Me.ValueType = valueType
		mStartValue = startValue
		mIncrement = increment

	End Sub

#End Region

End Class