' The MIT License (MIT)
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

Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports BlueHeron.Cron.Localization

''' <summary>
''' Extension methods.
''' </summary>
Friend Module Extensions

	''' <summary>
	''' Returns True if this <see cref="ValueType"/> value represents a single value.
	''' </summary>
	''' <param name="valueType">A <see cref="ValueType"/></param>
	''' <returns>True, if the given value is one of the following: <see cref="ValueType.Number"/>, <see cref="ValueType.MonthOfYear"/> or <see cref="ValueType.DayOfWeek"/></returns>
	<Extension(), DebuggerStepThrough()> Function IsSingleValueType(valueType As ValueType) As Boolean
		Dim intValueType As Integer = CInt(valueType)

		Return (intValueType = 4 OrElse intValueType = 5 OrElse intValueType = 6)

	End Function

	''' <summary>
	''' Returns True if this <see cref="ValueType"/> value represents a symbol.
	''' </summary>
	''' <param name="valueType">A <see cref="ValueType"/></param>
	''' <returns>True, if the given value is one of the following: <see cref="ValueType.Symbol_Hash"/>, <see cref="ValueType.Symbol_Last"/>, <see cref="ValueType.Symbol_WeekDay"/> or <see cref="ValueType.Symbol_LastWeekDay"/></returns>
	<Extension(), DebuggerStepThrough()> Function IsSymbolValueType(valueType As ValueType) As Boolean
		Dim intValueType As Integer = CInt(valueType)

		Return (intValueType > 7 AndAlso intValueType <= 11)

	End Function

	''' <summary>
	''' Returns an array of values from the given start value to the given end value.
	''' If <paramref name="from"/> is larger than <paramref name="toVal"/>, an empty array is returned.
	''' </summary>
	''' <param name="from">Start value</param>
	''' <param name="toVal">End value</param>
	''' <param name="stepVal">Increment step</param>
	''' <returns>An <see cref="IEnumerable(Of Integer)"/></returns>
	<Extension(), DebuggerStepThrough()> Iterator Function [To](from As Integer, toVal As Integer, stepVal As Integer) As IEnumerable(Of Integer)

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
	<Extension(), DebuggerStepThrough()> Function ToDisplay(paramType As ParameterType, isPlural As Boolean) As String

		Select Case paramType
			Case ParameterType.Day, ParameterType.DayOfWeek
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
	<Extension(), DebuggerStepThrough()> Function ToDisplay(dow As DayOfWeek) As String

		Return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(CType(dow, System.DayOfWeek))

	End Function

	''' <summary>
	''' Returns the human-readable, localized representation of this <see cref="MonthOfYear"/>.
	''' </summary>
	''' <param name="moy">This <see cref="MonthOfYear"/></param>
	<Extension(), DebuggerStepThrough()> Function ToDisplay(moy As MonthOfYear) As String

		Return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(moy)

	End Function

#Region " DateTime " ' adapted from https://github.com/HangfireIO/Cronos/tree/master/src/Cronos

	<Extension()> Function GetAmbiguousIntervalEnd(zone As TimeZoneInfo, ambiguousTime As Date) As DateTimeOffset
		Dim dstTransitionEnd As Date = GetDstTransitionEndDateTime(zone, ambiguousTime)

		Return New DateTimeOffset(dstTransitionEnd, zone.BaseUtcOffset)

	End Function

	<Extension()> Function GetDaylightOffset(zone As TimeZoneInfo, ambiguousDateTime As Date) As TimeSpan
		Dim offsets As TimeSpan() = GetAmbiguousOffsets(zone, ambiguousDateTime)
		Dim baseOffset As TimeSpan = zone.BaseUtcOffset

		If offsets(0) <> baseOffset Then
			Return offsets(0)
		End If

		Return offsets(1)

	End Function

	<Extension()> Function GetDaylightTimeEnd(zone As TimeZoneInfo, ambiguousTime As Date, daylightOffset As TimeSpan) As DateTimeOffset
		Dim daylightTransitionEnd As Date = GetDstTransitionEndDateTime(zone, ambiguousTime)

		Return New DateTimeOffset(daylightTransitionEnd.AddTicks(-1), daylightOffset)

	End Function

	<Extension()> Function GetDaylightTimeStart(zone As TimeZoneInfo, invalidDateTime As Date) As DateTimeOffset
		Dim dstTransitionDateTime As New Date(invalidDateTime.Year, invalidDateTime.Month, invalidDateTime.Day, invalidDateTime.Hour, invalidDateTime.Minute, 0, 0, invalidDateTime.Kind)

		Do While zone.IsInvalidTime(dstTransitionDateTime)
			dstTransitionDateTime = dstTransitionDateTime.AddMinutes(1)
		Loop

		Dim dstOffset As TimeSpan = zone.GetUtcOffset(dstTransitionDateTime)

		Return New DateTimeOffset(dstTransitionDateTime, dstOffset)

	End Function

	<Extension()> Function GetStandardTimeStart(zone As TimeZoneInfo, ambiguousTime As Date, daylightOffset As TimeSpan) As DateTimeOffset
		Dim dstTransitionEnd As Date = GetDstTransitionEndDateTime(zone, ambiguousTime)

		Return New DateTimeOffset(dstTransitionEnd, daylightOffset).ToOffset(zone.BaseUtcOffset)

	End Function

	''' This method is a workaround for a difference between the .NET Framework, .NET Core And Mono in handling a transition from daylight saving time (DST) to standard time (ST).
	''' During such a transition the timespan of one hour before the transition point is repeated.
	''' .NET Framework and .NET Core consider backward DST transition as [1:00 DST --> 2:00 ST)--[1:00 ST --> 2:00 ST]. So 2:00 is not ambiguous, but 1:00 is ambiguous.
	''' Mono considers backward DST transition as [1:00 ST --> 2:00 DST]--(1:00 ST --> 2:00 ST]. So 2:00 is ambiguous, but 1:00 is not ambiguous.
	''' To have the same behaviour for all frameworks, add 1 tick to ambiguousTime. Thus 1:00 is ambiguous and 2:00 is not ambiguous. 
	Function IsAmbiguousTime(zone As TimeZoneInfo, ambiguousTime As Date) As Boolean

		Return zone.IsAmbiguousTime(ambiguousTime.AddTicks(1))

	End Function

	''' This method is a workaround for a difference between the .NET Framework, .NET Core And Mono in handling a transition from daylight saving time (DST) to standard time (ST).
	''' During such a transition the timespan of one hour before the transition point is repeated.
	''' .NET Framework and .NET Core consider backward DST transition as [1:00 DST --> 2:00 ST)--[1:00 ST --> 2:00 ST]. So 2:00 is not ambiguous, but 1:00 is ambiguous.
	''' Mono considers backward DST transition as [1:00 ST --> 2:00 DST]--(1:00 ST --> 2:00 ST]. So 2:00 is ambiguous, but 1:00 is not ambiguous.
	''' To have the same behaviour for all frameworks, add 1 tick to ambiguousTime. Thus 1:00 is ambiguous and 2:00 is not ambiguous. 
	Private Function GetAmbiguousOffsets(zone As TimeZoneInfo, ambiguousTime As Date) As TimeSpan()

		Return zone.GetAmbiguousTimeOffsets(ambiguousTime.AddTicks(1))

	End Function

	Private Function GetDstTransitionEndDateTime(zone As TimeZoneInfo, ambiguousDateTime As Date) As Date
		Dim dstTransitionDateTime As New DateTime(ambiguousDateTime.Year, ambiguousDateTime.Month, ambiguousDateTime.Day, ambiguousDateTime.Hour, ambiguousDateTime.Minute, 0, 0, ambiguousDateTime.Kind)

		Do While zone.IsAmbiguousTime(dstTransitionDateTime)
			dstTransitionDateTime = dstTransitionDateTime.AddMinutes(1)
		Loop

		Return dstTransitionDateTime

	End Function

#End Region

End Module