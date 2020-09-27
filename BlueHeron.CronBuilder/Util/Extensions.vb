Imports System.Runtime.CompilerServices

''' <summary>
''' Extension methods.
''' </summary>
Module Extensions

	''' <summary>
	''' Validates the given value for the given parameter type and value type.
	''' </summary>
	''' <param name="parameterType">A <see cref="ParameterType"/></param>
	''' <param name="valueType">A <see cref="ParameterValueType"/></param>
	''' <param name="value">The value to validate</param>
	''' <returns>True if the given value is valid</returns>
	<Extension>
	Friend Function Validate(parameterType As ParameterType, valueType As ParameterValueType, value As Object) As Boolean
		Dim blOK As Boolean

		If parameterType = ParameterType.Minute Then ' only integers 0 through 59
			Dim rst As Integer

			blOK = ((valueType And ParameterValueType.Value) = ParameterValueType.Value) AndAlso Integer.TryParse(CStr(value), rst) AndAlso (rst >= 0) AndAlso (rst <= 59)
		ElseIf parameterType = ParameterType.Hour Then  ' only integers 0 through 23
			Dim rst As Integer

			blOK = ((valueType And ParameterValueType.Value) = ParameterValueType.Value) AndAlso Integer.TryParse(CStr(value), rst) AndAlso (rst >= 0) AndAlso (rst <= 23)
		ElseIf parameterType = ParameterType.Day Then  ' only integers 1 through 31
			Dim rst As Integer

			blOK = ((valueType And ParameterValueType.Value) = ParameterValueType.Value) AndAlso Integer.TryParse(CStr(value), rst) AndAlso (rst >= 1) AndAlso (rst <= 31)
		ElseIf parameterType = ParameterType.Month Then ' integers 1 through 12 OR JAN through DEC
			If (valueType And ParameterValueType.Value) = ParameterValueType.Value Then
				Dim rst As Integer

				blOK = Integer.TryParse(CStr(value), rst) AndAlso (rst >= 1) AndAlso (rst <= 31)
			End If
			If Not blOK Then ' may be MonthOfYear value
				If (valueType And ParameterValueType.Month) = ParameterValueType.Month Then
					Dim rst As MonthOfYear

					blOK = [Enum].TryParse(value.ToString, rst)
				End If
			End If
		Else ' If parameterType=ParameterType.WeekDay ' integers 0 through 6 OR MON through SUN
			If (valueType And ParameterValueType.Value) = ParameterValueType.Value Then
				Dim rst As Integer

				blOK = Integer.TryParse(CStr(value), rst) AndAlso (rst >= 0) AndAlso (rst <= 6)
			End If
			If Not blOK Then ' may be DayOfWeek value
				If (valueType And ParameterValueType.DayOfweek) = ParameterValueType.DayOfweek Then
					Dim rst As DayOfWeek

					blOK = [Enum].TryParse(value.ToString, rst)
				End If
			End If
		End If

		Return blOK

	End Function

End Module