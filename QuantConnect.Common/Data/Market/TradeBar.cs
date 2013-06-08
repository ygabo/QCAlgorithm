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

using QuantConnect;
using QuantConnect.Logging;

namespace QuantConnect.Models {

    /// <summary>
    /// TradeBar MarketData Class for second and minute resolution data:
    /// </summary>
    public class TradeBar : MarketData {

        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        /// <summary>
        /// Public variable volume:
        /// </summary>
        public long Volume;
        
        /// <summary>
        /// Public variable opening price.
        /// </summary>
        public decimal Open;

        
        /// <summary>
        /// Public variable High Price:
        /// </summary>
        public decimal High;


        /// <summary>
        /// Public Variable Low Price
        /// </summary>
        public decimal Low;


        /// <summary>
        /// Closing price of the tradebar
        /// </summary>
        public decimal Close;

        /******************************************************** 
        * CLASS CONSTRUCTORS
        *********************************************************/
        /// <summary>
        /// Default Initializer:
        /// </summary>
        public TradeBar() {
            Symbol = "";
            Time = new DateTime();
            Open = 0; High = 0;
            Low = 0; Close = 0;
            Volume = 0;
            Type = MarketDataType.TradeBar;
            DataType = DataType.MarketData;
        }



        /// <summary>
        /// Parse a line from the CSV's into our trade bars.
        /// </summary>
        /// <param name="symbol">Symbol for this tick</param>
        /// <param name="baseDate">Base date of this tick</param>
        /// <param name="line">CSV from data files.</param>
        public TradeBar(SecurityType type, string line, string symbol, DateTime baseDate) {
            try {
                string[] csv = line.Split(',');
                //Parse the data into a trade bar:
                this.Symbol = symbol;
                base.Type = MarketDataType.TradeBar;
                decimal scaleFactor = 10000m;

                switch (type)
                {
                    case SecurityType.Equity:
                        Time = baseDate.Date.AddMilliseconds(Convert.ToInt32(csv[0]));
                        Open = Convert.ToDecimal(csv[1]) / scaleFactor;
                        High = Convert.ToDecimal(csv[2]) / scaleFactor;
                        Low = Convert.ToDecimal(csv[3]) / scaleFactor;
                        Close = Convert.ToDecimal(csv[4]) / scaleFactor;
                        Volume = Convert.ToInt64(csv[5]);
                        break;
                    case SecurityType.Forex:
                        Time = DateTime.ParseExact(csv[0], "yyyyMMdd HH:mm:ss.ffff", CultureInfo.InvariantCulture);
                        Open = Convert.ToDecimal(csv[1]);
                        High = Convert.ToDecimal(csv[2]);
                        Low = Convert.ToDecimal(csv[3]);
                        Close = Convert.ToDecimal(csv[4]);
                        break;
                }

                base.Price = Close;
            } catch (Exception err) {
                Log.Error("DataModels: TradeBar(): Error Initializing - " + type + " - " + err.Message + " - " + line);
            }
        }


        /// <summary>
        /// Initialize Trade Bar with OHLC Values:
        /// </summary>
        /// <param name="time">DateTime Timestamp of the bar</param>
        /// <param name="symbol">Market MarketType Symbol</param>
        /// <param name="open">Decimal Opening Price</param>
        /// <param name="high">Decimal High Price of this bar</param>
        /// <param name="low">Decimal Low Price of this bar</param>
        /// <param name="close">Decimal Close price of this bar</param>
        public TradeBar(DateTime time, string symbol, decimal open, decimal high, decimal low, decimal close, long volume) {
            base.Time = time;
            base.Symbol = symbol;
            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
            this.Volume = volume;
            base.Price = close;
            base.Type = MarketDataType.TradeBar;
        }


        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// Most recent, representative price
        /// </summary>
        public new decimal Price {
            get { return Close; }
        }

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/

    } // End Trade Bar Class

} // End QC Namespace
