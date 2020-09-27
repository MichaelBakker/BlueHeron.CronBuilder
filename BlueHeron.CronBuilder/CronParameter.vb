
''' <summary>
''' Base class for Cron parameters.
''' </summary>
Public MustInherit Class CronParameter

	''' <summary>
	''' The <see cref="ParameterType" /> of this parameter.
	''' </summary>
	Public ReadOnly Property ParameterType As ParameterType

	''' <summary>
	''' Returns the parameter expression.
	''' </summary>
	Public MustOverride Overrides Function ToString() As String

	''' <summary>
	''' Validates this parameter.
	''' </summary>
	''' <returns>True, if supplied values are valid</returns>
	Public MustOverride Function Validate() As Boolean

	''' <summary>
	''' Validates this parameter.
	''' </summary>
	''' <param name="errorMessage">Will hold an error message if validation fails</param>
	''' <returns>True, if supplied values are valid</returns>
	Public MustOverride Function Validate(ByRef errorMessage As String) As Boolean

	''' <summary>
	''' Creates a new CronParameter.
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/></param>
	Protected Sub New(paramType As ParameterType)

		ParameterType = paramType

	End Sub

End Class

''' <summary>
''' The wildcard parameter, i.e. '*', that matches all values.
''' </summary>
Public Class CronAnyParameter
	Inherits CronParameter

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return Asterix

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean

		Return True

	End Function

	''' <summary>
	''' Creates a new CronAnyParameter.
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/></param>
	Public Sub New(paramType As ParameterType)

		MyBase.New(paramType)

	End Sub

End Class

''' <summary>
''' Parameter that consists of a comma-separated list of values.
''' </summary>
Public Class CronListParameter
	Inherits CronParameter

#Region " Objects and variables "

	Private ReadOnly mList As List(Of Object)

#End Region

#Region " Properties "

	''' <summary>
	''' List of assigned values.
	''' </summary>
	Public ReadOnly Property Values As List(Of Object)

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Join(Comma, mList)

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		For Each it As Object In mList
			If Not ParameterType.Validate(ValueType, it) Then
				Return False
			End If
		Next

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean
		Dim messages As String = String.Empty
		Dim blValid As Boolean = True

		For Each it As Object In mList
			If Not ParameterType.Validate(ValueType, it) Then
				blValid = False
				messages &= vbCrLf & String.Format(errParameter, it)
			End If
		Next
		errorMessage = messages

		Return blValid

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronListParameter that should contain values of the given <see cref="ParameterValueType"/>.
	''' </summary>
	''' <param name="valueType">The expected <see cref="ParameterValueType"/></param>
	''' <param name="values">The assigned values</param>
	Public Sub New(paramType As ParameterType, valueType As ParameterValueType, values As Object())

		MyBase.New(paramType)
		Me.ValueType = valueType
		mList = New List(Of Object)(values)

	End Sub

#End Region

End Class

''' <summary>
''' Parameter that consists of a range of values.
''' </summary>
Public Class CronRangeParameter
	Inherits CronParameter

#Region " Objects and variables "

	Private ReadOnly mValueFrom As Object

	Private ReadOnly mValueTo As Object

#End Region

#Region " Properties "

	''' <summary>
	''' The assigned start value.
	''' </summary>
	Public ReadOnly Property From As Object

	''' <summary>
	''' The assigned end value.
	''' </summary>
	Public ReadOnly Property [To] As Object

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Format(fmtRange, mValueFrom, mValueTo)

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		If Not (ParameterType.Validate(ValueType, From) AndAlso ParameterType.Validate(ValueType, [To])) Then
			Return False
		End If

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean
		Dim blValid As Boolean = True

		If Not ParameterType.Validate(ValueType, mValueFrom) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(errParameter, mValueFrom)
		End If
		If Not ParameterType.Validate(ValueType, mValueTo) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(errParameter, mValueTo)
		End If

		Return blValid

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronRangeParameter that should contain two values of the given <see cref="ParameterValueType"/>.
	''' </summary>
	''' <param name="valueType">The expected <see cref="ParameterValueType"/></param>
	''' <param name="fromValue">The assigned start value</param>
	''' <param name="toValue">The assigned start value</param>
	Public Sub New(paramType As ParameterType, valueType As ParameterValueType, fromValue As Object, toValue As Object)

		MyBase.New(paramType)
		Me.ValueType = valueType
		mValueFrom = fromValue
		mValueTo = toValue

	End Sub

#End Region

End Class

''' <summary>
''' Parameter that consists of a start value and an increment.
''' </summary>
Public Class CronStepParameter
	Inherits CronParameter

#Region " Objects and variables "

	Private ReadOnly mStartValue As Object

	Private ReadOnly mIncrement As Object

#End Region

#Region " Properties "

	''' <summary>
	''' The assigned start value.
	''' </summary>
	Public ReadOnly Property Value As Object

	''' <summary>
	''' The assigned increment value.
	''' </summary>
	Public ReadOnly Property Increment As Object

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Format(fmtStep, mStartValue, mIncrement)

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		If Not (ParameterType.Validate(ValueType, mStartValue) AndAlso ParameterType.Validate(ValueType, mIncrement)) Then
			Return False
		End If

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean
		Dim blValid As Boolean = True

		If Not ParameterType.Validate(ValueType, mStartValue) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(errParameter, mStartValue)
		End If
		If Not ParameterType.Validate(ValueType, mIncrement) Then
			blValid = False
			errorMessage = vbCrLf & String.Format(errParameter, mIncrement)
		End If

		Return blValid

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronStepParameter that should contain two values of the given <see cref="ParameterValueType"/>.
	''' </summary>
	''' <param name="valueType">The expected <see cref="ParameterValueType"/></param>
	''' <param name="startValue">The assigned start value</param>
	''' <param name="increment">The assigned start value</param>
	Public Sub New(paramType As ParameterType, valueType As ParameterValueType, startValue As Object, increment As Object)

		MyBase.New(paramType)
		Me.ValueType = valueType
		mStartValue = startValue
		mIncrement = increment

	End Sub

#End Region

End Class

''' <summary>
''' Parameter that consists of a single value.
''' </summary>
Public Class CronValueParameter
	Inherits CronParameter

#Region " Objects and variables "

	Private ReadOnly mValue As Object

#End Region

#Region " Properties "

	''' <summary>
	''' The assigned value.
	''' </summary>
	Public ReadOnly Property Value As Object

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType

#End Region

#Region " Public methods and functions "

	''' <inheritdoc cref="CronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return Value.ToString

	End Function

	''' <inheritdoc cref="CronParameter.Validate()" />
	Public Overrides Function Validate() As Boolean

		If Not ParameterType.Validate(ValueType, Value) Then
			Return False
		End If

		Return True

	End Function

	''' <inheritdoc cref="CronParameter.Validate(ByRef String)" />
	Public Overrides Function Validate(ByRef errorMessage As String) As Boolean
		Dim blValid As Boolean = True

		If Not ParameterType.Validate(ValueType, Value) Then
			blValid = False
			errorMessage = String.Format(errParameter, Value)
		End If

		Return blValid

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronValueParameter that should contain a value of the given <see cref="ParameterValueType"/>.
	''' </summary>
	''' <param name="valueType">The expected <see cref="ParameterValueType"/></param>
	''' <param name="value">The assigned value</param>
	Public Sub New(paramType As ParameterType, valueType As ParameterValueType, value As Object)

		MyBase.New(paramType)
		Me.ValueType = valueType
		mValue = value

	End Sub

#End Region

End Class