namespace BlueHeron.Cron;

internal class Constants
{
    internal const char Asterix = '*';
    internal const char Comma = ',';
    internal const char Hash = '#';
    internal const char Last = 'L';
    internal const string LastWeekday = "LW";
    internal const char Minus = '-';
    internal const char Slash = '/';
    internal const char Space = ' ';
    internal const char Unknown = '?';
    internal const char Weekday = 'W';

    internal const string fmtExpression = "{0} {1} {2} {3} {4}";

    internal const string fmtHash = "{0}#{1}";
	internal const string fmtRange = "{0}-{1}";
	internal const string fmtSeparate = "{0}, ";
	internal const string fmtSpaceRight = "{0} ";
	internal const string fmtSpaceBoth = " {0} ";
    internal const string fmtStep = "{0}/{1}";

    internal const string fmtSteppedRange = "{0}-{1}/{2}";
	internal const string fmtTime = "{0:HH:mm} ";
	internal const string fmtTuple = "{0} {1}";
	internal const string fmtTriple = fmtTuple + " {2}";
	internal const string fmtTupleSpacedLeft = " {0} {1}";

    internal static readonly Dictionary<ParameterType, int> MaximumValue = new() { { ParameterType.Day, 31 }, { ParameterType.Hour, 23 }, { ParameterType.Minute, 59 }, { ParameterType.Month, 12 }, { ParameterType.DayOfWeek, 6 } };
    internal static readonly Dictionary<ParameterType, int> MinimumValue = new() { { ParameterType.Day, 1 }, { ParameterType.Hour, 0 }, { ParameterType.Minute, 0 }, { ParameterType.Month, 1 }, { ParameterType.DayOfWeek, 0 } };
}
