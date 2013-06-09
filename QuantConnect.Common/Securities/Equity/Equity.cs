/*
* QUANTCONNECT.COM:
* Equity Class - Base class for Equity Security Types, Extension of Security.
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

//QuantConnect Libraries:
using QuantConnect;
using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect.Securities {


    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// A base "Market" Vehicle Class for Providing a common interface to Indexes / Security / FOREX Trading.
    /// </summary>
    public partial class Equity : Security {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        
        /******************************************************** 
        * CONSTRUCTOR/DELEGATE DEFINITIONS
        *********************************************************/
        /// <summary>
        /// Construct the Market Vehicle
        /// </summary>
        public Equity(string symbol, Resolution resolution, bool fillDataForward, decimal leverage, bool extendedMarketHours) :
            base(symbol, SecurityType.Equity, resolution, fillDataForward, leverage, extendedMarketHours) {
            
            //Holdings for new Vehicle:
            Cache = new EquityCache(this);
            Holdings = new EquityHolding(this);
            Exchange = new EquityExchange();
        }


        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// Equity Cache Class: Caching data, charting and orders.
        /// </summary>
        public new EquityCache Cache {
            get { return (EquityCache)base.Cache; }
            set { base.Cache = value; }
        }

        /// <summary>
        /// Equity Holdings Class: Cash, Quantity Held, Portfolio
        /// </summary>
        public new EquityHolding Holdings {
            get { return (EquityHolding)base.Holdings; }
            set { base.Holdings = value; }
        }

        /// <summary>
        /// Equity Exchange Class: Time open close.
        /// </summary>
        public new EquityExchange Exchange {
            get { return (EquityExchange)base.Exchange; }
            set { base.Exchange = value; }
        }

        /// <summary>
        /// Equity Security Transaction and Fill Models
        /// </summary>
        public new ISecurityTransactionModel Model {
            get { return (EquityTransactionModel)base.Model; }
            set { base.Model = value; }
        }


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/




    } // End Market

} // End QC Namespace
