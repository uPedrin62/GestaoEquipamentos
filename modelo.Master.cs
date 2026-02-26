using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace esoEquipamentos26
{
    public partial class modelo : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnSair_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("~/login.aspx", false);
        }

        protected void LoginStatus1_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            Session.Clear(); 
            Session.Abandon();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("~/login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        public void MostrarMensagem(string mensagem, string tipo)
        {
            // Usar ScriptManager para mostrar mensagem JavaScript
            string script = $"alert('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

        internal void LimparCampos()
        {

            foreach (Control control in MainContent.Controls)
            {
                if (control is TextBox)
                    ((TextBox)control).Text = string.Empty;
                if (control is DropDownList)
                    ((DropDownList)control).SelectedIndex = 0;
                if (control is CheckBox)
                    ((CheckBox)control).Checked = false;
                if (control is GridView)
                    ((GridView)control).SelectedIndex = -1;
            }
        }
    }
}