
''' <summary>
''' The wildcard parameter, i.e. '*', that matches all values.
''' </summary>
Public NotInheritable Class CronAnyParameter
	Inherits CronParameter

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return Asterix

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean

		Return True

	End Function

	''' <summary>
	''' Creates a new CronAnyParameter.
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/></param>
	Friend Sub New(paramType As ParameterType)

		MyBase.New(paramType)

	End Sub

End Class