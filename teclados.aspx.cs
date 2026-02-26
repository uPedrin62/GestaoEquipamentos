using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class teclados : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarMarcas();
                CarregarMarcasFormulario();
                CarregarTeclados();
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

        private void CarregarTeclados()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT m.IDModelo, 
                                       m.Nome,
                                       ISNULL(ma.Nome, 'Sem marca') AS Marca
                                FROM Modelo m
                                LEFT JOIN Marca ma ON m.Marca = ma.IDMarca
                                WHERE m.TipoEquipamento = 5";

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

                gridTeclados.DataSource = dt;
                gridTeclados.DataBind();
            }
        }

        protected void listMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridTeclados.SelectedIndex = -1;
            CarregarTeclados();
        }

        protected void gridTeclados_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Armazena o ID selecionado
            if (gridTeclados.SelectedIndex >= 0)
            {
                ViewState["SelectedID"] = gridTeclados.SelectedDataKey.Value;
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
                        // Primeiro elimina o teclado
                        string query = "DELETE FROM Teclado WHERE Modelo = @IDModelo";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@IDModelo", id);
                        
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        
                        // Depois elimina o modelo
                        query = "DELETE FROM Modelo WHERE IDModelo = @IDModelo";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@IDModelo", id);
                        cmd.ExecuteNonQuery();
                    }
                    
                    ViewState["SelectedID"] = null;
                    gridTeclados.SelectedIndex = -1;
                    CarregarTeclados();
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
                    CriarTeclado();
                }
                else if (modo == "Editar")
                {
                    AtualizarTeclado();
                }

                formularioDados.Visible = false;
                gridTeclados.SelectedIndex = -1;
                CarregarTeclados();
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
                string query = @"SELECT m.*, t.Idioma, t.Interface, t.TecladoNumerico, t.Bluetooth, t.Wireless
                               FROM Modelo m
                               LEFT JOIN Teclado t ON t.Modelo = m.IDModelo
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
                    
                    txtIdioma.Text = reader["Idioma"]?.ToString() ?? "";
                    txtInterface.Text = reader["Interface"]?.ToString() ?? "";
                    chkTecladoNumerico.Checked = reader["TecladoNumerico"] != DBNull.Value && Convert.ToBoolean(reader["TecladoNumerico"]);
                    chkBluetooth.Checked = reader["Bluetooth"] != DBNull.Value && Convert.ToBoolean(reader["Bluetooth"]);
                    chkWireless.Checked = reader["Wireless"] != DBNull.Value && Convert.ToBoolean(reader["Wireless"]);
                }
            }
        }

        private void CriarTeclado()
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
                }

                string sqlModelo = @"INSERT INTO Modelo (Nome, Marca, TipoEquipamento, Descricao, URL, URLDrivers, Foto) 
                                   VALUES (@Nome, @Marca, 5, @Descricao, @URL, @URLDrivers, @Foto); 
                                   SELECT SCOPE_IDENTITY();";
                SqlCommand cmdModelo = new SqlCommand(sqlModelo, conn);
                cmdModelo.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                cmdModelo.Parameters.AddWithValue("@Descricao", string.IsNullOrWhiteSpace(txtDescricao.Text) ? (object)DBNull.Value : txtDescricao.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URL", string.IsNullOrWhiteSpace(txtURL.Text) ? (object)DBNull.Value : txtURL.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URLDrivers", string.IsNullOrWhiteSpace(txtURLDrivers.Text) ? (object)DBNull.Value : txtURLDrivers.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Foto", fotoPath ?? (object)DBNull.Value);
                int idModelo = Convert.ToInt32(cmdModelo.ExecuteScalar());

                string sqlTeclado = @"INSERT INTO Teclado (Idioma, Interface, TecladoNumerico, Bluetooth, Wireless, Modelo)
                                    VALUES (@Idioma, @Interface, @TecladoNumerico, @Bluetooth, @Wireless, @Modelo)";
                SqlCommand cmdTeclado = new SqlCommand(sqlTeclado, conn);
                cmdTeclado.Parameters.AddWithValue("@Idioma", string.IsNullOrWhiteSpace(txtIdioma.Text) ? (object)DBNull.Value : txtIdioma.Text.Trim());
                cmdTeclado.Parameters.AddWithValue("@Interface", string.IsNullOrWhiteSpace(txtInterface.Text) ? (object)DBNull.Value : txtInterface.Text.Trim());
                cmdTeclado.Parameters.AddWithValue("@TecladoNumerico", chkTecladoNumerico.Checked);
                cmdTeclado.Parameters.AddWithValue("@Bluetooth", chkBluetooth.Checked);
                cmdTeclado.Parameters.AddWithValue("@Wireless", chkWireless.Checked);
                cmdTeclado.Parameters.AddWithValue("@Modelo", idModelo);
                cmdTeclado.ExecuteNonQuery();
            }
        }

        private void AtualizarTeclado()
        {
            int idModelo = Convert.ToInt32(ViewState["SelectedID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string fotoPath = null;
                bool atualizarFoto = false;
                if (uploadFoto.HasFile)
                {
                    string extensao = System.IO.Path.GetExtension(uploadFoto.FileName);
                    fotoPath = Guid.NewGuid().ToString() + extensao;
                    string caminho = Server.MapPath("~/administrador/uploads/");
                    System.IO.Directory.CreateDirectory(caminho);
                    uploadFoto.SaveAs(System.IO.Path.Combine(caminho, fotoPath));
                    atualizarFoto = true;
                }

                string sqlModelo = "UPDATE Modelo SET Nome = @Nome, Marca = @Marca, Descricao = @Descricao, " +
                                   "URL = @URL, URLDrivers = @URLDrivers" + 
                                   (atualizarFoto ? ", Foto = @Foto" : "") + 
                                   " WHERE IDModelo = @IDModelo";
                SqlCommand cmdModelo = new SqlCommand(sqlModelo, conn);
                cmdModelo.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                cmdModelo.Parameters.AddWithValue("@Descricao", string.IsNullOrWhiteSpace(txtDescricao.Text) ? (object)DBNull.Value : txtDescricao.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URL", string.IsNullOrWhiteSpace(txtURL.Text) ? (object)DBNull.Value : txtURL.Text.Trim());
                cmdModelo.Parameters.AddWithValue("@URLDrivers", string.IsNullOrWhiteSpace(txtURLDrivers.Text) ? (object)DBNull.Value : txtURLDrivers.Text.Trim());
                if (atualizarFoto)
                {
                    cmdModelo.Parameters.AddWithValue("@Foto", fotoPath);
                }
                cmdModelo.Parameters.AddWithValue("@IDModelo", idModelo);
                cmdModelo.ExecuteNonQuery();

                // Verifica se já existe um registro na tabela Teclado
                string sqlCheck = "SELECT COUNT(*) FROM Teclado WHERE Modelo = @Modelo";
                SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn);
                cmdCheck.Parameters.AddWithValue("@Modelo", idModelo);
                int count = (int)cmdCheck.ExecuteScalar();

                string sql;
                if (count > 0)
                {
                    // UPDATE se já existe
                    sql = "UPDATE Teclado SET Idioma = @Idioma, Interface = @Interface, " +
                          "TecladoNumerico = @TecladoNumerico, Bluetooth = @Bluetooth, Wireless = @Wireless " +
                          "WHERE Modelo = @Modelo";
                }
                else
                {
                    // INSERT se não existe
                    sql = "INSERT INTO Teclado (Idioma, Interface, TecladoNumerico, Bluetooth, Wireless, Modelo) " +
                          "VALUES (@Idioma, @Interface, @TecladoNumerico, @Bluetooth, @Wireless, @Modelo)";
                }

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Idioma", string.IsNullOrWhiteSpace(txtIdioma.Text) ? (object)DBNull.Value : txtIdioma.Text.Trim());
                cmd.Parameters.AddWithValue("@Interface", string.IsNullOrWhiteSpace(txtInterface.Text) ? (object)DBNull.Value : txtInterface.Text.Trim());
                cmd.Parameters.AddWithValue("@TecladoNumerico", chkTecladoNumerico.Checked);
                cmd.Parameters.AddWithValue("@Bluetooth", chkBluetooth.Checked);
                cmd.Parameters.AddWithValue("@Wireless", chkWireless.Checked);
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
            txtIdioma.Text = "";
            txtInterface.Text = "";
            chkTecladoNumerico.Checked = false;
            chkBluetooth.Checked = false;
            chkWireless.Checked = false;
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
            txtIdioma.Enabled = !desabilitar;
            txtInterface.Enabled = !desabilitar;
            chkTecladoNumerico.Enabled = !desabilitar;
            chkBluetooth.Enabled = !desabilitar;
            chkWireless.Enabled = !desabilitar;
        }
    }
}

