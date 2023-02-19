using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Models
{
    public class Meetindetails
    {
        public admin admin { get; set; }
        public Meeting meeting { get; set; }
        public user user { get; set; }
    }
}