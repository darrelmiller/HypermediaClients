using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ExpenseApprovalApp.Tools
{
    public static class ResponseMessageExtensions
    {

        public static bool HasContent(this HttpResponseMessage response)
        {
            return (response.Content != null && response.Content.Headers.ContentLength != 0);
        }
    }
}
