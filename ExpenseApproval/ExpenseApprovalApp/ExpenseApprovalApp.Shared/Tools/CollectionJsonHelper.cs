using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using WebApiContrib.CollectionJson;

namespace ExpenseApprovalApp.Tools
{
    public static class CollectionJsonHelper
    {
        public static Collection ParseCollectionJson(Stream contentStream)
        {
            var jsons = JsonSerializer.Create();
            return jsons.Deserialize<ReadDocument>(new JsonTextReader(new StreamReader(contentStream))).Collection;
        }
    }
}
