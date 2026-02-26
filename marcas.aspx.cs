using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace esoEquipamentos26.administrador
{
    public partial class marcas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //if (!IsPostBack)
            //{
            //    DataTable dtMarcas = ObterMarcas();
            //    // Aqui, mais tarde, vamos ligar ao GridView ou outro controlo
            //}

            if (!IsPostBack)
            {
                CarregarMarcas(); //mostra as marcas existentes no gridview
            }


            //Mostrar mensagem de sucesso da session
            if (Session["MensagemSucesso"] != null)
            {
                lblMensagem.Text = Session["MensagemSucesso"].ToString();
                lblMensagem.CssClass = "alert alert-success";
                lblMensagem.Visible = true;
                Session["MensagemSucesso"] = null; // Limpa a mensagem após exibir
            }

            if (IsPostBack)
            {
                CarregarMarcas();
            }
            // Limpa mensagem sempre que a página carrega (exceto no postback do botão inserir)
            //lblMensagem.Text = "";


        }


























        //método para adicionar uma nova marca:

        //private void InserirMarca(string nome)
        //{
        //    string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //    using (SqlConnection con = new SqlConnection(cs))
        //    {
        //        string sql = "INSERT INTO Marca (Nome) VALUES (@Nome)";

        //        using (SqlCommand cmd = new SqlCommand(sql, con))
        //        {
        //            cmd.Parameters.AddWithValue("@Nome", nome);
        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        private void InserirMarca(
        string nome,
        string morada,
        string codigoPostal,
        string email,
        string telefone,
        string localidade)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"INSERT INTO Marca
                       (Nome, Morada, CodigoPostal, Email, Telefone, Localidade)
                       VALUES (@Nome, @Morada, @CodigoPostal, @Email, @Telefone, @Localidade)";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Morada", morada);
                    cmd.Parameters.AddWithValue("@CodigoPostal", codigoPostal);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Telefone", telefone);
                    cmd.Parameters.AddWithValue("@Localidade", localidade);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

            }
        }


        //método para atualizar uma marca

        //private void AtualizarMarca(int id, string novoNome)
        //{
        //    string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //    using (SqlConnection con = new SqlConnection(cs))
        //    {
        //        string sql = "UPDATE Marca SET Nome = @Nome WHERE Id = @Id";

        //        using (SqlCommand cmd = new SqlCommand(sql, con))
        //        {
        //            cmd.Parameters.AddWithValue("@Nome", novoNome);
        //            cmd.Parameters.AddWithValue("@Id", id);
        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        private void AtualizarMarca(
        int idMarca,
        string nome,
        string morada,
        string codigoPostal,
        string email,
        string telefone,
        string localidade)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"UPDATE Marca
                       SET Nome = @Nome,
                           Morada = @Morada,
                           CodigoPostal = @CodigoPostal,
                           Email = @Email,
                           Telefone = @Telefone,
                           Localidade = @Localidade
                       WHERE IDMarca = @IDMarca";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IDMarca", idMarca);
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Morada", morada);
                    cmd.Parameters.AddWithValue("@CodigoPostal", codigoPostal);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Telefone", telefone);
                    cmd.Parameters.AddWithValue("@Localidade", localidade);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }








        //método para apagar uma marca
        //private void ApagarMarca(int id)
        //{
        //    string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //    using (SqlConnection con = new SqlConnection(cs))
        //    {
        //        string sql = "DELETE FROM Marcas WHERE Id = @Id";

        //        using (SqlCommand cmd = new SqlCommand(sql, con))
        //        {
        //            cmd.Parameters.AddWithValue("@Id", id);
        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}


        private void EliminarMarca(int idMarca)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                
                // Verificar se a marca está sendo usada em algum modelo
                string sqlVerificar = "SELECT COUNT(*) FROM Modelo WHERE Marca = @IDMarca";
                using (SqlCommand cmdVerificar = new SqlCommand(sqlVerificar, con))
                {
                    cmdVerificar.Parameters.AddWithValue("@IDMarca", idMarca);
                    int count = (int)cmdVerificar.ExecuteScalar();
                    
                    if (count > 0)
                    {
                        throw new Exception($"Não é possível eliminar esta marca porque existem {count} modelo(s) associado(s) a ela. Elimine primeiro os modelos.");
                    }
                }
                
                // Se não houver modelos associados, eliminar a marca
                string sql = "DELETE FROM Marca WHERE IDMarca = @IDMarca";
                using (SqlCommand comandoSql = new SqlCommand(sql, con))
                {
                    comandoSql.Parameters.AddWithValue("@IDMarca", idMarca);
                    comandoSql.ExecuteNonQuery();
                }
            }
        }

        //retorna todos os dados da tabela Marcas.
        //private DataTable ObterMarcas()
        //{
        //    DataTable dt = new DataTable();

        //    string cs = ConfigurationManager
        //                    .ConnectionStrings["DefaultConnection"]
        //                    .ConnectionString;

        //    using (SqlConnection con = new SqlConnection(cs))

        //    {

        //        string sql = "SELECT * FROM Marcas"; // nome da tabela

        //        using (SqlCommand cmd = new SqlCommand(sql, con))
        //        {
        //            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //            {
        //                da.Fill(dt);
        //            }
        //        }
        //    }

        //    return dt;
        //}


        //método que obtém todas as marcas da tabela Marca na base de dados e retorna num DataTable
        private DataTable ObterMarcas()
        {
            // tabela de memória para guardar os dados
            DataTable dt = new DataTable();

            //Procura ligar-se à base de dados através da connection string
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            //Procura conectar ao SQL Server através da connection string
            using (SqlConnection conectarsql = new SqlConnection(cs))
            {

                //buscar todas marcas
                string sql = "SELECT * FROM Marca"; // tabela correcta

                using (SqlCommand comandoSql = new SqlCommand(sql, conectarsql))   //cria um comando SQL com este texto e usa esta ligação para executá-lo
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(comandoSql))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }


        //Método que busca os dados do ObterMarcas e insere no GridView para  mostrar  na página
        private void CarregarMarcas()
        {
            // Liga os dados obtidos do método ObterMarcas() ao GridView
            gvMarcas.DataSource = null;  // Limpa a fonte de dados existente
            gvMarcas.DataSource = ObterMarcas();  // Obter todos os registros da tabela Marca
            gvMarcas.DataBind();                  // Atualiza o GridView para mostrar os dados na página
        }





        protected void btnInserirMarca_Click(object sender, EventArgs e)
        {
            try
            {
                //Leitura dos valores dos campos de texto
                string nome = txtNome.Text.Trim();
                string morada = txtMorada.Text.Trim();
                string codigoPostal = txtCodigoPostal.Text.Trim();
                string email = txtEmail.Text.Trim();
                string telefone = txtTelefone.Text.Trim();
                string localidade = txtLocalidade.Text;

                //Validar os campos
                //nome
                if (string.IsNullOrWhiteSpace(nome))
                {
                    lblMensagem.Text = "Insira o nome da marca!";
                    lblMensagem.CssClass = "alert alert-danger";
                    lblMensagem.Visible = true;

                    txtNome.BorderColor = System.Drawing.Color.Red;
                    txtNome.Focus();
                    return;
                }


                //Morada
                if (string.IsNullOrWhiteSpace(morada))
                {
                    lblMensagem.Text = "Insira a morada da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtMorada.BorderColor = System.Drawing.Color.Red;
                    txtMorada.Focus();
                    return;
                }


                //Código Postal
                //Validação
                if (string.IsNullOrWhiteSpace(codigoPostal))
                {
                    lblMensagem.Text = "Insira o código postal da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtCodigoPostal.BorderColor = System.Drawing.Color.Red;
                    txtCodigoPostal.Focus();
                    return;
                }

                //Verificar o cp no formato português
                if (!System.Text.RegularExpressions.Regex.IsMatch(codigoPostal, @"^\d{4}-\d{3}$"))
                {
                    lblMensagem.Text = "Código postal inválido! Use o formato 1234-567.";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtCodigoPostal.BorderColor = System.Drawing.Color.Red;
                    txtCodigoPostal.Focus();
                    return;
                }

                //Email
                //Validação
                if (string.IsNullOrWhiteSpace(email))
                {
                    lblMensagem.Text = "Insira o email da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtEmail.BorderColor = System.Drawing.Color.Red;
                    txtEmail.Focus();
                    return;
                }

                //Verificar o formato do email
                if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    lblMensagem.Text = "Email inválido! Exemplo: exemplo@gmail.com";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtEmail.BorderColor = System.Drawing.Color.Red;
                    txtEmail.Focus();
                    return;
                }


                //Telefone
                //Validação
                if (string.IsNullOrWhiteSpace(telefone))
                {
                    lblMensagem.Text = "Insira o telefone da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtTelefone.BorderColor = System.Drawing.Color.Red;
                    txtTelefone.Focus();
                    return;
                }

                // Validar formato português (fixo, móvel e opcional +351)
                if (!System.Text.RegularExpressions.Regex.IsMatch(telefone, @"^(\+351)?(2\d{8})|(9\d{8})$"))
                {
                    lblMensagem.Text = "Telefone inválido! Deve ser fixo (começa com 2) ou móvel (começa com 9), opcional +351.";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtTelefone.BorderColor = System.Drawing.Color.Red;
                    txtTelefone.Focus();
                    return;
                }


                //Localidade
                if (string.IsNullOrWhiteSpace(localidade))
                {
                    lblMensagem.Text = "Insira a localidade da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtLocalidade.BorderColor = System.Drawing.Color.Red;
                    txtLocalidade.Focus();
                    return;
                }

                //Chamar o método que insere na base de dados 
                InserirMarca(nome, morada, codigoPostal, email, telefone, localidade);

                //Mensagem de sucesso (no output do VS)
                System.Diagnostics.Debug.WriteLine("Marca inserida com sucesso.");

                // Mensagem de sucesso visível no formulário
                lblMensagem.Text = "Marca inserida com sucesso!";
                lblMensagem.CssClass = "alert alert-success";
                lblMensagem.Visible = true;


                // Limpar campos
                txtNome.Text = "";
                txtMorada.Text = "";
                txtCodigoPostal.Text = "";
                txtEmail.Text = "";
                txtTelefone.Text = "";
                txtLocalidade.Text = "";


                // Atualizar a lista de marcas (se houver GridView ou Repeater)
                //ObterMarcas();

                CarregarMarcas(); //atualiza o gridview para mostrar a nova marca inserida na página

                Session["MensagemSucesso"] = "Marca inserida com sucesso!";

                //Evitar duplicação no refresh da página
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                //Mensagem de erro (no output do VS)
                System.Diagnostics.Debug.WriteLine("Erro ao inserir marca: " + ex.Message);
                // Mensagem de erro visível no formulário
                lblMensagem.Text = "Erro ao inserir marca: " + ex.Message;
                lblMensagem.CssClass = "alert alert-danger";
                lblMensagem.Visible = true;
            }
        }

        protected void btnAtualizarMarca_Click(object sender, EventArgs e)
        {
            try
            {
                // --- Ler o ID da marca a atualizar ---
                // Supondo que tens um HiddenField ou obténs do GridView selecionado
                int idMarca;
                if (!int.TryParse(hfIDMarca.Value, out idMarca))
                {
                    lblMensagem.Text = "Selecione uma marca válida para atualizar!";
                    lblMensagem.CssClass = "alert alert-warning";
                    lblMensagem.Visible = true;
                    return;
                }


                //  Debug: ver qual ID vai ser atualizado
                System.Diagnostics.Debug.WriteLine("Atualizando marca com ID: " + hfIDMarca.Value);
                System.Diagnostics.Debug.WriteLine("Nome: " + txtNome.Text);
                System.Diagnostics.Debug.WriteLine("Morada: " + txtMorada.Text);
                System.Diagnostics.Debug.WriteLine("Código Postal: " + txtCodigoPostal.Text);
                System.Diagnostics.Debug.WriteLine("Email: " + txtEmail.Text);
                System.Diagnostics.Debug.WriteLine("Telefone: " + txtTelefone.Text);
                System.Diagnostics.Debug.WriteLine("Localidade: " + txtLocalidade.Text);


                // --- Ler os valores das caixas de texto ---
                string nome = txtNome.Text.Trim();
                string morada = txtMorada.Text.Trim();
                string codigoPostal = txtCodigoPostal.Text.Trim();
                string email = txtEmail.Text.Trim();
                string telefone = txtTelefone.Text.Trim();
                string localidade = txtLocalidade.Text.Trim();


                // --- Validações (mesmo estilo do botão Inserir) ---

                //nome
                if (string.IsNullOrWhiteSpace(nome))
                {
                    lblMensagem.Text = "Insira o nome da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtNome.BorderColor = System.Drawing.Color.Red;
                    txtNome.Focus();
                    return;
                }
                //morada
                if (string.IsNullOrWhiteSpace(morada))
                {
                    lblMensagem.Text = "Insira a morada!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtMorada.BorderColor = System.Drawing.Color.Red;
                    txtMorada.Focus();
                    return;
                }

                //Código Postal
                //Validação
                if (string.IsNullOrWhiteSpace(codigoPostal))
                {
                    lblMensagem.Text = "Insira o código postal da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtCodigoPostal.BorderColor = System.Drawing.Color.Red;
                    txtCodigoPostal.Focus();
                    return;
                }

                //Verificar o cp no formato português
                if (!System.Text.RegularExpressions.Regex.IsMatch(codigoPostal, @"^\d{4}-\d{3}$"))
                {
                    lblMensagem.Text = "Código postal inválido! Use o formato 1234-567.";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtCodigoPostal.BorderColor = System.Drawing.Color.Red;
                    txtCodigoPostal.Focus();
                    return;
                }

                //Email
                //Validação
                if (string.IsNullOrWhiteSpace(email))
                {
                    lblMensagem.Text = "Insira o email da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtEmail.BorderColor = System.Drawing.Color.Red;
                    txtEmail.Focus();
                    return;
                }

                //Verificar o formato do email
                if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    lblMensagem.Text = "Email inválido! Exemplo: exemplo@gmail.com";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtEmail.BorderColor = System.Drawing.Color.Red;
                    txtEmail.Focus();
                    return;
                }


                //Telefone
                //Validação
                if (string.IsNullOrWhiteSpace(telefone))
                {
                    lblMensagem.Text = "Insira o telefone da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtTelefone.BorderColor = System.Drawing.Color.Red;
                    txtTelefone.Focus();
                    return;
                }

                // Validar formato português (fixo, móvel e opcional +351)
                if (!System.Text.RegularExpressions.Regex.IsMatch(telefone, @"^(\+351)?(2\d{8}|9\d{8})$"))
                {
                    lblMensagem.Text = "Telefone inválido! Deve ser fixo (começa com 2) ou móvel (começa com 9), opcional +351.";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtTelefone.BorderColor = System.Drawing.Color.Red;
                    txtTelefone.Focus();
                    return;
                }


                //Localidade
                if (string.IsNullOrWhiteSpace(localidade))
                {
                    lblMensagem.Text = "Insira a localidade da marca!";
                    lblMensagem.ForeColor = System.Drawing.Color.Red;
                    txtLocalidade.BorderColor = System.Drawing.Color.Red;
                    txtLocalidade.Focus();
                    return;
                }

                // --- Chamar o método AtualizarMarca() passando todos os valores ---
                AtualizarMarca(idMarca, nome, morada, codigoPostal, email, telefone, localidade);

                // --- Atualizar GridView ---
                CarregarMarcas();

                // --- Limpar caixas de texto e ID da marca ---
                txtNome.Text = "";
                txtMorada.Text = "";
                txtCodigoPostal.Text = "";
                txtEmail.Text = "";
                txtTelefone.Text = "";
                txtLocalidade.Text = "";
                hfIDMarca.Value = ""; // limpa o hidden field do ID


                Session["MensagemSucesso"] = "Marca atualizada com sucesso!";

                //Evitar duplicação no refresh da página
                Response.Redirect(Request.RawUrl);

            }
            catch (Exception ex)
            {
                //Mensagem de erro (no output do VS)
                System.Diagnostics.Debug.WriteLine("Erro ao atualizar marca: " + ex.Message);
                // Mensagem de erro visível no formulário
                lblMensagem.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void gvMarcas_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Obter a linha selecionada
                GridViewRow row = gvMarcas.SelectedRow;
                
                if (row != null)
                {
                    // Guardar o ID da marca selecionada (está na coluna 1, mas está oculta, então usamos DataKey)
                    hfIDMarca.Value = gvMarcas.SelectedDataKey.Value.ToString();

                    // Debug: verificar o valor do HiddenField no Output do Visual Studio
                    System.Diagnostics.Debug.WriteLine("ID da marca selecionada: " + hfIDMarca.Value);

                    // Preencher os campos (começando do índice 1 porque 0 é o botão Select)
                    txtNome.Text = row.Cells[2].Text;
                    txtMorada.Text = row.Cells[3].Text;
                    txtCodigoPostal.Text = row.Cells[4].Text;
                    txtEmail.Text = row.Cells[5].Text;
                    txtTelefone.Text = row.Cells[6].Text;
                    txtLocalidade.Text = row.Cells[7].Text;

                    // Mensagem
                    lblMensagem.Text = "Marca carregada para edição.";
                    lblMensagem.CssClass = "alert alert-info";
                    lblMensagem.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao selecionar marca: " + ex.Message);
                lblMensagem.Text = "Erro ao selecionar marca: " + ex.Message;
                lblMensagem.CssClass = "alert alert-danger";
                lblMensagem.Visible = true;
            }
        }


        protected void gvMarcas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Torna a linha clicável
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackEventReference(gvMarcas, "Select$" + e.Row.RowIndex);
                e.Row.Style["cursor"] = "pointer";
                e.Row.ToolTip = "Clique para selecionar esta marca";
            }
        }

        protected void btnEliminarMarca_Click(object sender, EventArgs e)
        {
            try
            {
                // Ler o ID da marca a eliminar do HiddenField
                int idMarca;
                if (!int.TryParse(hfIDMarca.Value, out idMarca))
                {
                    lblMensagem.Text = "Selecione uma marca válida para eliminar!";
                    lblMensagem.CssClass = "alert alert-warning";
                    lblMensagem.Visible = true;
                    return;
                }

                // Debug opcional: ver qual ID vai ser eliminado
                System.Diagnostics.Debug.WriteLine("Eliminando marca com ID: " + hfIDMarca.Value);

                // Chamar o método de eliminar da base de dados
                EliminarMarca(idMarca);

                // Limpar as caixas de texto e o HiddenField
                txtNome.Text = "";
                txtMorada.Text = "";
                txtCodigoPostal.Text = "";
                txtEmail.Text = "";
                txtTelefone.Text = "";
                txtLocalidade.Text = "";
                hfIDMarca.Value = "";

                // Guardar mensagem de sucesso na sessão
                Session["MensagemSucesso"] = "Marca eliminada com sucesso!";

                // Redirecionar para evitar duplicação e atualizar a página
                Response.Redirect(Request.RawUrl);

            }
            catch (Exception ex)
            {
                // Mensagem de erro
                System.Diagnostics.Debug.WriteLine("Erro ao eliminar marca: " + ex.Message);
                lblMensagem.Text = ex.Message;
                lblMensagem.CssClass = "alert alert-danger";
                lblMensagem.Visible = true;
            }
        }
    }
}