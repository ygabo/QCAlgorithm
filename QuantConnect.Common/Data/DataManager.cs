/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Base Class for Algorithm.
* DataManager - Helper routines for the algorithm controller.
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System.Collections.Generic;
using System.Linq;

namespace QuantConnect.Models {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Enumerable Data Management Class
    /// </summary>
    public class DataManager {

        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/
        private int _totalCount = 0;
        private int _sentimentCount = 0;
        private int _marketDataCount = 0;

        /******************************************************** 
        * CLASS PUBLIC VARIABLES
        *********************************************************/
        /// <summary>
        /// The market data the user's requested
        /// </summary>
        public Dictionary<SecurityType, Dictionary<string, Resolution>> MarketData;
        private Dictionary<string, bool> MarketDataFillForward;
        private Dictionary<string, bool> MarketDataExtendedHours;
        
        /// <summary>
        /// Sentiment data user's requested
        /// </summary>
        public Dictionary<SentimentDataType, List<string>> SentimentData;

        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Initialise the Generic Data Manager Class
        /// </summary>
        public DataManager() {
            MarketData = new Dictionary<SecurityType, Dictionary<string, Resolution>>();

            MarketData.Add(SecurityType.Equity, new Dictionary<string, Resolution>());
            MarketData.Add(SecurityType.Forex, new Dictionary<string, Resolution>());
            MarketData.Add(SecurityType.Future, new Dictionary<string, Resolution>());
            MarketData.Add(SecurityType.Option, new Dictionary<string, Resolution>());

            MarketDataFillForward = new Dictionary<string, bool>();
            MarketDataExtendedHours = new Dictionary<string, bool>();
            SentimentData = new Dictionary<SentimentDataType, List<string>>();
        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// Get the total count of assets:
        /// </summary>
        public int TotalCount {
            get { return _totalCount; }
        }

        /// <summary>
        /// Get the total sentiment count:
        /// </summary>
        public int SentimentCount
        {
            get { return _sentimentCount; }
        }

        /// <summary>
        /// Total number of market data entries:
        /// </summary>
        public int MarketDataCount
        {
            get { return _marketDataCount; }
        }

        /// <summary>
        /// If the market data consists of only equities streams:
        /// </summary>
        public bool IsOnlyEquities
        {
            get { return (MarketData[SecurityType.Equity].Count == _totalCount); }
        }

        /// <summary>
        /// If the market data consists of only forex streams:
        /// </summary>
        public bool IsOnlyForex
        {
            get { return (MarketData[SecurityType.Forex].Count == _totalCount); }
        }


        /// <summary>
        /// If any of the requests needed extended hours return true.
        /// </summary>
        public bool HasExtendedMarketHours
        {
            get
            { return MarketDataExtendedHours.Keys.Any(symbol => MarketDataExtendedHours[symbol]); }
        }


        /// <summary>
        /// List of the securities types we've requested:
        /// </summary>
        public List<SecurityType> SecurityTypes
        {
            get
            {
                List<SecurityType> types = new List<SecurityType>();
                foreach (SecurityType securityType in MarketData.Keys)
                {
                    if (MarketData[securityType].Count > 0) types.Add(securityType);
                }
                return types;
            }
        }


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// If we don't have this sentiment data, add it to our list t
        /// </summary>
        /// <param name="type">Type of Sentiment Data required</param>
        /// <param name="symbol">Symbol of the requested sentiment data.</param>
        public void Add(SentimentDataType type, string symbol)
        {
            //Convert it all to uper case
            symbol = symbol.ToUpper();
            
            if (SentimentData.ContainsKey(type)) {
                if (SentimentData[type].Contains(symbol)) {
                    return;
                } else {
                    SentimentData[type].Add(symbol);
                    _totalCount++;
                    _sentimentCount++;
                }
            } else { 
                //Add a new type of sentiment:
                SentimentData.Add(type, new List<string>());
                SentimentData[type].Add(symbol);
                _totalCount++;
                _sentimentCount++;
            }
        }

        /// <summary>
        /// Add Market Data Required
        /// </summary>
        /// <param name="security">Market Data Asset</param>
        /// <param name="symbol">Symbol of the asset we're like</param>
        /// <param name="resolution">Resolution of Asset Required</param>
        /// <param name="fillDataForward">when there is no data pass the last tradebar forward</param>
        /// <param name="extendedMarketHours">Request premarket data as well when true </param>
        public void Add(SecurityType security, string symbol, Resolution resolution = Resolution.Minute, bool fillDataForward = true, bool extendedMarketHours = false)
        {
            //Convert it all to uper case
            symbol = symbol.ToUpper();

            //Add the security
            if (!MarketData.ContainsKey(security)) {
                MarketData.Add(security, new Dictionary<string, Resolution>());
            }

            //If we dont already have this symbol
            if (MarketData[security].ContainsKey(symbol) == false) {
                MarketData[security].Add(symbol, resolution);
                MarketDataFillForward.Add(symbol, fillDataForward);
                MarketDataExtendedHours.Add(symbol, extendedMarketHours);
                _totalCount++;
                _marketDataCount++;
            } else {
                //Update this symbol resolution
                MarketData[security][symbol] = resolution;
                MarketDataFillForward[symbol] = fillDataForward;
                MarketDataExtendedHours[symbol] = extendedMarketHours;
            }
        }


        /// <summary>
        /// Check if this symbol was a fill forward request,
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool IsFillForward(string symbol) {
            //Convert it all to uper case
            symbol = symbol.ToUpper();
            if (MarketDataFillForward.ContainsKey(symbol)) {
                return MarketDataFillForward[symbol];
            } else {
                return false;
            }
        }


        /// <summary>
        /// Check if this symbol was an extended market hours request
        /// </summary>
        /// <param name="symbol">Security symbol</param>
        /// <returns>true if user wants premarket data</returns>
        public bool IsExtendedMarketHours(string symbol) {
            //Convert it all to uper case
            symbol = symbol.ToUpper();
            if (MarketDataExtendedHours.ContainsKey(symbol)) {
                return MarketDataExtendedHours[symbol];
            } else {
                return false;
            }
        }

    } // End Algorithm MetaData Manager Class

} // End QC Namespace
