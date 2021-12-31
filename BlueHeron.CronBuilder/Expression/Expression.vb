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

Imports BlueHeron.Cron.Localization

''' <summary>
''' Object, that represents a Cron expression.
''' </summary>
<DebuggerDisplay("{Expression}")>
Public NotInheritable Class Expression
	Implements IEquatable(Of Expression)

#Region " Objects and variables "

	Private mDisplay As String
	Private mExpression As String
	Private ReadOnly mHumanizer As IHumanizer

#End Region

#Region " Properties "

	''' <summary>
	''' The collection of <see cref="Parameter"/> objects that are part of the expression.
	''' </summary>
	''' <returns>An array of <see cref="Parameter"/> objects</returns>
	Public ReadOnly Property Parameters As IEnumerable(Of Parameter)

	''' <summary>
	''' Human-readable, localized representation of this expression.
	''' </summary>
	Public ReadOnly Property Display As String
		Get
			If String.IsNullOrEmpty(mDisplay) Then
				mDisplay = mHumanizer.Humanize(Me)
			End If
			Return mDisplay
		End Get
	End Property

	''' <summary>
	''' Returns the textual representation of this expression.
	''' </summary>
	Public ReadOnly Property Expression As String
		Get
			If String.IsNullOrEmpty(mExpression) Then
				mExpression = String.Format(fmtExpression, Parameters(ParameterType.Minute).Value.ToString, Parameters(ParameterType.Hour).Value.ToString, Parameters(ParameterType.Day).Value.ToString, Parameters(ParameterType.Month).Value.ToString, Parameters(ParameterType.DayOfWeek).Value.ToString)
			End If
			Return mExpression
		End Get
	End Property

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Indicates whether the current <see cref="Expression"/> is equal to the object.
	''' </summary>
	''' <param name="other">The object to compare</param>
	''' <returns>True, if both objects are equal</returns>
	Public Shadows Function Equals(other As Object) As Boolean

		If (other Is Nothing) OrElse (Not TypeOf (other) Is Expression) Then
			Return False
		End If

		Return Equals(DirectCast(other, Expression))

	End Function

	''' <summary>
	''' Indicates whether the current <see cref="Expression"/> is equal to another <see cref="Expression"/>.
	''' </summary>
	''' <param name="other">The <see cref="Expression"/> to compare</param>
	''' <returns>True, if both objects are equal</returns>
	Public Shadows Function Equals(other As Expression) As Boolean Implements IEquatable(Of Expression).Equals

		If other Is Nothing Then
			Return False
		End If
		For i As Integer = 0 To 4
			If Parameters(i).Value <> other.Parameters(i).Value Then
				Return False
			End If
		Next

		Return True

	End Function

	''' <summary>
	''' Equality operator.
	''' </summary>
	''' <param name="left">The left <see cref="Expression"/></param>
	''' <param name="right">The right <see cref="Expression"/></param>
	Public Shared Operator =(left As Expression, right As Expression) As Boolean

		Return left.Equals(right)

	End Operator

	''' <summary>
	''' Inequality operator.
	''' </summary>
	''' <param name="left">The left <see cref="Expression"/></param>
	''' <param name="right">The right <see cref="Expression"/></param>
	Public Shared Operator <>(left As Expression, right As Expression) As Boolean

		Return Not left = right

	End Operator

	''' <summary>
	''' Returns the next date and time at which the schedule that is represented by this expression is matched.
	''' </summary>
	''' <returns>A date and time</returns>
	Public Function [Next]() As Date

		Return FindClosestDate(Date.Now, False)

	End Function

	''' <summary>
	''' Returns the first date and time on or after the given date at which the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the future must be found</param>
	''' <returns>A date and time</returns>
	Public Function [Next](datum As Date) As Date

		Return FindClosestDate(datum, False)

	End Function

	''' <summary>
	''' Returns the given number of date and time instances on or after the given date at which the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time from which to start matching future occurrences</param>
	''' <param name="count">The number of matched dates to return</param>
	''' <returns>An <see cref="IEnumerable(Of Date)"/></returns>
	Public Iterator Function [Next](datum As Date, count As Integer) As IEnumerable(Of Date)

		For i As Integer = 1 To Math.Max(1, count)
			datum = FindClosestDate(datum, False)
			Yield datum
			datum = datum.AddMinutes(1) ' move to 1 minute after current match
		Next

	End Function

	''' <summary>
	''' Returns the instances of date and time, falling in the given date range, at which the schedule that is represented by this expression is matched.
	''' The <paramref name="toDate"/> must be áfter the <paramref name="fromDate"/> parameter, else zero matches will be returned.
	''' </summary>
	''' <param name="fromDate">The start date and time from which to start matching future occurrences</param>
	''' <param name="toDate">The end date and time after which to stop matching future occurrences</param>
	''' <returns>An <see cref="IEnumerable(Of Date)"/></returns>
	Public Iterator Function [Next](fromDate As Date, toDate As Date) As IEnumerable(Of Date)

		If fromDate > toDate Then
			Yield Nothing
		End If

		Do While fromDate < toDate
			fromDate = FindClosestDate(fromDate, False)
			Yield fromDate
			fromDate = fromDate.AddMinutes(1) ' move to 1 minute after current match
		Loop

	End Function

	''' <summary>
	''' Returns a boolean, determining whether the current date and time are a match for this schedule.
	''' </summary>
	''' <returns>Boolean, True when the current date and time match the date and time pattern defined by this expression</returns>
	Public Function Poll() As Boolean

		Return Poll(Date.Now)

	End Function

	''' <summary>
	''' Returns a boolean, determining whether the given date and time are a match for this schedule.
	''' </summary>
	''' <returns>Boolean, True when the given date and time match the date and time pattern defined by this expression</returns>
	Public Function Poll(datum As Date) As Boolean
		Dim dtmNext As Date = [Next](datum)

		Return (datum.Year = dtmNext.Year) AndAlso (datum.Month = dtmNext.Month) AndAlso (datum.Day = dtmNext.Day) AndAlso (datum.Hour = dtmNext.Hour) AndAlso (datum.Minute = dtmNext.Minute)

	End Function

	''' <summary>
	''' Returns the previous date and time at which the schedule that is represented by this expression was matched.
	''' </summary>
	''' <returns>A date and time</returns>
	Public Function Previous() As Date

		Return FindClosestDate(Date.Now, True)

	End Function

	''' <summary>
	''' Returns the first date and time on or before the given date at which the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the past must be found</param>
	''' <returns>A date and time</returns>
	Public Function Previous(datum As Date) As Date

		Return FindClosestDate(datum, True)

	End Function

	''' <summary>
	''' Returns the given number of date and time instances on or before the given date at which the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the past must be found</param>
	''' <param name="count">The number of matched dates to return</param>
	''' <returns>An <see cref="IEnumerable(Of Date)"/></returns>
	Public Iterator Function Previous(datum As Date, count As Integer) As IEnumerable(Of Date)

		For i As Integer = 1 To Math.Max(1, count)
			datum = FindClosestDate(datum, True)
			Yield datum
			datum = datum.AddMinutes(-1) ' move to 1 minute before current match
		Next

	End Function

	''' <summary>
	''' Returns the instances of date and time, falling in the given date range, at which the schedule that is represented by this expression is matched.
	''' The <paramref name="toDate"/> must be befóre the <paramref name="fromDate"/> parameter, else zero matches will be returned.
	''' </summary>
	''' <param name="fromDate">The start date and time from which to start matching past occurrences</param>
	''' <param name="toDate">The end date and time after which to stop matching past occurrences</param>
	''' <returns>An <see cref="IEnumerable(Of Date)"/></returns>
	Public Iterator Function Previous(fromDate As Date, toDate As Date) As IEnumerable(Of Date)

		If toDate > fromDate Then
			Yield Nothing
		End If
		Do While fromDate > toDate
			fromDate = FindClosestDate(fromDate, True)
			Yield fromDate
			fromDate = fromDate.AddMinutes(-1) ' move to 1 minute before current match
		Loop

	End Function

	''' <summary>
	''' Returns the string representation of this expression.
	''' </summary>
	<DebuggerStepThrough()>
	Public Overrides Function ToString() As String

		Return Expression

	End Function

#End Region

#Region " Private methods and functions "

	''' <summary>
	''' Returns the first date and time before or after the given date at which the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the past or future must be found</param>
	''' <param name="goBack">If True, the closest match in the past is returned, else the closest match in the future is returned</param>
	''' <returns>A <see cref="Date"/></returns>
	Private Function FindClosestDate(datum As Date, goBack As Boolean) As Date
		Dim carry As Integer = 0 ' remembered value to add to next level (-1, 0 or +1)
		Dim minuteMatched, hourMatched, dayMatched, monthMatched, isMatch As Boolean
		Dim matchedMinute, matchedHour, matchedDay, matchedMonth, matchedYear As Integer

		Do Until isMatch ' start with smallest date/time component and work up
			If Not minuteMatched Then
				matchedMinute = FindClosestValue(Parameters(ParameterType.Minute).Matches, datum.Minute, goBack, carry)
				datum = New Date(datum.Year, datum.Month, datum.Day, datum.Hour, matchedMinute, 0).AddHours(carry)
				carry = 0
				minuteMatched = True
			End If
			If Not hourMatched Then
				matchedHour = FindClosestValue(Parameters(ParameterType.Hour).Matches, datum.Hour, goBack, carry)
				If (matchedHour <> datum.Hour) AndAlso (Not Parameters(ParameterType.Minute).Value.ValueType = ValueType.Number) Then ' must recalculate minute
					minuteMatched = False
				End If
				datum = New Date(datum.Year, datum.Month, datum.Day, matchedHour, If(minuteMatched, matchedMinute, If(goBack, MaximumValue(ParameterType.Minute), MinimumValue(ParameterType.Minute))), 0).AddDays(carry) ' update date to match
				carry = 0
				hourMatched = True
			End If
			If Not dayMatched Then
				matchedDay = FindClosestDay(datum, goBack, carry)
				If (matchedDay <> datum.Day) AndAlso (Not Parameters(ParameterType.Hour).Value.ValueType = ValueType.Number) Then  ' must recalculate minute and hour
					minuteMatched = False
					hourMatched = False
				End If
				datum = New Date(datum.Year, datum.Month, matchedDay, If(hourMatched, matchedHour, If(goBack, MaximumValue(ParameterType.Hour), MinimumValue(ParameterType.Hour))), If(minuteMatched, matchedMinute, If(goBack, MaximumValue(ParameterType.Minute), MinimumValue(ParameterType.Minute))), 0).AddMonths(carry)  ' update date to match
				carry = 0
				dayMatched = True
			End If
			If Not monthMatched Then
				matchedMonth = FindClosestValue(Parameters(ParameterType.Month).Matches, datum.Month, goBack, carry)
				matchedYear = datum.Year + If(carry > 0, 1, If(carry < 0, -1, 0))
				If (matchedMonth <> datum.Month) OrElse (matchedYear <> datum.Year) Then  ' must recalculate minute, hour and day
					minuteMatched = False
					hourMatched = False
					dayMatched = False
				End If
				datum = New Date(matchedYear, matchedMonth, If(dayMatched, matchedDay, If(goBack, MaximumValue(ParameterType.Day), MinimumValue(ParameterType.Day))), If(hourMatched, matchedHour, If(goBack, MaximumValue(ParameterType.Hour), MinimumValue(ParameterType.Hour))), If(minuteMatched, matchedMinute, If(goBack, MaximumValue(ParameterType.Minute), MinimumValue(ParameterType.Minute))), 0)  ' update date to match
				carry = 0
				monthMatched = True
			End If
			isMatch = (minuteMatched And hourMatched And dayMatched And monthMatched)
		Loop

		Return datum

	End Function

	''' <summary>
	''' Finds the closest matching number to the given value for the given pattern of numbers, searching either forward or backward.
	''' If the search moves beyond the beginning (looking backward) or end (looking forward), -1 or +1 respectively is carried to the next level (assuming: minute -> hour -> day -> month -> year).
	''' </summary>
	''' <param name="pattern">A <see cref="List(Of Integer)"/></param>
	''' <param name="value">The value to match</param>
	''' <param name="goBack">If true, search backwards for the closest match</param>
	''' <param name="carry">-1, 0 or +1 to be added to the next level</param>
	''' <returns>The closest matching number</returns>
	Private Function FindClosestValue(pattern As IEnumerable(Of Integer), value As Integer, goBack As Boolean, ByRef carry As Integer) As Integer

		If pattern.Contains(value) Then
			Return value ' exact match
		Else
			If goBack Then
				Dim backwardPattern As IEnumerable(Of Integer) = pattern.Where(Function(v) v < value)
				Dim cnt As Integer = backwardPattern.Count

				If cnt = 0 Then
					carry = -1
					Return pattern(pattern.Count - 1) ' highest value of previous cycle
				Else
					Return backwardPattern(cnt - 1) ' closest smaller value
				End If
			Else
				Dim forwardPattern As IEnumerable(Of Integer) = pattern.Where(Function(v) v > value)

				If forwardPattern.Count = 0 Then
					carry = 1
					Return pattern(0) ' lowest value of next cycle
				Else
					Return forwardPattern(0) ' closest higher value
				End If
			End If
		End If

	End Function

	''' <summary>
	''' Finds the closest matching day to the given date for this <see cref="Cron.Expression"/>, searching either forward or backward.
	''' For each cycle that the search moves beyond the beginning (looking backward) or end (looking forward), -1 or +1 respectively, is added to the number to be carried to the next level (assuming: minute -> hour -> day -> month -> year).
	''' </summary>
	''' <param name="datum">The date to match</param>
	''' <param name="goBack">If true, search backwards for the closest match</param>
	''' <param name="carry"></param>
	''' <returns></returns>
	Private Function FindClosestDay(datum As Date, goBack As Boolean, ByRef carry As Integer) As Integer
		Dim daysInMonth As Integer = Date.DaysInMonth(datum.Year, datum.Month)
		Dim dayPattern As List(Of Integer) = Parameters(ParameterType.Day).Matches.Take(daysInMonth).ToList ' current month may be 28, 29, 30 or 31 days in length
		Dim dowValueType As ValueType = Parameters(ParameterType.DayOfWeek).Value.ValueType

		If dowValueType <> ValueType.Any Then
			Dim dayOfWeekPattern As IEnumerable(Of Integer) = Parameters(ParameterType.DayOfWeek).Matches
			Dim filteredDayPattern As New List(Of Integer)

			dayPattern.ForEach(Sub(d)
								   If dayOfWeekPattern.Contains(New Date(datum.Year, datum.Month, d).DayOfWeek) Then
									   filteredDayPattern.Add(d)
								   End If
							   End Sub)

			If dowValueType = ValueType.DayOfWeek Then ' take intersection with DayOfWeek pattern
				dayPattern = filteredDayPattern
			ElseIf dowValueType = ValueType.Symbol_Hash Then ' take intersection with Hash pattern
				Dim n As Integer = Parameters(ParameterType.DayOfWeek).Value.Values(1).Value

				dayPattern = If(n > filteredDayPattern.Count, New List(Of Integer), {filteredDayPattern(n - 1)}.ToList)
			End If
		End If

		If dayPattern.Contains(datum.Day) Then
			Return datum.Day ' exact match
		Else
			If goBack Then
				Dim backwardPattern As IEnumerable(Of Integer) = dayPattern.Where(Function(v) v < datum.Day)
				Dim cnt As Integer = backwardPattern.Count

				If cnt = 0 Then
					carry -= 1
					Return FindClosestDay(New Date(datum.Year, datum.Month, 1).AddDays(-1), goBack, carry) ' continue search starting from the last day of the previous month
				Else
					Return backwardPattern(cnt - 1) ' closest day in past
				End If
			Else
				Dim forwardPattern As IEnumerable(Of Integer) = dayPattern.Where(Function(v) v > datum.Day)

				If forwardPattern.Count = 0 Then
					carry += 1
					Return FindClosestDay(New Date(datum.Year, datum.Month, daysInMonth).AddDays(1), goBack, carry)  ' continue search starting from the first day of the next month
				Else
					Return forwardPattern(0) ' closest day in future
				End If
			End If
		End If

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new <see cref="Expression"/>, using the given parameters.
	''' </summary>
	''' <param name="params">Existing array of <see cref="Parameter"/> objects</param>
	''' <param name="humanizer">The <see cref="IHumanizer"/> to use</param>
	<DebuggerStepThrough()>
	Friend Sub New(params As Parameter(), ByRef humanizer As IHumanizer)

		Parameters = params
		mHumanizer = humanizer

	End Sub

#End Region

End Class