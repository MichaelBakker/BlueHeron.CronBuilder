
''' <summary>
''' Object that represents an expression parameter value.
''' </summary>
Public NotInheritable Class ParameterValue

#Region " Objects and variables "

	Private mExpression As String
	Private mOriginalValue As Object
	Private mValue As Integer
	Private mValueType As ValueType

#End Region

#Region " Properties "

	''' <summary>
	''' The original value that was used to create this parameter value.
	''' </summary>
	Public ReadOnly Property OriginalValue As Object
		Get
			Return mOriginalValue
		End Get
	End Property

	''' <summary>
	''' This value as an integer. Only used when the <see cref="ValueType"/> is <see cref="ValueType.DayOfWeek"/>, <see cref="ValueType.MonthOfYear"/> or <see cref="ValueType.Number"/>.
	''' </summary>
	Public ReadOnly Property Value As Integer
		Get
			Return mValue
		End Get
	End Property

	''' <summary>
	''' Array of values that together make up this value.
	''' </summary>
	''' <returns>An array of <see cref="ParameterValue"/> objects</returns>
	Public ReadOnly Property Values As IEnumerable(Of ParameterValue)

	''' <summary>
	''' The <see cref="Cron.ValueType"/> of this value.
	''' </summary>
	''' <returns>A <see cref="Cron.ValueType"/> value</returns>
	Public ReadOnly Property ValueType As ValueType
		Get
			Return mValueType
		End Get
	End Property

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Any"/>.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Shared Function Any() As ParameterValue

		Return New ParameterValue

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.DayOfWeek"/>.
	''' </summary>
	''' <param name="value">The value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Shared Function DayOfWeek(value As DayOfWeek) As ParameterValue

		Return New ParameterValue(value)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of  a type that will be determined when parsing the value.
	''' If the value cannot be parsed, <see cref="Cron.ValueType.Unknown"/> is assigned and subsequent validation will fail.
	''' </summary>
	''' <param name="value">The value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Shared Function FromString(value As String) As ParameterValue

		Return New ParameterValue(value)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.MonthOfYear"/>.
	''' </summary>
	''' <param name="value">The value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Shared Function MonthOfYear(value As MonthOfYear) As ParameterValue

		Return New ParameterValue(value)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.List"/>.
	''' </summary>
	''' <param name="values">The values</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Friend Shared Function List(values As IEnumerable(Of ParameterValue)) As ParameterValue

		Return New ParameterValue(values, ValueType.List)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Number"/>.
	''' The validity of the number will be assessed when assigning this value to an expression parameter.
	''' </summary>
	''' <param name="value">The value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Shared Function Number(value As Integer) As ParameterValue

		Return New ParameterValue(value)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>.
	''' </summary>
	''' <param name="valueFrom">The start value</param>
	''' <param name="valueTo">The end value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Shared Function Range(valueFrom As ParameterValue, valueTo As ParameterValue) As ParameterValue

		Return New ParameterValue({valueFrom, valueTo}, ValueType.Range)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Step"/>.
	''' </summary>
	''' <param name="value">The start value</param>
	''' <param name="increment">The increment value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Shared Function [Step](value As ParameterValue, increment As ParameterValue) As ParameterValue

		Return New ParameterValue({value, increment}, ValueType.Step)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.SteppedRange"/>.
	''' </summary>
	''' <param name="valueFrom">The start value</param>
	''' <param name="valueTo">The end value</param>
	''' <param name="increment">The increment value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Shared Function SteppedRange(valueFrom As ParameterValue, valueTo As ParameterValue, increment As ParameterValue) As ParameterValue

		Return New ParameterValue({valueFrom, valueTo, increment}, ValueType.SteppedRange)

	End Function

	''' <summary>
	''' Returns the symbolic representation of this parameter value.
	''' </summary>
	Public Overrides Function ToString() As String

		If String.IsNullOrEmpty(mExpression) Then
			Select Case mValueType
				Case ValueType.Any
					mExpression = Asterix
				Case ValueType.Number
					mExpression = CStr(Value)
				Case ValueType.DayOfWeek
					mExpression = CType(Value, DayOfWeek).ToString
				Case ValueType.List
					mExpression = String.Join(Comma, Values)
				Case ValueType.MonthOfYear
					mExpression = CType(Value, MonthOfYear).ToString
				Case ValueType.Step
					mExpression = String.Format(fmtStep, Values(0), Values(1))
				Case ValueType.Range
					mExpression = String.Format(fmtRange, Values(0), Values(1))
				Case ValueType.SteppedRange
					mExpression = String.Format(fmtSteppedRange, Values(0), Values(1), Values(2))
				Case Else ' ParameterValueType.Unknown
					mExpression = Unknown
			End Select
		End If

		Return mExpression

	End Function

#End Region

#Region " Private methods and functions "

	''' <summary>
	''' Returns all integer values that match this parameter value for the given <see cref="ParameterType"/>.
	''' </summary>
	''' <returns>An <see cref="IEnumerable(Of Integer)"/></returns>
	Friend Function AsEnumerable(paramType As ParameterType) As IEnumerable(Of Integer)
		Dim mMatches As IEnumerable(Of Integer)
		Dim maxVal As Integer = MaximumValues(paramType)
		Dim minVal As Integer = MinimumValues(paramType)

		Select Case mValueType
			Case ValueType.Any
				mMatches = Enumerable.Range(minVal, maxVal - minVal + 1)
			Case ValueType.Number, ValueType.MonthOfYear, ValueType.DayOfWeek
				mMatches = {Math.Min(maxVal, Math.Max(Value, minVal))}
			Case ValueType.Step
				Dim incr As Integer = Values(1).Value
				Dim val As Integer = Math.Min(maxVal, Math.Max(Values(0).Value, minVal))

				If val <= (maxVal - incr) Then
					mMatches = val.To(maxVal, incr)
				Else
					mMatches = Array.Empty(Of Integer)
				End If
			Case ValueType.Range
				Dim fromVal As Integer = Math.Max(minVal, Values(0).Value)
				Dim toVal As Integer = Math.Min(maxVal, Values(1).Value)

				mMatches = Enumerable.Range(fromVal, toVal - minVal + 1)
			Case ValueType.SteppedRange
				Dim fromVal As Integer = Values(0).Value
				Dim toVal As Integer = Values(1).Value
				Dim incr As Integer = Values(2).Value

				mMatches = Math.Max(minVal, fromVal).To(Math.Min(maxVal, toVal), 1)
				mMatches.Concat(toVal.To(maxVal, incr))
			Case ValueType.List
				mMatches = Values.SelectMany(Function(v) v.AsEnumerable(paramType)).Distinct
			Case Else
				mMatches = Array.Empty(Of Integer)
		End Select

		Return mMatches

	End Function

	''' <summary>
	''' Sets the given fields.
	''' </summary>
	''' <param name="value">The value</param>
	''' <param name="valueType">The <see cref="Cron.ValueType"/></param>
	<DebuggerStepThrough()>
	Private Sub SetValue(value As Integer, valueType As ValueType)

		mOriginalValue = value
		mValueType = valueType
		mValue = value

	End Sub

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Any"/>
	''' </summary>
	<DebuggerStepThrough()> Private Sub New()

		SetValue(-1, ValueType.Any)

	End Sub

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Number"/>.
	''' The validity of the number will be assessed when assigning this value to a parameter.
	''' </summary>
	''' <param name="value">The value</param>
	<DebuggerStepThrough()> Private Sub New(value As Integer)

		SetValue(value, ValueType.Number)

	End Sub

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.DayOfWeek"/>.
	''' </summary>
	''' <param name="value">The value</param>
	<DebuggerStepThrough()> Private Sub New(value As DayOfWeek)

		SetValue(value, ValueType.DayOfWeek)

	End Sub

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.MonthOfYear"/>.
	''' </summary>
	''' <param name="value">The value</param>
	<DebuggerStepThrough()> Private Sub New(value As MonthOfYear)

		SetValue(value, ValueType.MonthOfYear)

	End Sub

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>, <see cref="Cron.ValueType.Step"/>, <see cref="Cron.ValueType.SteppedRange"/> or <see cref="Cron.ValueType.List"/>.
	''' </summary>
	''' <param name="values">The parameter values</param>
	''' <param name="valueType">The <see cref="Cron.ValueType"/></param>
	<DebuggerStepThrough()> Private Sub New(values As IEnumerable(Of ParameterValue), valueType As ValueType)

		mOriginalValue = values
		Me.Values = values
		mValueType = valueType

	End Sub

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of a type that will be determined when parsing the value.
	''' If the value cannot be parsed, <see cref="Cron.ValueType.Unknown"/> is assigned and subsequent validation will fail.
	''' </summary>
	''' <param name="value">The value</param>
	Friend Sub New(value As String)
		Dim rst As Integer

		mValueType = ValueType.Unknown
		If Integer.TryParse(CStr(value), rst) Then
			SetValue(CInt(value), ValueType.Number)
		Else
			Dim rstDow As DayOfWeek

			If [Enum].TryParse(value.ToString, rstDow) Then
				SetValue(rstDow, ValueType.DayOfWeek)
			Else
				Dim rstMoy As MonthOfYear

				If [Enum].TryParse(value.ToString, rstMoy) Then
					SetValue(rstMoy, ValueType.MonthOfYear)
				End If
			End If
		End If

		If mValueType = ValueType.Unknown Then ' list, range, step or steppedrange
			mOriginalValue = value

			If value.IndexOf(Comma) <> -1 Then
				mValueType = ValueType.List

				Values = value.Split(Comma).Select(Function(v) New ParameterValue(v))
			ElseIf value.IndexOf(Minus) <> -1 Then
				Dim vals As String() = value.Split(Minus)

				If vals.Count = 2 Then
					If vals(1).IndexOf(Slash) <> -1 Then
						Dim steps As String() = vals(1).Split(Slash)

						Values = {New ParameterValue(vals(0)), New ParameterValue(steps(0)), New ParameterValue(steps(1))}
						mValueType = ValueType.SteppedRange
					Else
						mValueType = ValueType.Range
						Values = vals.Select(Function(v) New ParameterValue(v))
					End If
				End If
			ElseIf value.IndexOf(Slash) <> -1 Then
				Dim vals As String() = value.Split(Slash)

				If vals.Count = 2 Then
					mValueType = ValueType.Step
					Values = vals.Select(Function(v) New ParameterValue(v))
				End If
			End If
		End If

	End Sub

#End Region

End Class