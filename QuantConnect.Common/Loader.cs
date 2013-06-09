/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals, V0.1
 * Safe DLL Loader -
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

//QuantConnect Project Libraries:
using QuantConnect.Logging;

namespace QuantConnect {

    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Loader : MarshalByRefObject {

        /******************************************************** 
        * CLASS VARIABLES
        *********************************************************/
        public AppDomain appDomain;


        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Creates a new instance of the class library in a new AppDomain, safely.
        /// </summary>
        /// <returns>bool success</returns>        
        public bool CreateInstance<T>(string assemblyPath, string baseTypeName, out T algorithmInstance) {

            //Default initialisation of Assembly.
            algorithmInstance = default(T);

            //First most basic check:
            if (!File.Exists(assemblyPath)) {
                return false;
            }

            //Create a new app domain with a generic name.
            CreateAppDomain();

            try {

                //Load the assembly:
                Assembly assembly = Assembly.LoadFrom(assemblyPath);

                //Get all the classes in library
                Type[] aTypes = assembly.GetTypes();

                //Get the list of extention classes in the library: 
                List<string> lTypes = GetExtendedTypeNames(assembly, baseTypeName);

                //No extensions, nothing to load.
                if (lTypes.Count == 0 || lTypes.Count > 1) {
                    return false;
                } else {
                    algorithmInstance = (T)assembly.CreateInstance(lTypes[0], true);
                }
            } catch (ReflectionTypeLoadException err) {
                Log.Error("QC.Loader.CreateInstance(): " + err.Message);
            } catch (Exception err) {
                Log.Error("QC.Loader.CreateInstance(): " + err.Message);
            }

            //Successful load.
            if (algorithmInstance != null) {
                return true;
            } else {
                return false;
            }
        }




        /// <summary>
        /// Get a list of all the matching type names in this DLL assembly:
        /// </summary>
        /// <typeparam name="T">type/interface we're searching for </typeparam>
        /// <param name="assembly">Assembly dll we're loading.</param>
        /// <param name="baseClassName">Class to instantiate in the library</param>
        /// <returns>String list of types available.</returns>
        public static List<string> GetExtendedTypeNames(Assembly assembly, string baseClassName) {
            return (from t in assembly.GetTypes()
                    where t.BaseType.Name == baseClassName && t.Name != baseClassName && t.GetConstructor(Type.EmptyTypes) != null
                    select t.FullName).ToList();
        }


        /// <summary>
        /// Create a safe application domain with a random name.
        /// </summary>
        /// <param name="appDomainName">Set the name if required</param>
        /// <returns>True on successful creation.</returns>
        private void CreateAppDomain(string appDomainName = "") {

            //Create new domain name if not supplied:
            if (string.IsNullOrEmpty(appDomainName)) {
                appDomainName = "qclibrary" + Guid.NewGuid().ToString().GetHashCode().ToString("x");
            }

            //Setup the new domain
            AppDomainSetup domainSetup = new AppDomainSetup();

            //Create the domain:
            appDomain = AppDomain.CreateDomain(appDomainName, null, domainSetup);
        }

            

        /// <summary>
        /// Unload this factory's appDomain.
        /// </summary>
        public void Unload() {
            if (this.appDomain != null) {
                AppDomain.Unload(this.appDomain);
                this.appDomain = null;
            }
        }

    } // End Algorithm Factory Class

} // End QC Namespace.
