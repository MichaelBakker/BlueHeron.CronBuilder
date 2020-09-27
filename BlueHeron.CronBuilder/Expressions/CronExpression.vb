
''' <summary>
''' Object, that represents a Cron expression.
''' </summary>
Public NotInheritable Class CronExpression

#Region " Objects and variables "

	Private mExpression As String

#End Region

#Region " Properties "

	''' <summary>
	''' The collection of <see cref="CronParameter"/> objects that are part of the expression.
	''' </summary>
	''' <returns>A <see cref="Dictionary(Of ParameterType, CronParameter)"/></returns>
	Public ReadOnly Property Parameters As Dictionary(Of ParameterType, CronParameter)

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Returns the first date and time after the given date when the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the future must be found</param>
	''' <returns>A date</returns>
	Public Function FirstOccurrenceAfter(datum As Date) As Date

		Return GetDateMatch(datum, False)

	End Function

	''' <summary>
	''' Returns the first date and time before the given date when the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the past must be found</param>
	''' <returns>A date</returns>
	Public Function LastOccurrenceBefore(datum As Date) As Date

		Return GetDateMatch(datum, False)

	End Function

	''' <summary>
	''' Returns the string representation of this expression.
	''' </summary>
	Public Overrides Function ToString() As String

		If String.IsNullOrEmpty(mExpression) Then
			mExpression = String.Format(fmtExpression, Parameters(ParameterType.Minute).ToString, Parameters(ParameterType.Hour).ToString, Parameters(ParameterType.Day).ToString, Parameters(ParameterType.Month).ToString, Parameters(ParameterType.WeekDay).ToString)
		End If
		Return mExpression

	End Function

#End Region

#Region " Private methods and functions "

	''' <summary>
	''' Returns the first date and time before or after the given date when the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the past or future must be found</param>
	''' <param name="goBack">If True, the closest match in the past is returned, else the closest match in the future</param>
	''' <returns>A date</returns>
	Private Function GetDateMatch(datum As Date, goBack As Boolean) As Date

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronExpression, defaulting to a '* * * * *' expression.
	''' </summary>
	Friend Sub New()

		Parameters = New Dictionary(Of ParameterType, CronParameter) From {
		{ParameterType.Minute, New CronAnyParameter(ParameterType.Minute)},
		{ParameterType.Hour, New CronAnyParameter(ParameterType.Hour)},
		{ParameterType.Day, New CronAnyParameter(ParameterType.Day)},
		{ParameterType.Month, New CronAnyParameter(ParameterType.Month)},
		{ParameterType.WeekDay, New CronAnyParameter(ParameterType.WeekDay)}
	}

	End Sub

#End Region

End Class