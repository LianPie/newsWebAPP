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
    public class responsesController : Controller
    {
        private newswebappEntities db = new newswebappEntities();

        // GET: responses
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["Userrole"]) > 0)
            {
                return View(db.tickets.OrderByDescending(p => p.priority).Where(s=> s.status == 1).ToList()); 
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
                return RedirectToAction("Login","users");
            
        }

        // GET: responses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            response response = db.responses.Find(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        // GET: responses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: responses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,title,content,adminid,ticketid")] response response)
        {
            if (ModelState.IsValid)
            {
               
                db.responses.Add(response);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(response);
        }

        // GET: responses/Edit/5
        public ActionResult Respond(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.targetTicket = db.tickets.Find(id);
            ViewBag.sender = db.users.Find(db.tickets.Find(id).senderid);
            
            if (ViewBag.targetTicket == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: responses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Respond([Bind(Include = "ID,title,content,adminid,ticketid")] response response)
        {
            if (ModelState.IsValid)
            {
                response.adminid = Convert.ToInt32(Session["Userid"]);
                //wtf
                db.responses.Add(response);
                db.SaveChanges();

                var targetTicket= db.tickets.Find(response.ticketid);
                targetTicket.status = 0;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(response);
        }

        // GET: responses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            response response = db.responses.Find(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        // POST: responses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            response response = db.responses.Find(id);
            db.responses.Remove(response);
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
