using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForms
{
    public partial class _DefaultLoggedInc : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Application, Server, Session, Request
            if (!Context.User.Identity.IsAuthenticated) Response.Redirect("Default.aspx");
           
            if (!IsPostBack)
                complete.Text= "Profile Saved";                
        }
    }
}