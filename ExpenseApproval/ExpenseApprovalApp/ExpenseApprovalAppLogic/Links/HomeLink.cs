using System;
using System.Net.Http;
using System.Threading.Tasks;
using ExpenseApprovalApp;
using ExpenseApprovalAppLogic.Tools;
using Tavis;
using Tavis.Home;

namespace ExpenseApprovalAppLogic.Links
{
    public class TavisLinkTypes
    {
        public const string Home = "urn:tavis:home";
        public const string Expenses = "urn:tavis:expenses";
        public const string Action = "urn:tavis:action";
        public const string Show = "urn:tavis:show";
    }



    /// <summary>
    /// In this application a HomeLink is designed to retreive an application/home+json document
    /// and look for a ShowLink in it and follow that ShowLink.
    /// </summary>
    /// <remarks> Home document could also contain links to the user's preferences, an Authentication Server,
    /// help pages, etc.
    /// </remarks>
    [LinkRelationType("urn:tavis:home")]
    public class HomeLink : BaseLink

    {
        public HomeLink()
        {
            this.AddRequestBuilder((request) =>
            {
                request.RequestUri = new Uri("http://pecan:9090/expenseapp/");
                request.AttachLink(this);
                return request;
            });
            KeepInHistory = false;
        }
       

        public async Task ProcessHomeLinkResponse(HttpResponseMessage response, ExpenseAppClientState clientState)
        {
            if (!response.HasContent()) return;

            var contentStream = await response.Content.ReadAsStreamAsync();

            ShowLink showLink = null;  // Need to find a showlink to follow

            switch (response.Content.Headers.ContentType.MediaType)
            {
                // Currently only support application/home+json
                case "application/home+json":
                    clientState.HomeDocument = HomeDocument.Parse(contentStream, clientState.LinkFactory);
                    showLink = clientState.HomeDocument.GetResource(LinkHelper.GetLinkRelationTypeName<ShowLink>()) as ShowLink;
                    break;
                
                // Add more media types here as desired.  WADL, SWAGGER, API Blueprint, WSDL, etc
            }

            if (showLink != null)
            {
                await clientState.FollowLinkAsync(showLink);
            }
        }
    }


}
