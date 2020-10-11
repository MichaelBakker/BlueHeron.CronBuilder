
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
''' </summary>
Public Enum ValueType
	''' <summary>
	''' No value is specified, meaning any value will match.
	''' </summary>
	Any = 0
	''' <summary>
	''' The parameter is a list of distinct values.
	''' </summary>
	List = 1
	''' <summary>
	''' The parameter is a range of values.
	''' </summary>
	Range = 2
	''' <summary>
	''' The parameter consists of a start value and an increment value.
	''' </summary>
	[Step] = 3
	''' <summary>
	''' The parameter consists of a single whole number equal to or greater than zero.
	''' </summary>
	Number = 4
	''' <summary>
	''' The parameter is a <see cref="Cron.MonthOfYear"/> value.
	''' </summary>
	MonthOfYear = 5
	''' <summary>
	''' The parameter is a <see cref="Cron.DayOfWeek"/> value.
	''' </summary>
	DayOfWeek = 6
	''' <summary>
	''' The parameter is a combination of a range of values and an increment value.
	''' </summary>
	SteppedRange = 7
	''' <summary>
	''' The value could not be determined.
	''' </summary>
	Unknown = 8
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