using System;
using System.Net.Http;
using System.Windows;
using SwitchClient.Classic;
using SwitchClient.Hyper;

namespace WpfSwitchClient
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(String.Format("http://{0}:9090/", Environment.MachineName))
            };

            var window = new MainWindow(new SwitchViewModel(new SwitchService(client)));
            //var window = new MainWindow(new SwitchHyperViewModel(client));
            window.Show();
        }
    }
}
