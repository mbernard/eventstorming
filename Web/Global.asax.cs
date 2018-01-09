using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Domain;
using Infra;

namespace Web
{
    public class MvcApplication : HttpApplication
    {
        public static readonly OrderPerUserRepository OrderPerUserRepository = new OrderPerUserRepository();
        public static readonly OutstandingOrders OutstandinOrders = new OutstandingOrders();
        public static readonly CommandExecutor CommandExecutor = new CommandExecutor(new[]
            {
                EventHandlers.OnOrderCreatedEventHandler(OrderPerUserRepository),
                EventHandlers.OutstandingOrdersEventHandlers(OutstandinOrders)
            },
            new List<Func<object, object>> {OrderSubmittedMapper});

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

        private static object OrderSubmittedMapper(object @event)
        {
            var orderSubmitted = @event as OrderSubmitted;
            if (orderSubmitted != null)
                return new OrderSubmitted_V2
                {
                    Address = orderSubmitted.Address,
                    Date = orderSubmitted.Date,
                    OrderId = orderSubmitted.OrderId
                };

            return @event;
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