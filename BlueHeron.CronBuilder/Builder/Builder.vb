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

Imports System.Collections.ObjectModel
Imports BlueHeron.Cron.Localization

' TODO 1: Support L, W
' TODO 2: Handle DST
' TODO 3: Support seconds and years
' TODO 4: Handle unreachable patterns (>28yr, hash symbol)
' TODO 5: Matches -> Enumerable to HashSet

''' <summary>
''' Object that generates and parses Cron expressions.
''' </summary>
Public NotInheritable Class Builder

#Region " Objects and variables "

	Private mFactory As IParameterFactory
	Private mHumanizer As IHumanizer

	Private Shared ReadOnly mDefaultParameters As Parameter() = {New Parameter(ParameterType.Minute, New ParameterValue), New Parameter(ParameterType.Hour, New ParameterValue), New Parameter(ParameterType.Day, New ParameterValue), New Parameter(ParameterType.Month, New ParameterValue), New Parameter(ParameterType.DayOfWeek, New ParameterValue)}
	Private ReadOnly mParameters(4) As Parameter

#End Region

#Region " Properties "

	''' <summary>
	''' The <see cref="IParameterFactory"/> to use to create <see cref="Parameter"/> and <see cref="ParameterValue"/> objects.
	''' </summary>
	''' <returns>An <see cref="IParameterFactory"/> implementation</returns>
	Private ReadOnly Property Factory As IParameterFactory
		Get
			If mFactory Is Nothing Then
				mFactory = New DefaultParameterFactory
			End If
			Return mFactory
		End Get
	End Property

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

	''' <summary>
	''' Readonly collection with the current <see cref="Parameter"/>s.
	''' </summary>
	''' <returns>An <see cref="IReadOnlyCollection(Of Parameter)"/></returns>
	Public ReadOnly Property Parameters As IReadOnlyCollection(Of Parameter)
		Get
			Return New ReadOnlyCollection(Of Parameter)(mParameters.ToList)
		End Get
	End Property

#End Region

#Region " Public methods and functions "

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Any"/>.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function Any() As ParameterValue

		Return Factory.CreateParameterValue

	End Function

	''' <summary>
	''' Returns the assembled <see cref="Expression"/>.
	''' </summary>
	''' <returns>An <see cref="Expression"/></returns>
	''' <exception cref="AggregateException">One or more errors occurred while validating this expression</exception>
	Public Function Build() As Expression

		If Validate() Then
			Dim expression As New Expression(DirectCast(mParameters.Clone, Parameter()), Humanizer)

			mDefaultParameters.CopyTo(mParameters, 0) ' reset internal parameters to Any

			Return expression
		End If

		Throw New AggregateException(Resources.errAggregateMessage, mParameters.Select(Function(p) New Exception(p.Message)))

	End Function

	''' <summary>
	''' Creates an <see cref="Expression" /> from the given string.
	''' </summary>
	''' <param name="expression">The string representation of a Cron expression</param>
	''' <exception cref="AggregateException">The expression contains one or more invalid parameters</exception>
	''' <returns>An <see cref="Expression"/></returns>
	Public Function Build(expression As String) As Expression
		Dim parts As String() = expression.Trim().Split({Space}, StringSplitOptions.RemoveEmptyEntries) ' handle double spaces and whitespace

		If parts.Count <> 5 Then
			mDefaultParameters.CopyTo(mParameters, 0) ' reset internal parameters to Any
			For i As Integer = 0 To 4
				mParameters(i).Message = String.Format(Resources.errParameterCount, expression)
			Next
		Else
			For i As Integer = 0 To 4
				[With](CType(i, ParameterType), parts(i))
			Next
		End If

		Return Build()

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.DayOfWeek"/>.
	''' </summary>
	''' <param name="value">The value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function DayOfWeek(value As DayOfWeek) As ParameterValue

		Return Factory.CreateParameterValue(value)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.List"/>.
	''' </summary>
	''' <param name="values">The values</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function List(values As IEnumerable(Of ParameterValue)) As ParameterValue

		Return Factory.CreateParameterValue(values, ValueType.List)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.MonthOfYear"/>.
	''' </summary>
	''' <param name="value">The value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function MonthOfYear(value As MonthOfYear) As ParameterValue

		Return Factory.CreateParameterValue(value)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Number"/>.
	''' The validity of the number will be assessed when assigning this value to an expression parameter.
	''' </summary>
	''' <param name="value">The value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function Number(value As Integer) As ParameterValue

		Return Factory.CreateParameterValue(value)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>.
	''' </summary>
	''' <param name="valueFrom">The start value</param>
	''' <param name="valueTo">The end value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function Range(valueFrom As ParameterValue, valueTo As ParameterValue) As ParameterValue

		Return Factory.CreateParameterValue({valueFrom, valueTo}, ValueType.Range)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Step"/>.
	''' </summary>
	''' <param name="value">The start value</param>
	''' <param name="increment">The increment value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function [Step](value As ParameterValue, increment As ParameterValue) As ParameterValue

		Return Factory.CreateParameterValue({value, increment}, ValueType.Step)

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.SteppedRange"/>.
	''' </summary>
	''' <param name="valueFrom">The start value</param>
	''' <param name="valueTo">The end value</param>
	''' <param name="increment">The increment value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function SteppedRange(valueFrom As ParameterValue, valueTo As ParameterValue, increment As ParameterValue) As ParameterValue

		Return Factory.CreateParameterValue({valueFrom, valueTo, increment}, ValueType.SteppedRange)

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
	''' Configures this <see cref="Builder"/> to use the given <see cref="IParameterFactory"/> to create <see cref="Parameter"/> and <see cref="ParameterValue"/> objects.
	''' </summary>
	''' <param name="factory">The <see cref="IParameterFactory"/> to use</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function Use(factory As IParameterFactory) As Builder

		mFactory = factory

		Return Me

	End Function

	''' <summary>
	''' Configures this <see cref="Builder"/> to use the given <see cref="BuildOptions"/> when validating and building <see cref="Expression"/> objects.
	''' If also using a custom <see cref="IParameterFactory"/>, call <see cref="Builder.Use(IParameterFactory)"/> first.
	''' </summary>
	''' <param name="options">The <see cref="BuildOptions"/> to use</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function Use(options As BuildOptions) As Builder

		Factory.BuildOptions = options

		Return Me

	End Function

	''' <summary>
	''' Looks for faults that were not caught upon construction of the parameters and returns false if one or more were found.
	''' </summary>
	''' <returns>Boolean, True if all <see cref="ParameterValue"/>s are valid</returns>
	Public Function Validate() As Boolean
		Dim blOK As Boolean = True

		For i As Integer = 0 To 4
			Dim p As Parameter = mParameters(i)

			If p.Value.ValueType = ValueType.Any Then
				Continue For ' nothing to validate
			ElseIf p.Value.ValueType.IsSingleValueType Then
				blOK = blOK And ValidateValue(p.ParameterType, p.Value)
			Else
				If p.Value.ValueType = ValueType.Unknown Then
					blOK = False
				Else
					If p.Value.ValueType.IsSymbolValueType Then
						If p.Value.ValueType = ValueType.Symbol_Hash Then
							blOK = blOK And ValidateValue(p.ParameterType, p.Value.Values(0)) ' checks DayOfWeek validity
							blOK = blOK And ((p.Value.Values(1).Value > 0) AndAlso (p.Value.Values(1).Value < 7)) ' there are between 1 and 6 weeks in a month
						End If
					Else
						For Each pv As ParameterValue In p.Value.Values
							blOK = blOK And ValidateValue(p.ParameterType, pv)
						Next
					End If
				End If
			End If
		Next

		Return blOK

	End Function

	''' <summary>
	''' Validates the given expression without actually building an <see cref="Expression"/>.
	''' </summary>
	''' <param name="expression">The string representation of a Cron expression</param>
	''' <returns>Boolean, True if all <see cref="ParameterValue"/>s are valid</returns>
	Public Function Validate(expression As String) As Boolean
		Dim blOK As Boolean = True
		Dim parts As String() = expression.Trim().Split({Space}, StringSplitOptions.RemoveEmptyEntries) ' handle double spaces and whitespace

		If parts.Count <> 5 Then
			blOK = False
			mDefaultParameters.CopyTo(mParameters, 0) ' reset internal parameters to Any
			For i As Integer = 0 To 4
				mParameters(i).Message = String.Format(Resources.errParameterCount, expression)
			Next
		Else
			For i As Integer = 0 To 4
				[With](CType(i, ParameterType), parts(i))

				Dim p As Parameter = mParameters(i)

				If p.IsFault Then
					blOK = False
				Else
					If p.Value.ValueType = ValueType.Any Then
						Continue For ' no values to validate
					ElseIf p.Value.ValueType.IsSingleValueType Then
						blOK = blOK And ValidateValue(p.ParameterType, p.Value)
					Else
						For Each pv As ParameterValue In p.Value.Values
							blOK = blOK And ValidateValue(p.ParameterType, pv)
						Next
					End If
				End If
			Next
			mDefaultParameters.CopyTo(mParameters, 0) ' reset internal parameters to Any
		End If

		Return blOK

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a <see cref="ValueType.Any"/> parameter.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function WithAny(parameterType As ParameterType) As Builder

		mParameters(parameterType) = Factory.CreateParameter(parameterType, ValueType.Any, Nothing)
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a list of <see cref="ParameterValue"/>s.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="values">One or more <see cref="ParameterValue"/> objects</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function WithList(parameterType As ParameterType, ParamArray values As ParameterValue()) As Builder

		mParameters(parameterType) = Factory.CreateParameter(parameterType, ValueType.List, values)
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a range parameter.
	''' Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> parameters are allowed.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="fromVal">Start value of the range</param>
	''' <param name="toVal">End value of the range</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function WithRange(parameterType As ParameterType, fromVal As ParameterValue, toVal As ParameterValue) As Builder

		mParameters(parameterType) = Factory.CreateParameter(parameterType, ValueType.Range, {fromVal, toVal})
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
	Public Function WithStep(parameterType As ParameterType, startValue As ParameterValue, increment As ParameterValue) As Builder

		mParameters(parameterType) = Factory.CreateParameter(parameterType, ValueType.Step, {startValue, increment})
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
	Public Function WithSteppedRange(parameterType As ParameterType, fromVal As ParameterValue, toVal As ParameterValue, incrementVal As ParameterValue) As Builder

		mParameters(parameterType) = New Parameter(parameterType, Factory.CreateParameterValue({fromVal, toVal, incrementVal}, ValueType.SteppedRange))
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to be a single value parameter.
	''' Only <see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> and <see cref="ValueType.MonthOfYear"/> are allowed.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="value">The <see cref="ParameterValue"/> to set</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function WithValue(parameterType As ParameterType, value As ParameterValue) As Builder

		mParameters(parameterType) = Factory.CreateParameter(parameterType, ValueType.Number, {value})
		Return Me

	End Function

	''' <summary>
	''' Configures the parameter of the given <see cref="ParameterType"/> to the parameter, represented by the given string.
	''' If the value cannot be parsed into a recognized <see cref="ParameterValue"/> an exception is thrown.
	''' </summary>
	''' <param name="parameterType">The <see cref="ParameterType"/> to configure</param>
	''' <param name="value">String representation of a parameter value</param>
	''' <returns>This <see cref="Builder"/></returns>
	Public Function [With](parameterType As ParameterType, value As String) As Builder

		If value = Asterix Then
			Return WithAny(parameterType)
		End If

		Dim paramValue As ParameterValue = Factory.CreateParameterValue(value)

		If paramValue.ValueType = ValueType.Unknown Then
			mParameters(parameterType) = New Parameter(parameterType, paramValue) With {.Message = String.Format(Resources.errParameter, value)}
		ElseIf paramValue.ValueType.IsSingleValueType Then
			mParameters(parameterType) = Factory.CreateParameter(parameterType, paramValue.ValueType, {paramValue})
		Else
			mParameters(parameterType) = Factory.CreateParameter(parameterType, paramValue.ValueType, paramValue.Values)
		End If

		Return Me

	End Function

	''' <summary>
	''' Returns a <see cref="ParameterValue"/> of  a type that will be determined when parsing the value.
	''' If the value cannot be parsed, <see cref="Cron.ValueType.Unknown"/> is assigned and subsequent validation will fail.
	''' </summary>
	''' <param name="value">The value</param>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public Function ParseValue(value As String) As ParameterValue

		Return Factory.CreateParameterValue(value)

	End Function

#End Region

#Region " Private methods and functions "

	''' <summary>
	''' Validates the given <see cref="ParameterValue"/> for the given <see cref="ParameterType"/>.
	''' All values that reach this point are of a single value type (<see cref="ValueType.Number"/>, <see cref="ValueType.DayOfWeek"/> or <see cref="ValueType.MonthOfYear"/>).
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/> of the <see cref="Parameter"/> that holds this value</param>
	''' <param name="pv">The <see cref="ParameterValue"/> to validate</param>
	''' <returns>True, if the value is valid for this <see cref="ParameterType"/></returns>
	Private Function ValidateValue(paramType As ParameterType, pv As ParameterValue) As Boolean
		Dim isOK As Boolean = True

		If pv.IsFault Then
			isOK = False
		Else
			If (pv.ValueType = ValueType.MonthOfYear) AndAlso (Not paramType = ParameterType.Month) Then ' look for MonthOfYear in the wrong places
				isOK = False
				pv.Message = String.Format(Resources.errParameterValueType, ValueType.MonthOfYear)
			ElseIf (pv.ValueType = ValueType.DayOfWeek) AndAlso (Not paramType = ParameterType.DayOfWeek) Then ' look for DayOfWeek in the wrong places
				isOK = False
				pv.Message = String.Format(Resources.errParameterValueType, ValueType.DayOfWeek)
			ElseIf pv.Value < MinimumValue(paramType) OrElse pv.Value > MaximumValue(paramType) Then
					isOK = False
				pv.Message = String.Format(Resources.errValueOutOfRange, pv.Value)
			End If
		End If

		Return isOK

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronBuilder, defaulting to a '* * * * *' expression.
	''' </summary>
	Public Sub New()

		mDefaultParameters.CopyTo(mParameters, 0)

	End Sub

#End Region

End Class