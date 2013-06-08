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
using System.Linq;
using System.Text;

using QuantConnect.Logging;

namespace QuantConnect.Models {

    /// <summary>
    /// Twitter Sentiment Class:
    /// </summary>
    public class StockPulse : SentimentData {

        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        /// <summary>
        /// StockPulse Buzz and Sentiment Variables:
        /// </summary>
        public float Buzz;

        /// <summary>
        /// Sentiment + or - indicator.
        /// </summary>
        public float Sentiment;


        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Initialise the StockPulse Object:
        /// </summary>
        /// <param name="csvRaw">Raw CSV Line</param>
        public StockPulse(string csvRaw) { 
            string[] csv = csvRaw.Split(',');
            this.Time = DateTime.Parse(csv[0]);
            this.Symbol = csv[1].ToUpper();
            this.Buzz = Convert.ToSingle(csv[2]);
            this.Sentiment = Convert.ToSingle(csv[3]);
            this.Type = SentimentDataType.StockPulse;
            this.DataType = DataType.SentimentData;
        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/



        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
    }

} // End QC Namespace
