
''' <summary>
''' Parameter that consists of a range of values.
''' </summary>
Public NotInheritable Class CronRangeParameter
	Implements ICronParameter

#Region " Objects and variables "

	Private mValues As List(Of Integer)
	Private mValid As Boolean?

#End Region

#Region " Properties "

	''' <summary>
	''' The assigned start value.
	''' </summary>
	Public ReadOnly Property From As Object

	''' <inheritdoc cref="ICronParameter.IsValid" />
	Public ReadOnly Property IsValid As Boolean? Implements ICronParameter.IsValid
		Get
			Return mValid
		End Get
	End Property

	''' <summary>
	''' The <see cref="ParameterType"/> of this parameter.
	''' </summary>
	Public ReadOnly Property ParameterType As ParameterType Implements ICronParameter.ParameterType

	''' <summary>
	''' The assigned end value.
	''' </summary>
	Public ReadOnly Property [To] As Object

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType Implements ICronParameter.ValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="ICronParameter.ToList()" />
	Public Function ToList() As List(Of Integer) Implements ICronParameter.ToList

		If mValid AndAlso mValues Is Nothing Then
			Dim fromVal As Integer = ToInteger(From).Value
			Dim toVal As Integer = ToInteger([To]).Value

			mValues = New List(Of Integer)
			For i As Integer = fromVal To toVal
				mValues.Add(i)
			Next
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String Implements ICronParameter.ToString

		Return String.Format(fmtRange, From, [To])

	End Function

	''' <inheritdoc cref="ICronParameter.Validate()" />
	Public Function Validate() As Boolean Implements ICronParameter.Validate

		mValid = True
		If Not (ParameterType.Validate(ValueType, From) AndAlso ParameterType.Validate(ValueType, [To])) Then
			mValid = False
		End If

		Return mValid.Value

	End Function

	''' <inheritdoc cref="ICronParameter.Validate(ByRef String)" />
	Public Function Validate(ByRef errorMessage As String) As Boolean Implements ICronParameter.Validate

		mValid = True
		If Not ParameterType.Validate(ValueType, From) Then
			mValid = False
			errorMessage = vbCrLf & String.Format(My.Resources.errParameter, From)
		End If
		If Not ParameterType.Validate(ValueType, [To]) Then
			mValid = False
			errorMessage = vbCrLf & String.Format(My.Resources.errParameter, [To])
		End If

		Return mValid.Value

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

		ParameterType = paramType
		Me.ValueType = valueType
		From = fromValue
		[To] = toValue

	End Sub

#End Region

End Class