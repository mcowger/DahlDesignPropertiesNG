using System.Windows.Controls;

namespace DahlDesignPropertiesNG
{
    /// <summary>
    /// Logique d'interaction pour SettingsControlDemo.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public DataPlugin Plugin { get; }

        public SettingsControl()
        {
            InitializeComponent();
        }

        public SettingsControl(DataPlugin plugin) : this()
        {
            this.Plugin = plugin;
        }


    }
}
