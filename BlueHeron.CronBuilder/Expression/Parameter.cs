using System.Diagnostics;
using Localization;

namespace BlueHeron.Cron;

/// <summary>
/// Class that represents an <see cref="Expression"/> parameter.
/// </summary>
[DebuggerDisplay("{ParameterType}: {Value}")]
public class Parameter
{
    #region Objects and variables

    private List<int> mMatches = null!;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="Parameter"/>.
    /// </summary>
    /// <param name="paramType">The <see cref="ParameterType"/></param>
    /// <param name="value">The <see cref="ParameterValue"/></param>
    internal Parameter(ParameterType paramType, ParameterValue value)
    {
        ParameterType = paramType;
        Value = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Determines whether this <see cref="Parameter"/> is faulty.
    /// </summary>
    public bool IsFault => Message != null || Value.IsFault;

    /// <summary>
    /// Returns all integer values that match the <see cref="Value"/> for the configured <see cref="ParameterType"/>.
    /// </summary>
    internal IReadOnlyList<int> Matches
    {
        get
        {
            mMatches ??= Value.AsList(ParameterType);
            return mMatches;
        }
    }

    /// <summary>
    /// Gets the error message, if an error occurred.
    /// </summary>
    public string? Message { get; internal set; }

    /// <summary>
    /// Gets the <see cref="Cron.ParameterType"/> of this parameter.
    /// </summary>
    public ParameterType ParameterType { get; }

    /// <summary>
    /// Gets the <see cref="ParameterValue"/>, in use by this parameter.
    /// </summary>
    public ParameterValue Value { get; }

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Creates a new <see cref="Parameter"/>.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterType"/></param>
    /// <param name="valueType">The <see cref="ValueType"/></param>
    /// <param name="values">The constituent values, if needed</param>
    /// <returns></returns>
    public static Parameter Create(ParameterType parameterType, ValueType valueType, IReadOnlyList<ParameterValue>? values = null)
    {
        if (valueType == ValueType.Any)
        {
            return new Parameter(parameterType, ParameterValue.Any());
        }
        else if (values == null || values.Count == 0) // all ValueTypes need values except ValueType.Any
        {
            return new Parameter(parameterType, new ParameterValue(values, valueType)) { Message = new ArgumentNullException(nameof(values)).Message };
        }
        else
        {
            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];

                if (!(value.ValueType.IsSingular() || (valueType == ValueType.Step && i == 0 && value.ValueType == ValueType.Any))) // support */3 steps
                {
                    return new Parameter(parameterType, value) { Message = string.Format(Resources.errParameterValueType, valueType) };
                }
            }
            return valueType switch
            {
                ValueType.List or ValueType.Range or ValueType.Step or ValueType.SteppedRange or ValueType.Symbol_Hash => new Parameter(parameterType, new ParameterValue(values, valueType)),
                ValueType.Number or ValueType.MonthOfYear or ValueType.DayOfWeek => new Parameter(parameterType, values[0]),
                _ => new Parameter(parameterType, ParameterValue.Any()),
            };
        }
    }

    #endregion
}