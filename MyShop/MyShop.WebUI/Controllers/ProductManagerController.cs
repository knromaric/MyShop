using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.DataAcces.InMemory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductManagerController : Controller
    {
        IRepository<Product> _productContext;
        IRepository<ProductCategory> _productCategoryContext;


        public ProductManagerController(IRepository<Product> productContext, 
            IRepository<ProductCategory> productCategoryContext)
        {
            _productContext = productContext;
            _productCategoryContext = productCategoryContext;
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
        public ActionResult CreateProduct(Product product, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            else
            {
                if(file != null)
                {
                    product.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath($"/Content/ProductImages/") + product.Image);
                }
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
        public ActionResult EditProduct(Product product, string id, HttpPostedFileBase file)
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

                if (file != null)
                {
                    productToEdit.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath($"/Content/ProductImages/") + productToEdit.Image);
                }
                productToEdit.Category = product.Category;
                productToEdit.Description = product.Description;
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