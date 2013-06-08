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

using QuantConnect;
using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect.Models {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/

    /// <summary>
    /// Base class for financial data objects:
    /// </summary>
    public class MarketData : DataBase {

        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        /// <summary>
        /// Id of this data tick
        /// </summary>
        public int Id = 0;


        /// <summary>
        /// MarketData entities have a price:
        /// </summary>
        public decimal Price;

        /// <summary>
        /// Type of the Market Data: Tick or TradeBar:
        /// </summary>
        public MarketDataType Type;

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/

    } // End Market Data Class


} // End QC Namespace
