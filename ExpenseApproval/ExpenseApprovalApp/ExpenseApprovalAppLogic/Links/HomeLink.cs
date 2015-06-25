using System;
using System.Net.Http;
using System.Threading.Tasks;
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
            Target = new Uri("http://pecan:9090/expenseapp/");
            KeepInHistory = false;
        }
       

    }


}
