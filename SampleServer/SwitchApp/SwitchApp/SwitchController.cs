using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Tavis.IANA;

namespace HypermediaAppServer.SwitchApp
{
    public class SwitchController : ApiController
    {
        public static bool SwitchState { get; set; }

     
        public HttpResponseMessage Get()
        {

            var response  =CreateSwitchStateResponse();
            response.Headers.CacheControl = new CacheControlHeaderValue(){MaxAge = new TimeSpan(0,0,2)};
            response.Headers.Add("Tavis-timestamp",DateTime.UtcNow.ToString());
            return response;
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
               // if (!TurtleSeason())
               //{
                    jObject.TurnOnLink = "switch/on";
               //}
            }
            
            
            content = new StringContent(jObject.ToString(), Encoding.UTF8,"application/switchstate+json");
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
           //     || TurtleSeason()
                ) return new HttpResponseMessage(HttpStatusCode.BadRequest);
            SwitchState = true;
            return CreateSwitchStateResponse();
        }

        [ActionName("off")]
        public HttpResponseMessage PostOff()
        {
            if (SwitchState == false) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            SwitchState = false;

            return CreateSwitchStateResponse();
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
    }
}