using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Domain;

using Infra;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static readonly OrderPerUserRepository OrderPerUserRepository = new OrderPerUserRepository();
        public static readonly CommandExecutor CommandExecutor = new CommandExecutor(new[]
        {
            EventHandlers.OnOrderCreatedEventHandler(OrderPerUserRepository)
        });

        static MvcApplication()
        {
//            var createOrder = new CreateOrder("1");
//            CommandExecutor.Execute<Order>(createOrder.OrderId, createOrder);
//            var createOrder2 = new CreateOrder("2");
//            CommandExecutor.Execute<Order>(createOrder2.OrderId, createOrder2);
//            var createOrder3 = new CreateOrder("3");
//            CommandExecutor.Execute<Order>(createOrder3.OrderId, createOrder3);
//        }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
