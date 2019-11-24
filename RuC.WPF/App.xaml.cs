using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace RuC.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Xceed.Wpf.Themes.Windows10.Licenser.LicenseKey = "XPT30-Y79PR-8ZP0H-WG0A";
            base.OnStartup(e);
        }
    }
}
