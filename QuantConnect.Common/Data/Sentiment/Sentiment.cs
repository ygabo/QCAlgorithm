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

    /// <summary>
    /// Base Class for Meta Data Types:
    /// </summary>
    public class SentimentData : DataBase {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        /// <summary>
        /// Sentiment data type:
        /// </summary>
        public SentimentDataType Type = SentimentDataType.Base;

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
    }

} // End QC Namespace
