using GameReaderCommon;
using IRacingReader;
using SimHub.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DahlDesignPropertiesNG
{
    sealed class SizedList<T> : List<T>
    {
        public int FixedCapacity { get; }
        public SizedList(int fixedCapacity)
        {
            this.FixedCapacity = fixedCapacity;
        }

        /// <summary>
        /// If the total number of item exceed the capacity, the oldest ones automatically gets removed..
        /// </summary>
        public new void Add(T item)
        {
            base.Add(item);
            if (base.Count > FixedCapacity)
            {
                base.RemoveAt(0);
            }
        }
    }
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
        private GameData data;
        private DataSampleEx irData;
        private SimHubProperties s;
        private readonly SizedList<GameData> PrevData = new SizedList<GameData>(10); // We can save 10 previous versions of gamedata.
        
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
        public Calcs(ref PluginManager pluginManager, ref DataPluginSettings Settings, ref SimHubProperties Props)
        {
            this.pluginManager = pluginManager;
            settings = Settings;
            s = Props;
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
        public void UpdateGameData(ref GameData d)
        {
            data = d;
            PrevData.Add(data); //save the old game data.
            if (data?.NewData?.GetRawDataObject() is DataSampleEx)
            {
                irData = data.NewData.GetRawDataObject() as DataSampleEx;
            }
        }

        [TargetHz(1)]
        public void SettingsUpdate()
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
        [TargetHz(1)]
        public void TireAttributesUpdate()
        {
            var LFCold = irData.Telemetry.LFcoldPressure;
            var RFCold = irData.Telemetry.RFcoldPressure;
            var LRCold = irData.Telemetry.LRcoldPressure;
            var RRCold = irData.Telemetry.RRcoldPressure;

            s.SetPropertyValue("PitServiceLFPCold", LFCold);
            s.SetPropertyValue("PitServiceRFPCold", RFCold);
            s.SetPropertyValue("PitServiceLRPCold", LRCold);
            s.SetPropertyValue("PitServiceRRPCold", RRCold);
        }
        [TargetHz(60)]
        public void SmoothGear()
        {
            //----------------------------------------------
            //--------SMOOTH GEAR---------------------------
            //----------------------------------------------
            // Example of how to look back to compare values over time.
            var Gear = data.NewData.Gear;
            
            if (PrevData.Count < 6)
            {
                s.SetPropertyValue("SmoothGear", Gear); // If we dont have 6 cycles of data, just return what we have
            }
            else
            {
                s.SetPropertyValue("SmoothGear", PrevData[5].NewData.Gear); // otherwise, we delay this for 6 cycles.
            }

        }
    }
}
