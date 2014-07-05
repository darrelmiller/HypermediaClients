using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using SwitchClient;
using SwitchClient.Hyper;

namespace WpfSwitchClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SwitchViewModel _model;


        public MainWindow(HttpClient client) : this()
        {
            _model = new SwitchViewModel(new SwitchService(client));
            //_model = new SwitchHyperViewModel(client);
            _model.PropertyChanged += _model_PropertyChanged;

            DataContext = _model;
            RefreshLight();
        }

        public MainWindow()
        {
            InitializeComponent();
        }
        void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action) RefreshLight);
        }

        private void RefreshLight()
        {
            Light.Fill = _model.On ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Gray);
        }

        private void TurnOffClick(object sender, RoutedEventArgs e)
        {
            _model.TurnOff();
        }

        private void TurnOnClick(object sender, RoutedEventArgs e)
        {
            _model.TurnOn();
        }

        
    }
}
