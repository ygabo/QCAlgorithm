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
using QuantConnect.Models;

namespace QuantConnect.Securities {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Enumerable Vehicle Management Class
    /// </summary>
    public class SecurityManager : IDictionary<string, Security> {

        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/

        //Internal dictionary implementation:
        private IDictionary<string, Security> _securityManager;
        private IDictionary<string, SecurityHolding> _securityHoldings;

        /******************************************************** 
        * CLASS PUBLIC VARIABLES
        *********************************************************/


        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Initialise the Algorithm Security Manager Class
        /// </summary>
        public SecurityManager() {
            _securityManager = new Dictionary<string, Security>(); 
            _securityHoldings = new Dictionary<string, SecurityHolding>();
        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/

        /// <summary>
        /// Dictionary Interface Implementation: The base add method.
        /// </summary>
        /// <param name="symbol">symbol for security we're trading</param>
        /// <param name="security">security object</param>
        public void Add(string symbol, Security security) {
            _securityManager.Add(symbol, security);
        }



        /// <summary>
        /// Wrapper around Add method to keep the API the same.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="resolution"></param>
        /// <param name="fillDataForward"></param>
        public void Add(string symbol, Resolution resolution = Resolution.Minute, bool fillDataForward = true) 
        {
            this.Add(symbol, SecurityType.Equity, resolution, fillDataForward);
        }


        /// <summary>
        /// Add a New Security we'll need data and updates for:
        /// </summary>
        /// <param name="symbol">Symbol of the Asset</param>
        /// <param name="type">Type of security</param>
        /// <param name="resolution">Resolution of Data Required</param>
        /// <param name="fillDataForward">If true, returns previous tradeBar when none are available in current second.</param>
        /// <param name="leverage">Leverage for this security, default = 1</param>
        public void Add(string symbol, SecurityType type = SecurityType.Equity, Resolution resolution = Resolution.Minute, bool fillDataForward = true, decimal leverage = 1, bool extendedMarketHours = false) {

            //Upper case sybol:
            symbol = symbol.ToUpper();

            //Maximum Data Usage: mainly RAM constraints but this has never been fully tested.
            if (GetResolutionCount(Resolution.Tick) == 10 && resolution == Resolution.Tick) {
                throw new Exception("We currently only support 10 tick assets at a time.");
            }
            if (GetResolutionCount(Resolution.Second) == 60 && resolution == Resolution.Second) {
                throw new Exception("We currently only support 60 second resolution securities at a time.");
            }
            if (GetResolutionCount(Resolution.Minute) == 500 && resolution == Resolution.Minute) {
                throw new Exception("We currently only support 500 minute assets at a time.");
            }

            //If we don't already have this asset, add it to the securities list.
            if (!_securityManager.ContainsKey(symbol)) {
                switch (type) {
                    case SecurityType.Equity:
                        this.Add(symbol, new Equity(symbol, resolution, fillDataForward, leverage, extendedMarketHours));
                        break;
                    case SecurityType.Forex:
                        this.Add(symbol, new Forex(symbol, resolution, fillDataForward, leverage, extendedMarketHours));
                        break;
                }
            } else {
                //Otherwise, we already have it, just change its resolution:
                Log.Trace("Algorithm.Markets.Add(): Changing resolution will overwrite portfolio");
                switch (type) {
                    case SecurityType.Equity:
                        _securityManager[symbol] = new Equity(symbol, resolution, fillDataForward, leverage, extendedMarketHours);
                        break;
                    case SecurityType.Forex:
                        _securityManager[symbol] = new Forex(symbol, resolution, fillDataForward, leverage, extendedMarketHours);
                        break;
                }
            }
        }
        
        
        /// <summary>
        /// Dictionary interface Implementation: The base keyvalue pair method.
        /// </summary>
        /// <param name="pair"></param>
        public void Add(KeyValuePair<string, Security> pair)
        {
            _securityManager.Add(pair.Key, pair.Value);
            _securityHoldings.Add(pair.Key, pair.Value.Holdings);
        }


        /// <summary>
        /// Dictionary Interface Implementation: Clear Dictionary
        /// </summary>
        public void Clear()
        {
            _securityManager.Clear();
        }


        /// <summary>
        /// Dictionary Interface Implementation: Contains Key Value Pair
        /// </summary>
        /// <param name="pair"></param>
        /// <returns>Bool true if contains this key</returns>
        public bool Contains(KeyValuePair<string, Security> pair)
        {
            return _securityManager.Contains(pair);
        }


        /// <summary>
        /// Dictionary Interface Implementation: Contains this symbol key
        /// </summary>
        /// <param name="symbol">Symbol we're checking for.</param>
        /// <returns>True if contains this symbol</returns>
        public bool ContainsKey(string symbol)
        {
            return _securityManager.ContainsKey(symbol);
        }


        /// <summary>
        /// Dictionary Interface Implementation: CopyTo:
        /// </summary>
        /// <param name="array">array we're outputting to</param>
        /// <param name="number">starting index of array</param>
        public void CopyTo(KeyValuePair<string, Security>[] array, int number)
        {
            _securityManager.CopyTo(array, number);
        }


        /// <summary>
        /// Dictionary Interface Implementation: Count
        /// </summary>
        public int Count
        {
            get { return _securityManager.Count; }
        }


        /// <summary>
        /// Dictionary Intface Implementation: IsReadOnly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _securityManager.IsReadOnly;  }
        }


        /// <summary>
        /// Dictionary Interface Implementaton: Remove this keyvalue pair
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, Security> pair)
        {
            return _securityManager.Remove(pair);
        }


        /// <summary>
        /// Remove this symbol security: Dictionary interface implementation
        /// </summary>
        /// <param name="symbol">string symbol we're searching for</param>
        /// <returns>true success</returns>
        public bool Remove(string symbol)
        {
            return _securityManager.Remove(symbol);
        }

        /// <summary>
        /// List of the Keys for the dictionary
        /// </summary>
        public ICollection<string> Keys
        {
            get { return _securityManager.Keys; }
        }


        /// <summary>
        /// Dictionary Interface Implementation: Try and get this security object with matching symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="security"></param>
        /// <returns></returns>
        public bool TryGetValue(string symbol, out Security security)
        {
            return _securityManager.TryGetValue(symbol, out security);
        }

        /// <summary>
        /// Get a list of the values for this dictionary
        /// </summary>
        public ICollection<Security> Values
        {
            get { return _securityManager.Values; }
        }


        /// <summary>
        /// Dictionary Interface Implementation: Get the Enumerator for this object
        /// </summary>
        /// <returns>Enumerable key value pair</returns>
        IEnumerator<KeyValuePair<string, Security>> IEnumerable<KeyValuePair<string, Security>>.GetEnumerator() {
            return _securityManager.GetEnumerator();
        }


        /// <summary>
        /// Publically accessibly enumerator for the portfolio class.
        /// </summary>
        /// <returns>Enumerator</returns>
        public IDictionary<string, SecurityHolding> GetInternalPortfolioCollection()
        {
            return _securityHoldings;
        }


        /// <summary>
        /// Dictionary Interface Implementation: Get the enumerator for this object:
        /// </summary>
        /// <returns>Enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _securityManager.GetEnumerator();
        }

        /// <summary>
        /// Indexer for the Security Manager:
        /// </summary>
        /// <param name="symbol">Symbol Index</param>
        /// <returns>Security</returns>
        public Security this[string symbol] {
            get {
                return _securityManager[symbol];
            }
            set {
                _securityManager[symbol] = value;
                _securityHoldings[symbol] = value.Holdings;
            }
        }


        /// <summary>
        /// Number of Securities that have this resolution.
        /// </summary>
        /// <param name="resolution">Resolution to look for.</param>
        /// <returns>int iCount.</returns>
        public int GetResolutionCount(Resolution resolution) {
            int count = 0;
            try {
                count = (from security in _securityManager.Values
                          where security.Resolution == resolution
                          select security.Resolution).Count();
            } catch (Exception err) {
                Log.Error("Algorithm.Market.GetResolutionCount(): " + err.Message);
            }
            return count;
        }


        /// <summary>
        /// Update the security properties, online functions with these data packets.
        /// </summary>
        /// <param name="frontier"></param>
        /// <param name="data"></param>
        public void Update(DateTime frontier, Dictionary<string, List<MarketData>> data) {
            try {

                foreach (string symbol in _securityManager.Keys)
                {
                    if (data.ContainsKey(symbol) && data[symbol].Count > 0)
                    {
                        //Loop over the data we have and update the cache:
                        foreach (MarketData dataPoint in data[symbol])
                        {
                            this[symbol].Update(frontier, dataPoint);
                        }
                    }
                    else
                    {
                        //Send in a null data and just update the time.
                        this[symbol].Update(frontier, null);
                    }
                }

            } catch (Exception err) {
                Log.Error("Algorithm.Market.Update(): " + err.Message);
            }
        }



    } // End Algorithm Vehicle Manager Class

} // End QC Namespace
