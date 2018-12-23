using MyShop.Core.Models;
using MyShop.DataAcces.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;

namespace MyShop.WebUI.Controllers
{
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> _context;


        public ProductCategoryManagerController(IRepository<ProductCategory> categoryContext)
        {
            _context = categoryContext;
        }

        public ActionResult Index()
        {
            var productcategories = _context.Collection().ToList();
            return View(productcategories);
        }

        public ActionResult CreateCategory()
        {
            var productCategory = new ProductCategory();
            return View(productCategory);
        }

        [HttpPost]
        public ActionResult CreateCategory(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }
            else
            {
                _context.Insert(productCategory);
                _context.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult EditCategory(string id)
        {
            var productCategory = _context.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCategory);
            }
        }

        [HttpPost]
        public ActionResult EditCategory(ProductCategory productCategory, string id)
        {
            var productCategoryToEdit = _context.Find(id);
            if (productCategoryToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(productCategory);
                }

                productCategoryToEdit.Category = productCategory.Category;

                _context.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult DeleteCategory(string id)
        {
            var productCategoryToDelete = _context.Find(id);

            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCategoryToDelete);
            }
        }

        [HttpPost]
        [ActionName("DeleteCategory")]
        public ActionResult ConfirmDeleteCategory(string id)
        {
            var productCategoryToDelete = _context.Find(id);

            if (productCategoryToDelete == null)
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