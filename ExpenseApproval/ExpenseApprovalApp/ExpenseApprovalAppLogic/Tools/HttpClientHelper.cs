using System.Net.Http;
using System.Threading.Tasks;
using ExpenseApprovalApp;
using Tavis;

namespace ExpenseApprovalAppLogic.Tools
{
    public static class  HttpClientHelper

    {

        public static async Task ProcessRedirect(HttpResponseMessage response, ExpenseAppClientState clientState, Link contextLink)
        {
            if ((int)response.StatusCode >= 300)
            {
                // Process redirect
                // TODO 
            }
        }

        public static bool ProcessClientError(HttpResponseMessage response, ExpenseAppClientState clientState, Link contextLink)
        {
            if ((int)response.StatusCode >= 400)
            {
                // Server claims we made a bad request
                // Don't change client state other than to record an error has occured
                // Create error instance, add it to the list of errors
                clientState.UserMessage = string.Format("{0} error returned while following {1} to {2}", response.ReasonPhrase,
                    contextLink.Relation, contextLink.Target.OriginalString);
                return true;
            }
            return false;
        }

        public static void ProcessServerErrors(HttpResponseMessage response, ExpenseAppClientState clientState)
        {
            if ((int)response.StatusCode >= 500)
            {
                // Server failed to process request correctly.
                // Don't change client state other than to record an error has occured
                // Create error instance, add it to the list of errors
                // If we keep getting these, make sure the user can exit the app or select another server
                clientState.UserMessage = "Server failed to process request successfully - " + response.ReasonPhrase;
            }
        }
    }

    public static class HttpStatusCodeExtensions
    {
        public static bool IsClientError(this System.Net.HttpStatusCode httpStatusCode)
        {
            return (int)httpStatusCode >= 400 && (int)httpStatusCode > 500;
        }

        public static bool IsServerError(this System.Net.HttpStatusCode httpStatusCode)
        {
            return (int)httpStatusCode >= 500;
        }

        public static bool IsRedirect(this System.Net.HttpStatusCode httpStatusCode)
        {
            return (int)httpStatusCode >= 300 && (int)httpStatusCode < 400;
        }

    }
}
