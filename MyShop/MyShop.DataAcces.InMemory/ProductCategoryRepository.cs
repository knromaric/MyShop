using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAcces.InMemory
{
    public class ProductCategoryRepository
    {
        readonly ObjectCache _cache = MemoryCache.Default;

        private List<ProductCategory> _productsCategories;

        public ProductCategoryRepository()
        {
            _productsCategories = _cache["productCategories"] as List<ProductCategory>;
            if (_productsCategories == null)
            {
                _productsCategories = new List<ProductCategory>();
            }
        }

        public void Commit()
        {
            _cache["productCategories"] = _productsCategories;
        }

        public void Insert(ProductCategory productCategory)
        {
            _productsCategories.Add(productCategory);
        }

        public void Update(ProductCategory productCategory)
        {
            var productCategoryToUpdate = _productsCategories.Find(p => p.Id == productCategory.Id);

            if (productCategoryToUpdate != null)
            {
                productCategoryToUpdate = productCategory;
            }
            else
            {
                throw new Exception("Product Category not found.");
            }
        }

        public ProductCategory Find(string id)
        {
            return _productsCategories.FirstOrDefault(p => p.Id == id);
        }

        public IQueryable<ProductCategory> GetProductCategories()
        {
            return _productsCategories.AsQueryable();
        }

        public void Delete(string id)
        {
            var productCategory = _productsCategories.FirstOrDefault(p => p.Id == id);

            if (productCategory != null)
            {
                _productsCategories.Remove(productCategory);
            }
            else
            {
                throw new Exception("Product Category not found...");
            }
        }
    }
}
