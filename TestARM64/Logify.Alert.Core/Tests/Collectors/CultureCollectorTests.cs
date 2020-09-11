﻿#if DEBUG
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Internal.Tests {
    public class CollectorTestsBase {
        TextWriter writer;
        StringBuilder content;
        ILogger logger;

        protected string Content { get { return content.ToString(); } }
        protected ILogger Logger { get { return logger; } }

        protected void SetupCore() {
            this.content = new StringBuilder();
            this.writer = new StringWriter(content);
            this.logger = new TextWriterLogger(writer);
        }
        protected void TearDownCore() {
            this.content = null;
            this.writer.Dispose();
            this.logger = null;
        }
    }

    [TestFixture]
    public class CurrentCultureCollectorTests : CollectorTestsBase {
        CurrentCultureCollector collector;
        CultureInfo culture;
        [SetUp]
        public void Setup() {
            SetupCore();
            this.culture = CultureInfo.CurrentCulture;
            CultureInfo testCulture = new CultureInfo("en-US");
            testCulture.DateTimeFormat.AMDesignator = "my_am";
            testCulture.DateTimeFormat.PMDesignator = "my_pm";
            testCulture.DateTimeFormat.DateSeparator = "my_dateseparator";
            testCulture.DateTimeFormat.TimeSeparator = "my_timeseparator";
            testCulture.DateTimeFormat.ShortTimePattern = "my_short_time";
            testCulture.DateTimeFormat.LongTimePattern = "my_long_time";
            testCulture.DateTimeFormat.ShortDatePattern = "my_short_date";
            testCulture.DateTimeFormat.LongDatePattern = "my_long_date";
            testCulture.DateTimeFormat.MonthDayPattern = "my_month_day";
            testCulture.DateTimeFormat.YearMonthPattern = "my_year_month";
            testCulture.DateTimeFormat.FullDateTimePattern = "my_full";
            testCulture.DateTimeFormat.AbbreviatedMonthNames = new string[] { 
                "q", "w", "e", "r", "t", "y", 
                "u", "i", "o", "p", "a", "s", 
                "d"
            };
            testCulture.DateTimeFormat.MonthNames = new string[] { 
                "full_q", "full_w", "full_e", "full_r", "full_t", "full_y", 
                "full_u", "full_i", "full_o", "full_p", "full_a", "full_s", 
                "full_d"
            };
            testCulture.DateTimeFormat.DayNames = new string[] { 
                "z", "x", "c", "v", "b", "f", "g"
            };
            testCulture.DateTimeFormat.ShortestDayNames = new string[] { 
                "short_z", "short_x", "short_c", "short_v", "short_b", "short_f", "short_g"
            };
            testCulture.NumberFormat.CurrencySymbol = "@";
            testCulture.NumberFormat.PercentSymbol = "?";
            Thread.CurrentThread.CurrentCulture = testCulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            this.collector = new CurrentCultureCollector();
        }
        [TearDown]
        public void TearDown() {
            TearDownCore();
            Thread.CurrentThread.CurrentCulture = culture;
            this.collector = null;
        }

        [Test]
        public void WhenCultureIsNotCustomize() {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            collector.Process(null, Logger);
            string expected =
@"""currentCulture"":{" + "\r\n" +
@"""name"":""en-US""," + "\r\n" +
@"""englishName"":""English (United States)""," + "\r\n" +
@"""displayName"":""English (United States)""," + "\r\n" +
@"""isCultureCustomize"":false" + "\r\n" +
@"}" + "\r\n";
            Assert.AreEqual(expected, Content);
        }

        [Test]
        public void WhenCultureIsCustomize() {
            collector.Process(null, Logger);
            string expected =
@"""currentCulture"":{" + "\r\n" +
@"""name"":""en-US""," + "\r\n" +
@"""englishName"":""English (United States)""," + "\r\n" +
@"""displayName"":""English (United States)""," + "\r\n" +
@"""isCultureCustomize"":true," + "\r\n" +
    @"""details"":{" + "\r\n" +
        @"""numberFormat"":{" + "\r\n" +
            @"""currency"":{" + "\r\n" +
                @"""decimalDigits"":2," + "\r\n" +
                @"""decimalSeparator"":"".""," + "\r\n" +
                @"""groupSeparator"":"",""," + "\r\n" +
                @"""groupSize"":[""3""]," + "\r\n" +
                @"""symbol"":""@""," + "\r\n" +
                @"""negativePatternID"":0," + "\r\n" +
                @"""positivePatternID"":0" + "\r\n" +
            @"}," + "\r\n" +
            @"""number"":{" + "\r\n" +
                @"""decimalDigits"":2," + "\r\n" +
                @"""decimalSeparator"":"".""," + "\r\n" +
                @"""groupSeparator"":"",""," + "\r\n" +
                @"""groupSize"":[""3""]," + "\r\n" +
                @"""negativePatternID"":1" + "\r\n" +
            @"}," + "\r\n" +
            @"""percent"":{" + "\r\n" +
                @"""decimalDigits"":2," + "\r\n" +
                @"""decimalSeparator"":"".""," + "\r\n" +
                @"""groupSeparator"":"",""," + "\r\n" +
                @"""groupSize"":[""3""]," + "\r\n" +
                @"""symbol"":""?""," + "\r\n" +
                @"""negativePatternID"":0," + "\r\n" +
                @"""positivePatternID"":0" + "\r\n" +
            @"}," + "\r\n" +
            @"""sign"":{" + "\r\n" +
                @"""positive"":""+""," + "\r\n" +
                @"""negative"":""-""" + "\r\n" +
            @"}," + "\r\n" +
            @"""infinity"":{" + "\r\n" +
                @"""positive"":""Infinity""," + "\r\n" +
                @"""negative"":""-Infinity""" + "\r\n" +
            @"}," + "\r\n" +
            @"""perMilleSymbol"":""‰""," + "\r\n" +
            @"""NaNSymbol"":""NaN""," + "\r\n" +
            @"""digitShapes"":""None""," + "\r\n" +
            @"""nativeDigits"":[""0"",""1"",""2"",""3"",""4"",""5"",""6"",""7"",""8"",""9""]" + "\r\n" +
        @"}," + "\r\n" +
        @"""dateTime"":{" + "\r\n" +
            @"""dateSeparator"":""my_dateseparator""," + "\r\n" +
            @"""timeSeparator"":""my_timeseparator""," + "\r\n" +
            @"""firstDayOfWeek"":""Sunday""," + "\r\n" +
            @"""am"":""my_am""," + "\r\n" +
            @"""pm"":""my_pm""," + "\r\n" +
            @"""abbreviatedDayNames"":[""Sun"",""Mon"",""Tue"",""Wed"",""Thu"",""Fri"",""Sat""]," + "\r\n" +
            @"""abbreviatedMonthNames"":[""q"",""w"",""e"",""r"",""t"",""y"",""u"",""i"",""o"",""p"",""a"",""s"",""d""]," + "\r\n" +
            @"""dayNames"":[""z"",""x"",""c"",""v"",""b"",""f"",""g""]," + "\r\n" +
            @"""monthNames"":[""full_q"",""full_w"",""full_e"",""full_r"",""full_t"",""full_y"",""full_u"",""full_i"",""full_o"",""full_p"",""full_a"",""full_s"",""full_d""]," + "\r\n" +
            @"""shortestDayNames"":[""short_z"",""short_x"",""short_c"",""short_v"",""short_b"",""short_f"",""short_g""]," + "\r\n" +
            @"""pattern"":{" + "\r\n" +
                @"""shortTime"":""my_short_time""," + "\r\n" +
                @"""longTime"":""my_long_time""," + "\r\n" +
                @"""shortDate"":""my_short_date""," + "\r\n" +
                @"""longDate"":""my_long_date""," + "\r\n" +
                @"""monthDay"":""my_month_day""," + "\r\n" +
                @"""yearMonth"":""my_year_month""," + "\r\n" +
                @"""fullDateTime"":""my_full""," + "\r\n" +
                @"""sortableDateTime"":""yyyy'-'MM'-'dd'T'HH':'mm':'ss""," + "\r\n" +
                @"""universalSortableDateTime"":""yyyy'-'MM'-'dd HH':'mm':'ss'Z'""," + "\r\n" +
                @"""RFC1123"":""ddd, dd MMM yyyy HH':'mm':'ss 'GMT'""" + "\r\n" +
            @"}" + "\r\n" +
        @"}," + "\r\n" +
        @"""listSeparator"":"",""," + "\r\n" +
        @"""isRightToLeft"":false," + "\r\n" +
        @"""ANSICodePage"":1252," + "\r\n" +
        @"""EBCDICCodePage"":37," + "\r\n" +
        @"""LCID"":1033," + "\r\n" +
        @"""MacCodePage"":10000," + "\r\n" +
        @"""OEMCodePage"":437," + "\r\n" +
        @"""isNeutralCulture"":false," + "\r\n" +
        @"""ISO2"":""en""," + "\r\n" +
        @"""ISO1"":""eng""," + "\r\n" +
        @"""WinAPI"":""ENU""" + "\r\n" +
    @"}" + "\r\n" +
@"}" + "\r\n";
            Assert.AreEqual(expected, Content);
        }
    }

    [TestFixture]
    public class CurrentUICultureCollectorTests : CollectorTestsBase {
        CurrentUICultureCollector collector;
        CultureInfo culture;
        [SetUp]
        public void Setup() {
            SetupCore();
            this.culture = CultureInfo.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", false);
            this.collector = new CurrentUICultureCollector();
        }
        [TearDown]
        public void TearDown() {
            TearDownCore();
            Thread.CurrentThread.CurrentUICulture = culture;
            this.collector = null;
        }

        [Test]
        public void BeginWriteObject() {
            collector.Process(null, Logger);
            string expected =
@"""currentUICulture"":{" + "\r\n" +
@"""name"":""en-US""," + "\r\n" +
@"""englishName"":""English (United States)""," + "\r\n" +
@"""displayName"":""English (United States)""," + "\r\n" +
@"""isCultureCustomize"":false" + "\r\n" +
@"}" + "\r\n";
            Assert.AreEqual(expected, Content);
        }
    }
}
#endif