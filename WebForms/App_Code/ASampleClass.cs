using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebForms.App_Code;

namespace WebForms.App_Code
{
    public class ASampleClass : XmlPropertyObject
    {
        public string aProperty1 { get; set; }
        public string aProperty2 { get; set; }

        public ASampleClass() { }

        public ASampleClass(int ID) : base(ID) { }
    }
}