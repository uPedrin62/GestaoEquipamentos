using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class participacoes : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarLocaisFiltro();
                CarregarLocaisFormulario();
                CarregarTecnicos();
                CarregarParticipacoes();
                CarregarEstatisticas();
            }
        }

        void CarregarLocaisFiltro()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDLocal, Nome FROM Local ORDER BY Nome";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    ddlFiltroLocal.DataSource = cmd.ExecuteReader();
                    ddlFiltroLocal.DataTextField = "Nome";
                    ddlFiltroLocal.DataValueField = "IDLocal";
                    ddlFiltroLocal.DataBind();
                }
            }
            ddlFiltroLocal.Items.Insert(0, new ListItem("-- Todos os Locais --", ""));
        }

        void CarregarLocaisFormulario()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDLocal, Nome FROM Local ORDER BY Nome";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    ddlLocal.DataSource = cmd.ExecuteReader();
                    ddlLocal.DataTextField = "Nome";
                    ddlLocal.DataValueField = "IDLocal";
                    ddlLocal.DataBind();
                }
            }
            ddlLocal.Items.Insert(0, new ListItem("-- Selecione um local --", ""));
        }

        void CarregarTecnicos()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDTecnico, Nome FROM Tecnico ORDER BY Nome";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    ddlTecnico.DataSource = cmd.ExecuteReader();
                    ddlTecnico.DataTextField = "Nome";
                    ddlTecnico.DataValueField = "IDTecnico";
                    ddlTecnico.DataBind();
                }
            }
            ddlTecnico.Items.Insert(0, new ListItem("-- Não atribuído --", ""));
        }

        void CarregarEquipamentosPorLocal(string idLocal)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"SELECT DISTINCT e.IDEquipamento, e.Numero 
                             FROM Equipamento e
                             INNER JOIN LocalEquipamento le ON e.IDEquipamento = le.Equipamento
                             WHERE le.Local = @IDLocal
                             ORDER BY e.Numero";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDLocal", idLocal);
                    con.Open();
                    ddlEquipamento.DataSource = cmd.ExecuteReader();
                    ddlEquipamento.DataTextField = "Numero";
                    ddlEquipamento.DataValueField = "IDEquipamento";
                    ddlEquipamento.DataBind();
                }
            }
            ddlEquipamento.Items.Insert(0, new ListItem("-- Selecione um equipamento --", ""));
        }

        protected void btnNovaParticipacao_Click(object sender, EventArgs e)
        {
            pnlNovaParticipacao.Visible = true;
            LimparCampos();
        }

        protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlLocal.SelectedValue))
            {
                CarregarEquipamentosPorLocal(ddlLocal.SelectedValue);
                DefinirGravidadePorLocal(ddlLocal.SelectedValue);
            }
            else
            {
                ddlEquipamento.Items.Clear();
                ddlEquipamento.Items.Insert(0, new ListItem("-- Selecione um local primeiro --", ""));
                ddlGravidade.SelectedValue = "3"; // Reset to default
            }
        }

        void DefinirGravidadePorLocal(string idLocal)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT Prioridade FROM Local WHERE IDLocal = @IDLocal";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDLocal", idLocal);
                    con.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        // Map Local Priority to Gravity Level
                        // Priority 1 (Baixa) -> Gravity 1
                        // Priority 2 (Normal) -> Gravity 3
                        // Priority 3 (Alta) -> Gravity 4
                        // Priority 4 (Crítica) -> Gravity 5
                        byte prioridade = Convert.ToByte(result);
                        switch (prioridade)
                        {
                            case 1:
                                ddlGravidade.SelectedValue = "1";
                                break;
                            case 2:
                                ddlGravidade.SelectedValue = "3";
                                break;
                            case 3:
                                ddlGravidade.SelectedValue = "4";
                                break;
                            case 4:
                                ddlGravidade.SelectedValue = "5";
                                break;
                            default:
                                ddlGravidade.SelectedValue = "3";
                                break;
                        }
                    }
                }
            }
        }

        protected void ddlFiltroLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarParticipacoes();
            CarregarEstatisticas();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlLocal.SelectedValue) ||
                string.IsNullOrEmpty(ddlEquipamento.SelectedValue) ||
                string.IsNullOrEmpty(txtDescricao.Text.Trim()))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Preencha todos os campos obrigatórios!');", true);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string sql = @"INSERT INTO Participacao 
                                 (Equipamento, DescricaoProblema, DataParticipacao, Urgente, NivelGravidade, Tecnico)
                                 VALUES (@Equipamento, @Descricao, @Data, @Urgente, @Gravidade, @Tecnico)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Equipamento", ddlEquipamento.SelectedValue);
                        cmd.Parameters.AddWithValue("@Descricao", txtDescricao.Text.Trim());
                        cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Urgente", chkUrgente.Checked);
                        cmd.Parameters.AddWithValue("@Gravidade", ddlGravidade.SelectedValue);

                        if (string.IsNullOrEmpty(ddlTecnico.SelectedValue))
                            cmd.Parameters.AddWithValue("@Tecnico", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Tecnico", ddlTecnico.SelectedValue);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                pnlNovaParticipacao.Visible = false;
                CarregarParticipacoes();
                CarregarEstatisticas();
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Participação criada com sucesso!');", true);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Erro ao criar participação: {ex.Message}');", true);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlNovaParticipacao.Visible = false;
            LimparCampos();
        }

        void LimparCampos()
        {
            ddlLocal.SelectedIndex = 0;
            ddlEquipamento.Items.Clear();
            ddlEquipamento.Items.Insert(0, new ListItem("-- Selecione um local primeiro --", ""));
            txtDescricao.Text = "";
            ddlGravidade.SelectedValue = "3";
            chkUrgente.Checked = false;
            ddlTecnico.SelectedIndex = 0;
        }

        void CarregarParticipacoes()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"
                    SELECT 
                        p.IDParticipacao,
                        e.Numero,
                        l.Nome AS LocalNome,
                        p.DescricaoProblema, 
                        p.DataParticipacao,
                        p.DataResolucao,
                        ISNULL(p.Urgente, 0) AS Urgente,
                        ISNULL(p.NivelGravidade, 0) AS NivelGravidade,
                        ISNULL(t.Nome, 'Não atribuído') AS NomeTecnico
                    FROM Participacao p
                    LEFT JOIN Equipamento e ON p.Equipamento = e.IDEquipamento
                    LEFT JOIN LocalEquipamento le ON e.IDEquipamento = le.Equipamento
                    LEFT JOIN Local l ON le.Local = l.IDLocal
                    LEFT JOIN Tecnico t ON p.Tecnico = t.IDTecnico";

                if (!string.IsNullOrEmpty(ddlFiltroLocal.SelectedValue))
                {
                    sql += " WHERE le.Local = @local";
                }
                sql += " ORDER BY CASE WHEN p.DataResolucao IS NULL THEN 0 ELSE 1 END, p.DataParticipacao DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                {
                    if (!string.IsNullOrEmpty(ddlFiltroLocal.SelectedValue))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@local", ddlFiltroLocal.SelectedValue);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gridParticipacoes.DataSource = dt;
                    gridParticipacoes.DataBind();
                }
            }
        }

        void CarregarEstatisticas()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Participacao", con))
                    lblTotal.Text = cmd.ExecuteScalar().ToString();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Participacao WHERE DataResolucao IS NULL", con))
                    lblPendentes.Text = cmd.ExecuteScalar().ToString();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Participacao WHERE DataResolucao IS NOT NULL", con))
                    lblResolvidas.Text = cmd.ExecuteScalar().ToString();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Participacao WHERE Urgente = 1", con))
                    lblUrgentes.Text = cmd.ExecuteScalar().ToString();
            }
        }

        protected void gridParticipacoes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridParticipacoes.PageIndex = e.NewPageIndex;
            CarregarParticipacoes();
        }


        protected void btnResolver_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar se o usuário é técnico
                if (Session["userType"] == null || Session["userType"].ToString() != "tecnico")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Apenas técnicos podem resolver participações!');", true);
                    return;
                }

                Button btn = (Button)sender;
                int idParticipacao = Convert.ToInt32(btn.CommandArgument);

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string sql = "UPDATE Participacao SET DataResolucao = GETDATE() WHERE IDParticipacao = @IDParticipacao";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@IDParticipacao", idParticipacao);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                CarregarParticipacoes();
                CarregarEstatisticas();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Participação resolvida com sucesso!');", true);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao resolver participação: {ex.Message}');", true);
            }
        }

        protected void timerRefresh_Tick(object sender, EventArgs e)
        {
            CarregarParticipacoes();
            CarregarEstatisticas();
        }

        protected void gridParticipacoes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Linha selecionada - os botões Alterar e Eliminar ficam disponíveis
            int idParticipacao = Convert.ToInt32(gridParticipacoes.DataKeys[gridParticipacoes.SelectedIndex].Value);
            CarregarDadosParticipacao(idParticipacao);
        }

        void CarregarDadosParticipacao(int idParticipacao)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"SELECT p.*, e.IDEquipamento, le.Local
                             FROM Participacao p
                             INNER JOIN Equipamento e ON p.Equipamento = e.IDEquipamento
                             INNER JOIN LocalEquipamento le ON e.IDEquipamento = le.Equipamento
                             WHERE p.IDParticipacao = @IDParticipacao";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDParticipacao", idParticipacao);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        pnlNovaParticipacao.Visible = true;
                        ddlLocal.SelectedValue = dr["Local"].ToString();
                        CarregarEquipamentosPorLocal(dr["Local"].ToString());
                        ddlEquipamento.SelectedValue = dr["IDEquipamento"].ToString();
                        txtDescricao.Text = dr["DescricaoProblema"].ToString();
                        ddlGravidade.SelectedValue = dr["NivelGravidade"].ToString();
                        chkUrgente.Checked = Convert.ToBoolean(dr["Urgente"]);
                        
                        if (dr["Tecnico"] != DBNull.Value)
                            ddlTecnico.SelectedValue = dr["Tecnico"].ToString();
                        else
                            ddlTecnico.SelectedIndex = 0;
                    }
                }
            }
        }

        protected void btnAlterar_Click(object sender, EventArgs e)
        {
            if (gridParticipacoes.SelectedIndex == -1)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Selecione uma participação para alterar!');", true);
                return;
            }

            int idParticipacao = Convert.ToInt32(gridParticipacoes.DataKeys[gridParticipacoes.SelectedIndex].Value);

            if (string.IsNullOrEmpty(ddlLocal.SelectedValue) ||
                string.IsNullOrEmpty(ddlEquipamento.SelectedValue) ||
                string.IsNullOrEmpty(txtDescricao.Text.Trim()))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Preencha todos os campos obrigatórios!');", true);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string sql = @"UPDATE Participacao 
                                 SET Equipamento = @Equipamento,
                                     DescricaoProblema = @Descricao,
                                     Urgente = @Urgente,
                                     NivelGravidade = @Gravidade,
                                     Tecnico = @Tecnico
                                 WHERE IDParticipacao = @IDParticipacao";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@IDParticipacao", idParticipacao);
                        cmd.Parameters.AddWithValue("@Equipamento", ddlEquipamento.SelectedValue);
                        cmd.Parameters.AddWithValue("@Descricao", txtDescricao.Text.Trim());
                        cmd.Parameters.AddWithValue("@Urgente", chkUrgente.Checked);
                        cmd.Parameters.AddWithValue("@Gravidade", ddlGravidade.SelectedValue);

                        if (string.IsNullOrEmpty(ddlTecnico.SelectedValue))
                            cmd.Parameters.AddWithValue("@Tecnico", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Tecnico", ddlTecnico.SelectedValue);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                pnlNovaParticipacao.Visible = false;
                gridParticipacoes.SelectedIndex = -1;
                CarregarParticipacoes();
                CarregarEstatisticas();
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Participação alterada com sucesso!');", true);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Erro ao alterar participação: {ex.Message}');", true);
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (gridParticipacoes.SelectedIndex == -1)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Selecione uma participação para eliminar!');", true);
                return;
            }

            int idParticipacao = Convert.ToInt32(gridParticipacoes.DataKeys[gridParticipacoes.SelectedIndex].Value);

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string sql = "DELETE FROM Participacao WHERE IDParticipacao = @IDParticipacao";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@IDParticipacao", idParticipacao);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                pnlNovaParticipacao.Visible = false;
                gridParticipacoes.SelectedIndex = -1;
                CarregarParticipacoes();
                CarregarEstatisticas();
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Participação eliminada com sucesso!');", true);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Erro ao eliminar participação: {ex.Message}');", true);
            }
        }
    }
}
