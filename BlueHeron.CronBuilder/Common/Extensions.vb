Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports BlueHeron.Cron.Localization

''' <summary>
''' Extension methods.
''' </summary>
Public Module Extensions

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
	''' Returns the appropriate, localized preposition for the given <see cref="ParameterType"/>.
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/></param>
	''' <returns>Localized versions of at, on or in.</returns>
	<Extension(), DebuggerStepThrough()> Public Function Preposition(paramType As ParameterType) As String

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

	''' <summary>
	''' Returns an array of values from the given start value to the given end value.
	''' If <paramref name="from"/> is larger than <paramref name="toVal"/>, an empty array is returned.
	''' </summary>
	''' <param name="from">Start value</param>
	''' <param name="toVal">End value</param>
	''' <param name="stepVal">Increment step</param>
	''' <returns>An <see cref="IEnumerable(Of Integer)"/></returns>
	<Extension(), DebuggerStepThrough()> Friend Iterator Function [To](from As Integer, toVal As Integer, stepVal As Integer) As IEnumerable(Of Integer)

		If from > toVal Then
			Exit Function
		Else
			For i As Integer = from To toVal Step stepVal
				Yield i
			Next
		End If

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

		Return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(CType(dow, System.DayOfWeek))

	End Function

	''' <summary>
	''' Returns the human-readable, localized representation of this <see cref="MonthOfYear"/>.
	''' </summary>
	''' <param name="moy">This <see cref="MonthOfYear"/></param>
	<Extension(), DebuggerStepThrough()> Public Function ToDisplay(moy As MonthOfYear) As String

		Return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(moy)

	End Function

End Module