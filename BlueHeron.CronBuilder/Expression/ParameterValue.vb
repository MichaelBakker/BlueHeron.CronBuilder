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

''' <summary>
''' Structure that represents an expression parameter value.
''' </summary>
Public Class ParameterValue

#Region " Objects and variables "

	Private mExpression As String

#End Region

#Region " Properties "

	''' <summary>
	''' Determines whether this <see cref="ParameterValue"/> is at fault.
	''' </summary>
	Public ReadOnly Property IsFault As Boolean
		Get
			Return Not String.IsNullOrEmpty(Message)
		End Get
	End Property

	''' <summary>
	''' Any error message.
	''' </summary>
	Public Property Message As String

	''' <summary>
	''' The original value that was used to create this parameter value.
	''' </summary>
	Public ReadOnly Property OriginalValue As Object

	''' <summary>
	''' This value as an integer. Only used when the <see cref="ValueType"/> is <see cref="Cron.ValueType.DayOfWeek"/>, <see cref="Cron.ValueType.MonthOfYear"/> or <see cref="Cron.ValueType.Number"/>.
	''' </summary>
	Public ReadOnly Property Value As Integer

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

#End Region

#Region " Operators and overrides "

	''' <summary>
	''' Determines whether this instance and the specified object have the same value.
	''' </summary>
	''' <param name="obj">The object to compare to this instance</param>
	Public Overrides Function Equals(obj As Object) As Boolean

		Return If(TypeOf (obj) Is String, ToString().Equals(obj), GetHashCode.Equals(obj.GetHashCode))

	End Function

	''' <summary>
	''' Returns a hash code for this object.
	''' </summary>
	Public Overrides Function GetHashCode() As Integer

		If OriginalValue Is Nothing Then
			Return Asterix.GetHashCode
		End If

#If NETSTANDARD2_0 Or NETFRAMEWORK Then
		Return (17 * 31 + ValueType.GetHashCode()) * 31 + OriginalValue.GetHashCode()
#Else
		Return HashCode.Combine(ValueType, OriginalValue)
#End If

	End Function

	''' <summary>
	''' Equality operator.
	''' </summary>
	''' <param name="left">The left <see cref="ParameterValue"/></param>
	''' <param name="right">The right <see cref="ParameterValue"/></param>
	Public Shared Operator =(left As ParameterValue, right As ParameterValue) As Boolean

		Return left.Equals(right)

	End Operator

	''' <summary>
	''' Inequality operator.
	''' </summary>
	''' <param name="left">The left <see cref="ParameterValue"/></param>
	''' <param name="right">The right <see cref="ParameterValue"/></param>
	Public Shared Operator <>(left As ParameterValue, right As ParameterValue) As Boolean

		Return Not left = right

	End Operator

	''' <summary>
	''' Returns the symbolic representation of this parameter value.
	''' </summary>
	Public Overrides Function ToString() As String

		If String.IsNullOrEmpty(mExpression) Then
			Select Case ValueType
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
				Case ValueType.Symbol_Hash
					mExpression = String.Format(fmtHash, Values(0), Values(1))
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

		Select Case ValueType
			Case ValueType.Any
				mMatches = Enumerable.Range(MinimumValue(paramType), MaximumValue(paramType) - MinimumValue(paramType) + 1)
			Case ValueType.Number, ValueType.MonthOfYear, ValueType.DayOfWeek
				mMatches = {Value}
			Case ValueType.Step
				If Values(0).ValueType = ValueType.Any Then
					mMatches = MinimumValue(paramType).To(MaximumValue(paramType), Values(1).Value)
				Else
					mMatches = Values(0).Value.To(MaximumValue(paramType), Values(1).Value)
				End If
			Case ValueType.Range
				If Values(0).Value <= Values(1).Value Then
					mMatches = Enumerable.Range(Values(0).Value, Values(1).Value - MinimumValue(paramType) + 1)
				Else
					mMatches = Array.Empty(Of Integer)
				End If
			Case ValueType.SteppedRange
				mMatches = Values(0).Value.To(Values(1).Value, Values(2).Value)
			Case ValueType.List
				mMatches = Values.SelectMany(Function(v) v.AsEnumerable(paramType)).Distinct
			Case ValueType.Symbol_Hash
				mMatches = {Values(0).Value}
			Case Else
				mMatches = Array.Empty(Of Integer)
		End Select

		Return mMatches

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new <see cref="ParameterValue"/> that defaults to 'Any', i.e. '*'.
	''' </summary>
	<DebuggerStepThrough()>
	Public Sub New()
	End Sub

	''' <summary>
	''' Creates a new <see cref="ParameterValue"/> using the given fields.
	''' </summary>
	''' <param name="val">The value</param>
	''' <param name="valType">The <see cref="Cron.ValueType"/></param>
	<DebuggerStepThrough()>
	Public Sub New(val As Integer, valType As ValueType)

		OriginalValue = Value
		ValueType = valType
		Value = val

	End Sub

	''' <summary>
	''' Creates a <see cref="ParameterValue"/> of type <see cref="Cron.ValueType.Range"/>, <see cref="Cron.ValueType.Step"/>, <see cref="Cron.ValueType.SteppedRange"/> or <see cref="Cron.ValueType.List"/>.
	''' </summary>
	''' <param name="values">The parameter values</param>
	''' <param name="valType">The <see cref="Cron.ValueType"/></param>
	<DebuggerStepThrough()> Public Sub New(values As IEnumerable(Of ParameterValue), valType As ValueType)

		OriginalValue = values
		Me.Values = If(values, Array.Empty(Of ParameterValue))
		ValueType = valType

	End Sub

#End Region

End Class