
''' <summary>
''' The wildcard parameter, i.e. '*', that matches all values.
''' </summary>
Public NotInheritable Class CronAnyParameter
	Inherits CronParameterBase

#Region " Public Methods and functions "

	''' <inheritdoc cref="ICronParameter.AsEnumerable()" />
	Public Overrides Function AsEnumerable() As IEnumerable(Of Integer)

		If mValues Is Nothing Then
			mValues = New List(Of Integer)
			For i As Integer = MinimumValues(ParameterType) To MaximumValues(ParameterType)
				mValues.Add(i)
			Next
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return Asterix

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronAnyParameter.
	''' </summary>
	''' <param name="paramType">The <see cref="ParameterType"/></param>
	Friend Sub New(paramType As ParameterType)

		MyBase.New(paramType, ParameterValue.Any)

	End Sub

#End Region

End Class