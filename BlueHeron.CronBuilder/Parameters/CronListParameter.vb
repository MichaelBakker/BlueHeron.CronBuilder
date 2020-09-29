
''' <summary>
''' Parameter that consists of a comma-separated list of values.
''' </summary>
Public NotInheritable Class CronListParameter
	Implements ICronParameter

#Region " Objects and variables "

	Private mValues As List(Of Integer)

#End Region

#Region " Properties "

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

		If mValues Is Nothing Then
			mValues = New List(Of Integer)
			Values.ForEach(Sub(v) mValues.Add(ToInteger(v)))
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String Implements ICronParameter.ToString

		Return String.Join(Comma, Values)

	End Function

	''' <inheritdoc cref="ICronParameter.Validate()" />
	Public Function Validate() As Boolean Implements ICronParameter.Validate

		For Each it As Object In Values
			If Not ParameterType.Validate(ValueType, it) Then
				Return False
			End If
		Next

		Return True

	End Function

	''' <inheritdoc cref="ICronParameter.Validate(ByRef String)" />
	Public Function Validate(ByRef errorMessage As String) As Boolean Implements ICronParameter.Validate
		Dim messages As String = String.Empty
		Dim blValid As Boolean = True

		For Each it As Object In Values
			If Not ParameterType.Validate(ValueType, it) Then
				blValid = False
				messages &= vbCrLf & String.Format(My.Resources.errParameter, it)
			End If
		Next
		errorMessage = messages

		Return blValid

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