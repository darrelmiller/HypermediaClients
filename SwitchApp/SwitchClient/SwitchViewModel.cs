using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SwitchClient
{
    public class SwitchViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private readonly SwitchService _service;
        private bool _switchState;

        public SwitchViewModel(SwitchService service)
        {
            _service = service;
            _switchState = service.GetSwitchStateAsync().Result;
        }

        
        private bool SwitchState {
            get
            {
                return _switchState; 
            }
             set
            {
                _service.SetSwitchStateAsync(value).Wait();
                _switchState = value; 
                OnPropertyChanged();
                OnPropertyChanged("CanTurnOn");
                OnPropertyChanged("CanTurnOff");
            }
        }

        public bool On
        {
            get { return SwitchState; }
        }

        public void TurnOff()
        {
            SwitchState = false;
        }

        public void TurnOn()
        {
            SwitchState = true;
        }


        public bool CanTurnOn
        {
            get { return SwitchState == false; }
        }

        public bool CanTurnOff
        {
            get { return SwitchState; }
        }
        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
