
''' <summary>
''' Parameter that consists of a start value and an increment.
''' </summary>
Public NotInheritable Class CronStepParameter
	Implements ICronParameter

#Region " Objects and variables "

	Private mValues As List(Of Integer)

#End Region

#Region " Properties "

	''' <summary>
	''' The assigned increment value.
	''' </summary>
	Public ReadOnly Property Increment As Object

	''' <summary>
	''' The <see cref="ParameterType"/> of this parameter
	''' </summary>
	Public ReadOnly Property ParameterType As ParameterType Implements ICronParameter.ParameterType

	''' <summary>
	''' The assigned start value.
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
			Dim value As Integer = ToInteger(value)
			Dim increment As Integer = ToInteger(increment)
			Dim maxVal As Integer = MaximumValues(ParameterType)

			mValues = New List(Of Integer)
			For i As Integer = value To maxVal Step increment
				mValues.Add(i)
			Next
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String Implements ICronParameter.ToString

		Return String.Format(fmtStep, Value, Increment)

	End Function

	''' <inheritdoc cref="ICronParameter.Validate()" />
	Public Function Validate() As Boolean Implements ICronParameter.Validate

		If Not (ParameterType.Validate(ValueType, Value) AndAlso ParameterType.Validate(ValueType, Increment)) Then
			Return False
		End If

		Return True

	End Function

	''' <inheritdoc cref="ICronParameter.Validate(ByRef String)" />
	Public Function Validate(ByRef errorMessage As String) As Boolean Implements ICronParameter.Validate
		Dim blValid As Boolean = True

		If Not ParameterType.Validate(ValueType, Value) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(My.Resources.errParameter, Value)
		End If
		If Not ParameterType.Validate(ValueType, Increment) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(My.Resources.errParameter, Increment)
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

		ParameterType = paramType
		Me.ValueType = valueType
		Value = startValue
		Me.Increment = increment

	End Sub

#End Region

End Class