using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        private IBasketService _basketService;
        private IOrderService _orderService; 

        public BasketController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }
        // GET: Basket
        public ActionResult Index()
        {
            var model = _basketService.GetBasketItems(HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string id)
        {
            _basketService.AddToBasket(HttpContext, id);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string id)
        {
            _basketService.RemoveFromBasket(HttpContext, id);

            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            var basketSummary = _basketService.GetBasketSummary(HttpContext);

            return PartialView(basketSummary);
        }

        public ActionResult Checkout()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Checkout(Order order)
        {
            var basketItems = _basketService.GetBasketItems(HttpContext);
            order.OrderStatus = "Order Created";

            // process the payment 

            order.OrderStatus = "Payment Processed";
            _orderService.CreateOrder(order, basketItems);
            _basketService.ClearBasket(HttpContext);

            return RedirectToAction("ThankYou", new { OrderId = order.Id });
        }

        public ActionResult ThankYou(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}