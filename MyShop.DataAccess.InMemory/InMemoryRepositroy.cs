using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.InMemory
{
    public class InMemoryRepositroy<T> where T : BaseEntity
    {
        ObjectCache cache = MemoryCache.Default;
        List<T> items;
        string classname = typeof(T).Name;
        public InMemoryRepositroy()
        {
            items = cache[classname] as List<T>;
            if(items == null)
            {
                items = new List<T>();

            }

        }
        public void Commit()
        {
            cache[classname] = items;
        }
        public void Insert(T x)
        {
            items.Add(x);

        }
        public void Update(T x)
        {
            T tToUpdate = items.Find(i => i.Id == x.Id);
            if(tToUpdate == null)
            {
                throw new Exception(classname + " Not Found");
            }
            else
            {
                tToUpdate = x;
            }
        }
        public void Delete(string id)
        {
            T tToDelete = items.Find(i => i.Id == id);
            if(tToDelete == null)
            {
                throw new Exception(classname + " Not Found!");
            }
            else
            {
                items.Remove(tToDelete);
            }
        }
        public T Find(string id)
        {
            T tToFind = items.Find(i => i.Id == id);
            if(tToFind == null)
            {
                throw new Exception(classname + " Not Found!");
            }
            else
            {
                return tToFind;
            }
        }
        public IQueryable<T> Collection()
        {
            return items.AsQueryable();
        }
    }
}
