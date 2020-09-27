
''' <summary>
''' Parameter that consists of a comma-separated list of values.
''' </summary>
Public NotInheritable Class CronListParameter
	Inherits CronParameter

#Region " Objects and variables "

	Private ReadOnly mList As List(Of Object)

#End Region

#Region " Properties "

	''' <summary>
	''' List of assigned values.
	''' </summary>
	Public ReadOnly Property Values As List(Of Object)

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Join(Comma, mList)

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		For Each it As Object In mList
			If Not ParameterType.Validate(ValueType, it) Then
				Return False
			End If
		Next

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean
		Dim messages As String = String.Empty
		Dim blValid As Boolean = True

		For Each it As Object In mList
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

		MyBase.New(paramType)
		Me.ValueType = valueType
		mList = New List(Of Object)(values)

	End Sub

#End Region

End Class