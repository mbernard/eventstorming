using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Domain;

namespace Web.Controllers
{
    public class KitchenController : Controller

    {
        [HttpGet]
        public ActionResult List()
        {
            return this.View("Index", MvcApplication.OutstandinOrders);
        }

        [HttpPost]
        public ActionResult StartOrderPreperation(string id)
        {
            MvcApplication.OutstandinOrders.Apply(new OrderStarted { OrderId = id });
            return this.View("Index", MvcApplication.OutstandinOrders);
        }

        [HttpPost]
        public ActionResult FinishOrder(string id)
        {
            MvcApplication.OutstandinOrders.Apply(new OrderPrepared { OrderId = id });
            return this.View("Index", MvcApplication.OutstandinOrders);
        }

        [HttpPost]
        public ActionResult CancelOrder(string id)
        {
            MvcApplication.OutstandinOrders.Apply(new OrderCanceled { OrderId = id });
            return this.View("Index", MvcApplication.OutstandinOrders);
        }
    }
}