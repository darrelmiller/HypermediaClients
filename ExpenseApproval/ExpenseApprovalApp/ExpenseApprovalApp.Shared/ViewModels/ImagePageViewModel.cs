using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ExpenseApprovalApp.Tools;

namespace ExpenseApprovalApp.ViewModels
{
    public class ImagePageViewModel : INotifyPropertyChanged
    {
        private BitmapImage _image;
        private ClientState _ClientState;
        public IDelegateCommand BackCommand { protected set; get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value; 
                OnPropertyChanged();
            }
        }

        public ImagePageViewModel()
        {
            BackCommand = new DelegateCommand(ExecuteBack);
        }


        private async Task ExecuteBack(object param)
        {
            await _ClientState.BackAsync();
        }

        internal void LoadImage(ClientState clientState)
        {
            _ClientState = clientState;
            Image = clientState.CurrentImage;
        }

        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
