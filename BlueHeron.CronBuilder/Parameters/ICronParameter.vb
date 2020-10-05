
''' <summary>
''' Interface definition for Cron parameters.
''' </summary>
Public Interface ICronParameter

#Region " Properties "

	''' <summary>
	''' Returns a value, determining whether this parameter has passed validation.
	''' </summary>
	''' <returns>True, if the parameter passed validation, False if it failed, and Null / Nothing if the parameter has not been validated</returns>
	ReadOnly Property IsValid As Boolean?

	''' <summary>
	''' The <see cref="ParameterType" /> of this parameter.
	''' </summary>
	ReadOnly Property ParameterType As ParameterType

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Methods and functions "

	''' <summary>
	''' Returns all integer values that match this parameter.
	''' </summary>
	''' <returns>A <see cref="List(Of Integer)"/></returns>
	Function ToList() As List(Of Integer)

	''' <summary>
	''' Returns the parameter expression.
	''' </summary>
	Function ToString() As String

	''' <summary>
	''' Validates this parameter.
	''' </summary>
	''' <returns>True, if supplied values are valid</returns>
	Function Validate() As Boolean

	''' <summary>
	''' Validates this parameter.
	''' </summary>
	''' <param name="errorMessage">Will hold an error message if validation fails</param>
	''' <returns>True, if supplied values are valid</returns>
	Function Validate(ByRef errorMessage As String) As Boolean

#End Region

End Interface