Imports BlueHeron.Cron.Localization

''' <summary>
''' Object, that represents a Cron expression.
''' </summary>
<DebuggerDisplay("{Expression}")>
Public NotInheritable Class Expression

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
				mExpression = String.Format(fmtExpression, Parameters(ParameterType.Minute).Value.ToString, Parameters(ParameterType.Hour).Value.ToString, Parameters(ParameterType.Day).Value.ToString, Parameters(ParameterType.Month).Value.ToString, Parameters(ParameterType.WeekDay).Value.ToString)
			End If
			Return mExpression
		End Get
	End Property

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Returns the first date and time on or after the given date when the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the future must be found</param>
	''' <returns>A date and time</returns>
	Public Function [Next](datum As Date) As Date

		Return FindClosestDate(datum, False)

	End Function

	''' <summary>
	''' Returns the given number of date and time instances on or after the given date when the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the future must be found</param>
	''' <param name="count">The number of matched dates to return</param>
	''' <returns>An <see cref="IEnumerable(Of Date)"/></returns>
	Public Function [Next](datum As Date, count As Integer) As IEnumerable(Of Date)
		Dim lstResults As New List(Of Date)

		For i As Integer = 1 To Math.Max(1, count)
			lstResults.Add(FindClosestDate(datum, False))
			datum = datum.AddMinutes(1)
		Next

		Return lstResults

	End Function

	''' <summary>
	''' Returns a boolean, determining whether the current date and time are a match for this schedule.
	''' </summary>
	''' <returns>Boolean, True when the current date and time match the date and time pattern defined by this expression</returns>
	Public Function Poll() As Boolean
		Dim dtmNow As Date = Date.Now
		Dim dtmNext As Date = [Next](dtmNow)

		Return (dtmNow.Year = dtmNext.Year) AndAlso (dtmNow.Month = dtmNext.Month) AndAlso (dtmNow.Day = dtmNext.Day) AndAlso (dtmNow.Hour = dtmNext.Hour) AndAlso (dtmNow.Minute = dtmNext.Minute)

	End Function

	''' <summary>
	''' Returns the first date and time on or before the given date when the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the past must be found</param>
	''' <returns>A date and time</returns>
	Public Function Previous(datum As Date) As Date

		Return FindClosestDate(datum, True)

	End Function

	''' <summary>
	''' Returns the given number of date and time instances on or before the given date when the schedule that is represented by this expression is matched.
	''' </summary>
	''' <param name="datum">The date and time to which the closest match in the past must be found</param>
	''' <param name="count">The number of matched dates to return</param>
	''' <returns>An <see cref="IEnumerable(Of Date)"/></returns>
	Public Function Previous(datum As Date, count As Integer) As IEnumerable(Of Date)
		Dim lstResults As New List(Of Date)

		For i As Integer = 1 To Math.Max(1, count)
			lstResults.Add(FindClosestDate(datum, True))
			datum = datum.AddMinutes(-1)
		Next

		Return lstResults

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
	''' Returns the first date and time before or after the given date when the schedule that is represented by this expression is matched.
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
				datum = New Date(datum.Year, datum.Month, datum.Day, matchedHour, If(minuteMatched, matchedMinute, If(goBack, MaximumValues(ParameterType.Minute), MinimumValues(ParameterType.Minute))), 0).AddDays(carry) ' update date to match
				carry = 0
				hourMatched = True
			End If
			If Not dayMatched Then
				matchedDay = FindClosestDay(datum, goBack, carry)
				If (matchedDay <> datum.Day) AndAlso (Not Parameters(ParameterType.Hour).Value.ValueType = ValueType.Number) Then  ' must recalculate minute and hour
					minuteMatched = False
					hourMatched = False
				End If
				datum = New Date(datum.Year, datum.Month, matchedDay, If(hourMatched, matchedHour, If(goBack, MaximumValues(ParameterType.Hour), MinimumValues(ParameterType.Hour))), If(minuteMatched, matchedMinute, If(goBack, MaximumValues(ParameterType.Minute), MinimumValues(ParameterType.Minute))), 0).AddMonths(carry)  ' update date to match
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
				datum = New Date(matchedYear, matchedMonth, If(dayMatched, matchedDay, If(goBack, MaximumValues(ParameterType.Day), MinimumValues(ParameterType.Day))), If(hourMatched, matchedHour, If(goBack, MaximumValues(ParameterType.Hour), MinimumValues(ParameterType.Hour))), If(minuteMatched, matchedMinute, If(goBack, MaximumValues(ParameterType.Minute), MinimumValues(ParameterType.Minute))), 0)  ' update date to match
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
	<DebuggerStepThrough()> Private Function FindClosestValue(pattern As IEnumerable(Of Integer), value As Integer, goBack As Boolean, ByRef carry As Integer) As Integer

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
	<DebuggerStepThrough()> Private Function FindClosestDay(datum As Date, goBack As Boolean, ByRef carry As Integer) As Integer
		Dim daysInMonth As Integer = Date.DaysInMonth(datum.Year, datum.Month)
		Dim dayPattern As List(Of Integer) = Parameters(ParameterType.Day).Matches.Take(daysInMonth).ToList ' current month may be 28, 29, 30 or 31 days in length

		If Parameters(ParameterType.WeekDay).Value.ValueType <> ValueType.Any Then ' take intersection with DayOfWeek pattern
			Dim dayOfWeekPattern As IEnumerable(Of Integer) = Parameters(ParameterType.WeekDay).Matches
			Dim filteredDayPattern As New List(Of Integer)

			dayPattern.ForEach(Sub(d)
								   If dayOfWeekPattern.Contains(New Date(datum.Year, datum.Month, d).DayOfWeek) Then
									   filteredDayPattern.Add(d)
								   End If
							   End Sub)
			dayPattern = filteredDayPattern
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
	''' Creates a new CronExpression, using the given parameters.
	''' </summary>
	''' <param name="params">Existing array of <see cref="Parameter"/> objects</param>
	<DebuggerStepThrough()>
	Friend Sub New(params As Parameter(), ByRef humanizer As IHumanizer)

		Parameters = params
		mHumanizer = humanizer

	End Sub

#End Region

End Class