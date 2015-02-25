using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ExpenseApprovalAppLogic.Tools;

namespace ExpenseApprovalAppLogic.ViewModels
{
    public class ImagePageViewModel : INotifyPropertyChanged
    {
        private readonly ExpenseAppClientState _clientState;
        private Stream _image;


        public IDelegateCommand BackCommand { protected set; get; }
        public event PropertyChangedEventHandler PropertyChanged;
     
        public ImagePageViewModel(ExpenseAppClientState clientState)
        {
            _clientState = clientState;
            _clientState.PropertyChanged += _clientState_PropertyChanged;
            Image = _clientState.CurrentImage;
            BackCommand = new DelegateCommand(ExecuteBack);
        }

        public Stream Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        void _clientState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentImage":
                    Image = _clientState.CurrentImage;
                    break;
            }
        }

        private async Task ExecuteBack(object param)
        {
            await _clientState.BackAsync();
        }
      
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
