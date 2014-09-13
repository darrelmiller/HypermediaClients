using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
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

        public async Task ProcessShowLinkResponse(HttpResponseMessage response, ClientState clientState)
        {
            if (!response.HasContent() && response.Content.Headers.ContentType != null) return;  // If we don't know the content-type, we can't show it

            var contentStream = await response.Content.ReadAsStreamAsync();

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/vnd.collection+json":
                    clientState.CurrentCollection = CollectionJsonHelper.ParseCollectionJson(contentStream);
                    break;
                case "image/jpeg":
                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(contentStream.AsRandomAccessStream());
                    clientState.CurrentImage = bitmap;
                    break;

                case "image/tiff":

                    break;
                case "application/pdf":
                    break;
            }
        }
    }
}