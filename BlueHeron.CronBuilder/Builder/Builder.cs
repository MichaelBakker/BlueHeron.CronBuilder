using BlueHeron.Cron.Localization;
using Localization;

namespace BlueHeron.Cron;

// TODO 1: Support L, W
// TODO 2: Handle DST
// TODO 3: Support seconds and years
// TODO 4: Handle unreachable patterns (>28yr, hash symbol)
// TODO 5: Matches -> Enumerable to HashSet

/// <summary>
/// Object that generates and parses Cron expressions.
/// </summary>
public sealed class Builder
{
    #region Objects and variables

    private IHumanizer mHumanizer = null!;
    private List<Parameter> mParameters;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new CronBuilder, defaulting to a '* * * * *' expression.
    /// </summary>
    public Builder()
    {
        mParameters = DefaultParameters;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the default parameters.
    /// </summary>
    private static List<Parameter> DefaultParameters => [new Parameter(ParameterType.Minute, ParameterValue.Any()), new Parameter(ParameterType.Hour, ParameterValue.Any()), new Parameter(ParameterType.Day, ParameterValue.Any()), new Parameter(ParameterType.Month, ParameterValue.Any()), new Parameter(ParameterType.DayOfWeek, ParameterValue.Any())];

    /// <summary>
    /// Gets the <see cref="IHumanizer"/> to use to create human-readable, localized representations of <see cref="Expression"/> objects.
    /// </summary>
    private IHumanizer Humanizer
    {
        get
        {
            mHumanizer ??= new DefaultHumanizer();
            return mHumanizer;
        }
    }

    /// <summary>
    /// Returns the current <see cref="Parameter"/> collection as <see cref="IReadOnlyList{Parameter}"/>.
    /// </summary>
    public IReadOnlyList<Parameter> Parameters => mParameters.AsReadOnly();

    /// <summary>
	/// Configure the <see cref="Builder"/> to support the following symbols:
	/// <list type="table">
	/// <listheader><term>Symbol</term><description>Usage</description></listheader>
	/// <item><term>L</term><description>Used differently in each of the two fields in which it is allowed:
	/// In the <see cref="ParameterType.Day"/> field, L selects the last day of the month, which is 31 for January and 29 for February on leap years.
	/// When used in the <see cref="ParameterType.DayOfWeek"/> field by itself, it means Saturday. But if used in the <see cref="ParameterType.DayOfWeek"/> field after another value, L selects the last xx day of the month. For example, 6L selects the last Friday of the month.
	/// When using the L special character, do not specify lists or ranges of values, because this may give confusing results.</description></item>
	/// <item><term>W</term><description>Used to specify the weekday (Monday-Friday) nearest to the given day.
	/// For example, if you specify 15W as the value for the <see cref="ParameterType.Day"/> field, the nearest weekday to the 15th Of the month is selected.
	/// So if the 15th is a Saturday, Friday the 14th is selected. If the 15th is a Sunday, Monday the 16th is selected.
	/// If the 15th is a Tuesday, Tuesday the 15th is selected.
	/// However if you specify 1W as the value for <see cref="ParameterType.Day"/>, and the 1st is a Saturday, Monday the 3rd is selected, as the selection rules do not allow for crossing over the boundary of a month's days to the previous or the subsequent month.
	/// The W character can only be used to specify a single day, not a range or list of days.</description></item>
	/// <item><term>#</term><description>Used to specify the nth XXX (or XX) day of the month.
	/// For example, the value FRI#3 or 6#3 in the <see cref="ParameterType.DayOfWeek"/> field means the third Friday of the month (6 or FRI = Friday, and #3 = the 3rd occurrence in the month).</description></item>
	/// </list>
	/// Note: Not all libraries or systems support all of the symbols mentioned above.
	/// E.g.: The Hangfire JobServer doesn't support any of these symbols.
	/// </summary>
	/// <remarks>Default: <see langword="false"/></remarks>
    public bool SupportSymbols { get; set; }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Returns the assembled <see cref="Expression"/> and resets the builder.
    /// </summary>
    /// <returns>An <see cref="Expression"/></returns>
    /// <exception cref="AggregateException">One or more errors occurred while validating this expression</exception>
    public Expression Build()
    {
        if (Validate())
        {
            var expression = new Expression([.. mParameters], Humanizer);

            mParameters = DefaultParameters; // reset internal parameters to Any
            return expression;
        }
        throw new AggregateException(Resources.errAggregateMessage, mParameters.Where(p => p.IsFault).Select(p => new Exception(p.Message)));
    }

    /// <summary>
    /// Creates an <see cref="Expression"/> from the given string representation.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public Expression Build(string expression)
    {
        var parts = expression.Trim().Split(Constants.Space, StringSplitOptions.RemoveEmptyEntries); // handle double spaces and whitespace
        var cnt = parts.Length;

        if (cnt == 5)
        {
            for (var i = 0; i < cnt; i++)
            {
                With((ParameterType)i, parts[i]);
            }
        }
        else
        {
            mParameters = DefaultParameters; // reset internal parameters to Any
            for (var i = 0; i < 5; i++)
            {
                mParameters[i].Message = string.Format(Resources.errParameterCount, expression);
            }
        }
        return Build();
    }

    /// <summary>
    ///  Configures this <see cref="Builder"/> to use the given <see cref="IHumanizer"/> to create human-readable, localized representations of <see cref="Expression"/> objects.
    /// </summary>
    /// <param name="humanizer">The <see cref="IHumanizer"/> to use</param>
    /// <returns>This <see cref="Builder"/></returns>
    public Builder Use(IHumanizer humanizer)
    {
        mHumanizer = humanizer;
        return this;
    }

    /// <summary>
    /// Looks for faults that were not caught upon construction of the parameters and returns false if one or more were found.
    /// </summary>
    /// <returns><see langword="true"/> if all <see cref="ParameterValue"/>s are valid, else <see langword="false"/></returns>
    public bool Validate()
    {
        var blOk = true;

        for (var i = 0; i < 5; i++)
        {
            var p = mParameters[i];

            if (p.Value.ValueType == ValueType.Any)
            {
                continue;
            }
            else if (p.Value.ValueType == ValueType.Unknown)
            {
                blOk = false;
            }
            else if (p.Value.ValueType.IsSingular())
            {
                blOk &= p.ParameterType.Validate(p.Value);
            }
            else if (p.Value.ValueType.IsSymbol())
            {
                if (p.Value.ValueType == ValueType.Symbol_Hash)
                {
                    if (p.Value.Values.Count < 2)
                    {
                        blOk = false;
                    }
                    else
                    {
                        blOk &= p.ParameterType.Validate(p.Value.Values[0]);
                        blOk &= p.Value.Values[1].Value > 0 && p.Value.Values[1].Value < 7; // there are between 1 and 6 weeks in a month
                    }
                }
                // TODO: other symbols
            }
            else
            {
                foreach (var pv in p.Value.Values)
                {
                    blOk &= p.ParameterType.Validate(pv);
                }
            }
        }
        return blOk;
    }

    /// <summary>
    /// Validates the given expression without actually building an <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The string representation of a Cron expression</param>
    /// <returns><see langword="true"/> if the expression can be parsed into 5 valid <see cref="ParameterValue"/>s; else <see langword="false"/></returns>
    public bool Validate(string expression)
    {
        var blOk = true;
        var parts = expression.Trim().Split(Constants.Space, StringSplitOptions.RemoveEmptyEntries); // handle double spaces and whitespace

        if (parts.Length != 5)
        {
            blOk = false;
            mParameters = DefaultParameters; // reset internal parameters to 'Any'
            for (var i = 0; i < 5; i++)
            {
                mParameters[i].Message = string.Format(Resources.errParameterCount, expression);
            }
        }
        else
        {
            for (var i = 0; i < 5; i++)
            {
                With((ParameterType)i, parts[i]);

                var p = mParameters[i];
                if (p.IsFault)
                {
                    blOk = false;
                }
            }
            if (blOk)
            {
                blOk = Validate();
            }
            mParameters = DefaultParameters; // reset internal parameters to 'Any'
        }
        return blOk;
    }

    /// <summary>
    /// Configures the parameter of the given <see cref="ParameterType"/> to the parameter, represented by the given string.
    /// If the value cannot be parsed into a recognized <see cref="ParameterValue"/> an exception is thrown.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
    /// <param name="value">String representation of a parameter value</param>
    /// <returns>This <see cref="Builder"/></returns>
    public Builder With(ParameterType parameterType, string value)
    {
        if (value == Constants.Asterix.ToString())
        {
            return WithAny(parameterType);
        }

        var paramValue = ParameterValue.Parse(value);

        if (paramValue.ValueType == ValueType.Unknown)
        {
            mParameters[(int)parameterType] = new Parameter(parameterType, paramValue) { Message = string.Format(Resources.errParameter, value) };
        }
        else if (paramValue.ValueType.IsSingular())
        {
            mParameters[(int)parameterType] = Parameter.Create(parameterType, paramValue.ValueType, [paramValue]);
        }
        else
        {
            mParameters[(int)parameterType] = Parameter.Create(parameterType, paramValue.ValueType, paramValue.Values);
        }
        return this;
    }

    /// <summary>
    /// Configures the parameter of the given <see cref="ParameterType"/> to be a <see cref="ValueType.Any"/> parameter.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
    /// <returns>This <see cref="Builder"/></returns>
    public Builder WithAny(ParameterType parameterType)
    {
        mParameters[(int)parameterType] = Parameter.Create(parameterType, ValueType.Any, null);
        return this;
    }

    /// <summary>
    /// Configures the parameter of the given <see cref="ParameterType"/> to be a list of <see cref="ParameterValue"/>s.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
    /// <param name="values">Array of one or more <see cref="ParameterValue"/> objects</param>
    /// <returns>This <see cref="Builder"/></returns>
    public Builder WithList(ParameterType parameterType, params ParameterValue[] values)
    {
        mParameters[(int)parameterType] = Parameter.Create(parameterType, ValueType.List, values);
        return this;
    }

    /// <summary>
    /// Configures the parameter of the given <see cref="ParameterType"/> to be a range parameter.
    /// Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> parameters are allowed.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
    /// <param name="from">The start value of the range</param>
    /// <param name="to">The end value of the range</param>
    /// <returns>This <see cref="Builder"/></returns>
    public Builder WithRange(ParameterType parameterType, ParameterValue from, ParameterValue to)
    {
        mParameters[(int)parameterType] = Parameter.Create(parameterType, ValueType.Range, [from, to]);
        return this;
    }

    /// <summary>
    /// Configures the parameter of the given <see cref="ParameterType"/> to be a step or interval parameter.
    /// Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> parameters are allowed.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
    /// <param name="startValue">The start value</param>
    /// <param name="increment">The increment</param>
    /// <returns>This <see cref="Builder"/></returns>
    public Builder WithStep(ParameterType parameterType, ParameterValue startValue, ParameterValue increment)
    {
        mParameters[(int)parameterType] = Parameter.Create(parameterType, ValueType.Step, [startValue, increment]);
        return this;
    }

    /// <summary>
    /// Configures the parameter of the given <see cref="ParameterType"/> to be a range parameter.
    /// Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> parameters are allowed.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
    /// <param name="from">The start value of the range</param>
    /// <param name="to">The end value of the range</param>
    /// <param name="increment">The increment</param>
    /// <returns>This <see cref="Builder"/></returns>
    public Builder WithSteppedRange(ParameterType parameterType, ParameterValue from, ParameterValue to, ParameterValue increment)
    {
        mParameters[(int)parameterType] = Parameter.Create(parameterType, ValueType.SteppedRange, [from, to, increment]);
        return this;
    }

    /// <summary>
    /// Configures the parameter of the given <see cref="ParameterType"/> to be a single value parameter.
    /// Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> are allowed.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
    /// <param name="value">The <see cref="ParameterValue"/> to set</param>
    /// <returns>This <see cref="Builder"/></returns>
    public Builder WithValue(ParameterType parameterType, ParameterValue value)
    {
        mParameters[(int)parameterType] = Parameter.Create(parameterType, ValueType.Number, [value]);
        return this;
    }

    #endregion
}