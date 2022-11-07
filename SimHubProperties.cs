using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DahlDesignPropertiesNG
{
    internal class SimHubProperties
    {
        private readonly PluginManager pluginManager;
        private readonly Type type;
        public SimHubProperties(ref PluginManager pluginManager, Type type)
        {
            this.pluginManager = pluginManager;
            this.type = type;
        }
        /// <summary>
        /// Simplified method for adding properties
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value">Optional intitial value</param>
        /// <param name="description">Optional description</param>
        public void AddProperty(string Name, object Value = null, string description = null)
        {
            pluginManager.AddProperty(Name, type, Value, description);
        }
        /// <summary>
        /// Simplified method for updating properties
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value">New value</param>
        public void SetPropertyValue(string Name, object Value)
        {
            pluginManager.SetPropertyValue(Name, type, Value);
        }
    }
}
