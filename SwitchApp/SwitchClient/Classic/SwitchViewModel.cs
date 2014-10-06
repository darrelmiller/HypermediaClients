using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SwitchClient.Classic
{
    public class SwitchViewModel : ISwitchViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private readonly SwitchService _service;
        private bool _switchState;

        public SwitchViewModel(SwitchService service)
        {
            _service = service;
            _switchState = service.GetSwitchStateAsync().Result;
        }


        private async Task SetSwitchStateAsync(bool value ) {
                await _service.SetSwitchStateAsync(value);
                _switchState = value; 
                OnPropertyChanged();
                OnPropertyChanged("CanTurnOn");
                OnPropertyChanged("CanTurnOff");
        }

        public bool On
        {
            get { return _switchState; }
        }

        public Task TurnOff()
        {
            return SetSwitchStateAsync(false);
        }

        public Task TurnOn()
        {
            return SetSwitchStateAsync(true);
        }


        public bool CanTurnOn
        {
            get { return _switchState == false; }
        }

        public bool CanTurnOff
        {
            get { return _switchState; }
        }
        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
