<%@ Page Title="" Language="C#" MasterPageFile="~/administrador/modelo.Master"
    AutoEventWireup="true" CodeBehind="impressoras.aspx.cs" Inherits="esoEquipamentos26.administrador.impressoras" 
    ResponseEncoding="utf-8" %>

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
            background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 30px;
        }
        
        .table td, .table th {
            text-align: center;
            vertical-align: middle;
        }
        
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
            <h1 class="mb-0">🖨️ Impressoras</h1>
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
                    <asp:GridView runat="server" ID="gvImpressoras" CssClass="table table-striped table-hover" 
                        EmptyDataText="Não existem impressoras para a marca selecionada."
                        OnSelectedIndexChanged="gvImpressoras_SelectedIndexChanged"
                        DataKeyNames="ID" AutoGenerateColumns="false">
                        <Columns>
                            <asp:CommandField ShowSelectButton="true" SelectText="Selecionar" />
                            <asp:BoundField DataField="ID" HeaderText="ID" Visible="false" />
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
                    <asp:Button Text="Criar impressora" runat="server" CssClass="btn btn-primary ms-4" ID="buttonCriar" OnClick="buttonCriar_Click" />
                    <asp:Button Text="Editar" runat="server" CssClass="btn btn-primary ms-4" ID="buttonEditar" OnClick="buttonEditar_Click" />
                    <asp:Button Text="Eliminar" runat="server" CssClass="btn btn-danger ms-4" ID="buttonEliminar" OnClick="buttonEliminar_Click" OnClientClick="return confirm('Tem a certeza que deseja eliminar esta impressora?');" />
                </div>
            </div>
        </div>

        <div id="formularioDados" runat="server" Visible="false">
            <div class="section-card">
                <h5 class="section-title">Informa&ccedil;&otilde;es B&aacute;sicas</h5>
                
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
                    <label class="col-sm-2 col-form-label text-end">Descri&ccedil;&atilde;o</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
                    </div>
                </div>
            </div>

            <div class="section-card">
                <h5 class="section-title">URLs e Links</h5>
                
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
                <h5 class="section-title">Especifica&ccedil;&otilde;es T&eacute;cnicas</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Tipo</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtTipo" runat="server" CssClass="form-control" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Resolu&ccedil;&atilde;o M&aacute;xima</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtResolucaoMaxima" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">C&oacute;digo Tinteiros</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtCodigoTinteiros" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Impress&atilde;o Duplex</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkImpressaoDuplex" runat="server" CssClass="form-check-input" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Impress&atilde;o a Cores</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkImpressaoCor" runat="server" CssClass="form-check-input" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Scanner</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkScanner" runat="server" CssClass="form-check-input" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Visor</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkVisor" runat="server" CssClass="form-check-input" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Wi-Fi</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkWifi" runat="server" CssClass="form-check-input" />
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Bluetooth</label>
                    <div class="col-sm-2 d-flex align-items-center">
                        <asp:CheckBox ID="chkBluetooth" runat="server" CssClass="form-check-input" />
                    </div>
                </div>
            </div>

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
