using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderManagerController : Controller
    {
        private IOrderService _orderService;

        public OrderManagerController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // GET: OrderManager
        public ActionResult Index()
        {
            var orders = _orderService.GetOrderList().ToList();
            return View(orders);
        }

        public ActionResult UpdateOrder(string id)
        {
            ViewBag.StatusList = new List<string>() {
                "Order Created", 
                "Payment Processed", 
                "Order Shipped", 
                "Order Complete"
            };

            Order order = _orderService.GetOrder(id);

            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateOrder(Order updatedOrder, string id)
        {
            var order = _orderService.GetOrder(id);

            order.OrderStatus = updatedOrder.OrderStatus;

            _orderService.UpdateOrder(order);

            return RedirectToAction("Index");
        }
    }
}