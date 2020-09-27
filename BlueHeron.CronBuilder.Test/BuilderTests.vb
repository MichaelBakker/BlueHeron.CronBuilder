Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace BlueHeron.Cron.Test

	<TestClass>
	Public Class BuilderTests

		<TestMethod>
		Sub TestDefault()
			Dim builder As New CronBuilder
			Dim defaultExpression As String = builder.Build

			Debug.Assert(defaultExpression = "* * * * *")

		End Sub



	End Class

End Namespace