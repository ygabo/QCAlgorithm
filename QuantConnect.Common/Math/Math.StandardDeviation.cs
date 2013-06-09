/*
* QUANTCONNECT.COM - 
* QC.Math Library of Statistics Routines for Algorithm
*/

/**********************************************************
 * USING NAMESPACES
 **********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//QuantConnect Project Libraries:
using QuantConnect.Logging;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    public partial class QCMath {

        class QCStandardDeviation {
            /******************************************************** 
            * CLASS VARIABLES
            *********************************************************/

            /******************************************************** 
            * CLASS METHODS
            *********************************************************/
            /// <summary>
            /// Find the standard deviation of a STArray data set
            /// Formula found from here : http://www.johndcook.com/variance.gif
            /// </summary>
            /// <param name="dX">STArray VEctor</param>
            /// <returns>decimal dVariance of the Vector</returns>
            public static decimal GetVariance(IList<decimal> dX) {

                /*
                    *                 1     |                              |
                    *      s^2 = ---------- | n*sum( Xi^2 )  - sum(Xi) ^ 2 |
                    *             n(n - 1)  |                              |
                    * 
                    */

                decimal dN = dX.Count;
                decimal dSumXX = QCMath.VectorSquaredSum(dX);
                decimal dSumX2 = (decimal)System.Math.Pow((double)dX.Sum(), 2);

                decimal dVariance = (1 / (dN * (dN - 1))) * ((dN * dSumXX) - dSumX2);

                if (dVariance < 0) {
                    //Log.Error("QCMath: GetVariance(): Error calculating variance");
                    //Normally occurs when vector is identical, eg, 30 rows of $50.1, Variance=0:

                    dVariance = 0;
                }
                return dVariance;
            }




            /// <summary>
            /// Get the standard deviation of the data set,
            /// </summary>
            public static decimal GetDeviation(IList<decimal> dX) {
                return (decimal)System.Math.Sqrt((double)GetVariance(dX));
            }

        } // End of STD DeV Class
    } // End of Math Partial
} // End of Namespace
