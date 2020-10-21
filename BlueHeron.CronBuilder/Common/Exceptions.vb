
''' <summary>
''' An <see cref="Exception"/> object that is thrown by the <see cref="Builder"/> when modifying a parameter fails.
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
	''' The <see cref="ParameterType"/> of the <see cref="Parameter" /> for which the value was meant.
	''' </summary>
	''' <returns>A <see cref="ParameterType"/></returns>
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
	''' <param name="paramType">The <see cref="ParameterType"/> of the <see cref="Parameter" /> for which the value was meant</param>
	''' <param name="valType"> The (interpreted) <see cref="Cron.ValueType"/> of the value</param>
	''' <param name="value">The value that caused the exception</param>
	''' <param name="msg">The exception message</param>
	Friend Sub New(paramType As ParameterType?, valType As ValueType, value As String, msg As String)

		MyBase.New(msg)
		ParameterType = paramType
		ValueType = valType
		OriginalValue = value

	End Sub

#End Region

End Class

''' <summary>
'''  An <see cref="Exception"/> object that is thrown by the <see cref="Builder"/> when validating the <see cref="ParameterValue"/>s fails.
''' </summary>
<Serializable(), CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification:="Not thrown outside of library"), CodeAnalysis.SuppressMessage("Usage", "CA2229:Implement serialization constructors", Justification:="Not thrown outside of library")>
Public NotInheritable Class ParserAggregateException
	Inherits Exception

#Region " Properties "

	''' <summary>
	''' The <see cref="ParserException"/>s that occurred.
	''' </summary>
	''' <returns>An <see cref="IEnumerable(Of ParserException)"/></returns>
	Public ReadOnly Property Errors As IEnumerable(Of ParserException)

#End Region

#Region " Construction "


	''' <summary>
	''' Creates a new ParserAggregateException.
	''' </summary>
	''' <param name="msg">Friendly error message</param>
	''' <param name="exceptions">The <see cref="ParserException"/>s that occurred</param>
	Friend Sub New(msg As String, exceptions As IEnumerable(Of ParserException))

		MyBase.New(msg)
		Errors = exceptions

	End Sub

#End Region

End Class