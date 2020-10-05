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
	Public Function Validate(parameterType As ParameterType, valueType As ParameterValueType, value As Object) As Boolean
		Dim blOK As Boolean

		If parameterType = ParameterType.Minute Then ' only integers 0 through 59
			Dim rst As Integer

			blOK = ((valueType And ParameterValueType.Value) = ParameterValueType.Value) AndAlso Integer.TryParse(CStr(value), rst) AndAlso (rst >= MinimumValues(parameterType)) AndAlso (rst <= MaximumValues(parameterType))
		ElseIf parameterType = ParameterType.Hour Then  ' only integers 0 through 23
			Dim rst As Integer

			blOK = ((valueType And ParameterValueType.Value) = ParameterValueType.Value) AndAlso Integer.TryParse(CStr(value), rst) AndAlso (rst >= MinimumValues(parameterType)) AndAlso (rst <= MaximumValues(parameterType))
		ElseIf parameterType = ParameterType.Day Then  ' only integers 1 through 31
			Dim rst As Integer

			blOK = ((valueType And ParameterValueType.Value) = ParameterValueType.Value) AndAlso Integer.TryParse(CStr(value), rst) AndAlso (rst >= MinimumValues(parameterType)) AndAlso (rst <= MaximumValues(parameterType))
		ElseIf parameterType = ParameterType.Month Then ' integers 1 through 12 OR JAN through DEC
			If (valueType And ParameterValueType.Value) = ParameterValueType.Value Then
				Dim rst As Integer

				blOK = Integer.TryParse(CStr(value), rst) AndAlso (rst >= MinimumValues(parameterType)) AndAlso (rst <= MaximumValues(parameterType))
			End If
			If Not blOK Then ' may be MonthOfYear value or string value "JAN" etc.
				If (valueType And ParameterValueType.Month) = ParameterValueType.Month Then
					Dim rst As MonthOfYear

					blOK = [Enum].TryParse(value.ToString, rst)
				End If
			End If
		Else ' If parameterType=ParameterType.WeekDay ' integers 0 through 6 OR MON through SUN
			If (valueType And ParameterValueType.Value) = ParameterValueType.Value Then
				Dim rst As Integer

				blOK = Integer.TryParse(CStr(value), rst) AndAlso (rst >= MinimumValues(parameterType)) AndAlso (rst <= MaximumValues(parameterType))
			End If
			If Not blOK Then ' may be DayOfWeek value or string value "MON" etc.
				If (valueType And ParameterValueType.DayOfweek) = ParameterValueType.DayOfweek Then
					Dim rst As DayOfWeek

					blOK = [Enum].TryParse(value.ToString, rst)
				End If
			End If
		End If

		Return blOK

	End Function

	''' <summary>
	''' Returns True if this <see cref="ParameterValueType"/> value represents a single value.
	''' </summary>
	''' <param name="valueType">A <see cref="ParameterValueType"/></param>
	''' <returns>True, if the given value is one of the following: <see cref="ParameterValueType.Value"/>, <see cref="ParameterValueType.Month"/> or <see cref="ParameterValueType.DayOfweek"/></returns>
	<Extension(), DebuggerStepThrough()>
	Public Function IsSingleValueType(valueType As ParameterValueType) As Boolean
		Dim intValueType As Integer = CInt(valueType)

		Return (intValueType = 8 OrElse intValueType = 16 OrElse intValueType = 32)

	End Function

	''' <summary>
	''' Converts the given value to its corresponding integer value.
	''' </summary>
	''' <param name="value">The value to convert</param>
	''' <returns>An integer if parsing was successful, else Null / Nothing</returns>
	<DebuggerStepThrough()>
	Friend Function ToInteger(value As Object) As Integer?
		Dim rst As Integer = -1

		If Not Integer.TryParse(CStr(value), rst) Then ' also parses enum values like. DayOfWeek.MON or MonthOfYear.JAN
			Dim rstDow As DayOfWeek

			If [Enum].TryParse(value.ToString, rstDow) Then
				rst = rstDow
			Else
				Dim rstMoy As MonthOfYear

				If [Enum].TryParse(value.ToString, rstMoy) Then
					rst = rstMoy
				End If
			End If
		End If

		Return If(rst < 0, Nothing, rst)

	End Function



End Module