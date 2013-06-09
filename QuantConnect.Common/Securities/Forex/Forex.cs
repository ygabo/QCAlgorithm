/*
* QUANTCONNECT.COM:
* FOREX Class - Base class for FX Objects, Extension of Security
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using QuantConnect;
using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect.Securities {


    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// FOREX Implementation of the base Market Class.
    /// </summary>
    public partial class Forex : Security {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        
        /******************************************************** 
        * CONSTRUCTOR/DELEGATE DEFINITIONS
        *********************************************************/
        /// <summary>
        /// Construct the Forex Object
        /// </summary>
        public Forex(string symbol, Resolution resolution, bool fillDataForward, decimal leverage, bool extendedMarketHours) :
            base(symbol, SecurityType.Forex, resolution, fillDataForward, leverage, extendedMarketHours) {
            
            //Holdings for new Vehicle:
            Cache = new ForexCache(this);
            Holdings = new ForexHolding(this);
            Exchange = new ForexExchange();
        }


        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// Forex Cache Class: Caching data, charting and orders.
        /// </summary>
        public new ForexCache Cache {
            get { return (ForexCache)base.Cache; }
            set { base.Cache = value; }
        }

        /// <summary>
        /// Forex Holdings Class: Cash, Quantity Held, Portfolio
        /// </summary>
        public new ForexHolding Holdings {
            get { return (ForexHolding)base.Holdings; }
            set { base.Holdings = value; }
        }

        /// <summary>
        /// Forex Exchange Class: Time open close.
        /// </summary>
        public new ForexExchange Exchange {
            get { return (ForexExchange)base.Exchange; }
            set { base.Exchange = value; }
        }

        /// <summary>
        /// Forex Security Transaction and Fill Models
        /// </summary>
        public new ISecurityTransactionModel Model {
            get { return (ForexTransactionModel)base.Model; }
            set { base.Model = value; }
        }


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/




    } // End Market

} // End QC Namespace
