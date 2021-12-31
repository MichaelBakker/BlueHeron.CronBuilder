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
