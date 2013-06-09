/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Base Class for Algorithm.
* Estimize object classes - extension of sentiment data objects.
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace QuantConnect.Models {


    /// <summary>
    /// Crowd Earnings Estimize Class:
    /// </summary>
    public class Estimize : SentimentData {


        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/

        /// <summary>
        /// Initialise the Estimize Object:
        /// </summary>
        /// <param name="csvRow">Raw CSV row.</param>
        /// <param name="symbol">Symbol of this estimize object</param>
        public Estimize(string csvRow, string symbol) { 
            string[] csv = csvRow.Split(',');
            this.Time = DateTime.Parse(csv[0]);
            this.UserId = csv[1];
            this.Eps = Convert.ToDecimal(csv[2]);
            this.Revenue = Convert.ToDecimal(csv[3]);
            this.WallStEps = Convert.ToDecimal(csv[4]);
            this.WallStRevenue = Convert.ToDecimal(csv[5]);
            this.FiscalYear = Convert.ToInt32(csv[6]);
            this.FiscalQuarter = Convert.ToInt32(csv[7]);
            this.Industry = csv[8];
            this.Sector = csv[9];
            this.ReportsAt = DateTime.Parse(csv[10]);
            this.Symbol = symbol;
            this.Type = SentimentDataType.Estimize;
            this.DataType = DataType.SentimentData;
        }

        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        /// <summary>
        /// User ID who made this estimate
        /// </summary>
        public string UserId;
        /// <summary>
        /// Earning Per Symbol Prediction from the USer:
        /// </summary>
        public decimal Eps;
        /// <summary>
        /// Revenue prediction from the user
        /// </summary>
        public decimal Revenue;
        /// <summary>
        /// Wall Street EPS prediction
        /// </summary>
        public decimal WallStEps;
        /// <summary>
        /// Wall Street Revenue Prediction
        /// </summary>
        public decimal WallStRevenue;
        /// <summary>
        /// Fiscal year this applies to
        /// </summary>
        public int FiscalYear;
        /// <summary>
        /// Fiscal Quarter this applies to
        /// </summary>
        public int FiscalQuarter;
        /// <summary>
        /// Industry we're operating in
        /// </summary>
        public string Industry;
        /// <summary>
        /// Sector this prediction applies to
        /// </summary>
        public string Sector;
        /// <summary>
        /// Reports at timestamp.
        /// </summary>
        public DateTime ReportsAt;

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/



        /******************************************************** 
        * CLASS METHODS
        *********************************************************/



    }



} // End QC Namespace
