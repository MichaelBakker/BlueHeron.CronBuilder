
''' <summary>
''' Enumeration of possible parts of a Cron expression.
''' </summary>
Public Enum ParameterType
	''' <summary>
	''' The parameter describes the minute part of the expression.
	''' </summary>
	Minute = 0
	''' <summary>
	''' The parameter describes the hour part of the expression.
	''' </summary>
	Hour = 1
	''' <summary>
	''' The parameter describes the day (of the month) part of the expression.
	''' </summary>
	Day = 2
	''' <summary>
	''' The parameter describes the month part of the expression.
	''' </summary>
	Month = 3
	''' <summary>
	''' The parameter describes the day (of the week) part of the expression.
	''' </summary>
	WeekDay = 4
End Enum

''' <summary>
''' Enumeration of possible parameter value types.
''' Valid values:
''' <list type="bullet">
''' <item>0:	Any</item>
''' <item>8:	A single integer value</item>
''' <item>9:	List of integers</item>
''' <item>10:	Range of integers</item>
''' <item>12:	Step of integers</item>
''' <item>16:	A single <see cref="MonthOfYear"/> value</item>
''' <item>17:	List of <see cref="MonthOfYear"/> values</item>
''' <item>18:	Range of <see cref="MonthOfYear"/> values</item>
''' <item>20:	Step of <see cref="MonthOfYear"/> values</item>
''' <item>32:	A single <see cref="DayOfWeek"/> value</item>
''' <item>33:	List of <see cref="DayOfWeek"/> values</item>
''' <item>34:	Range of <see cref="DayOfWeek"/> values</item> 
''' <item>36:	Step of <see cref="DayOfWeek"/> values</item>
''' </list>
''' <hr />
''' Valid combinations:
''' <list type="bullet">
''' <item>Minute:	0,8,9,10,12</item>
''' <item>Hour:		0,8,9,10,12</item>
''' <item>Day:		0,8,9,10,12</item>
''' <item>Month:	0,8,9,10,12,16,17,18,20</item>
''' <item>WeekDay:	0,8,9,10,12,32,33,34,36</item>
''' </list>
''' </summary>
<Flags>
Public Enum ParameterValueType
	''' <summary>
	''' No value specified, meaning all values are a match.
	''' </summary>
	Any = 0
	''' <summary>
	''' The parameter is a list of distinct values.
	''' May be combined with the <see cref="ParameterValueType.Month"/> or <see cref="ParameterValueType.DayOfweek"/> parameter types.
	''' </summary>
	List = 1
	''' <summary>
	''' The parameter is a range of values.
	''' May be combined with the <see cref="ParameterValueType.Month"/> or <see cref="ParameterValueType.DayOfweek"/> parameter types.
	''' </summary>
	Range = 2
	''' <summary>
	''' The parameter consists of a start value and a step value.
	''' May be combined with the <see cref="ParameterValueType.Month"/> or <see cref="ParameterValueType.DayOfweek"/> parameter types.
	''' </summary>
	[Step] = 4
	''' <summary>
	''' The parameter consists of a single value.
	''' May be combined with the <see cref="ParameterValueType.Month"/> or <see cref="ParameterValueType.DayOfweek"/> parameter types.
	''' </summary>
	Value = 8
	''' <summary>
	''' The parameter is a <see cref="MonthOfYear"/> value.
	''' May be combined with the <see cref="ParameterValueType.List"/>,<see cref="ParameterValueType.Range"/>,<see cref="ParameterValueType.Step"/>, or <see cref="ParameterValueType.Value"/> parameter types.
	''' </summary>
	Month = 16
	''' <summary>
	''' The parameter is a <see cref="DayOfWeek"/> value.
	''' May be combined with the <see cref="ParameterValueType.List"/>,<see cref="ParameterValueType.Range"/>,<see cref="ParameterValueType.Step"/>, or <see cref="ParameterValueType.Value"/> parameter types.
	''' </summary>
	DayOfweek = 32
End Enum

''' <summary>
''' Enumeration of possible months in a year.
''' </summary>
Public Enum MonthOfYear
	''' <summary>
	''' January.
	''' </summary>
	JAN = 1
	''' <summary>
	''' February.
	''' </summary>
	FEB = 2
	''' <summary>
	''' March.
	''' </summary>
	MAR = 3
	''' <summary>
	''' April.
	''' </summary>
	APR = 4
	''' <summary>
	''' May.
	''' </summary>
	MAY = 5
	''' <summary>
	''' June.
	''' </summary>
	JUN = 6
	''' <summary>
	''' July.
	''' </summary>
	JUL = 7
	''' <summary>
	''' August.
	''' </summary>
	AUG = 8
	''' <summary>
	''' September.
	''' </summary>
	SEP = 9
	''' <summary>
	''' October.
	''' </summary>
	OCT = 10
	''' <summary>
	''' November.
	''' </summary>
	NOV = 11
	''' <summary>
	''' December.
	''' </summary>
	DEC = 12
End Enum

''' <summary>
''' Enumeration of possible days in the week.
''' </summary>
Public Enum DayOfWeek
	''' <summary>
	''' Monday.
	''' </summary>
	MON = 1
	''' <summary>
	''' Tuesday.
	''' </summary>
	TUE = 2
	''' <summary>
	''' Wednesday.
	''' </summary>
	WED = 3
	''' <summary>
	''' Thursday.
	''' </summary>
	THU = 4
	''' <summary>
	''' Friday.
	''' </summary>
	FRI = 5
	''' <summary>
	''' Saturday.
	''' </summary>
	SAT = 6
	''' <summary>
	''' Sunday.
	''' </summary>
	SUN = 0
End Enum