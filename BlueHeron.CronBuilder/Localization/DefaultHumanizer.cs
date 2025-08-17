using System.Diagnostics;
using System.Text;
using Localization;

namespace BlueHeron.Cron.Localization;

/// <summary>
/// Object that converts an <see cref="Expression"/> into a human-readable, localized representation, using a pattern that probably only works for a few european languages.
/// </summary>
public class DefaultHumanizer : IHumanizer
{
    #region Public methods and functions

    /// <inheritdoc cref="IHumanizer.Humanize(Expression)"/>
    public string Humanize(Expression expression)
    {
        try
        {
            var parameterDisplays = new string[4];
            var prepositions = new string[4];
            var sb = new StringBuilder(256);
            var isAny = false;

            for (var i = 0; i < 5; i++)
            {
                var p = expression.Parameters[i];
                parameterDisplays[i] = GetDisplay(p.Value, p.ParameterType);
                prepositions[i] = GetPreposition(p.ParameterType);
            }

            sb.AppendFormat(Constants.fmtSpaceRight, prepositions[0]);
            if (expression.Parameters[0].Value.ValueType == ValueType.Number && expression.Parameters[1].Value.ValueType == ValueType.Number)
            {
                sb.AppendFormat(Constants.fmtTime, new DateTime(DateTime.MinValue.Year, 1, 1, expression.Parameters[1].Value.Value, expression.Parameters[0].Value.Value, 0));
                prepositions[2] = Resources.of;
            }
            else
            {
                if (!expression.Parameters[0].Value.ValueType.IsSingular())
                {
                    sb.AppendFormat(Constants.fmtSpaceRight, Resources.every);
                }
                isAny = expression.Parameters[0].Value.ValueType == ValueType.Any;
                sb.AppendFormat(Constants.fmtSpaceRight, parameterDisplays[0]);

                if (!expression.Parameters[1].Value.ValueType.IsSingular() && isAny)
                {
                    sb.AppendFormat(Constants.fmtSpaceRight, string.Format(Constants.fmtTriple, prepositions[1], !expression.Parameters[1].Value.ValueType.IsSingular() ? Resources.every : string.Empty, parameterDisplays[1]));
                }
                isAny = expression.Parameters[1].Value.ValueType == ValueType.Any;
            }
            for (var i = 2; i < 5; i++)
            {
                if (!(isAny && expression.Parameters[i].Value.ValueType == ValueType.Any))
                {
                    sb.AppendFormat(Constants.fmtSpaceRight, prepositions[i]);
                    if (!expression.Parameters[i].Value.ValueType.IsSingular())
                    {
                        sb.AppendFormat(Constants.fmtSpaceRight, Resources.every);
                    }
                    sb.AppendFormat(Constants.fmtSpaceRight, parameterDisplays[i]);
                }
                isAny = expression.Parameters[i].Value.ValueType == ValueType.Any;
            }
            return sb.ToString().Trim();
        }
        catch { }

        return Resources.errAggregateMessage;
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Returns a human-readable, localized representation of the given parameter value for the given <see cref="ParameterType"/>.
    /// </summary>
    /// <param name="paramValue">The <see cref="ParameterValue"/></param>
    /// <param name="paramType">The <see cref="ParameterType"/> to which this value belongs</param>
    protected virtual string GetDisplay(ParameterValue paramValue, ParameterType paramType)
    {
        var rst = string.Empty;

        switch (paramValue.ValueType)
        {
            case ValueType.Any:
                rst = paramType.ToDisplay(false);
                break;
            case ValueType.DayOfWeek:
                rst = ((DayOfWeek)paramValue.Value).ToDisplay();
                break;
            case ValueType.MonthOfYear:
                rst = ((MonthOfYear)paramValue.Value).ToDisplay();
                break;
            case ValueType.List:
                var i = 0;
                var length = paramValue.Values.Count;

                foreach (var value in paramValue.Values)
                {
                    i += 1;
                    rst += length > 1 && i == length ? string.Format(Constants.fmtTupleSpacedLeft, Resources.and, GetDisplay(value, paramType, ValueDisplayType.Postfix)) : string.Format(Constants.fmtSeparate, GetDisplay(value, paramType, ValueDisplayType.Postfix));
                }
                rst = rst.TrimEnd([Constants.Space, Constants.Comma]);
                break;
            case ValueType.Number:
                rst = GetDisplay(paramValue, paramType, ValueDisplayType.Postfix);
                break;
            case ValueType.Range:
                rst = string.Format(Constants.fmtTriple, GetDisplay(paramValue.Values[0], paramType, ValueDisplayType.Postfix), Resources.through, GetDisplay(paramValue.Values[1], paramType, ValueDisplayType.ValueOnly));
                break;
            case ValueType.Step:
                var stepVal = GetDisplay(paramValue.Values[1], paramType, ValueDisplayType.Prefix);
                var paramStep = paramValue.Values[0];
                rst = paramStep.ValueType == ValueType.Any ? stepVal : string.Format(Constants.fmtTriple, stepVal, Resources.startingWith, GetDisplay(paramStep, paramType, ValueDisplayType.ValueOnly));
                break;
            case ValueType.SteppedRange:
                rst = string.Format(Constants.fmtTriple, GetDisplay(paramValue.Values[0], paramType, ValueDisplayType.Postfix), Resources.through, GetDisplay(paramValue.Values[1], paramType, ValueDisplayType.ValueOnly)) + string.Format(Constants.fmtSpaceBoth, Resources.andThen) + string.Format(Constants.fmtTriple, GetDisplay(paramValue.Values[2], paramType, ValueDisplayType.Prefix), Resources.startingWith, GetDisplay(paramValue.Values[1], paramType, ValueDisplayType.ValueOnly));
                break;
            case ValueType.Symbol_Hash:

                break;
            default:
                rst = Constants.Unknown.ToString();
                break;
        }
        return rst;
    }

    /// <summary>
    /// Returns a human-readable, localized representation of the given parameter value's <see cref="ParameterValue.Value"/> for the given <see cref="ParameterType"/> and <see cref="ValueDisplayType"/>.
    /// </summary>
    /// <param name="paramValue">The <see cref="ParameterValue"/></param>
    /// <param name="paramType">The <see cref="ParameterType"/> to which this value belongs</param>
    /// <param name="valueDisplayType">The <see cref="ValueDisplayType"/> to use</param>
    protected virtual string GetDisplay(ParameterValue paramValue, ParameterType paramType, ValueDisplayType valueDisplayType)
    {
        var rst = string.Empty;

        switch (valueDisplayType)
        {
            case ValueDisplayType.ValueOnly:
                if (paramType == ParameterType.DayOfWeek || paramValue.ValueType == ValueType.DayOfWeek)
                {
                    rst = ((DayOfWeek)paramValue.Value).ToDisplay();
                }
                else if (paramType == ParameterType.Month || paramValue.ValueType == ValueType.MonthOfYear)
                {
                    rst = ((MonthOfYear)paramValue.Value).ToDisplay();
                }
                else
                {
                    rst = (paramValue.Value >= 0 ? paramValue.Value : Constants.Unknown).ToString();
                }
                break;
            case ValueDisplayType.Prefix:
                rst = paramValue.Value > 1 ? string.Format(Constants.fmtTuple, paramValue.Value, paramType.ToDisplay(true)) : paramType.ToDisplay(false);
                break;
            case ValueDisplayType.Postfix:
                if (paramType == ParameterType.DayOfWeek || paramValue.ValueType == ValueType.DayOfWeek)
                {
                    rst = ((DayOfWeek)paramValue.Value).ToDisplay();
                }
                else if (paramType == ParameterType.Month || paramValue.ValueType == ValueType.MonthOfYear)
                {
                    rst = ((MonthOfYear)paramValue.Value).ToDisplay();
                }
                else
                {
                    rst = string.Format(Constants.fmtTuple, paramType.ToDisplay(false), paramValue.Value);
                }
                break;
        }
        return rst;
    }

    /// <summary>
    /// Returns the appropriate, localized preposition for the given <see cref="ParameterType"/>.
    /// </summary>
	/// <param name="paramType">The <see cref="ParameterType"/></param>
	/// <returns>Localized versions of at, on or in.</returns>
    [DebuggerStepThrough()]
    protected virtual string GetPreposition(ParameterType paramType)
    {
        return paramType switch
        {
            ParameterType.Minute => Resources.atMinute,
            ParameterType.Hour => Resources.of,
            ParameterType.Day => Resources.onDay,
            ParameterType.Month => Resources.inMonth,
            _ => Resources.onDay // ParameterType.DayOfWeek
        };
    }

    #endregion
}