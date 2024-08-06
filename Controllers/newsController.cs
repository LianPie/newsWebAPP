using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class newsController : Controller
    {
        private newswebappEntities db = new newswebappEntities();
        public static string GetUserIpAddress(HttpRequestBase request)
        {
            string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = request.ServerVariables["REMOTE_ADDR"];
            }
            return ipAddress;
        }

        // GET: news
        public ActionResult Index()
        {
            ViewBag.role = Convert.ToInt32(Session["Userrole"]);
            ViewBag.uid = Convert.ToInt32(Session["Userid"]);
            var models = (from user in db.users // Access Users DbSet
                          join news in db.news on user.ID equals news.userID into newsGroup // Join News DbSet
                          from news in newsGroup.DefaultIfEmpty() // Left outer join
                          where news != null // Filter based on existence of news record
                          select new newsModel
                          {
                              DisplayName = user != null ? user.displayname : null,
                              userID = user != null ? user.ID : 0,
                              NewsID = news.ID, // Use null-conditional operator for missing news
                              Title = news.title,
                              Image = news.image,
                              category = news.cat,
                              tag = news.tag,
                              date = news.publishDate,
                              views = news.views
                          }).ToList();


            return View(models);
        }
        // GET: news
        public ActionResult Latest()
        {
            return View(db.news.OrderByDescending(v => v.views).ToList());
        }

        // GET: news/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            var user = db.users.Find(news.userID);
            ViewBag.user = user;

            // Get the user's IP address
            string userIpAddress = GetUserIpAddress(Request);

            // Check if this IP address has already viewed this article
            DateTime thresholdDate = DateTime.Now.AddHours(-24);
            bool hasViewed = db.viewlogs.Any(v => v.newsid == id && v.ipadd == userIpAddress && v.viewdate >= thresholdDate);
            if (!hasViewed)
            {
                // Increment the view count
                news.views++;
                db.SaveChanges();

                // Log the view
                db.viewlogs.Add(new viewlog
                {
                    ipadd = userIpAddress,
                    newsid = news.ID,
                    viewdate = DateTime.Now
                });
                db.SaveChanges();
            }

            return View(news);
        
        }

        // GET: news/Create
        public ActionResult Create()
        {
            if (Session["User"] != null)
            {
                setLists();
              //var models=  new Tuple<List<category>, List<tag>>(cat, tags);
                return View();
            }
            if (Session["User"] != null)
            {
                ViewBag.categoryTitle = db.categories.ToList();
                return View();
            }
            else
                return RedirectToAction("login", "users");
        }

        // POST: news/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,title,image,link,summary,cat,tag,publishDate,userID,Content,views")] news news)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    setLists();
                   
                    if (news.title != null && news.link != null && news.cat != null && news.tag != null)
                    {
                        var image = Request.Files[0];
                        if (image.ContentLength > 1024 * 1024)
                        {
                            //check if fields are empty
                            ModelState.AddModelError("imageFile", "File size cannot exceed 1MB.");
                            return RedirectToAction("Index");
                        }

                        // Check file extension
                        string fileExtension = Path.GetExtension(image.FileName).ToLower();
                        //if (fileExtension != ".jpg" || fileExtension != ".jpeg" || fileExtension != ".png")
                        if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(fileExtension))
                        {
                            ModelState.AddModelError("image", "Only JPG, JPEG, and PNG files are allowed.");
                        }
                        else
                        {
                            int length = 10; // Desired length of the random string
                            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                            char[] chars = new char[length];
                            Random rand = new Random();
                            string imgname = "";
                            for (int i = 0; i < length; i++)
                            {
                                chars[i] = allowedChars[rand.Next(0, allowedChars.Length)];
                                imgname += "" + chars[i];
                            }
                            var path = $"~/Media/news/" + imgname + fileExtension;
                            image.SaveAs(Server.MapPath(path));
                            news.image = path;
                        }
                        news.userID = Convert.ToInt32(Session["Userid"]);
                        news.views = 0;
                        news.publishDate = DateTime.Now;


                        db.news.Add(news);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    
                    }
                    else
                    {
                        ModelState.AddModelError("", "no field can be empty");
                    }




                }
            }

            return View(news);
        }

        // GET: news/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["User"] != null)
            {
                if (Convert.ToInt32(Session["Userrole"]) == 1)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    news news = db.news.Find(id);
                    if (news == null)
                    {
                        return HttpNotFound();
                    }
                    else if ( news.userID == Convert.ToInt32(Session["Userid"]) || Convert.ToInt32(Session["Userrole"]) == 1)
                    {
                        return View(news);
                    }
                    else
                        return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("index", "home");
            
        }

        // POST: news/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,title,image,link,summary,cat,tag,publishDate,userID,Content,views")] news news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: news/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["User"] != null)
            {
                if (Convert.ToInt32(Session["Userrole"]) == 1)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    news news = db.news.Find(id);
                    if (news == null)
                    {
                        return HttpNotFound();
                    }
                    else if (news.userID == Convert.ToInt32(Session["Userid"]) || Convert.ToInt32(Session["Userrole"]) == 1)
                    {
                        return View(news);
                    }
                    else
                        return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
                return RedirectToAction("index", "home");
        }

        // POST: news/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            news news = db.news.Find(id);
            db.news.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        void setLists()
        {
            var cat = db.categories.ToList();
            ViewBag.categoryTitle = cat;
            var tags = db.tags.ToList();
            ViewBag.tagTitle = tags;
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
