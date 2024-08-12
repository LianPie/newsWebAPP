using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class tagsController : Controller
    {
        private newswebappEntities db = new newswebappEntities();

        // GET: tags
        public ActionResult Index()
        {
            string userrole = Convert.ToString(Session["Userrole"]);

            if (userrole.Contains("tag&catManagment"))
                ViewBag.role = 1;
            else ViewBag.role = 0;

            return View(db.tags.ToList());
        }

        // GET: tags/Details/5
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
            tag tags = db.tags.Find(id);
            ViewBag.title = tags.title;
            if (tags == null)
            {
                return HttpNotFound();
            }
            //search for record that contains "tag title" in tag field.
            return View(db.news.Where(n => n.tag.Contains(tags.title)));
        }

        // GET: tags/Create
        public ActionResult Create()
        {
            if (Session["User"] != null)
            {
                string userrole = Convert.ToString(Session["Userrole"]);

                if (userrole.Contains("tag&catManagment"))
                {
                    return View();
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("index", "home");
        }

        // POST: tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "code,link,title")] tag tag)
        {
            if (ModelState.IsValid)
            {
                tag.link = "tags/"+tag.title;
                db.tags.Add(tag);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tag);
        }

        // GET: tags/Edit/5
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
                    tag tags = db.tags.Find(id);
                    if (tags == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tags);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("index", "home");
        }

        // POST: tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "code,link,title")] tag tag)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tag).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tag);
        }

        // GET: tags/Delete/5
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
                    tag tags = db.tags.Find(id);
                    if (tags == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tags);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("index", "home");
        }

        // POST: tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tag tag = db.tags.Find(id);
            db.tags.Remove(tag);
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
