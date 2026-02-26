using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace esoEquipamentos26.administrador
{
    public partial class locais : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarLocais();
            }
        }

        private void CarregarLocais()
        {
            
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT IDLocal, Nome, Descricao, Prioridade,
                               CASE 
                                   WHEN Nome LIKE 'Sala [0-9]%' OR Nome LIKE 'Sala [1-2][0-9]' THEN 'Salas'
                                   WHEN Nome LIKE 'Sala LF%' OR Nome LIKE 'Sala LQ%' OR Nome LIKE 'Sala LM%' OR Nome LIKE 'Sala D%' OR Nome LIKE 'Salas INF%' OR Nome LIKE 'Salas MEC%' OR Nome = 'Sala 6A' THEN 'Laboratorios'
                                   WHEN Nome IN ('Sala dos Professores', 'Reprografia (Repo)', 'Papelaria', 'Secretaria') THEN 'Administrativos'
                                   WHEN Nome IN ('Cantina', 'Entrada', 'Bar', 'Biblioteca') THEN 'Comuns'
                                   WHEN Nome IN ('Pavilhão de Educação Física', 'Auditório') THEN 'Especiais'
                                   ELSE 'Outros'
                               END as Categoria
                        FROM Local";

                    // Aplicar filtros
                    List<string> filtros = new List<string>();
                    
                    if (!string.IsNullOrEmpty(listCategoria.SelectedValue))
                    {
                        string categoria = listCategoria.SelectedValue;
                        switch (categoria)
                        {
                            case "Salas":
                                filtros.Add("(Nome LIKE 'Sala [0-9]%' OR Nome LIKE 'Sala [1-2][0-9]')");
                                break;
                            case "Laboratorios":
                                filtros.Add("(Nome LIKE 'Sala LF%' OR Nome LIKE 'Sala LQ%' OR Nome LIKE 'Sala LM%' OR Nome LIKE 'Sala D%' OR Nome LIKE 'Salas INF%' OR Nome LIKE 'Salas MEC%' OR Nome = 'Sala 6A')");
                                break;
                            case "Administrativos":
                                filtros.Add("Nome IN ('Sala dos Professores', 'Reprografia (Repo)', 'Papelaria', 'Secretaria')");
                                break;
                            case "Comuns":
                                filtros.Add("Nome IN ('Cantina', 'Entrada', 'Bar', 'Biblioteca')");
                                break;
                            case "Especiais":
                                filtros.Add("Nome IN ('Pavilhão de Educação Física', 'Auditório')");
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(listPrioridade.SelectedValue))
                    {
                        filtros.Add("Prioridade = " + listPrioridade.SelectedValue);
                    }

                    if (filtros.Count > 0)
                    {
                        query += " WHERE " + string.Join(" AND ", filtros);
                    }

                    query += " ORDER BY Nome";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gridLocais.DataSource = dt;
                    gridLocais.DataBind();
                }
            
           
        }

        protected void listCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarLocais();
        }

        protected void listPrioridade_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarLocais();
        }

        protected void gridLocais_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implementar se necessário
        }

        protected void gridLocais_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;
                Label lblPrioridade = (Label)e.Row.FindControl("lblPrioridade");

                if (lblPrioridade != null)
                {
                    byte prioridade = Convert.ToByte(drv["Prioridade"]);
                    switch (prioridade)
                    {
                        case 1:
                            lblPrioridade.Text = "Baixa";
                            lblPrioridade.CssClass = "badge badge-prioridade-1";
                            break;
                        case 2:
                            lblPrioridade.Text = "Normal";
                            lblPrioridade.CssClass = "badge badge-prioridade-2";
                            break;
                        case 3:
                            lblPrioridade.Text = "Alta";
                            lblPrioridade.CssClass = "badge badge-prioridade-3";
                            break;
                        case 4:
                            lblPrioridade.Text = "Crítica";
                            lblPrioridade.CssClass = "badge badge-prioridade-4";
                            break;
                        default:
                            lblPrioridade.Text = "Normal";
                            lblPrioridade.CssClass = "badge bg-secondary";
                            break;
                    }
                }
            }
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            if (gridLocais.SelectedIndex >= 0)
            {
                int idLocal = Convert.ToInt32(gridLocais.DataKeys[gridLocais.SelectedIndex].Value);
                CarregarLocalParaVisualizacao(idLocal);
            }
            
        }

        protected void buttonCriar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            formularioDados.Visible = true;
            buttonGuardar.Text = "Guardar";
        }

        protected void buttonEditar_Click(object sender, EventArgs e)
        {
            if (gridLocais.SelectedIndex >= 0)
            {
                int idLocal = Convert.ToInt32(gridLocais.DataKeys[gridLocais.SelectedIndex].Value);
                CarregarLocalParaEdicao(idLocal);
                formularioDados.Visible = true;
                buttonGuardar.Text = "Atualizar";
            }
            
        }

        protected void buttonEliminar_Click(object sender, EventArgs e)
        {
            if (gridLocais.SelectedIndex >= 0)
            {
                int idLocal = Convert.ToInt32(gridLocais.DataKeys[gridLocais.SelectedIndex].Value);
                EliminarLocal(idLocal);
            }
            
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
               
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd;

                        if (gridLocais.SelectedIndex < 0)
                        {
                            // Criar novo local
                            cmd = new SqlCommand("INSERT INTO Local (Nome, Descricao, Prioridade) VALUES (@Nome, @Descricao, @Prioridade)", conn);
                        }
                        else
                        {
                            // Atualizar local
                            int idLocal = Convert.ToInt32(gridLocais.DataKeys[gridLocais.SelectedIndex].Value);
                            cmd = new SqlCommand("UPDATE Local SET Nome = @Nome, Descricao = @Descricao, Prioridade = @Prioridade WHERE IDLocal = @IDLocal", conn);
                            cmd.Parameters.AddWithValue("@IDLocal", idLocal);
                        }

                        cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                        cmd.Parameters.AddWithValue("@Descricao", string.IsNullOrEmpty(txtDescricao.Text.Trim()) ? (object)DBNull.Value : txtDescricao.Text.Trim());
                        cmd.Parameters.AddWithValue("@Prioridade", Convert.ToByte(ddlPrioridade.SelectedValue));

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            string mensagem = gridLocais.SelectedIndex < 0 ? "Local adicionado com sucesso!" : "Local atualizado com sucesso!";
                            
                            LimparFormulario();
                            CarregarLocais();
                            formularioDados.Visible = false;
                        }
                        
                    }
                
            }
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            formularioDados.Visible = false;
        }

        protected void buttonFechar_Click(object sender, EventArgs e)
        {
            formularioDados.Visible = false;
        }

        private void CarregarLocalParaVisualizacao(int idLocal)
        {
            
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT IDLocal, Nome, Descricao, Prioridade FROM Local WHERE IDLocal = @IDLocal";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IDLocal", idLocal);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtNome.Text = reader["Nome"].ToString();
                        txtDescricao.Text = reader["Descricao"] != DBNull.Value ? reader["Descricao"].ToString() : "";
                        ddlPrioridade.SelectedValue = reader["Prioridade"].ToString();
                        
                        // Determinar categoria baseada no nome
                        string nome = reader["Nome"].ToString();
                        string categoria = DeterminarCategoria(nome);
                        ddlCategoria.SelectedValue = categoria;

                        // Desabilitar campos para visualização
                        txtNome.Enabled = false;
                        txtDescricao.Enabled = false;
                        ddlPrioridade.Enabled = false;
                        ddlCategoria.Enabled = false;
                        buttonGuardar.Visible = false;
                        buttonCancelar.Visible = false;

                        formularioDados.Visible = true;
                    }
                }
            
            
        }

        private void CarregarLocalParaEdicao(int idLocal)
        {
            
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT IDLocal, Nome, Descricao, Prioridade FROM Local WHERE IDLocal = @IDLocal";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IDLocal", idLocal);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtNome.Text = reader["Nome"].ToString();
                        txtDescricao.Text = reader["Descricao"] != DBNull.Value ? reader["Descricao"].ToString() : "";
                        ddlPrioridade.SelectedValue = reader["Prioridade"].ToString();
                        
                        // Determinar categoria baseada no nome
                        string nome = reader["Nome"].ToString();
                        string categoria = DeterminarCategoria(nome);
                        ddlCategoria.SelectedValue = categoria;

                        // Habilitar campos para edição
                        txtNome.Enabled = true;
                        txtDescricao.Enabled = true;
                        ddlPrioridade.Enabled = true;
                        ddlCategoria.Enabled = true;
                        buttonGuardar.Visible = true;
                        buttonCancelar.Visible = true;
                    }
                }
            
            
        }

        private string DeterminarCategoria(string nome)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(nome, @"^Sala [0-9]+$") || 
                System.Text.RegularExpressions.Regex.IsMatch(nome, @"^Sala [1-2][0-9]$"))
                return "Salas";
            
            if (nome.StartsWith("Sala LF") || nome.StartsWith("Sala LQ") || nome.StartsWith("Sala LM") || 
                nome.StartsWith("Sala D") || nome.StartsWith("Salas INF") || nome.StartsWith("Salas MEC") || 
                nome == "Sala 6A")
                return "Laboratorios";
            
            if (nome == "Sala dos Professores" || nome == "Reprografia (Repo)" || 
                nome == "Papelaria" || nome == "Secretaria")
                return "Administrativos";
            
            if (nome == "Cantina" || nome == "Entrada" || nome == "Bar" || nome == "Biblioteca")
                return "Comuns";
            
            if (nome == "Pavilhão de Educação Física" || nome == "Auditório")
                return "Especiais";
            
            return "";
        }

        private void EliminarLocal(int idLocal)
        {
            
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Verificar se o local está sendo usado na tabela LocalEquipamento
                    string checkQuery = "SELECT COUNT(*) FROM LocalEquipamento WHERE Local = @IDLocal";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@IDLocal", idLocal);

                    conn.Open();
                    int count = (int)checkCmd.ExecuteScalar();

                    

                    // Se não está sendo usado, pode eliminar
                    string deleteQuery = "DELETE FROM Local WHERE IDLocal = @IDLocal";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@IDLocal", idLocal);

                    int result = deleteCmd.ExecuteNonQuery();

                   
                }
            
        }

        private void LimparFormulario()
        {
            txtNome.Text = "";
            txtDescricao.Text = "";
            ddlPrioridade.SelectedValue = "2";
            ddlCategoria.SelectedIndex = 0;
            
            // Habilitar campos
            txtNome.Enabled = true;
            txtDescricao.Enabled = true;
            ddlPrioridade.Enabled = true;
            ddlCategoria.Enabled = true;
            buttonGuardar.Visible = true;
            buttonCancelar.Visible = true;
            
            gridLocais.SelectedIndex = -1;
        }
    }
}