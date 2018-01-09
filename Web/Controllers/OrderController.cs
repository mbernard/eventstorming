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

        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Index(string id)
        {
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

        [HttpPost]
        public ActionResult PayOrder(string orderId, decimal amount)
        {
            MvcApplication.CommandExecutor.Execute<Order>(orderId, new PayOrder(orderId, amount));

            return View("OrderPaid");
        }

        [HttpGet]
        public ActionResult PayOrder(string id)
        {
            var orderPerUser = MvcApplication.OrderPerUserRepository.GetForOrder(id);

            return this.View("PayOrder", orderPerUser);
        }

        [HttpGet]
        public ActionResult OrderCancellationConfirmation()
        {
            return this.View("OrderCancellationConfirmation");
        }

        [HttpPost]
        public ActionResult CreateOrder()
        {
            var orderId = $"order-{Guid.NewGuid()}";
            MvcApplication.CommandExecutor.Execute<Order>(orderId, new CreateOrder(orderId));

            return RedirectToAction("Index", "Order", new {id = orderId});
        }
        
        [HttpPost]
        public ActionResult AddItemToOrder(string orderId, string name, decimal price)
        {
            MvcApplication.CommandExecutor.Execute<Order>(orderId, new AddItemToOrder(name, price));

            return RedirectToAction("Index", "Order", new {id = orderId});

        }
        

        [HttpPost]
        public ActionResult SubmitOrder(string orderId)
        {
            MvcApplication.CommandExecutor.Execute<Order>(orderId, new SubmitOrder());

            return RedirectToAction("Index", "Order", new {id = orderId});
        }
        
        
    }
}