/*
* QUANTCONNECT.COM - 
* QC.Time - Basic Time Helper Classes
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections.Generic;

//QuantConnect Project Libraries:
using QuantConnect.Logging;
using QuantConnect.Securities;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/

    /// <summary>
    /// Basic time helper classes.
    /// </summary>
    public class Time {

        /******************************************************** 
        * CLASS VARIABLES:
        *********************************************************/
        /// <summary>
        /// Storage for blocking timer:
        /// </summary>
        private static Dictionary<string, DateTime> timedRepeat = new Dictionary<string, DateTime>();


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Create a C# DateTime from a UnixTimestamp
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            System.DateTime time = DateTime.Now;
            try {
                // Unix timestamp is seconds past epoch
                time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                time = time.AddSeconds(unixTimeStamp);
            } catch (Exception err) {
                Log.Error("Time.UnixTimeStampToDateTime(): " + unixTimeStamp.ToString() + err.Message);
            }
            return time;
        }

        /// <summary>
        /// Convert a Datetime to Unix Timestamp
        /// </summary>
        public static double DateTimeToUnixTimeStamp(DateTime time) {
            double timestamp = 0;
            try {
                timestamp = (time - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
            } catch (Exception err) {
                Log.Error("Time.DateTimeToUnixTimeStamp(): " + time.ToOADate().ToString() + err.Message);
            }
            return timestamp;
        }

        /// <summary>
        /// Get the current timestamp:
        /// </summary>
        public static double TimeStamp() {
            return Time.DateTimeToUnixTimeStamp(DateTime.UtcNow);
        }

        /// <summary>
        /// Parse a standard YY MM DD date into a DateTime.
        /// </summary>
        public static DateTime ParseDate(string dateToParse) {
            DateTime date = DateTime.Now;
            try {
                //First try the exact option:
                try {
                    date = DateTime.ParseExact(dateToParse, DateFormat.SixCharacter, System.Globalization.CultureInfo.InvariantCulture);
                } catch {

                    try {
                        date = DateTime.ParseExact(dateToParse, DateFormat.EightCharacter, System.Globalization.CultureInfo.InvariantCulture);
                    } catch {

                        try {
                            date = DateTime.ParseExact(dateToParse.Substring(0, 19), DateFormat.JsonFormat, System.Globalization.CultureInfo.InvariantCulture);

                        } catch {
                            if (DateTime.TryParse(dateToParse, out date) == false) {
                                Log.Error("Time.ParseDate(): Malformed Date: " + dateToParse);
                            } else {
                                return date;
                            }
                        }
                    }                        
                }                    
            } catch (Exception err) {
                Log.Error("Time.ParseDate(): " + err.Message);
            }
            return date;
        }



        /// <summary>
        /// Parse a DB time to C# DateTime.
        /// </summary>
        /// <param name="DBDate">DateTime string from database</param>
        /// <returns>DateTime.</returns>
        public static DateTime ParseDBTime(string DBDate) {
            try {
                return DateTime.ParseExact(DBDate, DateFormat.DB, System.Globalization.CultureInfo.InvariantCulture);
            } catch {
                try {
                    return DateTime.ParseExact(DBDate, "d/M/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                } catch {
                    return new DateTime();
                }
            }
        }


        /// <summary>
        /// Define an enumerable Date Range:
        /// </summary>
        /// <param name="from">start date</param>
        /// <param name="thru">end date</param>
        /// <returns>Enumerable Date Range:</returns>
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru) {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }


        /// <summary>
        /// Define an enumerable Date Range:
        /// </summary>
        /// <param name="securities">Securities we have in portfolio</param>
        /// <param name="from">start date</param>
        /// <param name="thru">end date</param>
        /// <returns>Enumerable Date Range:</returns>
        public static IEnumerable<DateTime> EachTradeableDay(SecurityManager securities, DateTime from, DateTime thru) {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1)) {
                if (Time.TradableDate(securities, day)) {
                    yield return day;
                }
            }   
        }


        /// <summary>
        /// Make sure this date is not a holiday, or weekend:
        /// </summary>
        /// <param name="securities">Security Manager</param>
        /// <param name="day">DateTime to check if trade-able.</param>
        /// <returns>True if Tradeable</returns>
        public static bool TradableDate(SecurityManager securities, DateTime day) {
            bool tradeable = false;
            try {
                foreach (Security security in securities.Values)
                {
                    switch (security.Type)
                    {
                        case SecurityType.Equity:
                            //Add 12 hours to make it 12 midday each day
                            if (((Equity)security).Exchange.DateIsOpen(day)) tradeable = true;
                            break;

                        case SecurityType.Forex:
                            //Add 17 hours to make it 5pm each day, FX is open every day except saturday at 5pm
                            if (((Forex)security).Exchange.DateIsOpen(day)) tradeable = true;
                            break;
                    }
                }
            } catch (Exception err) {
                Log.Error("Time.TradeableDate(): " + err.Message);
            }

            return tradeable;
        }


        /// <summary>
        /// Number of Tradeable Dates within this Period.
        /// </summary>
        /// <param name="securities">Securities we're trading</param>
        /// <param name="start">Start of Date Loop</param>
        /// <param name="finish">End of Date Loop</param>
        /// <returns>Number of Dates</returns>
        public static int TradeableDates(SecurityManager securities, DateTime start, DateTime finish) {
            int count = 0;
            try {
                foreach (DateTime day in Time.EachDay(start, finish)) {
                    if (Time.TradableDate(securities, day)) {
                        count++;
                    }
                }
            } catch (Exception err) {
                Log.Error("Time.TradeableDates(): " + err.Message);
            }
            return count;
        }



        /// <summary>
        /// If more than the timeout has passed,
        /// </summary>
        /// <param name="key">name of this loop</param>
        /// <param name="timeout">seconds delay for loop</param>
        /// <returns>true if time out</returns>
        public static bool TimedRepeat(string key, int timeout) {

            try {
                if (timedRepeat.ContainsKey(key) == false) {
                    //We don't have this already:
                    timedRepeat.Add(key, DateTime.Now);
                    //Return true the first time:
                    return true;
                } else { 
                    //We already have this, check in with this:
                    if ((DateTime.Now - timedRepeat[key]) > TimeSpan.FromSeconds(timeout)) {
                        timedRepeat[key] = DateTime.Now;
                        return true;
                    }
                }
            } catch (Exception err) {
                Log.Error("Time.TimedRepeat(): " + err.Message);
            }
            return false;
        }


    } // End Time Class

} // End QC Namespace
