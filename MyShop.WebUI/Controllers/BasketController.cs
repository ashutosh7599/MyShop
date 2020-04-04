using MyShop.Core.Contracts;
using MyShop.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IRepositroy<Customer> customers;
        IBasketService basketService;
        IOrderService orderService;
        public BasketController(IBasketService basketService,IOrderService OrderService,IRepositroy<Customer> c)
        {
            this.basketService = basketService;
            this.orderService = OrderService;
            this.customers = c;
        }
        
        public ActionResult Index()
        {
            var model = basketService.GetBasketItems(this.HttpContext);
            
            return View(model);
        }
        public JsonResult AddToBasket(string Id)
        {
            basketService.AddToBasket(this.HttpContext, Id);
            return Json("Success");
        }
        public ActionResult RemoveFromBasket(string Id)
        {
            basketService.RemoveFromBasket(this.HttpContext, Id);
            return RedirectToAction("Index");

        }

        public JsonResult BasketSummary()
        {
            var basketSummary = basketService.GetBasketSummary(this.HttpContext);
            
            return Json(basketSummary);

        }


        [Authorize]
        public ActionResult Checkout()
        {
            Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);
            if (customer != null)
            {
                Order order = new Order()
                {
                    Email = customer.Email,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    State = customer.State,
                    ZipCode = customer.ZipCode,
                    Street = customer.Street,
                    City = customer.City

                };
                return View(order);

            }
            else
                return RedirectToAction("Error");
        }
        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order)
        {
            order.Email = User.Identity.Name;
            var basketItems = basketService.GetBasketItems(this.HttpContext);
            order.OrderStatus = "Ordered Created";
            order.OrderStatus = "Payment Processed";
            orderService.CreateOrder(order, basketItems);
            basketService.ClearBasket(this.HttpContext);
            return RedirectToAction("Thankyou", new { OrderId = order.Id });

        }
        public ActionResult Thankyou(string OrderId)
        {
            ViewBag.OrderId = OrderId;
            return View();
        }
    }
}