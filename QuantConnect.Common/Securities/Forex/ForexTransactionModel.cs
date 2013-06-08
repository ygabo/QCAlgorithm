/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Base Class for Algorithm.
* Algorithm Transaction Model
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
    /// Forex Transaction Model Class: Specific transaction fill models for FOREX orders
    /// </summary>
    public class ForexTransactionModel : ISecurityTransactionModel {

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
        public ForexTransactionModel() {

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
                Log.Error("ForexTransactionModel.TransOrderDirection.Fill(): " + err.Message);
            }
        }



        /// <summary>
        /// Get the Slippage approximation for this order:
        /// </summary>
        public virtual decimal GetSlippageApproximation(Security security, Order order)
        {
            //Return 0 by default
            decimal slippage = 0;
            //For FOREX, the slippage is the Bid/Ask Spread for Tick, and an approximation for the 
            switch (security.Resolution)
            {
                case Resolution.Minute:
                case Resolution.Second:
                    //Get the last data packet:
                    TradeBar lastBar = (TradeBar)security.GetLastData();
                    //Assume slippage is 1/1000th of the price
                    slippage = lastBar.Price*0.001m;
                    break;

                case Resolution.Tick:
                    Tick lastTick = (Tick) security.GetLastData();
                    switch (order.Direction)
                    {
                        case OrderDirection.Buy:
                            //We're buying, assume slip to Asking Price.
                            slippage = Math.Abs(order.Price - lastTick.AskPrice);
                            break;

                        case OrderDirection.Sell:
                            //We're selling, assume slip to the bid price.
                            slippage = Math.Abs(order.Price - lastTick.BidPrice);
                            break;
                    }
                    break;
            }
            return slippage;
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

            } catch (Exception err) {
                Log.Error("ForexTransactionModel.TransOrderDirection.MarketFill(): " + err.Message);
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

                //Check if the Stop Order was filled: opposite to a limit order
                if (order.Direction == OrderDirection.Sell) {
                    //-> 1.1 Sell Stop: If Price below setpoint, Sell:
                    if (security.Price < order.Price) {
                        order.Status = OrderStatus.Filled;
                    }
                } else if (order.Direction == OrderDirection.Buy) {
                    //-> 1.2 Buy Stop: If Price Above Setpoint, Buy:
                    if (security.Price > order.Price) {
                        order.Status = OrderStatus.Filled;
                    }
                }
            } catch (Exception err) {
                Log.Error("ForexTransactionModel.TransOrderDirection.StopFill(): " + err.Message);
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
                if (order.Direction == OrderDirection.Buy) {
                    //Buy limit seeks lowest price
                    if (marketDataMinPrice < order.Price) {
                        order.Status = OrderStatus.Filled;
                    }

                } else if (order.Direction == OrderDirection.Sell) {
                    //Sell limit seeks highest price possible
                    if (marketDataMaxPrice > order.Price) {
                        order.Status = OrderStatus.Filled;
                    }
                }
            } catch (Exception err) {
                Log.Error("ForexTransactionModel.TransOrderDirection.LimitFill(): " + err.Message);
            }
        }



        /// <summary>
        /// Get the fees from one order, interactive brokers model.
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        public virtual decimal GetOrderFee(decimal quantity, decimal price)
        {
            //Modelled order fee to 
            return 0;
        }

    } // End Algorithm Transaction Filling Classes

} // End QC Namespace
