using Windows.UI.Xaml.Media.Imaging;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using ExpenseApprovalApp.ViewModels;

namespace ExpenseApprovalApp
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ImagePage : Page
    {

        private ImagePageViewModel _ViewModel = new ImagePageViewModel();


        public ImagePage()
        {
            this.InitializeComponent();
            DataContext = _ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var clientState = e.Parameter as ClientState;
            _ViewModel.LoadImage(clientState.CurrentImage);
        }

    }
}
