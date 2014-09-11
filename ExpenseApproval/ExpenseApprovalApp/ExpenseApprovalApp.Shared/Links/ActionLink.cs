using System.Net.Http;
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
    }
}