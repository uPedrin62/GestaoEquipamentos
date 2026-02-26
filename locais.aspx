<%@ Page Title="" Language="C#" MasterPageFile="~/administrador/modelo.Master"
    AutoEventWireup="true" CodeBehind="locais.aspx.cs" Inherits="esoEquipamentos26.administrador.locais" %>

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
        
        .section-title {
            color: #495057;
            font-weight: 600;
            margin-bottom: 15px;
            padding-bottom: 8px;
            border-bottom: 2px solid #007bff;
            display: inline-block;
        }
        
        .badge-prioridade-1 { background-color: #28a745; }
        .badge-prioridade-2 { background-color: #17a2b8; }
        .badge-prioridade-3 { background-color: #ffc107; color: #212529; }
        .badge-prioridade-4 { background-color: #dc3545; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="main-header">
            <h1 class="mb-0">Locais</h1>
        </div>
    </div>

    <div class="container mt-4">
        <div class="section-card">
            <div class="row mb-3 align-items-center">
                <label class="col-sm-1 col-form-label text-end">Categoria</label>
                <div class="col-sm-4">
                    <asp:DropDownList 
                        ID="listCategoria" 
                        runat="server" 
                        AutoPostBack="true" 
                        OnSelectedIndexChanged="listCategoria_SelectedIndexChanged"
                        CssClass="form-select border-secondary w-100">
                        <asp:ListItem Text="-- todas as categorias --" Value="" />
                        <asp:ListItem Text="Salas de Aula" Value="Salas" />
                        <asp:ListItem Text="Laboratórios" Value="Laboratorios" />
                        <asp:ListItem Text="Serviços Administrativos" Value="Administrativos" />
                        <asp:ListItem Text="Espaços Comuns" Value="Comuns" />
                        <asp:ListItem Text="Espaços Especiais" Value="Especiais" />
                    </asp:DropDownList>
                </div>
                <label class="col-sm-1 col-form-label text-end">Prioridade</label>
                <div class="col-sm-4">
                    <asp:DropDownList 
                        ID="listPrioridade" 
                        runat="server" 
                        AutoPostBack="true" 
                        OnSelectedIndexChanged="listPrioridade_SelectedIndexChanged"
                        CssClass="form-select border-secondary w-100">
                        <asp:ListItem Text="-- todas as prioridades --" Value="" />
                        <asp:ListItem Text="Baixa" Value="1" />
                        <asp:ListItem Text="Normal" Value="2" />
                        <asp:ListItem Text="Alta" Value="3" />
                        <asp:ListItem Text="Crítica" Value="4" />
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <div class="section-card">
            <div class="row mb-4 align-items-center mt-4">
                <div class="col-sm-1"></div>
                <div class="col-sm-10">
                    <asp:GridView runat="server" ID="gridLocais" CssClass="table table-striped table-hover" 
                        EmptyDataText="Não existem locais para os filtros selecionados."
                        OnRowDataBound="gridLocais_RowDataBound" 
                        OnSelectedIndexChanged="gridLocais_SelectedIndexChanged"
                        DataKeyNames="IDLocal" AutoGenerateColumns="false">
                        <Columns>
                            <asp:CommandField ShowSelectButton="true" SelectText="Selecionar" />
                            <asp:BoundField DataField="IDLocal" HeaderText="ID" Visible="false" />
                            <asp:BoundField DataField="Nome" HeaderText="Nome" />
                            <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                            <asp:TemplateField HeaderText="Prioridade">
                                <ItemTemplate>
                                    <asp:Label ID="lblPrioridade" runat="server" CssClass="badge" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Categoria" HeaderText="Categoria" />
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
                    <asp:Button Text="Criar Local" runat="server" CssClass="btn btn-primary ms-4" ID="buttonCriar" OnClick="buttonCriar_Click" />
                    <asp:Button Text="Editar" runat="server" CssClass="btn btn-primary ms-4" ID="buttonEditar" OnClick="buttonEditar_Click" />
                    <asp:Button Text="Eliminar" runat="server" CssClass="btn btn-danger ms-4" ID="buttonEliminar" OnClick="buttonEliminar_Click" OnClientClick="return confirm('Tem a certeza que deseja eliminar este local?');" />
                </div>
            </div>
        </div>

        <div id="formularioDados" runat="server" visible="false">
            <div class="section-card">
                <h5 class="section-title">📍 Informações do Local</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Nome</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="300" />
                        <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome"
                            ErrorMessage="O nome é obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Local" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Categoria</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                            <asp:ListItem Text="-- Selecione a categoria --" Value="" />
                            <asp:ListItem Text="Salas de Aula" Value="Salas" />
                            <asp:ListItem Text="Laboratórios" Value="Laboratorios" />
                            <asp:ListItem Text="Serviços Administrativos" Value="Administrativos" />
                            <asp:ListItem Text="Espaços Comuns" Value="Comuns" />
                            <asp:ListItem Text="Espaços Especiais" Value="Especiais" />
                        </asp:DropDownList>
                    </div>
                    <label class="col-sm-2 col-form-label text-end">Prioridade</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select">
                            <asp:ListItem Value="1" Text="Baixa" />
                            <asp:ListItem Value="2" Text="Normal" Selected="True" />
                            <asp:ListItem Value="3" Text="Alta" />
                            <asp:ListItem Value="4" Text="Crítica" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Descrição</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="3" 
                            CssClass="form-control" MaxLength="500" />
                    </div>
                </div>
            </div>

            <!-- Botões -->
            <div class="section-card">
                <div class="row mt-4">
                    <div class="col text-end">
                        <asp:Button ID="buttonGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" Style="width: 200px;" OnClick="buttonGuardar_Click" ValidationGroup="Local" />
                        <asp:Button ID="buttonCancelar" runat="server" Text="Cancelar" CssClass="btn btn-warning ms-2" Style="width: 200px;" OnClick="buttonCancelar_Click" CausesValidation="false" />
                        <asp:Button ID="buttonFechar" runat="server" Text="Fechar" CssClass="btn btn-secondary ms-2" Style="width: 200px;" OnClick="buttonFechar_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>