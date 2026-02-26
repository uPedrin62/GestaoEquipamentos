using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class utilizadores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ObterUtilizadores();
            }
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedDataKey"] != null)
            {
                string idUtilizador = ViewState["SelectedDataKey"].ToString();
                CarregarDadosUtilizador(idUtilizador);
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
                string idUtilizador = ViewState["SelectedDataKey"].ToString();
                CarregarDadosUtilizador(idUtilizador);
                ViewState["opcao"] = "u";
                ViewState["idUtilizador"] = idUtilizador;
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
                    string idUtilizador = ViewState["SelectedDataKey"].ToString();
                    EliminarUtilizador(idUtilizador);
                    ObterUtilizadores();
                    ViewState["SelectedRowIndex"] = null;
                    ViewState["SelectedDataKey"] = null;
                    gridUtilizadores.SelectedIndex = -1;
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
                    InserirUtilizador();
                }
                else if (opcao == "u")
                {
                    AtualizarUtilizador();
                }
                ObterUtilizadores();
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

        protected void gridUtilizadores_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gridUtilizadores.SelectedIndex >= 0)
            {
                ViewState["SelectedRowIndex"] = gridUtilizadores.SelectedIndex;
                ViewState["SelectedDataKey"] = gridUtilizadores.SelectedDataKey.Value;
            }
        }

        void CarregarDadosUtilizador(string idUtilizador)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDUtilizador, Nome, Email, Telefone, Senha FROM Utilizadores WHERE IDUtilizador = @IDUtilizador";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDUtilizador", idUtilizador);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNome.Text = reader["Nome"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtTelefone.Text = reader["Telefone"].ToString();
                            txtSenha.Attributes["placeholder"] = "••••••••";
                            txtSenha.Text = "";
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
        }

        void ObterUtilizadores()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string sql = "SELECT IDUtilizador, Nome, Email, Telefone FROM Utilizadores ORDER BY Nome";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        var reader = cmd.ExecuteReader();
                        var dataTable = new System.Data.DataTable();
                        dataTable.Load(reader);
                        gridUtilizadores.DataSource = dataTable;
                        gridUtilizadores.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao carregar utilizadores: {ex.Message}');", true);
            }
        }

        void InserirUtilizador()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                
                // Gerar ID automaticamente
                string novoID = GerarIDUtilizador(con);
                
                string sql = @"INSERT INTO Utilizadores (IDUtilizador, Nome, Email, Telefone, Senha)
                             VALUES (@IDUtilizador, @Nome, @Email, @Telefone, @Senha)";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDUtilizador", novoID);
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Senha", string.IsNullOrWhiteSpace(txtSenha.Text) ? "123456" : txtSenha.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        string GerarIDUtilizador(SqlConnection con)
        {
            string sql = "SELECT COUNT(*) FROM Utilizadores";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                int count = (int)cmd.ExecuteScalar();
                return "U" + (count + 1).ToString("D3"); // U001, U002, etc
            }
        }

        void AtualizarUtilizador()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string idUtilizador = ViewState["idUtilizador"].ToString();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql;
                if (!string.IsNullOrWhiteSpace(txtSenha.Text))
                {
                    sql = @"UPDATE Utilizadores SET Nome = @Nome, Email = @Email, Telefone = @Telefone, Senha = @Senha
                             WHERE IDUtilizador = @IDUtilizador";
                }
                else
                {
                    sql = @"UPDATE Utilizadores SET Nome = @Nome, Email = @Email, Telefone = @Telefone
                             WHERE IDUtilizador = @IDUtilizador";
                }
                
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text.Trim());
                    cmd.Parameters.AddWithValue("@IDUtilizador", idUtilizador);
                    
                    if (!string.IsNullOrWhiteSpace(txtSenha.Text))
                    {
                        cmd.Parameters.AddWithValue("@Senha", txtSenha.Text.Trim());
                    }
                    
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        void EliminarUtilizador(string idUtilizador)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "DELETE FROM Utilizadores WHERE IDUtilizador = @IDUtilizador";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDUtilizador", idUtilizador);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
