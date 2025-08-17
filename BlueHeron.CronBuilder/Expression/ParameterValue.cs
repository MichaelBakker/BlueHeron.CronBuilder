using System.Diagnostics;
using Localization;

namespace BlueHeron.Cron;

/// <summary>
/// Class that represents an <see cref="Expression"/> parameter value.
/// </summary>
public class ParameterValue
{
    #region Objects and variables

    private string mExpression = null!;
    private readonly List<ParameterValue> mValues = null!;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="ParameterValue"/> that defaults to 'Any', i.e. '*'.
    /// </summary>
    [DebuggerStepThrough()]
    private ParameterValue() { mValues = []; }

    /// <summary>
    /// Creates a new <see cref="ParameterValue"/> using the given parameters.
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="valueType">The <see cref="Cron.ValueType"/></param>
    [DebuggerStepThrough()]
    private ParameterValue(int value, ValueType valueType) : this()
    {
        OriginalValue = value;
        ValueType = valueType;
        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>, <see cref="Cron.ValueType.Step"/>, <see cref="Cron.ValueType.SteppedRange"/> or <see cref="Cron.ValueType.List"/>.
    /// </summary>
    /// <param name="values">The parameter values</param>
    /// <param name="valueType">The <see cref="Cron.ValueType"/></param>
    [DebuggerStepThrough()]
    internal ParameterValue(IEnumerable<ParameterValue>? values, ValueType valueType)
    {
        OriginalValue = values;
        mValues = values == null ? [] : [.. values];
        ValueType = valueType;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Determines whether this <see cref="ParameterValue"/> is faulty. 
    /// </summary>
    public bool IsFault => Message != null;

    /// <summary>
    /// Gets any error message, if an error occurred.
    /// </summary>
    public string? Message { get; internal set; }

    /// <summary>
    /// Gets the original value that was used to create this parameter value, if any.
    /// </summary>
    public object? OriginalValue { get; }

    /// <summary>
    /// Gets this value as an integer. Only used when the <see cref="ValueType"/> is <see cref="Cron.ValueType.DayOfWeek"/>, <see cref="Cron.ValueType.MonthOfYear"/> or <see cref="Cron.ValueType.Number"/>.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Gets the array of values that together make up this value, if the <see cref="ValueType"/> is a range.
    /// </summary>
    public IReadOnlyList<ParameterValue> Values => mValues;

    /// <summary>
    /// Gets the <see cref="Cron.ValueType"/> of this value.
    /// </summary>
    public ValueType ValueType { get; }

    #endregion

    #region Operators

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns><see langword="true"/> if the values are equal; else <see langword="false"/></returns>
    public static bool operator ==(ParameterValue left, ParameterValue right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <returns><see langword="true"/> if the values are not equal; else <see langword="false"/></returns>
    public static bool operator !=(ParameterValue left, ParameterValue right) => !left.Equals(right);

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of type <see cref="ValueType.Any"/> (i.e. '*').
    /// </summary>
    /// <returns>A <see cref="ParameterValue"/> of type <see cref="ValueType.Any"/></returns>
    public static ParameterValue Any()
    {
        return new ParameterValue();
    }

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of type <see cref="ValueType.DayOfWeek"/>.
    /// </summary>
    /// <param name="value">The week day</param>
    /// <returns>A <see cref="ParameterValue"/></returns>
    public static ParameterValue DayOfWeek(DayOfWeek value)
    {
        return new ParameterValue((int)value, ValueType.DayOfWeek);
    }

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of type <see cref="ValueType.List"/>.
    /// </summary>
    /// <param name="values">The values</param>
    /// <returns>A <see cref="ParameterValue"/></returns>
    public static ParameterValue List(IEnumerable<ParameterValue> values)
    {
        return new ParameterValue(values, ValueType.List);
    }

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of type <see cref="ValueType.MonthOfYear"/>.
    /// </summary>
    /// <param name="value">The month</param>
    /// <returns>A <see cref="ParameterValue"/></returns>
    public static ParameterValue MonthOfYear(MonthOfYear value)
    {
        return new ParameterValue((int)value, ValueType.MonthOfYear);
    }

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of type <see cref="ValueType.Number"/>.
    /// The validity of the number will be asserted when assigning this value to an expression parameter.
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>A <see cref="ParameterValue"/></returns>
    public static ParameterValue Number(int value)
    {
        return new ParameterValue(value, ValueType.Number);
    }

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of a type that will be determined when parsing the value.
    /// If the value cannot be parsed, <see cref="Cron.ValueType.Unknown"/> is assigned and subsequent validation will fail.
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>A <see cref="ParameterValue"/></returns>
    public static ParameterValue Parse(string value)
    {
        var valueType = ValueType.Unknown;
        List<ParameterValue> values = null!;
        int rst;

        if (int.TryParse(value, out rst))
        {
            if (rst >= 0)
            {
                return Number(rst);
            }
        }
        else
        {
            if (Enum.TryParse(value, out DayOfWeek rstDow))
            {
                return DayOfWeek(rstDow);
            }
            else
            {
                if (Enum.TryParse(value, out MonthOfYear rstMoy))
                {
                    return MonthOfYear(rstMoy);
                }
                else // not a single value type
                {
                    if (value.Contains(Constants.Comma)) // list
                    {
                        valueType = ValueType.List;
                        values = [.. value.Split(Constants.Comma).Select(Parse)];
                    }
                    else if (value.Contains(Constants.Minus)) // range or stepped range
                    {
                        var vals = value.Split(Constants.Minus);
                        if (vals.Length == 2) // valid number of parameters
                        {
                            var startVal = vals[0];
                            var strSteps = vals[1];
                            if (strSteps.Contains(Constants.Slash)) // stepped range
                            {
                                var steps = strSteps.Split(Constants.Slash);

                                if (steps.Length == 2) // valid number of parameters
                                {
                                    values = [Parse(startVal), Parse(steps[0]), Parse(steps[1])];
                                    valueType = ValueType.SteppedRange;
                                }
                            }
                            else // range
                            {
                                values = [Parse(startVal), Parse(strSteps)];
                                valueType = ValueType.Range;
                            }
                        }
                    }
                    else if (value.Contains(Constants.Slash)) // step
                    {
                        var vals = value.Split(Constants.Slash);
                        if (vals.Length == 2)
                        {
                            values = [Parse(vals[0]), Parse(vals[1])];
                            valueType = ValueType.Step;
                        }
                    }
                    else
                    {
                        if (value.Contains(Constants.Hash))
                        {
                            var vals = value.Split(Constants.Hash);
                            if (vals.Length == 2)
                            {
                                values = [Parse(vals[0]), Parse(vals[1])];
                                valueType = ValueType.Symbol_Hash;
                            }
                        }
                        else if (value.Contains(Constants.Last))
                        {
                            var vals = value.Split(Constants.Last);

                        }
                        else if (value.Contains(Constants.Weekday))
                        {
                            var vals = value.Split(Constants.Weekday);

                        }
                        else if (value.Contains(Constants.LastWeekday))
                        {
                            var vals = value.Split(Constants.LastWeekday);

                        }
                    }
                }
            }
        }
        if (valueType == ValueType.Unknown)
        {
            return new ParameterValue([], ValueType.Unknown) { Message = string.Format(Resources.errParameter, value) };
        }
        return new ParameterValue(values, valueType);
    }

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of type <see cref="ValueType.Range"/>.
    /// </summary>
    /// <param name="from">The start value</param>
    /// <param name="to">The end value</param>
    /// <returns>A <see cref="ParameterValue"/></returns>
    public static ParameterValue Range(ParameterValue from, ParameterValue to)
    {
        return new ParameterValue([from, to], ValueType.Range);
    }

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of type <see cref="ValueType.Step"/>.
    /// </summary>
    /// <param name="value">The start value</param>
    /// <param name="increment">The increment</param>
    /// <returns>A <see cref="ParameterValue"/></returns>
    public static ParameterValue Step(ParameterValue value, ParameterValue increment)
    {
        return new ParameterValue([value, increment], ValueType.Step);
    }

    /// <summary>
    /// Returns a <see cref="ParameterValue"/> of type <see cref="ValueType.SteppedRange"/>.
    /// </summary>
    /// <param name="from">The start value</param>
    /// <param name="to">The end value</param>
    /// <param name="increment">The increment</param>
    /// <returns>The <see cref="ParameterValue"/></returns>
    public static ParameterValue SteppedRange(ParameterValue from, ParameterValue to, ParameterValue increment)
    {
        return new ParameterValue([from, to, increment], ValueType.SteppedRange);
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Determines whether this instance and the specified object have the same value.
    /// </summary>
    /// <param name="obj">The object to compare to this instance</param>
    /// <returns><see langword="true"/> if both objects have the same value; else <see langword="false"/></returns>
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is ParameterValue)
        {
            return GetHashCode().Equals(obj.GetHashCode());
        }
        return ToString().Equals(obj.ToString());
    }

    /// <summary>
    /// Returns a hash code for this object.
    /// </summary>
    /// <returns>An <see langword="int"/> value</returns>
    public override int GetHashCode()
    {
        if (OriginalValue == null)
        {
            return Constants.Asterix.GetHashCode();
        }
        return (17 * 31 + ValueType.GetHashCode()) * 31 + OriginalValue.GetHashCode();
    }

    /// <summary>
    /// Overridden to return the symbolic representation of this parameter value.
    /// </summary>
    public override string ToString()
    {
        if (string.IsNullOrEmpty(mExpression))
        {
            mExpression = ValueType switch
            {
                ValueType.Any => Constants.Asterix.ToString(),
                ValueType.Number => Value.ToString(),
                ValueType.DayOfWeek => ((DayOfWeek)Value).ToString(),
                ValueType.List => Values == null ? Constants.Unknown.ToString() : string.Join(Constants.Comma, Values),
                ValueType.MonthOfYear => ((MonthOfYear)Value).ToString(),
                ValueType.Step => string.Format(Constants.fmtStep, mValues[0], mValues[1]),
                ValueType.Range => string.Format(Constants.fmtRange, mValues[0], mValues[1]),
                ValueType.SteppedRange => string.Format(Constants.fmtSteppedRange, mValues[0], mValues[1], mValues[2]),
                ValueType.Symbol_Hash => string.Format(Constants.fmtHash, mValues[0], mValues[1]),
                _ => Constants.Unknown.ToString(), // ParameterValueType.Unknown
            };
        }
        return mExpression;
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Returns all integer values that match this parameter value for the given <see cref="ParameterType"/>.
    /// </summary>
    /// <param name="paramType">The <see cref="ParameterType"/> to match</param>
    /// <returns>A <see cref="List{int}"/></returns>
    internal List<int> AsList(ParameterType paramType)
    {
        return [.. ValueType switch
        {
            ValueType.Any => Enumerable.Range(Constants.MinimumValue[paramType], Constants.MaximumValue[paramType] - Constants.MinimumValue[paramType] + 1),
            ValueType.Number or ValueType.MonthOfYear or ValueType.DayOfWeek => [ Value ],
            ValueType.Step => mValues[0].ValueType == ValueType.Any ? Constants.MinimumValue[paramType].To(Constants.MaximumValue[paramType], mValues[1].Value) : mValues[0].Value.To(Constants.MaximumValue[paramType], mValues[1].Value),
            ValueType.Range => mValues[0].Value <= mValues[1].Value ? Enumerable.Range(mValues[0].Value, mValues[1].Value - Constants.MinimumValue[paramType] + 1) : [],
            ValueType.SteppedRange => mValues[0].Value.To(mValues[1].Value, mValues[2].Value),
            ValueType.List => mValues.SelectMany(v => v.AsList(paramType)).Distinct(),
            ValueType.Symbol_Hash => [ mValues[0].Value ],
            _ => [],
        }];
    }

    #endregion
}