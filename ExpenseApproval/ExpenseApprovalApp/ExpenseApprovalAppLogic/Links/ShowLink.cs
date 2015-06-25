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
            KeepInHistory = true;
        }
    }
}