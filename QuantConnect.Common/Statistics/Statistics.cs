/*
*   QUANTCONNECT.COM - 
*   QC.Statistics - Generate statistics on the equity and orders
*/

/**********************************************************
 * USING NAMESPACES
 **********************************************************/
using System;
using System.Linq;
using System.Collections.Generic;


//QuantConnect Project Libraries:
using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/

    public class Statistics {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/

        /******************************************************** 
        * CONSTRUCTOR/DELEGATE DEFINITIONS
        *********************************************************/

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Run a full set of orders and return a 
        /// </summary>
        /// <param name="equity">Equity value over time.</param>
        /// <param name="profitLoss">profit loss from trades</param>
        /// <param name="startingCash">Amount of starting cash in USD </param>
        /// <param name="fractionOfYears">Number of years as a double number 1 = 1 year. </param>
        /// <returns>Statistics Array, Broken into Annual Periods</returns>
        public static Dictionary<string, Dictionary<Statistic, decimal>> Generate(SortedDictionary<DateTime, decimal> equity, SortedDictionary<DateTime, decimal> profitLoss, decimal startingCash, double fractionOfYears = 1) { 
            
            //Initialise the response:
            decimal profitLossValue = 0, runningCash = startingCash;
		    List<int> years = new List<int>();
            SortedDictionary<int, int> annualTrades = new SortedDictionary<int,int>();
		    SortedDictionary<int, int> annualWins = new SortedDictionary<int,int>();
            SortedDictionary<int, int> annualLosses = new SortedDictionary<int,int>();
            SortedDictionary<int, decimal> annualLossTotal = new SortedDictionary<int, decimal>();
            SortedDictionary<int, decimal> annualWinTotal = new SortedDictionary<int, decimal>();
            SortedDictionary<int, decimal> annualNetProfit = new SortedDictionary<int, decimal>();
            Dictionary<string, Dictionary<Statistic, decimal>> statistics = new Dictionary<string, Dictionary<Statistic, decimal>>();

            //Set defaults in case of failure:
            decimal totalTrades = 0;
            decimal totalWins = 0;
            decimal totalLosses = 0;
            decimal averageWin = 0;
            decimal averageLoss = 0;
            decimal averageWinRatio = 0;
            decimal winRate = 0;
            decimal lossRate = 0;
            decimal totalNetProfit = 0;
            decimal averageAnnualReturn = 0;
            int frequency = 0;

            try {
                //Run over each equity day:
                foreach (DateTime closedTrade in profitLoss.Keys) {

                    profitLossValue = profitLoss[closedTrade];

                    //Check if this date is in the "years" array:
                    int year = closedTrade.Year;
                    if (!years.Contains(year)) {
                        //Initialise a new year holder:
                        years.Add(year);
                        annualTrades.Add(year, 0);
                        annualWins.Add(year, 0);
                        annualWinTotal.Add(year, 0);
                        annualLosses.Add(year, 0);
                        annualLossTotal.Add(year, 0);
                        //lStatistics.Add(iYear.ToString(), new Dictionary<Statistic, decimal>());
                    }

                    //Add another trade:
                    annualTrades[year]++;

                    //Profit loss tracking:
                    if (profitLossValue > 0) {
                        annualWins[year]++;
                        annualWinTotal[year] += profitLossValue / runningCash;
                    } else {
                        annualLosses[year]++;
                        annualLossTotal[year] += profitLossValue / runningCash;
                    }

                    //Increment the cash:
                    runningCash += profitLossValue;
                }

                //Get the annual percentage of profit and loss:
                foreach (int year in years) {
                    annualNetProfit[year] = (annualWinTotal[year] + annualLossTotal[year]);
                }


                //Sum the totals:
                try {
                    if (profitLoss.Keys.Count > 0) {
                        totalTrades = annualTrades.Values.Sum();
                        totalWins = annualWins.Values.Sum();
                        totalLosses = annualLosses.Values.Sum();
                        totalNetProfit = (equity.Values.LastOrDefault() / startingCash) - 1;

                        try {

                            if (fractionOfYears > 0) {
                                averageAnnualReturn = totalNetProfit / Convert.ToDecimal(fractionOfYears);
                            } else {
                                averageAnnualReturn = totalNetProfit;
                            }

                        } catch (Exception err) {
                            Log.Error("Statistics() Annual Average Return: " + err.Message);
                            averageAnnualReturn = annualNetProfit.Values.Average();
                        }

                        //-> Handle Div/0 Errors
                        if (totalWins == 0) {
                            averageWin = 0;
                        } else {
                            averageWin = annualWinTotal.Values.Sum() / totalWins;
                        }
                        if (totalLosses == 0) {
                            averageLoss = 0;
                            averageWinRatio = 0;
                        } else {
                            averageLoss = annualLossTotal.Values.Sum() / totalLosses;
                            averageWinRatio = Math.Abs(averageWin / averageLoss);
                        }
                        if (totalTrades == 0) {
                            winRate = 0;
                            lossRate = 0;
                        } else {
                            winRate = Math.Round(totalWins / totalTrades, 5);
                            lossRate = Math.Round(totalLosses / totalTrades, 5);
                        }

                        //Get the frequency:
                        frequency = (int)Statistics.Frequency(totalTrades, equity.Keys.FirstOrDefault(), equity.Keys.LastOrDefault());
                    }

                } catch (Exception err) {
                    Log.Error("Statistics.RunOrders(): Second Half: " + err.Message);
                }

                //Add the over all results first, break down by year later:
                statistics.Add("Overall", new Dictionary<Statistic, decimal>() { 
                    { Statistic.TotalTrades, Math.Round(totalTrades, 0) },

                    { Statistic.AverageWin, Math.Round(averageWin * 100, 2)  },

                    { Statistic.AverageLoss, Math.Round(averageLoss * 100, 2) },

                    { Statistic.AverageAnnualReturn, Math.Round(averageAnnualReturn * 100, 3) },

                    { Statistic.Drawdown, (Statistics.Drawdown(equity, 3) * 100) },

                    { Statistic.Expectancy, Math.Round((winRate * averageWinRatio) - (lossRate), 3) },

                    { Statistic.NetProfit, Math.Round(totalNetProfit * 100, 3) },

                    { Statistic.SharpeRatio, Statistics.SharpeRatio(equity, startingCash, averageAnnualReturn) },

                    { Statistic.LossRate, Math.Round(lossRate * 100)  },

                    { Statistic.WinRate, Math.Round(winRate * 100) }, 

                    { Statistic.ProfitLossRatio, Statistics.ProfitLossRatio(averageWin, averageLoss) },

                    { Statistic.TradeFrequency, frequency }
                });
                
            } catch (Exception err) {
                Log.Error("QC.Statistics.RunOrders(): " + err.Message + err.InnerException + err.TargetSite);
            }
            return statistics;
        }




        /// <summary>
        /// Return profit loss ratio safely.
        /// </summary>
        /// <param name="averageWin"></param>
        /// <param name="averageLoss"></param>
        /// <returns></returns>
        public static decimal ProfitLossRatio(decimal averageWin, decimal averageLoss) {
            if (averageLoss == 0) {
                return 9;
            } else {
                return Math.Round(averageWin / Math.Abs(averageLoss), 2);
            }
        }




        /// <summary>
        /// Get an approximation of the trade frequency:
        /// </summary>
        /// <param name="dTotalTrades">Number of trades in this period</param>
        /// <param name="dtStart">Start of Period</param>
        /// <param name="dtEnd">End of Period</param>
        /// <returns>Enum Frequency</returns>
        public static TradeFrequency Frequency(decimal totalTrades, DateTime start, DateTime end) { 
                
            //Average number of trades per day:
            decimal period = Convert.ToDecimal((end - start).TotalDays);

            if (period == 0) {
                return TradeFrequency.Weekly;
            }

            decimal averageDaily = totalTrades / period;

            if (averageDaily > 200m) {
                return TradeFrequency.Secondly;
            } else if (averageDaily > 50m) {
                return TradeFrequency.Minutely;
            } else if (averageDaily > 5m) {
                return TradeFrequency.Hourly;
            } else if (averageDaily > 0.75m) {
                return TradeFrequency.Daily;
            } else {
                return TradeFrequency.Weekly;
            }
        }


        /// <summary>
        /// Get the Drawdown Statistic for this Period.
        /// </summary>
        /// <param name="equityOverTime">Array of portfolio value over time.</param>
        /// <param name="rounding">Round the drawdown statistics </param>
        /// <returns>Draw down percentage over period.</returns>
        public static decimal Drawdown(SortedDictionary<DateTime, decimal> equityOverTime, int rounding = 2) {
            //Initialise:
            int priceMaximum = 0;
            int previousMinimum = 0;
            int previousMaximum = 0;

            try
            {
                List<decimal> lPrices = equityOverTime.Values.ToList<decimal>();
                for (int id = 0; id < lPrices.Count; id++) {
                    if (lPrices[id] >= lPrices[priceMaximum]) {
                        priceMaximum = id;
                    } else {
                        if ((lPrices[priceMaximum] - lPrices[id]) > (lPrices[previousMaximum] - lPrices[previousMinimum])) {
                            previousMaximum = priceMaximum;
                            previousMinimum = id;
                        }
                    }
                }
                return Math.Round((lPrices[previousMaximum] - lPrices[previousMinimum]) / lPrices[previousMaximum], rounding);
            } catch(Exception err)
            {
                Log.Error("Statistics.Drawdown(): " + err.Message);
            }
            return 0;
        } // End Drawdown:



        /// <summary>
        /// Get the Sharpe Ratio of this period:
        /// </summary>
        /// <param name="equity">Equity of this period.</param>
        /// <param name="averageAnnualGrowth">Percentage annual growth</param>
        /// <param name="rounding">decimal rounding</param>
        /// <param name="startingCash">Starting cash for the conversion to percentages</param>
        /// <returns>decimal sharpe.</returns>
        public static decimal SharpeRatio(SortedDictionary<DateTime, decimal> equity, decimal startingCash, decimal averageAnnualGrowth, int rounding = 1) { 
            //Initialise                
            double sharpe = 0;

            try {
                decimal[] equityCash = equity.Values.ToArray();
                List<decimal> equityPercent = new List<decimal>();

                foreach (decimal equityValue in equityCash) {
                    equityPercent.Add(equityValue / startingCash);
                }

                //Get the Standard Deviation
                double standardDeviation = Convert.ToDouble(QCMath.StandardDeviation(equityPercent.ToArray()));

                if (standardDeviation > 0) {
                    sharpe = (Convert.ToDouble(averageAnnualGrowth) - 0.01) / standardDeviation;
                }
            } catch (Exception err) {
                Log.Error("Statistics.SharpeRatio(): " + err.Message);
            }

            //Round to two significant figures:
            decimal decimalSharpe = Convert.ToDecimal(sharpe);

            if (decimalSharpe > 10) {
                decimalSharpe = Math.Round(decimalSharpe, 0);
            } else if (decimalSharpe > 0 & decimalSharpe < 10) {
                decimalSharpe = Math.Round(decimalSharpe, 1);
            } else if (decimalSharpe < 0) {
                decimalSharpe = Math.Round(decimalSharpe, 0);
            }

            return decimalSharpe;
        }


    } // End of Statistics

} // End of Namespace
