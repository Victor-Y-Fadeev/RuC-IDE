using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Globalization;

namespace RuC.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static List<CultureInfo> m_Languages = new List<CultureInfo>();

		public static List<CultureInfo> Languages
		{
			get
			{
				return m_Languages;
			}
		}

		public App()
		{
			InitializeComponent();
			App.LanguageChanged += App_LanguageChanged;

			m_Languages.Clear();
			m_Languages.Add(new CultureInfo("en-US"));
			m_Languages.Add(new CultureInfo("ru-RU"));

			Language = RuC.WPF.Properties.Settings.Default.DefaultLanguage;
		}

		public static event EventHandler LanguageChanged;

		public static CultureInfo Language
		{
			get
			{
				return System.Threading.Thread.CurrentThread.CurrentUICulture;
			}
			set
			{
				if (value == null) throw new ArgumentNullException("value");
				if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

				System.Threading.Thread.CurrentThread.CurrentUICulture = value;

				ResourceDictionary dict = new ResourceDictionary();
				switch (value.Name)
				{
					case "ru-RU":
						dict.Source = new Uri(String.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
						break;
					default:
						dict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
						break;
				}

				ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
											  where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
											  select d).First();
				if (oldDict != null)
				{
					int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
					Application.Current.Resources.MergedDictionaries.Remove(oldDict);
					Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
				}
				else
				{
					Application.Current.Resources.MergedDictionaries.Add(dict);
				}

				LanguageChanged(Application.Current, new EventArgs());
			}
		}

		private void App_LanguageChanged(Object sender, EventArgs e)
		{
			RuC.WPF.Properties.Settings.Default.DefaultLanguage = Language;
			RuC.WPF.Properties.Settings.Default.Save();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			Xceed.Wpf.Toolkit.Licenser.LicenseKey = "WTK38-P00GR-RHFGW-0GSA";
			Xceed.Wpf.Themes.Windows10.Licenser.LicenseKey = "XPT30-Y79PR-8ZP0H-WG0A";
			base.OnStartup(e);
		}
	}
}
