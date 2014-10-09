using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SwitchClient;


namespace WPSwitchApp
{
    public sealed partial class MainPage : Page
    {
        private ISwitchViewModel _model;

        public MainPage()
        {
            this.InitializeComponent();

        }
        
        async void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!Dispatcher.HasThreadAccess)
            {
                // This is not refreshing properly on device.
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RefreshLight);

            }
            else
            {
                RefreshLight();  
            }

        }

        private void RefreshLight()
        {
            Light.Fill = _model.On ? new SolidColorBrush(Windows.UI.Colors.Red) : new SolidColorBrush(Windows.UI.Colors.Gray);
        }

        private void TurnOffClick(object sender, RoutedEventArgs e)
        {
            _model.TurnOff();
        }

        private void TurnOnClick(object sender, RoutedEventArgs e)
        {
            _model.TurnOn();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _model = (ISwitchViewModel)e.Parameter;
            _model.PropertyChanged += _model_PropertyChanged;

            DataContext = _model;
            RefreshLight();
           
        }
    }
}
