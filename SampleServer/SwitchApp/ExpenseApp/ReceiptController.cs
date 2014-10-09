using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;
using Tavis;

namespace HypermediaAppServer.ExpenseApp
{
    public class ReceiptController : ApiController
    {
        private readonly IUrlFactory _urlFactory;

        public ReceiptController(IUrlFactory urlFactory)
        {
            _urlFactory = urlFactory;
        }

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
            var backLink = new Link()
            {
                Relation = "urn:tavis:show",
                Target = _urlFactory.CreateUrl<ExpensesController>(),
                Title = "back"
            };
            httpResponseMessage.Headers.AddLinkHeader(backLink);
            return new ResponseMessageResult(httpResponseMessage);
        }
    }
}
