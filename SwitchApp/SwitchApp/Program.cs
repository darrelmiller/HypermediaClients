using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Newtonsoft.Json.Linq;

namespace SwitchApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverUrl = new Uri("http://localhost:9090");
            var config = new HttpSelfHostConfiguration(serverUrl);
            ConfigureApi(config);

            var server = new HttpSelfHostServer(config);

            server.OpenAsync().Wait();


            Console.WriteLine("Server listening on " + serverUrl.AbsoluteUri);
            Console.ReadLine();



            server.CloseAsync().Wait();
        }

     

        private static void ConfigureApi(HttpSelfHostConfiguration config)
        {
           config.Routes.MapHttpRoute("","{controller}/{action}");
        }
    }


    public class SwitchController : ApiController
    {
        public static bool SwitchState { get; set; }

        [ActionName("state")]
        public HttpResponseMessage Get()
        {

            HttpContent content;
            if (Request.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/switchstate+json")))
            {
                content = CreateSwitchContent();
            }
            else
            {
                content = new StringContent(SwitchState.ToString());
            }

            return new HttpResponseMessage()
            {
                Content = content
            };
        }

        private static HttpContent CreateSwitchContent()
        {
            HttpContent content;
            dynamic jObject = new JObject();
            jObject.On = SwitchState.ToString();
            if (SwitchState)
            {
                jObject.TurnOffLink = "switch/off";
            }
            else
            {
                jObject.TurnOnLink = "switch/on";    
            }
            
            
            content = new StringContent(jObject.ToString());
            return content;
        }

        [ActionName("on")]
        public HttpResponseMessage PostOn()
        {
            if (SwitchState == true) return new HttpResponseMessage(HttpStatusCode.BadRequest);
            SwitchState = true;
            Console.WriteLine("Switch is On");
            return new HttpResponseMessage()
            {
                Content = CreateSwitchContent()
            };
        }

        [ActionName("off")]
        public HttpResponseMessage PostOff()
        {
            if (SwitchState == false) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            SwitchState = false;
            Console.WriteLine("Switch is Off");
            return new HttpResponseMessage()
            {
                Content = CreateSwitchContent()
            };
        }        
    
    }
}
