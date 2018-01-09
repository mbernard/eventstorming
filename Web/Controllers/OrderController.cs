using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Domain;

namespace Web.Controllers
{
    public class OrderController : Controller
    {
        [HttpGet]
        public ActionResult Index(string id)
        {
            //var orderPerUser = new OrderPerUser
            //{
            //    OrderDate = DateTime.Now.AddDays(-3),
            //    Items = new List<(string Name, decimal Price)>
            //    {
            //        ("Pizza", 10.00m),
            //        ("Fries", 2.00m),
            //        ("Coke", 1.00m)
            //    },
            //    TotalPrice = 13.00m
            //};

            var orderPerUser = MvcApplication.OrderPerUserRepository.GetForOrder(id);

            return this.View("Index", orderPerUser);
        }

        [HttpPost]
        public ActionResult CancelOrder(string orderId)
        {
            //var order = new Order();
            //order.Execute(new CancelOrder(id));

            return this.RedirectToAction("OrderCancellationConfirmation");
        }

        [HttpGet]
        public ActionResult OrderCancellationConfirmation()
        {
            return this.View("OrderCancellationConfirmation");
        }
    }
}