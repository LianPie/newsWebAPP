using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TheNewsController : Controller
    {
        // GET: TheNews
        public ActionResult submit()
        {
            return View();
        }
    }
}