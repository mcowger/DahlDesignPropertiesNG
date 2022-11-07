using System.Windows.Controls;

namespace DahlDesignPropertiesNG
{
    /// <summary>
    /// Logique d'interaction pour SettingsControlDemo.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public DahlDesignPlugin Plugin { get; }

        public SettingsControl()
        {
            InitializeComponent();
        }

        public SettingsControl(DahlDesignPlugin plugin) : this()
        {
            this.Plugin = plugin;
        }


    }
}
