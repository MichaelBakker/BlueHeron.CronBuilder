
''' <summary>
''' Object that uses a fluent pattern to easily generate Cron expressions.
''' </summary>
Public NotInheritable Class CronBuilder

#Region " Objects and variables "

	Private ReadOnly mExpression As CronExpression

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Returns the <see cref="CronExpression"/>, optionally validating it.
	''' </summary>
	''' <exception cref="ArgumentException">The expression contains one or more invalid parameters</exception>
	''' <returns>A <see cref="CronExpression"/></returns>
	Public Function Build() As CronExpression
		Dim messages As String = String.Empty

		mExpression.Expression = String.Empty ' force recreation (necessary when the same CronBuilder is used multiple times)
		mExpression.Parameters.Values.ToList.ForEach(Sub(p)
														 Dim msg As String = String.Empty

														 If Not p.Validate(msg) Then
															 messages &= vbCrLf & msg
														 End If
													 End Sub)

		If Not String.IsNullOrEmpty(messages) Then
			Throw New ArgumentException(String.Format(My.Resources.errParameterWithMessage, mExpression, messages))
		End If

		Return mExpression

	End Function

	''' <summary>
	''' Creates a <see cref="CronExpression" /> from the given string and returns it after validation.
	''' </summary>
	''' <param name="cronExpression">The string representation of a Cron expression</param>
	''' <exception cref="ArgumentException">The expression contains one or more invalid parameters</exception>
	''' <returns>A <see cref="CronExpression"/></returns>
	Public Function Build(cronExpression As String) As CronExpression
		Dim parts As String() = cronExpression.Split({Space}, StringSplitOptions.RemoveEmptyEntries)

		If parts.Count <> 5 Then
			Throw New ArgumentException(My.Resources.errParameterCount)
		End If

		For i As Integer = 0 To 4
			Dim paramType As ParameterType = CType(i, ParameterType)
			Dim part As String = parts(i)

			If part = Asterix Then
				mExpression.Parameters(paramType) = New CronAnyParameter(paramType)
			Else
				Dim intValue As Integer

				If Integer.TryParse(part, intValue) Then
					mExpression.Parameters(paramType) = New CronValueParameter(paramType, ParameterValueType.Value, intValue)
				Else
					If paramType = ParameterType.Month Then ' check for MonthOfYear value

					ElseIf paramType = ParameterType.WeekDay Then ' check for DayOfWeek value

					Else ' list, range or step

					End If
				End If
			End If
		Next

		Return Build()

	End Function

	''' <summary>
	''' Modifies part of the expression using the given parameters.
	''' </summary>
	''' <param name="parameterType">The <paramref name="parameterType">expression part</paramref> to modify</param>
	''' <param name="parameterValueType">The <paramref name="parameterValueType">type of the value(s) to apply</paramref></param>
	''' <param name="values">Variable length array of values. Valid argument count depends on the <paramref name="parameterValueType"/> value</param>
	''' <returns>The modified <see cref="CronBuilder"/>. Call <see cref="CronBuilder.Build()"/> to obtain the modified expression</returns>
	Public Function [With](parameterType As ParameterType, parameterValueType As ParameterValueType, ParamArray values As Object()) As CronBuilder
		Dim parameter As ICronParameter
		Dim valueCount As Integer = If(values Is Nothing, 0, values.Count)
		Dim intValueType As Integer

		If parameterValueType = ParameterValueType.List OrElse parameterValueType = ParameterValueType.Range OrElse parameterValueType = ParameterValueType.Step Then
			parameterValueType = parameterValueType Or ParameterValueType.Value ' Accept step, range and list without specification and assume its a ParameterValue.Value type
		End If
		intValueType = CInt(parameterValueType)

		If Not ValidIntegerCombinations.Contains(intValueType) Then
			If parameterType = ParameterType.Month Then
				If Not ValidMonthCombinations.Contains(intValueType) Then
					Throw New ArgumentException(My.Resources.errParameterValueType)
				End If
			ElseIf parameterType = ParameterType.WeekDay Then
				If Not ValidWeekCombinations.Contains(intValueType) Then
					Throw New ArgumentException(My.Resources.errParameterValueType)
				End If
			Else
				Throw New ArgumentException(My.Resources.errParameterValueType)
			End If
		End If

		If (parameterValueType And ParameterValueType.List) = ParameterValueType.List Then
			If valueCount < 2 Then
				Throw New ArgumentException(My.Resources.errArgumentCount)
			End If
			parameter = New CronListParameter(parameterType, parameterValueType, values)
		ElseIf (parameterValueType And ParameterValueType.Range) = ParameterValueType.Range Then
			If valueCount <> 2 Then
				Throw New ArgumentException(My.Resources.errArgumentCount)
			End If
			parameter = New CronRangeParameter(parameterType, parameterValueType, values(0), values(1))
		ElseIf (parameterValueType And ParameterValueType.Step) = ParameterValueType.Step Then
			If valueCount <> 2 Then
				Throw New ArgumentException(My.Resources.errArgumentCount)
			End If
			parameter = New CronStepParameter(parameterType, parameterValueType, values(0), values(1))
		ElseIf parameterValueType.IsSingleValueType Then ' 1 value needed
			If valueCount <> 1 Then
				Throw New ArgumentException(My.Resources.errArgumentCount)
			End If
			parameter = New CronValueParameter(parameterType, parameterValueType, values(0))
		Else ' 0: ParameterValueType.Any
			parameter = New CronAnyParameter(parameterType)
		End If

		mExpression.Parameters(parameterType) = parameter

		Return Me

	End Function

#End Region

#Region " Private methods and functions "



#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronBuilder, defaulting to a '* * * * *' expression.
	''' </summary>
	Public Sub New()

		mExpression = New CronExpression

	End Sub

#End Region

End Class