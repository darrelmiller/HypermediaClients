using System.Net.Http;

namespace ExpenseApprovalAppLogic {

    public static class ResponseMessageExtensions
    {

        public static bool HasContent(this HttpResponseMessage response)
        {
            return (response.Content != null && response.Content.Headers.ContentLength != 0);
        }
    }
}
