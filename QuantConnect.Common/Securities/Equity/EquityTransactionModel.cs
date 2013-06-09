/*
* QUANTCONNECT.COM - Equity Transaction Model
* Default Equities Transaction Model
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

namespace QuantConnect.Securities {

    /******************************************************** 
    * QUANTCONNECT PROJECT LIBRARIES
    *********************************************************/
    using QuantConnect.Models;


    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Default Transaction Model for Equity Security Orders
    /// </summary>
    public class EquityTransactionModel : ISecurityTransactionModel {

        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/


        /******************************************************** 
        * CLASS PUBLIC VARIABLES
        *********************************************************/



        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Initialise the Algorithm Transaction Class
        /// </summary>
        public EquityTransactionModel() {

        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/





        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Perform neccessary check to see if the model has been filled, appoximate the best we can.
        /// </summary>
        /// <param name="vehicle">Asset we're working with</param>
        /// <param name="order">Order class to check if filled.</param>
        public virtual void Fill(Security vehicle, ref Order order) {
            try {
                switch (order.Type) {
                    case OrderType.Limit:
                        LimitFill(vehicle, ref order);
                        break;
                    case OrderType.Stop:
                        StopFill(vehicle, ref order);
                        break;
                    case OrderType.Market:
                        MarketFill(vehicle, ref order);
                        break;
                }
            } catch (Exception err) {
                Log.Error("Equity.TransOrderDirection.Fill(): " + err.Message);
            }
        }



        /// <summary>
        /// Get the Slippage approximation for this order:
        /// </summary>
        public virtual decimal GetSlippageApproximation(Security security, Order order) {
            return 0;
        }



        /// <summary>
        /// Model the slippage on a market order: fixed percentage of order price
        /// </summary>
        /// <param name="security">Asset we're working with</param>
        /// <param name="order">Order to update</param>
        public virtual void MarketFill(Security security, ref Order order) {

            try {
                //Calculate the model slippage: e.g. 0.01c
                decimal slip = GetSlippageApproximation(security, order);

                switch (order.Direction)
                {
                    case OrderDirection.Buy:
                        order.Price = security.Price;
                        order.Price += slip;
                        break;
                    case OrderDirection.Sell:
                        order.Price = security.Price;
                        order.Price -= slip;
                        break;
                }

                //Market orders fill instantly.
                order.Status = OrderStatus.Filled;

                //Round off:
                order.Price = Math.Round(order.Price, 2);

            } catch (Exception err) {
                Log.Error("Equity.TransOrderDirection.MarketFill(): " + err.Message);
            }
        }




        /// <summary>
        /// Check if the model has stopped out our position yet:
        /// </summary>
        /// <param name="security">Asset we're working with</param>
        /// <param name="order">Stop Order to Check, return filled if true</param>
        public virtual void StopFill(Security security, ref Order order) {
            try {
                //If its cancelled don't need anymore checks:
                if (order.Status == OrderStatus.Canceled) return;

                //Calculate the model slippage: e.g. 0.01c
                decimal slip = GetSlippageApproximation(security, order);

                //Check if the Stop Order was filled: opposite to a limit order
                switch (order.Direction)
                {
                    case OrderDirection.Sell:
                        //-> 1.1 Sell Stop: If Price below setpoint, Sell:
                        if (security.Price < order.Price) {
                            order.Status = OrderStatus.Filled;
                            order.Price = security.Price;
                            order.Price -= slip;
                        }
                        break;
                    case OrderDirection.Buy:
                        //-> 1.2 Buy Stop: If Price Above Setpoint, Buy:
                        if (security.Price > order.Price) {
                            order.Status = OrderStatus.Filled;
                            order.Price = security.Price;
                            order.Price += slip;
                        }
                        break;
                }

                //Round off:
                order.Price = Math.Round(order.Price, 2);

            } catch (Exception err) {
                Log.Error("Equity.TransOrderDirection.StopFill(): " + err.Message);
            }
        }



        /// <summary>
        /// Check if the price MarketDataed to our limit price yet:
        /// </summary>
        /// <param name="security">Asset we're working with</param>
        /// <param name="order">Limit order in market</param>
        public virtual void LimitFill(Security security, ref Order order) {

            //Initialise;
            decimal marketDataMinPrice = 0;
            decimal marketDataMaxPrice = 0;

            try {
                //If its cancelled don't need anymore checks:
                if (order.Status == OrderStatus.Canceled) return;

                //Calculate the model slippage: e.g. 0.01c
                decimal slip = GetSlippageApproximation(security, order);

                //Depending on the resolution, return different data types:
                MarketData marketData = security.GetLastData();

                if (marketData.Type == MarketDataType.TradeBar) {
                    marketDataMinPrice = ((TradeBar)marketData).Low;
                    marketDataMaxPrice = ((TradeBar)marketData).High;
                } else {
                    marketDataMinPrice = marketData.Price;
                    marketDataMaxPrice = marketData.Price;
                }

                //-> Valid Live/Model Order: 
                switch (order.Direction)
                {
                    case OrderDirection.Buy:
                        //Buy limit seeks lowest price
                        if (marketDataMinPrice < order.Price) {
                            order.Status = OrderStatus.Filled;
                            order.Price = security.Price;
                            order.Price += slip;
                        }
                        break;
                    case OrderDirection.Sell:
                        //Sell limit seeks highest price possible
                        if (marketDataMaxPrice > order.Price) {
                            order.Status = OrderStatus.Filled;
                            order.Price = security.Price;
                            order.Price -= slip;
                        }
                        break;
                }

                //Round off:
                order.Price = Math.Round(order.Price, 2);

            } catch (Exception err) {
                Log.Error("Equity.TransOrderDirection.LimitFill(): " + err.Message);
            }
        }



        /// <summary>
        /// Get the fees from one order, interactive brokers model.
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        public virtual decimal GetOrderFee(decimal quantity, decimal price) {
            decimal tradeFee = 0;
            quantity = Math.Abs(quantity);
            decimal tradeValue = (price * quantity);

            //Per share fees
            if (quantity < 500) {
                tradeFee = quantity * 0.013m;
            } else {
                tradeFee = quantity * 0.008m;
            }

            //Maximum Per Order: 0.5%
            //Minimum per order. $1.0
            if (tradeFee < 1) {
                tradeFee = 1;
            } else if (tradeFee > (0.005m * tradeValue)) {
                tradeFee = 0.005m * tradeValue;
            }

            //Always return a positive fee.
            return Math.Abs(tradeFee);
        }

    } // End Algorithm Transaction Filling Classes

} // End QC Namespace
