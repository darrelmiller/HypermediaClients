using System.IO;
using CollectionJson;
using Newtonsoft.Json;

namespace ExpenseApprovalAppLogic.Tools
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
