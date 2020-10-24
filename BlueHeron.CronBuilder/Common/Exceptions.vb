
''' <summary>
''' An <see cref="Exception"/> object that is thrown by the <see cref="Builder"/> when modifying a <see cref="Parameter"/> or building an <see cref="Expression"/> fails.
''' </summary>
<Serializable(), CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification:="Not thrown outside of library"), CodeAnalysis.SuppressMessage("Usage", "CA2229:Implement serialization constructors", Justification:="Not thrown outside of library")>
Public NotInheritable Class ParserException
	Inherits Exception

#Region " Properties "

	''' <summary>
	''' The value that caused the exception.
	''' </summary>
	''' <returns>The value</returns>
	Public ReadOnly Property OriginalValue As String

	''' <summary>
	''' The <see cref="Cron.ParameterType"/> of the <see cref="Parameter" /> for which the value was meant.
	''' </summary>
	''' <returns>A <see cref="Cron.ParameterType"/></returns>
	Public ReadOnly Property ParameterType As ParameterType?

	''' <summary>
	''' The (interpreted) <see cref="Cron.ValueType"/> of the value.
	''' </summary>
	''' <returns>A <see cref="Cron.ValueType"/></returns>
	Public ReadOnly Property ValueType As ValueType

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new ParserException.
	''' </summary>
	''' <param name="paramType">The <see cref="Cron.ParameterType"/> of the <see cref="Parameter" /> for which the value was meant</param>
	''' <param name="valType"> The (interpreted) <see cref="Cron.ValueType"/> of the value</param>
	''' <param name="value">The value that caused the exception</param>
	''' <param name="msg">The exception message</param>
	<DebuggerStepThrough()> Friend Sub New(paramType As ParameterType?, valType As ValueType, value As String, msg As String)

		MyBase.New(msg)
		ParameterType = paramType
		ValueType = valType
		OriginalValue = value

	End Sub

#End Region

End Class