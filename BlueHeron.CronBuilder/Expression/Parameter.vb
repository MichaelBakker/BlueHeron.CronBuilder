﻿
''' <summary>
''' Structure that represents an expression parameter.
''' </summary>
<DebuggerDisplay("{ParameterType}: {Value}")> <CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification:="Unused")>
Public Structure Parameter

#Region " Objects and variables "

	Private mMatches As IEnumerable(Of Integer)

#End Region

#Region " Properties "

	''' <summary>
	''' Returns all integer values that match the <see cref="Value"/> for this <see cref="ParameterType"/>.
	''' </summary>
	''' <returns>An <see cref="IEnumerable(Of Integer)"/></returns>
	Friend ReadOnly Property Matches As IEnumerable(Of Integer)
		Get
			If mMatches Is Nothing Then
				mMatches = Value.AsEnumerable(ParameterType)
			End If
			Return mMatches
		End Get
	End Property

	''' <summary>
	''' The <see cref="Cron.ParameterType"/> of this parameter.
	''' </summary>
	''' <returns>A <see cref="Cron.ParameterType"/> value</returns>
	Public ReadOnly Property ParameterType As ParameterType

	''' <summary>
	''' Returns the <see cref="ParameterValue"/>, in use by this parameter.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/></returns>
	Public ReadOnly Property Value As ParameterValue

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new parameter.
	''' </summary>
	''' <param name="paramType">The <see cref="Cron.ParameterType"/></param>
	''' <param name="val">The <see cref="ParameterValue"/></param>
	Friend Sub New(paramType As ParameterType, val As ParameterValue)

		ParameterType = paramType
		Value = val

	End Sub

#End Region

End Structure