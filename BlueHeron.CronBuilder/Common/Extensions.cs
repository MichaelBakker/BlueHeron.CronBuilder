using System.Diagnostics;
using System.Globalization;
using Localization;

namespace BlueHeron.Cron;

public static class Extensions
{
    #region Enums

    /// <summary>
    /// Returns <see langword="true"/> if this <see cref="ValueType"/> value represents a single value.
    /// </summary>
    /// <param name="valueType">This <see cref="ValueType"/></param>
    /// <returns><see langword="true"/>, if the given value is one of the following: <see cref="ValueType.Number"/>, <see cref="ValueType.MonthOfYear"/> or <see cref="ValueType.DayOfWeek"/></returns>
    [DebuggerStepThrough()]
    internal static bool IsSingular(this ValueType valueType)
    {
        var intVal = (int)valueType;
        return (intVal == 4 ) || (intVal == 5) || (intVal == 6);
    }

    /// <summary>
    /// Returns <see langword="true"/> if this <see cref="ValueType"/> value represents a symbol.
    /// </summary>
    /// <param name="valueType">This <see cref="ValueType"/></param>
    /// <returns><see langword="true"/>, if the given value is one of the following: <see cref="ValueType.Symbol_Hash"/>, <see cref="ValueType.Symbol_Last"/>, <see cref="ValueType.Symbol_WeekDay"/> or <see cref="ValueType.Symbol_LastWeekDay"/></returns>
    [DebuggerStepThrough()]
    internal static bool IsSymbol(this ValueType valueType)
    {
        var intVal = (int)valueType;
        return intVal > 7 && intVal <= 11;
    }

    /// <summary>
    /// Returns the human-readable, localized representation of this <see cref="ParameterType"/>.
    /// </summary>
    /// <param name="paramType">This <see cref="ParameterType"/></param>
    /// <param name="asPlural">If <see langword="true"/>, return the plural form</param>
    /// <returns>A <see langword="string"/> representation of this <see cref="ParameterType"/></returns>
    [DebuggerStepThrough()]
    public static string ToDisplay(this ParameterType paramType, bool asPlural)
    {
        return paramType switch
        {
            ParameterType.Day or ParameterType.DayOfWeek => asPlural ? Resources.days : Resources.day,
            ParameterType.Hour => asPlural ? Resources.hours : Resources.hour,
            ParameterType.Minute => asPlural ? Resources.minutes : Resources.minute,
            ParameterType.Month => asPlural ? Resources.months : Resources.month,
            _ => $"{Constants.Unknown}",
        };
    }

    /// <summary>
    /// Returns the human-readable, localized representation of this <see cref="DayOfWeek"/>.
    /// </summary>
    /// <param name="dow">This <see cref="DayOfWeek"/></param>
    /// <returns>A <see langword="string"/> representation of this <see cref="DayOfWeek"/></returns>
    [DebuggerStepThrough()]
    public static string ToDisplay(this DayOfWeek dow)
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((System.DayOfWeek)dow);
    }

    /// <summary>
    /// Returns the human-readable, localized representation of this <see cref="MonthOfYear"/>.
    /// </summary>
    /// <param name="moy">This <see cref="MonthOfYear"/></param>
    /// <returns>A <see langword="string"/> representation of this <see cref="MonthOfYear"/></returns>
    [DebuggerStepThrough()]
    public static string ToDisplay(this MonthOfYear moy)
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)moy);
    }

    /// <summary>
    /// Validates the given <see cref="ParameterValue"/> for this <see cref="ParameterType"/>.
    /// All values that reach this point are of a single value type (<see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> or <see cref="ValueType.MonthOfYear"/>).
    /// </summary>
    /// <param name="paramType">This <see cref="ParameterType"/></param>
    /// <param name="pv">The <see cref="ParameterValue"/> to validate</param>
    /// <returns><see langword="true"/> if the value is valid for this <see cref="ParameterType"/>, else <see langword="false"/></returns>
    public static bool Validate(this ParameterType paramType, ParameterValue pv)
    {
        if (pv.IsFault)
        {
            return false;
        }
        if (pv.ValueType == ValueType.MonthOfYear && paramType != ParameterType.Month)
        {
            pv.Message = string.Format(Resources.errParameterValueType, ValueType.MonthOfYear);
            return false;
        }
        if (pv.ValueType == ValueType.DayOfWeek && paramType != ParameterType.DayOfWeek)
        {
            pv.Message = string.Format(Resources.errParameterValueType, ValueType.DayOfWeek);
            return false;
        }
        if (pv.Value < Constants.MinimumValue[paramType] || pv.Value > Constants.MaximumValue[paramType])
        {
            pv.Message = string.Format(Resources.errValueOutOfRange, pv.Value);
            return false;
        }
        return true;
    }

    #endregion

    #region Numbers

    /// <summary>
    /// Returns an array of values from the given start value to the given end value.
    /// If <paramref name="from"/> is larger than <paramref name="toVal"/>, an empty array is returned.
    /// </summary>
    /// <param name="from">This value, the start value</param>
    /// <param name="to">The end value</param>
    /// <param name="step">The increment step</param>
    /// <returns></returns>
    internal static IEnumerable<int> To(this int from, int to, int step)
    {
        if (from > to)
        {
            yield break;
        }
        for (var i = from; i <= to; i+=step)
        {
            yield return i;
        }
    }

    #endregion

    #region TimeZoneInfo (adapted from https://github.com/HangfireIO/Cronos/tree/master/src/Cronos)



    #endregion
}