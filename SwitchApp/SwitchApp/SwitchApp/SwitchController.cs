using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace HypermediaAppServer.SwitchApp
{
    public class SwitchController : ApiController
    {
        public static bool SwitchState { get; set; }

     
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
                //if (!TurtleSeason())
                //{
                    jObject.TurnOnLink = "switch/on";
                //}
            }
            
            
            content = new StringContent(jObject.ToString());
            return content;
        }

        private static bool TurtleSeason()
        {
            return true;
        }

        [ActionName("on")]
        public HttpResponseMessage PostOn()
        {
            if (SwitchState == true
               // || TurtleSeason()
                ) return new HttpResponseMessage(HttpStatusCode.BadRequest);
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