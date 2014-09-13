using System.Net.Http;
using System.Threading.Tasks;
using ExpenseApprovalApp.Tools;
using Tavis;

namespace ExpenseApprovalApp.Links
{
    [LinkRelationType("urn:tavis:action")]
    public class ActionLink : Link
    {
       

        public override HttpRequestMessage CreateRequest()
        {
            Method = HttpMethod.Post;
            var request = base.CreateRequest();
            request.AttachLink(this);
            return request;
        }


        public async Task ProcessActionLinkResponse(HttpResponseMessage response, ClientState clientState)
        {
            if (!response.HasContent()) return;

            var contentStream = await response.Content.ReadAsStreamAsync();

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/vnd.collection+json":
                    clientState.CurrentCollection = CollectionJsonHelper.ParseCollectionJson(contentStream);
                    break;
            }
        }
    }
}