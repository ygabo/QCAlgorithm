/*
* QUANTCONNECT.COM: Interface for Security Transaction Model Classes
* Implement this interface to define your own transaction model
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//QuantConnect Libraries:
using QuantConnect;
using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect.Securities {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Security Transaction Model
    /// </summary>
    public interface ISecurityTransactionModel {

        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/
            


        /******************************************************** 
        * CLASS PUBLIC VARIABLES
        *********************************************************/



        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
  


        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/



        /******************************************************** 
        * CLASS METHODS
        *********************************************************/

        /// <summary>
        /// Perform neccessary check to see if the model has been filled, appoximate the best we can.
        /// </summary>
        /// <param name="asset">Asset we're trading this order</param>
        /// <param name="order">Order class to check if filled.</param>
        void Fill(Security asset, ref Order order);



        /// <summary>
        /// Get the Slippage approximation for this order:
        /// </summary>
        decimal GetSlippageApproximation(Security asset, Order order);


        /// <summary>
        /// Model the slippage on a market order: fixed percentage of order price
        /// </summary>
        /// <param name="asset">Asset we're trading this order</param>
        /// <param name="order">Order to update</param>
        void MarketFill(Security asset, ref Order order);


        /// <summary>
        /// Check if the model has stopped out our position yet:
        /// </summary>
        /// <param name="asset">Asset we're trading this order</param>
        /// <param name="order">Stop Order to Check, return filled if true</param>
        void StopFill(Security asset, ref Order order);



        /// <summary>
        /// Model for a limit fill.
        /// </summary>
        /// <param name="asset">Stock Object to use to help model limit fill</param>
        /// <param name="order">Order to fill. Alter the values directly if filled.</param>
        void LimitFill(Security asset, ref Order order);



        /// <summary>
        /// Get the fees from one order. Currently defaults to interactive
        /// </summary>
        /// <param name="quantity">Quantity for this Order</param>
        /// <param name="price">Average Price for this Order</param>
        /// <returns>Decimal value of the Order Fee</returns>
        decimal GetOrderFee(decimal quantity, decimal price);

    } // End Algorithm Transaction Model Interface

} // End QC Namespace
