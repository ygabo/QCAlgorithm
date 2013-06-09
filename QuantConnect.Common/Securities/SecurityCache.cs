/*
* QUANTCONNECT.COM: Secuity Cache
* Common caching class for storing historical ticks etc.
*/

/**********************************************************
 * USING NAMESPACES
 **********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

//QuantConnect Libraries:
using QuantConnect;
using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect.Securities {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Common Caching Spot For Market Data and Averaging. 
    /// </summary>
    public class SecurityCache {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        /// <summary>
        /// Asset we're operating on.
        /// </summary>
        public virtual Security Vehicle { get; set; }

        /// <summary>
        /// Cache for the orders processed
        /// </summary>
        public List<Order> OrderCache;                //Orders Cache

        /// <summary>
        /// Last data for this security.
        /// </summary>
        private MarketData _lastData;
        
        /// <summary>
        /// Incoming Data Cached.
        /// </summary>
        public List<MarketData> DataCache;             //Cache for entire loaded model

        //DATA NAITIVE VECTORS
        public int cacheRows = 24000;                  //Per Seconds MarketData samples
        public int cacheCols = 8;                     //Cols for storing in naitive format
        public decimal[][] primaryCache;                //Storage for the Naitive decimals Depth Cache
        public int primaryCacheIdx;

        /// <summary>
        /// Colour Mark Caches for Algo Design:
        /// </summary>
        public Dictionary<Color, ChartList> colorMarkCache;


        /******************************************************** 
        * CONSTRUCTOR/DELEGATE DEFINITIONS
        *********************************************************/
        /// <summary>
        /// Start a new Cache for the set Index Code
        /// </summary>
        public SecurityCache(Security vehicle) {
            this.Vehicle = vehicle;
                
            //Reset Pointers:
            primaryCacheIdx = 0;

            //ORDER CACHES:
            OrderCache = new List<Order>();

            //DATA CACHES
            DataCache = new List<MarketData>();

            //MATH CACHE: STORE AS LOW LEVEL OBJECT FOR SPEED
            primaryCache = Allocate(cacheCols, cacheRows);

            // CHARTING CACHES:
            colorMarkCache = new Dictionary<Color, ChartList>();               
        }


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Allocate the data needed for the primary caches:
        /// </summary>
        /// <param name="n">Number of spots to free for n and m decimal arrays</param>
        /// <returns>Initalised decimal[][] ready to be set</returns>
        private static decimal[][] Allocate(int n, int m) {
            decimal[][] dResult = new decimal[n][];
            for (int i = 0; i < n; i++) {
                dResult[i] = new decimal[m];
            }
            return dResult;
        }



        /// <summary>
        /// Add the mark to the colour cache for dynamic graphing.
        /// </summary>
        public virtual void AddMark(Color color, string text) {
            if (!colorMarkCache.ContainsKey(color)) {
                colorMarkCache.Add(color, new ChartList());
            }
            colorMarkCache[color].Add(Vehicle.Time, Vehicle.Price, text);
        }



        /// <summary>
        /// Add a list of new MarketData samples to the cache
        /// </summary>
        public virtual void AddData(MarketData data) {
            //Only add to the database when its not in use.
            lock (DataCache) {
                //Record as Last Added Packet:
                _lastData = data;
                //Add it to the depth cache:
                DataCache.Add(data);
            }
        }



        /// <summary>
        /// Get Last Data Packet Recieved for this Vehicle.
        /// </summary>
        /// <returns></returns>
        public virtual MarketData GetData() {
            return _lastData;
        }



        /// <summary>
        /// Add a TransOrderDirection
        /// </summary>
        public virtual void AddOrder(Order order) {
            lock (OrderCache) {
                OrderCache.Add(order);
            }
        }



        /// <summary>
        /// Reset as many of the Cache's as possible.
        /// </summary>
        public virtual void Reset() {
            //Data Cache
            DataCache = new List<MarketData>();
            _lastData = new MarketData();
                
            //Order Cache:
            OrderCache = new List<Order>();

            //Naitive Cache:
            primaryCacheIdx = 0;
            primaryCache = Allocate(cacheCols, cacheRows);
        }


    } //End Cache

} //End Namespace