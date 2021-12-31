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
''' Structure that represents an expression parameter.
''' </summary>
<DebuggerDisplay("{ParameterType}: {Value}")> <CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification:="Unused")>
Public Class Parameter

#Region " Objects and variables "

	Private mMatches As IEnumerable(Of Integer)

#End Region

#Region " Properties "

	''' <summary>
	''' Determines whether this <see cref="Parameter"/> is at fault.
	''' </summary>
	Public ReadOnly Property IsFault As Boolean
		Get
			Return Not String.IsNullOrEmpty(Message)
		End Get
	End Property

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
	''' Any error message.
	''' </summary>
	Public Property Message As String

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
	Public Sub New(paramType As ParameterType, val As ParameterValue)

		ParameterType = paramType
		Value = val

	End Sub

#End Region

End Class