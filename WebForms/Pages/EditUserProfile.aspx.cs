using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms.App_Code;
using System.Web.Security;
using WebForms;


namespace WebForms.Pages
{
    public partial class EditUserProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ObjectID"] != null)
                {
                    int id = Convert.ToInt32(Request.QueryString["ObjectID"]);
                    UserProfile up = new UserProfile(id);

                    tbAddress.Text = up.Address;
                    tbCity.Text = up.City;
                    tbEmail.Text = up.Email;
                    tbFullName.Text = up.FullName;
                    tbPhone.Text = up.Phone;
                    tbState.Text = up.State;
                    up.UserName = Context.User.Identity.Name;
                    btnSave.Visible = false;
                    ButtonUpd.Visible = true;
                    SetFormValues();
                }
                else
                {
                    btnSave.Visible = true;
                    ButtonUpd.Visible = false;
                    BtnReturn.Visible = false;
                    GetFormValues();
                    SetFormValues();
                }
            }
                      
        }
        protected void SetFormValues()
        {
            MembershipUser mu = Membership.GetUser(Context.User.Identity.Name);
            tbEmail.Text = mu.Email;           
        }
        protected void GetFormValues()
        {
            UserProfile up = new UserProfile();
            up.Address = tbAddress.Text;
            up.City = tbCity.Text;
            up.Email = tbEmail.Text;
            up.FullName = tbFullName.Text;
            up.Phone = tbPhone.Text;
            up.State = tbState.Text;
            up.UserName = Context.User.Identity.Name;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            UserProfile up = new UserProfile();
            up.Address = tbAddress.Text;
            up.City = tbCity.Text;
            up.Email = tbEmail.Text;
            up.FullName = tbFullName.Text;
            up.Phone = tbPhone.Text;
            up.State = tbState.Text;
            up.UserName = Context.User.Identity.Name;
            int v = up.Insert();
            Response.Redirect("~/Pages/EditUserProfile.aspx?ObjectID=" + v);
            btnSave.Text = Convert.ToString(v) + " Profile Saved";
            btnCancel.Visible = false;
            ButtonUpd.Visible = true;
            BtnReturn.Visible = true;
            
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DefaultLoggedIn.aspx");
        }

        protected void ButtonUpd_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(Request.QueryString["ObjectID"]);
            UserProfile up = new UserProfile(id);
            up.Address = tbAddress.Text;
            up.City = tbCity.Text;
            up.Email = tbEmail.Text;
            up.FullName = tbFullName.Text;
            up.Phone = tbPhone.Text;
            up.State = tbState.Text;
            up.UserName = Context.User.Identity.Name;

            up.Update();

            ButtonUpd.Text = Convert.ToString(id) + " Profile Updated";
            BtnReturn.Visible = true;
        }

        protected void BtnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DefaultLoggedInc.aspx");
        }
    }
}