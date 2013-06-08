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

using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect.Models {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Base Data Class: Type, Timestamp, Key -- Base Features.
    /// </summary>
    public class DataBase {

        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/

        /******************************************************** 
        * CLASS PUBLIC VARIABLES
        *********************************************************/
        /// <summary>
        /// Time keeper of data -- all data is timeseries based.
        /// </summary>
        public DateTime Time;
        
        
        /// <summary>
        /// Symbol for underlying Security
        /// </summary>
        public string Symbol;
        

        /// <summary>
        /// Type of this data: 
        ///     - Market Data - Able to be bought or sold.
        ///     - Sentiment Data - Crowd Sentiment Information.
        /// </summary>
        public DataType DataType;

        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Initialise the Base Data Class
        /// </summary>
        public DataBase() { }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/

    } // End Base Data Class

} // End QC Namespace
