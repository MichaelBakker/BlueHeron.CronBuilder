﻿
''' <summary>
''' Parameter that consists of a range of values.
''' </summary>
Public NotInheritable Class CronRangeParameter
	Inherits CronParameterBase

#Region " Public methods and functions "

	''' <inheritdoc cref="ICronParameter.AsEnumerable()" />
	Public Overrides Function AsEnumerable() As IEnumerable(Of Integer)

		If mValues Is Nothing Then
			Dim fromVal As Integer = Value.Values(0).Value
			Dim toVal As Integer = Value.Values(1).Value
			Dim maxVal As Integer = MaximumValues(ParameterType)
			Dim minVal As Integer = MinimumValues(ParameterType)

			mValues = New List(Of Integer)
			For i As Integer = Math.Max(minVal, fromVal) To Math.Min(maxVal, toVal)
				mValues.Add(i)
			Next
		End If

		Return mValues

	End Function

	''' <inheritdoc cref="ICronParameter.ToString()" />
	Public Overrides Function ToString() As String

		Return String.Format(fmtRange, Value.Values(0), Value.Values(1))

	End Function

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new CronRangeParameter.
	''' </summary>
	''' <param name="fromValue">The assigned start value</param>
	''' <param name="toValue">The assigned end value</param>
	Friend Sub New(paramType As ParameterType, fromValue As ParameterValue, toValue As ParameterValue)

		MyBase.New(paramType, ParameterValue.Range(fromValue, toValue))

	End Sub

#End Region

End Class