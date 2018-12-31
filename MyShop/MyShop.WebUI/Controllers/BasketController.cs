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
        IRepository<Customer> _customerContext;
        private IBasketService _basketService;
        private IOrderService _orderService; 

        public BasketController(IBasketService basketService, IOrderService orderService, IRepository<Customer> customerContext)
        {
            _basketService = basketService;
            _orderService = orderService;
            _customerContext = customerContext;
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

        [Authorize]
        public ActionResult Checkout()
        {
            Customer customer = _customerContext.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if(customer != null)
            {
                Order order = new Order()
                {
                    Email = customer.Email, 
                    City = customer.City, 
                    State = customer.State, 
                    Street = customer.Street, 
                    ZipCode = customer.ZipCode, 
                    FirstName = customer.FirstName, 
                    LastName = customer.LastName
                };

                return View(order);
            }
            else
            {
                return RedirectToAction("Error");
            }
            
        }


        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order)
        {
            var basketItems = _basketService.GetBasketItems(HttpContext);
            order.OrderStatus = "Order Created";
            order.Email = User.Identity.Name;

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