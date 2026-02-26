using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class equipamentos : System.Web.UI.Page
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["opcao"] = "";
                ViewState["IDEquipamento"] = null;

                GetLocaisFiltro();
                GetTiposEquipamento();
                PreencherLocaisDropDown();

                OcultarFormulario();
            }
        }

        // UI helpers

        private void MostrarFormulario(string modo)
        {
            // modo: "ver" | "editar"
            formularioEquipamento.Visible = true;
            lblMsg.Text = "";

            painelVerEquipamento.Visible = (modo == "ver");
            painelEditarEquipamento.Visible = (modo == "editar");
        }

        private void OcultarFormulario()
        {
            formularioEquipamento.Visible = false;
            painelVerEquipamento.Visible = false;
            painelEditarEquipamento.Visible = false;
        }

        private void LimparFormulario()
        {
            ddlTipoEquipamento.ClearSelection();
            if (ddlTipoEquipamento.Items.Count > 0) ddlTipoEquipamento.SelectedIndex = 0;

            ddlMarca.Items.Clear();
            ddlMarca.Items.Insert(0, new ListItem("Marca", ""));

            ddlModelo.Items.Clear();
            ddlModelo.Items.Insert(0, new ListItem("Modelo", ""));

            txtNumero.Text = "";
            txtNumeroESO.Text = "";
            txtNumeroCMOdivelas.Text = "";

            txtDataEntrada.Text = "";
            txtDataAbate.Text = "";
            txtGarantiaAte.Text = "";

            txtDescricao.Text = "";
            ddlEstado.SelectedValue = "Operacional";

            // locais
            ddlLocalAtual.ClearSelection();
            if (ddlLocalAtual.Items.Count > 0) ddlLocalAtual.SelectedIndex = 0;
            txtDataEntradaLocal.Text = DateTime.Today.ToString("yyyy-MM-dd");

            gridHistoricoLocais.DataSource = null;
            gridHistoricoLocais.DataBind();
        }

        //private string TecnicoAtual()
        //{
        //    return Session["user"] as string; // IDTecnico
        //}

        //  binds base

        private void GetLocaisFiltro()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("SELECT IDLocal, Nome FROM Local ORDER BY Nome", con))
            {
                con.Open();
                DataTable t = new DataTable();
                using (SqlDataReader r = cmd.ExecuteReader()) t.Load(r);

                listLocais.DataSource = t;
                listLocais.DataTextField = "Nome";
                listLocais.DataValueField = "IDLocal";
                listLocais.DataBind();
                listLocais.Items.Insert(0, new ListItem("Selecione uma sala", ""));
            }
        }

        private void GetTiposEquipamento()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("SELECT ID, Nome FROM TipoEquipamento ORDER BY Nome", con))
            {
                con.Open();
                DataTable t = new DataTable();
                using (SqlDataReader r = cmd.ExecuteReader()) t.Load(r);

                ddlTipoEquipamento.DataSource = t;
                ddlTipoEquipamento.DataTextField = "Nome";
                ddlTipoEquipamento.DataValueField = "ID";
                ddlTipoEquipamento.DataBind();
                ddlTipoEquipamento.Items.Insert(0, new ListItem("Tipo de equipamento", ""));
            }

            ddlMarca.Items.Clear();
            ddlMarca.Items.Insert(0, new ListItem("Marca", ""));
            ddlModelo.Items.Clear();
            ddlModelo.Items.Insert(0, new ListItem("Modelo", ""));
        }

        private void PreencherLocaisDropDown()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("SELECT IDLocal, Nome FROM Local ORDER BY Nome", con))
            {
                con.Open();
                DataTable t = new DataTable();
                using (SqlDataReader r = cmd.ExecuteReader()) t.Load(r);

                ddlLocalAtual.DataSource = t;
                ddlLocalAtual.DataTextField = "Nome";
                ddlLocalAtual.DataValueField = "IDLocal";
                ddlLocalAtual.DataBind();
                ddlLocalAtual.Items.Insert(0, new ListItem("Selecione um local", ""));
            }
        }

        private void GetMarcas()
        {
            if (string.IsNullOrEmpty(ddlTipoEquipamento.SelectedValue))
            {
                ddlMarca.Items.Clear();
                ddlMarca.Items.Insert(0, new ListItem("Marca", ""));
                return;
            }

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT IDMarca, Nome
            FROM Marca
            WHERE IDMarca IN (SELECT Marca FROM Modelo WHERE TipoEquipamento = @tipo)
            ORDER BY Nome;", con))
            {
                cmd.Parameters.Add("@tipo", SqlDbType.Int).Value = Convert.ToInt32(ddlTipoEquipamento.SelectedValue);

                con.Open();
                DataTable t = new DataTable();
                using (SqlDataReader r = cmd.ExecuteReader()) t.Load(r);

                ddlMarca.DataSource = t;
                ddlMarca.DataTextField = "Nome";
                ddlMarca.DataValueField = "IDMarca";
                ddlMarca.DataBind();
                ddlMarca.Items.Insert(0, new ListItem("Marca", ""));
            }
        }

        private void GetModelos()
        {
            if (string.IsNullOrEmpty(ddlTipoEquipamento.SelectedValue) || string.IsNullOrEmpty(ddlMarca.SelectedValue))
            {
                ddlModelo.Items.Clear();
                ddlModelo.Items.Insert(0, new ListItem("Modelo", ""));
                return;
            }

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT IDModelo, Nome
            FROM Modelo
            WHERE Marca = @marca AND TipoEquipamento = @tipo
            ORDER BY Nome;", con))
            {
                cmd.Parameters.Add("@marca", SqlDbType.Int).Value = Convert.ToInt32(ddlMarca.SelectedValue);
                cmd.Parameters.Add("@tipo", SqlDbType.Int).Value = Convert.ToInt32(ddlTipoEquipamento.SelectedValue);

                con.Open();
                DataTable t = new DataTable();
                using (SqlDataReader r = cmd.ExecuteReader()) t.Load(r);

                ddlModelo.DataSource = t;
                ddlModelo.DataTextField = "Nome";
                ddlModelo.DataValueField = "IDModelo";
                ddlModelo.DataBind();
                ddlModelo.Items.Insert(0, new ListItem("Modelo", ""));
            }
        }

        // obter dados dos equipamentos que se encontram no local

        protected void listLocais_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                string query;
                SqlCommand cmd;

                if (string.IsNullOrEmpty(listLocais.SelectedValue))
                {
                    // Mostra TODOS os equipamentos
                    query = @"
                    SELECT
                        E.IDEquipamento,
                        T.Nome  AS Tipo,
                        MC.Nome AS Marca,
                        MD.Nome AS Modelo,
                        E.Numero,
                        E.NumeroESO,
                        ISNULL(L.Nome, '(Sem local)') AS LocalAtual,
                        ISNULL(E.Estado, 'Operacional') AS Estado
                    FROM Equipamento E
                    INNER JOIN Modelo MD         ON MD.IDModelo = E.Modelo
                    INNER JOIN Marca MC          ON MC.IDMarca = MD.Marca
                    INNER JOIN TipoEquipamento T ON T.ID = MD.TipoEquipamento
                    LEFT JOIN LocalEquipamento LE ON LE.Equipamento = E.IDEquipamento AND LE.DataSaida IS NULL
                    LEFT JOIN Local L ON L.IDLocal = LE.Local
                    ORDER BY T.Nome, MC.Nome, MD.Nome;";
                    
                    cmd = new SqlCommand(query, con);
                }
                else
                {
                    // Mostra equipamentos do local selecionado
                    query = @"
                    SELECT
                        E.IDEquipamento,
                        T.Nome  AS Tipo,
                        MC.Nome AS Marca,
                        MD.Nome AS Modelo,
                        E.Numero,
                        E.NumeroESO,
                        ISNULL(L.Nome, '(Sem local)') AS LocalAtual,
                        ISNULL(E.Estado, 'Operacional') AS Estado
                    FROM LocalEquipamento LE
                    INNER JOIN Equipamento E     ON E.IDEquipamento = LE.Equipamento
                    INNER JOIN Modelo MD         ON MD.IDModelo = E.Modelo
                    INNER JOIN Marca MC          ON MC.IDMarca = MD.Marca
                    INNER JOIN TipoEquipamento T ON T.ID = MD.TipoEquipamento
                    LEFT JOIN Local L            ON L.IDLocal = LE.Local
                    WHERE LE.Local = @local AND LE.DataSaida IS NULL
                    ORDER BY T.Nome, MC.Nome, MD.Nome;";
                    
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@local", SqlDbType.Int).Value = Convert.ToInt32(listLocais.SelectedValue);
                }

                con.Open();
                DataTable t = new DataTable();
                using (SqlDataReader r = cmd.ExecuteReader()) t.Load(r);

                gridEquipamentos.DataSource = t;
                gridEquipamentos.DataBind();
            }
        }

        // eventos botões - criar, editar e ver

        protected void buttonCriarEquipamento_Click(object sender, EventArgs e)
        {
            ViewState["opcao"] = "i";
            ViewState["IDEquipamento"] = null;

            LimparFormulario();

            // se já houver local selecionado no filtro (listLocais), pré-seleciona no form referente aos locais
            if (!string.IsNullOrEmpty(listLocais.SelectedValue) && ddlLocalAtual.Items.FindByValue(listLocais.SelectedValue) != null)
                ddlLocalAtual.SelectedValue = listLocais.SelectedValue;

            MostrarFormulario("editar");
        }

        protected void buttonEditarEquipamento_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridEquipamentos.SelectedIndex < 0 || gridEquipamentos.SelectedDataKey == null || gridEquipamentos.SelectedDataKey.Value == null)
                {
                    lblMsg.Text = "Selecione um equipamento na lista primeiro.";
                    return;
                }

                int idEquipamento = Convert.ToInt32(gridEquipamentos.SelectedDataKey.Value);

                ViewState["opcao"] = "u";
                ViewState["IDEquipamento"] = idEquipamento;

                LimparFormulario();
                PreencherFormularioParaEdicao(idEquipamento);

                MostrarFormulario("editar");
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Erro: " + ex.Message;
            }
        }

        protected void buttonVerEquipamento_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridEquipamentos.SelectedIndex < 0 || gridEquipamentos.SelectedDataKey == null || gridEquipamentos.SelectedDataKey.Value == null)
                {
                    lblMsg.Text = "Selecione um equipamento na lista primeiro.";
                    return;
                }

                int idEquipamento = Convert.ToInt32(gridEquipamentos.SelectedDataKey.Value);

                LimparLabelsVer();
                PreencherLabelsVerEquipamento(idEquipamento);

                MostrarFormulario("ver");
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Erro: " + ex.Message;
            }
        }

        protected void buttonFechar_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            LimparFormulario();
            OcultarFormulario();
            gridEquipamentos.SelectedIndex = -1;
        }

        // preencher formulário para editar

        private void PreencherFormularioParaEdicao(int idEquipamento)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT
                E.IDEquipamento,
                E.Modelo AS IDModelo,
                MD.Marca AS IDMarca,
                MD.TipoEquipamento AS IDTipo,
                E.Numero,
                E.NumeroESO,
                E.NumeroCMOdivelas,
                E.DataEntrada,
                E.DataAbate,
                E.GarantiaAte,
                E.Descricao,
                E.Estado
            FROM Equipamento E
            JOIN Modelo MD ON MD.IDModelo = E.Modelo
            WHERE E.IDEquipamento = @id;", con))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idEquipamento;

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return;

                    string idTipo = r["IDTipo"] == DBNull.Value ? "" : r["IDTipo"].ToString();
                    string idMarca = r["IDMarca"] == DBNull.Value ? "" : r["IDMarca"].ToString();
                    string idModelo = r["IDModelo"] == DBNull.Value ? "" : r["IDModelo"].ToString();

                    // Define o tipo primeiro
                    if (!string.IsNullOrEmpty(idTipo) && ddlTipoEquipamento.Items.FindByValue(idTipo) != null)
                    {
                        ddlTipoEquipamento.SelectedValue = idTipo;
                    }
                    
                    // Carrega as marcas baseado no tipo
                    GetMarcas();
                    
                    // Define a marca
                    if (!string.IsNullOrEmpty(idMarca) && ddlMarca.Items.FindByValue(idMarca) != null)
                    {
                        ddlMarca.SelectedValue = idMarca;
                    }
                    
                    // Carrega os modelos baseado na marca e tipo
                    GetModelos();
                    
                    // Define o modelo
                    if (!string.IsNullOrEmpty(idModelo) && ddlModelo.Items.FindByValue(idModelo) != null)
                    {
                        ddlModelo.SelectedValue = idModelo;
                    }

                    txtNumero.Text = r["Numero"] == DBNull.Value ? "" : r["Numero"].ToString();
                    txtNumeroESO.Text = r["NumeroESO"] == DBNull.Value ? "" : r["NumeroESO"].ToString();
                    txtNumeroCMOdivelas.Text = r["NumeroCMOdivelas"] == DBNull.Value ? "" : r["NumeroCMOdivelas"].ToString();

                    txtDataEntrada.Text = r["DataEntrada"] == DBNull.Value ? "" : ((DateTime)r["DataEntrada"]).ToString("yyyy-MM-dd");
                    txtDataAbate.Text = r["DataAbate"] == DBNull.Value ? "" : ((DateTime)r["DataAbate"]).ToString("yyyy-MM-dd");
                    txtGarantiaAte.Text = r["GarantiaAte"] == DBNull.Value ? "" : ((DateTime)r["GarantiaAte"]).ToString("yyyy-MM-dd");

                    txtDescricao.Text = r["Descricao"] == DBNull.Value ? "" : r["Descricao"].ToString();

                    string estado = r["Estado"] == DBNull.Value ? "Operacional" : r["Estado"].ToString();
                    if (ddlEstado.Items.FindByValue(estado) != null) ddlEstado.SelectedValue = estado;
                    else ddlEstado.SelectedValue = "Operacional";
                }
            }

            CarregarHistoricoLocais(idEquipamento);

            int? localAtual = ObterLocalAtual(idEquipamento);
            if (localAtual.HasValue && ddlLocalAtual.Items.FindByValue(localAtual.Value.ToString()) != null)
                ddlLocalAtual.SelectedValue = localAtual.Value.ToString();

            if (string.IsNullOrWhiteSpace(txtDataEntradaLocal.Text))
                txtDataEntradaLocal.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        // obter histórico de locais

        private void CarregarHistoricoLocais(int idEquipamento)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT
                L.Nome AS LocalNome,
                LE.DataEntrada,
                LE.DataSaida,
                TE.Nome AS TecnicoEntrada,
                TS.Nome AS TecnicoSaida,
                LE.DataSaida AS DataFimProp
            FROM LocalEquipamento LE
            INNER JOIN Local L ON L.IDLocal = LE.Local
            LEFT JOIN Tecnico TE ON TE.IDTecnico = LE.TecnicoEntrada
            LEFT JOIN Tecnico TS ON TS.IDTecnico = LE.TecnicoSaida
            WHERE LE.Equipamento = @id
            ORDER BY
                CASE WHEN LE.DataSaida IS NULL THEN 0 ELSE 1 END,
                LE.DataEntrada DESC;", con))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idEquipamento;

                con.Open();
                DataTable t = new DataTable();
                using (SqlDataReader r = cmd.ExecuteReader())
                    t.Load(r);

                gridHistoricoLocais.DataSource = t;
                gridHistoricoLocais.DataBind();
            }
        }

        private int? ObterLocalAtual(int idEquipamento)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 1 Local
            FROM LocalEquipamento
            WHERE Equipamento = @id AND DataSaida IS NULL
            ORDER BY DataEntrada DESC;", con))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idEquipamento;
                con.Open();

                object v = cmd.ExecuteScalar();
                if (v == null || v == DBNull.Value) return null;
                return Convert.ToInt32(v);
            }
        }

        // equipamentos + locais - guardar (insert/update)

        protected void buttonGuardarEquipamento_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            string opcao = ViewState["opcao"] as string;

            // obter ID do técnico da sessão
            string tecnicoId = Session["user"] == null ? null : Session["user"].ToString();

            if (string.IsNullOrWhiteSpace(tecnicoId))
            {
                lblMsg.Text = "Sessão expirada ou utilizador não definido.";
                return;
            }

            if (string.IsNullOrEmpty(ddlLocalAtual.SelectedValue))
            {
                lblMsg.Text = "Selecione o local atual do equipamento.";
                return;
            }

            DateTime dataEntradaLocal;
            if (!DateTime.TryParse(txtDataEntradaLocal.Text, out dataEntradaLocal))
            {
                lblMsg.Text = "Indique uma data válida para a entrada no local.";
                return;
            }

            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    try
                    {
                        int idEquipamento;

                        if (opcao == "i")
                        {
                            idEquipamento = InserirEquipamentoRetornaId(con, tx);
                            ViewState["IDEquipamento"] = idEquipamento;
                            ViewState["opcao"] = "u";
                        }
                        else if (opcao == "u" && ViewState["IDEquipamento"] != null)
                        {
                            idEquipamento = Convert.ToInt32(ViewState["IDEquipamento"]);
                            AtualizarEquipamento(con, tx, idEquipamento);
                        }
                        else
                        {
                            lblMsg.Text = "Selecione Criar ou Editar antes de guardar.";
                            tx.Rollback();
                            return;
                        }

                        int idLocalNovo = Convert.ToInt32(ddlLocalAtual.SelectedValue);

                        DefinirOuMudarLocal(
                            con,
                            tx,
                            idEquipamento,
                            idLocalNovo,
                            dataEntradaLocal,
                            tecnicoId
                        );

                        tx.Commit();

                        // Seleciona o local do equipamento no dropdown
                        if (listLocais.Items.FindByValue(idLocalNovo.ToString()) != null)
                        {
                            listLocais.SelectedValue = idLocalNovo.ToString();
                        }

                        // Recarrega a grid para mostrar o equipamento
                        listLocais_SelectedIndexChanged(null, null);

                        // Limpa seleção e formulário
                        gridEquipamentos.SelectedIndex = -1;
                        LimparFormulario();
                        OcultarFormulario();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        lblMsg.Text = "Erro ao guardar: " + ex.Message;
                    }
                }
            }
        }

        private int InserirEquipamentoRetornaId(SqlConnection con, SqlTransaction tx)
        {
            using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO Equipamento
            (
                Modelo, Numero, NumeroESO, NumeroCMOdivelas,
                DataEntrada, DataAbate, GarantiaAte, Descricao, Estado
            )
            OUTPUT INSERTED.IDEquipamento
            VALUES
            (
                @Modelo, @Numero, @NumeroESO, @NumeroCMOdivelas,
                @DataEntrada, @DataAbate, @GarantiaAte, @Descricao, @Estado
            );", con, tx))
            {
                cmd.Parameters.Add("@Modelo", SqlDbType.Int).Value =
                    string.IsNullOrEmpty(ddlModelo.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlModelo.SelectedValue);

                cmd.Parameters.Add("@Numero", SqlDbType.NVarChar, 25).Value = NullSeVazio(txtNumero.Text);
                cmd.Parameters.Add("@NumeroESO", SqlDbType.NVarChar, 25).Value = NullSeVazio(txtNumeroESO.Text);
                cmd.Parameters.Add("@NumeroCMOdivelas", SqlDbType.NVarChar, 25).Value = NullSeVazio(txtNumeroCMOdivelas.Text);
                cmd.Parameters.Add("@DataEntrada", SqlDbType.Date).Value = DataOuNull(txtDataEntrada.Text);
                cmd.Parameters.Add("@DataAbate", SqlDbType.Date).Value = DataOuNull(txtDataAbate.Text);
                cmd.Parameters.Add("@GarantiaAte", SqlDbType.Date).Value = DataOuNull(txtGarantiaAte.Text);
                cmd.Parameters.Add("@Descricao", SqlDbType.NVarChar, 2500).Value = NullSeVazio(txtDescricao.Text);
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 50).Value = ddlEstado.SelectedValue;

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void AtualizarEquipamento(SqlConnection con, SqlTransaction tx, int idEquipamento)
        {
            using (SqlCommand cmd = new SqlCommand(@"
            UPDATE Equipamento
            SET
                Modelo = @Modelo,
                Numero = @Numero,
                NumeroESO = @NumeroESO,
                NumeroCMOdivelas = @NumeroCMOdivelas,
                DataEntrada = @DataEntrada,
                DataAbate = @DataAbate,
                GarantiaAte = @GarantiaAte,
                Descricao = @Descricao,
                Estado = @Estado
            WHERE IDEquipamento = @IDEquipamento;", con, tx))
            {
                cmd.Parameters.Add("@IDEquipamento", SqlDbType.Int).Value = idEquipamento;

                cmd.Parameters.Add("@Modelo", SqlDbType.Int).Value =
                    string.IsNullOrEmpty(ddlModelo.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlModelo.SelectedValue);

                cmd.Parameters.Add("@Numero", SqlDbType.NVarChar, 25).Value = NullSeVazio(txtNumero.Text);
                cmd.Parameters.Add("@NumeroESO", SqlDbType.NVarChar, 25).Value = NullSeVazio(txtNumeroESO.Text);
                cmd.Parameters.Add("@NumeroCMOdivelas", SqlDbType.NVarChar, 25).Value = NullSeVazio(txtNumeroCMOdivelas.Text);
                cmd.Parameters.Add("@DataEntrada", SqlDbType.Date).Value = DataOuNull(txtDataEntrada.Text);
                cmd.Parameters.Add("@DataAbate", SqlDbType.Date).Value = DataOuNull(txtDataAbate.Text);
                cmd.Parameters.Add("@GarantiaAte", SqlDbType.Date).Value = DataOuNull(txtGarantiaAte.Text);
                cmd.Parameters.Add("@Descricao", SqlDbType.NVarChar, 2500).Value = NullSeVazio(txtDescricao.Text);
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 50).Value = ddlEstado.SelectedValue;

                cmd.ExecuteNonQuery();
            }
        }

        private void DefinirOuMudarLocal(SqlConnection con, SqlTransaction tx, int idEquipamento, int idLocalNovo, DateTime dataEntradaNovoLocal, string tecnicoId)
        {
            int? localAtual = null;

            using (SqlCommand cmdAtual = new SqlCommand(@"
            SELECT TOP 1 Local
            FROM LocalEquipamento
            WHERE Equipamento = @eq AND DataSaida IS NULL
            ORDER BY DataEntrada DESC;", con, tx))
            {
                cmdAtual.Parameters.Add("@eq", SqlDbType.Int).Value = idEquipamento;
                object v = cmdAtual.ExecuteScalar();
                if (v != null && v != DBNull.Value)
                    localAtual = Convert.ToInt32(v);
            }

            if (localAtual.HasValue && localAtual.Value == idLocalNovo)
                return;

            using (SqlCommand cmdFechar = new SqlCommand(@"
            UPDATE LocalEquipamento
            SET DataSaida = @dataSaida, TecnicoSaida = @tecSaida
            WHERE Equipamento = @eq AND DataSaida IS NULL;", con, tx))
            {
                cmdFechar.Parameters.Add("@dataSaida", SqlDbType.Date).Value = dataEntradaNovoLocal.Date;
                cmdFechar.Parameters.Add("@tecSaida", SqlDbType.NVarChar, 50).Value = tecnicoId;
                cmdFechar.Parameters.Add("@eq", SqlDbType.Int).Value = idEquipamento;
                cmdFechar.ExecuteNonQuery();
            }

            using (SqlCommand cmdInsert = new SqlCommand(@"
            INSERT INTO LocalEquipamento (Equipamento, Local, DataEntrada, TecnicoEntrada, DataSaida, TecnicoSaida)
            VALUES (@eq, @local, @dataEntrada, @tecEntrada, NULL, NULL);", con, tx))
            {
                cmdInsert.Parameters.Add("@eq", SqlDbType.Int).Value = idEquipamento;
                cmdInsert.Parameters.Add("@local", SqlDbType.Int).Value = idLocalNovo;
                cmdInsert.Parameters.Add("@dataEntrada", SqlDbType.Date).Value = dataEntradaNovoLocal.Date;
                cmdInsert.Parameters.Add("@tecEntrada", SqlDbType.NVarChar, 50).Value = tecnicoId;
                cmdInsert.ExecuteNonQuery();
            }
        }

        // validação de dados - string e date

        private object NullSeVazio(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return DBNull.Value;
            return valor.Trim();
        }

        private object DataOuNull(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return DBNull.Value;

            DateTime data;
            if (DateTime.TryParse(valor, out data))
                return data;

            return DBNull.Value;
        }

        // modo VER

        private void LimparLabelsVer()
        {
            lblVerTipo.Text = "";
            lblVerMarca.Text = "";
            lblVerModelo.Text = "";
            lblVerLocal.Text = "";

            lblVerNumero.Text = "";
            lblVerNumeroESO.Text = "";
            lblVerNumeroCMO.Text = "";

            lblVerDataEntrada.Text = "";
            lblVerDataAbate.Text = "";
            lblVerGarantiaAte.Text = "";
            lblVerEstado.Text = "";
            lblVerDescricao.Text = "";

            rowVerDataAbate.Visible = false;
        }

        private void PreencherLabelsVerEquipamento(int idEquipamento)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT
                E.Numero,
                E.NumeroESO,
                E.NumeroCMOdivelas,
                E.DataEntrada,
                E.DataAbate,
                E.GarantiaAte,
                E.Descricao,
                E.Estado,
                T.Nome  AS TipoNome,
                MC.Nome AS MarcaNome,
                MD.Nome AS ModeloNome,
                L.Nome  AS LocalNome
            FROM Equipamento E
            JOIN Modelo MD ON MD.IDModelo = E.Modelo
            JOIN Marca MC ON MC.IDMarca = MD.Marca
            JOIN TipoEquipamento T ON T.ID = MD.TipoEquipamento
            LEFT JOIN LocalEquipamento LE ON LE.Equipamento = E.IDEquipamento AND LE.DataSaida IS NULL
            LEFT JOIN Local L ON L.IDLocal = LE.Local
            WHERE E.IDEquipamento = @id;", con))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idEquipamento;

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return;

                    lblVerTipo.Text = r["TipoNome"] == DBNull.Value ? "" : r["TipoNome"].ToString();
                    lblVerMarca.Text = r["MarcaNome"] == DBNull.Value ? "" : r["MarcaNome"].ToString();
                    lblVerModelo.Text = r["ModeloNome"] == DBNull.Value ? "" : r["ModeloNome"].ToString();
                    lblVerLocal.Text = r["LocalNome"] == DBNull.Value ? "(Sem local atribuído)" : r["LocalNome"].ToString();

                    lblVerNumero.Text = r["Numero"] == DBNull.Value ? "" : r["Numero"].ToString();
                    lblVerNumeroESO.Text = r["NumeroESO"] == DBNull.Value ? "" : r["NumeroESO"].ToString();
                    lblVerNumeroCMO.Text = r["NumeroCMOdivelas"] == DBNull.Value ? "" : r["NumeroCMOdivelas"].ToString();

                    lblVerDataEntrada.Text = r["DataEntrada"] == DBNull.Value ? "" : ((DateTime)r["DataEntrada"]).ToString("yyyy-MM-dd");
                    lblVerGarantiaAte.Text = r["GarantiaAte"] == DBNull.Value ? "" : ((DateTime)r["GarantiaAte"]).ToString("yyyy-MM-dd");

                    string estado = r["Estado"] == DBNull.Value ? "" : r["Estado"].ToString();
                    lblVerEstado.Text = estado;

                    // DataAbate só se estado for "Inutilizado"
                    if (!string.IsNullOrEmpty(estado) && estado.Trim().ToLower() == "inutilizado")
                    {
                        rowVerDataAbate.Visible = true;
                        lblVerDataAbate.Text = r["DataAbate"] == DBNull.Value ? "" : ((DateTime)r["DataAbate"]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        rowVerDataAbate.Visible = false;
                    }

                    lblVerDescricao.Text = r["Descricao"] == DBNull.Value ? "" : r["Descricao"].ToString();
                }
            }
        }

        // dropdown events

        //quando é selecionado um tipo de equipamento, preenche Marcas (apenas com as marcas que têm equipamentos desse tipo)
        protected void ddlTipoEquipamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetMarcas();
            GetModelos();
        }

        //quando é selecionada uma marca, preenche com os modelos correspondentes (em função do tipo de equipamento)
        protected void ddlMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetModelos();
        }
    }
}