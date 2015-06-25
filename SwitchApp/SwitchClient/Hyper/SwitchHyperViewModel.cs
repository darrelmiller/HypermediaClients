using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SwitchClient.Hyper
{
    public class SwitchHyperViewModel : ISwitchViewModel,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly SynchronizationContext _CallingContext;

        private readonly HttpClient _client;
        private SwitchDocument _switchStateDocument = new SwitchDocument();
        

        public SwitchHyperViewModel(HttpClient client)
        {
            _CallingContext = SynchronizationContext.Current;
            _client = client;
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/switchstate+json"));
            Initialize();
        }

        private async Task Initialize()
        {
            var resp = await _client.GetAsync(SwitchDocument.SelfLink);
            UpdateState(resp);

        }  
        private async Task UpdateState(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                _switchStateDocument = SwitchDocument.Load(await httpResponseMessage.Content.ReadAsStreamAsync());

                OnPropertyChanged();
                OnPropertyChanged("CanTurnOn");
                OnPropertyChanged("CanTurnOff");
            }
        }

        public bool On
        {
            get { return _switchStateDocument.On; }
        }

        public async Task TurnOff()
        {
            var response = await _client.PostAsync(_switchStateDocument.TurnOffLink, null);
            await UpdateState(response);
        }

        public async Task TurnOn()
        {
            var response = await _client.PostAsync(_switchStateDocument.TurnOnLink, null);
            await UpdateState(response);
        }

        public bool CanTurnOn
        {
            get { return _switchStateDocument.TurnOnLink != null; }
        }

        public bool CanTurnOff
        {
            get { return _switchStateDocument.TurnOffLink != null; }
        }
     

        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                _CallingContext.Post((obj) => handler(this, new PropertyChangedEventArgs(propertyName)),null);
                
            }
        }

    }
}
