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
using System.Threading;
using System.Threading.Tasks;

//QuantConnect Project Libraries:
using QuantConnect.Logging;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    public static class Extensions {
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
        /// List Extension Method: Move one element from A to B.
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="list">List we're operating on.</param>
        /// <param name="oldIndex">Index of variable we want to move.</param>
        /// <param name="newIndex">New location for the variable</param>
        public static void Move<T>(this List<T> list, int oldIndex, int newIndex) {
            T oItem = list[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex) newIndex--;
            list.Insert(newIndex, oItem);
        }


        /// <summary>
        /// Convert a string into a Byte Array
        /// </summary>
        /// <param name="str">String to Convert to Bytes.</param>
        /// <returns>Byte Array</returns>
        public static byte[] GetBytes(this string str) {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }



        /// <summary>
        /// Convert a byte array into a string.
        /// </summary>
        /// <param name="bytes">byte array to convert.</param>
        /// <returns>String from Bytes.</returns>
        public static string GetString(this byte[] bytes) {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }


        /// <summary>
        /// Round a DateTime to nearest Timespan Period.
        /// </summary>
        /// <param name="time">TimeSpan To Round</param>
        /// <param name="roundingInterval">Rounding Unit</param>
        /// <param name="roundingType">Rounding method</param>
        /// <returns>Rounded timespan</returns>
        public static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval, MidpointRounding roundingType) {
            return new TimeSpan(
                Convert.ToInt64(System.Math.Round(
                    time.Ticks / (decimal)roundingInterval.Ticks,
                    roundingType
                )) * roundingInterval.Ticks
            );
        }

        /// <summary>
        /// Default Timespan Rounding 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="roundingInterval"></param>
        /// <returns></returns>
        public static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval) {
            return Round(time, roundingInterval, MidpointRounding.ToEven);
        }


        /// <summary>
        /// Round a DateTime to the nearest unit.
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="roundingInterval"></param>
        /// <returns></returns>
        public static DateTime Round(this DateTime datetime, TimeSpan roundingInterval) {
            return new DateTime((datetime - DateTime.MinValue).Round(roundingInterval).Ticks);
        }


        /// <summary>
        /// Explicitly Round UP to the Nearest TimeSpan Unit.
        /// </summary>
        /// <param name="time">Base Time to Round UP</param>
        /// <param name="d">TimeSpan Unit</param>
        /// <returns>Rounded DateTime</returns>
        public static DateTime RoundUp(this DateTime time, TimeSpan d) {
            return new DateTime(((time.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }

    }
}
