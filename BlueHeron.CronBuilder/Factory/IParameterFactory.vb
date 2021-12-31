
''' <summary>
''' Interface definition for objects that create <see cref="Parameter"/> and <see cref="ParameterValue"/> objects.
''' </summary>
Public Interface IParameterFactory

	''' <summary>
	''' The <see cref="BuildOptions"/> that is used.
	''' If not set through <see cref="Builder.Use(BuildOptions)"/>, then <see cref="BuildOptions.Default"/> is used.
	''' </summary>
	''' <returns>A <see cref="BuildOptions"/> object</returns>
	Property BuildOptions As BuildOptions

	''' <summary>
	''' Creates a new <see cref="Parameter"/>.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/></param>
	''' <param name="valueType">The <see cref="ValueType"/></param>
	''' <param name="values">Array of one or more <see cref="ParameterValue"/> objects</param>
	''' <returns>A <see cref="Parameter"/></returns>
	Function CreateParameter(parameterType As ParameterType, valueType As ValueType, values As IEnumerable(Of ParameterValue)) As Parameter

	''' <summary>
	''' The value 'Any' (i.e. '*').
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/> of <see cref="ValueType.Any"/></returns>
	Overloads Function CreateParameterValue() As ParameterValue

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of a <see cref="Cron.ValueType"/> that is determined when parsing the value.
	''' If the value cannot be parsed, <see cref="Cron.ValueType.Unknown"/> is assigned.
	''' </summary>
	''' <param name="value">The value</param>
	Overloads Function CreateParameterValue(value As String) As ParameterValue

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Number"/>.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/> of <see cref="ValueType.Number"/></returns>
	Overloads Function CreateParameterValue(value As Integer) As ParameterValue

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.DayOfWeek"/>.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/> of <see cref="ValueType.DayOfWeek"/></returns>
	Overloads Function CreateParameterValue(value As DayOfWeek) As ParameterValue

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.MonthOfYear"/>.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/> of <see cref="ValueType.MonthOfYear"/></returns>
	Overloads Function CreateParameterValue(value As MonthOfYear) As ParameterValue

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>, <see cref="Cron.ValueType.Step"/>, <see cref="Cron.ValueType.SteppedRange"/> or <see cref="Cron.ValueType.List"/>.
	''' </summary>
	''' <param name="values">The parameter values</param>
	''' <param name="valueType">The <see cref="Cron.ValueType"/></param>
	''' <returns>A <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>, <see cref="Cron.ValueType.Step"/>, <see cref="Cron.ValueType.SteppedRange"/> or <see cref="Cron.ValueType.List"/></returns>
	Overloads Function CreateParameterValue(values As IEnumerable(Of ParameterValue), valueType As ValueType) As ParameterValue

End Interface