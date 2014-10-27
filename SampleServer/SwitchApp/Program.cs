using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.SelfHost;
using HypermediaAppServer.ExpenseApp;
using HypermediaAppServer.ExpenseApp.Model;
using HypermediaAppServer.SwitchApp;
using Microsoft.Practices.Unity;
using Tavis;

namespace HypermediaAppServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverUrl = new Uri("http://pecan:9001");
            var config = new HttpSelfHostConfiguration(serverUrl);
            ConfigureApi(config, serverUrl);

            var server = new HttpSelfHostServer(config);

            server.OpenAsync().Wait();

            string exit = "";
            while (String.IsNullOrWhiteSpace(exit))
            {
                Console.WriteLine("Server listening on " + serverUrl.AbsoluteUri);
                exit = Console.ReadLine();
                Console.Clear();
            }




            server.CloseAsync().Wait();
        }



        private static void ConfigureApi(HttpSelfHostConfiguration config, Uri serverUrl)
        {
          

            var route = new TreeRoute("");

            route.AddWithPath("switch", r => r.To<SwitchController>());
            route.AddWithPath("switch/on", r => r.To<SwitchController>().ToAction("on"));
            route.AddWithPath("switch/off", r => r.To<SwitchController>().ToAction("off"));

            route.AddWithPath("expenseapp", r => r.To<ExpenseAppController>());
            route.AddWithPath("expenses", r => r.To<ExpensesController>());
            route.AddWithPath("expense/{expenseId}", r => r.To<ExpenseController>());
            route.AddWithPath("expense/{expenseId}/approve", r => r.To<ExpenseController>().ToAction("Approve"));
            route.AddWithPath("expense/{expenseId}/reject", r => r.To<ExpenseController>().ToAction("Reject"));
            route.AddWithPath("receipt", r => r.To<ReceiptController>());

            
            config.Routes.Add("",route);
            var container = config.CreateUnityContainer();
            container.RegisterInstance<DataService>(new DataService());
            container.RegisterInstance<IUrlFactory>(new UrlFactory(route, serverUrl));
            config.MessageHandlers.Add(new ConsoleRequestLogger());
        }

      
    }

    public class ConsoleRequestLogger : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("> {0} {1}",request.Method,request.RequestUri.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped));
            ProcessHeader(request.Headers, (name, value) => Console.WriteLine("> {0}: {1}", name, value));   

            if (request.Content != null)
            {
                ProcessHeader(request.Content.Headers, (name,value)=>Console.WriteLine("> {0}: {1}", name, value));   
            }

            var response = await base.SendAsync(request, cancellationToken);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("< {0} {1}", (int)response.StatusCode, response.ReasonPhrase);
            ProcessHeader(response.Headers, (name, value) => Console.WriteLine("< {0}: {1}", name, value));
            if (response.Content != null)
            {
                ProcessHeader(response.Content.Headers, (name, value) => Console.WriteLine("< {0}: {1}", name, value));
                Console.WriteLine();
                var body = await response.Content.ReadAsStringAsync();
                if (body.Length > 3000)
                {
                    body = body.Substring(0, 3000) + "...";
                }
                Console.WriteLine(body);
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-------------------------------------------------------------------------------------");
            Console.ResetColor();
            return response;
        }

        private static void ProcessHeader(HttpHeaders headers, Action<string,string> headerAction)
        {
            foreach (var httpRequestHeader in headers)
            {
                foreach (var headerValue in httpRequestHeader.Value)
                {
                    headerAction(httpRequestHeader.Key, headerValue);
                }
            }
        }
    }

    public interface IUrlFactory
    {
        Uri CreateUrl<T>() where T : IHttpController;
        Uri CreateUrl(string relativeUri);
    }
    public class UrlFactory : IUrlFactory
    {
        private readonly TreeRoute _rootRoute;
        private readonly Uri _baseUrl;

        
        public UrlFactory(TreeRoute rootRoute, Uri baseUrl)
        {
            _rootRoute = rootRoute;
            _baseUrl = baseUrl;
        }

        
        public Uri CreateUrl(string relativeUri)
        {
            return new Uri(_baseUrl, relativeUri);
        }
        public Uri CreateUrl<T>() where T : IHttpController
        {
            return new Uri(_baseUrl, _rootRoute.GetUrlForController(typeof (T)));
        }
    }

   
}
