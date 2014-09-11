using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Windows.UI.Xaml.Data;
using ExpenseApprovalApp.Tools;
using Tavis;

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

    }
}
