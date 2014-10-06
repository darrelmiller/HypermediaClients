using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using SwitchClient;
using SwitchClient.Classic;
using SwitchClient.Hyper;


namespace WPSwitchApp
{
    public sealed partial class MainPage : Page
    {
        private ISwitchViewModel _model;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        

        async void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!Dispatcher.HasThreadAccess)
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RefreshLight);

            }
            //else
            //{
            //    RefreshLight();
            //}

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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _model = (ISwitchViewModel)e.Parameter;
            _model.PropertyChanged += _model_PropertyChanged;

            DataContext = _model;
            RefreshLight();
            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
