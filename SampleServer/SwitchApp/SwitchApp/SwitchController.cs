using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace HypermediaAppServer.SwitchApp
{
    public class SwitchController : ApiController
    {
        public static bool SwitchState { get; set; }

     
        public HttpResponseMessage Get()
        {
            var response = CreateSwitchStateResponse();
            response.Headers.CacheControl = new CacheControlHeaderValue(){MaxAge = new TimeSpan(0,0,2)};
   
            return response;
        }

        private HttpResponseMessage CreateSwitchStateResponse()
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

            dynamic jObject = new JObject();
            jObject.On = SwitchState.ToString();

            if (SwitchState && CanTurnOff())
            {
                jObject.TurnOffLink = "switch/off";
            }
            else
            {
                if (CanTurnOn())
                {
                    jObject.TurnOnLink = "switch/on";
                }
            }

            var content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/switchstate+json");
            return content;
        }

        [ActionName("on")]
        public HttpResponseMessage PostOn()
        {
            if (SwitchState == true || !CanTurnOn()) 
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

            SwitchState = true;
            return CreateSwitchStateResponse();
        }

        [ActionName("off")]
        public HttpResponseMessage PostOff()
        {
            if (SwitchState == false || !CanTurnOff()) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            SwitchState = false;
            return CreateSwitchStateResponse();
        }

        // Permissions, Timed limits, External State
        private static bool CanTurnOn()
        {
            return true;  
        }
        private static bool CanTurnOff()
        {
            return true;
        }
 
    }
}