using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class modelos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    ObterMarcas();
                    ObterTiposEquipamento();
                    ObterMarcasFormulario();
                    ObterTiposEquipamentoFormulario();
                    ObterModelos();
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro no Page_Load: {ex.Message}');", true);
                }
            }
        }

        protected void listMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Limpar seleção da grid
                ViewState["SelectedRowIndex"] = null;
                ViewState["SelectedDataKey"] = null;
                gridModelos.SelectedIndex = -1;
                
                // Recarregar modelos
                ObterModelos();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao filtrar marcas: {ex.Message}');", true);
            }
        }

        protected void listTipoEquipamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Limpar seleção da grid
                ViewState["SelectedRowIndex"] = null;
                ViewState["SelectedDataKey"] = null;
                gridModelos.SelectedIndex = -1;
                
                // Recarregar modelos
                ObterModelos();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao filtrar tipos: {ex.Message}');", true);
            }
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedDataKey"] != null)
            {
                // Obter o ID do modelo selecionado do ViewState
                string idModelo = ViewState["SelectedDataKey"].ToString();
                
                // Carregar os dados do modelo
                CarregarDadosModelo(idModelo);
                
                // Configurar para modo visualização
                ViewState["opcao"] = "v";
                formularioDados.Visible = true;
                
                // Desabilitar todos os campos para visualização
                DesabilitarCampos(true);
                
                //botões
                buttonGuardar.Visible = false;
                buttonFechar.Visible = true;
                buttonCancelar.Visible = false;
            }
        }

        protected void buttonCriar_Click(object sender, EventArgs e)
        {
            // Limpar todos os campos
            LimparCampos();
            
            // Habilitar todos os campos para criação
            DesabilitarCampos(false);
            
            //botões
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
                // Obter o ID do modelo selecionado do ViewState
                string idModelo = ViewState["SelectedDataKey"].ToString();
                
                // Carregar os dados do modelo
                CarregarDadosModelo(idModelo);
                
                // Configurar para modo edição
                ViewState["opcao"] = "u";
                ViewState["idModelo"] = idModelo;
                formularioDados.Visible = true;
                
                // Habilitar todos os campos para edição
                DesabilitarCampos(false);
                
                //botões
                buttonGuardar.Visible = true;
                buttonFechar.Visible = false;
                buttonCancelar.Visible = true;
            }
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string opcao = ViewState["opcao"].ToString();

                if (opcao == "i")
                {
                    // Insert - criar modelo
                    InserirModelo();
                    ObterModelos();
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Modelo criado com sucesso!');", true);
                }
                else if (opcao == "u")
                {
                    // Update - atualizar modelo
                    AtualizarModelo();
                    ObterModelos();
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Modelo atualizado com sucesso!');", true);
                }

                // Limpar campos e ocultar formulário
                LimparCampos();
                formularioDados.Visible = false;
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao guardar: {ex.Message}');", true);
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

        #region Métodos Auxiliares

        void CarregarDadosModelo(string idModelo)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"
                    SELECT 
                        M.Nome, M.Marca, M.Descricao, M.URL, M.URLDrivers, 
                        M.TipoEquipamento, M.Foto
                    FROM Modelo M
                    WHERE M.IDModelo = @IDModelo
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            try
                            {
                                // Preencher campos básicos
                                txtNome.Text = reader["Nome"].ToString();
                                txtDescricao.Text = reader["Descricao"].ToString();
                                txtURL.Text = reader["URL"].ToString();
                                txtURLDrivers.Text = reader["URLDrivers"].ToString();
                                
                                // Selecionar marca no dropdown
                                string marcaId = reader["Marca"].ToString();
                                if (ddlMarca.Items.FindByValue(marcaId) != null)
                                {
                                    ddlMarca.SelectedValue = marcaId;
                                }

                                // Selecionar tipo equipamento no dropdown
                                string tipoId = reader["TipoEquipamento"].ToString();
                                if (ddlTipoEquipamento.Items.FindByValue(tipoId) != null)
                                {
                                    ddlTipoEquipamento.SelectedValue = tipoId;
                                }

                                // Carregar foto se existir
                                if (!reader.IsDBNull(reader.GetOrdinal("Foto")))
                                {
                                    string foto = reader["Foto"].ToString();
                                    fotoModelo.ImageUrl = "~/administrador/uploads/" + foto;
                                    fotoModelo.Visible = true;
                                }
                                else
                                {
                                    fotoModelo.Visible = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao carregar dados: {ex.Message}');", true);
                            }
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Nenhum dado encontrado para ID: {idModelo}');", true);
                        }
                    }
                }
            }
        }

        void LimparCampos()
        {
            txtNome.Text = "";
            txtDescricao.Text = "";
            txtURL.Text = "";
            txtURLDrivers.Text = "";
            
            if (ddlMarca.Items.Count > 0)
                ddlMarca.SelectedIndex = 0;
            if (ddlTipoEquipamento.Items.Count > 0)
                ddlTipoEquipamento.SelectedIndex = 0;
                
            fotoModelo.Visible = false;
        }

        void DesabilitarCampos(bool desabilitar)
        {
            txtNome.Enabled = !desabilitar;
            txtDescricao.Enabled = !desabilitar;
            txtURL.Enabled = !desabilitar;
            txtURLDrivers.Enabled = !desabilitar;
            ddlMarca.Enabled = !desabilitar;
            ddlTipoEquipamento.Enabled = !desabilitar;
            uploadFoto.Enabled = !desabilitar;
        }

        #endregion

        #region Funções

        void ObterMarcas()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDMarca, Nome FROM Marca ORDER BY Nome";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();

                    listMarca.DataSource = cmd.ExecuteReader();
                    listMarca.DataTextField = "Nome";
                    listMarca.DataValueField = "IDMarca";
                    listMarca.DataBind();
                }
            }

            listMarca.Items.Insert(0, new ListItem("-- todas as marcas --", ""));
        }

        void ObterTiposEquipamento()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT ID, Nome FROM TipoEquipamento ORDER BY Nome";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();

                    listTipoEquipamento.DataSource = cmd.ExecuteReader();
                    listTipoEquipamento.DataTextField = "Nome";
                    listTipoEquipamento.DataValueField = "ID";
                    listTipoEquipamento.DataBind();
                }
            }

            listTipoEquipamento.Items.Insert(0, new ListItem("-- todos os tipos --", ""));
        }

        void ObterMarcasFormulario()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT IDMarca, Nome FROM Marca ORDER BY Nome";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();

                    ddlMarca.DataSource = cmd.ExecuteReader();
                    ddlMarca.DataTextField = "Nome";
                    ddlMarca.DataValueField = "IDMarca";
                    ddlMarca.DataBind();
                }
            }

            ddlMarca.Items.Insert(0, new ListItem("-- selecione a marca --", ""));
        }

        void ObterTiposEquipamentoFormulario()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = "SELECT ID, Nome FROM TipoEquipamento ORDER BY Nome";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();

                    ddlTipoEquipamento.DataSource = cmd.ExecuteReader();
                    ddlTipoEquipamento.DataTextField = "Nome";
                    ddlTipoEquipamento.DataValueField = "ID";
                    ddlTipoEquipamento.DataBind();
                }
            }

            ddlTipoEquipamento.Items.Insert(0, new ListItem("-- selecione o tipo --", ""));
        }

        void ObterModelos()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"
                    SELECT 
                        M.IDModelo,
                        M.Nome,
                        Ma.Nome AS Marca,
                        TE.Nome AS TipoEquipamento
                    FROM Modelo M
                    INNER JOIN Marca Ma ON M.Marca = Ma.IDMarca
                    INNER JOIN TipoEquipamento TE ON M.TipoEquipamento = TE.ID
                    WHERE M.Nome IS NOT NULL 
                      AND M.Nome != ''";

                // Adicionar filtro de marca se selecionada
                if (listMarca.SelectedIndex > 0 && !string.IsNullOrEmpty(listMarca.SelectedValue))
                {
                    sql += " AND M.Marca = @MarcaID";
                }

                // Adicionar filtro de tipo equipamento se selecionado
                if (listTipoEquipamento.SelectedIndex > 0 && !string.IsNullOrEmpty(listTipoEquipamento.SelectedValue))
                {
                    sql += " AND M.TipoEquipamento = @TipoID";
                }

                sql += " ORDER BY M.Nome";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    // Adicionar parâmetros se filtros ativos
                    if (listMarca.SelectedIndex > 0 && !string.IsNullOrEmpty(listMarca.SelectedValue))
                    {
                        cmd.Parameters.AddWithValue("@MarcaID", listMarca.SelectedValue);
                    }

                    if (listTipoEquipamento.SelectedIndex > 0 && !string.IsNullOrEmpty(listTipoEquipamento.SelectedValue))
                    {
                        cmd.Parameters.AddWithValue("@TipoID", listTipoEquipamento.SelectedValue);
                    }

                    con.Open();
                    gridModelos.DataSource = cmd.ExecuteReader();
                    gridModelos.DataBind();
                }
            }
        }

        #endregion

        protected void gridModelos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Não é mais necessário ocultar colunas pois estamos usando AutoGenerateColumns="false"
        }

        protected void gridModelos_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Armazenar a seleção no ViewState
            if (gridModelos.SelectedIndex >= 0)
            {
                ViewState["SelectedRowIndex"] = gridModelos.SelectedIndex;
                ViewState["SelectedDataKey"] = gridModelos.SelectedDataKey.Value;
            }
        }

        #region BD

        protected void InserirModelo()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Foto (opcional)
            string nomeFoto = null;

            if (uploadFoto.HasFile)
            {
                string extensao = Path.GetExtension(uploadFoto.FileName);
                nomeFoto = Guid.NewGuid().ToString() + extensao;

                string caminho = Server.MapPath("~/administrador/uploads/");
                Directory.CreateDirectory(caminho);

                uploadFoto.SaveAs(Path.Combine(caminho, nomeFoto));
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"
                    INSERT INTO Modelo
                    (Nome, Marca, Descricao, URL, URLDrivers, TipoEquipamento, Foto)
                    VALUES
                    (@Nome, @Marca, @Descricao, @URL, @URLDrivers, @TipoEquipamento, @Foto)
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                    cmd.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                    cmd.Parameters.AddWithValue("@Descricao", txtDescricao.Text.Trim());
                    cmd.Parameters.AddWithValue("@URL", txtURL.Text.Trim());
                    cmd.Parameters.AddWithValue("@URLDrivers", txtURLDrivers.Text.Trim());
                    cmd.Parameters.AddWithValue("@TipoEquipamento", ddlTipoEquipamento.SelectedValue);

                    if (string.IsNullOrEmpty(nomeFoto))
                        cmd.Parameters.AddWithValue("@Foto", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@Foto", nomeFoto);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        void AtualizarModelo()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string idModelo = ViewState["idModelo"].ToString();

            // Foto (opcional)
            string nomeFoto = null;
            bool atualizarFoto = false;

            if (uploadFoto.HasFile)
            {
                string extensao = Path.GetExtension(uploadFoto.FileName);
                nomeFoto = Guid.NewGuid().ToString() + extensao;

                string caminho = Server.MapPath("~/administrador/uploads/");
                Directory.CreateDirectory(caminho);

                uploadFoto.SaveAs(Path.Combine(caminho, nomeFoto));
                atualizarFoto = true;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"
                    UPDATE Modelo SET
                        Nome = @Nome,
                        Marca = @Marca,
                        Descricao = @Descricao,
                        URL = @URL,
                        URLDrivers = @URLDrivers,
                        TipoEquipamento = @TipoEquipamento" +
                        (atualizarFoto ? ", Foto = @Foto" : "") + @"
                    WHERE IDModelo = @IDModelo
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                    cmd.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                    cmd.Parameters.AddWithValue("@Descricao", txtDescricao.Text.Trim());
                    cmd.Parameters.AddWithValue("@URL", txtURL.Text.Trim());
                    cmd.Parameters.AddWithValue("@URLDrivers", txtURLDrivers.Text.Trim());
                    cmd.Parameters.AddWithValue("@TipoEquipamento", ddlTipoEquipamento.SelectedValue);
                    cmd.Parameters.AddWithValue("@IDModelo", idModelo);

                    if (atualizarFoto)
                        cmd.Parameters.AddWithValue("@Foto", nomeFoto);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}