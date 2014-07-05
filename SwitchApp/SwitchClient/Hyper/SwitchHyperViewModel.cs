using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace SwitchClient.Hyper
{
    public class SwitchHyperViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly HttpClient _client;
        private SwitchDocument _switchStateDocument = new SwitchDocument();

        public SwitchHyperViewModel(HttpClient client)
        {
            _client = client;
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/switchstate+json"));
            _client.GetAsync(SwitchDocument.SelfLink).ContinueWith(t => UpdateState(t.Result)).Wait();
        }

        public bool On
        {
            get { return _switchStateDocument.On; }
        }

        public bool CanTurnOn
        {
            get { return _switchStateDocument.TurnOnLink != null; }
        }

        public bool CanTurnOff
        {
            get { return _switchStateDocument.TurnOffLink != null; }
        }

        public void TurnOff()
        {
            _client.PostAsync(_switchStateDocument.TurnOffLink, null).ContinueWith(t => UpdateState(t.Result));
        }

        public void TurnOn()
        {
            _client.PostAsync(_switchStateDocument.TurnOnLink, null).ContinueWith(t => UpdateState(t.Result));
        }

        private void UpdateState(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                _switchStateDocument = SwitchDocument.Load(httpResponseMessage.Content.ReadAsStreamAsync().Result);

                OnPropertyChanged();
                OnPropertyChanged("CanTurnOn");
                OnPropertyChanged("CanTurnOff");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
