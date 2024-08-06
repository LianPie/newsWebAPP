using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Controllers
{
    public class usersController : Controller
    {
        private newswebappEntities db = new newswebappEntities();

        private void ValidatePassword(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Password cannot be empty");
            }
            else
            {
                if (password != confirmPassword)
                {
                    ModelState.AddModelError("", "Passwords don't match");
                }

                if (password.Length < 8)
                {
                    ModelState.AddModelError("", "Password must be at least 8 characters long");
                }

                if (!password.Any(char.IsUpper))
                {
                    ModelState.AddModelError("", "Password must contain at least one uppercase letter");
                }

                if (!password.Any(char.IsLower))
                {
                    ModelState.AddModelError("", "Password must contain at least one lowercase letter");
                }

                if (!password.Any(char.IsDigit))
                {
                    ModelState.AddModelError("", "Password must contain at least one digit");
                }

                if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                {
                    ModelState.AddModelError("", "Password must contain at least one special character");
                }
            }
        }

        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("", "Username cannot be empty");
            }
            else
            {
                if (username.Length < 3)
                {
                    ModelState.AddModelError("", "Username must be at least 3 characters long");
                }

                if (username.Length > 20)
                {
                    ModelState.AddModelError("", "Username must not exceed 20 characters");
                }

                if (!username.All(char.IsLetterOrDigit))
                {
                    ModelState.AddModelError("", "Username can only contain letters and digits");
                }
            }

        }


        // GET: users
        public ActionResult Index()
        {   if (Convert.ToInt32(Session["Userrole"]) > 0)
            {
                @ViewBag.role = "admin";
            }
            else
            {
                @ViewBag.role = "user";
            }

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
                if (Convert.ToInt32(Session["Userrole"]) == 1)
                {
                    return View(db.users.ToList());
                }
                else { 
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
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
        public ActionResult Register([Bind(Include = "ID,usename,password,displayname,role")] user user)
        {   
            if (ModelState.IsValid)
            {
                ValidatePassword(user.password, Request.Form["ConfirmPassword"]);
                ValidateUsername(user.usename);
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
                        // Hash the password
                        using (var sha256 = SHA256.Create())
                        {
                            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.password));
                            user.password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                        }



                        db.users.Add(user);
                        db.SaveChanges();

                        //session start
                        Session["User"] = user.usename;
                        Session["Userid"] = user.ID;
                        Session["Userrole"] = user.role;

                        return RedirectToAction("Index");
                    }
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
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.password));
                    user.password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
                if (existingUser != null && existingUser.password == user.password)
                {
                    //session start
                    Session["User"] = existingUser.usename;
                    Session["Userid"] = existingUser.ID;
                    Session["Userrole"] = existingUser.role;

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
        public ActionResult Edit([Bind(Include = "ID,usename,password,displayname,role")] user user)
        {
            if (ModelState.IsValid)
            {
                var u = db.users.Find(user.ID);

                if (user.password != null){
                    // Hash the password
                    using (var sha256 = SHA256.Create())
                    {
                        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.password));
                        user.password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                        u.password = user.password;
                    }
                }

                u.displayname = user.displayname;

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
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
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
