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
    /// Exchange Class - Information and Helper Tools for Exchange Situation
    /// </summary>
    public class SecurityExchange {

        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        private DateTime _frontier;

        /******************************************************** 
        * CLASS CONSTRUCTION
        *********************************************************/
        /// <summary>
        /// Initialise the exchange for this vehicle.
        /// </summary>
        public SecurityExchange() {
                
        }
        
        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// Timezone for the exchange
        /// </summary>
        public string TimeZone {
            get;
            set;
        }



        /// <summary>
        /// System Time - the time on the most recent data MarketData, this is the commonly used time.
        /// </summary>
        public DateTime Time {
            get {
                return _frontier;
            }
        }


        /// <summary>
        /// Property version of the equity ExchangeOpen method:
        /// </summary>
        public virtual bool ExchangeOpen {
            get { return DateTimeIsOpen(Time); }
        }

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Check whether we are past a certain time: simple method for wrapping datetime.
        /// </summary>
        public bool TimeIsPast(int iHour, int iMin, int iSec = 0) {

            if (Time.Hour > iHour) {
                return true;
            
            } else if (Time.Hour < iHour) {
                return false;
            
            } else if (Time.Hour == iHour) {
                if (Time.Minute > iMin) {
                    return true;

                } else if (Time.Minute < iMin) {
                    return false;

                } else if (Time.Minute == iMin) {
                    //Minute Equal, Check Seconds.
                    if (Time.Second >= iSec) {
                        return true;
                    } else if (Time.Second < iSec) {
                        return false;
                    }
                }
            }
            return false;
        }



        /// <summary>
        /// Set the current datetime:
        /// </summary>
        /// <param name="newTime">Most recent data tick</param>
        public void SetDateTimeFrontier(DateTime newTime) {
            this._frontier = newTime;
        }


        /// <summary>
        /// Check if the date is open.
        /// </summary>
        /// <param name="dateToCheck">Date to check</param>
        /// <returns>Return true if the exchange is open for this date</returns>
        public virtual bool DateIsOpen(DateTime dateToCheck)
        {
            return true;
        }

        /// <summary>
        /// Ensure this date time is open
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public virtual bool DateTimeIsOpen(DateTime dateTime)
        {
            if (!DateIsOpen(dateTime))
                return false;

            return true;
        }

    } //End of MarketExchange


} //End Namespace