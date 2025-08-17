using System.Diagnostics;
using BlueHeron.Cron.Localization;

namespace BlueHeron.Cron;

/// <summary>
/// Object, that represents a Cron expression.
/// </summary>
public sealed class Expression : IEquatable<Expression>
{
    #region Objects and variables

    private string? mDisplay;
    private string? mExpression;
    private readonly IHumanizer mHumanizer;
    private readonly List<Parameter> mParameters;

    #endregion

    #region Construction

    /// <summary>
	/// Creates a new <see cref="Expression"/>, using the given parameters.
	/// </summary>
	/// <param name="params">Existing array of <see cref="Parameter"/> objects</param>
	/// <param name="humanizer">The <see cref="IHumanizer"/> to use</param>
	[DebuggerStepThrough()]
	internal Expression(Parameter[] parameters, IHumanizer humanizer)
    {
        mParameters = [.. parameters];
        mHumanizer = humanizer;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the human-readable, localized representation of this <see cref="Cron.Expression"/>.
    /// </summary>
    public string Display
    {
        get
        {
            mDisplay ??= mHumanizer.Humanize(this);
            return mDisplay;
        }
    }

    /// <summary>
    /// Gets the collection of <see cref="Parameter"/> objects that are part of the expression.
    /// </summary>
    public IReadOnlyList<Parameter> Parameters => mParameters;

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Returns the next date and time that matches the schedule that is represented by this expression.
    /// </summary>
    /// <returns>A <see cref="DateTime"/></returns>
    public DateTime Next()
    {
        return FindClosestDate(DateTime.Now, false);
    }

    /// <summary>
    /// Returns the first date and time on or after the given date and time that matches the schedule that is represented by this expression.
    /// </summary>
    /// <param name="datum">The date and time for which the closest match in the future must be found</param>
    /// <returns>A <see cref="DateTime"/></returns>
    public DateTime Next(DateTime datum)
    {
        return FindClosestDate(datum, false);
    }

    /// <summary>
    /// Returns the given number of date and time instances on or after the given date that match the schedule that is represented by this expression.
    /// </summary>
    /// <param name="datum">The date and time from which to start matching future occurrences</param>
    /// <param name="count">The number of matches to return; should be one or more; lower values result in one match</param>
    /// <returns>An <see cref="IEnumerable{DateTime}"/></returns>
    public IEnumerable<DateTime> Next(DateTime datum, int count)
    {
        count = Math.Max(1, count);
        for (var i = 1; i <= count; i++)
        {
            datum = FindClosestDate(datum, false);
            yield return datum;
            datum = datum.AddMinutes(1); // move to one minute after current match
        }
    }

    /// <summary>
    /// Returns the instances of date and time, falling in the given date range, that match the schedule that is represented by this expression.
    /// The <paramref name="toDate"/> must be áfter the <paramref name="fromDate"/> parameter, else zero matches will be returned.
    /// </summary>
    /// <param name="fromDate">The start date and time from which to start matching future occurrences</param>
    /// <param name="toDate">The end date and time after which to stop matching future occurrences</param>
    /// <returns>An <see cref="IEnumerable{DateTime}"</returns>
    public IEnumerable<DateTime> Next(DateTime fromDate, DateTime toDate)
    {
        if (fromDate > toDate)
        {
            yield break;
        }
        while (fromDate < toDate)
        {
            fromDate = FindClosestDate(fromDate, false);
            yield return fromDate;
            fromDate = fromDate.AddMinutes(1); // move to one minute after current match
        }
    }

    /// <summary>
    /// Returns a boolean, determining whether the current date and time are a match for the schedule represented by this expression.
    /// </summary>
    /// <returns>Boolean, <see langword="true"/> if the current date and time match the date and time pattern defined by this expression; else <see langword="false"/></returns>
    public bool Poll()
    {
        return Poll(DateTime.Now);
    }

    /// <summary>
    /// Returns a boolean, determining whether the givent date and time are a match for the schedule represented by this expression.
    /// </summary>
    /// <param name="datum">The date and time to match</param>
    /// <returns>Boolean, <see langword="true"/> if the given date and time match the date and time pattern defined by this expression; else <see langword="false"/></returns>
    public bool Poll(DateTime datum)
    {
        var dtmNext = Next(datum);
        return (datum.Year == dtmNext.Year) && (datum.Month == dtmNext.Month) && (datum.Day == dtmNext.Day) && (datum.Hour == dtmNext.Hour) && (datum.Minute == dtmNext.Minute);
    }

    /// <summary>
    /// Returns the previous date and time that matches the schedule that is represented by this expression.
    /// </summary>
    /// <returns>A <see cref="DateTime"/></returns>
    public DateTime Previous()
    {
        return FindClosestDate(DateTime.Now, true);
    }

    /// <summary>
    /// Returns the first date and time on or before the given date and time that matches the schedule that is represented by this expression.
    /// </summary>
    /// <param name="datum">The date and time for which the closest match in the past must be found</param>
    /// <returns>A <see cref="DateTime"/></returns>
    public DateTime Previous(DateTime datum)
    {
        return FindClosestDate(datum, true);
    }

    /// <summary>
    /// Returns the given number of date and time instances on or before the given date that match the schedule that is represented by this expression.
    /// </summary>
    /// <param name="datum">The date and time from which to start matching past occurrences</param>
    /// <param name="count">The number of matches to return; should be one or more; lower values result in one match</param>
    /// <returns>An <see cref="IEnumerable{DateTime}"/></returns>
    public IEnumerable<DateTime> Previous(DateTime datum, int count)
    {
        count = Math.Max(1, count);
        for (var i = 1; i <= count; i++)
        {
            datum = FindClosestDate(datum, true);
            yield return datum;
            datum = datum.AddMinutes(-1); // move to one minute before current match
        }
    }

    /// <summary>
    /// Returns the instances of date and time, falling in the given date range, that match the schedule that is represented by this expression.
    /// The <paramref name="toDate"/> must be befóre the <paramref name="fromDate"/> parameter, else zero matches will be returned.
    /// </summary>
    /// <param name="fromDate">The start date and time from which to start matching past occurrences</param>
    /// <param name="toDate">The end date and time after which to stop matching past occurrences</param>
    /// <returns>An <see cref="IEnumerable{DateTime}"</returns>
    public IEnumerable<DateTime> Previous(DateTime fromDate, DateTime toDate)
    {
        if (toDate > fromDate)
        {
            yield break;
        }
        while (fromDate > toDate)
        {
            fromDate = FindClosestDate(fromDate, true);
            yield return fromDate;
            fromDate = fromDate.AddMinutes(-1); // move to one minute before current match
        }
    }

    #endregion

    #region Operators

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="left">The left <see cref="Expression"/></param>
    /// <param name="right">The right <see cref="Expression"/></param>
    /// <returns><see langword="true"/> if the <see cref="Expression"/>s are equal</returns>
    public static bool operator ==(Expression left, Expression right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="left">The left <see cref="Expression"/></param>
    /// <param name="right">The right <see cref="Expression"/></param>
    /// <returns><see langword="true"/> if the <see cref="Expression"/>s are not equal</returns>
    public static bool operator !=(Expression left, Expression right) => !left.Equals(right);

    #endregion

    #region Overrides

    /// <summary>
    /// Indicates whether this <see cref="Expression"/> is equal to the other object.
    /// </summary>
    /// <param name="other">The other object</param>
    /// <returns><see langword="true"/> if the objects are equal; else <see langword="false"/></returns>
    public override bool Equals(object? other)
    {
        if (other == null || other.GetType() != typeof(Expression))
        {
            return false;
        }
        return Equals((Expression)other);
    }

    /// <summary>
    /// Indicates whether this <see cref="Expression"/> is equal to the other <see cref="Expression"/>.
    /// </summary>
    /// <param name="other">The other <see cref="Expression"/></param>
    /// <returns><see langword="true"/> if the <see cref="Expression"/>s are equal; else <see langword="false"/></returns>
    public bool Equals(Expression? other)
    {
        if (other is null)
        {
            return false;
        }
        for (var i = 0; i < 5; i++)
        {
            if (mParameters[i].Value != other.mParameters[i].Value)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns a hash code, created from the string representation.
    /// </summary>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    /// Overridden to return the string representation of this <see cref="Expression"/>.
    /// </summary>
    public override string ToString()
    {
        mExpression ??= string.Format(Constants.fmtExpression, mParameters[(int)ParameterType.Minute].Value.ToString(), mParameters[(int)ParameterType.Hour].Value.ToString(), mParameters[(int)ParameterType.Day].Value.ToString(), mParameters[(int)ParameterType.Month].Value.ToString(), mParameters[(int)ParameterType.DayOfWeek].Value.ToString());
        return mExpression;
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Returns the first date and time before or after the given date where the schedule that is represented by this expression is a match.
    /// </summary>
    /// <param name="datum">The date and time for which the closest match in the past or future must be found</param>
    /// <param name="goBack">If <see langword="true"/>, the closest match in the past is returned, else the closest match in the future is returned</param>
    /// <returns>A <see cref="DateTime"/></returns>
    private DateTime FindClosestDate(DateTime datum, bool goBack)
    {
        var carry = 0; // remembered value to add to next level (-1, 0 or +1)
        bool minuteMatched = false, hourMatched = false, dayMatched = false, monthMatched = false, isMatch = false;
        int matchedMinute = 0, matchedHour = 0, matchedDay = 0, matchedMonth, matchedYear;

        while (!isMatch)
        {
            if (!minuteMatched)
            {
                matchedMinute = FindClosestValue(Parameters[(int)ParameterType.Minute].Matches, datum.Minute, goBack, ref carry);
                datum = new DateTime(datum.Year, datum.Month, datum.Day, datum.Hour, matchedMinute, 0).AddHours(carry);
                carry = 0;
                minuteMatched = true;
            }
            if (!hourMatched)
            {
                matchedHour = FindClosestValue(Parameters[(int)ParameterType.Hour].Matches, datum.Hour, goBack, ref carry);
                if (matchedHour != datum.Hour && Parameters[(int)ParameterType.Minute].Value.ValueType != ValueType.Number) // must recalculate minute
                {
                    minuteMatched = false;
                }
                datum = new DateTime(datum.Year, datum.Month, datum.Day, matchedHour, minuteMatched ? matchedMinute : goBack ? Constants.MaximumValue[ParameterType.Minute] : Constants.MinimumValue[ParameterType.Minute], 0).AddDays(carry); // update date to match
                carry = 0;
                hourMatched = true;
            }
            if (!dayMatched)
            {
                matchedDay = FindClosestDay(datum, goBack, ref carry);
                if (matchedDay != datum.Day && Parameters[(int)ParameterType.Hour].Value.ValueType != ValueType.Number) // must recalculate minute and hour
                {
                    minuteMatched = false;
                    hourMatched = false;
                }
                datum = new DateTime(datum.Year, datum.Month, matchedDay, hourMatched ? matchedHour : goBack ? Constants.MaximumValue[ParameterType.Hour] : Constants.MinimumValue[ParameterType.Hour], minuteMatched ? matchedMinute : goBack ? Constants.MaximumValue[ParameterType.Minute] : Constants.MinimumValue[ParameterType.Minute], 0).AddMonths(carry); // update date to match
                carry = 0;
                dayMatched = true;
            }
            if (!monthMatched)
            {
                matchedMonth = FindClosestValue(Parameters[(int)ParameterType.Month].Matches, datum.Month, goBack, ref carry);
                matchedYear = datum.Year + (carry > 0 ? 1 : carry < 0 ? -1 : 0);
                if (matchedMonth != datum.Month || matchedYear != datum.Year) // must recalculate minute, hour and day
                {
                    minuteMatched = false;
                    hourMatched = false;
                    dayMatched = false;
                }
                datum = new DateTime(matchedYear, matchedMonth,
                    dayMatched ? matchedDay : goBack ? Constants.MaximumValue[ParameterType.Day] : Constants.MinimumValue[ParameterType.Day],
                    hourMatched ? matchedHour : goBack ? Constants.MaximumValue[ParameterType.Hour] : Constants.MinimumValue[ParameterType.Hour],
                    minuteMatched ? matchedMinute : goBack ? Constants.MaximumValue[ParameterType.Minute] : Constants.MinimumValue[ParameterType.Minute], 0); // update date to match
                carry = 0;
                monthMatched = true;
            }
            isMatch = minuteMatched & hourMatched & dayMatched & monthMatched;
        }
        return datum;
    }

    /// <summary>
    /// Finds the closest matching day to the given date for this <see cref="Cron.Expression"/>, searching either forward or backward.
    /// For each cycle that the search moves beyond the beginning (looking backward) or end (looking forward), -1 or +1 respectively, is added to the number to be carried to the next level (assuming: minute -> hour -> day -> month -> year).
    /// </summary>
    /// <param name="datum">The date to match</param>
    /// <param name="goBack">If <see langword="true"/>, search backwards in time for the closest match, else search forwards in time</param>
    /// <param name="carry">-1, 0 or +1 to be added to the next level</param>
    /// <returns>The closest matching number</returns>
    private int FindClosestDay(DateTime datum, bool goBack, ref int carry)
    {
        var daysInMonth = DateTime.DaysInMonth(datum.Year, datum.Month);
        var dayPattern = Parameters[(int)ParameterType.Day].Matches.Take(daysInMonth).ToList(); // current month may be 28, 29, 30 or 31 days in length
        var dowValueType = Parameters[(int)ParameterType.DayOfWeek].Value.ValueType;

        if (dowValueType != ValueType.Any)
        {
            var dayOfWeekPattern = Parameters[(int)ParameterType.DayOfWeek].Matches;
            List<int> filteredDayPattern = [];

            dayPattern.ForEach(d =>
            {
                if (dayOfWeekPattern.Contains((int)new DateTime(datum.Year, datum.Month, d).DayOfWeek))
                {
                    filteredDayPattern.Add(d);
                }
            });
            if (dowValueType == ValueType.DayOfWeek) // take intersection with DayOfWeek pattern
            {
                dayPattern = filteredDayPattern;
            }
            else if (dowValueType == ValueType.Symbol_Hash) // take intersection with Hash pattern
            {
                var n = Parameters[(int)ParameterType.DayOfWeek].Value.Values[1].Value;

                dayPattern = n > filteredDayPattern.Count ? [] : [filteredDayPattern[n - 1]] ;
            }
        }
        if (dayPattern.Contains(datum.Day))
        {
            return datum.Day; // exact match
        }
        if (goBack)
        {
            var backwardPattern = dayPattern.Where(v => v < datum.Day).ToList();

            if (backwardPattern.Count == 0)
            {
                carry -= 1;
                return FindClosestDay(new DateTime(datum.Year, datum.Month, 1).AddDays(-1), goBack, ref carry); // continue search starting from the last day of the previous month
            }
            return backwardPattern[^1]; // closest day in past
        }
        else
        {
            var forwardPattern = dayPattern.Where(v => v > datum.Day).ToList();

            if (forwardPattern.Count == 0)
            {
                carry += 1;
                return FindClosestDay(new DateTime(datum.Year, datum.Month, daysInMonth).AddDays(1), goBack, ref carry); // continue search starting from the first day of the next month
            }
            return forwardPattern[0]; // closest day in future
        }
    }

    /// <summary>
    /// Returns the closest matching number to the given value for the given pattern of numbers, searching either forward or backward.
    /// If the search moves beyond the beginning (looking backward) or end (looking forward), -1 or +1 respectively is carried to the next level (assuming: minute -> hour -> day -> month -> year).
    /// </summary>
    /// <param name="pattern">A <see cref="List{int}"/></param>
    /// <param name="value">The value to match</param>
    /// <param name="goBack">If <see langword="true"/>, search backwards in time for the closest match, else search forwards in time</param>
    /// <param name="carry">-1, 0 or +1 to be added to the next level</param>
    /// <returns>The closest matching number</returns>
    private static int FindClosestValue(IReadOnlyList<int> pattern, int value, bool goBack, ref int carry)
    {
        if (pattern.Contains(value))
        {
            return value;
        }
        if (goBack)
        {
            var backwardPattern = pattern.Where(v => v < value).ToList();

            if (backwardPattern.Count == 0)
            {
                carry = -1;
                return pattern[^1]; // highest value of previous cycle
            }
            return backwardPattern[^1]; // closest smaller value
        }
        else
        {
            var forwardPattern = pattern.Where(v => v > value).ToList();

            if (forwardPattern.Count == 0)
            {
                carry = 1;
                return pattern[0]; // lowest value of next cycle
            }
            return forwardPattern[0]; // closest higher value
        }
    }

    #endregion
}