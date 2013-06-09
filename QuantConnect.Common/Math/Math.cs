/*
* QUANTCONNECT.COM - Extension routines for QC.
* QC.Math Library of Statistics Routines for Algorithm
*/

/**********************************************************
 * USING NAMESPACES
 **********************************************************/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

//QuantConnect Project Libraries:
using QuantConnect.Logging;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/

    public partial class QCMath {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        public static readonly decimal Pi = 3.14159265358979323846264338327950288419716939937510m;
        public static readonly decimal Phi = 1.6180339887498948482m;
        public static readonly decimal InvPhi = 0.6180339887498948482m;
        public static readonly decimal Phi618 = 0.6180339887498948482m;
        public static readonly decimal Phi381 = 0.5819660112501051518m;

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Wrapper for System.Math Power Function
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Pow(double x, double y) {
            return System.Math.Pow(x, y);
        }


        /// <summary>
        /// Wrapper for System.Math.Round.
        /// </summary>
        /// <param name="dNumber">Number to Round.</param>
        /// <param name="iDecimalPlaces">Number of Decimal Places</param>
        /// <returns></returns>
        public static decimal Round(decimal dNumber, int iDecimalPlaces = 2) {
            return System.Math.Round(dNumber, iDecimalPlaces);
        }


        /// <summary>
        /// Returns a random number within range.
        /// </summary>
        /// <returns></returns>
        public static decimal Random(bool bRound = false, int iRoundDP = 0) {

            decimal dRandom = 0;
            System.Random rGenerator = new System.Random();
            dRandom = (decimal)rGenerator.NextDouble();

            if (bRound) {
                dRandom = Math.Round(dRandom, iRoundDP);
            }
            return dRandom;
        }




        /// <summary>
        /// Find the mean value of a stock 
        /// </summary>
        /// <param name="dX">dX Array</param>
        /// <returns>decimal Average Value</returns>
        public static decimal Average(decimal[] dX) {
            return dX.Average();
        }




        /// <summary>
        /// Calculate the Spread of Prices from first to last price
        /// </summary>
        /// <param name="dX">decimal Price Array</param>
        /// <returns>decimal dDollar shift</returns>
        public static decimal Spread(decimal[] dX) {
            if (dX.Length > 0) {
                //Find the spread of the most recent price to the first one.
                return (dX[dX.Length - 1] - dX[0]);
            } else {
                return 0;
            }
        }



        /// <summary>
        /// A multiple max filter:
        /// </summary>
        public static decimal Max(params decimal[] dValues) {
            decimal dMax = decimal.MinValue;
            for (int i = 0; i < dValues.Length; i++) {
                if (dValues[i] > dMax) dMax = dValues[i];
            }
            return dMax;
        }




        /// <summary>
        /// A multiple min filter:
        /// </summary>
        public static decimal Min(params decimal[] dValues) {
            decimal dMin = decimal.MaxValue;
            for (int i = 0; i < dValues.Length; i++) {
                if (dValues[i] < dMin) dMin = dValues[i];
            }
            return dMin;
        }



        /// <summary>
        /// Find the Absolulte Spread of prices inthe time period.
        /// </summary>
        /// <param name="dX">decimal Price Array</param>
        /// <returns>Dollars shift</returns>
        public static decimal AbsSpread(decimal[] dX) {
            if (dX.Length > 0) {
                //Find the % AbsSpread of the Highest Price to Lowest Price in this period.
                return (dX.Max() - dX.Min());
            } else {
                return 0;
            }
        }



        /// <summary>
        /// Return the absolute value
        /// </summary>
        /// <param name="dValue">+- value.</param>
        /// <returns>Absolute</returns>
        public static decimal Abs(decimal dValue) {
            return System.Math.Abs(dValue);
        }


        /// <summary>
        /// Return the Median value from a set of data
        /// </summary>
        /// <param name="sortedprice"> SORTED Price Array </param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        public static decimal GetMedian(decimal[] sortedprice, int startIndex = 0, int endIndex = 0) {
            int i = 0;
            return GetMedian(sortedprice, out i, false, startIndex, endIndex);
        }
        /// <summary>
        /// Get the Median of an array
        /// </summary>
        /// <param name="sortedprice"></param>
        /// <param name="medianIndex"></param>
        /// <param name="bSort"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static decimal GetMedian(decimal[] sortedprice, out int medianIndex, bool bSort = false, int startIndex = 0, int endIndex = 0) {

            //Data Model:
            // a,b,c,d,e
            // 0,1,2,3,4 = 5
            decimal median = 0;
            decimal priceCount;

            //Artificially set end point of the data 
            if (endIndex > 0) {
                priceCount = endIndex - startIndex;
            } else {
                priceCount = sortedprice.Count();
            }

            //Find out if even or odd number of data points:
            bool even = ((priceCount % 2) == 0);

            //Find the mid point
            decimal midNumber = startIndex + (priceCount / 2);

            //Sort the data so can accuractely pick median
            if (bSort) {
                sortedprice.OrderBy(x => x);
            }

            //For odd count arrays, index is direct, for even, is average.
            medianIndex = (int)System.Math.Floor(midNumber);

            try {
                if (even) {
                    if (priceCount > 2) {
                        //If an even number of data points, need to find the average of B & C.
                        median = (sortedprice[medianIndex] + sortedprice[medianIndex + 1]) / 2;
                    } else {
                        //If its only one or two entries in the dataarray, just return approximation
                        median = sortedprice[medianIndex];
                    }
                } else {
                    //If this is an odd number
                    median = sortedprice[medianIndex];
                }
            } catch (Exception err) {
                //Break if there is an error finding the median.
                Log.Error("QCMath: GetMedian(): " + err.Message);
            }

            return median;
        }




        /// <summary>
        /// Get the Profitability on the predicted price vs current avgHoldings price
        /// </summary>
        /// <param name="dPredicteprice">Predicted price.</param>
        /// <param name="dAvgHoldingsPrice">Current holdings price.</param>
        /// <returns>Profitability percentage.</returns>
        public static decimal GetProfitability(decimal dPredicteprice, decimal dAvgHoldingsPrice) {
            return Math.Round(dPredicteprice / dAvgHoldingsPrice, 3);
        }



        /// <summary>
        /// Multiply two decimal arrays return the vector results
        /// </summary>
        /// <param name="dX">Vector A to multiply</param>
        /// <param name="dY">Vector B to multiply</param>
        /// <returns>Return the vector multiplication</returns>
        public static decimal[] VectorMultiply(decimal[] dX, decimal[] dY) {
            decimal[] dNew = new decimal[dX.Length];
            for (int i = 0; i < dX.Length; i++) {
                dNew[i] = dX[i] * dY[i];
            }
            return dNew;
        }


        /// <summary>
        /// Multiply two decimal arrays return the vector results
        /// </summary>
        /// <param name="dX">Vector A to Square</param>
        /// <returns>Return the vector multiplication</returns>
        public static decimal VectorSquaredSum(IList<decimal> dX) {
            decimal dSum = 0;
            for (int i = 0; i < dX.Count; i++) {
                dSum += dX[i] * dX[i];
            }
            return dSum;
        }


        /// <summary>
        /// Check if a dSubject is within dError% of a dTarget.
        /// </summary>
        public static bool WithinError(decimal dTest, decimal dTargetCenter, decimal dError, bool bUseAbsolute = false) {

            decimal dLowBracket = 1 - dError;
            decimal dHighBracket = 1 + dError;

            if (!bUseAbsolute) {
                if (dTest > 0) {
                    if ((dTargetCenter * dHighBracket) > dTest && (dTargetCenter * dLowBracket) < dTest) {
                        return true;
                    }
                } else {
                    Log.Error("QCMath.WithinError(): Only useful above zero.");
                }
            } else {
                //Use the error term as an absolute error.
                if ((dTargetCenter + dError) > dTest && (dTargetCenter - dError) < dTest) {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// Bracket Zero E.g.: X gt -50, X lt 50. Within error of Zero.
        /// Test if dSubject brackets Zero within dBracket
        /// </summary>
        public static bool BracketZero(decimal dSubject, decimal dBracket) {
            //E.g.: X > -50, X < 50. Within error of Zero.
            if ((dSubject > -dBracket) && (dSubject < dBracket)) {
                //Within error bracket.
                return true;
            }
            return false;
        }
            


        /// <summary>
        /// Check if the subject is greater than the limit, if greater- cap subject at limit. If less return subject.
        /// </summary>
        public static decimal LimitMax(decimal dSubject, decimal dLimit, decimal? dSetTo = null) {
            if (dSubject > dLimit) {
                if (dSetTo == null) {
                    return dLimit;
                } else {
                    return dSetTo.Value;
                }
            } else {
                return dSubject;
            }
        }



        /// <summary>
        /// Check if the subject is less than the limit, if lesser - fix subject at limit. If less return subject.
        /// </summary>
        public static decimal LimitMin(decimal dSubject, decimal dLimit, decimal? dSetTo = null) {
            if (dSubject < dLimit) {
                if (dSetTo == null) {
                    return dLimit;
                } else {
                    return dSetTo.Value;
                }
            } else {
                return dSubject;
            }
        }



        /// <summary>
        /// Convert Radians to Degrees for Ease of use:
        /// </summary>
        /// <param name="dAngle">Angle in Radians (0-2pi)</param>
        /// <returns>Degrees (0-360)</returns>
        public static decimal DegreeToRadian(decimal dAngle) {
            return (QCMath.Pi * dAngle / (decimal)180.0);
        }




        /// <summary>
        /// Return a tidied string value of dollars.
        /// </summary>
        /// <param name="price"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static string TidyToDollars(decimal price, int decimals = 2) {
            return Math.Round(price, decimals).ToString("C" + decimals.ToString());
        }



        /// <summary>
        /// Get the Variance of our dataset.
        /// </summary>
        public static decimal Variance(decimal[] dX) {
            return QCMath.QCStandardDeviation.GetVariance(dX);
        }

        /// <summary>
        /// Get the standard deviation of an array dataset.
        /// </summary>
        public static decimal StandardDeviation(decimal[] dX) {
            if (dX.Length == 0) return 0;
            return QCMath.QCStandardDeviation.GetDeviation(dX);
        }


        /// <summary>
        /// Tidy a text box number into a string.
        /// </summary>
        /// <param name="sNumberString"></param>
        /// <returns></returns>
        public static int TidyStringInt(string sNumberString) {
            return (int)TidyStringNumber(sNumberString);
        }


        /// <summary>
        /// Tidy a string into a decimal, return 0 if not parsable.
        /// </summary>
        /// <param name="sNumberString">Number in a string</param>
        /// <returns>decimal value of string.</returns>
        public static decimal TidyStringNumber(string sNumberString) {
            decimal dDollars = 0;
            string sDollars = "";

            //Attempt to filter out the number
            try {
                CultureInfo MyCultureInfo = new CultureInfo("en-US");
                char[] cNumberString = sNumberString.ToCharArray();

                //Robust method for making a number from a string, remove the characters:
                foreach (char c in cNumberString) {
                    //Comma or Dot we'll slide as non numeric characters to add.
                    if ((c == '.') || (c == ',') || (c == '-')) {
                        sDollars += c.ToString();
                        continue;
                    } else if (c == '(') {
                        //Include the subtrOrderDirection as well.
                        sDollars += '-';
                    } else if (IsNumber(c)) {
                        //Otherwise if its a number add it to string. Skip "E" "Euro" etc.
                        sDollars += c.ToString();
                    }
                }

                try {
                    dDollars = (decimal)Decimal.Parse(sDollars, NumberStyles.Currency, MyCultureInfo);
                } catch (Exception err) {
                    //No point logging, can happen often.
                    Log.Trace("TidyStringNumber() Error Parsing Number(" + sNumberString + "):  " + err.Message);
                    dDollars = 0;
                }
            } catch {
                //De nada
            }
            return dDollars;
        }


        /// <summary>
        /// Test if a string is a number and can be parsed
        /// </summary>
        /// <param name="Expression">String to be tested.</param>
        /// <returns>Bool/Number</returns>
        public static bool IsNumber(object Expression) {
            bool bIsNum;
            decimal dRetNum;
            bIsNum = decimal.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any,
                        System.Globalization.NumberFormatInfo.InvariantInfo, out dRetNum);
            return bIsNum;
        }


        /// <summary>
        /// Test if the variable "x" is between A and B
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool XisBetweenAandB(decimal x, decimal a, decimal b) {
            if ((x >= a) && (x <= b)) {
                //X is within or ON the boundariesof A and B
                return true;
            } else {

                return false;
            }
        }

    } // End of Math

} // End of Namespace
