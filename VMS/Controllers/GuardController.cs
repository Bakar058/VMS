using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VMS.Controllers
{
    public class GuardController : Controller
    {
        // GET: Guard
       
        public ActionResult Scanner()
        {
            return View();
        }
    }
}