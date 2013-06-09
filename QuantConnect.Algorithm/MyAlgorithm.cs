/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Example starting point for a locally compiled user algorithm
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


using QuantConnect.Securities;
using QuantConnect.Models;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Example user algorithm class - use this as a base point for your strategy.
    /// </summary>
    public class MyAlgorithm : QCAlgorithm {

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        
        /// <summary>
        /// Initialise your algorithm here.
        /// </summary>
        public override void Initialize() {
            //Configure Start and End Date:
            SetStartDate(2012, 01, 01);
            SetEndDate(2012, 12, 31);
            //Request IBM Data
            AddSecurity(SecurityType.Equity, "IBM", Resolution.Minute, true, false);
            //Set your starting capital:
            SetCash(50000);
        }

        /// <summary>
        /// Handle your tradeBar's here.
        /// </summary>
        /// <param name="symbols">Dictionary of data objects</param>
        public override void OnTradeBar(Dictionary<string, TradeBar> data) {
            //Use the data to generate orders.
            if (!Portfolio.HoldStock)
            {
                Order("IBM", 10, OrderType.Market);
            }
        }

    } // End Algorithm Template

} // End QC Namespace
