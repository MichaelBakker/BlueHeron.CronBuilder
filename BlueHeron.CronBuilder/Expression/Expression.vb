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
	Public Property Expression As String
		Get
			If String.IsNullOrEmpty(mExpression) Then
				mExpression = String.Format(fmtExpression, Parameters(ParameterType.Minute).Value.ToString, Parameters(ParameterType.Hour).Value.ToString, Parameters(ParameterType.Day).Value.ToString, Parameters(ParameterType.Month).Value.ToString, Parameters(ParameterType.WeekDay).Value.ToString)
			End If
			Return mExpression
		End Get
		Friend Set(value As String)
			mExpression = value
		End Set
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
	''' <param name="goBack">If True, the closest match in the past is returned, else the closest match in the future</param>
	''' <returns>A date</returns>
	Private Function FindClosestDate(datum As Date, goBack As Boolean) As Date
		Dim carry As Integer = 0 ' remembered value to add to next level (-1, 0 or +1)
		Dim minuteMatched As Boolean = False
		Dim matchedMinute, matchedHour, matchedDay, matchedMonth, matchedYear As Integer
		Dim isMatch As Boolean

		Do Until minuteMatched
			matchedMinute = FindClosestValue(Parameters(ParameterType.Minute).Matches, datum.Minute, goBack, carry) ' start with smallest component and work up
			datum = New Date(datum.Year, datum.Month, datum.Day, datum.Hour, matchedMinute, 0).AddHours(carry)
			carry = 0
			minuteMatched = True
			matchedHour = FindClosestValue(Parameters(ParameterType.Hour).Matches, datum.Hour, goBack, carry)
			If (matchedHour <> datum.Hour) AndAlso (Not Parameters(ParameterType.Minute).Value.ValueType = ValueType.Number) Then ' must recalculate minute
				minuteMatched = False
			End If
			datum = New Date(datum.Year, datum.Month, datum.Day, matchedHour, If(minuteMatched, matchedMinute, 0), 0).AddDays(carry)
			carry = 0
		Loop

		carry = 0

		Do Until isMatch ' if WeekDay parameter is not any, recalculation of the day is necessary after matching the month and year
			matchedDay = FindClosestDay(datum, goBack, carry)
			datum = New Date(datum.Year, datum.Month, matchedDay, matchedHour, matchedMinute, 0).AddMonths(carry)
			carry = 0
			matchedMonth = FindClosestValue(Parameters(ParameterType.Month).Matches, datum.Month, goBack, carry)
			matchedYear = datum.Year + carry
			isMatch = (Parameters(ParameterType.WeekDay).Value.ValueType = ValueType.Any) OrElse ((matchedMonth = datum.Month) AndAlso (matchedYear = datum.Year) AndAlso (carry = 0)) ' Day, DayOfWeek, Month ánd Year have been matched
			datum = New Date(matchedYear, matchedMonth, matchedDay, matchedHour, matchedMinute, 0)
			carry = 0
		Loop

		Return datum

	End Function

	''' <summary>
	''' Finds the closest matching number to the given value for the given pattern of numbers, searching either forward or backward.
	''' If the search moves beyond the beginning (looking backward) or end (looking forward), -1 or +1 respectively is carried to the next level (assuming: minute -> hour -> day -> month -> year).
	''' </summary>
	''' <param name="pattern">An <see cref="IEnumerable(Of Integer)"/></param>
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

				If backwardPattern.Count = 0 Then
					carry = -1
					Return pattern.Last(Function(v) v > value) ' highest value of previous cycle
				Else
					Return backwardPattern.Last ' closest smaller value
				End If
			Else
				Dim forwardPattern As IEnumerable(Of Integer) = pattern.Where(Function(v) v > value)

				If forwardPattern.Count = 0 Then
					carry = 1
					Return pattern.First(Function(v) v < value) ' lowest value of next cycle
				Else
					Return forwardPattern.First ' closest higher value
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

		If Parameters(ParameterType.WeekDay).Value.ValueType <> ValueType.Any Then ' take the DayOfWeek pattern into account
			Dim dayOfWeekPattern As IEnumerable(Of Integer) = Parameters(ParameterType.WeekDay).Matches
			Dim filteredDayPattern As New List(Of Integer)

			dayPattern.ForEach(Sub(d)
								   If dayOfWeekPattern.Contains(CInt(New Date(datum.Year, datum.Month, d).DayOfWeek)) Then
									   filteredDayPattern.Add(d)
								   End If
							   End Sub)
			dayPattern = filteredDayPattern
		End If

		If dayPattern.Contains(datum.Day) Then
			Return datum.Day ' exact match
		Else
			If goBack Then
				Dim backwardPattern As List(Of Integer) = dayPattern.Where(Function(v) v < datum.Day).ToList

				If backwardPattern.Count = 0 Then
					carry -= 1
					Return FindClosestDay(New Date(datum.Year, datum.Month, 1).AddDays(-1), goBack, carry) ' continue search starting from the last day of the previous month
				Else
					Return backwardPattern.Last ' closest day in past
				End If
			Else
				Dim forwardPattern As List(Of Integer) = dayPattern.Where(Function(v) v > datum.Day).ToList

				If forwardPattern.Count = 0 Then
					carry += 1
					Return FindClosestDay(New Date(datum.Year, datum.Month, daysInMonth).AddDays(1), goBack, carry)  ' continue search starting from the first day of the next month
				Else
					Return forwardPattern.First ' closest day in future
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