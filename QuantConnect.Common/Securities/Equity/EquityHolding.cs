/*
	STOCKTRACK.ORG - Automated Stock Trading, v0.9
		Created Jan 2010 by Jared Broad
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