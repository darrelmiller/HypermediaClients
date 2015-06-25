using System.Net.Http;
using System.Threading.Tasks;
using Tavis;

namespace ExpenseApprovalAppLogic.Tools
{

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
