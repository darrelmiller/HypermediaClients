using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;

namespace HypermediaAppServer.ExpenseApp
{
    public class ReceiptController : ApiController
    {
        public IHttpActionResult Get(string filename)
        {
            var resourceStream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), filename);
            if (resourceStream == null) return new NotFoundResult(Request);
            var httpResponseMessage = new HttpResponseMessage() { RequestMessage = Request, Content = new StreamContent(resourceStream) };
            var ext = Path.GetExtension(filename);
            switch (ext)
            {
                case ".jpg": httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    break;
                case ".png": httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    break;
                case ".pdf": httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    break;
                case ".tif": httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/tiff");
                    break;
            }
            
            return new ResponseMessageResult(httpResponseMessage);
        }
    }
}
