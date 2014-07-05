using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SwitchClient.Hyper
{
    public class SwitchDocument
    {


        public static SwitchDocument Load(Stream stream)
        {
            var switchStateDocument = new SwitchDocument();
            var jObject = JObject.Load(new JsonTextReader(new StreamReader(stream)));
            foreach (var jProp in jObject.Properties())
            {
                switch (jProp.Name)
                {
                    case "On":
                        switchStateDocument.On = (bool)jProp.Value;
                        break;
                    case "TurnOnLink":
                        switchStateDocument.TurnOnLink = new Uri((string)jProp.Value, UriKind.RelativeOrAbsolute);
                        break;
                    case "TurnOffLink":
                        switchStateDocument.TurnOffLink = new Uri((string)jProp.Value, UriKind.RelativeOrAbsolute);
                        break;
                }
            }
            return switchStateDocument;
        }

        public bool On { get; private set; }
        public Uri TurnOnLink { get; set; }
        public Uri TurnOffLink { get; set; }

        public static Uri SelfLink
        {
            get { return new Uri("switch/state", UriKind.Relative); }
        }

    }
}
