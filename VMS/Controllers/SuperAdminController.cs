using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Models;
using VMS.App_Start;
using System.Data.Entity;


namespace VMS.Controllers
{
    [SuperAdminAuthorize]
    public class SuperAdminController : Controller
    {
        VMSEntities db = new VMSEntities();
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string username, string password, string ReturnUrl)
        {
            string username1 = "admin";
            string password1 = "admin@123";
            if (username.Equals(""))
            {
                ModelState.AddModelError("username", "Username Cannot Be Empty!!");
            }
            if (password.Equals(""))
            {
                ModelState.AddModelError("password", "Passowrd Cannot Be Empty!!");
            }

            if (!username.Equals("") && !password.Equals(""))
            {
                bool correct;
                if(username.Equals(username1)&& password.Equals(password1))
                {
                    correct = true;
                }
                else
                {
                    correct = false;
                }
                // var credentials = db.admins.Where(model => model.email == email && model.password == password).FirstOrDefault();
                if (correct == true)
                {


                    Session["SuperAdminusername"] = username;

                    TempData["l_msg"] = "success";
                    HttpCookie cookie = new HttpCookie("SuperAdminCookie", "LoggedIn");
                    cookie.Expires = DateTime.MinValue; ;
                    Response.Cookies.Add(cookie);

                    if (ReturnUrl != null)
                        return Redirect(ReturnUrl);
                    else
                        return RedirectToAction("SuperAdminProfile");
                }
                else
                {
                    TempData["l_msg"] = "fail";

                }


            }

            return View();
        }
        public ActionResult SuperAdminProfile()
        {
            return View();
        }
        public ActionResult GetMeetings()
        {
            var meetings = db.Meetings.ToList();
            return View(meetings);
        }
        public ActionResult GetVisits()
        {
            var visit = db.visits.ToList();
            return View(visit);
        }

        public ActionResult approve(int id, int ch)
        {
            var visit = db.visits.Single(model => model.Id == id);
            if (ch == 0)
            {
                visit.Id = -1;
                db.Entry(visit).State = EntityState.Modified;
                db.SaveChanges();
                TempData["meeting"] = "decline";

            }
            else if (ch == 1)
            {
                visit.approval = 1;

                TempData["meeting"] = "approved";


                db.Entry(visit).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("GetVisits", "SuperAdmin");
        }


        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View();
        }
    }
}