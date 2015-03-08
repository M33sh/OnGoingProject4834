using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms.App_Code;

namespace WebForms.Pages
{
    public partial class ASamplePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ObjectID"] != null)
                {
                    int id = Convert.ToInt32(Request.QueryString["ObjectID"]);
                    ASampleClass ac = new ASampleClass(id);

                    TextBox1.Text = ac.aProperty1;
                    TextBox2.Text = ac.aProperty2;
                    ButtonSave.Visible = false;
                    ButtonUpdate.Visible = true;
                }
                else
                {
                    ButtonSave.Visible = true;
                    ButtonUpdate.Visible = false;
                }
            }

        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            ASampleClass ac = new ASampleClass();
            ac.aProperty1 = TextBox1.Text;
            ac.aProperty2 = TextBox2.Text;
            int v = ac.Insert();
            Label3.Text = Convert.ToString(v) + " Inserted";
        }

        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            
            int id = Convert.ToInt32(Request.QueryString["ObjectID"]);
            ASampleClass ac = new ASampleClass(id);
            ac.aProperty1 = TextBox1.Text;
            ac.aProperty2 = TextBox2.Text;
            ac.Update();
            Label3.Text = Convert.ToString(id) + " Updated";
        }
    }
}