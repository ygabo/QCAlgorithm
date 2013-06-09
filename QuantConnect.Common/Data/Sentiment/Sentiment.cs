/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Base Class for Algorithm.
* Sentiment base class object - for intangible data types.
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

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
