//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class admin
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public admin()
        {
            this.Meetings = new HashSet<Meeting>();
        }
    
        public int id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string email { get; set; }
        public string Phone { get; set; }
        public string CNIC { get; set; }
        public string Designation { get; set; }
        public string gender { get; set; }
        public string profile_pic { get; set; }
        public string cnic_pic { get; set; }
        public string password { get; set; }
        public string confirm_password { get; set; }

        public int status { get; set; }
        public string otp { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Meeting> Meetings { get; set; }
    }
}
