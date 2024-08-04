using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        newswebappEntities db = new newswebappEntities();

        public ActionResult Index()
        {
            ViewBag.newsCount = db.news.Count();
            ViewBag.views = db.viewlogs.Where(v => DbFunctions.TruncateTime(v.viewdate) == DateTime.Today).Count();
            setTitle();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            setTitle();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            setTitle();
            return View();
        }

        void setTitle()
        {
            ViewBag.titlemenu = "hi user";
        }
    }
}