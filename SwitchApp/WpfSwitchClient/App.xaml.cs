using System;
using System.Net.Http;
using System.Windows;

namespace WpfSwitchClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(String.Format("http://{0}:9090/", Environment.MachineName))
            };

            var window = new MainWindow(client);
            window.Show();
        }
    }
}
