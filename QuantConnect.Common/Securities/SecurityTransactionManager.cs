/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Base Class for Algorithm.
* Algorithm Transaction Recorder
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
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Algorithm Transactions Manager - Recording Transactions
    /// </summary>
    public class SecurityTransactionManager {

        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/
        private SecurityManager Securities;
        

        /******************************************************** 
        * CLASS PUBLIC VARIABLES
        *********************************************************/
        //System for keeping Live/modelled data uniform.
        private int _orderID = 1;
        private decimal _minimumOrderSize = 0;
        private int _minimumOrderQuantity = 1;

        //Order Monitor Cache:
        //Order Cache: Record of Orders
        public Dictionary<int, Order> ProcessedOrders = new Dictionary<int, Order>();
        public Dictionary<int, Order> OutstandingOrders = new Dictionary<int,Order>();
        public Dictionary<DateTime, decimal> TransactionRecord = new Dictionary<DateTime, decimal>();

        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Initialise the Algorithm Transaction Class
        /// </summary>
        public SecurityTransactionManager(SecurityManager security) {

            //Private reference for processing transactions
            this.Securities = security;

            //Initialise the Order Caches:
            this.ProcessedOrders = new Dictionary<int, Order>();
            this.OutstandingOrders = new Dictionary<int, Order>();
        }


        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/

        /// <summary>
        /// Configurable Minimum Order Size to override bad orders, Default 0:
        /// </summary>
        public decimal MinimumOrderSize {
            get {
                return _minimumOrderSize;
            }
        }


        /// <summary>
        /// Configurable Minimum Order Quantity: Default 0
        /// </summary>
        public int MinimumOrderQuantity {
            get {
                return _minimumOrderQuantity;
            }
        }

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
            
        /// <summary>
        /// Add an Order and return the Order ID or negative if an error.
        /// </summary>
        public virtual int AddOrder(Order order, SecurityPortfolioManager portfolio) {

            try {
                //Ensure its rounded off:
                order.Status = order.Status = OrderStatus.Submitted;

                //Perform last minute validations on the requested order to ensure not loco.
                /*int orderError = ValidateOrder(order, portfolio, order.Time, Int32.MaxValue, order.Price);
                if (orderError < 0) {
                    //The order hasn't past validations: return no order:
                    return orderError;
                }*/

                //Increment the global ID counter.
                order.Id = _orderID++;

                //This hasn't been processed yet: add to outstanding cache:
                AddOutstandingOrder(order);

            } catch (Exception err) {
                Log.Error("Algorithm.Transaction.AddOrder(): " + err.Message);
            }

            return order.Id;
        }


        /// <summary>
        /// Update an order yet to be filled / stop / limit.
        /// </summary>
        /// <param name="order">Order to Update</param>
        /// <param name="portfolio"></param>
        /// <returns>id if the order we modified.</returns>
        public int UpdateOrder(Order order, SecurityPortfolioManager portfolio, int maxOrders) {
            try {
                //Update the order from the behaviour
                int id = order.Id;
                order.Time = Securities[order.Symbol].Time;

                //Run through a list of prepurchase checks, if any are false stop the transaction
                int orderError = ValidateOrder(order, portfolio, order.Time, maxOrders, order.Price);
                if (orderError < 0) {
                    return orderError;
                }

                if (OutstandingOrders.ContainsKey(id)) {
                    //-> If its already filled return false; can't be updated
                    if (OutstandingOrders[id].Status == OrderStatus.Filled || OutstandingOrders[id].Status == OrderStatus.Canceled) {
                        return -5;
                    } else {
                        OutstandingOrders[id] = order;
                    }

                } else {
                    //-> Its not in the orders cache, shouldn't get here
                    return -6;
                }

                return 0;

            } catch (Exception err) {
                Log.Error("Algorithm.Transactions.UpdateOrder(): " + err.Message);
                return -7;
            }
        }



        /// <summary>
        /// Scan through all the outstanding order cache and see if any have been filled:
        /// </summary>
        /// <returns>Dictionary fillErrors of order key with error-id value: 0 for no error.</returns>
        public virtual Dictionary<Order, int> RefreshOrderModel(SecurityPortfolioManager portfolio, int maxOrders, bool skipValidations = false) {

            //Remove outstanding after to preserve the iterating list:
            int orderError = 0;
            Dictionary<Order, int> orderStatus = new Dictionary<Order, int>();
            List<int> ordersToRemove = new List<int>();

            try {
                //Loop by the order id's for easy updating.
                foreach (int id in OutstandingOrders.Keys) {
                    //Fetch the required order:
                    Order order = OutstandingOrders[id];

                    //Now re-validate the order:
                    if (skipValidations == false)
                    {
                        orderError = ValidateOrder(order, portfolio, order.Time, maxOrders, order.Price);
                        orderStatus.Add(order, orderError);
                        if (orderError != 0)
                        {
                            Log.Trace("Order Rejected: Symbol:" + order.Symbol + " Price: " + order.Price + "  Time: " + order.Time.ToLongTimeString());
                            continue;
                        }
                        orderStatus.Remove(order);
                    }

                    //IF order is valid -- use the fill model to determine fill status:
                    Securities[order.Symbol].Model.Fill(Securities[order.Symbol], ref order);
                    
                    //If its filled, update the local & behaviour holdings.
                    switch (order.Status)
                    {
                        case OrderStatus.Filled:
                            order.Time = Securities[order.Symbol].Time;
                            ordersToRemove.Add(order.Id);
                            portfolio.ProcessFill(order);   //Although not returned to parent, fill here to people can't order more than buying power.
                            ProcessedOrders.Add(order.Id, order);
                            break;

                        case OrderStatus.Canceled:
                            order.Time = Securities[order.Symbol].Time;
                            ordersToRemove.Add(order.Id);
                            break;
                    }

                    //Update the order: set to 0 for successful exit.
                    orderStatus.Add(order, 0);

                    Console.WriteLine("NEW ORDER: Price: " + order.Price.ToString("C") + " Date: " + order.Time.Date.ToLongDateString() + " Time:" + order.Time.ToLongTimeString() + " Symbol: " + order.Symbol);
                }

                //Remove all requested id's:
                ordersToRemove.ForEach(i => RemoveOutstandingOrder(i));

            } catch (Exception err) {
                Log.Error("Algorithm.Transaction.RefreshOrderModel(): " + err.Message);
            }

            return orderStatus;
        }



        /// <summary>
        /// Add an order to attempt a fill. If this is a market order it will be filled immediately.
        /// </summary>
        /// <param name="order">New Order to Fill</param>
        public virtual void AddOutstandingOrder(Order order) {
            try
            {
                //Add the order to the cache to monitor
                if (!OutstandingOrders.ContainsKey(order.Id)) {
                    OutstandingOrders.Add(order.Id, order);
                } else {
                    Log.Error("Security.Holdings.AddOutstandingOrder(): Duplicate order id in OutstandingOrderCache");
                }
            }
            catch (Exception err)
            {
                Log.Error("TransactionManager.AddOutstandingOrder(): " + err.Message);
            }
        }


        /// <summary>
        /// Remove this order from outstanding queue: its been filled or cancelled.
        /// </summary>
        /// <param name="orderId">Specific order id to remove</param>
        public virtual void RemoveOutstandingOrder(int orderId) {
            try
            {
                if (OutstandingOrders.ContainsKey(orderId)) {
                    OutstandingOrders.Remove(orderId);
                } else {
                    Log.Error("Security.Holdings.RemoveOutstandingOrder(): Cannot remove outstanding order, not found");
                }
            }
            catch (Exception err)
            {
                Log.Error("TransactionManager.RemoveOutstandingOrder(): " + err.Message);
            }
        }


        /// <summary>
        /// Validate the transOrderDirection is a sensible choice, factoring in basic limits.
        /// </summary>
        /// <param name="order">Order to Validate</param>
        /// <param name="portfolio">Security portfolio object we're working on.</param>
        /// <param name="time">Current actual time</param>
        /// <param name="maxOrders">Maximum orders per day/period before rejecting.</param>
        /// <param name="price">Current actual price of security</param>
        /// <returns>If negative its an error, or if 0 no problems.</returns>
        public int ValidateOrder(Order order, SecurityPortfolioManager portfolio, DateTime time, int maxOrders = 50, decimal price = 0) {

            //Calculate the run mode:

            //-1: Order quantity must not be zero
            if (order.Quantity == 0 || order.Direction == OrderDirection.Hold) {
                return -1;
            }

            //-2: There is no data yet for this security - please wait for data (market order price not available yet)
            if (order.Price <= 0) {
                return -2;
            }

            //-3: Attempting market order outside of market hours
            if (!portfolio[order.Symbol].Vehicle.Exchange.ExchangeOpen && order.Type == OrderType.Market) {
                return -3;
            }

            //-4: Insufficient capital to execute order
            if (GetSufficientCapitalForOrder(portfolio, order) == false) {
                return -4;
            }

            //-5: Exceeded maximum allowed orders for one analysis period.
            if (ProcessedOrders.Count > maxOrders) {
                return -5;
            }

            //-6: Order timestamp error. Order appears to be executing in the future
            if (order.Time > time) {
                return -6;
            }

            return 0;
        }



        /// <summary>
        /// Check if there is sufficient capital to execute this order.
        /// </summary>
        /// <param name="portfolio">Our portfolio</param>
        /// <param name="order">Order we're checking</param>
        /// <returns>True if suficient capital.</returns>
        private bool GetSufficientCapitalForOrder(SecurityPortfolioManager portfolio, Order order)
        {
            //First simple check, when don't hold stock, this will always increase portfolio regardless of direction
            if (Math.Abs(GetOrderRequiredBuyingPower(order)) > portfolio.GetBuyingPower(order.Symbol, order.Direction)) {
                return false;
            } else {
                return true;
            }
        }



        /// <summary>
        /// Using leverage property of security find the required cash for this order:
        /// </summary>
        /// <param name="order">Order to check</param>
        /// <returns>decimal cash required to purchase order</returns>
        private decimal GetOrderRequiredBuyingPower(Order order)
        {
            try {
                return Math.Abs(order.Value) / Securities[order.Symbol].Leverage;    
            } 
            catch(Exception err)
            {
                Log.Error("Security.TransactionManager.GetOrderRequiredBuyingPower(): " + err.Message);
            }
            //Prevent all orders if leverage is 0.
            return decimal.MaxValue;
        }


        /// <summary>
        /// Given this portfolio and order, what would the final portfolio holdings be if it were filled.
        /// </summary>
        /// <param name="portfolio">Portfolio we're running</param>
        /// <param name="order">Order requested to process </param>
        /// <returns>decimal final holdings </returns>
        private decimal GetExpectedFinalHoldings(SecurityPortfolioManager portfolio, Order order)
        {
            decimal expectedFinalHoldings = 0;

            if (portfolio.TotalAbsoluteHoldings > 0) {
                foreach (Security company in Securities.Values) 
                {
                    if (order.Symbol == company.Symbol) 
                    {
                        //If the same holding, we must check if its long or short.
                        expectedFinalHoldings += Math.Abs(company.Holdings.HoldingValue + (order.Price * (decimal)order.Quantity));
                        Log.Debug("HOLDINGS: " + company.Holdings.HoldingValue + " - " + "ORDER: (P: " + order.Price + " Q:" + order.Quantity + ") EXPECTED FINAL HOLDINGS: " + expectedFinalHoldings + " BUYING POWER: " + portfolio.GetBuyingPower(order.Symbol));
                    } else {
                        //If not the same asset, then just add the absolute holding to the final total:
                        expectedFinalHoldings += company.Holdings.AbsoluteHoldings;
                    }
                }
            } else {
                //First purchase: just make calc abs order size:
                expectedFinalHoldings = (order.Price * (decimal)order.Quantity);
            }

            return expectedFinalHoldings;
        }


    } // End Algorithm Transaction Filling Classes


} // End QC Namespace
