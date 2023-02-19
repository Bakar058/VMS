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
using ZXing;
using ZXing.Common;

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
        public ActionResult AddMeeting([Bind(Include ="id,user_id,admin_id,meeting_date,time_start,time_end,duration,purpose")]Meeting meeting)
        {
            if (ModelState.IsValid)
            {

                meeting.qrcode = GenerateQRCode(meeting, 400, 400);
                meeting.status = 0;
                meeting.approval = 0;
                db.Meetings.Add(meeting);
                var a=db.SaveChanges();
                if (a > 0)
                {
                    TempData["meeting_added"] = "success";
                    TempData["meeting_msg"] = "Meeting Is Added Succesfully. Wait For The Approvel";
                    return RedirectToAction("UserProfile");
                }
                else
                {
                    TempData["meeting_added"] = "error";
                    TempData["meeting_msg"] = "Meeting Is Not Added. Something Went Wrong";
                    return View();
                }
                
            }
            return View();
           
        }

        public string GenerateQRCode(Meeting meeting, int width, int height)
        {
            string data = meeting.id.ToString() + " \n"+ meeting.user_id.ToString()+" \n"+meeting.admin_id.ToString() + " \n"+ meeting.meeting_date.ToLongDateString().ToString() + " \n"+ meeting.duration.ToString() + " \n";
            
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height
                }
            };
          
            var bitmap = barcodeWriter.Write(data);
            var fileName = "Meeting"+meeting.id.ToString()+meeting.meeting_date.Day+"-"+meeting.meeting_date.Month+"-"+meeting.meeting_date.Year+meeting.time_start.Hours+"-"+meeting.time_start.Minutes+".png" ;
            string path = "~/Meetingimages/" + fileName;
            var filePath = Path.Combine(Server.MapPath("~/Meetingimages"), fileName);
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            return path;
           // return File(filePath, "image/png");
        }
        public ActionResult GetMeetings(int id)
        {
            var meetings = db.Meetings.Where(model => model.user_id == id);
            return View(meetings);
        }
        public ActionResult MeetingDetail(int id)
        {
            /* var meetingData = from m in db.Meetings
                               join u1 in db.users on m.user_id equals u1.Id // retrieve the user who created the meeting
                               join f in db.admins on m.admin_id equals f.id
                               where m.id == id // filter the result set based on MeetingId
                               select new
                               {
                                   MeetingId = m.id,
                                   date = m.meeting_date,
                                   time_start = m.time_start,
                                   time_end = m.time_end,
                                   duration =m.duration,
                                   status = m.status,
                                   qrcode =m.qrcode,
                                   purpose = m.Purpose,
                                   username=u1.First_Name + " "+u1.Last_Name,
                                   useremail= u1.email,
                                   facultyname=f.First_Name+" "+f.Last_Name,
                                   facultyemail=f.email,
                                   facultydesignation=f.Designation

                               };*/

            // retrieve the first (and only) result
            //  var meeting = meetingData.FirstOrDefault();

            Meetindetails md = new Meetindetails();

          md.meeting = db.Meetings.Where(model => model.id == id).FirstOrDefault();
            md.admin = db.admins.Where(model => model.id == md.meeting.admin_id).FirstOrDefault();
            md.user = db.users.Where(model => model.Id == md.meeting.user_id).FirstOrDefault();
            return View(md);
        }
        public ActionResult ChangeStatus(int id,int ch)
        {
            var meeting = db.Meetings.Where(model=>model.id== id).FirstOrDefault(); 
            if (ch == 1)
            {
                meeting.status = -1;
                db.Entry(meeting).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("MeetingDetail", new { id = meeting.id });
        }
        public ActionResult check(DateTime date,int id)
        {
            int count = db.Meetings.Where(model => model.meeting_date == date && model.admin_id==id).Count();
           // Session["c"] = count;
         //   count = 10;
            if (count>1)
            {
                return Json(new { success = false});
            }
            else
            {
                return Json(new { success = true });
            }
        }
        public ActionResult checktime(DateTime date, int id,string time)
        {
            var enteredTime = TimeSpan.Parse(time);

            // Query the database to retrieve the e
            int count = db.Meetings.Where(model => model.meeting_date == date && model.admin_id == id && model.time_start <= enteredTime && model.time_end >= enteredTime).Count();
            // Session["c"] = count;
            //   count = 10;
            if (count > 0)
            {
                return Json(new { success = false });
            }
            else
            {
                return Json(new { success = true });
            }
        }
        public ActionResult Search(string search )
        {
            db.Configuration.ProxyCreationEnabled = false;
            var results = db.admins.Where(x => x.First_Name.Contains(search) || x.Designation.Contains(search)).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string id,string old_password, string password)
        {

            int u_id = Convert.ToInt32(id);
            var user = db.users.Where(model => model.Id == u_id && model.password == old_password).FirstOrDefault();
            if(user != null)
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
    }
}