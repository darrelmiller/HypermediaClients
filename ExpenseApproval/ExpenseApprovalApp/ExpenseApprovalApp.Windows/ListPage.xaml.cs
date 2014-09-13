using System.Dynamic;
using ExpenseApprovalApp.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ExpenseApprovalApp.Links;
using ExpenseApprovalApp.ViewModels;

namespace ExpenseApprovalApp
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ListPage : Page
    {
        private ListPageViewModel _viewModel;
        private ClientState _clientState;


        public ListPageViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public ListPage()
        {
            this.InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _clientState = e.Parameter as ClientState;
            _clientState.PropertyChanged += _clientState_PropertyChanged;
            _viewModel = new ListPageViewModel(_clientState);
            DataContext = _viewModel;
        }

        void _clientState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCollection")
            {
                _viewModel.RefreshItems();
            }
        }
 
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _clientState.FollowLinkAsync(new HomeLink());
        }
    }
}
