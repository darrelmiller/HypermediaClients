using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ExpenseApprovalAppLogic.Tools;
using Tavis;

namespace ExpenseApprovalAppLogic.Links
{
    [LinkRelationType("urn:tavis:show")]
    public class ShowLink : BaseLink
    {
        public ShowLink()
        {
            AddRequestBuilder(new InlineRequestBuilder((r) => { r.AttachLink(this);
                                                                  return r;
            }));
            KeepInHistory = true;
        }
        

        public async Task ProcessShowLinkResponse(HttpResponseMessage response, ExpenseAppClientState clientState)
        {
            if (!response.HasContent() && response.Content.Headers.ContentType != null) return;  // If we don't know the content-type, we can't show it


            var contentStream = await response.Content.ReadAsStreamAsync();

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/vnd.collection+json":
                    clientState.CurrentCollection = CollectionJsonHelper.ParseCollectionJson(contentStream);
                    break;
                case "image/jpeg":
                      clientState.CurrentImage = contentStream;
                    break;

                case "image/tiff":

                    break;
                case "application/pdf":
                    clientState.CurrentFile = contentStream;
       
                    break;
            }

          

        }
    }
}