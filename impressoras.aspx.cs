using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class impressoras : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarMarcas();
                CarregarMarcasFormulario();
                CarregarImpressoras();
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

        private void CarregarImpressoras()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT i.ID, 
                                       ISNULL(mo.Nome, 'Sem nome') AS Nome,
                                       ISNULL(ma.Nome, 'Sem marca') AS Marca
                                FROM Impressora i
                                LEFT JOIN Modelo mo ON i.Modelo = mo.IDModelo
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

                gvImpressoras.DataSource = dt;
                gvImpressoras.DataBind();
            }
        }

        protected void listMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvImpressoras.SelectedIndex = -1;
            CarregarImpressoras();
        }

        protected void gvImpressoras_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Armazena o ID selecionado
            if (gvImpressoras.SelectedIndex >= 0)
            {
                ViewState["SelectedID"] = gvImpressoras.SelectedDataKey.Value;
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
                        string query = "DELETE FROM Impressora WHERE ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ViewState["SelectedID"] = null;
                    gvImpressoras.SelectedIndex = -1;
                    CarregarImpressoras();
                    // Alerta removido
                }
                catch (Exception ex)
                {
                    // Erro ao eliminar
                }
            }
            else
            {
                // Alerta removido
            }
        }



        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string modo = ViewState["Modo"]?.ToString();

                if (modo == "Criar")
                {
                    CriarImpressora();
                }
                else if (modo == "Editar")
                {
                    AtualizarImpressora();
                }

                formularioDados.Visible = false;
                CarregarImpressoras();
                // Alerta removido
            }
            catch (Exception ex)
            {
                // Erro ao guardar
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
                string query = @"SELECT i.*, mo.Marca, mo.Nome as NomeModelo, mo.Descricao, mo.URL, mo.URLDrivers, mo.Foto
                               FROM Impressora i
                               LEFT JOIN Modelo mo ON i.Modelo = mo.IDModelo
                               WHERE i.ID = @ID";

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
                    
                    txtTipo.Text = reader["Tipo"]?.ToString() ?? "";
                    txtResolucaoMaxima.Text = reader["ResolucaoMaxima"]?.ToString() ?? "";
                    txtCodigoTinteiros.Text = reader["CodigoTinteiros"]?.ToString() ?? "";
                    chkImpressaoDuplex.Checked = reader["ImpressaoDuplex"] != DBNull.Value && Convert.ToBoolean(reader["ImpressaoDuplex"]);
                    chkImpressaoCor.Checked = reader["ImpressaoCor"] != DBNull.Value && Convert.ToBoolean(reader["ImpressaoCor"]);
                    chkScanner.Checked = reader["Scanner"] != DBNull.Value && Convert.ToBoolean(reader["Scanner"]);
                    chkVisor.Checked = reader["Visor"] != DBNull.Value && Convert.ToBoolean(reader["Visor"]);
                    chkWifi.Checked = reader["Wifi"] != DBNull.Value && Convert.ToBoolean(reader["Wifi"]);
                    chkBluetooth.Checked = reader["Bluetooth"] != DBNull.Value && Convert.ToBoolean(reader["Bluetooth"]);
                }
            }
        }

        private void CriarImpressora()
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
                                   VALUES (@Nome, @Marca, 1, @Descricao, @URL, @URLDrivers, @Foto); 
                                   SELECT SCOPE_IDENTITY();";
                SqlCommand cmdModelo = new SqlCommand(sqlModelo, conn);
                cmdModelo.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                cmdModelo.Parameters.AddWithValue("@Descricao", string.IsNullOrWhiteSpace(txtDescricao.Text) ? (object)DBNull.Value : txtDescricao.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URL", string.IsNullOrWhiteSpace(txtURL.Text) ? (object)DBNull.Value : txtURL.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URLDrivers", string.IsNullOrWhiteSpace(txtURLDrivers.Text) ? (object)DBNull.Value : txtURLDrivers.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Foto", fotoPath ?? (object)DBNull.Value);
                int idModelo = Convert.ToInt32(cmdModelo.ExecuteScalar());

                string sqlImpressora = @"INSERT INTO Impressora (Tipo, ImpressaoDuplex, ResolucaoMaxima, ImpressaoCor, Scanner, Visor, Wifi, Bluetooth, CodigoTinteiros, Modelo)
                                       VALUES (@Tipo, @ImpressaoDuplex, @ResolucaoMaxima, @ImpressaoCor, @Scanner, @Visor, @Wifi, @Bluetooth, @CodigoTinteiros, @Modelo)";
                SqlCommand cmdImpressora = new SqlCommand(sqlImpressora, conn);
                cmdImpressora.Parameters.AddWithValue("@Tipo", string.IsNullOrWhiteSpace(txtTipo.Text) ? (object)DBNull.Value : txtTipo.Text.Trim());
                cmdImpressora.Parameters.AddWithValue("@ImpressaoDuplex", chkImpressaoDuplex.Checked);
                cmdImpressora.Parameters.AddWithValue("@ResolucaoMaxima", string.IsNullOrWhiteSpace(txtResolucaoMaxima.Text) ? (object)DBNull.Value : txtResolucaoMaxima.Text.Trim());
                cmdImpressora.Parameters.AddWithValue("@ImpressaoCor", chkImpressaoCor.Checked);
                cmdImpressora.Parameters.AddWithValue("@Scanner", chkScanner.Checked);
                cmdImpressora.Parameters.AddWithValue("@Visor", chkVisor.Checked);
                cmdImpressora.Parameters.AddWithValue("@Wifi", chkWifi.Checked);
                cmdImpressora.Parameters.AddWithValue("@Bluetooth", chkBluetooth.Checked);
                cmdImpressora.Parameters.AddWithValue("@CodigoTinteiros", txtCodigoTinteiros.Text.Trim());
                cmdImpressora.Parameters.AddWithValue("@Modelo", idModelo);
                cmdImpressora.ExecuteNonQuery();
            }
        }

        private void AtualizarImpressora()
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
                string queryModelo = "SELECT Modelo FROM Impressora WHERE ID = @ID";
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

                string sql = @"UPDATE Impressora SET Tipo = @Tipo, ImpressaoDuplex = @ImpressaoDuplex, ResolucaoMaxima = @ResolucaoMaxima,
                             ImpressaoCor = @ImpressaoCor, Scanner = @Scanner, Visor = @Visor, Wifi = @Wifi, Bluetooth = @Bluetooth, CodigoTinteiros = @CodigoTinteiros
                             WHERE ID = @ID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Tipo", string.IsNullOrWhiteSpace(txtTipo.Text) ? (object)DBNull.Value : txtTipo.Text.Trim());
                cmd.Parameters.AddWithValue("@ImpressaoDuplex", chkImpressaoDuplex.Checked);
                cmd.Parameters.AddWithValue("@ResolucaoMaxima", string.IsNullOrWhiteSpace(txtResolucaoMaxima.Text) ? (object)DBNull.Value : txtResolucaoMaxima.Text.Trim());
                cmd.Parameters.AddWithValue("@ImpressaoCor", chkImpressaoCor.Checked);
                cmd.Parameters.AddWithValue("@Scanner", chkScanner.Checked);
                cmd.Parameters.AddWithValue("@Visor", chkVisor.Checked);
                cmd.Parameters.AddWithValue("@Wifi", chkWifi.Checked);
                cmd.Parameters.AddWithValue("@Bluetooth", chkBluetooth.Checked);
                cmd.Parameters.AddWithValue("@CodigoTinteiros", txtCodigoTinteiros.Text.Trim());
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
            txtTipo.Text = "";
            txtResolucaoMaxima.Text = "";
            txtCodigoTinteiros.Text = "";
            chkImpressaoDuplex.Checked = false;
            chkImpressaoCor.Checked = false;
            chkScanner.Checked = false;
            chkVisor.Checked = false;
            chkWifi.Checked = false;
            chkBluetooth.Checked = false;
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
            txtTipo.Enabled = !desabilitar;
            txtResolucaoMaxima.Enabled = !desabilitar;
            txtCodigoTinteiros.Enabled = !desabilitar;
            chkImpressaoDuplex.Enabled = !desabilitar;
            chkImpressaoCor.Enabled = !desabilitar;
            chkScanner.Enabled = !desabilitar;
            chkVisor.Enabled = !desabilitar;
            chkWifi.Enabled = !desabilitar;
            chkBluetooth.Enabled = !desabilitar;
        }
    }
}

