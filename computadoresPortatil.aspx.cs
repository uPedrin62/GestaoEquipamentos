using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class computadoresPortatil : System.Web.UI.Page
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
            try
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
                              C.Processador, C.Motherboard, C.RAM, C.Armazenamento, C.TamanhoEcra, 
                              C.Resolucao, C.EcraTatil, C.PlacaGrafica, C.ColunasImcorporadas, 
                              C.ColunasNumero, C.Auscultador, C.CamaraIncorporada, C.PortasHDMI, 
                              C.PortasVGA, C.PortasUSB
                              FROM Modelo M INNER JOIN ComputadorPortatil C ON M.IDModelo = C.Modelo
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
                            txtProcessador.Text = reader["Processador"].ToString();
                            txtMotherboard.Text = reader["Motherboard"].ToString();
                            txtRAM.Text = reader["RAM"].ToString();
                            txtArmazenamento.Text = reader["Armazenamento"].ToString();
                            txtPlacaGrafica.Text = reader["PlacaGrafica"].ToString();
                            txtTamanhoEcra.Text = reader["TamanhoEcra"].ToString();
                            txtResolucao.Text = reader["Resolucao"].ToString();
                            chkEcraTatil.Checked = Convert.ToBoolean(reader["EcraTatil"]);
                            chkColunasIncorporadas.Checked = Convert.ToBoolean(reader["ColunasImcorporadas"]);
                            txtColunasNumero.Text = reader["ColunasNumero"].ToString();
                            chkAuscultador.Checked = Convert.ToBoolean(reader["Auscultador"]);
                            chkCamaraIncorporada.Checked = Convert.ToBoolean(reader["CamaraIncorporada"]);
                            txtPortasUSB.Text = reader["PortasUSB"].ToString();
                            txtPortasHDMI.Text = reader["PortasHDMI"].ToString();
                            txtPortasVGA.Text = reader["PortasVGA"].ToString();

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
            txtProcessador.Text = "";
            txtMotherboard.Text = "";
            txtRAM.Text = "";
            txtArmazenamento.Text = "";
            txtPlacaGrafica.Text = "";
            txtTamanhoEcra.Text = "";
            txtResolucao.Text = "";
            chkEcraTatil.Checked = false;
            chkColunasIncorporadas.Checked = false;
            txtColunasNumero.Text = "";
            chkAuscultador.Checked = false;
            chkCamaraIncorporada.Checked = false;
            txtPortasUSB.Text = "";
            txtPortasHDMI.Text = "";
            txtPortasVGA.Text = "";
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
            txtProcessador.Enabled = !desabilitar;
            txtMotherboard.Enabled = !desabilitar;
            txtRAM.Enabled = !desabilitar;
            txtArmazenamento.Enabled = !desabilitar;
            txtPlacaGrafica.Enabled = !desabilitar;
            txtTamanhoEcra.Enabled = !desabilitar;
            txtResolucao.Enabled = !desabilitar;
            chkEcraTatil.Enabled = !desabilitar;
            chkColunasIncorporadas.Enabled = !desabilitar;
            txtColunasNumero.Enabled = !desabilitar;
            chkAuscultador.Enabled = !desabilitar;
            chkCamaraIncorporada.Enabled = !desabilitar;
            txtPortasUSB.Enabled = !desabilitar;
            txtPortasHDMI.Enabled = !desabilitar;
            txtPortasVGA.Enabled = !desabilitar;
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
                                  INNER JOIN ComputadorPortatil C ON M.IDModelo = C.Modelo
                                  WHERE M.TipoEquipamento = 2 AND M.Nome IS NOT NULL AND M.Nome != ''
                                  AND C.Processador IS NOT NULL AND C.Processador != ''";

                    if (listMarca.SelectedIndex > 0 && !string.IsNullOrEmpty(listMarca.SelectedValue))
                        sql += " AND M.Marca = @MarcaID";

                    sql += " ORDER BY M.Nome";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (listMarca.SelectedIndex > 0 && !string.IsNullOrEmpty(listMarca.SelectedValue))
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
                Response.Write($"<script>alert('Erro ao carregar computadores: {ex.Message}');</script>");
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
                                       VALUES (@Nome, @Marca, @Descricao, @URL, @URLDrivers, 2, @Foto);
                                       SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    string idModelo;
                    using (SqlCommand cmd = new SqlCommand(sqlModelo, con))
                    {
                        cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                        cmd.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                        cmd.Parameters.AddWithValue("@Descricao", txtDescricao.Text.Trim());
                        cmd.Parameters.AddWithValue("@URL", txtURL.Text.Trim());
                        cmd.Parameters.AddWithValue("@URLDrivers", txtURLDrivers.Text.Trim());
                        cmd.Parameters.AddWithValue("@Foto", (object)nomeFoto ?? DBNull.Value);
                        idModelo = cmd.ExecuteScalar().ToString();
                    }
                    
                    string sqlComputador = @"INSERT INTO ComputadorPortatil 
                                           (Processador, Motherboard, RAM, Armazenamento, TamanhoEcra, Resolucao, 
                                            EcraTatil, PlacaGrafica, ColunasImcorporadas, ColunasNumero, Auscultador, 
                                            CamaraIncorporada, PortasHDMI, PortasVGA, PortasUSB, Modelo)
                                           VALUES (@Processador, @Motherboard, @RAM, @Armazenamento, @TamanhoEcra, 
                                                   @Resolucao, @EcraTatil, @PlacaGrafica, @ColunasImcorporadas, 
                                                   @ColunasNumero, @Auscultador, @CamaraIncorporada, @PortasHDMI, 
                                                   @PortasVGA, @PortasUSB, @Modelo)";

                    using (SqlCommand cmd = new SqlCommand(sqlComputador, con))
                    {
                        cmd.Parameters.AddWithValue("@Processador", txtProcessador.Text.Trim());
                        cmd.Parameters.AddWithValue("@Motherboard", txtMotherboard.Text.Trim());
                        cmd.Parameters.AddWithValue("@RAM", txtRAM.Text.Trim());
                        cmd.Parameters.AddWithValue("@Armazenamento", txtArmazenamento.Text.Trim());
                        cmd.Parameters.AddWithValue("@TamanhoEcra", txtTamanhoEcra.Text.Trim());
                        cmd.Parameters.AddWithValue("@Resolucao", txtResolucao.Text.Trim());
                        cmd.Parameters.AddWithValue("@EcraTatil", chkEcraTatil.Checked);
                        cmd.Parameters.AddWithValue("@PlacaGrafica", txtPlacaGrafica.Text.Trim());
                        cmd.Parameters.AddWithValue("@ColunasImcorporadas", chkColunasIncorporadas.Checked);
                        cmd.Parameters.AddWithValue("@ColunasNumero", byte.TryParse(txtColunasNumero.Text, out byte cn) ? cn : 0);
                        cmd.Parameters.AddWithValue("@Auscultador", chkAuscultador.Checked);
                        cmd.Parameters.AddWithValue("@CamaraIncorporada", chkCamaraIncorporada.Checked);
                        cmd.Parameters.AddWithValue("@PortasHDMI", byte.TryParse(txtPortasHDMI.Text, out byte ph) ? ph : 0);
                        cmd.Parameters.AddWithValue("@PortasVGA", byte.TryParse(txtPortasVGA.Text, out byte pv) ? pv : 0);
                        cmd.Parameters.AddWithValue("@PortasUSB", byte.TryParse(txtPortasUSB.Text, out byte pu) ? pu : 0);
                        cmd.Parameters.AddWithValue("@Modelo", idModelo);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Erro ao inserir computador: {ex.Message}');</script>");
                throw;
            }
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
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
                    cmd.Parameters.AddWithValue("@Marca", ddlMarca.SelectedValue);
                    cmd.Parameters.AddWithValue("@Descricao", txtDescricao.Text.Trim());
                    cmd.Parameters.AddWithValue("@URL", txtURL.Text.Trim());
                    cmd.Parameters.AddWithValue("@URLDrivers", txtURLDrivers.Text.Trim());
                    cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                    if (atualizarFoto) cmd.Parameters.AddWithValue("@Foto", nomeFoto);
                    cmd.ExecuteNonQuery();
                }

                string sqlComputador = @"UPDATE ComputadorPortatil SET Processador = @Processador, 
                                       Motherboard = @Motherboard, RAM = @RAM, Armazenamento = @Armazenamento, 
                                       TamanhoEcra = @TamanhoEcra, Resolucao = @Resolucao, EcraTatil = @EcraTatil, 
                                       PlacaGrafica = @PlacaGrafica, ColunasImcorporadas = @ColunasImcorporadas, 
                                       ColunasNumero = @ColunasNumero, Auscultador = @Auscultador, 
                                       CamaraIncorporada = @CamaraIncorporada, PortasHDMI = @PortasHDMI, 
                                       PortasVGA = @PortasVGA, PortasUSB = @PortasUSB WHERE Modelo = @Modelo";

                using (SqlCommand cmd = new SqlCommand(sqlComputador, con))
                {
                    cmd.Parameters.AddWithValue("@Processador", txtProcessador.Text.Trim());
                    cmd.Parameters.AddWithValue("@Motherboard", txtMotherboard.Text.Trim());
                    cmd.Parameters.AddWithValue("@RAM", txtRAM.Text.Trim());
                    cmd.Parameters.AddWithValue("@Armazenamento", txtArmazenamento.Text.Trim());
                    cmd.Parameters.AddWithValue("@TamanhoEcra", txtTamanhoEcra.Text.Trim());
                    cmd.Parameters.AddWithValue("@Resolucao", txtResolucao.Text.Trim());
                    cmd.Parameters.AddWithValue("@EcraTatil", chkEcraTatil.Checked);
                    cmd.Parameters.AddWithValue("@PlacaGrafica", txtPlacaGrafica.Text.Trim());
                    cmd.Parameters.AddWithValue("@ColunasImcorporadas", chkColunasIncorporadas.Checked);
                    cmd.Parameters.AddWithValue("@ColunasNumero", byte.TryParse(txtColunasNumero.Text, out byte cn) ? cn : 0);
                    cmd.Parameters.AddWithValue("@Auscultador", chkAuscultador.Checked);
                    cmd.Parameters.AddWithValue("@CamaraIncorporada", chkCamaraIncorporada.Checked);
                    cmd.Parameters.AddWithValue("@PortasHDMI", byte.TryParse(txtPortasHDMI.Text, out byte ph) ? ph : 0);
                    cmd.Parameters.AddWithValue("@PortasVGA", byte.TryParse(txtPortasVGA.Text, out byte pv) ? pv : 0);
                    cmd.Parameters.AddWithValue("@PortasUSB", byte.TryParse(txtPortasUSB.Text, out byte pu) ? pu : 0);
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
                
                // First delete from ComputadorPortatil table
                string sqlComputador = "DELETE FROM ComputadorPortatil WHERE Modelo = @IDModelo";
                using (SqlCommand cmd = new SqlCommand(sqlComputador, con))
                {
                    cmd.Parameters.AddWithValue("@IDModelo", idModelo);
                    cmd.ExecuteNonQuery();
                }
                
                // Then delete from Modelo table
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