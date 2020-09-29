
''' <summary>
''' String constants and format strings.
''' </summary>
Module Constants

	Friend Const Asterix As String = "*"
	Friend Const Comma As String = ","c
	Friend Const Space As String = " "c

	Friend Const fmtExpression As String = "{0} {1} {2} {3} {4}"
	Friend Const fmtRange As String = "{0}-{1}"
	Friend Const fmtStep As String = "{0}/{1}"

	Friend ReadOnly MaximumValues As New Dictionary(Of ParameterType, Integer) From {{ParameterType.Day, 31}, {ParameterType.Hour, 23}, {ParameterType.Minute, 59}, {ParameterType.Month, 12}, {ParameterType.WeekDay, 6}}
	Friend ReadOnly MinimumValues As New Dictionary(Of ParameterType, Integer) From {{ParameterType.Day, 1}, {ParameterType.Hour, 0}, {ParameterType.Minute, 0}, {ParameterType.Month, 1}, {ParameterType.WeekDay, 0}}
	Friend ReadOnly ValidIntegerCombinations As New List(Of Integer) From {0, 8, 9, 10, 12}
	Friend ReadOnly ValidMonthCombinations As New List(Of Integer) From {16, 17, 18, 20}
	Friend ReadOnly ValidWeekCombinations As New List(Of Integer) From {32, 33, 34, 36}

End Module