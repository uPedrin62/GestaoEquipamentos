using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class computadoresDesktop : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ObterMarcas();
                ObterMarcasFormulario();
                ObterComputadores();
            }
        }

        protected void listMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["SelectedRowIndex"] = null;
            ViewState["SelectedDataKey"] = null;
            gridComputadores.SelectedIndex = -1;
            ObterComputadores();
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedDataKey"] != null)
            {
                string idModelo = ViewState["SelectedDataKey"].ToString();
                CarregarDadosComputador(idModelo);
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
                string idModelo = ViewState["SelectedDataKey"].ToString();
                CarregarDadosComputador(idModelo);
                ViewState["opcao"] = "u";
                ViewState["idModelo"] = idModelo;
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
                    string idModelo = ViewState["SelectedDataKey"].ToString();
                    EliminarComputador(idModelo);
                    ObterComputadores();
                    ViewState["SelectedRowIndex"] = null;
                    ViewState["SelectedDataKey"] = null;
                    gridComputadores.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao eliminar: {ex.Message}');", true);
                }
            }
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            
                string opcao = ViewState["opcao"].ToString();
                if (opcao == "i")
                {
                    InserirComputador();
                }
                else if (opcao == "u")
                {
                    AtualizarComputador();
                }
                ObterComputadores();
                LimparCampos();
                formularioDados.Visible = false;
            
           
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            formularioDados.Visible = false;
        }

        protected void buttonFechar_Click(object sender, EventArgs e)
        {
            formularioDados.Visible = false;
        }

        protected void gridComputadores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void gridComputadores_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gridComputadores.SelectedIndex >= 0)
            {
                ViewState["SelectedRowIndex"] = gridComputadores.SelectedIndex;
                ViewState["SelectedDataKey"] = gridComputadores.SelectedDataKey.Value;
            }
        }

        void CarregarDadosComputador(string idModelo)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"SELECT M.Nome, M.Marca, M.Descricao, M.URL, M.URLDrivers, M.Foto,
                              C.Caixa, C.Processador, C.Motherboard, C.RAM, C.Armazenamento, 
                              C.PlacaGrafica, C.FonteAlimentacao, C.NumeroPortasUSB, 
                              C.NumeroPortasHDMI, C.PortaVGA, C.DisplayPort, 
                              C.PortaEthernet, C.WiFi
                              FROM Modelo M INNER JOIN Computador C ON M.IDModelo = C.Modelo
                              WHERE M.IDModelo = @IDModelo";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNome.Text = reader["Nome"].ToString();
                            txtDescricao.Text = reader["Descricao"].ToString();
                            txtURL.Text = reader["URL"].ToString();
                            txtURLDrivers.Text = reader["URLDrivers"].ToString();
                            txtCaixa.Text = reader["Caixa"].ToString();
                            txtProcessador.Text = reader["Processador"].ToString();
                            txtMotherboard.Text = reader["Motherboard"].ToString();
                            txtRAM.Text = reader["RAM"].ToString();
                            txtArmazenamento.Text = reader["Armazenamento"].ToString();
                            txtPlacaGrafica.Text = reader["PlacaGrafica"].ToString();
                            txtFonteAlimentacao.Text = reader["FonteAlimentacao"].ToString();
                            txtPortasUSB.Text = reader["NumeroPortasUSB"].ToString();
                            txtPortasHDMI.Text = reader["NumeroPortasHDMI"].ToString();
                            chkPortaVGA.Checked = Convert.ToBoolean(reader["PortaVGA"]);
                            chkDisplayPort.Checked = Convert.ToBoolean(reader["DisplayPort"]);
                            chkPortaEthernet.Checked = Convert.ToBoolean(reader["PortaEthernet"]);
                            chkWiFi.Checked = Convert.ToBoolean(reader["WiFi"]);

                            string marcaId = reader["Marca"].ToString();
                            if (ddlMarca.Items.FindByValue(marcaId) != null)
                                ddlMarca.SelectedValue = marcaId;

                            if (!reader.IsDBNull(reader.GetOrdinal("Foto")))
                            {
                                fotoModelo.ImageUrl = "~/administrador/uploads/" + reader["Foto"].ToString();
                                fotoModelo.Visible = true;
                            }
                            else
                            {
                                fotoModelo.Visible = false;
                            }
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
            txtCaixa.Text = "";
            txtProcessador.Text = "";
            txtMotherboard.Text = "";
            txtRAM.Text = "";
            txtArmazenamento.Text = "";
            txtPlacaGrafica.Text = "";
            txtFonteAlimentacao.Text = "";
            txtPortasUSB.Text = "";
            txtPortasHDMI.Text = "";
            chkPortaVGA.Checked = false;
            chkDisplayPort.Checked = false;
            chkPortaEthernet.Checked = false;
            chkWiFi.Checked = false;
            if (ddlMarca.Items.Count > 0) ddlMarca.SelectedIndex = 0;
            fotoModelo.Visible = false;
        }

        void DesabilitarCampos(bool desabilitar)
        {
            txtNome.Enabled = !desabilitar;
            ddlMarca.Enabled = !desabilitar;
            txtDescricao.Enabled = !desabilitar;
            txtURL.Enabled = !desabilitar;
            txtURLDrivers.Enabled = !desabilitar;
            uploadFoto.Enabled = !desabilitar;
            txtCaixa.Enabled = !desabilitar;
            txtProcessador.Enabled = !desabilitar;
            txtMotherboard.Enabled = !desabilitar;
            txtRAM.Enabled = !desabilitar;
            txtArmazenamento.Enabled = !desabilitar;
            txtPlacaGrafica.Enabled = !desabilitar;
            txtFonteAlimentacao.Enabled = !desabilitar;
            txtPortasUSB.Enabled = !desabilitar;
            txtPortasHDMI.Enabled = !desabilitar;
            chkPortaVGA.Enabled = !desabilitar;
            chkDisplayPort.Enabled = !desabilitar;
            chkPortaEthernet.Enabled = !desabilitar;
            chkWiFi.Enabled = !desabilitar;
        }

        void ObterMarcas()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IDMarca, Nome FROM Marca ORDER BY Nome", con))
                {
                    con.Open();
                    listMarca.DataSource = cmd.ExecuteReader();
                    listMarca.DataTextField = "Nome";
                    listMarca.DataValueField = "IDMarca";
                    listMarca.DataBind();
                }
            }
            listMarca.Items.Insert(0, new ListItem("-- selecione a marca --", ""));
        }

        void ObterMarcasFormulario()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IDMarca, Nome FROM Marca ORDER BY Nome", con))
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

        void ObterComputadores()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string sql = @"SELECT M.IDModelo, M.Nome, Ma.Nome AS Marca
                                  FROM Modelo M 
                                  INNER JOIN Marca Ma ON M.Marca = Ma.IDMarca
                                  INNER JOIN Computador C ON M.IDModelo = C.Modelo
                                  WHERE M.TipoEquipamento = 1";

                    // Só filtra por marca se uma marca válida foi selecionada
                    if (listMarca.SelectedIndex > 0 && !string.IsNullOrEmpty(listMarca.SelectedValue) && listMarca.SelectedValue != "")
                        sql += " AND M.Marca = @MarcaID";

                    sql += " ORDER BY M.Nome";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (listMarca.SelectedIndex > 0 && !string.IsNullOrEmpty(listMarca.SelectedValue) && listMarca.SelectedValue != "")
                            cmd.Parameters.AddWithValue("@MarcaID", listMarca.SelectedValue);

                        con.Open();
                        var reader = cmd.ExecuteReader();
                        
                        var dataTable = new System.Data.DataTable();
                        dataTable.Load(reader);
                        
                        gridComputadores.DataSource = dataTable;
                        gridComputadores.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao carregar computadores: {ex.Message}');", true);
            }
        }

        void InserirComputador()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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
                    con.Open();
                    
                    string sqlModelo = @"INSERT INTO Modelo (Nome, Marca, Descricao, URL, URLDrivers, TipoEquipamento, Foto)
                                       VALUES (@Nome, @Marca, @Descricao, @URL, @URLDrivers, 1, @Foto);
                                       SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    string idModelo;
                    using (SqlCommand cmd = new SqlCommand(sqlModelo, con))
                    {
                        cmd.Parameters.AddWithValue("@Nome", TruncarTexto(txtNome.Text.Trim(), 300));
                        cmd.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                        cmd.Parameters.AddWithValue("@Descricao", TruncarTexto(txtDescricao.Text.Trim(), 2500));
                        cmd.Parameters.AddWithValue("@URL", TruncarTexto(txtURL.Text.Trim(), 300));
                        cmd.Parameters.AddWithValue("@URLDrivers", TruncarTexto(txtURLDrivers.Text.Trim(), 500));
                        cmd.Parameters.AddWithValue("@Foto", (object)nomeFoto ?? DBNull.Value);
                        idModelo = cmd.ExecuteScalar().ToString();
                    }
                    
                    string sqlComputador = @"INSERT INTO Computador 
                                           (Caixa, Processador, Motherboard, RAM, Armazenamento, PlacaGrafica, 
                                            FonteAlimentacao, NumeroPortasUSB, NumeroPortasHDMI, PortaVGA, 
                                            DisplayPort, PortaEthernet, WiFi, Modelo)
                                           VALUES (@Caixa, @Processador, @Motherboard, @RAM, @Armazenamento, 
                                                   @PlacaGrafica, @FonteAlimentacao, @USB, @HDMI, @VGA, 
                                                   @DisplayPort, @Ethernet, @WiFi, @Modelo)";

                    using (SqlCommand cmd = new SqlCommand(sqlComputador, con))
                    {
                        string caixa = TruncarTexto(txtCaixa.Text, 150);
                        string processador = TruncarTexto(txtProcessador.Text, 200);
                        string motherboard = TruncarTexto(txtMotherboard.Text, 200);
                        string ram = TruncarTexto(txtRAM.Text, 200);
                        string armazenamento = TruncarTexto(txtArmazenamento.Text, 200);
                        string placaGrafica = TruncarTexto(txtPlacaGrafica.Text, 250);
                        string fonteAlimentacao = TruncarTexto(txtFonteAlimentacao.Text, 200);
                        
                        cmd.Parameters.AddWithValue("@Caixa", caixa);
                        cmd.Parameters.AddWithValue("@Processador", processador);
                        cmd.Parameters.AddWithValue("@Motherboard", motherboard);
                        cmd.Parameters.AddWithValue("@RAM", ram);
                        cmd.Parameters.AddWithValue("@Armazenamento", armazenamento);
                        cmd.Parameters.AddWithValue("@PlacaGrafica", placaGrafica);
                        cmd.Parameters.AddWithValue("@FonteAlimentacao", fonteAlimentacao);
                        cmd.Parameters.AddWithValue("@USB", byte.TryParse(txtPortasUSB.Text, out byte usb) ? usb : 0);
                        cmd.Parameters.AddWithValue("@HDMI", byte.TryParse(txtPortasHDMI.Text, out byte hdmi) ? hdmi : 0);
                        cmd.Parameters.AddWithValue("@VGA", chkPortaVGA.Checked);
                        cmd.Parameters.AddWithValue("@DisplayPort", chkDisplayPort.Checked);
                        cmd.Parameters.AddWithValue("@Ethernet", chkPortaEthernet.Checked);
                        cmd.Parameters.AddWithValue("@WiFi", chkWiFi.Checked);
                        cmd.Parameters.AddWithValue("@Modelo", idModelo);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Erro ao inserir computador: {ex.Message}');", true);
                throw;
            }
        }

        string TruncarTexto(string texto, int tamanhoMaximo)
        {
            if (string.IsNullOrEmpty(texto))
                return "";
            
            texto = texto.Trim();
            
            if (texto.Length > tamanhoMaximo)
            {
                return texto.Substring(0, tamanhoMaximo);
            }
            
            return texto;
        }

        void AtualizarComputador()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string idModelo = ViewState["idModelo"].ToString();
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
                con.Open();
                string sqlModelo = "UPDATE Modelo SET Nome = @Nome, Marca = @Marca, Descricao = @Descricao, " +
                                   "URL = @URL, URLDrivers = @URLDrivers" + 
                                   (atualizarFoto ? ", Foto = @Foto" : "") + " WHERE IDModelo = @IDModelo";

                using (SqlCommand cmd = new SqlCommand(sqlModelo, con))
                {
                    cmd.Parameters.AddWithValue("@Nome", TruncarTexto(txtNome.Text.Trim(), 300));
                    cmd.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                    cmd.Parameters.AddWithValue("@Descricao", TruncarTexto(txtDescricao.Text.Trim(), 2500));
                    cmd.Parameters.AddWithValue("@URL", TruncarTexto(txtURL.Text.Trim(), 300));
                    cmd.Parameters.AddWithValue("@URLDrivers", TruncarTexto(txtURLDrivers.Text.Trim(), 500));
                    cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                    if (atualizarFoto) cmd.Parameters.AddWithValue("@Foto", nomeFoto);
                    cmd.ExecuteNonQuery();
                }

                string sqlComputador = @"UPDATE Computador SET Caixa = @Caixa, Processador = @Processador, 
                                       Motherboard = @Motherboard, RAM = @RAM, Armazenamento = @Armazenamento, 
                                       PlacaGrafica = @PlacaGrafica, FonteAlimentacao = @FonteAlimentacao, 
                                       NumeroPortasUSB = @USB, NumeroPortasHDMI = @HDMI, PortaVGA = @VGA, 
                                       DisplayPort = @DisplayPort, PortaEthernet = @Ethernet, WiFi = @WiFi 
                                       WHERE Modelo = @Modelo";

                using (SqlCommand cmd = new SqlCommand(sqlComputador, con))
                {
                    cmd.Parameters.AddWithValue("@Caixa", TruncarTexto(txtCaixa.Text.Trim(), 150));
                    cmd.Parameters.AddWithValue("@Processador", TruncarTexto(txtProcessador.Text.Trim(), 200));
                    cmd.Parameters.AddWithValue("@Motherboard", TruncarTexto(txtMotherboard.Text.Trim(), 200));
                    cmd.Parameters.AddWithValue("@RAM", TruncarTexto(txtRAM.Text.Trim(), 200));
                    cmd.Parameters.AddWithValue("@Armazenamento", TruncarTexto(txtArmazenamento.Text.Trim(), 200));
                    cmd.Parameters.AddWithValue("@PlacaGrafica", TruncarTexto(txtPlacaGrafica.Text.Trim(), 250));
                    cmd.Parameters.AddWithValue("@FonteAlimentacao", TruncarTexto(txtFonteAlimentacao.Text.Trim(), 200));
                    cmd.Parameters.AddWithValue("@USB", byte.TryParse(txtPortasUSB.Text, out byte usb) ? usb : 0);
                    cmd.Parameters.AddWithValue("@HDMI", byte.TryParse(txtPortasHDMI.Text, out byte hdmi) ? hdmi : 0);
                    cmd.Parameters.AddWithValue("@VGA", chkPortaVGA.Checked);
                    cmd.Parameters.AddWithValue("@DisplayPort", chkDisplayPort.Checked);
                    cmd.Parameters.AddWithValue("@Ethernet", chkPortaEthernet.Checked);
                    cmd.Parameters.AddWithValue("@WiFi", chkWiFi.Checked);
                    cmd.Parameters.AddWithValue("@Modelo", idModelo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        void EliminarComputador(string idModelo)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                
                string sqlComputador = "DELETE FROM Computador WHERE Modelo = @IDModelo";
                using (SqlCommand cmd = new SqlCommand(sqlComputador, con))
                {
                    cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                    cmd.ExecuteNonQuery();
                }
                
                string sqlModelo = "DELETE FROM Modelo WHERE IDModelo = @IDModelo";
                using (SqlCommand cmd = new SqlCommand(sqlModelo, con))
                {
                    cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
