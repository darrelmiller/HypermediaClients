using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TodoApp
{
    public class ClientState
    {
        private HttpClient _httpClient;

        public ClientState(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
        }
        
    }
}

      
      