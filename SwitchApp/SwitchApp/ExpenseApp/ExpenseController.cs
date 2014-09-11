using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using HypermediaAppServer.ExpenseApp.Model;

namespace HypermediaAppServer.ExpenseApp
{
    public class ExpenseController : ApiController
    {
        private readonly IUrlFactory _urlFactory;
        private readonly DataService _dataService;

        public ExpenseController(IUrlFactory urlFactory, DataService dataService)
        {
            _urlFactory = urlFactory;
            _dataService = dataService;
        }

        public IHttpActionResult Get(string expenseId)
        {
            return new ResponseMessageResult(new HttpResponseMessage());
        }

        [ActionName("Approve")]
        public IHttpActionResult PostApprove(int expenseId)
        {
            var expense = _dataService.ExpenseRepository.Get(expenseId);

            if (expense.Status == "approved") return new BadRequestResult(Request);

            expense.Status = "approved";

            return new OkResult(Request);
        }

        [ActionName("Reject")]
        public IHttpActionResult PostReject(int expenseId)
        {
            var expense = _dataService.ExpenseRepository.Get(expenseId);
            if (expense.Status == "unapproved") return new BadRequestResult(Request);

            expense.Status = "unapproved";

            return new OkResult(Request);

        }

    }
}
