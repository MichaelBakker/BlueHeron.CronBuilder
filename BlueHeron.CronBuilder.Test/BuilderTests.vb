Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace BlueHeron.Cron.Test

	''' <summary>
	''' Tests can be double-checked at: https://crontab.guru
	''' </summary>
	<TestClass>
	Public Class BuilderTests

#Region " Objects and variables "

		Private Shared mBuilder As CronBuilder

#End Region

		<TestMethod>
		Sub Test0_Default()
			Dim defaultExpression As String

			mBuilder = New CronBuilder
			defaultExpression = mBuilder.Build.Expression

			Debug.Assert(defaultExpression = "* * * * *")

		End Sub

		Sub Test0_ValidationAndMessages()

			With mBuilder

			End With

		End Sub

		<TestMethod>
		Sub Test1_ValueParameters()
			Dim expectedExpression As String = "23 0-20 1/2 1 *"
			Dim parameterizedExpression As CronExpression = mBuilder.
				With(ParameterType.Minute, ParameterValueType.Value, 23).
				With(ParameterType.Hour, ParameterValueType.Range Or ParameterValueType.Value, 0, 20).
				With(ParameterType.Day, ParameterValueType.Step Or ParameterValueType.Value, 1, 2).
				With(ParameterType.Month, ParameterValueType.Value, 1).
				With(ParameterType.WeekDay, ParameterValueType.Any).
				Build()

			Debug.Assert(parameterizedExpression.Expression = expectedExpression)

		End Sub

		<TestMethod>
		Sub Test2_MonthAndDayOfWeekParameters()
			Dim expectedExpression As String = "0 0-23 * APR-OCT MON"
			Dim parameterizedExpression As CronExpression = mBuilder.
				With(ParameterType.Minute, ParameterValueType.Value, 0).
				With(ParameterType.Hour, ParameterValueType.Range Or ParameterValueType.Value, 0, 23).
				With(ParameterType.Day, ParameterValueType.Any).
				With(ParameterType.Month, ParameterValueType.Range Or ParameterValueType.Month, "APR", "OCT").
				With(ParameterType.WeekDay, ParameterValueType.DayOfweek, DayOfWeek.MON). ' both text and enum value are supported
				Build() ' validation is performed by default

			Debug.Assert(parameterizedExpression.Expression = expectedExpression)

		End Sub

		<TestMethod()>
		Sub Test3_DateMatchingSingleValue()
			Dim dtmTest As New Date(2020, 9, 29) ' is a tuesday

			Dim expression As CronExpression = mBuilder.
				With(ParameterType.Minute, ParameterValueType.Value, 0).
				With(ParameterType.Hour, ParameterValueType.Value, 12).
				With(ParameterType.Day, ParameterValueType.Any).
				With(ParameterType.Month, ParameterValueType.Any).
				With(ParameterType.WeekDay, ParameterValueType.Any).
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

			Dim expression2 As CronExpression = mBuilder.
				With(ParameterType.Minute, ParameterValueType.Value, 0).
				With(ParameterType.Hour, ParameterValueType.Value, 12).
				With(ParameterType.Day, ParameterValueType.Any).
				With(ParameterType.Month, ParameterValueType.Any).
				With(ParameterType.WeekDay, ParameterValueType.DayOfweek, DayOfWeek.MON).
				Build() ' every monday at noon

			matchedAfter = expression2.Next(dateToMatchLater)
			matchedBefore = expression2.Previous(dateToMatchEarlier)

			Debug.Assert(matchedAfter = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0).AddDays(6)) '  first monday after test date
			Debug.Assert(matchedBefore = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0).AddDays(-1)) ' last monday before testdate

		End Sub

		<TestMethod()>
		Sub Test4_DateMatchingWeekOfDayValue()
			Dim dtmTest As New Date(2020, 9, 29)
			Dim expression As CronExpression = mBuilder.
				With(ParameterType.Minute, ParameterValueType.Value, 0).
				With(ParameterType.Hour, ParameterValueType.Value, 12).
				With(ParameterType.Day, ParameterValueType.Range, 1, 7). ' ParameterValueType.Value is assumed if ParameterValueType is not specified explicitly
				With(ParameterType.Month, ParameterValueType.Any).
				With(ParameterType.WeekDay, ParameterValueType.DayOfweek, DayOfWeek.MON).
				Build() ' every first monday of any month at noon
			Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
			Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am 
			Dim matchedAfter As Date = expression.Next(dateToMatchLater)
			Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

			Debug.Assert(matchedAfter = New Date(2020, 10, 5, 12, 0, 0)) ' first monday of month after testdate
			Debug.Assert(matchedBefore = New Date(2020, 9, 7, 12, 0, 0)) ' first monday of month before testdate

		End Sub

		<TestMethod()>
		Sub Test5_DateMatchingRangeCombinations()
			Dim dtmTest As New Date(2020, 9, 29)
			Dim expression As CronExpression = mBuilder.
				With(ParameterType.Minute, ParameterValueType.Value, 0).
				With(ParameterType.Hour, ParameterValueType.Value, 12).
				With(ParameterType.Day, ParameterValueType.Range Or ParameterValueType.Value, 1, 7).
				With(ParameterType.Month, ParameterValueType.Step Or ParameterValueType.Value, 2, 2).
				With(ParameterType.WeekDay, ParameterValueType.DayOfweek, DayOfWeek.MON).
				Build() ' every first monday of even months at noon
			Dim dateToMatchLater As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0) ' 1pm
			Dim dateToMatchEarlier As New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0) ' 11am 
			Dim matchedAfter As Date = expression.Next(dateToMatchLater)
			Dim matchedBefore As Date = expression.Previous(dateToMatchEarlier)

			Debug.Assert(matchedAfter = New Date(2020, 10, 5, 12, 0, 0)) ' first monday of first matching month after testdate
			Debug.Assert(matchedBefore = New Date(2020, 8, 3, 12, 0, 0)) ' first monday of last matching month before testdate

		End Sub

	End Class

End Namespace