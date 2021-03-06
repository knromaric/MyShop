﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTests
    {
        [TestMethod]
        public void CanAddBasketItems()
        {
            //SETUP
            IRepository<Basket> baskets = new MockContext<Basket>();
            IRepository<Product> products = new MockContext<Product>();
            IRepository<Order> orders = new MockContext<Order>();
            IRepository<Customer> Customers = new MockContext<Customer>();
            var httpContext = new HttpMockContext();
            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);

            //ARRANGE
            var controller = new BasketController(basketService, orderService, Customers);
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            //ACT
            controller.AddToBasket("1");
            Basket basket = baskets.Collection().FirstOrDefault();

            //ASSERT
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual("1", basket.BasketItems.ToList().FirstOrDefault().ProductId);
        }

        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            //SETUP
            IRepository<Basket> baskets = new MockContext<Basket>();
            IRepository<Product> products = new MockContext<Product>();
            IRepository<Order> orders = new MockContext<Order>();
            IRepository<Customer> Customers = new MockContext<Customer>();
            products.Insert(new Product() { Id = "1", Price = 20.00m });
            products.Insert(new Product() { Id = "2", Price = 60.00m });

            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 }); 
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1 });

            baskets.Insert(basket);

            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);
            var controller = new BasketController(basketService, orderService, Customers);

            var httpContext = new HttpMockContext();
            httpContext.Request.Cookies.Add(new HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);


            var result = controller.BasketSummary() as PartialViewResult;
            var basketSummary = (BasketSummaryViewModel)result.ViewData.Model;

            Assert.AreEqual(3, basketSummary.BasketCount);
            Assert.AreEqual(100.00m, basketSummary.BasketTotal);

        }

        [TestMethod]
        public void CanCheckoutAndCreateOrder()
        {
            IRepository<Product> products = new MockContext<Product>();
            products.Insert(new Product() { Id = "1", Price = 10.00m }); 
            products.Insert(new Product() { Id = "2", Price = 5.00m });

            IRepository<Basket> baskets = new MockContext<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2, BasketId = basket.Id });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1, BasketId = basket.Id });


            baskets.Insert(basket);


            IBasketService basketService = new BasketService(products, baskets);
            IRepository<Order> orders = new MockContext<Order>();
            IOrderService orderService = new OrderService(orders);
            IRepository<Customer> Customers = new MockContext<Customer>();

            Customers.Insert(new Customer() { Id = "1", Email="roma@gmail.com", ZipCode="5555" });

            IPrincipal FakeUser = new GenericPrincipal(new GenericIdentity("roma@gmail.com", "Forms"), null);

            var controller = new BasketController(basketService, orderService, Customers);
            var httpContext = new HttpMockContext
            {
                User = FakeUser
            };

            httpContext.Request.Cookies.Add(new HttpCookie("eCommerceBasket")
            {
                Value = basket.Id
            });

            controller.ControllerContext = new ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);


            //Act
            Order order = new Order();
            controller.Checkout(order);

            //Assert

            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);

            Order orderInRep = orders.Find(order.Id);
            Assert.AreEqual(2, orderInRep.OrderItems.Count);
        
        }
    }
}
