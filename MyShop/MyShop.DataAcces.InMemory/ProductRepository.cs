using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

using MyShop.Core.Models;

namespace MyShop.DataAcces.InMemory
{
    public class ProductRepository
    {
        readonly ObjectCache _cache = MemoryCache.Default;

        private List<Product> _products;

        public ProductRepository()
        {
            _products = _cache["products"] as List<Product>;
            if(_products == null)
            {
                _products = new List<Product>();
            }
        }

        public void Commit()
        {
            _cache["products"] = _products; 
        }

        public void Insert(Product product)
        {
            _products.Add(product);
        }

        public void Update(Product product)
        {
            var productToUpdate = _products.Find(p => p.Id == product.Id);

            if(productToUpdate != null)
            {
                productToUpdate = product;
            }
            else
            {
                throw new Exception("Product not found.");
            }
        }

        public Product Find(string id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public IQueryable<Product> GetProducts()
        {
            return _products.AsQueryable();
        }

        public void  Delete(string id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if(product != null)
            {
                _products.Remove(product);
            }
            else
            {
                throw new Exception("Product not found...");
            }


        }
    }
}
