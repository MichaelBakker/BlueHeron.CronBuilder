' The MIT License (MIT)
' 
' Copyright (c) 2020 Michael Bakker
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.

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
	''' The parameter describes the day part of the expression.
	''' </summary>
	Day = 2
	''' <summary>
	''' The parameter describes the month part of the expression.
	''' </summary>
	Month = 3
	''' <summary>
	''' The parameter describes the day-of-week part of the expression.
	''' </summary>
	DayOfWeek = 4
End Enum

''' <summary>
''' Possible ways to 'humanize' a value.
''' </summary>
Public Enum ValueDisplayType
	''' <summary>
	''' Only the value is returned.
	''' </summary>
	ValueOnly = 0
	''' <summary>
	''' The value is returned, followed by the value type (e.g.: 2 days, 2 months).
	''' </summary>
	Prefix = 1
	''' <summary>
	''' The value is returned, preceded by the value type ( (e.g.: day 2, february).
	''' </summary>
	Postfix = 2
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
	''' The parameter is a hash symbol, i.e. '#'.
	''' </summary>
	Symbol_Hash = 8
	''' <summary>
	''' The parameter is a Last symbol, i.e. 'L'.
	''' </summary>
	Symbol_Last = 9
	''' <summary>
	''' The parameter is a Weekday symbol, i.e. 'W'
	''' </summary>
	Symbol_WeekDay = 10
	''' <summary>
	''' The parameter is a combination of a Last and a Weekday symbol, i.e. 'LW'.
	''' </summary>
	Symbol_LastWeekDay = 11
	''' <summary>
	''' The value could not be determined.
	''' </summary>
	Unknown = 12
End Enum