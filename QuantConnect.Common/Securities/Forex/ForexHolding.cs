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
    /// FOREX Holdings Override: Any Properties specifically for FOREX Holdings Cases:
    /// </summary>
    public class ForexHolding : SecurityHolding {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        public new Forex Vehicle { get; set; }

        /******************************************************** 
        * CONSTRUCTOR/DELEGATE DEFINITIONS
        *********************************************************/
        /// <summary>
        /// Forex Holding Class
        /// </summary>
        public ForexHolding(Forex pair) :
            base(pair) {
                this.Vehicle = pair;
        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
            

        /******************************************************** 
        * CLASS METHODS 
        *********************************************************/
            


    } // End Equity Holdings:
} //End Namespace