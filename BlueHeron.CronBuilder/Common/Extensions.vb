Imports System.Runtime.CompilerServices
Imports BlueHeron.Cron.Localization

''' <summary>
''' Extension methods.
''' </summary>
Public Module Extensions

	''' <summary>
	''' Capitalizes the first character of the given sentence.
	''' </summary>
	''' <param name="strIn">The string to capitalize</param>
	<Extension(), DebuggerStepThrough()> Friend Function CapitalizeSentence(strIn As String) As String

		Return strIn.Substring(0, 1).ToUpper & strIn.Substring(1)

	End Function

	''' <summary>
	''' Returns True if this <see cref="ValueType"/> value represents a single value.
	''' </summary>
	''' <param name="valueType">A <see cref="ValueType"/></param>
	''' <returns>True, if the given value is one of the following: <see cref="ValueType.Number"/>, <see cref="ValueType.MonthOfYear"/> or <see cref="ValueType.DayOfWeek"/></returns>
	<Extension(), DebuggerStepThrough()> Public Function IsSingleValueType(valueType As ValueType) As Boolean
		Dim intValueType As Integer = CInt(valueType)

		Return (intValueType = 4 OrElse intValueType = 5 OrElse intValueType = 6)

	End Function

	''' <summary>
	''' Returns the human-readable, localized representation of this <see cref="ParameterType"/>.
	''' </summary>
	''' <param name="paramType">This <see cref="ParameterType"/></param>
	''' <param name="isPlural">If true, the plural form is returned</param>
	<Extension(), DebuggerStepThrough()> Public Function ToDisplay(paramType As ParameterType, isPlural As Boolean) As String

		Select Case paramType
			Case ParameterType.Day, ParameterType.WeekDay
				Return If(isPlural, Resources.days, Resources.day)
			Case ParameterType.Hour
				Return If(isPlural, Resources.hours, Resources.hour)
			Case ParameterType.Minute
				Return If(isPlural, Resources.minutes, Resources.minute)
			Case Else ' ParameterType.Month
				Return If(isPlural, Resources.months, Resources.month)
		End Select

	End Function

	''' <summary>
	''' Returns the human-readable, localized representation of this <see cref="DayOfWeek"/>.
	''' </summary>
	''' <param name="dow">This <see cref="DayOfweek"/></param>
	<Extension(), DebuggerStepThrough()> Public Function ToDisplay(dow As DayOfWeek) As String

		Select Case dow
			Case DayOfWeek.FRI
				Return Resources.friday
			Case DayOfWeek.MON
				Return Resources.monday
			Case DayOfWeek.SAT
				Return Resources.saturday
			Case DayOfWeek.SUN
				Return Resources.sunday
			Case DayOfWeek.THU
				Return Resources.thursday
			Case DayOfWeek.TUE
				Return Resources.tuesday
			Case Else ' DayOfWeek.WED
				Return Resources.wednesday
		End Select

	End Function

	''' <summary>
	''' Returns the human-readable, localized representation of this <see cref="MonthOfYear"/>.
	''' </summary>
	''' <param name="moy">This <see cref="MonthOfYear"/></param>
	<Extension(), DebuggerStepThrough()> Public Function ToDisplay(moy As MonthOfYear) As String

		Return String.Format(fmtMonth, CInt(moy))

	End Function

End Module