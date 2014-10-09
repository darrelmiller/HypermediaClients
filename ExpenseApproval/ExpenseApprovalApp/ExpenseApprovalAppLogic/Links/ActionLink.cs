using System.Net.Http;
using System.Threading.Tasks;
using ExpenseApprovalAppLogic;
using ExpenseApprovalAppLogic.Links;
using ExpenseApprovalAppLogic.Tools;
using Tavis;

namespace ExpenseApprovalApp.Links
{
    [LinkRelationType("urn:tavis:action")]
    public class ActionLink : BaseLink
    {

        public ActionLink()
        {
            this.AddRequestBuilder((request) =>
            {
                request.Method = HttpMethod.Post;

                request.AttachLink(this);
                return request;
            });
            KeepInHistory = false;
        }
       


        public async Task ProcessActionLinkResponse(HttpResponseMessage response, ExpenseAppClientState clientState)
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