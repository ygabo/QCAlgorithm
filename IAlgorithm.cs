/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals, V0.1
 * Created by Jared Broad
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantConnect  {

    /******************************************************** 
    * QUANTCONNECT PROJECT LIBRARIES
    *********************************************************/
    using QuantConnect.Securities;
    using QuantConnect.Models;

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// Interface for Algorithm Class Libraries
    /// </summary>
    public interface IAlgorithm {

        /******************************************************** 
        * INTERFACE PROPERTIES:
        *********************************************************/
        /// <summary>
        /// Get/Set the Data Manager
        /// </summary>
        DataManager DataManager {
            get;
            set;
        }

        /// <summary>
        /// Security Object Collection Class
        /// </summary>
        SecurityManager Securities { 
            get; 
            set; 
        }

        /// <summary>
        /// Security Portfolio Management Class:
        /// </summary>
        SecurityPortfolioManager Portfolio { 
            get; 
            set; 
        }

        /// <summary>
        /// Security Transaction Processing Class.
        /// </summary>
        SecurityTransactionManager Transacions { 
            get; 
            set;
        }

        /// <summary>
        /// Set a public name for the algorithm.
        /// </summary>
        string Name {
            get;
            set;
        }

        /// <summary>
        /// Get the current date/time.
        /// </summary>
        DateTime Time {
            get;
        }

        /// <summary>
        /// Get Requested Simulation Start Date
        /// </summary>
        DateTime StartDate {
            get;
        }

        /// <summary>
        /// Get Requested Simulation End Date
        /// </summary>
        DateTime EndDate {
            get;
        }

        /// <summary>
        /// Accessor for Filled Orders:
        /// </summary>
        Dictionary<int, Order> Orders {
            get;
        }

        /// <summary>
        /// Run Simulation Mode for the algorithm: Automatic, Parallel or Series.
        /// </summary>
        RunMode RunMode {
            get;
        }

        /// <summary>
        /// Indicator if the algorithm has been initialised already. When this is true cash and securities cannot be modified.
        /// </summary>
        bool Locked {
            get;
        }

        /******************************************************** 
        * INTERFACE METHODS
        *********************************************************/
        /// <summary>
        /// Initialise the Algorithm and Prepare Required Data:
        /// </summary>
        void Initialize();


        /// <summary>
        /// Update the algorithm calculations:
        /// </summary>
        /// <param name="symbols">Dictionary of TradeBar Data-Objects for this timeslice</param>
        void OnTradeBar(Dictionary<string, TradeBar> symbols);


        /// <summary>
        /// Handler for Tick Events
        /// </summary>
        /// <param name="ticks">Tick Data Packet</param>
        void OnTick(Dictionary<string, List<Tick>> ticks);


        /// <summary>
        /// A crowd sourced earnings estimate.
        /// </summary>
        /// <param name="estimates">Earning estimate signal from a user</param>
        void OnEstimize(Dictionary<string, List<Estimize>> estimates);


        /// <summary>
        /// Twitter Sentiment Estimate
        /// </summary>
        /// <param name="reports">Individual stock pulse for this timeslice</param>
        void OnStockPulse(Dictionary<string, StockPulse> reports);


        /// <summary>
        /// Call this method at the end of the algorithm.
        /// </summary>
        void OnExitSimulation();

        /// <summary>
        /// Set the DateTime Frontier: This is the master time and is 
        /// </summary>
        /// <param name="time"></param>
        void SetDateTime(DateTime time);


        /// <summary>
        /// Set the run mode of the algorithm: series, parallel or automatic.
        /// </summary>
        /// <param name="mode">Run mode to select, default Automatic</param>
        void SetRunMode(RunMode mode = RunMode.Automatic);


        /// <summary>
        /// Set the start date of the simulation period. This must be within available data.
        /// </summary>
        /// <param name="year">Year to start simulation</param>
        /// <param name="month">Month to start simulation</param>
        /// <param name="day">Date to start simulation</param>
        void SetStartDate(int year, int month, int day);
        void SetStartDate(DateTime start);


        /// <summary>
        /// Set the end simulation date for the algorithm. This must be within available data.
        /// </summary>
        /// <param name="year">integer year to end simulation period</param>
        /// <param name="month">integer month to end simulation period</param>
        /// <param name="day">integer day to end simulation period</param>
        /// <exception cref="not found">Error thrown When date is invalid</exception>
        void SetEndDate(int year, int month, int day);
        void SetEndDate(DateTime end);


        /// <summary>
        /// Set the Transaction Models to use:
        /// </summary>
        void SetModels();


        /// <summary>
        /// Set the algorithm as initialized and locked. No more cash or security changes.
        /// </summary>
        void SetLocked();


        /// <summary>
        /// Get a list of the debug messages sent so far.
        /// </summary>
        /// <returns>List of string debug messages.</returns>
        List<string> GetDebugMessages();


        /// <summary>
        /// Set a required MarketType-symbol and resolution for the simulator to prepare
        /// </summary>
        /// <param name="securityType">MarketType Enum: Equity, Commodity, FOREX or Future</param>
        /// <param name="symbol">Symbol Representation of the MarketType, e.g. AAPL</param>
        /// <param name="resolution">Resolution of the MarketType required: MarketData, Second or Minute</param>
        /// <param name="fillDataForward">If true, returns the last available data even if none in that timeslice.</param>
        /// <param name="leverage">leverage for this security</param>
        void AddSecurity(SecurityType securityType, string symbol, Resolution resolution, bool fillDataForward, decimal leverage);


        /// <summary>
        /// Add sentiment data to your algorithm.
        /// </summary>
        /// <param name="sentimentType">Type of sentiment: estimize or stockpulse</param>
        /// <param name="symbol">String symbol of asset you're requesting</param>
        void AddSentimentData(SentimentDataType sentimentType, string symbol);


        /// <summary>
        /// Set the starting capital for the strategy
        /// </summary>
        /// <param name="startingCash">decimal starting capital, default $100,000</param>
        void SetCash(decimal startingCash);


        /// <summary>
        /// Send an order to the transaction manager.
        /// </summary>
        /// <param name="symbol">Symbol we want to purchase</param>
        /// <param name="quantity">Quantity to buy, + is long, - short.</param>
        /// <param name="type">Market, Limit or Stop Order</param>
        /// <returns>Integer Order ID.</returns>
        int Order(string symbol, int quantity, OrderType type = OrderType.Market);


        /// <summary>
        /// Liquidate your portfolio holdings:
        /// </summary>
        /// <param name="symbolToLiquidate">Specific asset to liquidate, defaults to all.</param>
        /// <returns>list of order ids</returns>
        List<int> Liquidate(string symbolToLiquidate = "");


        /// <summary>
        /// Terminate the algorithm on exiting the current event processor. 
        /// If have holdings at the end of the algorithm/day they will be liquidated at market prices.
        /// If running a series analysis this command skips the current day (and doesn't liquidate).
        /// </summary>
        /// <param name="message">Exit message</param>
        void Quit(string message = "");


        /// <summary>
        /// Set the quit flag true / false.
        /// </summary>
        /// <param name="quit">When true quits the algorithm event loop for this day</param>
        void SetQuit(bool quit);

        /// <summary>
        /// Get the quit flag state. 
        /// </summary>
        /// <returns>Boolean quit flag</returns>
        bool GetQuit();
    }

}
