/*
* QUANTCONNECT.COM: Security Object
* This is a trable asset, that you build into a portfolio.
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

//QuantConnect Libraries:
using QuantConnect;
using QuantConnect.Logging;
using QuantConnect.Models;

namespace QuantConnect.Securities {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// A base "Market" Vehicle Class for Providing a common interface to Indexes / Equities / FOREX Trading.
    /// </summary>
    public partial class Security {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        /// <summary>
        /// Public Symbol Property: Code for the asset:
        /// </summary>
        public string Symbol {
            get {
                return _symbol;
            }
        }
        
        /// <summary>
        /// Public Type of Market: Equity Forex etc.
        /// </summary>
        public SecurityType Type {
            get {
                return _type;
            }
        }


        /// <summary>
        /// Public Resolution of this Market Asset.
        /// </summary>
        public Resolution Resolution {
            get {
                return _resolution;
            }
        }


        /// <summary>
        /// Public Readonly Property: If there's no new data packets for each second, we'll fill in the last packet we recieved.
        /// </summary>
        public bool IsFillDataForward {
            get {
                return _isFillDataForward;
            }
        }

        /// <summary>
        /// When its an extended market hours vehidle return true. Read only, set this when requesting the assets.
        /// </summary>
        public bool IsExtendedMarketHours
        {
            get {
                return _isExtendedMarketHours;
            }
        }

        /// <summary>
        /// Security Cache Class: Order and Data Storage:
        /// </summary>
        public virtual SecurityCache Cache { get; set; }

        /// <summary>
        /// Security Holdings Manager: Cash, Holdings, Quantity
        /// </summary>
        public virtual SecurityHolding Holdings { get; set; }

        /// <summary>
        /// Security Exchange Details Class: 
        /// </summary>
        public virtual SecurityExchange Exchange { get; set; }

        /// <summary>
        /// Security Transaction Model Storage
        /// </summary>
        public virtual ISecurityTransactionModel Model { get; set; }

        //Market Data Type:
        private Type _dataType = typeof(TradeBar);
        private string _symbol = "";
        private SecurityType _type = SecurityType.Equity;
        private Resolution _resolution = Resolution.Second;
        private bool _isFillDataForward = false;
        private bool _isExtendedMarketHours = false;
        private decimal _leverage = 1;

        /******************************************************** 
        * CONSTRUCTOR/DELEGATE DEFINITIONS
        *********************************************************/
        /// <summary>
        /// Construct the Market Vehicle
        /// </summary>
        public Security(string symbol, SecurityType type, Resolution resolution, bool fillDataForward, decimal leverage, bool extendedMarketHours) {
            //Set Basics:
            this._symbol = symbol;
            this._type = type;
            this._resolution = resolution;
            this._isFillDataForward = fillDataForward;
            this._leverage = leverage;
            this._isExtendedMarketHours = extendedMarketHours;

            //Holdings for new Vehicle:
            Cache = new SecurityCache(this);
            Holdings = new SecurityHolding(this);
            Exchange = new SecurityExchange();

            //Cannot initalise a default model.
            Model = null;

            //Set data type:
            if (resolution == Resolution.Minute || resolution == Resolution.Second) {
                _dataType = typeof(TradeBar);
            } else {
                _dataType = typeof(Tick);
            }
        }



        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// Read only property that checks if we currently own stock in the company.
        /// </summary>
        public virtual bool HoldStock {
            //Get a boolean, true if we own this stock.
            get {
                if (Holdings.AbsoluteQuantity > 0) {
                    //If we find stock in the holdings table we own stock, return true.
                    return true;
                } else {
                    //No stock found.
                    return false;
                }
            }
        }



        /// <summary>
        /// Local Time for this Market 
        /// </summary>
        public virtual DateTime Time {
            get {
                return Exchange.Time;
            }
        }



        /// <summary>
        /// Get the current value of a Market Code
        /// </summary>
        public virtual decimal Price {
            //Get the current Market value from the database
            get {
                MarketData data = GetLastData();
                if (data != null) {
                    return data.Price;
                } else {
                    //Error fetching depth
                    return 0;
                }
            }
        }



        /// <summary>
        /// Leverage for this Security.
        /// </summary>
        public virtual decimal Leverage
        {
            get { 
                return _leverage; 
            }
        }



        /// <summary>
        /// If this uses tradebar data, return the most recent high.
        /// </summary>
        public virtual decimal High {
            get { 
                MarketData data = GetLastData();
                if (data.Type == MarketDataType.TradeBar) {
                    return ((TradeBar)data).High;
                } else {
                    return data.Price;
                }
            }
        }


        /// <summary>
        /// If this uses tradebar data, return the most recent low.
        /// </summary>
        public virtual decimal Low {
            get {
                MarketData data = GetLastData();
                if (data.Type == MarketDataType.TradeBar) {
                    return ((TradeBar)data).Low;
                } else {
                    return data.Price;
                }
            }
        }

        /// <summary>
        /// If this uses tradebar data, return the most recent close.
        /// </summary>
        public virtual decimal Close {
            get {
                MarketData data = GetLastData();
                if (data.Type == MarketDataType.TradeBar) {
                    return ((TradeBar)data).Close;
                } else {
                    return data.Price;
                }
            }
        }


        /// <summary>
        /// If this uses tradebar data, return the most recent open.
        /// </summary>
        public virtual decimal Open {
            get {
                MarketData data = GetLastData();
                if (data.Type == MarketDataType.TradeBar) {
                    return ((TradeBar)data).Open;
                } else {
                    return data.Price;
                }
            }
        }
        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        
        /// <summary>
        /// Get a single data packet
        /// </summary>
        /// <returns></returns>
        public MarketData GetLastData() {
            return this.Cache.GetData();
        }


        /// <summary>
        /// Update the Market Online Calculations:
        /// </summary>
        /// <param name="data">New Data packet:</param>
        /// <param name="frontier"></param>
        public void Update(DateTime frontier, MarketData data) { 

            //Update the Exchange/Timer:
            Exchange.SetDateTimeFrontier(frontier);

            //Add new point to cache:
            if (data != null)
            {
                Cache.AddData(data);
            }

            //Update Online Calculations:
        }


    } // End Market

} // End QC Namespace
