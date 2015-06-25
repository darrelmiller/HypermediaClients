using System.Net.Http;
using System.Threading.Tasks;
using ExpenseApprovalAppLogic.Tools;
using Tavis;

namespace ExpenseApprovalAppLogic.Links
{
    [LinkRelationType("urn:tavis:action")]
    public class ActionLink : BaseLink
    {
        public ActionLink()
        {
            this.Method = HttpMethod.Post;
            KeepInHistory = false;
        }

    }
}