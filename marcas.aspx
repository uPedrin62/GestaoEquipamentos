<%@ Page Title="" Language="C#" MasterPageFile="~/administrador/modelo.Master"
    AutoEventWireup="true" CodeBehind="marcas.aspx.cs" Inherits="esoEquipamentos26.administrador.marcas" %>

<%@ MasterType VirtualPath="~/administrador/modelo.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <style>
        /* Footer sempre embaixo */
        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
        }
        
        body {
            display: flex;
            flex-direction: column;
        }
        
        .main-content {
            flex: 1;
        }
        
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="main-header">
            <h1 class="mb-0">Marcas</h1>
        </div>
    </div>

    <div class="container mt-4">
        <div class="section-card">
            <div class="row mb-4 align-items-center">
                <div class="col-sm-12">
                    <asp:GridView ID="gvMarcas" runat="server" CssClass="table table-striped table-hover" 
                        EmptyDataText="Não existem marcas registadas."
                        OnSelectedIndexChanged="gvMarcas_SelectedIndexChanged"
                        OnRowDataBound="gvMarcas_RowDataBound"
                        DataKeyNames="IDMarca"
                        AutoGenerateColumns="false" AutoGenerateSelectButton="true">
                        <Columns>
                            <asp:BoundField DataField="IDMarca" HeaderText="ID" Visible="false" />
                            <asp:BoundField DataField="Nome" HeaderText="Nome" />
                            <asp:BoundField DataField="Morada" HeaderText="Morada" />
                            <asp:BoundField DataField="CodigoPostal" HeaderText="Código Postal" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="Telefone" HeaderText="Telefone" />
                            <asp:BoundField DataField="Localidade" HeaderText="Localidade" />
                        </Columns>
                        <SelectedRowStyle CssClass="grid-selected fw-bold" />
                        <HeaderStyle CssClass="table-primary" />
                    </asp:GridView>
                </div>
            </div>

            <div class="row my-4 align-items-center">
                <div class="col-sm-12">
                    <asp:Button ID="btnInserirMarca" runat="server" Text="Inserir Marca" CssClass="btn btn-primary" OnClick="btnInserirMarca_Click" />
                    <asp:Button ID="btnAtualizarMarca" runat="server" Text="Atualizar Marca" CssClass="btn btn-primary ms-4" OnClick="btnAtualizarMarca_Click" CausesValidation="false" />
                    <asp:Button ID="btnEliminarMarca" runat="server" Text="Eliminar Marca" CssClass="btn btn-danger ms-4" OnClick="btnEliminarMarca_Click" OnClientClick="return confirm('Tem a certeza que deseja eliminar esta marca?');" CausesValidation="false" />
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-sm-12">
                    <asp:Label ID="lblMensagem" runat="server" Text="" CssClass="alert alert-success" Visible="false" />
                </div>
            </div>
        </div>

        <div class="section-card">
            <h5 class="section-title">📋 Informações da Marca</h5>
            
            <div class="row mb-3">
                <label class="col-sm-2 col-form-label text-end">Nome</label>
                <div class="col-sm-10">
                    <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 col-form-label text-end">Morada</label>
                <div class="col-sm-10">
                    <asp:TextBox ID="txtMorada" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 col-form-label text-end">Código Postal</label>
                <div class="col-sm-4">
                    <asp:TextBox ID="txtCodigoPostal" runat="server" CssClass="form-control" />
                </div>
                <label class="col-sm-2 col-form-label text-end">Localidade</label>
                <div class="col-sm-4">
                    <asp:TextBox ID="txtLocalidade" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 col-form-label text-end">Email</label>
                <div class="col-sm-4">
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" />
                </div>
                <label class="col-sm-2 col-form-label text-end">Telefone</label>
                <div class="col-sm-4">
                    <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" />
                </div>
            </div>

            <asp:HiddenField ID="hfIDMarca" runat="server" />
        </div>
    </div>
</asp:Content>
