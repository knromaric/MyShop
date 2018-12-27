using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyShop.WebUI.Tests.Mocks
{
    public class MockContext<T>: IRepository<T> where T: BaseEntity
    {
        private List<T> _itemCollection;

        public MockContext()
        {
            _itemCollection = new List<T>();   
        }

        public void Commit()
        {
            return;
        }

        public void Insert(T item)
        {
            _itemCollection.Add(item);
        }

        public void Update(T item)
        {
            var itemToUpdate = _itemCollection.Find(i => i.Id == item.Id);

            if (itemToUpdate != null)
            {
                itemToUpdate = item;
            }
            else
            {
                throw new Exception("Not found");
            }
        }

        public T Find(string id)
        {
            var item = _itemCollection.Find(i => i.Id == id);

            if (item == null)
            {
                throw new Exception("Not found");
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
                throw new Exception("Not found");
            }
        }
    }
}
