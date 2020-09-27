
''' <summary>
''' Base class for Cron parameters.
''' </summary>
Public MustInherit Class CronParameter

	''' <summary>
	''' The <see cref="ParameterType" /> of this parameter.
	''' </summary>
	Public ReadOnly Property ParameterType As ParameterType

	''' <summary>
	''' Returns the parameter expression.
	''' </summary>
	Public MustOverride Overrides Function ToString() As String

	''' <summary>
	''' Validates this parameter.
	''' </summary>
	''' <returns>True, if supplied values are valid</returns>
	Public MustOverride Function Validate() As Boolean

	''' <summary>
	''' Validates this parameter.
	''' </summary>
	''' <param name="errorMessage">Will hold an error message if validation fails</param>
	''' <returns>True, if supplied values are valid</returns>
	Public MustOverride Function Validate(ByRef errorMessage As String) As Boolean

	''' <summary>
	''' Creates a new CronParameter.
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/></param>
	Protected Sub New(paramType As ParameterType)

		ParameterType = paramType

	End Sub

End Class