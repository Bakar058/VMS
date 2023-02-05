using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VMS.Models;
using System.Security.Cryptography;
using System.Data.Entity;
using VMS.App_Start;

namespace VMS.Controllers
{

    [UserAuthorize]
    public class UserController : Controller
    {
        private static RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

        // GET: User
        VMSEntities db = new VMSEntities();
        [AllowAnonymous]
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult KeepAlive()
        {
            Session.Timeout = 60;
            return Content("Session has been kept alive.");
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SignUp([Bind(Include = "id,First_Name,Last_Name,email,phone,CNIC,gender,password,confirm_password")] user user, HttpPostedFileBase profile_img, HttpPostedFileBase cnic_img)
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

                    var data = db.users.Where(model => model.email == user.email).FirstOrDefault();
                    if(data != null)
                    {
                        TempData["signup"] = "fail";
                        TempData["s_msg"] = "Email Already In Use";
                        return View();
                    }

                    string filename = user.First_Name + user.Last_Name;
                    string fileExt = Path.GetExtension(profile_img.FileName);



                    if (fileExt.Equals(".png") || fileExt.Equals(".jpg") || fileExt.Equals(".jpeg"))
                    {
                        filename = filename + fileExt;
                        user.profile_pic = "~/userimage/" + filename;
                        filename = Path.Combine(Server.MapPath("~/userimage/"), filename);
                        profile_img.SaveAs(filename);
                    }
                    filename = user.First_Name + user.Last_Name + "  CINC";
                    fileExt = Path.GetExtension(cnic_img.FileName);



                    if (fileExt.Equals(".png") || fileExt.Equals(".jpg") || fileExt.Equals(".jpeg"))
                    {
                        filename = filename + fileExt;
                        user.cnic_pic = "~/userimage/" + filename;
                        filename = Path.Combine(Server.MapPath("~/userimage/"), filename);
                        profile_img.SaveAs(filename);
                    }
                    user.status = 0;

                    string otp = GenerateOTP(6);
                    user.otp= otp;

                    string to = user.email; ;
                    string subject = "Email Varification";
                    string body = "Hello, "+ user.First_Name +" "+user.Last_Name+". Your OTP IS " +otp;
                    sendMail(to, subject, body);


                    db.users.Add(user);
                    var a = db.SaveChanges();
                    
                    if (a > 0)
                    {

                        TempData["signup"] = "success";
                        TempData["s_msg"] = "Account Created Succesfully";
                        return RedirectToAction("VarifyEmail",new {email=user.email});

                    }

                }


              
            }
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
        public ActionResult VarifyEmail(string otp,string email)
        {
            var user = db.users.Where(model => model.email==email && model.otp==otp).FirstOrDefault();
          if(user!= null)
            {
                user.status = 1;
                db.Entry(user).State = EntityState.Modified;
                user.confirm_password = user.password;
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
            if(password.Equals(""))
            {
                ModelState.AddModelError("password", "Passowrd Cannot Be Empty!!");
            }

            if(!email.Equals("") && !password.Equals(""))
            {

                var credentials = db.users.Where(model => model.email == email && model.password == password).FirstOrDefault();
                if (credentials!=null)
                {
                    if (credentials.status == 1)
                    {

                       

                        Session["username"] = credentials.First_Name + "  " + credentials.Last_Name;
                        Session["img"] = credentials.profile_pic;
                        TempData["l_msg"] = "success";
                        Session["user"] = credentials;
                        HttpCookie cookie = new HttpCookie("UserCookie", "LoggedIn");
                        cookie.Expires = DateTime.MinValue;
                        Response.Cookies.Add(cookie);
                       // Session["role"] = "User";

                        if (ReturnUrl != null)
                            return Redirect(ReturnUrl);
                        else
                            return RedirectToAction("UserProfile");
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
            HttpCookie cookie = new HttpCookie("UserCookie", "LoggedIn");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            Session.Remove("username");
            Session.Remove("img");
            Session.Remove("user");

            // Redirect the user to the home page
            return RedirectToAction("Login", "User");
        }

        public ActionResult UserProfile()
        {
            return View();
        }
    
        public ActionResult AddMeeting()
        {
            var admins = db.admins.ToList();
            ViewBag.admins = admins;
            ViewData["admins"] = admins;
            return View();
        }
        [HttpPost]
        public ActionResult AddMeeting(Meeting meeting)
        {
            return View(meeting);
        }
     
      
    }
}