Imports BlueHeron.Cron.Localization

''' <summary>
''' The <see cref="IParameterFactory"/> that is used to create <see cref="Parameter"/> and <see cref="ParameterValue"/> objects by default.
''' </summary>
Public NotInheritable Class DefaultParameterFactory
	Implements IParameterFactory

#Region " Objects and variables "

	Private mOptions As BuildOptions

#End Region

#Region " Properties "

	''' <summary>
	''' The <see cref="BuildOptions"/> that is used.
	''' If not set through <see cref="Builder.Use(BuildOptions)"/>, then <see cref="BuildOptions.Default"/> is used.
	''' </summary>
	''' <returns>A <see cref="BuildOptions"/> object</returns>
	Public Property BuildOptions As BuildOptions Implements IParameterFactory.BuildOptions
		Get
			If mOptions Is Nothing Then
				mOptions = BuildOptions.Default
			End If
			Return mOptions
		End Get
		Set(value As BuildOptions)
			mOptions = value
		End Set
	End Property

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Creates a new <see cref="Parameter"/>.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/></param>
	''' <param name="valueType">The <see cref="ValueType"/></param>
	''' <param name="values">Array of one or more <see cref="ParameterValue"/> objects</param>
	''' <returns>A <see cref="Parameter"/></returns>
	Friend Function CreateParameter(parameterType As ParameterType, valueType As ValueType, values As IEnumerable(Of ParameterValue)) As Parameter Implements IParameterFactory.CreateParameter

		If valueType = ValueType.Any Then
			Return New Parameter(parameterType, CreateParameterValue)
		ElseIf values Is Nothing OrElse values.Count = 0 Then
			Return New Parameter(parameterType, CreateParameterValue(values, valueType)) With {.Message = New ArgumentNullException(NameOf(values)).Message}
		Else
			For i As Integer = 0 To values.Count - 1
				If Not (values(i).ValueType.IsSingleValueType OrElse (valueType = ValueType.Step AndAlso i = 0 AndAlso values(i).ValueType = ValueType.Any)) Then ' support */3 steps
					Return New Parameter(parameterType, values(i)) With {.Message = String.Format(Resources.errParameterValueType, valueType)}
				End If
			Next
			Select Case valueType
				Case ValueType.List, ValueType.Range, ValueType.Step, ValueType.SteppedRange, ValueType.Symbol_Hash
					Return New Parameter(parameterType, CreateParameterValue(values, valueType))
				Case ValueType.Number, ValueType.MonthOfYear, ValueType.DayOfWeek
					Return New Parameter(parameterType, values(0))
				Case Else ' default to Any
					Return New Parameter(parameterType, CreateParameterValue)
			End Select
		End If

	End Function

	''' <summary>
	''' The value 'Any' (i.e. '*').
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/> of <see cref="ValueType.Any"/></returns>
	Friend Function CreateParameterValue() As ParameterValue Implements IParameterFactory.CreateParameterValue

		Return New ParameterValue

	End Function

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of a <see cref="Cron.ValueType"/> that is determined when parsing the value.
	''' If the value cannot be parsed, <see cref="Cron.ValueType.Unknown"/> is assigned.
	''' </summary>
	''' <param name="value">The value</param>
	Friend Function CreateParameterValue(value As String) As ParameterValue Implements IParameterFactory.CreateParameterValue
		Dim valueType As ValueType = ValueType.Unknown
		Dim values As IEnumerable(Of ParameterValue) = Nothing
		Dim rst As Integer

		If Integer.TryParse(value, rst) Then
			If rst >= 0 Then
				Return CreateParameterValue(rst)
			End If
		Else
			Dim rstDow As DayOfWeek

			If [Enum].TryParse(value.ToString, rstDow) Then
				Return CreateParameterValue(rstDow)
			Else
				Dim rstMoy As MonthOfYear

				If [Enum].TryParse(value.ToString, rstMoy) Then
					Return CreateParameterValue(rstMoy)
				Else 'not a single value type
					If value.IndexOf(Comma) <> -1 Then
						valueType = ValueType.List

						values = value.Split(Comma).Select(Function(v) CreateParameterValue(v)).ToList ' .ToList to prevent multiple enumerations (and CreateparameterValue(...) calls)
					ElseIf value.IndexOf(Minus) <> -1 Then
						Dim vals As String() = value.Split(Minus)

						If vals.Count = 2 Then
							If vals(1).IndexOf(Slash) <> -1 Then
								Dim steps As String() = vals(1).Split(Slash)

								values = {CreateParameterValue(vals(0)), CreateParameterValue(steps(0)), CreateParameterValue(steps(1))}
								valueType = ValueType.SteppedRange
							Else
								valueType = ValueType.Range
								values = {CreateParameterValue(vals(0)), CreateParameterValue(vals(1))}
							End If
						End If
					ElseIf value.IndexOf(Slash) <> -1 Then
						Dim vals As String() = value.Split(Slash)

						If vals.Count = 2 Then
							valueType = ValueType.Step
							values = {CreateParameterValue(vals(0)), CreateParameterValue(vals(1))}
						End If
					ElseIf BuildOptions.SupportSymbols Then
						If value.IndexOf(Hash) <> -1 Then
							Dim vals As String() = value.Split(Hash)

							If vals.Count = 2 Then
								valueType = ValueType.Symbol_Hash
								values = {CreateParameterValue(vals(0)), CreateParameterValue(vals(1))}
							End If
						ElseIf value.IndexOf(Last) <> -1 Then
							Dim vals As String() = value.Split(Last)


						ElseIf value.IndexOf(Weekday) <> -1 Then
							Dim vals As String() = value.Split(Weekday)


						ElseIf value.IndexOf(LastWeekday) <> -1 Then
#If NETSTANDARD2_0 Or NETFRAMEWORK Then
							Dim vals As String() = value.Split(LastWeekday.ToCharArray)
#Else
							Dim vals As String() = value.Split(LastWeekday)
#End If


						End If
					End If
				End If
			End If
		End If

		If valueType = ValueType.Unknown Then
			Return New ParameterValue(Array.Empty(Of ParameterValue), ValueType.Unknown) With {.Message = String.Format(Resources.errParameter, value)}
		End If

		Return CreateParameterValue(values, valueType)

	End Function

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Number"/>.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/> of <see cref="ValueType.Number"/></returns>
	Friend Function CreateParameterValue(value As Integer) As ParameterValue Implements IParameterFactory.CreateParameterValue

		Return New ParameterValue(value, ValueType.Number)

	End Function

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.DayOfWeek"/>.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/> of <see cref="ValueType.DayOfWeek"/></returns>
	Friend Function CreateParameterValue(value As DayOfWeek) As ParameterValue Implements IParameterFactory.CreateParameterValue

		Return New ParameterValue(value, ValueType.DayOfWeek)

	End Function

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.MonthOfYear"/>.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/> of <see cref="ValueType.MonthOfYear"/></returns>
	Friend Function CreateParameterValue(value As MonthOfYear) As ParameterValue Implements IParameterFactory.CreateParameterValue

		Return New ParameterValue(value, ValueType.MonthOfYear)

	End Function

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>, <see cref="Cron.ValueType.Step"/>, <see cref="Cron.ValueType.SteppedRange"/> or <see cref="Cron.ValueType.List"/>.
	''' </summary>
	''' <param name="values">The parameter values</param>
	''' <param name="valueType">The <see cref="Cron.ValueType"/></param>
	''' <returns>A <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>, <see cref="Cron.ValueType.Step"/>, <see cref="Cron.ValueType.SteppedRange"/> or <see cref="Cron.ValueType.List"/></returns>
	Friend Function CreateParameterValue(values As IEnumerable(Of ParameterValue), valueType As ValueType) As ParameterValue Implements IParameterFactory.CreateParameterValue

		Return New ParameterValue(values, valueType)

	End Function

#End Region

End Class