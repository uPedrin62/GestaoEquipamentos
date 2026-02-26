using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class videoprojetor : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarMarcas();
                CarregarMarcasFormulario();
                CarregarVideoprojectores();
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

        private void CarregarVideoprojectores()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT m.IDModelo, 
                                       m.Nome,
                                       ISNULL(ma.Nome, 'Sem marca') AS Marca
                                FROM Modelo m
                                LEFT JOIN Marca ma ON m.Marca = ma.IDMarca
                                WHERE m.TipoEquipamento = 6";

                if (listMarca.SelectedValue != "0")
                {
                    query += " AND ma.IDMarca = @IDMarca";
                }

                query += " ORDER BY m.Nome";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (listMarca.SelectedValue != "0")
                {
                    cmd.Parameters.AddWithValue("@IDMarca", listMarca.SelectedValue);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gridVideoprojectores.DataSource = dt;
                gridVideoprojectores.DataBind();
            }
        }

        protected void listMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridVideoprojectores.SelectedIndex = -1;
            CarregarVideoprojectores();
        }

        protected void gridVideoprojectores_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gridVideoprojectores.SelectedIndex >= 0)
            {
                ViewState["SelectedID"] = gridVideoprojectores.SelectedDataKey.Value;
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
        }

        protected void buttonEliminar_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedID"] != null)
            {
                try
                {
                    int idModelo = Convert.ToInt32(ViewState["SelectedID"]);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        
                        string query = "DELETE FROM Videoprojector WHERE Modelo = @IDModelo";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                        cmd.ExecuteNonQuery();
                        
                        query = "DELETE FROM Modelo WHERE IDModelo = @IDModelo";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                        cmd.ExecuteNonQuery();
                    }
                    
                    ViewState["SelectedID"] = null;
                    gridVideoprojectores.SelectedIndex = -1;
                    CarregarVideoprojectores();
                }
                catch (Exception ex)
                {
                    // Erro ao eliminar
                }
            }
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string modo = ViewState["Modo"]?.ToString();

                if (modo == "Criar")
                {
                    CriarVideoprojector();
                }
                else if (modo == "Editar")
                {
                    AtualizarVideoprojector();
                }

                formularioDados.Visible = false;
                gridVideoprojectores.SelectedIndex = -1;
                CarregarVideoprojectores();
            }
            catch (Exception ex)
            {
                // Mostrar erro para debug
                Master.MostrarMensagem("Erro ao guardar: " + ex.Message, "erro");
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

        private void CarregarDados(int idModelo)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT m.*, v.Luminosidade, v.ResolucaoNativa, v.FullHD, v.HDMIPortas, v.USBPortas, v.Wifi, v.AltifalantesIncorporados
                               FROM Modelo m
                               LEFT JOIN Videoprojector v ON v.Modelo = m.IDModelo
                               WHERE m.IDModelo = @IDModelo";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IDModelo", idModelo);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlMarca.SelectedValue = reader["Marca"]?.ToString() ?? "0";
                    txtNome.Text = reader["Nome"]?.ToString() ?? "";
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
                    
                    txtLuminosidade.Text = reader["Luminosidade"]?.ToString() ?? "";
                    txtResolucaoNativa.Text = reader["ResolucaoNativa"]?.ToString() ?? "";
                    txtHDMIPortas.Text = reader["HDMIPortas"] != DBNull.Value ? reader["HDMIPortas"].ToString() : "";
                    txtUSBPortas.Text = reader["USBPortas"] != DBNull.Value ? reader["USBPortas"].ToString() : "";
                    chkFullHD.Checked = reader["FullHD"] != DBNull.Value && Convert.ToBoolean(reader["FullHD"]);
                    chkWifi.Checked = reader["Wifi"] != DBNull.Value && Convert.ToBoolean(reader["Wifi"]);
                    chkAltifalantes.Checked = reader["AltifalantesIncorporados"] != DBNull.Value && Convert.ToBoolean(reader["AltifalantesIncorporados"]);
                }
            }
        }

        private void CriarVideoprojector()
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
                                   VALUES (@Nome, @Marca, 6, @Descricao, @URL, @URLDrivers, @Foto); 
                                   SELECT SCOPE_IDENTITY();";
                SqlCommand cmdModelo = new SqlCommand(sqlModelo, conn);
                cmdModelo.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                cmdModelo.Parameters.AddWithValue("@Descricao", string.IsNullOrWhiteSpace(txtDescricao.Text) ? (object)DBNull.Value : txtDescricao.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URL", string.IsNullOrWhiteSpace(txtURL.Text) ? (object)DBNull.Value : txtURL.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URLDrivers", string.IsNullOrWhiteSpace(txtURLDrivers.Text) ? (object)DBNull.Value : txtURLDrivers.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Foto", fotoPath ?? (object)DBNull.Value);
                int idModelo = Convert.ToInt32(cmdModelo.ExecuteScalar());

                string sqlVideoprojector = @"INSERT INTO Videoprojector (Luminosidade, ResolucaoNativa, FullHD, HDMIPortas, USBPortas, Wifi, AltifalantesIncorporados, Modelo)
                                           VALUES (@Luminosidade, @ResolucaoNativa, @FullHD, @HDMIPortas, @USBPortas, @Wifi, @AltifalantesIncorporados, @Modelo)";
                SqlCommand cmdVideoprojector = new SqlCommand(sqlVideoprojector, conn);
                cmdVideoprojector.Parameters.AddWithValue("@Luminosidade", string.IsNullOrWhiteSpace(txtLuminosidade.Text) ? (object)DBNull.Value : txtLuminosidade.Text.Trim());
                cmdVideoprojector.Parameters.AddWithValue("@ResolucaoNativa", string.IsNullOrWhiteSpace(txtResolucaoNativa.Text) ? (object)DBNull.Value : txtResolucaoNativa.Text.Trim());
                cmdVideoprojector.Parameters.AddWithValue("@FullHD", chkFullHD.Checked);
                cmdVideoprojector.Parameters.AddWithValue("@HDMIPortas", string.IsNullOrWhiteSpace(txtHDMIPortas.Text) ? (object)DBNull.Value : Convert.ToByte(txtHDMIPortas.Text));
                cmdVideoprojector.Parameters.AddWithValue("@USBPortas", string.IsNullOrWhiteSpace(txtUSBPortas.Text) ? (object)DBNull.Value : Convert.ToByte(txtUSBPortas.Text));
                cmdVideoprojector.Parameters.AddWithValue("@Wifi", chkWifi.Checked);
                cmdVideoprojector.Parameters.AddWithValue("@AltifalantesIncorporados", chkAltifalantes.Checked);
                cmdVideoprojector.Parameters.AddWithValue("@Modelo", idModelo);
                cmdVideoprojector.ExecuteNonQuery();
            }
        }

        private void AtualizarVideoprojector()
        {
            int idModelo = Convert.ToInt32(ViewState["SelectedID"]);

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

                // Verifica se já existe um registro na tabela Videoprojector
                string sqlCheck = "SELECT COUNT(*) FROM Videoprojector WHERE Modelo = @Modelo";
                SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn);
                cmdCheck.Parameters.AddWithValue("@Modelo", idModelo);
                int count = (int)cmdCheck.ExecuteScalar();

                string sql;
                if (count > 0)
                {
                    // UPDATE se já existe
                    sql = "UPDATE Videoprojector SET Luminosidade = @Luminosidade, ResolucaoNativa = @ResolucaoNativa, " +
                          "FullHD = @FullHD, HDMIPortas = @HDMIPortas, USBPortas = @USBPortas, Wifi = @Wifi, " +
                          "AltifalantesIncorporados = @AltifalantesIncorporados WHERE Modelo = @Modelo";
                }
                else
                {
                    // INSERT se não existe
                    sql = "INSERT INTO Videoprojector (Luminosidade, ResolucaoNativa, FullHD, HDMIPortas, USBPortas, Wifi, AltifalantesIncorporados, Modelo) " +
                          "VALUES (@Luminosidade, @ResolucaoNativa, @FullHD, @HDMIPortas, @USBPortas, @Wifi, @AltifalantesIncorporados, @Modelo)";
                }

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Luminosidade", string.IsNullOrWhiteSpace(txtLuminosidade.Text) ? (object)DBNull.Value : txtLuminosidade.Text.Trim());
                cmd.Parameters.AddWithValue("@ResolucaoNativa", string.IsNullOrWhiteSpace(txtResolucaoNativa.Text) ? (object)DBNull.Value : txtResolucaoNativa.Text.Trim());
                cmd.Parameters.AddWithValue("@FullHD", chkFullHD.Checked);
                cmd.Parameters.AddWithValue("@HDMIPortas", string.IsNullOrWhiteSpace(txtHDMIPortas.Text) ? (object)DBNull.Value : Convert.ToByte(txtHDMIPortas.Text));
                cmd.Parameters.AddWithValue("@USBPortas", string.IsNullOrWhiteSpace(txtUSBPortas.Text) ? (object)DBNull.Value : Convert.ToByte(txtUSBPortas.Text));
                cmd.Parameters.AddWithValue("@Wifi", chkWifi.Checked);
                cmd.Parameters.AddWithValue("@AltifalantesIncorporados", chkAltifalantes.Checked);
                cmd.Parameters.AddWithValue("@Modelo", idModelo);
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
            txtLuminosidade.Text = "";
            txtResolucaoNativa.Text = "";
            txtHDMIPortas.Text = "";
            txtUSBPortas.Text = "";
            chkFullHD.Checked = false;
            chkWifi.Checked = false;
            chkAltifalantes.Checked = false;
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
            txtLuminosidade.Enabled = !desabilitar;
            txtResolucaoNativa.Enabled = !desabilitar;
            txtHDMIPortas.Enabled = !desabilitar;
            txtUSBPortas.Enabled = !desabilitar;
            chkFullHD.Enabled = !desabilitar;
            chkWifi.Enabled = !desabilitar;
            chkAltifalantes.Enabled = !desabilitar;
        }
    }
}