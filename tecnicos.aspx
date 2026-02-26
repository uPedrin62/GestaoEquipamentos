<%@ Page Title="" Language="C#" MasterPageFile="~/administrador/modelo.Master" 
    AutoEventWireup="true" CodeBehind="tecnicos.aspx.cs" Inherits="esoEquipamentos26.administrador.tecnicos" 
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
            background: linear-gradient(135deg, #17a2b8 0%, #138496 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 30px;
        }
        
        .table td, .table th {
            text-align: center;
            vertical-align: middle;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="main-header">
            <h1 class="mb-0">T&eacute;cnicos</h1>
        </div>
    </div>

    <div class="container mt-4">
        <div class="section-card">
            <div class="row mb-4 align-items-center mt-4">
                <div class="col-sm-12">
                    <asp:GridView runat="server" ID="gridTecnicos" CssClass="table table-striped table-hover" 
                        EmptyDataText="N&atilde;o existem t&eacute;cnicos cadastrados."
                        OnSelectedIndexChanged="gridTecnicos_SelectedIndexChanged"
                        DataKeyNames="IDTecnico" AutoGenerateColumns="false">
                        <Columns>
                            <asp:CommandField ShowSelectButton="true" SelectText="Selecionar" />
                            <asp:BoundField DataField="IDTecnico" HeaderText="ID" Visible="false" />
                            <asp:BoundField DataField="Nome" HeaderText="Nome" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="Telefone" HeaderText="Telefone" />
                        </Columns>
                        <SelectedRowStyle CssClass="grid-selected fw-bold" />
                        <HeaderStyle CssClass="table-primary" />
                    </asp:GridView>
                </div>
            </div>

            <div class="row my-4 align-items-center">
                <div class="col-sm-12">
                    <asp:Button Text="Ver" runat="server" CssClass="btn btn-primary" ID="buttonVer" OnClick="buttonVer_Click" />
                    <asp:Button Text="Criar t&eacute;cnico" runat="server" CssClass="btn btn-primary ms-4" ID="buttonCriar" OnClick="buttonCriar_Click" />
                    <asp:Button Text="Editar" runat="server" CssClass="btn btn-primary ms-4" ID="buttonEditar" OnClick="buttonEditar_Click" />
                    <asp:Button Text="Eliminar" runat="server" CssClass="btn btn-danger ms-4" ID="buttonEliminar" OnClick="buttonEliminar_Click" 
                        OnClientClick="return confirm('Tem a certeza que deseja eliminar este t&eacute;cnico?');" />
                </div>
            </div>
        </div>

        <div id="formularioDados" runat="server" visible="false">
            <div class="section-card">
                <h5 class="section-title">Dados do T&eacute;cnico</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Nome</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="200" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Email</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="200" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Telefone</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" MaxLength="9" />
                        <small class="text-muted">9 d&iacute;gitos</small>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Senha</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtSenha" runat="server" CssClass="form-control" TextMode="Password" MaxLength="100" />
                        <small class="text-muted">M&iacute;nimo 6 caracteres</small>
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
