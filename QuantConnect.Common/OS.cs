/*
* QUANTCONNECT.COM - 
* QC.OS -- Operating System Checks for Cross Platform C#
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.IO;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/

    public class OS {
        /******************************************************** 
        * CLASS PROPERTIES
        *********************************************************/
        /// <summary>
        /// Global Flag :: Operating System
        /// </summary>
        public static bool IsLinux {
            get {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        /// <summary>
        /// Global Flag :: Operating System
        /// </summary>
        public static bool IsWindows {
            get {
                return !IsLinux;
            }
        }


        /// <summary>
        /// Character Separating directories in this OS:
        /// </summary>
        public static string PathSeparation {
            get {
                return Path.DirectorySeparatorChar.ToString();
            }
        }

    } // End OS Class
} // End QC Namespace
