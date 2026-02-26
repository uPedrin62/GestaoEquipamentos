using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class monitores : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarMarcas();
                CarregarMarcasFormulario();
                CarregarMonitores();
            }
        }

        private void CarregarMarcas()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT IDMarca, Nome FROM Marca ORDER BY Nome";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                listMarca.DataSource = dt;
                listMarca.DataTextField = "Nome";
                listMarca.DataValueField = "IDMarca";
                listMarca.DataBind();

                listMarca.Items.Insert(0, new ListItem("Todas as marcas", "0"));
            }
        }

        private void CarregarMarcasFormulario()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT IDMarca, Nome FROM Marca ORDER BY Nome";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlMarca.DataSource = dt;
                ddlMarca.DataTextField = "Nome";
                ddlMarca.DataValueField = "IDMarca";
                ddlMarca.DataBind();

                ddlMarca.Items.Insert(0, new ListItem("Selecione uma marca", "0"));
            }
        }

        private void CarregarMonitores()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT m.ID, 
                                       ISNULL(mo.Nome, 'Sem nome') AS Nome,
                                       ISNULL(ma.Nome, 'Sem marca') AS Marca
                                FROM Monitor m
                                LEFT JOIN Modelo mo ON m.Modelo = mo.IDModelo
                                LEFT JOIN Marca ma ON mo.Marca = ma.IDMarca";

                if (listMarca.SelectedValue != "0")
                {
                    query += " WHERE ma.IDMarca = @IDMarca";
                }

                query += " ORDER BY mo.Nome";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (listMarca.SelectedValue != "0")
                {
                    cmd.Parameters.AddWithValue("@IDMarca", listMarca.SelectedValue);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gridMonitores.DataSource = dt;
                gridMonitores.DataBind();
            }
        }

        protected void listMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridMonitores.SelectedIndex = -1;
            CarregarMonitores();
        }

        protected void gridMonitores_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Armazena o ID selecionado
            if (gridMonitores.SelectedIndex >= 0)
            {
                ViewState["SelectedID"] = gridMonitores.SelectedDataKey.Value;
            }
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedID"] != null)
            {
                CarregarDados(Convert.ToInt32(ViewState["SelectedID"]));
                ViewState["Modo"] = "Ver";
                formularioDados.Visible = true;
                DesabilitarCampos(true);
                buttonGuardar.Visible = false;
            }
            else
            {
                // Alerta removido
            }
        }

        protected void buttonCriar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            ViewState["Modo"] = "Criar";
            ViewState["SelectedID"] = null;
            formularioDados.Visible = true;
            DesabilitarCampos(false);
            buttonGuardar.Visible = true;
        }

        protected void buttonEditar_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedID"] != null)
            {
                CarregarDados(Convert.ToInt32(ViewState["SelectedID"]));
                ViewState["Modo"] = "Editar";
                formularioDados.Visible = true;
                DesabilitarCampos(false);
                buttonGuardar.Visible = true;
            }
            else
            {
                // Alerta removido
            }
        }

        protected void buttonEliminar_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedID"] != null)
            {
                try
                {
                    int id = Convert.ToInt32(ViewState["SelectedID"]);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Monitor WHERE ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);
                        
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    
                    ViewState["SelectedID"] = null;
                    gridMonitores.SelectedIndex = -1;
                    CarregarMonitores();
                    // Alerta removido
                }
                catch (Exception ex)
                {
                    // Erro ao eliminar
                }
            }
            else
            {
                // Nenhum item selecionado
            }
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string modo = ViewState["Modo"]?.ToString();

                if (modo == "Criar")
                {
                    CriarMonitor();
                }
                else if (modo == "Editar")
                {
                    AtualizarMonitor();
                }

                formularioDados.Visible = false;
                CarregarMonitores();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao guardar: {ex.Message}');", true);
            }
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            formularioDados.Visible = false;
            LimparCampos();
        }

        protected void buttonFechar_Click(object sender, EventArgs e)
        {
            formularioDados.Visible = false;
            LimparCampos();
        }

        private void CarregarDados(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT m.*, mo.Marca, mo.Nome as NomeModelo, mo.Descricao, mo.URL, mo.URLDrivers, mo.Foto
                               FROM Monitor m
                               LEFT JOIN Modelo mo ON m.Modelo = mo.IDModelo
                               WHERE m.ID = @ID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlMarca.SelectedValue = reader["Marca"]?.ToString() ?? "0";
                    txtNome.Text = reader["NomeModelo"]?.ToString() ?? "";
                    txtDescricao.Text = reader["Descricao"]?.ToString() ?? "";
                    txtURL.Text = reader["URL"]?.ToString() ?? "";
                    txtURLDrivers.Text = reader["URLDrivers"]?.ToString() ?? "";
                    
                    if (reader["Foto"] != DBNull.Value && !string.IsNullOrEmpty(reader["Foto"].ToString()))
                    {
                        fotoModelo.ImageUrl = reader["Foto"].ToString();
                        fotoModelo.Visible = true;
                    }
                    else
                    {
                        fotoModelo.Visible = false;
                    }
                    
                    txtTamanho.Text = reader["TamanhoEcrã"]?.ToString() ?? "";
                    txtResolucao.Text = reader["Resolução"]?.ToString() ?? "";
                    txtTipoHD.Text = reader["TipoHD"]?.ToString() ?? "";
                    txtTecnologiaApresentacao.Text = reader["TecnologiaApresentação"]?.ToString() ?? "";
                    txtTipoPainel.Text = reader["TipoPainel"]?.ToString() ?? "";
                    txtFormatoEcra.Text = reader["FormatoEcrã"]?.ToString() ?? "";
                    txtColunasNumero.Text = reader["ColunasNúmero"] != DBNull.Value ? reader["ColunasNúmero"].ToString() : "";
                    txtPortasHDMI.Text = reader["PortasHDMI"] != DBNull.Value ? reader["PortasHDMI"].ToString() : "";
                    txtPortasVGA.Text = reader["PortasVGA"] != DBNull.Value ? reader["PortasVGA"].ToString() : "";
                    txtPortasDVI.Text = reader["PortasDVI"] != DBNull.Value ? reader["PortasDVI"].ToString() : "";
                    txtAltura.Text = reader["Altura"]?.ToString() ?? "";
                    txtComprimento.Text = reader["Comprimento"]?.ToString() ?? "";
                    txtLargura.Text = reader["Largura"]?.ToString() ?? "";
                    chkTactil.Checked = reader["EcrãTáctil"] != DBNull.Value && Convert.ToBoolean(reader["EcrãTáctil"]);
                    chkCamara.Checked = reader["CâmaraIncorporada"] != DBNull.Value && Convert.ToBoolean(reader["CâmaraIncorporada"]);
                    chkSaidaAuscultador.Checked = reader["Saídauscultador"] != DBNull.Value && Convert.ToBoolean(reader["Saídauscultador"]);
                }
            }
        }

        private void CriarMonitor()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string fotoPath = null;
                if (uploadFoto.HasFile)
                {
                    string extensao = System.IO.Path.GetExtension(uploadFoto.FileName);
                    fotoPath = Guid.NewGuid().ToString() + extensao;
                    string caminho = Server.MapPath("~/administrador/uploads/");
                    System.IO.Directory.CreateDirectory(caminho);
                    uploadFoto.SaveAs(System.IO.Path.Combine(caminho, fotoPath));
                    fotoPath = "~/administrador/uploads/" + fotoPath;
                }

                string sqlModelo = @"INSERT INTO Modelo (Nome, Marca, TipoEquipamento, Descricao, URL, URLDrivers, Foto) 
                                   VALUES (@Nome, @Marca, 2, @Descricao, @URL, @URLDrivers, @Foto); 
                                   SELECT SCOPE_IDENTITY();";
                SqlCommand cmdModelo = new SqlCommand(sqlModelo, conn);
                cmdModelo.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                cmdModelo.Parameters.AddWithValue("@Descricao", string.IsNullOrWhiteSpace(txtDescricao.Text) ? (object)DBNull.Value : txtDescricao.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URL", string.IsNullOrWhiteSpace(txtURL.Text) ? (object)DBNull.Value : txtURL.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URLDrivers", string.IsNullOrWhiteSpace(txtURLDrivers.Text) ? (object)DBNull.Value : txtURLDrivers.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Foto", fotoPath ?? (object)DBNull.Value);
                int idModelo = Convert.ToInt32(cmdModelo.ExecuteScalar());

                string sqlMonitor = @"INSERT INTO Monitor (TamanhoEcrã, Resolução, TipoHD, TecnologiaApresentação, TipoPainel, EcrãTáctil, FormatoEcrã, ColunasNúmero, CâmaraIncorporada, PortasHDMI, PortasVGA, PortasDVI, Altura, Comprimento, Largura, Saídauscultador, Modelo)
                                    VALUES (@TamanhoEcra, @Resolucao, @TipoHD, @TecnologiaApresentacao, @TipoPainel, @EcraTatil, @FormatoEcra, @ColunasNumero, @Camara, @PortasHDMI, @PortasVGA, @PortasDVI, @Altura, @Comprimento, @Largura, @SaidaAuscultador, @Modelo)";
                SqlCommand cmdMonitor = new SqlCommand(sqlMonitor, conn);
                cmdMonitor.Parameters.AddWithValue("@TamanhoEcra", string.IsNullOrWhiteSpace(txtTamanho.Text) ? (object)DBNull.Value : txtTamanho.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@Resolucao", string.IsNullOrWhiteSpace(txtResolucao.Text) ? (object)DBNull.Value : txtResolucao.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@TipoHD", string.IsNullOrWhiteSpace(txtTipoHD.Text) ? (object)DBNull.Value : txtTipoHD.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@TecnologiaApresentacao", string.IsNullOrWhiteSpace(txtTecnologiaApresentacao.Text) ? (object)DBNull.Value : txtTecnologiaApresentacao.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@TipoPainel", string.IsNullOrWhiteSpace(txtTipoPainel.Text) ? (object)DBNull.Value : txtTipoPainel.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@EcraTatil", chkTactil.Checked);
                cmdMonitor.Parameters.AddWithValue("@FormatoEcra", string.IsNullOrWhiteSpace(txtFormatoEcra.Text) ? (object)DBNull.Value : txtFormatoEcra.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@ColunasNumero", string.IsNullOrWhiteSpace(txtColunasNumero.Text) ? (object)DBNull.Value : Convert.ToInt32(txtColunasNumero.Text));
                cmdMonitor.Parameters.AddWithValue("@Camara", chkCamara.Checked);
                cmdMonitor.Parameters.AddWithValue("@PortasHDMI", string.IsNullOrWhiteSpace(txtPortasHDMI.Text) ? (object)DBNull.Value : Convert.ToByte(txtPortasHDMI.Text));
                cmdMonitor.Parameters.AddWithValue("@PortasVGA", string.IsNullOrWhiteSpace(txtPortasVGA.Text) ? (object)DBNull.Value : Convert.ToByte(txtPortasVGA.Text));
                cmdMonitor.Parameters.AddWithValue("@PortasDVI", string.IsNullOrWhiteSpace(txtPortasDVI.Text) ? (object)DBNull.Value : Convert.ToByte(txtPortasDVI.Text));
                cmdMonitor.Parameters.AddWithValue("@Altura", string.IsNullOrWhiteSpace(txtAltura.Text) ? (object)DBNull.Value : txtAltura.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@Comprimento", string.IsNullOrWhiteSpace(txtComprimento.Text) ? (object)DBNull.Value : txtComprimento.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@Largura", string.IsNullOrWhiteSpace(txtLargura.Text) ? (object)DBNull.Value : txtLargura.Text.Trim());
                cmdMonitor.Parameters.AddWithValue("@SaidaAuscultador", chkSaidaAuscultador.Checked);
                cmdMonitor.Parameters.AddWithValue("@Modelo", idModelo);
                cmdMonitor.ExecuteNonQuery();
            }
        }

        private void AtualizarMonitor()
        {
            int id = Convert.ToInt32(ViewState["SelectedID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string fotoPath = null;
                if (uploadFoto.HasFile)
                {
                    string extensao = System.IO.Path.GetExtension(uploadFoto.FileName);
                    fotoPath = Guid.NewGuid().ToString() + extensao;
                    string caminho = Server.MapPath("~/administrador/uploads/");
                    System.IO.Directory.CreateDirectory(caminho);
                    uploadFoto.SaveAs(System.IO.Path.Combine(caminho, fotoPath));
                    fotoPath = "~/administrador/uploads/" + fotoPath;
                }

                // Obter IDModelo
                string queryModelo = "SELECT Modelo FROM Monitor WHERE ID = @ID";
                SqlCommand cmdGetModelo = new SqlCommand(queryModelo, conn);
                cmdGetModelo.Parameters.AddWithValue("@ID", id);
                int idModelo = Convert.ToInt32(cmdGetModelo.ExecuteScalar());

                string sqlModelo = "UPDATE Modelo SET Nome = @Nome, Marca = @Marca, Descricao = @Descricao, " +
                                   "URL = @URL, URLDrivers = @URLDrivers" + 
                                   (fotoPath != null ? ", Foto = @Foto" : "") + 
                                   " WHERE IDModelo = @IDModelo";
                SqlCommand cmdModelo = new SqlCommand(sqlModelo, conn);
                cmdModelo.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                cmdModelo.Parameters.AddWithValue("@Descricao", string.IsNullOrWhiteSpace(txtDescricao.Text) ? (object)DBNull.Value : txtDescricao.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URL", string.IsNullOrWhiteSpace(txtURL.Text) ? (object)DBNull.Value : txtURL.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URLDrivers", string.IsNullOrWhiteSpace(txtURLDrivers.Text) ? (object)DBNull.Value : txtURLDrivers.Text.Trim());
                if (fotoPath != null)
                {
                    cmdModelo.Parameters.AddWithValue("@Foto", fotoPath);
                }
                cmdModelo.Parameters.AddWithValue("@IDModelo", idModelo);
                cmdModelo.ExecuteNonQuery();

                string sql = @"UPDATE Monitor SET TamanhoEcrã = @TamanhoEcra, Resolução = @Resolucao, TipoHD = @TipoHD, TecnologiaApresentação = @TecnologiaApresentacao, TipoPainel = @TipoPainel,
                             EcrãTáctil = @EcraTatil, FormatoEcrã = @FormatoEcra, ColunasNúmero = @ColunasNumero, CâmaraIncorporada = @Camara, PortasHDMI = @PortasHDMI, PortasVGA = @PortasVGA, PortasDVI = @PortasDVI,
                             Altura = @Altura, Comprimento = @Comprimento, Largura = @Largura, Saídauscultador = @SaidaAuscultador
                             WHERE ID = @ID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TamanhoEcra", string.IsNullOrWhiteSpace(txtTamanho.Text) ? (object)DBNull.Value : txtTamanho.Text.Trim());
                cmd.Parameters.AddWithValue("@Resolucao", string.IsNullOrWhiteSpace(txtResolucao.Text) ? (object)DBNull.Value : txtResolucao.Text.Trim());
                cmd.Parameters.AddWithValue("@TipoHD", string.IsNullOrWhiteSpace(txtTipoHD.Text) ? (object)DBNull.Value : txtTipoHD.Text.Trim());
                cmd.Parameters.AddWithValue("@TecnologiaApresentacao", string.IsNullOrWhiteSpace(txtTecnologiaApresentacao.Text) ? (object)DBNull.Value : txtTecnologiaApresentacao.Text.Trim());
                cmd.Parameters.AddWithValue("@TipoPainel", string.IsNullOrWhiteSpace(txtTipoPainel.Text) ? (object)DBNull.Value : txtTipoPainel.Text.Trim());
                cmd.Parameters.AddWithValue("@EcraTatil", chkTactil.Checked);
                cmd.Parameters.AddWithValue("@FormatoEcra", string.IsNullOrWhiteSpace(txtFormatoEcra.Text) ? (object)DBNull.Value : txtFormatoEcra.Text.Trim());
                cmd.Parameters.AddWithValue("@ColunasNumero", string.IsNullOrWhiteSpace(txtColunasNumero.Text) ? (object)DBNull.Value : Convert.ToInt32(txtColunasNumero.Text));
                cmd.Parameters.AddWithValue("@Camara", chkCamara.Checked);
                cmd.Parameters.AddWithValue("@PortasHDMI", string.IsNullOrWhiteSpace(txtPortasHDMI.Text) ? (object)DBNull.Value : Convert.ToByte(txtPortasHDMI.Text));
                cmd.Parameters.AddWithValue("@PortasVGA", string.IsNullOrWhiteSpace(txtPortasVGA.Text) ? (object)DBNull.Value : Convert.ToByte(txtPortasVGA.Text));
                cmd.Parameters.AddWithValue("@PortasDVI", string.IsNullOrWhiteSpace(txtPortasDVI.Text) ? (object)DBNull.Value : Convert.ToByte(txtPortasDVI.Text));
                cmd.Parameters.AddWithValue("@Altura", string.IsNullOrWhiteSpace(txtAltura.Text) ? (object)DBNull.Value : txtAltura.Text.Trim());
                cmd.Parameters.AddWithValue("@Comprimento", string.IsNullOrWhiteSpace(txtComprimento.Text) ? (object)DBNull.Value : txtComprimento.Text.Trim());
                cmd.Parameters.AddWithValue("@Largura", string.IsNullOrWhiteSpace(txtLargura.Text) ? (object)DBNull.Value : txtLargura.Text.Trim());
                cmd.Parameters.AddWithValue("@SaidaAuscultador", chkSaidaAuscultador.Checked);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();
            }
        }

        private void LimparCampos()
        {
            ddlMarca.SelectedIndex = 0;
            txtNome.Text = "";
            txtDescricao.Text = "";
            txtURL.Text = "";
            txtURLDrivers.Text = "";
            txtTamanho.Text = "";
            txtResolucao.Text = "";
            txtTipoHD.Text = "";
            txtTecnologiaApresentacao.Text = "";
            txtTipoPainel.Text = "";
            txtFormatoEcra.Text = "";
            txtColunasNumero.Text = "";
            txtPortasHDMI.Text = "";
            txtPortasVGA.Text = "";
            txtPortasDVI.Text = "";
            txtAltura.Text = "";
            txtComprimento.Text = "";
            txtLargura.Text = "";
            chkTactil.Checked = false;
            chkCamara.Checked = false;
            chkSaidaAuscultador.Checked = false;
            fotoModelo.Visible = false;
        }

        private void DesabilitarCampos(bool desabilitar)
        {
            ddlMarca.Enabled = !desabilitar;
            txtNome.Enabled = !desabilitar;
            txtDescricao.Enabled = !desabilitar;
            txtURL.Enabled = !desabilitar;
            txtURLDrivers.Enabled = !desabilitar;
            uploadFoto.Enabled = !desabilitar;
            txtTamanho.Enabled = !desabilitar;
            txtResolucao.Enabled = !desabilitar;
            txtTipoHD.Enabled = !desabilitar;
            txtTecnologiaApresentacao.Enabled = !desabilitar;
            txtTipoPainel.Enabled = !desabilitar;
            txtFormatoEcra.Enabled = !desabilitar;
            txtColunasNumero.Enabled = !desabilitar;
            txtPortasHDMI.Enabled = !desabilitar;
            txtPortasVGA.Enabled = !desabilitar;
            txtPortasDVI.Enabled = !desabilitar;
            txtAltura.Enabled = !desabilitar;
            txtComprimento.Enabled = !desabilitar;
            txtLargura.Enabled = !desabilitar;
            chkTactil.Enabled = !desabilitar;
            chkCamara.Enabled = !desabilitar;
            chkSaidaAuscultador.Enabled = !desabilitar;
        }
    }
}

