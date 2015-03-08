using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForms.Pages
{
    public partial class MovieGridView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          
        }

        //protected void InsertButton_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("~/Pages/MovieGridView.aspx");
        //}

        protected void FormView1_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/MovieGridView.aspx");
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }        
}