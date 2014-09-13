
using Windows.UI.Xaml.Controls;

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
            DataContext = ViewModel;
        }

        public ImagePageViewModel ViewModel
        {
            get { return _ViewModel; }
            set { _ViewModel = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var clientState = e.Parameter as ClientState;
            ViewModel.LoadImage(clientState);
        }

    }
}
