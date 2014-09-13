using System;
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
            var serverUrl = new Uri("http://pecan:9090");
            var config = new HttpSelfHostConfiguration(serverUrl);
            ConfigureApi(config, serverUrl);

            var server = new HttpSelfHostServer(config);

            server.OpenAsync().Wait();


            Console.WriteLine("Server listening on " + serverUrl.AbsoluteUri);
            Console.ReadLine();



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
