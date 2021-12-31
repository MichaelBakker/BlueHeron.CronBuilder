
''' <summary>
''' Container for <see cref="Builder"/> options.
''' </summary>
Public NotInheritable Class BuildOptions

#Region " Objects and variables "

	Private Shared mDefault As BuildOptions

#End Region

#Region " Properties "

	''' <summary>
	''' Returns the default <see cref="BuildOptions"/>.
	''' </summary>
	''' <returns>A <see cref="BuildOptions"/> object</returns>
	Public Shared ReadOnly Property [Default] As BuildOptions
		Get
			If mDefault Is Nothing Then
				mDefault = New BuildOptions
			End If
			Return mDefault
		End Get
	End Property

	''' <summary>
	''' Configure the <see cref="Builder"/> to support the following symbols:
	''' <list type="table">
	''' <listheader><term>Symbol</term><description>Usage</description></listheader>
	''' <item><term>L</term><description>Used differently in each of the two fields in which it is allowed:
	''' In the <see cref="ParameterType.Day"/> field, L selects the last day of the month, which is 31 for January and 29 for February on leap years.
	''' When used in the <see cref="ParameterType.DayOfWeek"/> field by itself, it means Saturday. But if used in the <see cref="ParameterType.DayOfWeek"/> field after another value, L selects the last xx day of the month. For example, 6L selects the last Friday of the month.
	''' When using the L special character, do not specify lists or ranges of values, because this may give confusing results.</description></item>
	''' <item><term>W</term><description>Used to specify the weekday (Monday-Friday) nearest to the given day.
	''' For example, if you specify 15W as the value for the <see cref="ParameterType.Day"/> field, the nearest weekday to the 15th Of the month is selected.
	''' So if the 15th is a Saturday, Friday the 14th is selected. If the 15th is a Sunday, Monday the 16th is selected.
	''' If the 15th is a Tuesday, Tuesday the 15th is selected.
	''' However if you specify 1W as the value for <see cref="ParameterType.Day"/>, and the 1st is a Saturday, Monday the 3rd is selected, as the selection rules do not allow for crossing over the boundary of a month's days to the previous or the subsequent month.
	''' The W character can only be used to specify a single day, not a range or list of days.</description></item>
	''' <item><term>#</term><description>Used to specify the nth XXX (or XX) day of the month.
	''' For example, the value FRI#3 or 6#3 in the <see cref="ParameterType.DayOfWeek"/> field means the third Friday of the month (6 or FRI = Friday, and #3 = the 3rd occurrence in the month).</description></item>
	''' </list>
	''' Note: Not all libraries or systems support all of the symbols mentioned above.
	''' E.g.: The Hangfire JobServer doesn't support any of these symbols.
	''' </summary>
	''' <remarks>Default: <code>False</code></remarks>
	Public Property SupportSymbols As Boolean = False

#End Region

#Region " Construction "

	''' <summary>
	''' Creates a new <see cref="BuildOptions"/> object.
	''' </summary>
	Public Sub New()
	End Sub

	''' <summary>
	''' Creates a new <see cref="BuildOptions"/> object, based on the given parameters.
	''' </summary>
	''' <param name="supportSymbols">Configure the <see cref="Builder"/> to support the "L", "W"and "#" symbol</param>
	Public Sub New(supportSymbols As Boolean)

		Me.SupportSymbols = supportSymbols

	End Sub

#End Region

End Class