/*
* QUANTCONNECT.COM: Equity Holding.cs
* Equity Holding Class - Track orders, portfolio, cash, leverage etc.
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
    /// Equity Holdings Override: Specifically for Equity Holdings Cases:
    /// </summary>
    public class EquityHolding : SecurityHolding {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        public new Equity Vehicle { get; set; }

        /******************************************************** 
        * CONSTRUCTOR/DELEGATE DEFINITIONS
        *********************************************************/
        /// <summary>
        /// Equity Holding Class
        /// </summary>
        public EquityHolding(Equity vehicle) :
            base(vehicle) {
                this.Vehicle = vehicle;
        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
            

        /******************************************************** 
        * CLASS METHODS 
        *********************************************************/
            


    } // End Equity Holdings:
} //End Namespace