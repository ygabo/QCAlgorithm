/*
 * QUANTCONNECT.COM - 
 * GLobal Enums
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Newtonsoft.Json;

//QuantConnect Project Libraries:
using QuantConnect.Logging;

namespace QuantConnect {

    /******************************************************** 
    * GLOBAL CONST
    *********************************************************/
    /// <summary>
    /// ShortCut Date Format Strings:
    /// </summary>
    public static class DateFormat {
        public static string SixCharacter = "yyMMdd";
        public static string EightCharacter = "yyyyMMdd";
        public static string JsonFormat = "yyyy-MM-ddThh:mm:ss";
        public const string DB = "yyyy-MM-dd HH:mm:ss";
        public const string UI = "yyyyMMdd HH:mm:ss";
        public const string EXT = "yyyy-MM-dd HH:mm:ss";
    }

    /******************************************************** 
    * GLOBAL ENUMS DEFINITIONS
    *********************************************************/
    
    /// <summary>
    /// Types of Run Mode: Series, Parallel or Auto.
    /// </summary>
    public enum RunMode { 
        Automatic,
        Series,
        Parallel
    }

    /******************************************************** 
    * GLOBAL ENUMS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Type of Tradable Security / Underlying Asset
    /// </summary>
    public enum SecurityType {
        Base,
        Equity,
        Option,
        Commodity,
        Forex,
        Future
    }
    
    /// <summary>
    /// Types of Data:
    /// </summary>
    public enum DataType { 
        MarketData,         // Tradable -- Build a portfolio
        SentimentData       // Sentiment -- Indicator Data about a Security.
    }

    /// <summary>
    /// Market Data Type Definition:
    /// </summary>
    public enum MarketDataType {
        Base,
        TradeBar,
        Tick
    }

    /// <summary>
    /// Data types available from spryware decoding
    /// </summary>
    public enum TickType {
        Trade,
        Quote
    }

    /// <summary>
    /// MetaData Options: Twitter, Estimize, News etc.
    /// </summary>
    public enum SentimentDataType { 
        Base,
        Estimize,
        StockPulse
    } 

    /// <summary>
    /// Resolution of data requested:
    /// </summary>
    public enum Resolution {
        Tick,
        Second,
        Minute
    }   

    /// <summary>
    /// State of the Instance:
    /// </summary>
    public enum State {
        Busy,
        Idle
    }

    /// <summary>
    /// Get the file status
    /// </summary>
    public enum FileStatus { 
        Active,
        Deleted
    }

    /// <summary>
    /// Use standard HTTP Status Codes for communication between servers:
    /// </summary>
    public enum ResponseCode {
        OK = 200,
        Unauthorized = 401,
        NotFound = 404,
        NotImplemented = 501,
        MalformedRequest = 502,
        CompilerError = 503
    }


    /// <summary>
    /// Types of Statistics the Algorithm Manager Records.
    /// </summary>
    public enum Statistic {
        SharpeRatio,
        Drawdown,
        NetProfit,
        Expectancy,
        AverageWin,
        AverageLoss,
        AverageAnnualReturn,
        TotalTrades,
        WinRate,
        LossRate,
        ProfitLossRatio,
        TradeFrequency
    }

    /// <summary>
    /// Trade Frequency Options
    /// </summary>
    public enum TradeFrequency { 
        Secondly,
        Minutely,
        Hourly,
        Daily,
        Weekly
    }


    /// <summary>
    /// enum Period - Enum of all the analysis periods, AS integers. Reference "Period" Array to access the values
    /// </summary>
    public enum Period {
        TenSeconds = 10,
        ThirtySeconds = 30,
        OneMinute = 60,
        TwoMinutes = 120,
        ThreeMinutes = 180,
        FiveMinutes = 300,
        TenMinutes = 600,
        FifteenMinutes = 900,
        TwentyMinutes = 1200,
        ThirtyMinutes = 1800,
        OneHour = 3600,
        TwoHours = 7200,
        FourHours = 14400,
        SixHours = 21600
    }
    

    /******************************************************** 
    * GLOBAL CUSTOM ITERATORS
    *********************************************************/
    /// <summary>
    /// Data available at every chart point
    /// </summary>
    public class ChartPoint {
        DateTime time;
        decimal price;
        string tag;

        public ChartPoint(DateTime time, decimal price, string tag = "") {
            this.time = time;
            this.price = price;
            this.tag = tag;
        }
    }

    /// <summary>
    /// Charting Caching Storage - Custom charting lines
    /// </summary>
    public class ChartList : List<ChartPoint> {

        public void Add(DateTime time, decimal price, string tag = "") {
            this.Add(new ChartPoint(time, price, tag));
        }
    }

    /// <summary>
    /// Base class for Dictionary-Double Properties using in the equity property class.
    ///  - When creating online exp averages, etc, only calculate the time periods requested not all of them.
    /// </summary>
    public class PeriodIndexedDictionary<TVal> {
        public Dictionary<Period, TVal> RawData;
        /// <summary>
        /// Dictionary constructor
        /// </summary>
        public PeriodIndexedDictionary() {
            RawData = new Dictionary<Period, TVal>();
        }

        /// <summary>
        /// PeriodIndexedDictionary - Control access and setting of the value results to minimise work
        /// </summary>
        public TVal this[Period period] {
            get {
                lock (RawData) {
                    if (!RawData.ContainsKey(period)) {
                        RawData.Add(period, default(TVal));
                        //Now we know we need this information, update it with the real stuff instead of default.
                        Update(period);
                    }
                    return RawData[period];
                }
            }
            set {
                lock (RawData) {
                    if (!RawData.ContainsKey(period)) {
                        RawData.Add(period, value);
                    } else {
                        RawData[period] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Virtual for override in the later 
        /// </summary>
        public virtual void Update(Period period) {
            RawData[period] = default(TVal);
        }
    }




    /******************************************************** 
    * GLOBAL MARKETS
    *********************************************************/
    /// <summary>
    /// Global Market Short Codes and their full versions: (used in tick objects)
    /// </summary>
    public static class MarketCodes {

        // US Market Codes
        public static Dictionary<string, string> US = new Dictionary<string, string>() {
            {"A", "American Stock Exchange"},
            {"B", "Boston Stock Exchange"},
            {"C", "National Stock Exchange"},
            {"D", "FINRA ADF"},
            {"I", "International Securities Exchange"},
            {"J", "Direct Edge A"},
            {"K", "Direct Edge X"},
            {"M", "Chicago Stock Exchange"},
            {"N", "New York Stock Exchange"},
            {"P", "Nyse Arca Exchange"},
            {"Q", "NASDAQ OMX"},
            {"T", "NASDAQ OMX"},
            {"U", "OTC Bulletin Board"},
            {"u", "Over-the-Counter trade in Non-NASDAQ issue"},
            {"W", "Chicago Board Options Exchange"},
            {"X", "Philadelphia Stock Exchange"},
            {"Y", "BATS Y-Exchange, Inc"},
            {"Z", "BATS Exchange, Inc"}
        };

        //Canada Market Short Codes:
        public static Dictionary<string, string> Canada = new Dictionary<string, string>() {
            {"T", "Toronto"},
            {"V", "Venture"}
        };
    }


    /// <summary>
    /// US Public Holidays - Not Tradeable:
    /// </summary>
    public static class USHoliday {

        public static List<DateTime> Dates = new List<DateTime>() { 
            /* New Years Day*/
            new DateTime(1998, 01, 01),
            new DateTime(1999, 01, 01),
            new DateTime(2001, 01, 01),
            new DateTime(2002, 01, 01),
            new DateTime(2003, 01, 01),
            new DateTime(2004, 01, 01),
            new DateTime(2006, 01, 02),
            new DateTime(2007, 01, 01),
            new DateTime(2008, 01, 01),
            new DateTime(2009, 01, 01),
            new DateTime(2010, 01, 01),
            new DateTime(2011, 01, 01),
            new DateTime(2012, 01, 02),
            new DateTime(2013, 01, 01),
            new DateTime(2014, 01, 01),
            
            /* Day of Mouring */
            new DateTime(2007, 01, 02),

            /* World Trade Center */
            new DateTime(2001, 09, 11),
            new DateTime(2001, 09, 12),
            new DateTime(2001, 09, 13),
            new DateTime(2001, 09, 14),

            /* Regan Funeral */
            new DateTime(2004, 06, 11),

            /* Hurricane Sandy */
            new DateTime(2012, 10, 29),
            new DateTime(2012, 10, 30),

            /* Martin Luther King Jnr Day*/
            new DateTime(1998, 01, 19),
            new DateTime(1999, 01, 18),
            new DateTime(2000, 01, 17),
            new DateTime(2001, 01, 15),
            new DateTime(2002, 01, 21),
            new DateTime(2003, 01, 20),
            new DateTime(2004, 01, 19),
            new DateTime(2005, 01, 17),
            new DateTime(2006, 01, 16),
            new DateTime(2007, 01, 15),
            new DateTime(2008, 01, 21),
            new DateTime(2009, 01, 19),
            new DateTime(2010, 01, 18),
            new DateTime(2011, 01, 17),
            new DateTime(2012, 01, 16),
            new DateTime(2013, 01, 21),
            new DateTime(2014, 01, 20),

            /* Washington / Presidents Day */
            new DateTime(1998, 02, 16),
            new DateTime(1999, 02, 15),
            new DateTime(2000, 02, 21),
            new DateTime(2001, 02, 19),
            new DateTime(2002, 02, 18),
            new DateTime(2003, 02, 17),
            new DateTime(2004, 02, 16),
            new DateTime(2005, 02, 21),
            new DateTime(2006, 02, 20),
            new DateTime(2007, 02, 19),
            new DateTime(2008, 02, 18),
            new DateTime(2009, 02, 16),
            new DateTime(2010, 02, 15),
            new DateTime(2011, 02, 21),
            new DateTime(2012, 02, 20),
            new DateTime(2013, 02, 18),
            new DateTime(2014, 02, 17),

            /* Good Friday */
            new DateTime(1998, 04, 10),
            new DateTime(1999, 04, 02),
            new DateTime(2000, 04, 21),
            new DateTime(2001, 04, 13),
            new DateTime(2002, 03, 29),
            new DateTime(2003, 04, 18),
            new DateTime(2004, 04, 09),
            new DateTime(2005, 03, 25),
            new DateTime(2006, 04, 14),
            new DateTime(2007, 04, 06),
            new DateTime(2008, 03, 21),
            new DateTime(2009, 04, 10),
            new DateTime(2010, 04, 02),
            new DateTime(2011, 04, 22),
            new DateTime(2012, 04, 06),
            new DateTime(2013, 03, 29),
            new DateTime(2014, 04, 18),

            /* Memorial Day */
            new DateTime(1998, 05, 25),
            new DateTime(1999, 05, 31),
            new DateTime(2000, 05, 29),
            new DateTime(2001, 05, 28),
            new DateTime(2002, 05, 27),
            new DateTime(2003, 05, 26),
            new DateTime(2004, 05, 31),
            new DateTime(2005, 05, 30),
            new DateTime(2006, 05, 29),
            new DateTime(2007, 05, 28),
            new DateTime(2008, 05, 26),
            new DateTime(2009, 05, 25),
            new DateTime(2010, 05, 31),
            new DateTime(2011, 05, 30),
            new DateTime(2012, 05, 28),
            new DateTime(2013, 05, 27),
            new DateTime(2014, 05, 26),

            /* Independence Day */
            new DateTime(1998, 07, 03),
            new DateTime(1999, 07, 05),
            new DateTime(2000, 07, 04),
            new DateTime(2001, 07, 04),
            new DateTime(2002, 07, 04),
            new DateTime(2003, 07, 04),
            new DateTime(2004, 07, 05),
            new DateTime(2005, 07, 04),
            new DateTime(2006, 07, 04),
            new DateTime(2007, 07, 04),
            new DateTime(2008, 07, 04),
            new DateTime(2009, 07, 03),
            new DateTime(2010, 07, 05),
            new DateTime(2011, 07, 04),
            new DateTime(2012, 07, 04),
            new DateTime(2013, 07, 04),
            new DateTime(2014, 07, 04),
            new DateTime(2014, 07, 04),

            /* Labour Day */
            new DateTime(1998, 09, 07),
            new DateTime(1999, 09, 06),
            new DateTime(2000, 09, 04),
            new DateTime(2001, 09, 03),
            new DateTime(2002, 09, 02),
            new DateTime(2003, 09, 01),
            new DateTime(2004, 09, 06),
            new DateTime(2005, 09, 05),
            new DateTime(2006, 09, 04),
            new DateTime(2007, 09, 03),
            new DateTime(2008, 09, 01),
            new DateTime(2009, 09, 07),
            new DateTime(2010, 09, 06),
            new DateTime(2011, 09, 05),
            new DateTime(2012, 09, 03),
            new DateTime(2013, 09, 02),
            new DateTime(2014, 09, 01),

            /* Thanksgiving Day */
            new DateTime(1998, 11, 26),
            new DateTime(1999, 11, 25),
            new DateTime(2000, 11, 23),
            new DateTime(2001, 11, 22),
            new DateTime(2002, 11, 28),
            new DateTime(2003, 11, 27),
            new DateTime(2004, 11, 25),
            new DateTime(2005, 11, 24),
            new DateTime(2006, 11, 23),
            new DateTime(2007, 11, 22),
            new DateTime(2008, 11, 27),
            new DateTime(2009, 11, 26),
            new DateTime(2010, 11, 25),
            new DateTime(2011, 11, 24),
            new DateTime(2012, 11, 22),
            new DateTime(2013, 11, 28),
            new DateTime(2014, 11, 27),

            /* Christmas 1998-2014 */
            new DateTime(1998, 12, 25),
            new DateTime(1999, 12, 24),
            new DateTime(2000, 12, 25),
            new DateTime(2001, 12, 25),
            new DateTime(2002, 12, 25),
            new DateTime(2003, 12, 25),
            new DateTime(2004, 12, 24),
            new DateTime(2005, 12, 26),
            new DateTime(2006, 12, 25),
            new DateTime(2007, 12, 25),
            new DateTime(2008, 12, 25),
            new DateTime(2009, 12, 25),
            new DateTime(2010, 12, 24),
            new DateTime(2011, 12, 26),
            new DateTime(2012, 12, 25),
            new DateTime(2013, 12, 25),
            new DateTime(2014, 12, 25)
        };
    }
} // End QC Namespace:
