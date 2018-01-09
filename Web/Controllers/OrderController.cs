using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class OrderController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return this.View("Index");
        }

        [HttpPost]
        public ActionResult CancelOrder(string orderId)
        {
            return this.RedirectToAction("OrderCancellationConfirmation");
        }

        [HttpGet]
        public ActionResult OrderCancellationConfirmation()
        {
            return this.View("OrderCancellationConfirmation");
        }
    }
}