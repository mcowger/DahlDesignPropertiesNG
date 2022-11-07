using SimHub.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DahlDesignPropertiesNG
{
    /// <summary>
    /// Custom Attribute Class to allow this Calcs class to automatically contruct the list of functions to run.
    /// </summary>
    internal class TargetHzAttribute : System.Attribute
    {
        public int Hz;
        public TargetHzAttribute(int Hz)
        {
            this.Hz = Hz;
        }
    }
    /// <summary>
    /// Class to contain calculations used for the plugin.
    /// </summary>
    internal class Calcs
    {
        private readonly PluginManager pluginManager;
        private readonly DataPluginSettings settings;
        /// <summary>
        /// A threadsafe dictionary that is indexed by the rate at which to run a function (the key).
        /// The value is a list (that contains 'object' types) of each function that should run.
        /// 
        /// To add a new function, include it here, and make sure to add the custom attribute with the target run rate:
        ///         [TargetHz(1)]
        ///         public void UpdateSettings() {}
        /// </summary>
        internal readonly System.Collections.Concurrent.ConcurrentDictionary<int, List<MethodInfo>> IntervalMethodMapping = new ConcurrentDictionary<int, List<MethodInfo>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="Settings"></param>
        public Calcs(ref PluginManager pluginManager, ref DataPluginSettings Settings)
        {
            this.pluginManager = pluginManager;
            settings = Settings;
            SimHub.Logging.Current.Info("Initializing Calcs Class...");

            foreach (int Hz in IntervalController.HzValues) //Initialize the mapping of functions to when they should run.  Each one starts off empty.
            {
                IntervalMethodMapping.TryAdd(Hz, new List<MethodInfo>());
            }
            //Now go through the methods that exist in this class, grab their TargetHz attribute, and add them to the relevant list.
            foreach (MethodInfo m in typeof(Calcs).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (m.GetCustomAttributes().Any()) //Make sure there is an attribute at all
                {
                    var targetHz = m.GetCustomAttributes().First() as TargetHzAttribute;

                    IntervalMethodMapping[targetHz.Hz].Add(m); //Store this method info so we can call invoke on it later
                }
            }

            ///Print out each method with its rate of execution.
            foreach (int Hz in IntervalController.HzValues)
            {
                foreach (MethodInfo m in IntervalMethodMapping[Hz])
                {
                    SimHub.Logging.Current.Info("@ " + Hz + "Hz: " + m.Name);
                }
            }
        }
        /// <summary>
        /// Updates the Properties for various settings that are likely to be changed during the game.
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="Settings"></param>
        [TargetHz(1)]
        public void UpdateSettings()
        {
            SimHub.Logging.Current.Info("UpdateSettings Executed");
            //pluginManager.SetPropertyValue("DDUstartLED", this.GetType(), Settings.DDUstartLED);
            //pluginManager.SetPropertyValue("SW1startLED", this.GetType(), Settings.SW1startLED);
            //pluginManager.SetPropertyValue("DDUEnabled", this.GetType(), Settings.DDUEnabled);
            //pluginManager.SetPropertyValue("SW1Enabled", this.GetType(), Settings.SW1Enabled);
            //pluginManager.SetPropertyValue("DashLEDEnabled", this.GetType(), Settings.DashLEDEnabled);
            //pluginManager.SetPropertyValue("LapInfoScreen", this.GetType(), Settings.LapInfoScreen);
            //pluginManager.SetPropertyValue("ShiftTimingAssist", this.GetType(), Settings.ShiftTimingAssist);
            //pluginManager.SetPropertyValue("ShiftWarning", this.GetType(), Settings.ShiftWarning);
            //pluginManager.SetPropertyValue("ARBswapped", this.GetType(), Settings.SupercarSwapPosition);
            //pluginManager.SetPropertyValue("ARBstiffForward", this.GetType(), Settings.SupercarARBDirection);
            //pluginManager.SetPropertyValue("CoupleInCarToPit", this.GetType(), Settings.CoupleInCarToPit);
        }

    }
}
