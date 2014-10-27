
using System;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ExpenseApprovalAppLogic.ViewModels;


namespace ExpenseApprovalApp
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ImagePage : Page
    {

        private ImagePageViewModel _ViewModel;


        public ImagePage()
        {
            InitializeComponent();
           
            
        }

        public ImagePageViewModel ViewModel
        {
            get { return _ViewModel; }
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ViewModel= e.Parameter as ImagePageViewModel;
            DataContext = _ViewModel;
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(_ViewModel.Image.AsRandomAccessStream());
            Image.Source = bitmap;
        }

    }
}
