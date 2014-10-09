using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using ExpenseApprovalApp.Links;
using Tavis;

namespace ExpenseApprovalAppLogic
{
    public static class RequestMessageExtensions
    {
        public static void AttachLink(this HttpRequestMessage requestMessage, object link)
        {
            requestMessage.Properties.Add("tavis.currentlink", link);
        }
        public static Link ExtractLink(this HttpRequestMessage requestMessage)
        {
            return requestMessage.Properties["tavis.currentlink"] as Link;
        }
    }
}
