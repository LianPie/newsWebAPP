using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class usersController : Controller
    {
        private newswebappEntities db = new newswebappEntities();

        // GET: users
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                if (Session["Userid"] == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                user user = db.users.Find(Session["Userid"]);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
                return RedirectToAction("Login");
        }// GET: users
        public ActionResult allusers()
        {
            if (Session["User"] != null)
            {
                return View(db.users.ToList());
            }
            else
                return RedirectToAction("Login");
        }

        //registeration

        // GET: users/Create
        public ActionResult Register()
        {
            return View();
        }

        // POST: users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "ID,usename,password,displayname")] user user)
        {
            if (ModelState.IsValid)
            {

                //check if the username exists
                var check = db.users.FirstOrDefault(u => u.usename == user.usename);
                //checks username and password field in form
                if (user.usename != null && user.password != null)
                {
                    if (check != null)
                    {
                        ModelState.AddModelError("", " username already exists");
                    }
                    //if username is valid create user add to db
                    else
                    {
                        db.users.Add(user);
                        db.SaveChanges();

                        //session start
                        Session["User"] = user.usename;
                        Session["Userid"] = user.ID;

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("",
                        "username and password field can not be empty");
                }
            }
            return View(user);
        }

        // GET: users/logout
        public ActionResult Logout()
        {
            //session check, if exists -> destroy session
            if (Session["User"] != null)
            {
                Session.Abandon();
            }
            return RedirectToAction("Index");
        }

        // GET: users/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "usename,password")] user user)
        {
            if (ModelState.IsValid)
            {
                //check if user exists
                var existingUser = db.users.FirstOrDefault(u => u.usename == user.usename );
                if (existingUser != null && existingUser.password == user.password)
                {
                    //session start
                    Session["User"] = existingUser.usename;
                    Session["Userid"] = existingUser.ID;

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            return View(user);
        }

        // GET: users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["User"] != null)
            {

                if (Session["Userid"] == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                user user = db.users.Find(Session["Userid"]);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
                return View("Login");
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,usename,password,displayname")] user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        

        // GET: users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["User"] != null)
            {
                if (Session["Userid"] == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                user user = db.users.Find(Session["Userid"]);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
                return View("Login");
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed()
        {
            user user = db.users.Find(Session["Userid"]);
            db.users.Remove(user);
            db.SaveChanges();
            return Logout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
