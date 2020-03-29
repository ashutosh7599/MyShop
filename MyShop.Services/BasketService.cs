using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService
    {
        IRepositroy<Product> productContext;
        IRepositroy<Basket> basketContext;
        public const string BasketSessionName = "eCommerceBasket";
        public BasketService(IRepositroy<Product> productContext,IRepositroy<Basket> basketContext)
        {
            this.basketContext = basketContext;
            this.productContext = productContext;
        }
        private Basket GetBasket(HttpContextBase httpContext , bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);
            Basket basket = new Basket();
            if(cookie != null)
            {
                string basketId = cookie.Value;
                if(!string.IsNullOrEmpty(basketId))
                {
                    basket = basketContext.Find(basketId); 
                }
                else
                {
                    if(createIfNull)
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }
            return basket;

        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();
            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddSeconds(10000);
            httpContext.Response.Cookies.Add(cookie);
            return basket;

        }
        public void AddToBasket(HttpContextBase httpContext,string productid)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productid);
            if(item == null)
            {
                item = new BasketItem() {
                    BasketId = basket.Id,
                    ProductId = productid,
                    Quantity=1
                    


                };
                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity += 1;

            }
            basketContext.Commit();
        }
        public void RemoveFromBasket(HttpContextBase httpContext,string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);
            if(item != null)
            {
                basket.BasketItems.Remove(item);
                basketContext.Commit();
            }
        }
    }
}
