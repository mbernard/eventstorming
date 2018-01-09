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
        public ActionResult CancelOrder()
        {
            return this.View("CancelOrder");
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}