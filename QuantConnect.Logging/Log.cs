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
using System.IO;
using System.Reflection;
using System.Collections;

namespace QuantConnect.Logging {


    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    public class Log {
        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        private static object lockFile = new Object();
        private static string lastTraceText = "";
        private static string lastErrorText = "";
        private static string dateFormat = "yyyyMMdd HH:mm:ss";
        private static bool _debuggingEnabled = false;
        private static int _level = 1;
        
        /// <summary>
        /// Check if this is running on a LINUX OS
        /// </summary>
        private static bool IsLinux {
            get {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        /// <summary>
        /// Global flag whether to enable debugging logging:
        /// </summary>
        public static bool DebuggingEnabled {
            get {
                return _debuggingEnabled;
            }
            set {
                _debuggingEnabled = value;
            }
        }


        /// <summary>
        /// Set the minimum message level:
        /// </summary>
        public static int DebuggingLevel {
            get {
                return _level;
            }
            set {
                _level = value;
            }
        }

        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Log an Error
        /// </summary>
        /// <param name="error">String Error</param>
        public static void Error(string error) {
            try {
                Console.WriteLine(DateTime.Now.ToString(dateFormat) + " Error:: " + error);
            } catch (Exception err) {
                Console.WriteLine("Log.Error(): Error writing error: " + err.Message);
            }
        }


        /// <summary>
        /// Log a Trace Message
        /// </summary>
        /// <param name="traceText">String Trace</param>
        public static void Trace(string traceText) {
            try {
                Console.WriteLine(DateTime.Now.ToString(dateFormat) + " Trace:: " + traceText);
            } catch (Exception err) {
                Console.WriteLine("Log.Trace(): Error writing trace: "  +err.Message);
            }
        }



        /// <summary>
        /// Output to the console, and sleep the thread for a little period to monitor the results.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delay"></param>
        public static void Debug(string text, int level = 1, int delay = 50) {
            if (_debuggingEnabled && level >= _level) {
                Console.WriteLine(DateTime.Now.ToString(dateFormat) + " DEBUGGING :: " + text);
                System.Threading.Thread.Sleep(delay);
            }
        }




        /// <summary>
        /// C# Equivalent of Print_r in PHP:
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="recursion"></param>
        /// <returns></returns>
        public static string VarDump(object obj, int recursion = 0) {
            StringBuilder result = new StringBuilder();

            // Protect the method against endless recursion
            if (recursion < 5) {
                // Determine object type
                Type t = obj.GetType();

                // Get array with properties for this object
                PropertyInfo[] properties = t.GetProperties();

                foreach (PropertyInfo property in properties) {
                    try {
                        // Get the property value
                        object value = property.GetValue(obj, null);

                        // Create indenting string to put in front of properties of a deeper level
                        // We'll need this when we display the property name and value
                        string indent = String.Empty;
                        string spaces = "|   ";
                        string trail = "|...";

                        if (recursion > 0) {
                            indent = new StringBuilder(trail).Insert(0, spaces, recursion - 1).ToString();
                        }

                        if (value != null) {
                            // If the value is a string, add quotation marks
                            string displayValue = value.ToString();
                            if (value is string) displayValue = String.Concat('"', displayValue, '"');

                            // Add property name and value to return string
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, displayValue);

                            try {
                                if (!(value is ICollection)) {
                                    // Call var_dump() again to list child properties
                                    // This throws an exception if the current property value
                                    // is of an unsupported type (eg. it has not properties)
                                    result.Append(Log.VarDump(value, recursion + 1));
                                } else {
                                    // 2009-07-29: added support for collections
                                    // The value is a collection (eg. it's an arraylist or generic list)
                                    // so loop through its elements and dump their properties
                                    int elementCount = 0;
                                    foreach (object element in ((ICollection)value)) {
                                        string elementName = String.Format("{0}[{1}]", property.Name, elementCount);
                                        indent = new StringBuilder(trail).Insert(0, spaces, recursion).ToString();

                                        // Display the collection element name and type
                                        result.AppendFormat("{0}{1} = {2}\n", indent, elementName, element.ToString());

                                        // Display the child properties
                                        result.Append(Log.VarDump(element, recursion + 2));
                                        elementCount++;
                                    }

                                    result.Append(Log.VarDump(value, recursion + 1));
                                }
                            } catch { }
                        } else {
                            // Add empty (null) property to return string
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, "null");
                        }
                    } catch {
                        // Some properties will throw an exception on property.GetValue()
                        // I don't know exactly why this happens, so for now i will ignore them...
                    }
                }
            }

            return result.ToString();
        }
    }
}
