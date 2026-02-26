<%@ Page Title="" Language="C#" MasterPageFile="~/administrador/modelo.Master"
    AutoEventWireup="true" CodeBehind="computadoresPortatil.aspx.cs" Inherits="esoEquipamentos26.administrador.computadoresPortatil" %>

<%@ MasterType VirtualPath="~/administrador/modelo.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <style>
        .grid-selected,
        .grid-selected td {
            background-color: #C7CCDB !important;
        }
        
        .section-card {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
        }
        
        .main-header {
            background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 30px;
        }
        
        .table td, .table th {
            text-align: center;
            vertical-align: middle;
        }
        
        /* Sticky footer */
        html, body {
            height: 100%;
        }
        
        body {
            display: flex;
            flex-direction: column;
        }
        
        .container-fluid, .container {
            flex: 1;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="main-header">
            <h1 class="mb-0">Portátil</h1>
        </div>
    </div>

    <div class="container mt-4">
        <div class="section-card">
            <div class="row mb-3 align-items-center">
                <label class="col-sm-1 col-form-label text-end">Marca</label>
                <div class="col-sm-10">
                    <asp:DropDownList 
                        ID="listMarca" 
                        runat="server" 
                        AutoPostBack="true" 
                        OnSelectedIndexChanged="listMarca_SelectedIndexChanged"
                        CssClass="form-select border-secondary w-50">
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <div class="section-card">
            <div class="row mb-4 align-items-center mt-4">
                <div class="col-sm-1"></div>
                <div class="col-sm-10">
                    <asp:GridView runat="server" ID="gridComputadores" CssClass="table table-striped table-hover" 
                        EmptyDataText="Não existem computadores para a marca selecionada."
                        OnRowDataBound="gridComputadores_RowDataBound" 
                        OnSelectedIndexChanged="gridComputadores_SelectedIndexChanged"
                        DataKeyNames="IDModelo" AutoGenerateColumns="false">
                        <Columns>
                            <asp:CommandField ShowSelectButton="true" SelectText="Selecionar" />
                            <asp:BoundField DataField="IDModelo" HeaderText="ID" Visible="false" />
                            <asp:BoundField DataField="Nome" HeaderText="Nome" />
                            <asp:BoundField DataField="Marca" HeaderText="Marca" />
                        </Columns>
                        <SelectedRowStyle CssClass="grid-selected fw-bold" />
                        <HeaderStyle CssClass="table-primary" />
                    </asp:GridView>
                </div>
            </div>

            <div class="row my-4 align-items-center">
                <div class="col-sm-1"></div>
                <div class="col-sm-10">
                    <asp:Button Text="Ver" runat="server" CssClass="btn btn-primary" ID="buttonVer" OnClick="buttonVer_Click" />
                    <asp:Button Text="Criar computador" runat="server" CssClass="btn btn-primary ms-4" ID="buttonCriar" OnClick="buttonCriar_Click" />
                    <asp:Button Text="Editar" runat="server" CssClass="btn btn-primary ms-4" ID="buttonEditar" OnClick="buttonEditar_Click" />
                    <asp:Button Text="Eliminar" runat="server" CssClass="btn btn-danger ms-4" ID="buttonEliminar" OnClick="buttonEliminar_Click" OnClientClick="return confirm('Tem a certeza que deseja eliminar este computador?');" />
                </div>
            </div>
        </div>

        <div id="formularioDados" runat="server" visible="false">
            <div class="section-card">
                <h5 class="section-title">📋 Informações Básicas</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Nome</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Marca</label>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="ddlMarca" runat="server" CssClass="form-select" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Descrição</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
                    </div>
                </div>
            </div>

            <div class="section-card">
                <h5 class="section-title">🔗 URLs e Links</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">URL</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtURL" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">URL Drivers</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtURLDrivers" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Foto</label>
                    <div class="col-sm-10">
                        <asp:FileUpload runat="server" ID="uploadFoto" CssClass="form-control" />
                        <asp:Image runat="server" ID="fotoModelo" CssClass="mt-2 img-thumbnail" Style="max-width: 200px;" />
                    </div>
                </div>
            </div>

            <div class="section-card">
                <h5 class="section-title">💻 Especificações de Hardware</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Processador</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtProcessador" runat="server" CssClass="form-control" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Motherboard</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtMotherboard" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">RAM</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtRAM" runat="server" CssClass="form-control" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Armazenamento</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtArmazenamento" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Placa Gráfica</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtPlacaGrafica" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-sm-6"></div>
                </div>
            </div>

            <div class="section-card">
                <h5 class="section-title">🖥️ Ecrã</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Tamanho Ecrã</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtTamanhoEcra" runat="server" CssClass="form-control" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Resolução</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtResolucao" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Ecrã Tátil</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkEcraTatil" runat="server" CssClass="form-check-input" />
                    </div>
                    <div class="col-sm-8"></div>
                </div>
            </div>

            <div class="section-card">
                <h5 class="section-title">🔊 Áudio e Vídeo</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Colunas Incorporadas</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkColunasIncorporadas" runat="server" CssClass="form-check-input" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Número Colunas</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txtColunasNumero" runat="server" CssClass="form-control" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Auscultador</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkAuscultador" runat="server" CssClass="form-check-input" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Câmara Incorporada</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkCamaraIncorporada" runat="server" CssClass="form-check-input" />
                    </div>
                    <div class="col-sm-8"></div>
                </div>
            </div>

            <div class="section-card">
                <h5 class="section-title">🔌 Conectividade</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Portas USB</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txtPortasUSB" runat="server" CssClass="form-control" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Portas HDMI</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txtPortasHDMI" runat="server" CssClass="form-control" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Portas VGA</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txtPortasVGA" runat="server" CssClass="form-control" />
                    </div>
                </div>
            </div>

            <!-- Botões -->
            <div class="section-card">
                <div class="row mt-4">
                    <div class="col text-end">
                        <asp:Button ID="buttonGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" Style="width: 200px;" OnClick="buttonGuardar_Click" />
                        <asp:Button ID="buttonCancelar" runat="server" Text="Cancelar" CssClass="btn btn-warning ms-2" Style="width: 200px;" OnClick="buttonCancelar_Click" />
                        <asp:Button ID="buttonFechar" runat="server" Text="Fechar" CssClass="btn btn-secondary ms-2" Style="width: 200px;" OnClick="buttonFechar_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
