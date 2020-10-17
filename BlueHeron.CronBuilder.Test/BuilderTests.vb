Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BlueHeron.Cron.Localization

''' <summary>
''' Tests can be double-checked at: https://crontab.guru .
''' </summary>
<TestClass>
Public Class BuilderTests

#Region " Objects and variables "

	Private Const _ANY As String = "* * * * *"

	Private Shared mBuilder As Builder

#End Region

	<TestMethod>
	Sub Test00_Default()
		Dim defaultExpression As String

		mBuilder = New Builder
		defaultExpression = mBuilder.Build.Expression

		Debug.Assert(defaultExpression = _ANY)

	End Sub

	<TestMethod>
	Sub Test01_ValueParameters()
		Dim expectedExpression As String = "23 0-20 1/2 1 *"
		Dim parameterizedExpression As Expression = mBuilder.
			WithValue(ParameterType.Minute, ParameterValue.Number(23)).
			WithRange(ParameterType.Hour, ParameterValue.Number(0), ParameterValue.Number(20)).
			WithStep(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(2)).
			WithValue(ParameterType.Month, ParameterValue.Number(1)).
			WithAny(ParameterType.WeekDay). ' not always necessary; when using a new CronBuilder instance all parameters default to 'Any' (see Test00_Default)
			Build()

		Debug.Assert(parameterizedExpression.Expression = expectedExpression)

	End Sub

	<TestMethod>
	Sub Test02_MonthAndDayOfWeekParameters()
		Dim expectedExpression As String = "0 0-23 * APR-OCT MON"  ' integer, text and enum value are supported
		Dim parameterizedExpression As Expression = mBuilder.
			WithValue(ParameterType.Minute, ParameterValue.Number(0)).
			WithRange(ParameterType.Hour, ParameterValue.Number(0), ParameterValue.Number(23)).
			WithAny(ParameterType.Day).
			WithRange(ParameterType.Month, ParameterValue.FromString("APR"), ParameterValue.FromString("OCT")).
			WithValue(ParameterType.WeekDay, ParameterValue.DayOfWeek(DayOfWeek.MON)).
			Build() ' validation is performed by default

		Debug.Assert(parameterizedExpression.Expression = expectedExpression)

	End Sub

	<TestMethod()>
	Sub Test03_DateMatchingSingleValue()
		Dim dtmTest As New Date(2020, 9, 29) ' is a tuesday

		Dim expression As Expression = mBuilder.
			WithValue(ParameterType.Minute, ParameterValue.Number(0)).
			WithValue(ParameterType.Hour, ParameterValue.Number(12)).
			WithAny(ParameterType.Day).
			WithAny(ParameterType.Month).
			WithAny(ParameterType.WeekDay).
			Build() ' every day at noon
		Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
		Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am 
		Dim matchedAfter As Date = expression.Next(dateToMatchLater)
		Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

		Debug.Assert(matchedAfter = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day + 1, 12, 0, 0))
		Debug.Assert(matchedBefore = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day - 1, 12, 0, 0))

		matchedAfter = expression.Next(dateToMatchEarlier)
		matchedBefore = expression.Previous(dateToMatchLater)

		Debug.Assert(matchedAfter = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0))
		Debug.Assert(matchedBefore = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0))

		Dim expression2 As Expression = mBuilder.
			WithValue(ParameterType.Minute, ParameterValue.Number(0)).
			WithValue(ParameterType.Hour, ParameterValue.Number(12)).
			WithAny(ParameterType.Day).
			WithAny(ParameterType.Month).
			WithValue(ParameterType.WeekDay, ParameterValue.DayOfWeek(DayOfWeek.MON)).
			Build() ' every monday at noon

		matchedAfter = expression2.Next(dateToMatchLater)
		matchedBefore = expression2.Previous(dateToMatchEarlier)

		Debug.Assert(matchedAfter = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0).AddDays(6)) '  first monday after test date
		Debug.Assert(matchedBefore = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0).AddDays(-1)) ' last monday before testdate

	End Sub

	<TestMethod()>
	Sub Test04_DateMatchingWeekOfDayValue()
		Dim dtmTest As New Date(2020, 9, 29)
		Dim expression As Expression = mBuilder.
			WithValue(ParameterType.Minute, ParameterValue.Number(0)).
			WithValue(ParameterType.Hour, ParameterValue.Number(12)).
			WithRange(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(7)).
			WithAny(ParameterType.Month).
			WithValue(ParameterType.WeekDay, ParameterValue.DayOfWeek(DayOfWeek.MON)).
			Build() ' every first monday of any month at noon
		Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
		Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am 
		Dim matchedAfter As Date = expression.Next(dateToMatchLater)
		Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

		Debug.Assert(matchedAfter = New Date(2020, 10, 5, 12, 0, 0)) ' first monday of month after testdate
		Debug.Assert(matchedBefore = New Date(2020, 9, 7, 12, 0, 0)) ' first monday of month before testdate

	End Sub

	<TestMethod()>
	Sub Test05_DateMatchingRangeCombinations()
		Dim dtmTest As New Date(2020, 9, 29)
		Dim expression As Expression = mBuilder.
			WithValue(ParameterType.Minute, ParameterValue.Number(0)).
			WithValue(ParameterType.Hour, ParameterValue.Number(12)).
			WithRange(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(7)).
			WithStep(ParameterType.Month, ParameterValue.Number(2), ParameterValue.Number(2)).
			WithValue(ParameterType.WeekDay, ParameterValue.DayOfWeek(DayOfWeek.MON)).
			Build() ' every first monday of even months at noon
		Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
		Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am 
		Dim matchedAfter As Date = expression.Next(dateToMatchLater)
		Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

		Debug.Assert(matchedAfter = New Date(2020, 10, 5, 12, 0, 0)) ' first monday of first matching month after testdate
		Debug.Assert(matchedBefore = New Date(2020, 8, 3, 12, 0, 0)) ' first monday of last matching month before testdate

	End Sub

	<TestMethod()>
	Sub Test06_ExpressionParsing()
		Dim expectedExpression As String = "23 0-20 1/2 1 *"
		Dim cronExpression As Expression = mBuilder.Build(expectedExpression)

		Debug.Assert(cronExpression.ToString = expectedExpression)

	End Sub

	<TestMethod()>
	Sub Test07_DateMatchingList()
		Dim dtmTest As New Date(2020, 9, 29, 12, 0, 0)
		Dim expression As Expression = mBuilder.
			WithValue(ParameterType.Minute, ParameterValue.Number(0)).
			WithList(ParameterType.Hour, ParameterValue.Number(1), ParameterValue.Number(2), ParameterValue.Number(3), ParameterValue.Step(ParameterValue.Number(4), ParameterValue.Number(2))).
			WithAny(ParameterType.Day).
			WithAny(ParameterType.Month).
			WithAny(ParameterType.WeekDay).
			Build() ' every 1st, 2nd, 3rd hour and then every second hour starting at hour 4 (i.e. 1, 2, 3, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22)
		Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
		Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am
		Dim matchedAfter As Date = expression.Next(dateToMatchLater)
		Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

		Debug.Assert(matchedAfter = New Date(2020, 9, 29, 14, 0, 0)) ' first match is 14:00
		Debug.Assert(matchedBefore = New Date(2020, 9, 29, 10, 0, 0)) ' last match is 10:00

	End Sub

	<TestMethod()>
	Sub Test08_Polling()
		Dim expression As Expression = mBuilder.Build(_ANY)

		Debug.Assert(expression.Poll = True)

	End Sub

	<TestMethod()>
	Sub Test09_ValidateOnCreation()
		Dim exception As ParserException = Nothing
		Dim misHapsAsText As IEnumerable(Of String) = {"Q"}

		For Each value As String In misHapsAsText
			Try
				Dim expr As Expression = mBuilder.With(ParameterType.Minute, value).Build
			Catch ex As ParserException
				exception = ex
			End Try

			Debug.Assert(Not exception Is Nothing AndAlso exception.Message = String.Format(Resources.errParameter, value))
			exception = Nothing
		Next

	End Sub

	<TestMethod()>
	Sub Test10_ValidateOnBuild()
		Dim misHapsDayOfWeekAndMonthOfYear As IEnumerable(Of String) = {"MON", "JAN"}
		Dim count As Integer = 0
		Dim errorCount As Integer = 0

		For Each value As String In misHapsDayOfWeekAndMonthOfYear
			count += 1
			Try
				Dim expr As Expression = mBuilder.With(ParameterType.Hour, value).Build
			Catch ex As ParserAggregateException
				errorCount += 1
			End Try

			Debug.Assert(errorCount = count)
		Next

	End Sub

	<TestMethod> Sub Test11_Humanizing()
		Dim humanized As String
		Dim expected As String() = {
			String.Format("{0} {1} {2}", Resources.atMinute, Resources.every, Resources.minute),
			""
		}
		Dim expressions As IEnumerable(Of Expression) = {
			mBuilder.
				WithAny(ParameterType.Minute).
				WithAny(ParameterType.Hour).
				WithAny(ParameterType.Day).
				WithAny(ParameterType.Month).
				WithAny(ParameterType.WeekDay).
				Build(), ' every minute
			mBuilder.
				WithValue(ParameterType.Minute, ParameterValue.Number(0)).
				WithValue(ParameterType.Hour, ParameterValue.Number(12)).
				WithRange(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(7)).
				WithStep(ParameterType.Month, ParameterValue.Number(2), ParameterValue.Number(2)).
				WithValue(ParameterType.WeekDay, ParameterValue.DayOfWeek(DayOfWeek.MON)).
				Build() ' every first monday of even months at noon
		}

		For i As Integer = 0 To expected.Count - 1
			humanized = expressions(i).Display

			Debug.Assert(humanized = expected(i))
		Next

	End Sub

End Class