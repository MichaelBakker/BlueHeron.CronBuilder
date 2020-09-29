
''' <summary>
''' The wildcard parameter, i.e. '*', that matches all values.
''' </summary>
Public NotInheritable Class CronAnyParameter
	Implements ICronParameter

#Region " Objects and variables "

	Private mValues As List(Of Integer)

#End Region

#Region " Properties "

	''' <summary>
	''' The <see cref="ParameterType"/> of this parameter.
	''' </summary>
	Public ReadOnly Property ParameterType As ParameterType Implements ICronParameter.ParameterType

	''' <summary>
	''' The expected <see cref="ParameterValueType"/>.
	''' </summary>
	Public ReadOnly Property ValueType As ParameterValueType = ParameterValueType.Any Implements ICronParameter.ValueType

#End Region

#Region " Public Methods and functions "

	''' <inheritdoc cref="ICronParameter.ToList()" />
	Public Function ToList() As List(Of Integer) Implements ICronParameter.ToList

		If mValues Is Nothing Then
			mValues = New List(Of Integer)
			For i As Integer = MinimumValues(ParameterType) To MaximumValues(ParameterType)
				mValues.Add(i)
			Next
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String Implements ICronParameter.ToString

		Return Asterix

	End Function

	''' <inheritdoc cref="ICronParameter.Validate()" />
	Public Function Validate() As Boolean Implements ICronParameter.Validate

		Return True

	End Function

	''' <inheritdoc cref="ICronParameter.Validate(ByRef String)" />
	Public Function Validate(ByRef errorMessage As String) As Boolean Implements ICronParameter.Validate

		Return True

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronAnyParameter.
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/></param>
	Friend Sub New(paramType As ParameterType)

		ParameterType = paramType

	End Sub

#End Region

End Class