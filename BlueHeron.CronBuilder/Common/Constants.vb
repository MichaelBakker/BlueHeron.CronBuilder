
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
	Friend Const fmtMonth As String = "{0:MMMM}"
	Friend Const fmtRange As String = "{0}-{1}"
	Friend Const fmtStep As String = "{0}/{1}"
	Friend Const fmtSteppedRange As String = "{0}-{1}/{2}"
	Friend Const fmtTriple As String = "{0} {1} {2}"
	Friend Const fmtTuple As String = "{0} {1}"

	Friend ReadOnly MaximumValues As New Dictionary(Of ParameterType, Integer) From {{ParameterType.Day, 31}, {ParameterType.Hour, 23}, {ParameterType.Minute, 59}, {ParameterType.Month, 12}, {ParameterType.WeekDay, 6}}
	Friend ReadOnly MinimumValues As New Dictionary(Of ParameterType, Integer) From {{ParameterType.Day, 1}, {ParameterType.Hour, 0}, {ParameterType.Minute, 0}, {ParameterType.Month, 1}, {ParameterType.WeekDay, 0}}

End Module