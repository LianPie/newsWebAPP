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
using System.Web.UI.WebControls;

namespace WebApplication1.Controllers
{
    public class usersController : Controller
    {
        private newswebappEntities db = new newswebappEntities();

        private int ValidatePassword(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Password cannot be empty");
                return 1;

            }
            else
            {
                if (password != confirmPassword)
                {
                    ModelState.AddModelError("", "Passwords don't match");
                    return 1;

                }

                if (password.Length < 8)
                {
                    ModelState.AddModelError("", "Password must be at least 8 characters long");
                    return 1;
                }

                if (!password.Any(char.IsUpper))
                {
                    ModelState.AddModelError("", "Password must contain at least one uppercase letter");
                    return 1;
                }

                if (!password.Any(char.IsLower))
                {
                    ModelState.AddModelError("", "Password must contain at least one lowercase letter");
                    return 1;
                }

                if (!password.Any(char.IsDigit))
                {
                    ModelState.AddModelError("", "Password must contain at least one digit");
                    return 1;
                }

                if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                {
                    ModelState.AddModelError("", "Password must contain at least one special character");
                    return 1;
                }

                return 0;
            }
        }

        private int ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("", "Username cannot be empty");
                return 1;
            }
            else
            {
                if (username.Length < 3)
                {
                    ModelState.AddModelError("", "Username must be at least 3 characters long");
                    return 1;
                }

                if (username.Length > 20)
                {
                    ModelState.AddModelError("", "Username must not exceed 20 characters");
                    return 1;
                }

                if (!username.All(char.IsLetterOrDigit))
                {
                    ModelState.AddModelError("", "Username can only contain letters and digits");
                    return 1;
                }
                return 0;
            }

        }


        // GET: users
        public ActionResult Index() {

            string userrole = Convert.ToString(Session["Userrole"]);
            if (userrole.Contains("addNews"))
            {
                @ViewBag.role = "jornalist";
            }
            if (userrole.Contains("newsManagement") || userrole.Contains("tag&catManagment") )
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
                string userrole = Convert.ToString(Session["Userrole"]);
                if (userrole.Contains("userManagment"))
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
                var cpass = Request.Form["ConfirmPassword"];
                var valpass = ValidatePassword(user.password, cpass);
                var valuser = ValidateUsername(user.usename);
                if (valpass == 1 || valuser == 1)
                {
                    return View();
                }
                //check if the username exists
                var check = db.users.FirstOrDefault(u => u.usename == user.usename);
                //checks username and password field in form
                if (user.usename != null && user.password != null)
                {
                    user.role = "none";

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

        public ActionResult Edituser(int? id)
        {
            if (Session["User"] != null)
            {
                string userrole = Convert.ToString(Session["Userrole"]);

                if (userrole.Contains("userManagment"))
                {

                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    user user = db.users.Find(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    return View(user);
                }
                else
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            }
            else
                return RedirectToAction("Login");
        }
           

        // POST: tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edituser([Bind(Include = "ID,usename,password,displayname,role")] user user, List<string> roles)
        {
            if (ModelState.IsValid)
            {
                user.role = "";
                foreach (string r in roles)
                {
                    if (r != roles[0])
                    {
                        user.role += ",";
                        user.role += r;
                    }
                    else
                        user.role += r;
                }
                var currentuser = db.users.Find(user.ID);
                currentuser.displayname = user.displayname;
                currentuser.role = user.role;
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
