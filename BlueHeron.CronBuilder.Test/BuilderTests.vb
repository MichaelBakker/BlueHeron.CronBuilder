Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BlueHeron.Cron.Localization

''' <summary>
''' Test methods.
''' </summary>
<TestClass>
Public Class BuilderTests

#Region " Objects and variables "

	Private Const _ANY As String = "* * * * *"

	Private Shared mBuilder As Builder

#End Region

	<TestMethod>
	Sub Test01_DefaultExpression()
		Dim defaultExpression As String

		mBuilder = New Builder
		defaultExpression = mBuilder.Build.Expression

		Debug.Assert(defaultExpression = _ANY)

	End Sub

	<TestMethod>
	Sub Test02_ParseParameters()
		Dim expectedExpression As String = "23 0-20 1/2 1 *"
		Dim parameterizedExpression As Expression = mBuilder.
			WithValue(ParameterType.Minute, mBuilder.Number(23)).
			WithRange(ParameterType.Hour, mBuilder.Number(0), mBuilder.Number(20)).
			WithStep(ParameterType.Day, mBuilder.Number(1), mBuilder.Number(2)).
			WithValue(ParameterType.Month, mBuilder.Number(1)).
			WithAny(ParameterType.DayOfWeek). ' not always necessary; when using a new CronBuilder instance all parameters default to 'Any' (see Test00_Default)
			Build()

		Debug.Assert(parameterizedExpression.Expression = expectedExpression)

		expectedExpression = "0 0-23 * APR-OCT MON"  ' integer, text and enum value are supported
		parameterizedExpression = mBuilder.
			WithValue(ParameterType.Minute, mBuilder.Number(0)).
			WithRange(ParameterType.Hour, mBuilder.Number(0), mBuilder.Number(23)).
			WithAny(ParameterType.Day).
			WithRange(ParameterType.Month, mBuilder.ParseValue("APR"), mBuilder.ParseValue("OCT")).
			WithValue(ParameterType.DayOfWeek, mBuilder.DayOfWeek(DayOfWeek.MON)).
			Build() ' validation is performed automatically

		Debug.Assert(parameterizedExpression.Expression = expectedExpression)

	End Sub

	<TestMethod()>
	Sub Test03_DateMatchingSingleValue()
		Dim dtmTest As New Date(2020, 9, 29) ' is a tuesday

		Dim expression As Expression = mBuilder.
			WithValue(ParameterType.Minute, mBuilder.Number(0)).
			WithValue(ParameterType.Hour, mBuilder.Number(12)).
			WithAny(ParameterType.Day).
			WithAny(ParameterType.Month).
			WithAny(ParameterType.DayOfWeek).
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
			WithValue(ParameterType.Minute, mBuilder.Number(0)).
			WithValue(ParameterType.Hour, mBuilder.Number(12)).
			WithAny(ParameterType.Day).
			WithAny(ParameterType.Month).
			WithValue(ParameterType.DayOfWeek, mBuilder.DayOfWeek(DayOfWeek.MON)).
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
			WithValue(ParameterType.Minute, mBuilder.Number(0)).
			WithValue(ParameterType.Hour, mBuilder.Number(12)).
			WithRange(ParameterType.Day, mBuilder.Number(1), mBuilder.Number(7)).
			WithAny(ParameterType.Month).
			WithValue(ParameterType.DayOfWeek, mBuilder.DayOfWeek(DayOfWeek.MON)).
			Build() ' every first monday of any month at noon
		Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
		Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am 
		Dim matchedAfter As Date = expression.Next(dateToMatchLater)
		Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

		Debug.Assert(matchedAfter = New Date(2020, 10, 5, 12, 0, 0)) ' first monday of month after testdate
		Debug.Assert(matchedBefore = New Date(2020, 9, 7, 12, 0, 0)) ' first monday of month before testdate

	End Sub

	<TestMethod()>
	Sub Test05_DateMatchingRangeCombinations1()
		Dim dtmTest As New Date(2020, 9, 29)
		Dim expression As Expression = mBuilder.
			WithValue(ParameterType.Minute, mBuilder.Number(0)).
			WithValue(ParameterType.Hour, mBuilder.Number(12)).
			WithRange(ParameterType.Day, mBuilder.Number(1), mBuilder.Number(7)).
			WithStep(ParameterType.Month, mBuilder.Number(2), mBuilder.Number(2)).
			WithValue(ParameterType.DayOfWeek, mBuilder.DayOfWeek(DayOfWeek.MON)).
			Build() ' every first monday of even months at noon
		Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
		Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am 
		Dim matchedAfter As Date = expression.Next(dateToMatchLater)
		Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

		Debug.Assert(matchedAfter = New Date(2020, 10, 5, 12, 0, 0)) ' first monday of first matching month after testdate
		Debug.Assert(matchedBefore = New Date(2020, 8, 3, 12, 0, 0)) ' first monday of last matching month before testdate

	End Sub

	<TestMethod()>
	Sub Test05_DateMatchingRangeCombinations2()
		Dim dtmTest As New Date(2020, 9, 29, 13, 1, 0)
		Dim expression As Expression = mBuilder.
			WithAny(ParameterType.Minute).
			WithStep(ParameterType.Hour, mBuilder.Any, mBuilder.Number(3)).
			WithAny(ParameterType.Day).
			WithAny(ParameterType.Month).
			WithAny(ParameterType.DayOfWeek).
			Build() ' at minute 0 of every third hour

		Dim matchedAfter As Date = expression.Next(dtmTest)

		Debug.Assert(matchedAfter = New Date(2020, 9, 29, 15, 0, 0)) ' 0, 3, 6, 9, 12, [15] , 18, 21

	End Sub

	<TestMethod()>
	Sub Test06_DateMatchingMonthAndYearBoundary()
		Dim dtmTest As New Date(2020, 11, 29, 23, 15, 0)
		Dim expression As Expression = mBuilder.Build("* 12 1-7 1/3 MON") ' at any minute of hour 12 on every first monday of every 3rd month starting with January

		Debug.Assert(expression.Next(dtmTest) = New Date(2021, 1, 4, 12, 0, 0))

	End Sub

	<TestMethod()>
	Sub Test07_DateMatchingList()
		Dim dtmTest As New Date(2020, 9, 29, 12, 0, 0)
		Dim expression As Expression = mBuilder.
			WithValue(ParameterType.Minute, mBuilder.Number(0)).
			WithList(ParameterType.Hour, mBuilder.Number(1), mBuilder.Number(2), mBuilder.Number(3), mBuilder.Step(mBuilder.Number(4), mBuilder.Number(2))).
			WithAny(ParameterType.Day).
			WithAny(ParameterType.Month).
			WithAny(ParameterType.DayOfWeek).
			Build() ' every 1st, 2nd, 3rd hour and then every second hour starting at hour 4 (i.e. 1, 2, 3, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22)
		Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
		Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am
		Dim matchedAfter As Date = expression.Next(dateToMatchLater)
		Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

		Debug.Assert(matchedAfter = New Date(2020, 9, 29, 14, 0, 0)) ' first match is 14:00
		Debug.Assert(matchedBefore = New Date(2020, 9, 29, 10, 0, 0)) ' last match is 10:00

	End Sub

	<TestMethod()>
	Sub Test08_ExpressionParsing()
		Dim expectedExpression As String = "23 0-20 1/2 1 *"
		Dim expression As Expression = mBuilder.Build(expectedExpression)

		Debug.Assert(expression.ToString = expectedExpression)

	End Sub

	<TestMethod()>
	Sub Test09_Polling()
		Dim expression As Expression = mBuilder.Build(_ANY)

		Debug.Assert(expression.Poll = True)

	End Sub

	<TestMethod()>
	Sub Test10_PollRange()
		Dim expression As Expression = mBuilder.WithValue(ParameterType.Minute, mBuilder.Number(0)).WithValue(ParameterType.Hour, mBuilder.Number(12)).WithRange(ParameterType.Day, mBuilder.Number(1), mBuilder.Number(7)).WithValue(ParameterType.DayOfWeek, mBuilder.DayOfWeek(DayOfWeek.MON)).WithAny(ParameterType.Month).Build() 'every first monday of the month at noon
		Dim matches12 As IEnumerable(Of Date) = expression.Next(New Date(2020, 10, 29, 13, 0, 0), 12) ' next 12 matches, starting at the given date and time
		Dim matchesDate As IEnumerable(Of Date) = expression.Next(New Date(2020, 10, 29, 13, 0, 0), New Date(2021, 10, 4, 12, 0, 0)) ' all matches within the date range starting at the given date and time ending at the given date and time
		Dim expected As Date() = {
			New Date(2020, 11, 2, 12, 0, 0),
			New Date(2020, 12, 7, 12, 0, 0),
			New Date(2021, 1, 4, 12, 0, 0),
			New Date(2021, 2, 1, 12, 0, 0),
			New Date(2021, 3, 1, 12, 0, 0),
			New Date(2021, 4, 5, 12, 0, 0),
			New Date(2021, 5, 3, 12, 0, 0),
			New Date(2021, 6, 7, 12, 0, 0),
			New Date(2021, 7, 5, 12, 0, 0),
			New Date(2021, 8, 2, 12, 0, 0),
			New Date(2021, 9, 6, 12, 0, 0),
			New Date(2021, 10, 4, 12, 0, 0)
			}

		Debug.Assert(matches12.Count = 12)
		For i As Integer = 0 To 11
			Debug.Assert(matches12(i) = expected(i))
		Next
		Debug.Assert(matchesDate.Count = 12)
		For i As Integer = 0 To 11
			Debug.Assert(matchesDate(i) = expected(i))
		Next

	End Sub

	<TestMethod()>
	Sub Test11_ValidationErrorOnCreation()
		Dim misHapsAsText As String() = {"", "Q", "5.7", "MUN", "-1"} ' value errors
		Dim misHapsAsValue As String() = {"1/2-3", "1#2-3"} ' type errors 

		For i As Integer = 0 To misHapsAsText.Length - 1
			mBuilder = mBuilder.With(ParameterType.Minute, misHapsAsText(i))

			Debug.Assert(mBuilder.Parameters(0).IsFault = True)
		Next

		For i As Integer = 0 To misHapsAsValue.Length - 1
			mBuilder = mBuilder.With(ParameterType.Minute, misHapsAsValue(i))

			Debug.Assert(mBuilder.Parameters(0).IsFault = True)
		Next

	End Sub

	<TestMethod()>
	Sub Test12_ValidationErrorOnBuild()
		Dim wrongMinutes As IEnumerable(Of String) = {"TUE", "MAR", "60"} ' valid minute values are numbers between 0 and 59
		Dim wrongHours As IEnumerable(Of String) = {"MON", "JAN", "24"} ' valid hour values are numbers between 0 and 23
		Dim wrongDays As IEnumerable(Of String) = {"0", "WED", "32"} ' valid day values are numbers between 1 and 31
		Dim wrongMonths As IEnumerable(Of String) = {"0", "WED", "13", "1#3"} ' valid month values are numbers between 1 and 12 or a MonthOfYear value
		Dim wrongDaysOfWeek As IEnumerable(Of String) = {"0", "7", "APR", "1#8", "12#1"} ' valid days of week values are numbers between 0 and 6, a DayOfWeek value, a hash (<DayOfWeek>#<1-6>)
		Dim count As Integer = 0
		Dim errorCount As Integer = 0

		For Each value As String In wrongMinutes
			count += 1
			Try
				Dim expr As Expression = mBuilder.With(ParameterType.Minute, value).Build
			Catch ex As AggregateException
				errorCount += 1
			End Try

			Debug.Assert(errorCount = count)
		Next

		count = 0
		errorCount = 0

		For Each value As String In wrongHours
			count += 1
			Try
				Dim expr As Expression = mBuilder.With(ParameterType.Hour, value).Build
			Catch ex As AggregateException
				errorCount += 1
			End Try

			Debug.Assert(errorCount = count)
		Next

		count = 0
		errorCount = 0

		For Each value As String In wrongDays
			count += 1
			Try
				Dim expr As Expression = mBuilder.With(ParameterType.Day, value).Build
			Catch ex As AggregateException
				errorCount += 1
			End Try

			Debug.Assert(errorCount = count)
		Next

		count = 0
		errorCount = 0

		For Each value As String In wrongMonths
			count += 1
			Try
				Dim expr As Expression = mBuilder.With(ParameterType.Month, value).Build
			Catch ex As AggregateException
				errorCount += 1
			End Try

			Debug.Assert(errorCount = count)
		Next

		count = 0
		errorCount = 0

		For Each value As String In wrongDaysOfWeek
			count += 1
			Try
				Dim expr As Expression = mBuilder.With(ParameterType.DayOfWeek, value).Build
			Catch ex As AggregateException
				errorCount += 1
			End Try

			Debug.Assert(errorCount = count)
		Next

		count = 0
		errorCount = 0

		Try
			Dim expr As Expression = mBuilder.
				With(ParameterType.Minute, "62").
				With(ParameterType.Hour, "25").
				With(ParameterType.Day, "33").
				With(ParameterType.Month, "0").
				With(ParameterType.DayOfWeek, "7").
				Build ' one expression with 5 wrong parameters -> 5 exceptions in AggregateException
		Catch ex As AggregateException
			count = 1
			errorCount = ex.InnerExceptions.Count
		End Try

		Debug.Assert(count = 1 AndAlso errorCount = 5)

	End Sub

	<TestMethod()> Sub Test13_ValidateExpression()

		Debug.Assert(mBuilder.Validate(" 1 2 3 4 Q") = False)
		Debug.Assert(mBuilder.Validate("30 12 1 1 *") = True)

	End Sub

	<TestMethod> Sub Test14_Humanizing()


		Dim e As Expression = mBuilder.Build("30 0 * * MON-SAT")
		Dim strHum As String = e.Display


		Dim humanized As String
		Dim expected As String() = {
			String.Format("{0} {1} {2}", Resources.atMinute, Resources.every, Resources.minute),
			String.Format("", 1, 2, 3, 4, 5, 6, 7, 8)
		}
		Dim expressions As IEnumerable(Of Expression) = {
			mBuilder.
				WithAny(ParameterType.Minute).
				WithAny(ParameterType.Hour).
				WithAny(ParameterType.Day).
				WithAny(ParameterType.Month).
				WithAny(ParameterType.DayOfWeek).
				Build(), ' every minute
			mBuilder.
				WithValue(ParameterType.Minute, mBuilder.Number(0)).
				WithStep(ParameterType.Hour, mBuilder.Number(0), mBuilder.Number(6)).
				WithRange(ParameterType.Day, mBuilder.Number(1), mBuilder.Number(7)).
				WithList(ParameterType.Month, mBuilder.Number(3), mBuilder.Number(6), mBuilder.Number(9), mBuilder.Number(12)).
				WithValue(ParameterType.DayOfWeek, mBuilder.DayOfWeek(DayOfWeek.MON)).
				Build()
		}

		For i As Integer = 0 To expected.Count - 2
			humanized = expressions(i).Display

			Debug.Assert(humanized = expected(i))
		Next




	End Sub

	<TestMethod> Sub Test15_Operators()
		Dim exprA As Expression = mBuilder.Build("30 12 1 1 *")
		Dim exprB As Expression = mBuilder.Build("0 12 1 1 MON")

		Debug.Assert(Not exprA.Equals(exprB))

		Debug.Assert(exprA <> exprB)

		Debug.Assert(mBuilder.Number(1).Equals(1) = False)

		Debug.Assert(mBuilder.Number(1).Equals("1") = True)

		Debug.Assert(mBuilder.Number(1) <> mBuilder.DayOfWeek(DayOfWeek.MON))

		Debug.Assert(mBuilder.DayOfWeek(DayOfWeek.MON).Equals("MON"))

		Debug.Assert(mBuilder.Any.Equals("*"))

	End Sub

	<TestMethod>
	Sub Test16_MatchHashSymbol()

		mBuilder.Use(New BuildOptions(True)) ' support symbols

		Dim expression As Expression = mBuilder.Build("0 12 * * 1#1") ' noon of every 1st monday of every month
		Dim results As IEnumerable(Of Date) = expression.Next(New Date(2020, 8, 1, 0, 0, 0), 6) ' get noon times of the next 6 first mondays of the month starting at saturday, august 1, 2020
		Dim expectedDates As Date() = {
			New Date(2020, 8, 3, 12, 0, 0),
			New Date(2020, 9, 7, 12, 0, 0),
			New Date(2020, 10, 5, 12, 0, 0),
			New Date(2020, 11, 2, 12, 0, 0),
			New Date(2020, 12, 7, 12, 0, 0),
			New Date(2021, 1, 4, 12, 0, 0)
		}

		For i As Integer = 0 To results.Count - 1
			Debug.Assert(results(i) = expectedDates(i))
		Next

	End Sub

End Class