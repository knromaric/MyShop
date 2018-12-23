using MyShop.Core.Models;
using MyShop.Core.ViewModels;
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
        InMemoryRepository<Product> _productContext;
        InMemoryRepository<ProductCategory> _productCategoryContext;


        public ProductManagerController()
        {
            _productContext = new InMemoryRepository<Product>();
            _productCategoryContext = new InMemoryRepository<ProductCategory>();
        }
        public ActionResult Index()
        {
            var products = _productContext.Collection().ToList();
            return View(products);
        }

        public ActionResult CreateProduct()
        {
            var viewModel = new ProductManagerViewModel
            {
                Product = new Product(),
                ProductCategories = _productCategoryContext.Collection()
            };
            return View(viewModel);
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
                _productContext.Insert(product);
                _productContext.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult EditProduct(string id)
        {
            var product = _productContext.Find(id);
            if(product == null)
            {
                return HttpNotFound();
            }
            else
            {
                var viewModel = new ProductManagerViewModel
                {
                    Product = product,
                    ProductCategories = _productCategoryContext.Collection()
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult EditProduct(Product product, string id)
        {
            var productToEdit = _productContext.Find(id);
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

                _productContext.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult DeleteProduct(string id)
        {
            var productToDelete = _productContext.Find(id);

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
            var productToDelete = _productContext.Find(id);

            if(productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                _productContext.Delete(id);
                _productContext.Commit();
                return RedirectToAction("Index");
            }
        }
    }
}