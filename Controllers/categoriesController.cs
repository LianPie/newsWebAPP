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
    public class categoriesController : Controller
    {
        private newswebappEntities db = new newswebappEntities();

        // GET: categories
        public ActionResult Index()
        {
            string userrole = Convert.ToString(Session["Userrole"]);

            if (userrole.Contains("tag&catManagment"))
                ViewBag.role = 1;
            else ViewBag.role = 0;
            return View(db.categories.ToList());
        }

        // GET: categories/Details/5
        public ActionResult Details(int? id)
        {
            string userrole = Convert.ToString(Session["Userrole"]);

            if (userrole.Contains("tag&catManagment"))
                ViewBag.role = 1;
            else ViewBag.role = 0;

            ViewBag.uid = Convert.ToInt32(Session["Userid"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.categories.Find(id);
            ViewBag.title = category.title;
            if (category == null)
            {
                return HttpNotFound();
            }
            //search for record that contains "cat title" in cat field.
            return View(db.news.Where(n => n.cat.Contains(category.title)));
        }

        // GET: categories/Create
        public ActionResult Create()
        {
            if (Session["User"] != null)
            {
                if (Convert.ToInt32(Session["Userrole"]) > 0)
                {
                    return View();
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("index","home");
        }

        // POST: categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,link,title")] category category)
        {
            if (ModelState.IsValid)
            {
                category.link = "categories/"+category.title;
                db.categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["User"] != null)
            {
                string userrole = Convert.ToString(Session["Userrole"]);

                if (userrole.Contains("tag&catManagment"))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    category category = db.categories.Find(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    return View(category);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("index", "home");
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,link,title")] category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["User"] != null)
            {
                string userrole = Convert.ToString(Session["Userrole"]);

                if (userrole.Contains("tag&catManagment"))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    category category = db.categories.Find(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    return View(category);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("index", "home");
        }

        // POST: categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            category category = db.categories.Find(id);
            db.categories.Remove(category);
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
