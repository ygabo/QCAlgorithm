/*
 * QUANTCONNECT.COM - 
 * Order Models -- Common enums and classes for orders
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections.Generic;

namespace QuantConnect {
    
    /******************************************************** 
    * ORDER CLASS DEFINITION
    *********************************************************/
    /// <summary>
    /// Type of the Order: Market, Limit or Stop
    /// </summary>
    public enum OrderType {
        Market,
        Limit,
        Stop
    }


    /// <summary>
    /// Direction of the Order:
    /// </summary>
    public enum OrderDirection { 
        Buy,
        Sell,
        Hold
    }


    /// <summary>
    /// Status of the order class.
    /// </summary>
    public enum OrderStatus {
        Submitted,
        PartiallyFilled,
        Filled,
        Canceled,
        None,
    }


    /// <summary>
    /// Indexed Order Codes:
    /// </summary>
    public static class OrderErrors {
        /// <summary>
        /// Order Validation Errors
        /// </summary>
        public static Dictionary<int, string> ErrorTypes = new Dictionary<int, string>() {
            {-1, "Order quantity must not be zero"},
            {-2, "There is no data yet for this security - please wait for data (market order price not available yet)"},
            {-3, "Attempting market order outside of market hours"},
            {-4, "Insufficient capital to execute order"},
            {-5, "Exceeded maximum allowed orders for one analysis period"},
            {-6, "Order timestamp error. Order appears to be executing in the future"},
            {-7, "General error in order"},
            {-8, "Order has already been filled and cannot be modified"},
        };


    }


    /// <summary>
    /// Trade Struct for Recording Results:
    /// </summary>
    public class Order {
        public int Id;
        public string Symbol;
        public decimal Price;
        public DateTime Time;
        public int Quantity;
        public OrderType Type;
        public OrderStatus Status;

        /// <summary>
        /// Order Direction Property based off Quantity.
        /// </summary>
        public OrderDirection Direction {
            get {
                if (Quantity > 0) {
                    return OrderDirection.Buy;
                } else if (Quantity < 0) {
                    return OrderDirection.Sell;
                } else {
                    return OrderDirection.Hold;
                }
            }
        }

        /// <summary>
        /// Get the Absolute (non-negative) quantity.
        /// </summary>
        public decimal AbsoluteQuantity {
            get {
                return Math.Abs(Quantity);
            }
        }

        /// <summary>
        /// Value of the Order:
        /// </summary>
        public decimal Value {
            get {
                return Convert.ToDecimal(Quantity) * Price;
            }
        }

        public Order(string symbol, int quantity, OrderType order, DateTime time, decimal price = 0) {
            this.Time = time;
            this.Price = price;
            this.Type = order;
            this.Quantity = quantity;
            this.Symbol = symbol;
            this.Status = OrderStatus.None;
        }
    }


    /// <summary>
    /// Orders Struct for communication with JS IDE (JS/PHP code uses hungarian notation)
    /// </summary>
    public class HungarianOrder {
        public int id;
        public string sSymbol;
        public decimal dPrice;
        public DateTime dtTime;
        public int iQuantity;
        public OrderType eType;
        public OrderStatus eStatus;

        /// <summary>
        /// Order Direction Property based off Quantity.
        /// </summary>
        public OrderDirection eDirection {
            get {
                if (iQuantity > 0) {
                    return OrderDirection.Buy;
                } else if (iQuantity < 0) {
                    return OrderDirection.Sell;
                } else {
                    return OrderDirection.Hold;
                }
            }
        }

        /// <summary>
        /// Get the decimal quantity
        /// </summary>
        public decimal dQuantity {
            get {
                return Convert.ToDecimal(iQuantity);
            }
        }

        /// <summary>
        /// Get the Absolute (non-negative) quantity.
        /// </summary>
        public decimal iAbsoluteQuantity {
            get {
                return Math.Abs(iQuantity);
            }
        }

        /// <summary>
        /// Value of the Order:
        /// </summary>
        public decimal dValue {
            get {
                return Convert.ToDecimal(iQuantity) * dPrice;
            }
        }

        public HungarianOrder(Order cOrder) {
            this.id = cOrder.Id;
            this.dtTime = cOrder.Time.Round(TimeSpan.FromSeconds(1));
            this.dPrice = cOrder.Price;
            this.eType = cOrder.Type;
            this.sSymbol = cOrder.Symbol;
            this.eStatus = cOrder.Status;
            this.iQuantity = cOrder.Quantity;
        }
    }

} // End QC Namespace:
