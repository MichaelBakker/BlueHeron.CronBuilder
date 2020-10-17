Imports System.Text
Imports BlueHeron.Cron.Localization

''' <summary>
''' Object that converts an <see cref="Expression"/> into a human-readable, localized representation, using a pattern that probably only works for a few european languages.
''' </summary>
Public Class DefaultHumanizer
	Implements IHumanizer

#Region " Public methods and functions "

	''' <inheritdoc cref="IHumanizer.Humanize(Expression)"/>
	Public Overridable Function Humanize(expression As Expression) As String Implements IHumanizer.Humanize
		Dim parameterDisplays(4) As String
		Dim prepositions(4) As String
		Dim sb As New StringBuilder(1024)
		Dim isAny As Boolean

		With expression
			For i As Integer = 0 To 4
				Dim p As Parameter = .Parameters(i)

				parameterDisplays(i) = Display(p.Value, p.ParameterType)
				prepositions(i) = p.ParameterType.Preposition
			Next

			sb.AppendFormat(fmtSpaceRight, prepositions(0))
			If .Parameters(0).Value.ValueType = ValueType.Number AndAlso .Parameters(1).Value.ValueType = ValueType.Number Then
				sb.AppendFormat(fmtTime, New Date(1, 1, 1, .Parameters(0).Value.Value, .Parameters(1).Value.Value, 0))
				prepositions(2) = Resources._of
			Else
				If .Parameters(0).Value.ValueType = ValueType.Any OrElse .Parameters(0).Value.ValueType = ValueType.Range OrElse .Parameters(0).Value.ValueType = ValueType.SteppedRange Then
					sb.AppendFormat(fmtSpaceRight, Resources.every)
				End If

				isAny = .Parameters(0).Value.ValueType = ValueType.Any
				sb.AppendFormat(fmtSpaceRight, parameterDisplays(0))

				If Not (.Parameters(1).Value.ValueType = ValueType.Any AndAlso isAny) Then
					sb.AppendFormat(fmtSpaceRight, String.Format(fmtTuple, prepositions(1), parameterDisplays(1)))
				End If
				isAny = .Parameters(1).Value.ValueType = ValueType.Any
			End If
			For i As Integer = 2 To 4
				If Not (isAny AndAlso .Parameters(i).Value.ValueType = ValueType.Any) Then
					sb.AppendFormat(fmtSpaceRight, prepositions(i))
					If .Parameters(i).Value.ValueType = ValueType.Any OrElse .Parameters(i).Value.ValueType = ValueType.Range OrElse .Parameters(i).Value.ValueType = ValueType.SteppedRange Then
						sb.AppendFormat(fmtSpaceRight, Resources.every)
					End If
					sb.AppendFormat(fmtSpaceRight, parameterDisplays(i))
				End If
				isAny = .Parameters(i).Value.ValueType = ValueType.Any
			Next
		End With

		Return sb.ToString.TrimEnd

	End Function

#End Region

#Region " Private methods and functions "

	''' <summary>
	''' Returns a human-readable, localized representation of the given parameter value for the given <see cref="ParameterType"/>.
	''' </summary>
	''' <param name="paramValue">The <see cref="ParameterValue"/></param>
	''' <param name="paramType">The <see cref="ParameterType"/> to which this value belongs</param>
	Protected Overridable Function Display(paramValue As ParameterValue, paramType As ParameterType) As String
		Dim rst As String = String.Empty

		With paramValue
			Select Case .ValueType
				Case ValueType.Any
					rst = paramType.ToDisplay(False)
				Case ValueType.DayOfWeek
					rst = CType(.Value, DayOfWeek).ToDisplay
				Case ValueType.List
					For Each v As ParameterValue In .Values
						rst &= String.Format(fmtTuple, rst, DisplayValue(v, paramType, ValueDisplayType.Postfix))
					Next
				Case ValueType.MonthOfYear
					rst = CType(.Value, MonthOfYear).ToDisplay
				Case ValueType.Number
					rst = DisplayValue(paramValue, paramType, ValueDisplayType.Postfix)
				Case ValueType.Range
					rst = String.Format(fmtTriple, DisplayValue(.Values(0), paramType, ValueDisplayType.Postfix), Resources.through, DisplayValue(.Values(1), paramType, ValueDisplayType.ValueOnly))
				Case ValueType.Step
					rst = String.Format(fmtTriple, DisplayValue(.Values(1), paramType, ValueDisplayType.Prefix), Resources.startingWith, DisplayValue(.Values(0), paramType, ValueDisplayType.ValueOnly))
				Case ValueType.SteppedRange
					rst = String.Format(fmtTriple, DisplayValue(.Values(0), paramType, ValueDisplayType.Postfix), Resources.through, DisplayValue(.Values(1), paramType, ValueDisplayType.ValueOnly)) &
						String.Format(fmtSpaceBoth, Resources.andThen) & String.Format(fmtTriple, DisplayValue(.Values(2), paramType, ValueDisplayType.Prefix), Resources.startingWith, DisplayValue(.Values(1), paramType, ValueDisplayType.ValueOnly))
				Case Else ' ParameterValueType.Unknown
					rst = Unknown
			End Select
		End With

		Return rst

	End Function

	''' <summary>
	''' Returns a human-readable, localized representation of the given parameter value's <see cref="ParameterValue.Value"/> for the given <see cref="ParameterType"/> and <see cref="ValueDisplayType"/>.
	''' </summary>
	''' <param name="paramValue">The <see cref="ParameterValue"/></param>
	''' <param name="paramType">The <see cref="ParameterType"/> to which this value belongs</param>
	''' <param name="valueDisplayType">The <see cref="ValueDisplayType"/> to use</param>
	Protected Overridable Function DisplayValue(paramValue As ParameterValue, paramType As ParameterType, valueDisplayType As ValueDisplayType) As String
		Dim rst As String = String.Empty

		With paramValue
			Select Case valueDisplayType
				Case ValueDisplayType.ValueOnly
					If paramType = ParameterType.WeekDay Or .ValueType = ValueType.DayOfWeek Then
						rst = CType(.Value, DayOfWeek).ToDisplay
					ElseIf paramType = ParameterType.Month Or .ValueType = ValueType.MonthOfYear Then
						rst = CType(.Value, MonthOfYear).ToDisplay
					Else
						rst = If(.Value >= 0, CStr(.Value), Unknown)
					End If
				Case ValueDisplayType.Prefix
					rst = If(.Value > 1, String.Format(fmtTuple, .Value, paramType.ToDisplay(True)), paramType.ToDisplay(False))
				Case ValueDisplayType.Postfix
					rst = String.Format(fmtTuple, paramType.ToDisplay(False), .Value)
			End Select
		End With

		Return rst

	End Function

#End Region

End Class