/*
	STOCKTRACK.ORG - Automated Stock Trading, V0.8
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
    ********************************************************/

    /// <summary>
    /// FX Specific Caching ontop of the SecurityCache obj. Common Caching Spot For Market Data and Averaging. 
    /// </summary>
    public class ForexCache : SecurityCache {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        public new Forex Vehicle { get; set; }

        /******************************************************** 
        * CONSTRUCTOR/DELEGATE DEFINITIONS
        *********************************************************/
        /// <summary>
        /// Start a new Cache for the set Index Code
        /// </summary>
        public ForexCache(Forex vehicle) :
            base(vehicle) {
                this.Vehicle = vehicle;
        }


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/



    } //End EquityCache Class
} //End Namespace