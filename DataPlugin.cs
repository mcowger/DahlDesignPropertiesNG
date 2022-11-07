using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using IC = DahlDesignPropertiesNG.IntervalController;

namespace DahlDesignPropertiesNG
{
    [PluginDescription("My plugin description")]
    [PluginAuthor("Author")]
    [PluginName("Demo plugin")]
    public class DataPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public DataPluginSettings Settings;
        public PluginManager PluginManager { get; set; }
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);
        public string LeftMenuTitle => "Dahl Design NG";
        /// <summary>
        /// Counter that updates every loop.  Lets us keep track of where we are in the 60Hz update cycle to not run too much code in the expensive data path.
        /// </summary>
        private int UpdateCounter = 0;
        private Calcs c;


        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (!(data.GameRunning || data.GameReplay)) {
                return; //Early return if the game isn't even running or replaying.
            }
            UpdateCounter = ++UpdateCounter == 60 ? 0 : UpdateCounter; //Update the counter every call.
            foreach (int Hz in IC.HzValues) // For each possible Hz value, give it a shot
            {
                if (IC.ShouldRunUpdate(CounterVal: UpdateCounter, Hz: Hz)) //If we should be running this Hz value, get started.  Otherwise move on.
                {
                    foreach (MethodInfo m in c.IntervalMethodMapping[Hz]) { // Each method - lets extract it and execute it.
                        m.Invoke(c, null); //Invoke the method against the class, with no parameters
                    }
                }
            }
        }

        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting DahlDesignProperties NG plugin");

            // Load settings
            Settings = this.ReadCommonSettings<DataPluginSettings>("GeneralSettings", () => new DataPluginSettings());
            c = new Calcs(ref pluginManager, ref Settings); // initalize the calcs class.
        }
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("GeneralSettings", Settings);
        }

        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControl(this);
        }


    }
}