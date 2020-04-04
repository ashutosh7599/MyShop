using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        IRepositroy<Product> context;
        IRepositroy<ProductCategory> productCategories;

        public HomeController(IRepositroy<Product> productContext, IRepositroy<ProductCategory> productCategoryContext)
        {
            context = productContext;
            productCategories = productCategoryContext;
           
        }
        public ActionResult Index(string Category=null)
        {
            List<Product> products;
            List<ProductCategory> categories = productCategories.Collection().ToList();
            if(Category == null)
            {
                products = context.Collection().ToList();
            }
            else
            {
                products = context.Collection().Where(p => p.Category == Category).ToList();
            }
            ProductListViewModel x = new ProductListViewModel();
            x.Product = products;
            x.ProductCategories = categories;
            return View(x);
        }
        public ActionResult Details(string id)
        {
            Product x = context.Find(id);
            if(x == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(x);
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}