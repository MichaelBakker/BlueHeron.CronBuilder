
''' <summary>
''' Parameter that consists of a comma-separated list of values.
''' </summary>
Public NotInheritable Class CronListParameter
	Implements ICronParameter

#Region " Objects and variables "

	Private mValues As List(Of Integer)
	Private mValid As Boolean?

#End Region

#Region " Properties "

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
	''' List of assigned values.
	''' </summary>
	Public ReadOnly Property Values As List(Of Object)

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType Implements ICronParameter.ValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="ICronParameter.ToList()" />
	Public Function ToList() As List(Of Integer) Implements ICronParameter.ToList

		If mValid AndAlso mValues Is Nothing Then
			mValues = New List(Of Integer)
			Values.ForEach(Sub(v) mValues.Add(ToInteger(v).Value))
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String Implements ICronParameter.ToString

		Return String.Join(Comma, Values)

	End Function

	''' <inheritdoc cref="ICronParameter.Validate()" />
	Public Function Validate() As Boolean Implements ICronParameter.Validate

		mValid = True
		For Each it As Object In Values
			If Not ParameterType.Validate(ValueType, it) Then
				mValid = False
				Exit For
			End If
		Next

		Return mValid.Value

	End Function

	''' <inheritdoc cref="ICronParameter.Validate(ByRef String)" />
	Public Function Validate(ByRef errorMessage As String) As Boolean Implements ICronParameter.Validate
		Dim messages As String = String.Empty

		mValid = True
		For Each it As Object In Values
			If Not ParameterType.Validate(ValueType, it) Then
				mValid = False
				messages &= vbCrLf & String.Format(My.Resources.errParameter, it)
			End If
		Next
		errorMessage = messages

		Return mValid.Value

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronListParameter that should contain values of the given <see cref="ParameterValueType"/>.
	''' </summary>
	''' <param name="valueType">The expected <see cref="ParameterValueType"/></param>
	''' <param name="values">The assigned values</param>
	Friend Sub New(paramType As ParameterType, valueType As ParameterValueType, values As Object())

		ParameterType = paramType
		Me.ValueType = valueType
		Me.Values = New List(Of Object)(values)

	End Sub

#End Region

End Class