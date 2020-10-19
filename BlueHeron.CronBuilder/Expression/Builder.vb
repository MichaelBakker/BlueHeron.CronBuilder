Imports BlueHeron.Cron.Localization

''' <summary>
''' Object that generates and parses Cron expressions.
''' </summary>
Public NotInheritable Class Builder

#Region " Objects and variables "

	Private mHumanizer As IHumanizer
	Private mParameters As Parameter()

#End Region

#Region " Properties "

	''' <summary>
	''' The <see cref="IHumanizer"/> to use to create human-readable, localized representations of <see cref="Expression"/> objects.
	''' </summary>
	''' <returns>An <see cref="IHumanizer"/> implementation</returns>
	Private ReadOnly Property Humanizer As IHumanizer
		Get
			If mHumanizer Is Nothing Then
				mHumanizer = New DefaultHumanizer
			End If
			Return mHumanizer
		End Get
	End Property

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Returns the assembled <see cref="Expression"/>.
	''' </summary>
	''' <param name="validateExpression">If True, parameter values will be validated before returning the <see cref="Expression"/></param>
	''' <returns>An <see cref="Expression"/></returns>
	''' <exception cref="ParserAggregateException">One or more errors occurred while validating this expression</exception>
	Public Function Build(Optional validateExpression As Boolean = True) As Expression
		Dim exceptions As List(Of ParserException) = Nothing

		If (Not validateExpression) OrElse Validate(exceptions) Then
			Dim expression As New Expression(mParameters, Humanizer)

			SetDefaultParameters() ' reset internal parameters to Any

			Return expression
		End If

		Throw New ParserAggregateException(Resources.errAggregateMessage, exceptions)

	End Function

	''' <summary>
	''' Creates a <see cref="Expression" /> from the given string and returns it after validation.
	''' </summary>
	''' <param name="cronExpression">The string representation of a Cron expression</param>
	''' <param name="validateExpression">If True, parameter values will be validated before returning the <see cref="Expression"/></param>
	''' <exception cref="ParserException">The expression contains one or more invalid parameters</exception>
	''' <returns>A <see cref="Expression"/></returns>
	Public Function Build(cronExpression As String, Optional validateExpression As Boolean = True) As Expression
		Dim parts As String() = cronExpression.Split({Space}, StringSplitOptions.RemoveEmptyEntries)

		If parts.Count <> 5 Then
			Throw New ParserException(ParameterType.Minute, ValueType.Unknown, cronExpression, Resources.errParameterCount)
		End If

		For i As Integer = 0 To 4
			[With](CType(i, ParameterType), parts(i))
		Next

		Return Build(validateExpression)

	End Function

	''' <summary>
	''' Configures this <see cref="Builder"/> to use the given <see cref="IHumanizer"/> to create human-readable, localized representations of <see cref="Expression"/> objects.
	''' </summary>
	''' <param name="humanizer">The <see cref="IHumanizer"/> to use</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function Use(humanizer As IHumanizer) As Builder

		mHumanizer = humanizer
		Return Me

	End Function

	''' <summary>
	''' Looks for faults that were not caught upon construction of the parameters and returns false if one or more were found.
	''' </summary>
	''' <param name="exceptions">A <see cref="List(Of Parserexception)"/> with the errors that occurred</param>
	''' <returns>Boolean, True if all <see cref="ParameterValue"/>s are valid</returns>
	Public Function Validate(ByRef exceptions As List(Of ParserException)) As Boolean
		Dim blOK As Boolean = True

		exceptions = New List(Of ParserException)

		For i As Integer = 0 To 4
			Dim p As Parameter = mParameters(i)

			If p.Value.ValueType = ValueType.Any Then
				Continue For
			ElseIf p.Value.ValueType.IsSingleValueType Then
				blOK = ValidateValue(p.ParameterType, p.Value, exceptions)
			Else
				For Each pv As ParameterValue In p.Value.Values
					blOK = ValidateValue(p.ParameterType, pv, exceptions)
				Next
			End If
		Next

		Return blOK

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a <see cref="ValueType.Any"/> parameter
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function WithAny(parameterType As ParameterType) As Builder

		mParameters(parameterType) = New Parameter(parameterType, ParameterValue.Any)
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a list of <see cref="ParameterValue"/>s.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="values">One or more <see cref="ParameterValue"/> objects</param>
	''' <returns>This <see cref="Builder"/></returns>
	''' <exception cref="ParserException">The values parameter is Null / Nothing or contains no <see cref="ParameterValue"/>s</exception>
	Public Function WithList(parameterType As ParameterType, ParamArray values As ParameterValue()) As Builder

		Return WithListInternal(parameterType, values)

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a range parameter.
	''' Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> parameters are allowed.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="fromVal">Start value of the range</param>
	''' <param name="toVal">End value of the range</param>
	''' <returns>This <see cref="Builder"/></returns>
	''' <exception cref="ParserException">One of the values is Null / Nothing or not a single value type</exception>
	Public Function WithRange(parameterType As ParameterType, fromVal As ParameterValue, toVal As ParameterValue) As Builder

		If fromVal Is Nothing Then
			Throw New ParserException(parameterType, ValueType.Range, Unknown, New NullReferenceException(NameOf(fromVal)).Message)
		End If
		If toVal Is Nothing Then
			Throw New ParserException(parameterType, ValueType.Range, Unknown, New NullReferenceException(NameOf(toVal)).Message)
		End If
		If Not fromVal.ValueType.IsSingleValueType Then
			Throw New ParserException(parameterType, fromVal.ValueType, fromVal.OriginalValue.ToString, String.Format(Resources.errParameterValueType, fromVal.ValueType))
		End If
		If Not toVal.ValueType.IsSingleValueType Then
			Throw New ParserException(parameterType, toVal.ValueType, toVal.OriginalValue.ToString, String.Format(Resources.errParameterValueType, toVal.ValueType))
		End If

		mParameters(parameterType) = New Parameter(parameterType, ParameterValue.Range(fromVal, toVal))
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a step or interval parameter.
	''' Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> parameters are allowed.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="startValue">Start value</param>
	''' <param name="increment">Increment value</param>
	''' <returns>This <see cref="Builder"/></returns>
	''' <exception cref="ParserException">One of the values is Null / Nothing or not a single value type</exception>
	Public Function WithStep(parameterType As ParameterType, startValue As ParameterValue, increment As ParameterValue) As Builder

		If startValue Is Nothing Then
			Throw New ParserException(parameterType, ValueType.Step, Unknown, New NullReferenceException(NameOf(startValue)).Message)
		End If
		If increment Is Nothing Then
			Throw New ParserException(parameterType, ValueType.Step, Unknown, New NullReferenceException(NameOf(increment)).Message)
		End If
		If Not (startValue.ValueType = ValueType.Any OrElse startValue.ValueType.IsSingleValueType) Then ' support */3 steps
			Throw New ParserException(parameterType, startValue.ValueType, startValue.OriginalValue.ToString, String.Format(Resources.errParameterValueType, startValue.ValueType))
		End If
		If Not increment.ValueType.IsSingleValueType Then
			Throw New ParserException(parameterType, increment.ValueType, increment.OriginalValue.ToString, String.Format(Resources.errParameterValueType, increment.ValueType))
		End If

		mParameters(parameterType) = New Parameter(parameterType, ParameterValue.Step(startValue, increment))
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a stepped range parameter.
	''' Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> parameters are allowed.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="fromVal">Start value of the range</param>
	''' <param name="toVal">End value of the range</param>
	''' <param name="incrementVal">Increment value</param>
	''' <returns>This <see cref="Builder"/></returns>
	''' <exception cref="ParserException">One of the values is Null / Nothing or not a single value type</exception>
	Public Function WithSteppedRange(parameterType As ParameterType, fromVal As ParameterValue, toVal As ParameterValue, incrementVal As ParameterValue) As Builder

		If fromVal Is Nothing Then
			Throw New ParserException(parameterType, ValueType.SteppedRange, Unknown, New NullReferenceException(NameOf(fromVal)).Message)
		End If
		If toVal Is Nothing Then
			Throw New ParserException(parameterType, ValueType.SteppedRange, Unknown, New NullReferenceException(NameOf(toVal)).Message)
		End If
		If incrementVal Is Nothing Then
			Throw New ParserException(parameterType, ValueType.SteppedRange, Unknown, New NullReferenceException(NameOf(incrementVal)).Message)
		End If
		If Not fromVal.ValueType.IsSingleValueType Then
			Throw New ParserException(parameterType, fromVal.ValueType, fromVal.OriginalValue.ToString, String.Format(Resources.errParameterValueType, fromVal.ValueType))
		End If
		If Not toVal.ValueType.IsSingleValueType Then
			Throw New ParserException(parameterType, toVal.ValueType, toVal.OriginalValue.ToString, String.Format(Resources.errParameterValueType, toVal.ValueType))
		End If
		If Not incrementVal.ValueType.IsSingleValueType Then
			Throw New ParserException(parameterType, incrementVal.ValueType, incrementVal.OriginalValue.ToString, String.Format(Resources.errParameterValueType, incrementVal.ValueType))
		End If

		mParameters(parameterType) = New Parameter(parameterType, ParameterValue.SteppedRange(fromVal, toVal, incrementVal))
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a single value parameter.
	''' Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> are allowed.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="value">The <see cref="ParameterValue"/> to set</param>
	''' <returns>This <see cref="Builder"/></returns>
	''' <exception cref="ParserException">The value is Null / Nothing or not a single value type</exception>
	Public Function WithValue(parameterType As ParameterType, value As ParameterValue) As Builder

		If value Is Nothing Then
			Throw New ParserException(parameterType, ValueType.Step, Unknown, New NullReferenceException(NameOf(value)).Message)
		End If
		If Not value.ValueType.IsSingleValueType Then
			Throw New ParserException(parameterType, value.ValueType, value.OriginalValue.ToString, String.Format(Resources.errParameterValueType, value.ValueType))
		End If

		mParameters(parameterType) = New Parameter(parameterType, value)
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to the parameter, represented by the given string.
	''' If the value cannot be parsed into a recognized <see cref="ParameterValue"/> an exception is thrown.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="value">String representation of a parameter value</param>
	''' <returns>This <see cref="Builder"/></returns>
	''' <exception cref="ParserException">The value is invalid</exception>
	Public Function [With](parameterType As ParameterType, value As String) As Builder

		If value = Asterix Then
			Return WithAny(parameterType)
		End If

		Dim paramValue As New ParameterValue(value)

		If paramValue.ValueType = ValueType.Unknown Then
			Throw New ParserException(parameterType, paramValue.ValueType, paramValue.OriginalValue.ToString, String.Format(Resources.errParameter, value))
		ElseIf paramValue.ValueType = ValueType.List Then
			Return WithListInternal(parameterType, paramValue.Values)
		ElseIf paramValue.ValueType = ValueType.Range Then
			Return WithRange(parameterType, paramValue.Values(0), paramValue.Values(1))
		ElseIf paramValue.ValueType = ValueType.Step Then
			Return WithStep(parameterType, paramValue.Values(0), paramValue.Values(1))
		ElseIf paramValue.ValueType = ValueType.SteppedRange Then
			Return WithSteppedRange(parameterType, paramValue.Values(0), paramValue.Values(1), paramValue.Values(2))
		Else ' Number, DayOfWeek or MonthOfYear
			Return WithValue(parameterType, paramValue)
		End If

	End Function

#End Region

#Region " Private methods and functions "

	''' <summary>
	''' Sets all parameters to Any.
	''' </summary>
	Private Sub SetDefaultParameters()

		mParameters = {New Parameter(ParameterType.Minute, ParameterValue.Any), New Parameter(ParameterType.Hour, ParameterValue.Any), New Parameter(ParameterType.Day, ParameterValue.Any), New Parameter(ParameterType.Month, ParameterValue.Any), New Parameter(ParameterType.WeekDay, ParameterValue.Any)}

	End Sub

	''' <summary>
	''' Validates the given <see cref="ParameterValue"/> for the given <see cref="ParameterType"/>.
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/> of the <see cref="Parameter"/> that holds this value</param>
	''' <param name="pv">The <see cref="ParameterValue"/> to validate</param>
	''' <param name="exceptions">Any <see cref="ParserException"/> that occurs will be added</param>
	''' <returns>True, if the value is valid for this <see cref="ParameterType"/></returns>
	Private Function ValidateValue(paramType As ParameterType, pv As ParameterValue, ByRef exceptions As List(Of ParserException)) As Boolean
		Dim blOK As Boolean = True

		If pv.ValueType = ValueType.Unknown Then ' look for ParameterValueType.Unknown
			blOK = False
			exceptions.Add(New ParserException(paramType, ValueType.Unknown, pv.OriginalValue.ToString, String.Format(Resources.errParameter, pv.OriginalValue)))
		End If
		If Not paramType = ParameterType.Month Then
			If pv.ValueType = ValueType.MonthOfYear Then ' look for MonthOfYear in the wrong places
				blOK = False
				exceptions.Add(New ParserException(paramType, ValueType.MonthOfYear, pv.OriginalValue.ToString, String.Format(Resources.errParameterValueType, ValueType.MonthOfYear)))
			End If
		End If
		If Not paramType = ParameterType.WeekDay Then
			If pv.ValueType = ValueType.DayOfWeek Then ' look for DayOfWeek in the wrong places
				blOK = False
				exceptions.Add(New ParserException(paramType, ValueType.DayOfWeek, pv.OriginalValue.ToString, String.Format(Resources.errParameterValueType, ValueType.DayOfWeek)))
			End If
		End If

		Return blOK

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a list of <see cref="ParameterValue"/>s.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="values">An <see cref="IEnumerable(Of ParameterValue)"/> objects</param>
	''' <returns>This <see cref="Builder"/></returns>
	''' <exception cref="ParserException">The values parameter is Null / Nothing or contains no <see cref="ParameterValue"/>s</exception>
	Private Function WithListInternal(parameterType As ParameterType, values As IEnumerable(Of ParameterValue)) As Builder

		If values Is Nothing OrElse values.Count = 0 Then
			Throw New ParserException(parameterType, ValueType.List, Unknown, New NullReferenceException(NameOf(values)).Message)
		End If

		mParameters(parameterType) = New Parameter(parameterType, ParameterValue.List(values))
		Return Me

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronBuilder, defaulting to a '* * * * *' expression.
	''' </summary>
	Public Sub New()

		SetDefaultParameters()

	End Sub

#End Region

End Class