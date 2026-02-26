using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class tecnicos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ObterTecnicos();
            }
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedDataKey"] != null)
            {
                string idTecnico = ViewState["SelectedDataKey"].ToString();
                CarregarDadosTecnico(idTecnico);
                ViewState["opcao"] = "v";
                formularioDados.Visible = true;
                DesabilitarCampos(true);
                buttonGuardar.Visible = false;
                buttonFechar.Visible = true;
                buttonCancelar.Visible = false;
            }
        }

        protected void buttonCriar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            DesabilitarCampos(false);
            buttonGuardar.Visible = true;
            buttonFechar.Visible = false;
            buttonCancelar.Visible = true;
            ViewState["opcao"] = "i";
            formularioDados.Visible = true;
        }

        protected void buttonEditar_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedDataKey"] != null)
            {
                string idTecnico = ViewState["SelectedDataKey"].ToString();
                CarregarDadosTecnico(idTecnico);
                ViewState["opcao"] = "u";
                ViewState["idTecnico"] = idTecnico;
                formularioDados.Visible = true;
                DesabilitarCampos(false);
                buttonGuardar.Visible = true;
                buttonFechar.Visible = false;
                buttonCancelar.Visible = true;
            }
        }

        protected void buttonEliminar_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedDataKey"] != null)
            {
                try
                {
                    string idTecnico = ViewState["SelectedDataKey"].ToString();
                    EliminarTecnico(idTecnico);
                    ObterTecnicos();
                    ViewState["SelectedRowIndex"] = null;
                    ViewState["SelectedDataKey"] = null;
                    gridTecnicos.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao eliminar: {ex.Message}');", true);
                }
            }
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string opcao = ViewState["opcao"].ToString();
                if (opcao == "i")
                {
                    InserirTecnico();
                }
                else if (opcao == "u")
                {
                    AtualizarTecnico();
                }
                ObterTecnicos();
                LimparCampos();
                formularioDados.Visible = false;
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro: {ex.Message}');", true);
            }
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            formularioDados.Visible = false;
        }

        protected void buttonFechar_Click(object sender, EventArgs e)
        {
            formularioDados.Visible = false;
        }

        protected void gridTecnicos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gridTecnicos.SelectedIndex >= 0)
            {
                ViewState["SelectedRowIndex"] = gridTecnicos.SelectedIndex;
                ViewState["SelectedDataKey"] = gridTecnicos.SelectedDataKey.Value;
            }
        }

        void CarregarDadosTecnico(string idTecnico)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDTecnico, Nome, Email, Telefone, Senha FROM Tecnico WHERE IDTecnico = @IDTecnico";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDTecnico", idTecnico);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNome.Text = reader["Nome"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtTelefone.Text = reader["Telefone"].ToString();
                            txtSenha.Text = reader["Senha"].ToString();
                        }
                    }
                }
            }
        }

        void LimparCampos()
        {
            txtNome.Text = "";
            txtEmail.Text = "";
            txtTelefone.Text = "";
            txtSenha.Text = "";
        }

        void DesabilitarCampos(bool desabilitar)
        {
            txtNome.Enabled = !desabilitar;
            txtEmail.Enabled = !desabilitar;
            txtTelefone.Enabled = !desabilitar;
            txtSenha.Enabled = !desabilitar;
        }

        void ObterTecnicos()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string sql = "SELECT IDTecnico, Nome, Email, Telefone FROM Tecnico ORDER BY Nome";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        var reader = cmd.ExecuteReader();
                        var dataTable = new System.Data.DataTable();
                        dataTable.Load(reader);
                        gridTecnicos.DataSource = dataTable;
                        gridTecnicos.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao carregar t&eacute;cnicos: {ex.Message}');", true);
            }
        }

        void InserirTecnico()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                
                // Gerar ID automaticamente
                string novoID = GerarIDTecnico(con);
                
                string sql = @"INSERT INTO Tecnico (IDTecnico, Nome, Email, Telefone, Senha)
                             VALUES (@IDTecnico, @Nome, @Email, @Telefone, @Senha)";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDTecnico", novoID);
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Senha", txtSenha.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        string GerarIDTecnico(SqlConnection con)
        {
            string sql = "SELECT COUNT(*) FROM Tecnico";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                int count = (int)cmd.ExecuteScalar();
                return "T" + (count + 1).ToString("D3"); // T001, T002, etc
            }
        }

        void AtualizarTecnico()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string idTecnico = ViewState["idTecnico"].ToString();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"UPDATE Tecnico SET Nome = @Nome, Email = @Email, Telefone = @Telefone, Senha = @Senha
                             WHERE IDTecnico = @IDTecnico";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Senha", txtSenha.Text.Trim());
                    cmd.Parameters.AddWithValue("@IDTecnico", idTecnico);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        void EliminarTecnico(string idTecnico)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "DELETE FROM Tecnico WHERE IDTecnico = @IDTecnico";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDTecnico", idTecnico);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
