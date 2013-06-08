/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Base Class for Algorithm.
* Algorithm MarketType Manager - Collection of Securities
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect.Models {
    
    /// <summary>
    /// MarketData MarketData Class for MarketData resolution data:
    /// </summary>
    public class Tick : MarketData {

        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        /// <summary>
        /// Type of the Tick: Trade or Quote.
        /// </summary>
        public TickType TickType = TickType.Trade;

        /// <summary>
        /// Quantity of the tick sale or quote.
        /// </summary>
        public int Quantity = 0;

        /// <summary>
        /// Exchange we are executing on.
        /// </summary>
        public string Exchange = "";

        /// <summary>
        /// Sale condition for the tick.
        /// </summary>
        public string SaleCondition = "";

        /// <summary>
        /// Bool whether this is a suspicious tick.
        /// </summary>
        public bool Suspicious = false;


        /// <summary>
        /// Bid Price for Tick - NOTE: We don't currently have quote data
        /// </summary>
        public decimal BidPrice = 0;

        /// <summary>
        /// Asking Price for the Tick - NOTE: We don't currently have quote data
        /// </summary>
        public decimal AskPrice = 0;

        /******************************************************** 
        * CLASS CONSTRUCTORS
        *********************************************************/
        /// <summary>
        /// Initialize Tick Class
        /// </summary>
        public Tick() {
            Price = 0;
            Type = MarketDataType.Tick;
            Time = new DateTime();
            TickType = TickType.Trade;
            Quantity = 0;
            Exchange = "";
            SaleCondition = "";
            Suspicious = false;
            DataType = DataType.MarketData;
        }


        /// <summary>
        /// Simple FX Tick
        /// </summary>
        /// <param name="time">Full date and time</param>
        /// <param name="symbol">Underlying Asset.</param>
        /// <param name="bid">Bid value</param>
        /// <param name="ask">Ask Value</param>
        public Tick(DateTime time, string symbol, decimal bid, decimal ask)
        {
            TickType = TickType.Quote;
            Time = time;
            Symbol = symbol;
            BidPrice = bid;
            AskPrice = ask;
            Price = bid + (ask - bid)/2;
        }


        /// <summary>
        /// FXCM Loader
        /// </summary>
        public Tick(string symbol, string line)
        {
            string[] csv = line.Split(',');

            TickType = TickType.Quote;
            Time = DateTime.ParseExact(csv[0], "yyyyMMdd HH:mm:ss.ffff", CultureInfo.InvariantCulture);
            Symbol = symbol;
            BidPrice = Convert.ToDecimal(csv[1]);
            AskPrice = Convert.ToDecimal(csv[2]);
            Price = BidPrice + (AskPrice - BidPrice) / 2;
        }


        /// <summary>
        /// Parse a tick data line from Zip files.
        /// </summary>
        /// <param name="line">CSV Line</param>
        /// <param name="baseDate">Base date for the tick</param>
        /// <param name="symbol">Symbol for this tick</param>
        public Tick(string line, string symbol, DateTime baseDate) {
            try {
                string[] csv = line.Split(',');
                this.TickType = TickType.Trade;
                base.Symbol = symbol;
                base.Time = baseDate.Date.AddMilliseconds(Convert.ToInt64(csv[0]));
                base.Price = Convert.ToDecimal(csv[1]) / 10000m;
                base.Type = MarketDataType.Tick;
                this.Quantity = Convert.ToInt32(csv[2]);

                if (csv.Length > 3) {
                    this.Exchange = csv[3];
                    this.SaleCondition = csv[4];
                    this.Suspicious = (csv[5] == "1") ? true : false;
                }
            } catch (Exception err) {
                Log.Error("Error Generating Tick: " + err.Message);
            }
        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/

    } // End Tick Class:

} // End QC Namespace
