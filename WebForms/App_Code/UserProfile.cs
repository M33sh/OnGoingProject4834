using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebForms.App_Code;

namespace WebForms.App_Code
{
    public class UserProfile : XmlPropertyObject
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string FullName { get; set; }

        [Required]
        [StringLength(70, MinimumLength = 3)]
        public string Address { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string City { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string State { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 7)]
        [RegularExpression(@"^\d{9,10}$")]
        public string Phone { get; set; }
        public string UserName { get { return Name; } set { Name = value; } }
        public string Email { get { return Description; } set { Description = value; } }

        public UserProfile() { }
        public UserProfile(int ID) : base(ID) { }
    }
}