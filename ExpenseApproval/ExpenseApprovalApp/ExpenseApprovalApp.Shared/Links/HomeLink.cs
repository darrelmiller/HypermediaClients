using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using ExpenseApprovalApp.Tools;
using Tavis;
using Tavis.Home;

namespace ExpenseApprovalApp.Links
{
    public class TavisLinkTypes
    {
        public const string Home = "urn:tavis:home";
        public const string Expenses = "urn:tavis:expenses";
        public const string Action = "urn:tavis:action";
        public const string Show = "urn:tavis:show";
    }


    [LinkRelationType("urn:tavis:home")]
    public class HomeLink : Link
    {
        public override HttpRequestMessage CreateRequest()
        {
            Target = new Uri("http://pecan:9090/expenseapp/");

            var request = base.CreateRequest();
            request.AttachLink(this);
            return request;
        }

        public async Task ProcessHomeLinkResponse(HttpResponseMessage response, ClientState clientState)
        {
            if (!response.HasContent()) return;

            var contentStream = await response.Content.ReadAsStreamAsync();

            ShowLink showLink = null;  // Need to find a showlink to follow

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/home+json":
                    clientState.HomeDocument = HomeDocument.Parse(contentStream, clientState.LinkFactory);
                    showLink = clientState.HomeDocument.GetResource(LinkHelper.GetLinkRelationTypeName<ShowLink>()) as ShowLink;
                    break;
            }

            if (showLink != null)
            {
                await clientState.FollowLinkAsync(showLink);
            }
        }
    }


}
