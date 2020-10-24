# BlueHeron.CronBuilder

---------------------------------------

A builder, parser and interpreter of [cron](https://en.wikipedia.org/wiki/Cron) expressions.
See the [change log](CHANGELOG.md) for changes and road map.




## NuGet

```

PM> Install-Package BlueHeron.CronBuilder

```



## About cron expressions

A cron expression is a very simple, but quite efficient way to define a schedule or time interval. This method was developed on Unix-based systems to efficiently define and store scheduled jobs.
These jobs were stored in a so called crontab file and had the following structure:

```
# ┌───────────── minute (0 - 59)
# │ ┌───────────── hour (0 - 23)
# │ │ ┌───────────── day of the month (1 - 31)
# │ │ │ ┌───────────── month (1 - 12 or JAN - DEC)
# │ │ │ │ ┌───────────── day of the week (0 - 6 or SUN - SAT)
# │ │ │ │ │
# │ │ │ │ │
# * * * * * [command to execute]
```

This notation is very compact and enables the efficient creation of a wide range of schedules without the need for a complicated set of objects , structures and values. Also, validation, polling and matching is simple and fast.
The BlueHeron.CronBuilder package is a .NET implementation of this notation.



## Features

- Create cron expressions
- Parse cron expressions
- Matching and polling
- Humanize expressions
- Extensible language support
- Limitations (are features too)
  

### Create cron expressions

```vb
Dim mBuilder as New BlueHeron.Cron.Builder
Dim expectedExpression As String = "23 0-20 1/2 1 *"
Dim parameterizedExpression As Expression = mBuilder.
	WithValue(ParameterType.Minute, ParameterValue.Number(23)).
	WithRange(ParameterType.Hour, ParameterValue.Number(0), ParameterValue.Number(20)).
	WithStep(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(2)).
	WithValue(ParameterType.Month, ParameterValue.Number(1)).
	WithAny(ParameterType.DayOfWeek).
	Build()

Debug.Assert(parameterizedExpression.Expression = expectedExpression)

expectedExpression = "0 0-23 * APR-OCT MON"  ' integer, text and enum value are supported
parameterizedExpression = mBuilder.
	WithValue(ParameterType.Minute, ParameterValue.Number(0)).
	WithRange(ParameterType.Hour, ParameterValue.Number(0), ParameterValue.Number(23)).
	WithAny(ParameterType.Day).
	WithRange(ParameterType.Month, ParameterValue.FromString("APR"), ParameterValue.FromString("OCT")).
	WithValue(ParameterType.DayOfWeek, ParameterValue.DayOfWeek(DayOfWeek.MON)).
	Build() ' validation is performed automatically

Debug.Assert(parameterizedExpression.Expression = expectedExpression)

```



### Parse cron expressions

```vb
Dim expectedExpression As String = "23 0-20 1/2 1 *"
Dim expression As Expression = mBuilder.Build(expectedExpression)

Debug.Assert(expression.ToString = expectedExpression)

```



### Matching and polling

```vb
Dim expression As Expression = mBuilder.
	WithValue(ParameterType.Minute, ParameterValue.Number(0)).
	WithValue(ParameterType.Hour, ParameterValue.Number(12)).
	WithRange(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(7)).
	WithValue(ParameterType.DayOfWeek, ParameterValue.DayOfWeek(DayOfWeek.MON)).
	WithAny(ParameterType.Month).
	Build() 'every first monday of the month at noon
Dim matches As IEnumerable(Of Date) = expression.Next(New Date(2020, 10, 29, 13, 0, 0), 12) ' next 12 matches, starting at the given date and time
Dim expected As Date() = {
    New Date(2020, 11, 2, 12, 0, 0),
    New Date(2020, 12, 7, 12, 0, 0),
    New Date(2021, 1, 4, 12, 0, 0),
    New Date(2021, 2, 1, 12, 0, 0),
    New Date(2021, 3, 1, 12, 0, 0),
    New Date(2021, 4, 5, 12, 0, 0),
    New Date(2021, 5, 3, 12, 0, 0),
    New Date(2021, 6, 7, 12, 0, 0),
    New Date(2021, 7, 5, 12, 0, 0),
    New Date(2021, 8, 2, 12, 0, 0),
    New Date(2021, 9, 6, 12, 0, 0),
    New Date(2021, 10, 4, 12, 0, 0)
    }

Debug.Assert(matches.Count = 12)
For i As Integer = 0 To 11
	Debug.Assert(matches(i) = expected(i))
Next

Dim expression As Expression = mBuilder.Build(_ANY) ' every minute

Debug.Assert(expression.Poll() = True) ' current date and time is a match

```



### Humanize expressions

Expressions us a `IHumanizer` implementation to convert the expression into a human-readable, localized sentence.
The default implementation (`DefaultHumanizer`) currently supports English, Dutch, German and French.
You can create and inject your own implementation to create human-readable sentences of your choice.

### Extensible language support

The library uses the [Multilingual App Toolkit](https://developer.microsoft.com/en-us/windows/downloads/multilingual-app-toolkit/) to localize all resources used, so it is easily extensible.
Users can create their own `IHumanizer` implementations to humanize expressions into a language and grammar of their own choice.

### Limitations

Using the cron syntax you cannot create an expression that gives a match like, say every 90 minutes.
To circumvent this you can create two schedules that match every three hours and give the second expression an offset of 90 minutes in relation to the first.



## License

[MIT](LICENSE)

