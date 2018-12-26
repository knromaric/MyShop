using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService: IBasketService
    {
        IRepository<Product> _productContext;
        IRepository<Basket> _basketContext;

        public const String BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> productContext, IRepository<Basket> basketContext)
        {
            _productContext = productContext;
            _basketContext = basketContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            var basket = new Basket();

            if(cookie != null)
            {
                string basketId = cookie.Value;

                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = _basketContext.Find(basketId);
                }
                else
                {
                    if (createIfNull)
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
            _basketContext.Insert(basket);
            _basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName)
            {
                Value = basket.Id,
                Expires = DateTime.Now.AddDays(1)
            };

            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if(item == null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };

                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity++;
            }

            _basketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem  item= basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if(item != null)
            {
                basket.BasketItems.Remove(item);
                _basketContext.Commit();
            }
        }

        public List<BasketItemViewModel> GetBasketItems(HttpContextBase httpContext)
        {
            var basket = GetBasket(httpContext, false);

            if(basket != null)
            {
                var result = (from b in basket.BasketItems
                              join p in _productContext.Collection() on b.ProductId equals p.Id
                              select new BasketItemViewModel()
                              {
                                  Id = b.Id,
                                  Quantity = b.Quantity,
                                  ProductName = p.Name,
                                  Price = p.Price,
                                  Image = p.Image
                              }).ToList();

                return result;
            }

            return new List<BasketItemViewModel>();
        }

        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            var basket = GetBasket(httpContext, false);
            var summaryViewModel = new BasketSummaryViewModel(0,0);

            if(basket != null)
            {
                int? basketCount = (from item in basket.BasketItems
                                    select item.Quantity).Sum();

                decimal? basketTotal = (from item in basket.BasketItems
                                        join p in _productContext.Collection() on item.ProductId equals p.Id
                                        select item.Quantity * p.Price).Sum();

                summaryViewModel.BasketCount = basketCount ?? 0;
                summaryViewModel.BasketTotal = basketTotal ?? decimal.Zero;

                return summaryViewModel;
            }
            else
            {
                return summaryViewModel; 
            }

        }




    }
}
