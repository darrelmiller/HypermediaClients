using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace HypermediaAppServer
{
    public static class HttpConfigurationExtensions
    {
        public static IUnityContainer CreateUnityContainer(this HttpConfiguration config)
        {
            var container = new UnityContainer();
            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().Where(t => t.IsAssignableFrom(typeof(IHttpController))),
                WithMappings.FromAllInterfaces);
            config.DependencyResolver = new UnityResolver(container);
            return container;
        }
    }
    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}
