using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        newswebappEntities db = new newswebappEntities();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            ViewBag.Message = "Register page";

            return View();
        }
        public ActionResult Login()
        {
            ViewBag.Message = "Login page";

            return View();
        }

        [HttpPost]
        public ActionResult Login(Login model)
        {
            // Replace this with actual authentication logic
            if (ModelState.IsValid)
            {
                var allusers = db.users.ToList();

                foreach (var user in allusers)
                {
                    if (model.username == user.usename && model.password == user.password)
                    {
                        // Successful login, redirect to home page or desired page
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Register(Register model)
        {
            if (ModelState.IsValid)

            {
                // Replace this with actual user creation logic, including password hashing
                // and storing user information in a database
                /*var allusers = db.users.ToList();

                foreach (var nuser in allusers)
                {
                    if (model.username == nuser.ToString())
                    {
                        ModelState.AddModelError("", "username already exists");
                        break;
                    }
                }*/

                /*db.users.Add(new user
                      {
                          usename = model.username,
                          password = model.password,
                          displayname = model.displayname
                      });
                      db.SaveChanges();
                }*/

                return RedirectToAction("Login");
            }
            return View(model);
        }
    }
}
            