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
''' String constants and format strings.
''' </summary>
Friend Module Constants

	Friend Const Asterix As Char = "*"c
	Friend Const Comma As Char = ","c
	Friend Const Hash As Char = "#"c
	Friend Const Last As Char = "L"c
	Friend Const LastWeekday As String = Last & Weekday
	Friend Const Minus As Char = "-"c
	Friend Const Slash As Char = "/"c
	Friend Const Space As Char = " "c
	Friend Const Unknown As Char = "?"c
	Friend Const Weekday As Char = "W"c

	Friend Const fmtExpression As String = "{0} {1} {2} {3} {4}"
	Friend Const fmtHash As String = "{0}#{1}"
	Friend Const fmtRange As String = "{0}-{1}"
	Friend Const fmtSeparate As String = "{0}, "
	Friend Const fmtSpaceRight As String = "{0} "
	Friend Const fmtSpaceBoth As String = Space & fmtSpaceRight
	Friend Const fmtStep As String = "{0}/{1}"
	Friend Const fmtSteppedRange As String = "{0}-{1}/{2}"
	Friend Const fmtTime As String = "{0:mm:HH} "
	Friend Const fmtTuple As String = "{0} {1}"
	Friend Const fmtTriple As String = fmtTuple & " {2}"
	Friend Const fmtTupleSpacedLeft As String = Space & fmtTuple

	Friend ReadOnly MaximumValue As New Dictionary(Of ParameterType, Integer) From {{ParameterType.Day, 31}, {ParameterType.Hour, 23}, {ParameterType.Minute, 59}, {ParameterType.Month, 12}, {ParameterType.DayOfWeek, 6}}
	Friend ReadOnly MinimumValue As New Dictionary(Of ParameterType, Integer) From {{ParameterType.Day, 1}, {ParameterType.Hour, 0}, {ParameterType.Minute, 0}, {ParameterType.Month, 1}, {ParameterType.DayOfWeek, 0}}

End Module