/*
* QUANTCONNECT.COM: Equity Exchange.cs
* Equity Exchange Class - Helper routine to determine if this asset type is currently open and tradable.
*/

/**********************************************************
 * USING NAMESPACES
 **********************************************************/
using System;

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
    public class EquityExchange : SecurityExchange {

        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/


        /******************************************************** 
        * CLASS CONSTRUCTION
        *********************************************************/
        /// <summary>
        /// Initialise Equity Exchange Objects
        /// </summary>
        public EquityExchange() : 
            base() {
        }



        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// US Equities Exchange Open Critieria
        /// </summary>
        public override bool ExchangeOpen {
            get
            {
                return DateTimeIsOpen(Time);
            }
        }


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Once live and looping, check if this datetime is open, before updating the security.
        /// </summary>
        /// <param name="dateToCheck">Time to check</param>
        /// <returns>True if open</returns>
        public override bool DateTimeIsOpen(DateTime dateToCheck)
        {
            if (!DateIsOpen(dateToCheck.Date))
                return false;

            //Market not open yet:
            if (dateToCheck.TimeOfDay.TotalHours < 9.5 || dateToCheck.TimeOfDay.TotalHours >= 16)
                return false;

            return true;
        }


        /// <summary>
        /// Conditions to check if the equity markets are open
        /// </summary>
        /// <param name="dateToCheck">datetime to check</param>
        /// <returns>true if open</returns>
        public override bool DateIsOpen(DateTime dateToCheck)
        {
            if (dateToCheck.DayOfWeek == DayOfWeek.Saturday || dateToCheck.DayOfWeek == DayOfWeek.Sunday)
                return false;

            //Check the date first.
            foreach (DateTime holiday in USHoliday.Dates) {
                if (dateToCheck.Date == holiday.Date) {
                    return false;
                }
            }

            return true;
        }



    } //End of EquityExchange

} //End Namespace