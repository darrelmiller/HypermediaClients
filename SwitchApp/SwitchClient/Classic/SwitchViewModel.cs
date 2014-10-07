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

        public bool On
        {
            get { return _switchState; }
        }

        public async Task TurnOff()
        {
            
            await _service.SetSwitchStateAsync(false);

            _switchState = false;

            OnPropertyChanged();
            OnPropertyChanged("CanTurnOn");
            OnPropertyChanged("CanTurnOff");
        }

        public async Task TurnOn()
        {
            await _service.SetSwitchStateAsync(true);

            _switchState = true;

            OnPropertyChanged();
            OnPropertyChanged("CanTurnOn");
            OnPropertyChanged("CanTurnOff");
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
