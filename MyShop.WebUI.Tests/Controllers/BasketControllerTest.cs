using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTest
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            IRepositroy<Basket> baskets = new MockContext<Basket>();
            IRepositroy<Order> orders = new MockContext<Order>();

            var httpContext = new MockHttpContext();
            IRepositroy<Product> products = new MockContext<Product>();
            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);
            IRepositroy < Customer > customer = new MockContext<Customer>();
            var controller = new BasketController(basketService,orderService,customer);
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller); 
            //basketService.AddToBasket(httpContext, "1");
            controller.AddToBasket( "1");
            Basket basket = baskets.Collection().FirstOrDefault();
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual("1", basket.BasketItems.ToList().FirstOrDefault().ProductId);
        }
        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            IRepositroy<Order> orders = new MockContext<Order>();
            IOrderService orderService = new OrderService(orders);

            IRepositroy<Basket> baskets = new MockContext<Basket>();
            IRepositroy<Product> products = new MockContext<Product>();
            products.Insert(new Product() { Id = "1", Price = 10.00m });
            products.Insert(new Product() { Id = "2", Price = 5.00m });
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem()
            {
                ProductId = "1",
                Quantity = 2
            });
            basket.BasketItems.Add(new BasketItem()
            {
                ProductId = "2",
                Quantity = 1
            });
            baskets.Insert(basket);
            IRepositroy<Customer> customer = new MockContext<Customer>();
            IBasketService basketService = new BasketService(products, baskets);
            var controller = new BasketController(basketService,orderService,customer);
            var httpcontext = new MockHttpContext();

            httpcontext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpcontext, new System.Web.Routing.RouteData(), controller);
            //var result = controller.BasketSummary() as PartialViewResult;
            //var basketSummary = (BasketSummaryViewModel)result.ViewData.Model;
            //Assert.AreEqual(3, basketSummary.BasketCount);
            //Assert.AreEqual(25.00m, basketSummary.BasketTotal);
        }
        [TestMethod]
        public void CanCheckoutAndCreateOrder()
        {
            IRepositroy<Product> products = new MockContext<Product>();
            products.Insert(new Product()
            {
                Id="1",Price=10.00m
            });
            products.Insert(new Product()
            {
                Id = "2",
                Price = 5.00m
            });
            IRepositroy<Basket> baskets = new MockContext<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId ="1",Quantity=2,BasketId=basket.Id});
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1, BasketId = basket.Id });
            baskets.Insert(basket);
            IBasketService basketService = new BasketService(products, baskets);
            IRepositroy<Order> orders = new MockContext<Order>();
            IOrderService orderService = new OrderService(orders);
            
            IRepositroy<Customer> customer = new MockContext<Customer>();
            var controller =new  BasketController(basketService, orderService,customer);
            customer.Insert(new Customer() { Id = "1", Email = "aks.hbd@outlook.com", ZipCode = "461001" });
            IPrincipal FakeUser = new GenericPrincipal(new GenericIdentity("aks.hbd@outlook.com","Forms"),null); 

            var httpContext = new MockHttpContext();
            httpContext.User = FakeUser;
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket")
            {
                Value = basket.Id
            }) ;
            controller.ControllerContext = new ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);
            Order order = new Order();
            controller.Checkout(order);
            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);
            Order orderInRep = orders.Find(order.Id);
            Assert.AreEqual(2,orderInRep.OrderItems.Count);
        }
    }
}
