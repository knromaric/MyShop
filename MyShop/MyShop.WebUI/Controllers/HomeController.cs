using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        IRepository<Product> _productContext;
        IRepository<ProductCategory> _productCategoryContext;


        public HomeController(IRepository<Product> productContext,
            IRepository<ProductCategory> productCategoryContext)
        {
            _productContext = productContext;
            _productCategoryContext = productCategoryContext;
        }
        public ActionResult Index(string category = null)
        {
            List<Product> products;
            List<ProductCategory> categories = _productCategoryContext.Collection().ToList();

            if (category == null)
            {
                products = _productContext.Collection().ToList();
            }
            else
            {
                products = _productContext.Collection().Where(p => p.Category == category).ToList();
            }

            var listProducts = new ProductListViewModel
            {
                Products = products,
                ProductCategories = categories
            };

            return View(listProducts);
        }

        public ActionResult Details(string id)
        {
            var product = _productContext.Find(id);

            if(product == null)
            {
                return HttpNotFound();
            }

            return View(product);
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