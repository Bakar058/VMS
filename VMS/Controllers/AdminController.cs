using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VMS.Models;
using VMS.App_Start;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;

namespace VMS.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private static RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

        // GET: Admin
        // GET: User
        VMSEntities db = new VMSEntities();
        [AllowAnonymous]
        public ActionResult SignUp()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SignUp([Bind(Include = "id,First_Name,Last_Name,email,Phone,CNIC,Designation,gender,password,confirm_password")] admin admin, HttpPostedFileBase profile_img, HttpPostedFileBase cnic_img)
        {

            if (ModelState.IsValid)
            {


                if (profile_img == null || cnic_img == null)
                {

                    if (profile_img == null)
                    {
                        ModelState.AddModelError("profile_img", "Profile Image is Required");
                    }
                    if (cnic_img == null)
                    {
                        ModelState.AddModelError("cnic_img", "CNIC Image is Required");

                    }
                    return View();
                }
                else
                {

                    var data = db.admins.Where(model => model.email == admin.email).FirstOrDefault();
                    if (data != null)
                    {
                        TempData["signup"] = "fail";
                        TempData["s_msg"] = "Email Already In Use";
                        return View();
                    }
                    string filename = admin.First_Name + admin.Last_Name;
                    string fileExt = Path.GetExtension(profile_img.FileName);



                    if (fileExt.Equals(".png") || fileExt.Equals(".jpg") || fileExt.Equals(".jpeg"))
                    {
                        filename = filename + fileExt;
                            admin.profile_pic = "~/adminimages/" + filename;
                        filename = Path.Combine(Server.MapPath("~/adminimages/"), filename);
                        profile_img.SaveAs(filename);
                    }
                    filename = admin.First_Name + admin.Last_Name + "  CINC";
                    fileExt = Path.GetExtension(cnic_img.FileName);



                    if (fileExt.Equals(".png") || fileExt.Equals(".jpg") || fileExt.Equals(".jpeg"))
                    {
                        filename = filename + fileExt;
                        admin.cnic_pic = "~/adminimages/" + filename;
                        filename = Path.Combine(Server.MapPath("~/adminimages/"), filename);
                        profile_img.SaveAs(filename);
                    }
                    admin.status = 0;

                    string otp = GenerateOTP(6);
                    admin.otp = otp;

                    string to = admin.email; ;
                    string subject = "Email Varification";
                    string body = "Hello, " + admin.First_Name + " " + admin.Last_Name + ". Your OTP IS " + otp;
                    sendMail(to, subject, body);

                    db.admins.Add(admin);
                    var a = db.SaveChanges();
                    if (a > 0)
                    {

                        TempData["signup"] = "success";
                        TempData["s_msg"] = "Account Created  Succesfully";
                        return RedirectToAction("VarifyEmail", new { email = admin.email });

                    }

                }



            }


            //  return RedirectToAction("Login");
            return View();
        }
        private static string GenerateOTP(int length)
        {
            const string chars = "0123456789";
            byte[] data = new byte[length];
            _rng.GetBytes(data);
            return new string(data.Select(b => chars[b % chars.Length]).ToArray());
        }
        void sendMail(string to, string subject, string body)
        {
            var credentials = new NetworkCredential("contactus@codesever.com", "contactuscodesever4321@$");

            // Create the message
            var message = new MailMessage()
            {
                From = new MailAddress("contactus@codesever.com"),
                Subject = subject,
                Body = body
            };

            message.To.Add(new MailAddress(to));

            // Send the message
            using (var client = new SmtpClient())
            {
                client.Credentials = credentials;
                client.Host = "smtp.titan.email";
                client.Port = 587; // use port 587 with STARTTLS
                client.EnableSsl = true; // ensure security
                client.Send(message);
            }
        }
        [AllowAnonymous]
        public ActionResult VarifyEmail(string email)
        {
            TempData["e"] = email;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult VarifyEmail(string otp, string email)
        {
            var admin = db.admins.Where(model => model.email == email && model.otp == otp).FirstOrDefault();
            if (admin != null)
            {
                admin.status = 1;
                db.Entry(admin).State = EntityState.Modified;
                admin.confirm_password = admin.password;
                db.SaveChanges();
                TempData["signup"] = "success";
                TempData["s_msg"] = "Email Varified Succesfully";
                return RedirectToAction("Login");

            }
            else
            {
                ModelState.AddModelError("otp", "InValid OTP. Please Enter Valid OTP!!!!!");
                return View();
            }

        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string email, string password, string ReturnUrl)
        {
            if (email.Equals(""))
            {
                ModelState.AddModelError("email", "Email Cannot Be Empty!!");
            }
            if (password.Equals(""))
            {
                ModelState.AddModelError("password", "Passowrd Cannot Be Empty!!");
            }

            if (!email.Equals("") && !password.Equals(""))
            {

                var credentials = db.admins.Where(model => model.email == email && model.password == password).FirstOrDefault();
                if (credentials != null)
                {
                    if (credentials.status == 1)
                    {

                        Session["Adminusername"] = credentials.First_Name + "  " + credentials.Last_Name;
                        Session["admin"] = credentials;
                        Session["Adminimg"] = credentials.profile_pic;
                        TempData["l_msg"] = "success";
                        HttpCookie cookie = new HttpCookie("AdminCookie", "LoggedIn");
                        cookie.Expires = DateTime.MinValue; ;
                        Response.Cookies.Add(cookie);

                        if (ReturnUrl != null)
                            return Redirect(ReturnUrl);
                        else
                            return RedirectToAction("AdminProfile");
                    }
                    else
                    {

                        ViewBag.email = email;
                        TempData["l_msg"] = "notverified";
                    }
                }
                else
                {


                    TempData["l_msg"] = "fail";
                }
            }

            return View();
        }
        public ActionResult Logout()
        {
            // Delete the authentication cookie
            HttpCookie cookie = new HttpCookie("AdminCookie", "LoggedIn");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            Session.Remove("Adminusername");
            Session.Remove("Adminimg");

            // Redirect the user to the home page
            return RedirectToAction("Login", "Admin");
        }

        public ActionResult AdminProfile()
        {
            return View();
        }
        public ActionResult GetMeetings(int id)
        {
            var meetings = db.Meetings.Where(model => model.admin_id == id);
            return View(meetings);
        }
        public ActionResult approve(int id, int ch)
        {
            var meeting = db.Meetings.Single(model => model.id == id);
            if (ch == 0)
            {
                meeting.approval = -1;
                db.Entry(meeting).State = EntityState.Modified;
                db.SaveChanges();
                TempData["meeting"] = "decline";

            }
            else if (ch == 1)
            {
                meeting.approval = 1;
               
                TempData["meeting"] = "approved";
               

                db.Entry(meeting).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("GetMeetings", "Admin", new {id=meeting.admin_id});
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string id, string old_password, string password)
        {

            int u_id = Convert.ToInt32(id);
            var user = db.users.Where(model => model.Id == u_id && model.password == old_password).FirstOrDefault();
            if (user != null)
            {
                user.password = password;
                user.confirm_password = password;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["p_change"] = "success";
            }
            else
            {
                TempData["p_change"] = "fail";
            }
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }
    }

}
