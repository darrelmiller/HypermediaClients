using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using HypermediaAppServer.ExpenseApp.Model;
using SwitchApp;
using WebApiContrib.CollectionJson;

namespace HypermediaAppServer.ExpenseApp
{
    public class ExpensesController : ApiController
    {
        private readonly IUrlFactory _urlFactory;
        private readonly DataService _dataService;

        public ExpensesController(IUrlFactory urlFactory, DataService dataService)
        {
            _urlFactory = urlFactory;
            _dataService = dataService;
        }

        public IHttpActionResult Get()
        {
            var expenses = _dataService.ExpenseRepository.GetAll();


            var collection = GetExpensesCollection(expenses,_urlFactory);
            collection.Href = Request.RequestUri;
            
            return new ResponseMessageResult(new HttpResponseMessage() {Content = new CollectionJsonContent(collection)});
        }

        public static Collection GetExpensesCollection(IEnumerable<Expense> Expenses, IUrlFactory urlFactory)
        {
            var eventsCollection = new Collection();

            foreach (var expense in Expenses)
            {
                var item = new Item();
                item.Href = urlFactory.CreateUrl("expense/"+ expense.Id);
                item.Data.Add(new Data { Name = "Description", Value = expense.Description });
                item.Data.Add(new Data { Name = "Amount", Value = expense.Amount.ToString() });
                item.Data.Add(new Data { Name = "Category", Value = expense.Category });


                item.Links.Add(new Link() { Href = urlFactory.CreateUrl(String.Format("expense/{0}/",expense.Id)), Prompt = "Details", Rel = "urn:tavis:show" });
                
                if (!String.IsNullOrEmpty(expense.ReceiptFileName))
                {
                    item.Links.Add(new Link()
                    {
                        Href = urlFactory.CreateUrl(String.Format("receipt?filename={0}", expense.ReceiptFileName)),
                        Prompt = "Receipt",
                        Rel = "urn:tavis:show"
                    });
                }

                if (expense.Status != "approved")
                {
                    item.Links.Add(new Link()
                    {
                        Href = urlFactory.CreateUrl(String.Format("expense/{0}/approve", expense.Id)),
                        Prompt = "Approve",
                        Rel = "urn:tavis:action"
                    }); //approvals
                }
                if (expense.Status != "unapproved")
                {
                    item.Links.Add(new Link()
                    {
                        Href = urlFactory.CreateUrl(String.Format("expense/{0}/reject", expense.Id)),
                        Prompt = "Reject",
                        Rel = "urn:tavis:action"
                    }); // reject?expenseId=44
                }
                eventsCollection.Items.Add(item);
            }
            return eventsCollection;
        }
    }
}
