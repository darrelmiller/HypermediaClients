using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Tavis;
using Tavis.Home;

namespace HypermediaAppServer.ExpenseApp
{
    public class ExpenseAppController : ApiController
    {
        private readonly IUrlFactory _urlFactory;

        public ExpenseAppController(IUrlFactory urlFactory)
        {
            _urlFactory = urlFactory;
        }

        public IHttpActionResult Get()
        {
            var homeDocument = new HomeDocument();
            homeDocument.AddResource(new Link()
            {
                Relation = "urn:tavis:show",
                Target = _urlFactory.CreateUrl<ExpensesController>()
            });

            
            return new ResponseMessageResult(new HttpResponseMessage() {Content = new HomeContent(homeDocument)});
        }
    }
}
