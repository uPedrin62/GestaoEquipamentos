using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;
using System.Web.UI;

namespace esoEquipamentos26
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            LoginError.Text = "";

            string email = Email.Text.Trim();
            string password = Password.Text;

            // Primeiro tenta autenticar como técnico
            string idTecnico = ValidarTecnico(email, password);
            if (!string.IsNullOrEmpty(idTecnico))
            {
                FormsAuthentication.SetAuthCookie(email, false);
                Session["user"] = idTecnico;
                Session["userType"] = "tecnico";
                Response.Redirect("~/administrador/dashboard.aspx");
                return;
            }

            // Depois tenta autenticar como utilizador
            string idUtilizador = ValidarUtilizador(email, password);
            if (!string.IsNullOrEmpty(idUtilizador))
            {
                FormsAuthentication.SetAuthCookie(email, false);
                Session["user"] = idUtilizador;
                Session["userType"] = "utilizador";
                Response.Redirect("~/administrador/dashboard.aspx");
                return;
            }

            // Se não for técnico nem utilizador, mostra erro
            LoginError.Text = "Email ou senha inválidos.";
        }

        private string ValidarTecnico(string email, string senha)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDTecnico FROM Tecnico WHERE Email = @Email AND Senha = @Senha";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        private string ValidarUtilizador(string email, string senha)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDUtilizador FROM Utilizadores WHERE Email = @Email AND Senha = @Senha";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
    }
}
