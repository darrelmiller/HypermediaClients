using System.Net.Http;
using ExpenseApprovalApp.Tools;
using Tavis;

namespace ExpenseApprovalApp.Links
{
    [LinkRelationType("urn:tavis:show")]
    public class ShowLink : Link
    {
        public override HttpRequestMessage CreateRequest()
        {
            var request = base.CreateRequest();
            request.AttachLink(this);
            return request;
        }

   
    }
}