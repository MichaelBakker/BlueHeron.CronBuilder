using System.Diagnostics;
using BlueHeron.Cron;
using DayOfWeek = BlueHeron.Cron.DayOfWeek;

namespace BlueHeron.CronBuilder.Test;

[TestClass]
public sealed class Tests
{
    #region Objects and variables

    private const string ANY = "* * * * *";

    private static Builder mBuilder = null!;

    #endregion

    #region Construction and destruction

    [ClassInitialize(InheritanceBehavior.None)]
    public static void ClassInit(TestContext context)
    {
        mBuilder = new();
    }

    [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
    public static void ClassCleanup()
    {
        mBuilder = null!;
    }

    #endregion

    [TestMethod]
    public void Test01_DefaultExpression()
    {
        Assert.AreEqual(ANY, new Builder().Build().ToString());
    }

    [TestMethod]
    public void Test02_ParseParameters()
    {
        var expectedExpression = "23 0-20 1/2 1 *";
        var expression = mBuilder.
            WithValue(ParameterType.Minute, ParameterValue.Number(23)).
            WithRange(ParameterType.Hour, ParameterValue.Number(0), ParameterValue.Number(20)).
            WithStep(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(2)).
            WithValue(ParameterType.Month, ParameterValue.Number(1)).
            WithAny(ParameterType.DayOfWeek). // not always necessary; when using a new CronBuilder instance all parameters default to 'Any' (see Test01_Default)
            Build();
        Assert.AreEqual(expectedExpression, expression.ToString());

        expectedExpression = "0 0-23 * APR-OCT MON"; // integer, text and enum value   supported
        expression = mBuilder.
            WithValue(ParameterType.Minute, ParameterValue.Number(0)).
            WithRange(ParameterType.Hour, ParameterValue.Number(0), ParameterValue.Number(23)).
            WithAny(ParameterType.Day).
            WithRange(ParameterType.Month, ParameterValue.Parse("APR"), ParameterValue.Parse("OCT")).
            WithValue(ParameterType.DayOfWeek, ParameterValue.DayOfWeek(DayOfWeek.MON)).
            Build(); // validation is performed automatically
        Assert.AreEqual(expectedExpression, expression.ToString());
    }

    [TestMethod]
    public void Test03_DateMatchingSingleValue()
    {
        var dtmTest = new DateTime(2020, 9, 29); // is a tuesday
        var expression = mBuilder.
            WithValue(ParameterType.Minute, ParameterValue.Number(0)).
            WithValue(ParameterType.Hour, ParameterValue.Number(12)).
            WithAny(ParameterType.Day).
            WithAny(ParameterType.Month).
            WithAny(ParameterType.DayOfWeek).
            Build(); // every day at noon
        var dateToMatchLater = new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0); // 1pm
        var dateToMatchEarlier = new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0); // 11am 
        var matchedAfter = expression.Next(dateToMatchLater);
        var matchedBefore = expression.Previous(dateToMatchEarlier);

        Assert.IsTrue(matchedAfter == new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day + 1, 12, 0, 0));
        Assert.IsTrue(matchedBefore == new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day - 1, 12, 0, 0));

        matchedAfter = expression.Next(dateToMatchEarlier);
        matchedBefore = expression.Previous(dateToMatchLater);

        Assert.IsTrue(matchedAfter == new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0));
        Assert.IsTrue(matchedBefore == new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0));

        var expression2 = mBuilder.
            WithValue(ParameterType.Minute, ParameterValue.Number(0)).
            WithValue(ParameterType.Hour, ParameterValue.Number(12)).
            WithAny(ParameterType.Day).
            WithAny(ParameterType.Month).
            WithValue(ParameterType.DayOfWeek, ParameterValue.DayOfWeek(DayOfWeek.MON)).
            Build(); // every monday at noon

        matchedAfter = expression2.Next(dateToMatchLater);
        matchedBefore = expression2.Previous(dateToMatchEarlier);

        Assert.IsTrue(matchedAfter == new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0).AddDays(6)); //  first monday after test date
        Assert.IsTrue(matchedBefore == new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 12, 0, 0).AddDays(-1)); // last monday before testdate
    }

    [TestMethod]
    public void Test04_DateMatchingWeekOfDayValue()
    {
        var dtmTest = new DateTime(2020, 9, 29);
        var expression = mBuilder.
            WithValue(ParameterType.Minute, ParameterValue.Number(0)).
            WithValue(ParameterType.Hour, ParameterValue.Number(12)).
            WithRange(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(7)).
            WithAny(ParameterType.Month).
            WithValue(ParameterType.DayOfWeek, ParameterValue.DayOfWeek(DayOfWeek.MON)).
            Build(); // every first monday of any month at noon -> support for Symbol_Hash not needed!
        var dateToMatchLater = new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0); // 1pm
        var dateToMatchEarlier = new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0); // 11am 
        var matchedAfter = expression.Next(dateToMatchLater);
        var matchedBefore = expression.Previous(dateToMatchEarlier);

        Assert.IsTrue(matchedAfter == new DateTime(2020, 10, 5, 12, 0, 0)); // first monday of month after testdate
        Assert.IsTrue(matchedBefore == new DateTime(2020, 9, 7, 12, 0, 0)); // first monday of month before testdate
    }

    [TestMethod]
    public void Test05_DateMatchingRangeCombinations1()
    {
        var dtmTest = new DateTime(2020, 9, 29);
        var expression = mBuilder.
            WithValue(ParameterType.Minute, ParameterValue.Number(0)).
            WithValue(ParameterType.Hour, ParameterValue.Number(12)).
            WithRange(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(7)).
            WithStep(ParameterType.Month, ParameterValue.Number(2), ParameterValue.Number(2)).
            WithValue(ParameterType.DayOfWeek, ParameterValue.DayOfWeek(DayOfWeek.MON)).
            Build(); // every first monday of even months at noon
        var dateToMatchLater = new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0); // 1pm
        var dateToMatchEarlier = new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0); // 11am
        var matchedAfter = expression.Next(dateToMatchLater);
        var matchedBefore = expression.Previous(dateToMatchEarlier);

        Assert.IsTrue(matchedAfter == new DateTime(2020, 10, 5, 12, 0, 0)); // first monday of first matching month after testdate
        Assert.IsTrue(matchedBefore == new DateTime(2020, 8, 3, 12, 0, 0)); // first monday of last matching month before testdate
    }

    [TestMethod]
    public void Test05_DateMatchingRangeCombinations2()
    {
        var dtmTest = new DateTime(2020, 9, 29, 13, 1, 0);
        var expression = mBuilder.
            WithValue(ParameterType.Minute, ParameterValue.Number(0)).
            WithStep(ParameterType.Hour, ParameterValue.Any(), ParameterValue.Number(3)).
            WithAny(ParameterType.Day).
            WithAny(ParameterType.Month).
            WithAny(ParameterType.DayOfWeek).
            Build(); // at minute 0 of every third hour
        var matchedAfter = expression.Next(dtmTest);

        Assert.IsTrue(matchedAfter == new DateTime(2020, 9, 29, 15, 0, 0)); // 0, 3, 6, 9, 12, [15] , 18, 21
    }

    [TestMethod]
    public void Test06_DateMatchingMonthAndYearBoundary()
    {
        var dtmTest = new DateTime(2020, 11, 29, 23, 15, 0);
        var expression = mBuilder.Build("* 12 1-7 1/3 MON"); // at any minute of hour 12 on every first monday of every 3rd month starting with January

        Assert.IsTrue(expression.Next(dtmTest) == new DateTime(2021, 1, 4, 12, 0, 0));
    }

    [TestMethod]
    public void Test07_DateMatchingList()
    {
        var dtmTest = new DateTime(2020, 9, 29, 12, 0, 0);
        var expression = mBuilder.
            WithValue(ParameterType.Minute, ParameterValue.Number(0)).
            WithList(ParameterType.Hour, ParameterValue.Number(1), ParameterValue.Number(2), ParameterValue.Number(3), ParameterValue.Step(ParameterValue.Number(4), ParameterValue.Number(2))).
            WithAny(ParameterType.Day).
            WithAny(ParameterType.Month).
            WithAny(ParameterType.DayOfWeek).
            Build(); // every 1st, 2nd, 3rd hour and then every second hour starting at hour 4 (i.e. 1, 2, 3, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22)

        var dateToMatchLater = new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 13, 0, 0); // 1pm
        var dateToMatchEarlier = new DateTime(dtmTest.Year, dtmTest.Month, dtmTest.Day, 11, 0, 0); // 11am
        var matchedAfter = expression.Next(dateToMatchLater);
        var matchedBefore = expression.Previous(dateToMatchEarlier);

        Assert.IsTrue(matchedAfter == new DateTime(2020, 9, 29, 14, 0, 0)); // first match is 14:00
        Assert.IsTrue(matchedBefore == new DateTime(2020, 9, 29, 10, 0, 0)); // last match is 10:00
    }

    [TestMethod]
    public void Test08_ExpressionParsing()
    {
        var expectedExpression = "23 0-20 1/2 1 *";
        var expression = mBuilder.Build(expectedExpression);

        Assert.AreEqual(expectedExpression, expression.ToString());
    }

    [TestMethod]
    public void Test09_Polling()
    {
        Assert.IsTrue(mBuilder.Build(ANY).Poll());
    }

    [TestMethod]
    public void Test10_PollRange()
    {
        var expression = mBuilder.WithValue(ParameterType.Minute, ParameterValue.Number(0)).WithValue(ParameterType.Hour, ParameterValue.Number(12)).WithRange(ParameterType.Day, ParameterValue.Number(1), ParameterValue.Number(7)).WithValue(ParameterType.DayOfWeek, ParameterValue.DayOfWeek(DayOfWeek.MON)).WithAny(ParameterType.Month).Build(); // every first monday of the month at noon
        var matches12 = expression.Next(new DateTime(2020, 10, 29, 13, 0, 0), 12).ToList(); // next 12 matches, starting at the given date and time
        var matchesDate = expression.Next(new DateTime(2020, 10, 29, 13, 0, 0), new DateTime(2021, 10, 4, 12, 0, 0)).ToList(); // all matches within the date range starting at the given date and time ending at the given date and time
        DateTime[] expected = [
            new DateTime(2020, 11, 2, 12, 0, 0),
            new DateTime(2020, 12, 7, 12, 0, 0),
            new DateTime(2021, 1, 4, 12, 0, 0),
            new DateTime(2021, 2, 1, 12, 0, 0),
            new DateTime(2021, 3, 1, 12, 0, 0),
            new DateTime(2021, 4, 5, 12, 0, 0),
            new DateTime(2021, 5, 3, 12, 0, 0),
            new DateTime(2021, 6, 7, 12, 0, 0),
            new DateTime(2021, 7, 5, 12, 0, 0),
            new DateTime(2021, 8, 2, 12, 0, 0),
            new DateTime(2021, 9, 6, 12, 0, 0),
            new DateTime(2021, 10, 4, 12, 0, 0)
            ];

        Assert.HasCount(12, matches12);
        for (var i = 0; i < 12; i++)
        {
            Assert.IsTrue(matches12[i] == expected[i]);
        }
        Assert.HasCount(12, matchesDate);
        for (var i = 0; i < 12; i++)
        {
            Assert.IsTrue(matchesDate[i] == expected[i]);
        }
    }

    [TestMethod]
    public void Test11_ValidationErrorOnCreation()
    {
        string[] misHapsAsText = ["", "Q", "5.7", "MUN", "-1"]; // value errors
        string[] misHapsAsValue = ["1/2-3", "1#2-3"]; // type errors

        for (var i = 0; i < misHapsAsText.Length; i++)
        {
            mBuilder = mBuilder.With(ParameterType.Minute, misHapsAsText[i]);
            Assert.IsTrue(mBuilder.Parameters[0].IsFault);
        }
        for (var i = 0; i < misHapsAsValue.Length; i++)
        {
            mBuilder = mBuilder.With(ParameterType.Minute, misHapsAsValue[i]);
            Assert.IsTrue(mBuilder.Parameters[0].IsFault);
        }
    }

    [TestMethod]
    public void Test12_ValidationErrorOnBuild()
    {
        string[] wrongMinutes = ["TUE", "MAR", "60"]; // valid minute values are numbers between 0 and 59
        string[] wrongHours = ["MON", "JAN", "24"]; // valid hour values are numbers between 0 and 23
        string[] wrongDays = ["0", "WED", "32"]; // valid day values are numbers between 1 and 31
        string[] wrongMonths = ["0", "WED", "13", "1#3"]; // valid month values are numbers between 1 and 12 or a MonthOfYear value
        string[] wrongDaysOfWeek = ["0", "7", "APR", "1#8", "12#1"]; // valid days of week values are numbers between 0 and 6, a DayOfWeek value, a hash (<DayOfWeek>#<1-6>)
        var count = 0;
        var errorCount = 0;

        foreach (var value in wrongMinutes)
        {
            count++;
            try
            {
                _ = mBuilder.With(ParameterType.Minute, value).Build();
            }
            catch (AggregateException ex)
            {
                Debug.WriteLine(ex);
                errorCount++;
            }
            Assert.AreEqual(count, errorCount);
        }
        count = 0;
        errorCount = 0;

        foreach (var value in wrongHours)
        {
            count++;
            try
            {
                _ = mBuilder.With(ParameterType.Hour, value).Build();
            }
            catch (AggregateException ex)
            {
                Debug.WriteLine(ex);
                errorCount++;
            }
            Assert.AreEqual(count, errorCount);
        }
        count = 0;
        errorCount = 0;

        foreach (var value in wrongDays)
        {
            count++;
            try
            {
                _ = mBuilder.With(ParameterType.Day, value).Build();
            }
            catch (AggregateException ex)
            {
                Debug.WriteLine(ex);
                errorCount++;
            }
            Assert.AreEqual(count, errorCount);
        }
        count = 0;
        errorCount = 0;

        foreach (var value in wrongMonths)
        {
            count++;
            try
            {
                _ = mBuilder.With(ParameterType.Month, value).Build();
            }
            catch (AggregateException ex)
            {
                Debug.WriteLine(ex);
                errorCount++;
            }
            Assert.AreEqual(count, errorCount);
        }
        count = 0;
        errorCount = 0;

        foreach (var value in wrongDaysOfWeek)
        {
            count++;
            try
            {
                _ = mBuilder.With(ParameterType.DayOfWeek, value).Build();
            }
            catch (AggregateException ex)
            {
                Debug.WriteLine(ex);
                errorCount++;
            }
            Assert.AreEqual(count, errorCount);
        }
        count = 0;
        errorCount = 0;

        try
        {
            _ = mBuilder.
                With(ParameterType.Minute, "62").
                With(ParameterType.Hour, "25").
                With(ParameterType.Day, "33").
                With(ParameterType.Month, "0").
                With(ParameterType.DayOfWeek, "7").
                Build(); // one expression with 5 wrong parameters -> 5 exceptions in AggregateException
        }
        catch (AggregateException ex)
        {
            count = 1;
            errorCount = ex.InnerExceptions.Count;
        }
        Assert.IsTrue(count == 1 && errorCount == 5);
    }

    [TestMethod]
    public void Test13_ValidateExpression()
    {
        Assert.IsFalse(mBuilder.Validate(" 1 2 3 4 Q"));
        Assert.IsTrue(mBuilder.Validate("30 12 1 1 *"));
    }

    [TestMethod]
    public void Test14_Humanizing()
    {
        var e = mBuilder.Build("30 0 * * MON-SAT");
        var strHum = e.Display;

        // stuff
    }

    [TestMethod]
    public void Test15_Operators()
    {
        var exprA = mBuilder.Build("30 12 1 1 *");
        var exprB = mBuilder.Build("0 12 1 1 MON");

        Assert.IsFalse(exprA.Equals(exprB));
        Assert.IsTrue(exprA != exprB);
        Assert.IsTrue(ParameterValue.Number(1).Equals(1));
        Assert.IsTrue(ParameterValue.Number(1).Equals("1"));
        Assert.IsTrue(ParameterValue.Number(1) != ParameterValue.DayOfWeek(DayOfWeek.MON));
        Assert.IsTrue(ParameterValue.DayOfWeek(DayOfWeek.MON).Equals("MON"));
        Assert.IsTrue(ParameterValue.Any().Equals("*"));
    }

    [TestMethod]
    public void Test16_MatchHashSymbol()
    {
        mBuilder.SupportSymbols = true; // support symbols
        
        var expression = mBuilder.Build("0 12 * * 1#1"); // noon of every 1st monday of every month
        var results = expression.Next(new DateTime(2020, 8, 1, 0, 0, 0), 6).ToList(); // get noon times of the next 6 first mondays of the month starting at saturday, august 1, 2020
        DateTime[] expectedDates = [
            new(2020, 8, 3, 12, 0, 0),
            new(2020, 9, 7, 12, 0, 0),
            new(2020, 10, 5, 12, 0, 0),
            new(2020, 11, 2, 12, 0, 0),
            new(2020, 12, 7, 12, 0, 0),
            new(2021, 1, 4, 12, 0, 0)
        ];

        for (var i = 0; i < results.Count; i++)
        {
            Assert.IsTrue(results[i] == expectedDates[i]);
        }
    }
}