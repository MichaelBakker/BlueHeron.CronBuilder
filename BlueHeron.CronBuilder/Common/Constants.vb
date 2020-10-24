
''' <summary>
''' String constants and format strings.
''' </summary>
Friend Module Constants

	Friend Const Asterix As Char = "*"c
	Friend Const Comma As Char = ","c
	Friend Const Minus As Char = "-"c
	Friend Const Slash As Char = "/"c
	Friend Const Space As Char = " "c
	Friend Const Unknown As String = "?"c

	Friend Const fmtExpression As String = "{0} {1} {2} {3} {4}"
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