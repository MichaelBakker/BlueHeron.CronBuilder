
''' <summary>
''' String constants and format strings.
''' </summary>
Module Constants

	Friend Const Asterix As String = "*"
	Friend Const Comma As String = ","c
	Friend Const Space As String = " "c

	Friend Const errArgumentCount As String = "Invalid number of arguments."
	Friend Const errParameterValueType As String = "Invalid ParameterValueType (or combination)."
	Friend Const errParameter As String = "{0} is invalid."
	Friend Const errParameterCount As String = "Invalid number of parameters."
	Friend Const errParameterWithMessage As String = errParameter & " {1}."

	Friend Const fmtExpression As String = "{0} {1} {2} {3} {4}"
	Friend Const fmtRange As String = "{0}-{1}"
	Friend Const fmtStep As String = "{0}/{1}"

	Friend ReadOnly ValidIntegerCombinations As New List(Of Integer) From {0, 8, 9, 10, 12}
	Friend ReadOnly ValidMonthCombinations As New List(Of Integer) From {16, 17, 18, 20}
	Friend ReadOnly ValidWeekCombinations As New List(Of Integer) From {32, 33, 34, 36}

End Module