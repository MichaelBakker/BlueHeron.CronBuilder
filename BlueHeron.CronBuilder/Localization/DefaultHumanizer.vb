﻿' The MIT License (MIT)
' 
' Copyright (c) 2020 Michael Bakker
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.

Imports System.Text

Namespace Localization

	''' <summary>
	''' Object that converts an <see cref="Expression"/> into a human-readable, localized representation, using a pattern that probably only works for a few european languages.
	''' </summary>
	Public Class DefaultHumanizer
		Implements IHumanizer

#Region " Public methods and functions "

		''' <inheritdoc cref="IHumanizer.Humanize(Expression)"/>
		Public Overridable Function Humanize(expression As Expression) As String Implements IHumanizer.Humanize

			Try
				Dim parameterDisplays(4) As String
				Dim prepositions(4) As String
				Dim sb As New StringBuilder(256)
				Dim isAny As Boolean

				With expression
					For i As Integer = 0 To 4
						Dim p As Parameter = .Parameters(i)

						parameterDisplays(i) = GetDisplay(p.Value, p.ParameterType)
						prepositions(i) = GetPreposition(p.ParameterType)
					Next

					sb.AppendFormat(fmtSpaceRight, prepositions(0))
					If .Parameters(0).Value.ValueType = ValueType.Number AndAlso .Parameters(1).Value.ValueType = ValueType.Number Then
						sb.AppendFormat(fmtTime, New Date(Date.MinValue.Year, 1, 1, .Parameters(1).Value.Value, .Parameters(0).Value.Value, 0))
						prepositions(2) = Resources._of
					Else
						If Not .Parameters(0).Value.ValueType.IsSingleValueType Then
							sb.AppendFormat(fmtSpaceRight, Resources.every)
						End If

						isAny = .Parameters(0).Value.ValueType = ValueType.Any
						sb.AppendFormat(fmtSpaceRight, parameterDisplays(0))

						If Not (.Parameters(1).Value.ValueType = ValueType.Any AndAlso isAny) Then
							sb.AppendFormat(fmtSpaceRight, String.Format(fmtTriple, prepositions(1), If(Not .Parameters(1).Value.ValueType.IsSingleValueType, Resources.every, String.Empty), parameterDisplays(1)))
						End If
						isAny = .Parameters(1).Value.ValueType = ValueType.Any
					End If
					For i As Integer = 2 To 4
						If Not (isAny AndAlso .Parameters(i).Value.ValueType = ValueType.Any) Then
							sb.AppendFormat(fmtSpaceRight, prepositions(i))
							If Not .Parameters(i).Value.ValueType.IsSingleValueType Then
								sb.AppendFormat(fmtSpaceRight, Resources.every)
							End If
							sb.AppendFormat(fmtSpaceRight, parameterDisplays(i))
						End If
						isAny = .Parameters(i).Value.ValueType = ValueType.Any
					Next
				End With

				Return sb.ToString.TrimEnd
			Catch ex As Exception
			End Try

			Return Resources.errAggregateMessage

		End Function

#End Region

#Region " Private methods and functions "

		''' <summary>
		''' Returns a human-readable, localized representation of the given parameter value for the given <see cref="ParameterType"/>.
		''' </summary>
		''' <param name="paramValue">The <see cref="ParameterValue"/></param>
		''' <param name="paramType">The <see cref="ParameterType"/> to which this value belongs</param>
		Protected Overridable Function GetDisplay(paramValue As ParameterValue, paramType As ParameterType) As String
			Dim rst As String = String.Empty

			With paramValue
				Select Case .ValueType
					Case ValueType.Any
						rst = paramType.ToDisplay(False)
					Case ValueType.DayOfWeek
						rst = CType(.Value, DayOfWeek).ToDisplay
					Case ValueType.List
						Dim i As Integer

						For Each v As ParameterValue In .Values
							i += 1
							rst &= If(.Values.Count > 1 AndAlso i = .Values.Count, String.Format(fmtTupleSpacedLeft, Resources._and, GetDisplay(v, paramType, ValueDisplayType.Postfix)), String.Format(fmtSeparate, GetDisplay(v, paramType, ValueDisplayType.Postfix)))
						Next
						rst.TrimEnd({Space, Comma})
					Case ValueType.MonthOfYear
						rst = CType(.Value, MonthOfYear).ToDisplay
					Case ValueType.Number
						rst = GetDisplay(paramValue, paramType, ValueDisplayType.Postfix)
					Case ValueType.Range
						rst = String.Format(fmtTriple, GetDisplay(.Values(0), paramType, ValueDisplayType.Postfix), Resources.through, GetDisplay(.Values(1), paramType, ValueDisplayType.ValueOnly))
					Case ValueType.Step
						Dim stepVal As String = GetDisplay(.Values(1), paramType, ValueDisplayType.Prefix)

						rst = If(.Values(0).ValueType = ValueType.Any, stepVal, String.Format(fmtTriple, stepVal, Resources.startingWith, GetDisplay(.Values(0), paramType, ValueDisplayType.ValueOnly)))
					Case ValueType.SteppedRange
						rst = String.Format(fmtTriple, GetDisplay(.Values(0), paramType, ValueDisplayType.Postfix), Resources.through, GetDisplay(.Values(1), paramType, ValueDisplayType.ValueOnly)) &
						String.Format(fmtSpaceBoth, Resources.andThen) & String.Format(fmtTriple, GetDisplay(.Values(2), paramType, ValueDisplayType.Prefix), Resources.startingWith, GetDisplay(.Values(1), paramType, ValueDisplayType.ValueOnly))
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
		Protected Overridable Function GetDisplay(paramValue As ParameterValue, paramType As ParameterType, valueDisplayType As ValueDisplayType) As String
			Dim rst As String = String.Empty

			With paramValue
				Select Case valueDisplayType
					Case ValueDisplayType.ValueOnly
						If paramType = ParameterType.DayOfWeek Or .ValueType = ValueType.DayOfWeek Then
							rst = CType(.Value, DayOfWeek).ToDisplay
						ElseIf paramType = ParameterType.Month Or .ValueType = ValueType.MonthOfYear Then
							rst = CType(.Value, MonthOfYear).ToDisplay
						Else
							rst = If(.Value >= 0, CStr(.Value), Unknown)
						End If
					Case ValueDisplayType.Prefix
						rst = If(.Value > 1, String.Format(fmtTuple, .Value, paramType.ToDisplay(True)), paramType.ToDisplay(False))
					Case ValueDisplayType.Postfix
						If paramType = ParameterType.DayOfWeek Or .ValueType = ValueType.DayOfWeek Then
							rst = CType(.Value, DayOfWeek).ToDisplay
						ElseIf paramType = ParameterType.Month Or .ValueType = ValueType.MonthOfYear Then
							rst = CType(.Value, MonthOfYear).ToDisplay
						Else
							rst = String.Format(fmtTuple, paramType.ToDisplay(False), .Value)
						End If
				End Select
			End With

			Return rst

		End Function

		''' <summary>
		''' Returns the appropriate, localized preposition for the given <see cref="ParameterType"/>.
		''' </summary>
		''' <param name="paramType">The <see cref="ParameterType"/></param>
		''' <returns>Localized versions of at, on or in.</returns>
		<DebuggerStepThrough()> Protected Overridable Function GetPreposition(paramType As ParameterType) As String

			Select Case paramType
				Case ParameterType.Minute
					Return Resources.atMinute
				Case ParameterType.Hour
					Return Resources._of
				Case ParameterType.Day
					Return Resources.onDay
				Case ParameterType.Month
					Return Resources.inMonth
				Case Else ' ParameterType.WeekDay
					Return Resources.onDay
			End Select

		End Function

#End Region

#Region " Construction "

		''' <summary>
		''' Creates a new instance of a <see cref="DefaultHumanizer"/>.
		''' </summary>
		Public Sub New()
		End Sub

#End Region

	End Class

End Namespace