
''' <summary>
''' Object that uses a fluent pattern to easily generate Cron expressions.
''' </summary>
Public NotInheritable Class CronBuilder

#Region " Properties "

	''' <summary>
	''' The collection of <see cref="CronParameter"/> objects that are part of the expression.
	''' </summary>
	''' <returns>A <see cref="Dictionary(Of ParameterType, CronParameter)"/></returns>
	Public ReadOnly Property Parameters As Dictionary(Of ParameterType, CronParameter)

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Assembles the Cron expression and returns it as a string.
	''' </summary>
	''' <param name="validate">If true, validation is performed and an error thrown upon failure</param>
	''' <param name="throwErrorMessages">Include detailed error messages when throwing an error</param>
	''' <exception cref="ArgumentException">The expression contains one or more invalid parameters</exception>
	''' <returns>A Cron expression</returns>
	Public Function Build(Optional validate As Boolean = True, Optional throwErrorMessages As Boolean = True) As String

		If validate Then
			If throwErrorMessages Then
				Dim messages As String = String.Empty

				Parameters.Values.ToList.ForEach(Sub(p)
													 Dim msg As String = String.Empty

													 If Not p.Validate(msg) Then
														 messages &= vbCrLf & msg
													 End If
												 End Sub)
				If Not String.IsNullOrEmpty(messages) Then
					Throw New ArgumentException(String.Format(errParameterWithMessage, GetExpression, messages))
				End If
			Else
				Parameters.Values.ToList.ForEach(Sub(p)
													 If Not p.Validate Then
														 Throw New ArgumentException(String.Format(errParameter, p))
													 End If
												 End Sub)
			End If
		End If

		Return GetExpression()

	End Function

	''' <summary>
	''' Creates a <see cref="CronBuilder" /> from the given expression.
	''' </summary>
	''' <param name="cronExpression">The string representation of a Cron expression</param>
	''' <param name="validate">If true, validation is performed and an error thrown upon failure</param>
	''' <param name="throwErrorMessages">Include detailed error messages when throwing an error</param>
	''' <exception cref="ArgumentException">The expression contains one or more invalid parameters</exception>
	''' <returns>A configured <see cref="CronBuilder"/></returns>
	Public Shared Function FromString(cronExpression As String, Optional validate As Boolean = True, Optional throwErrorMessages As Boolean = True) As CronBuilder
		Dim builder As New CronBuilder
		Dim parts As String() = cronExpression.Split(Space, StringSplitOptions.RemoveEmptyEntries)

		If parts.Count <> 5 Then
			Throw New ArgumentException(errParameterCount)
		End If

		' ...

		Return builder

	End Function

	''' <summary>
	''' Modifies part of the expression using the given parameters.
	''' </summary>
	''' <param name="parameterType">The <paramref name="parameterType">expression part</paramref> to modify</param>
	''' <param name="parameterValueType">The <paramref name="parameterValueType">type of the value(s) to apply</paramref></param>
	''' <param name="values">Variable length array of values. Valid argument count depends on the <paramref name="parameterValueType"/> value</param>
	''' <returns>The modified <see cref="CronBuilder"/>. Call <see cref="CronBuilder.Build(Boolean, Boolean)"/> to obtain the modified expression</returns>
	Public Function [With](parameterType As ParameterType, parameterValueType As ParameterValueType, ParamArray values As Object()) As CronBuilder
		Dim parameter As CronParameter
		Dim valueCount As Integer = If(values Is Nothing, 0, values.Count)
		Dim intValueType As Integer = CInt(parameterValueType)

		If Not ValidIntegerCombinations.Contains(intValueType) Then
			Throw New ArgumentException(errParameterValueType)
		End If
		If parameterType = ParameterType.Month Then
			If Not ValidMonthCombinations.Contains(intValueType) Then
				Throw New ArgumentException(errParameterValueType)
			End If
		ElseIf parameterType = ParameterType.WeekDay Then
			If Not ValidWeekCombinations.Contains(intValueType) Then
				Throw New ArgumentException(errParameterValueType)
			End If
		End If

		If (parameterValueType And ParameterValueType.List) = ParameterValueType.List Then
			If valueCount < 2 Then
				Throw New ArgumentException(errArgumentCount)
			End If
			parameter = New CronListParameter(parameterType, parameterValueType, values)
		ElseIf (parameterValueType And ParameterValueType.Range) = ParameterValueType.Range Then
			If valueCount <> 2 Then
				Throw New ArgumentException(errArgumentCount)
			End If
			parameter = New CronRangeParameter(parameterType, parameterValueType, values(0), values(1))
		ElseIf (parameterValueType And ParameterValueType.Step) = ParameterValueType.Step Then
			If valueCount <> 2 Then
				Throw New ArgumentException(errArgumentCount)
			End If
			parameter = New CronStepParameter(parameterType, parameterValueType, values(0), values(1))
		ElseIf (parameterValueType And ParameterValueType.Value) = ParameterValueType.Value Then ' 1 value needed
			If valueCount <> 1 Then
				Throw New ArgumentException(errArgumentCount)
			End If
			parameter = New CronValueParameter(parameterType, parameterValueType, values(0))
		Else ' 0: ParameterValueType.Any
			parameter = New CronAnyParameter(parameterType)
		End If

		Parameters(parameterType) = parameter
		Return Me

	End Function

#End Region

#Region " Private methods and functions "

	''' <summary>
	''' Returns the assembled expression.
	''' </summary>
	''' <returns>A Cron expression.</returns>
	Private Function GetExpression() As String

		Return String.Format(fmtExpression, Parameters(ParameterType.Minute).ToString, Parameters(ParameterType.Hour).ToString, Parameters(ParameterType.Day).ToString, Parameters(ParameterType.Month).ToString, Parameters(ParameterType.WeekDay).ToString)

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronBuilder, defaulting to a '* * * * *' expression.
	''' </summary>
	Public Sub New()

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