/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Base Class for Algorithm.
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


using QuantConnect.Securities;
using QuantConnect.Models;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// QC Algorithm Base Class - Handle the basic requirement of a trading algorithm, 
    /// allowing user to focus on event methods.
    /// </summary>
    public partial class QCAlgorithm : MarshalByRefObject, IAlgorithm {

        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/
        private DateTime _time = new DateTime();
        private DateTime _startDate = new DateTime(2012, 06, 01);   //Defaults.
        private DateTime _endDate = new DateTime(2012, 06, 30);
        private RunMode _runMode = RunMode.Automatic;
        private bool _locked = false;
        private String _resolution = "";
        private bool _quit = false;
        private List<string> _debugMessages = new List<string>();
        private List<string> _errorMessages = new List<string>();

        /******************************************************** 
        * CLASS PUBLIC VARIABLES
        *********************************************************/

        /// <summary>
        /// Securities Entities 
        /// --> Managers group and offer meta data analysis e.g. portfolio.
        /// </summary>
        public SecurityManager Securities { get; set; }
        public SecurityPortfolioManager Portfolio { get; set; }
        public SecurityTransactionManager Transacions { get; set; }

        /// <summary>
        /// Generic Data Manager - Required for compiling all data feeds in order,
        /// and passing them into algorithm event methods.
        /// </summary>
        public DataManager DataManager { get; set; }


        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Initialise the Algorithm
        /// </summary>
        public QCAlgorithm() { 
            //Initialise the Algorithm Helper Classes:
            //- Note - ideally these wouldn't be here, but because of the DLL we need to make the classes shared across 
            //  the Worker & Algorithm, limiting ability to do anything else.
            Securities = new SecurityManager();
            Transacions = new SecurityTransactionManager(Securities);
            Portfolio = new SecurityPortfolioManager(Securities, Transacions);

            //Initialise Data Manager 
            DataManager = new DataManager();

            //Initialise Error and Order Holders:
            Errors = new List<string>();

            //Initialise Algorithm RunMode to Automatic:
            _runMode = RunMode.Automatic;

            //Initialise to unlocked:
            _locked = false;

            //Initialise Start and End Dates:
            _startDate = new DateTime();
            _endDate = new DateTime();
        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// Set a public name for the algorithm.
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Get the current date/time.
        /// </summary>
        public DateTime Time {
            get {
                return _time;
            }
        }


        /// <summary>
        /// Get requested simulation start date set with SetStartDate()
        /// </summary>
        public DateTime StartDate {
            get {
                return _startDate;
            }
        }


        /// <summary>
        /// Get requested simulation end date set with SetEndDate()
        /// </summary>
        public DateTime EndDate {
            get {
                return _endDate;
            }
        }


        /// <summary>
        /// Catchable Error List.
        /// </summary>
        public List<string> Errors {
            get;
            set;
        }


        /// <summary>
        /// Accessor for Filled Orders:
        /// </summary>
        public Dictionary<int, Order> Orders {
            get {
                return Transacions.ProcessedOrders;
            }
        }


        /// <summary>
        /// Simulation Server setup RunMode for the Algorithm: Automatic, Parallel or Series.
        /// </summary>
        public RunMode RunMode {
            get {
                return _runMode;
            }
        }


        /// <summary>
        /// Check if the algorithm is locked from any further init changes.
        /// </summary>
        public bool Locked {
            get {
                return _locked;
            }
        }



        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        
        /// <summary>
        /// Initialise the data and resolution requiredv 
        /// </summary>
        public virtual void Initialize() {
            //Setup Required Data
            throw new NotImplementedException("Please override the Intitialize() method");
        }

        /// <summary>
        /// New data routine: handle new data packets. Algorithm starts here..
        /// </summary>
        /// <param name="symbols">Dictionary of MarketData Objects</param>
        public virtual void OnTradeBar(Dictionary<string, TradeBar> symbols) {
            //Algorithm Implementation
            throw new NotImplementedException("Please override the OnTradeBar() method");
        }

        /// <summary>
        /// Twitter sentiment data in 10 minute bars:
        /// </summary>
        /// <param name="reports">Buzz/Sentiment values of twitter for this stock.</param>
        public virtual void OnStockPulse(Dictionary<string, StockPulse> reports) {
            //StockPulse Implementation
            throw new NotImplementedException("Please override the OnStockPulse() method");
        }

        /// <summary>
        /// Handle a new incoming Tick Packet:
        /// </summary>
        /// <param name="ticks">Ticks arriving at the same moment come in a list. Because the "tick" data is actually list ordered within a second, you can get lots of ticks at once.</param>
        public virtual void OnTick(Dictionary<string, List<Tick>> ticks) {
            //Algorithm Implementation
            throw new NotImplementedException("Please override the OnTick() method");
        }

        /// <summary>
        /// Handle a new incoming Estimize Packet:
        /// </summary>
        /// <param name="estimates">New Estimize</param>
        public virtual void OnEstimize(Dictionary<string, List<Estimize>> estimates) {
            //Estimize Implementation
            throw new NotImplementedException("Please override the OnEstimize() method");
        }

        /// <summary>
        /// Set the current datetime frontier: the most forward looking tick so far. This is used by backend to advance time. Do not modify
        /// </summary>
        /// <param name="frontier">Current datetime.</param>
        public void SetDateTime(DateTime frontier) {
            this._time = frontier;
        }

        /// <summary>
        /// Set the RunMode for the Servers. If you are running an overnight algorithm, you must select series.
        /// Automatic will analyse the selected data, and if you selected only minute data we'll select series for you.
        /// </summary>
        /// <param name="mode">Enum RunMode with options Series, Parallel or Automatic. Automatic scans your requested symbols and resolutions and makes a decision on the fastest analysis</param>
        public void SetRunMode(RunMode mode) {
            if (!Locked) {
                this._runMode = mode;
            } else {
                throw new Exception("Algorithm.SetRunMode(): Cannot change run mode after algorithm initialized.");
            }
        }

        /// <summary>
        /// Set the requested balance to launch this algorithm
        /// </summary>
        /// <param name="startingCash">Minimum required cash</param>
        public void SetCash(decimal startingCash) {
            if (!Locked) {
                Portfolio.SetCash(startingCash);
            } else {
                throw new Exception("Algorithm.SetCash(): Cannot change cash available after algorithm initialized.");
            }
        }


        /// <summary>
        /// Set the Transaction Models: allow user to override the default transaction models
        /// </summary>
        public virtual void SetModels() 
        {
            try
            {
                //Define the transaction and fill models we'd like our securities to use.
                //Users can generate their own models by implementing the Interface ISecurityTransactionModel.
                EquityModel = new EquityTransactionModel();
                ForexModel = new ForexTransactionModel();

                foreach (string symbol in Securities.Keys)
                {
                    switch (Securities[symbol].Type)
                    {
                        //Currently only have equity data.
                        case SecurityType.Equity:
                            Securities[symbol].Model = EquityModel;
                            break;

                        //FOREX data coming soon.
                        case SecurityType.FOREX:
                            Securities[symbol].Model = ForexModel;
                            break;
                    }
                }
                //Custom example of transaction model for a penny stock: 
                // ->   You can derive a transaction model for a single company if required.
                //      Securities["PENNY"].Model = new PennyStockTransactionModel();
                //      public class PennyStockTransactionModel : ISecurityTransactionModel { ... }
            }
            catch (Exception err)
            {
                throw new Exception("Algorithm.SetModels(): " + err.Message);
            }
        }



        /// <summary>
        /// Wrapper for SetStartDate(DateTime). Set the start date for simulation.
        /// Must be less than end date.
        /// </summary>
        /// <param name="year">int year</param>
        /// <param name="month">int month</param>
        /// <param name="day">int day</param>
        public void SetStartDate(int year, int month, int day) 
        {
            try {
                this.SetStartDate(new DateTime(year, month, day));
            } catch (Exception err) {
                throw new Exception("Date Invalid: " + err.Message);
            }
        }

        /// <summary>
        /// Wrapper for SetEndDate(datetime). Set the end simulation date. 
        /// </summary>
        /// <param name="year">int year</param>
        /// <param name="month">int month</param>
        /// <param name="day">int day</param>
        public void SetEndDate(int year, int month, int day) 
        {
            try {
                this.SetEndDate(new DateTime(year, month, day));
            } catch (Exception err) {
                throw new Exception("Date Invalid: " + err.Message);
            }
        }

        /// <summary>
        /// Set the start date for the simulation 
        /// Must be less than end date and within data available
        /// </summary>
        /// <param name="start">Datetime start date</param>
        public void SetStartDate(DateTime start) 
        { 
            //Validate the start date:
            //1. Check range;
            if (start < (new DateTime(1998, 01, 01))) {
                throw new Exception("Please select data between January 1st, 1998 to July 31st, 2012.");
            }
            //2. Check end date greater:
            if (_endDate != new DateTime()) {
                if (start > _endDate) {
                    throw new Exception("Please select start date less than end date.");
                }
            }
            //3. Check not locked already:
            if (!Locked) {
                this._startDate = start;
            } else {
                throw new Exception("Algorithm.SetStartDate(): Cannot change start date after algorithm initialized.");
            }
        }

        /// <summary>
        /// Set the end date for a simulation. 
        /// Must be greater than the start date
        /// </summary>
        /// <param name="end"></param>
        public void SetEndDate(DateTime end) 
        { 
            //Validate:
            //1. Check Range:
            if (end > (new DateTime(2012, 07, 31))) {
                throw new Exception("Please select data between January 1st, 1998 to July 31st, 2012.");
            }
            //2. Check start date less:
            if (_startDate != new DateTime()) {
                if (end < _startDate) {
                    throw new Exception("Please select end date greater than start date.");
                }
            }
            //3. Check not locked already:
            if (!Locked) {
                this._endDate = end;
            } else {
                throw new Exception("Algorithm.SetEndDate(): Cannot change end date after algorithm initialized.");
            }
        }

        /// <summary>
        /// Lock the algorithm initialization to avoid messing with cash and data streams.
        /// </summary>
        public void SetLocked() 
        {
            this._locked = true;
        }

        /// <summary>
        /// Get a list of the debug messages sent so far.
        /// </summary>
        /// <returns>List of debug string messages sent</returns>
        public List<string> GetDebugMessages() 
        {
            return _debugMessages;
        }

        /// <summary>
        /// Add specified data to required list. QC will funnel this data to the handle data routine.
        /// </summary>
        /// <param name="securityType">MarketType Type: Equity, Commodity, Future or FOREX</param>
        /// <param name="symbol">Symbol Reference for the MarketType</param>
        /// <param name="resolution">Resolution of the Data Required</param>
        /// <param name="fillDataForward">When no data available on a tradebar, return the last data that was generated</param>
        /// <param name="leverage">Custom leverage per security</param>
        public void AddSecurity(SecurityType securityType, string symbol, Resolution resolution = Resolution.Minute, bool fillDataForward = true, decimal leverage = 0) 
        {
            try {
                if (!_locked) {
                    symbol = symbol.ToUpper();

                    if (securityType != SecurityType.Equity) {
                        throw new Exception("We only support equities at this time.");
                    }

                    if (_resolution != "" && _resolution != resolution.ToString()) {
                        throw new Exception("We can only accept one resolution at this time. Make all your datafeeds the lowest resolution you require.");
                    }

                    //If it hasn't been set, use some defaults based on the portfolio type:
                    if (leverage <= 0)
                    {
                        switch (securityType)
                        {
                            case SecurityType.Equity:
                                leverage = 1;
                                break;
                            case SecurityType.FOREX:
                                leverage = 50;
                                break;
                        }
                    }

                    //Add the symbol to Data Manager -- generate unified data streams for algorithm events
                    DataManager.Add(securityType, symbol, resolution, fillDataForward);

                    //Add the symbol to Securities Manager -- manage collection of portfolio entities for easy access.
                    Securities.Add(symbol, securityType, resolution, fillDataForward, leverage);
                    
                } else {
                    throw new Exception("Algorithm.AddSecurity(): Cannot add another security after algorithm running.");
                }

            } catch (Exception err) {
                Error("Algorithm.AddMarketData(): " + err.Message);
            }
        }

        /// <summary>
        /// Add this sentiment-data to list of requested data streams for events:
        /// </summary>
        /// <param name="type">Enum SentimentDataType: Estimize, Stockpulse</param>
        /// <param name="symbol">String symbol of asset we'd like sentiment data</param>
        public void AddSentimentData(SentimentDataType type, string symbol) 
        {
            try {
                if (!_locked) {
                    symbol = symbol.ToUpper();
                    DataManager.Add(type, symbol);
                } else {
                    throw new Exception("Algorithm.AddMetaData(): Modify meta-data requested while algorithm running.");
                }
            } catch (Exception err) {
                Error("Algorithm.AddMetaData(): " + err.Message);
            }
        }

        /// <summary>
        /// Submit a new order for quantity of symbol using type order.
        /// </summary>
        /// <param name="type">Buy/Sell Limit or Market Order Type.</param>
        /// <param name="symbol">Symbol of the MarketType Required.</param>
        /// <param name="quantity">Number of shares to request.</param>
        public int Order(string symbol, int quantity, OrderType type = OrderType.Market) 
        {
            //Add an order to the transacion manager class:
            int orderId = -1;
            decimal price = 0;
            string orderRejected = "Order Rejected at " + Time.ToShortDateString() + " " + Time.ToShortTimeString() + ": ";

            //Internals use upper case symbols.
            symbol = symbol.ToUpper();

            //Ordering 0 is useless.
            if (quantity == 0) {
                return orderId;
            }

            if (type != OrderType.Market) {
                Debug(orderRejected + "Currently only market orders supported.");
            }

            //If we're not tracking this symbol: throw error:
            if (!Securities.ContainsKey(symbol)) {
                Debug(orderRejected + "You haven't requested " + symbol + " data. Add this with AddSecurity() in the Initialize() Method.");
            }

            //Set a temporary price for validating order for market orders:
            if (type == OrderType.Market) {
                price = Securities[symbol].Price;
            }

            try
            {
                orderId = Transacions.AddOrder(new Order(symbol, quantity, type, Time, price), Portfolio);

                if (orderId < 0) { 
                    //Order failed validaity checks and was rejected:
                    Debug(orderRejected + OrderErrors.ErrorTypes[orderId]);
                }
            }
            catch (Exception err) {
                Error("Algorithm.Order(): Error sending order. " + err.Message);
            }
            return orderId;
        }

        /// <summary>
        /// Liquidate all holdings. Called at the end of day for tick-strategies.
        /// </summary>
        /// <returns>Array of order ids for liquidated symbols</returns>
        public List<int> Liquidate(string symbolToLiquidate = "")
        {
            int quantity = 0;
            List<int> orderIdList = new List<int>();

            symbolToLiquidate = symbolToLiquidate.ToUpper();

            foreach (string symbol in Securities.Keys) {

                //Send market order to liquidate if 1, we have stock, 2, symbol matches.
                if (Portfolio[symbol].HoldStock && (symbol == symbolToLiquidate || symbolToLiquidate == "")) {
                    
                    if (Portfolio[symbol].IsLong)
                    {
                        quantity = -Portfolio[symbol].Quantity;
                    }
                    else
                    {
                        quantity = Math.Abs(Portfolio[symbol].Quantity);
                    }
                    //Liquidate at market price.
                    orderIdList.Add(Transacions.AddOrder(new Order(symbol, quantity, OrderType.Market, Time, Securities[symbol].Price), Portfolio));
                }
            }
            return orderIdList;
        }

        /// <summary>
        /// Send a debug message to the console:
        /// </summary>
        /// <param name="message">Message to send to debug console</param>
        public void Debug(string message)
        {
            if (message == "") return;
            _debugMessages.Add(message);
        }

        /// <summary>
        /// Send Error Message to the Console.
        /// </summary>
        /// <param name="message">Message to display in errors grid</param>
        public void Error(string message)
        {
            if (message == "") return;
            _errorMessages.Add(message);
        }

        /// <summary>
        /// Terminate the algorithm on exiting the current event processor. 
        /// If have holdings at the end of the algorithm/day they will be liquidated at market prices.
        /// If running a series analysis this command skips the current day (and doesn't liquidate).
        /// </summary>
        /// <param name="message">Exit message</param>
        public void Quit(string message = "") 
        {
            Debug("Quit(): " + message);
            _quit = true;
        }

        /// <summary>
        /// Check if the Quit Flag is Set:
        /// </summary>
        public void SetQuit(bool quit) 
        {
            _quit = quit;
        }

        /// <summary>
        /// Get the quit flag state.
        /// </summary>
        /// <returns>Boolean true if set to quit event loop.</returns>
        public bool GetQuit() 
        {
            return _quit;
        }

    } // End Algorithm Template

} // End QC Namespace
