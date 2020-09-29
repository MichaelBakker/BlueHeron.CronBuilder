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
			defaultExpression = mBuilder.Build.ToString

			Debug.Assert(defaultExpression = "* * * * *")

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
				Build() ' validation is performed by default

			Debug.Assert(parameterizedExpression.ToString = expectedExpression)

			parameterizedExpression = mBuilder.Build(True, False) ' no messages
			Debug.Assert(parameterizedExpression.ToString = expectedExpression)

			parameterizedExpression = mBuilder.Build(False, False) ' no validation
			Debug.Assert(parameterizedExpression.ToString = expectedExpression)


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

			Debug.Assert(parameterizedExpression.ToString = expectedExpression)

			parameterizedExpression = mBuilder.Build(True, False) ' no messages
			Debug.Assert(parameterizedExpression.ToString = expectedExpression)

			parameterizedExpression = mBuilder.Build(False, False) ' no validation
			Debug.Assert(parameterizedExpression.ToString = expectedExpression)

		End Sub

		<TestMethod()>
		Public Sub Test3_DateMatchingSingleValue()
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

			Dim matchedAfter As Date = expression.FirstOccurrenceAfter(dateToMatchLater)
			Dim matchedBefore As Date = expression.LastOccurrenceBefore(dateToMatchEarlier)

			Debug.Assert(matchedAfter = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day + 1, 12, 0, 0))
			Debug.Assert(matchedBefore = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day - 1, 12, 0, 0))

			matchedAfter = expression.FirstOccurrenceAfter(dateToMatchEarlier)
			matchedBefore = expression.LastOccurrenceBefore(dateToMatchLater)

			Debug.Assert(matchedAfter = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0))
			Debug.Assert(matchedBefore = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0))

			Dim expression2 As CronExpression = mBuilder.
				With(ParameterType.Minute, ParameterValueType.Value, 0).
				With(ParameterType.Hour, ParameterValueType.Value, 12).
				With(ParameterType.Day, ParameterValueType.Any).
				With(ParameterType.Month, ParameterValueType.Any).
				With(ParameterType.WeekDay, ParameterValueType.DayOfweek, DayOfWeek.MON).
				Build() ' every monday at noon

			matchedAfter = expression2.FirstOccurrenceAfter(dateToMatchLater)
			matchedBefore = expression2.LastOccurrenceBefore(dateToMatchEarlier)

			Debug.Assert(matchedAfter = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0).AddDays(6)) '  first monday after test date
			Debug.Assert(matchedBefore = New Date(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0).AddDays(-1)) ' last monday before testdate

		End Sub

	End Class

End Namespace