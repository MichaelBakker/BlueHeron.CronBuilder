Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace BlueHeron.Cron.Test

	''' <summary>
	''' Tests can be double-checked at: https://crontab.guru
	''' </summary>
	<TestClass>
	Public Class BuilderTests

		<TestMethod>
		Sub TestDefault()
			Dim builder As New CronBuilder
			Dim defaultExpression As String = builder.Build.ToString

			Debug.Assert(defaultExpression = "* * * * *")

		End Sub

		<TestMethod>
		Sub TestParameters()
			Dim expectedExpression As String = "23 0-20 1/2 1 *"
			Dim builder As New CronBuilder
			Dim parameterizedExpression As CronExpression = builder.
				With(ParameterType.Minute, ParameterValueType.Value, 23).
				With(ParameterType.Hour, ParameterValueType.Range Or ParameterValueType.Value, 0, 20).
				With(ParameterType.Day, ParameterValueType.Step Or ParameterValueType.Value, 1, 2).
				With(ParameterType.Month, ParameterValueType.Value, 1).
				With(ParameterType.WeekDay, ParameterValueType.Any).
				Build() ' validation is performed by default

			Debug.Assert(parameterizedExpression.ToString = expectedExpression)

			parameterizedExpression = builder.Build(True, False) ' no messages
			Debug.Assert(parameterizedExpression.ToString = expectedExpression)

			parameterizedExpression = builder.Build(False, False) ' no validation
			Debug.Assert(parameterizedExpression.ToString = expectedExpression)


		End Sub

	End Class

End Namespace