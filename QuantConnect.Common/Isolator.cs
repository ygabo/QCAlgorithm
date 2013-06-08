/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals, V0.1
 * Isolate memory and time usage of algorithm
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace QuantConnect {

    /******************************************************** 
    * QUANTCONNECT PROJECT LIBRARY
    *********************************************************/
    using QuantConnect.Logging;

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    public class Isolator {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
           
        /// <summary>
        /// Create a MD5 Hash of a string.
        /// </summary>
        public static string MD5(string stringToHash) {

            string hash = "";
                
            try {
                MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(stringToHash);
                byte[] hashArray = md5.ComputeHash(inputBytes);

                // step 2, convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashArray.Length; i++) {
                    sb.Append(hashArray[i].ToString("X2"));
                }
                hash = sb.ToString();
            } catch (Exception err) {
                Log.Error("QC.Security.MD5(): Error creating MD5: " + err.Message);
            }

            return hash;
        }



        /// <summary>
        /// Execute a code block with a maximum timeout.
        /// </summary>
        /// <param name="timeSpan">Timeout.</param>
        /// <param name="codeBlock">Code to execute</param>
        /// <returns>True if successful, False if Cancelled.</returns>
        public static bool ExecuteWithTimeLimit(TimeSpan timeSpan, Action codeBlock) {
            try {
                long memoryCap = 500 * 1024 * 1024;
                DateTime dtEnd = DateTime.Now + timeSpan;
                Task task = Task.Factory.StartNew(() => codeBlock());

                while (!task.IsCompleted && DateTime.Now < dtEnd) {
                    if (GC.GetTotalMemory(false) > memoryCap) {
                        Log.Error("Security.ExecuteWithTimeLimit(): Memory maxed out: " + GC.GetTotalMemory(false).ToString());
                        break;
                    }
                }

                if (task.IsCompleted == false) {
                    Log.Error("Security.ExecuteWithTimeLimit(): Operation timed out");
                }

                return task.IsCompleted;
            } catch (AggregateException ae) {
                throw ae.InnerExceptions[0];
            }
        }

    }
}
