using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using WebForms.App_Code;

namespace WebForms.App_Code
{
    public class Movies : XmlPropertyObject
    {
        public string id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string YearReleased { get; set; }
        public string Rating { get; set; }      
    }
} 