using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAcces.InMemory
{
    public class InMemoryRepository<T>  where T :  BaseEntity
    {
        private ObjectCache _cache = MemoryCache.Default;
        private List<T> _itemCollection;
        readonly string className;

        public InMemoryRepository()
        {
            className = typeof(T).Name;
            _itemCollection = _cache[className] as List<T>;
            if(_itemCollection == null)
            {
                _itemCollection = new List<T>();
            }
        }

        public void Commit()
        {
            _cache[className] = _itemCollection;
        }

        public void Insert(T item)
        {
            _itemCollection.Add(item);
        }

        public void Update(T item)
        {
            var itemToUpdate = _itemCollection.Find(i => i.Id == item.Id);

            if(itemToUpdate != null)
            {
                itemToUpdate = item;
            }
            else
            {
                throw new Exception(className + "Not found");
            }
        }

        public T Find(string id)
        {
            var item = _itemCollection.Find(i => i.Id == id);

            if(item == null)
            {
                throw new Exception(className + "Not found");
            }
            else
            {
                return item;
            }
        } 

        public IQueryable<T> Collection()
        {
            return _itemCollection.AsQueryable();
        }

        public void Delete(string id)
        {
            var itemToDelete = _itemCollection.Find(i => i.Id == id);

            if (itemToDelete != null)
            {
                _itemCollection.Remove(itemToDelete);
            }
            else
            {
                throw new Exception(className + "Not found");
            }
        }


    }
}
