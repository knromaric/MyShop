using MyShop.Core.Models;
using MyShop.DataAcces.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MyShop.WebUI.Controllers
{
    public class ProductManagerController : Controller
    {
        ProductRepository _context;


        public ProductManagerController()
        {
            _context = new ProductRepository();
        }
        public ActionResult Index()
        {
            var products = _context.GetProducts().ToList();
            return View(products);
        }

        public ActionResult CreateProduct()
        {
            var product = new Product();
            return View(product);
        }

        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            else
            {
                _context.Insert(product);
                _context.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult EditProduct(string id)
        {
            var product = _context.Find(id);
            if(product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }

        [HttpPost]
        public ActionResult EditProduct(Product product, string id)
        {
            var productToEdit = _context.Find(id);
            if(productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }

                productToEdit.Category = product.Category;
                productToEdit.Description = product.Description;
                productToEdit.Image = product.Image;
                productToEdit.Name = product.Name;
                productToEdit.Price = product.Price;

                _context.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult DeleteProduct(string id)
        {
            var productToDelete = _context.Find(id);

            if(productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productToDelete);
            }
        }

        [HttpPost]
        [ActionName("DeleteProduct")]
        public ActionResult ConfirmDelete(string id)
        {
            var productToDelete = _context.Find(id);

            if(productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                _context.Delete(id);
                _context.Commit();
                return RedirectToAction("Index");
            }
        }
    }
}