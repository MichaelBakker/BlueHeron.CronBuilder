
''' <summary>
''' Interface definition for Cron parameters.
''' </summary>
Public Interface ICronParameter

#Region " Properties "

	''' <summary>
	''' The <see cref="ParameterType" /> of this parameter.
	''' </summary>
	ReadOnly Property ParameterType As ParameterType

	''' <summary>
	''' Returns the <see cref="ParameterValue"/>, in use by this parameter.
	''' </summary>
	''' <returns>A <see cref="ParameterValue"/></returns>
	ReadOnly Property Value As ParameterValue

#End Region

#Region " Methods and functions "

	''' <summary>
	''' Returns all integer values that match this parameter.
	''' </summary>
	''' <returns>A <see cref="List(Of Integer)"/></returns>
	Function AsEnumerable() As IEnumerable(Of Integer)

	''' <summary>
	''' Returns the parameter expression.
	''' </summary>
	Function ToString() As String

#End Region

End Interface