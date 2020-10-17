
Namespace Localization

	''' <summary>
	''' Interface definition for objects that convert <see cref="Expression"/> objects into human-readable, localized representations.
	''' </summary>
	Public Interface IHumanizer

		''' <summary>
		''' Converts the given <see cref="Expression"/> into a human-readable, localized representation.
		''' </summary>
		''' <param name="expression">The <see cref="Expression"/> to convert</param>
		''' <returns>Human-readable, localized representation of the <see cref="Expression"/></returns>
		Function Humanize(expression As Expression) As String

	End Interface

End Namespace
