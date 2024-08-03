using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
            if (Session["User"] != null)
            {
                if (Convert.ToInt32(Session["Userrole"]) > 0)
                {
                    var models = (from user in db.users // Access Users DbSet
                                  join tickets in db.tickets on user.ID equals tickets.senderid into ticketsgroup // Join News DbSet
                                  from tickets in ticketsgroup.DefaultIfEmpty() // Left outer join
                                  where tickets != null // Filter based on existence of news record
                                  select new ticketlist
                                  {
                                      username = user != null ? user.usename : null,
                                      ticketID = tickets.ID, // Use null-conditional operator for missing news
                                      ticketTitle = tickets.title,
                                      priority = tickets.priority,
                                      status = tickets.status
                                  }).OrderByDescending(p => p.priority).Where(s => s.status == 1 || s.status ==3).ToList();


                    return View(models);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("Login", "users");

        }
        public ActionResult test()
        {
            return View(db.responses.ToList());
            
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
                response.adminid = Convert.ToInt32(Session["Userid"]);
                var targetTicket = db.tickets.Find(response.ticketid);
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
            var u = db.users.Find(db.tickets.Find(id).senderid);
            ViewBag.sender = u.usename;
            
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
                var targetTicket = db.tickets.Find(response.ticketid);

                if (response.ID == 0)
                {   
                    db.responses.Add(response);
                    targetTicket.status = 2;
                    targetTicket.responsid = response.ID;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }
                else
                {
                    var res = db.responses.Find(response.ID);
                    res.content += "\n" + response.content;
                    targetTicket.status = 2;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }


                
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
