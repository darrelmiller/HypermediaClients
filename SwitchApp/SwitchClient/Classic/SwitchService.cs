using System.Net.Http;
using System.Threading.Tasks;

namespace SwitchClient.Classic
{
    public class SwitchService
    {
        private const string SwitchStateResource = "/switch";
        private const string SwitchOnResource = "/switch/on";
        private const string SwitchOffResource = "/switch/off";

        private readonly HttpClient _client;

        public SwitchService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> GetSwitchStateAsync()
        {
            var result = await _client.GetStringAsync(SwitchStateResource).ConfigureAwait(false);

            return bool.Parse(result);
        }

        public Task SetSwitchStateAsync(bool newstate)
        {
            if (newstate)
            {
                return _client.PostAsync(SwitchOnResource,null);
            }
            else
            {
                return _client.PostAsync(SwitchOffResource, null);
            }
        }
    }
}
