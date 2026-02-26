<%@ Page Title="" Language="C#" MasterPageFile="~/administrador/modelo.Master"
    AutoEventWireup="true" CodeBehind="participacoes.aspx.cs" Inherits="esoEquipamentos26.administrador.participacoes" 
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

        .stats-card {
            border-radius: 8px;
            padding: 15px;
            text-align: center;
            color: white;
            margin-bottom: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    
    <div class="container-fluid">
        <div class="main-header">
            <h1 class="mb-0">🛠️ Participações</h1>
        </div>
    </div>

    <div class="container mt-4">
        <!-- Filtro Automático -->
        <div class="section-card">
            <div class="row mb-3 align-items-center">
                <label class="col-sm-2 col-form-label text-end">Filtrar por Local</label>
                <div class="col-sm-10">
                    <asp:DropDownList ID="ddlFiltroLocal" runat="server" CssClass="form-select w-50" AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroLocal_SelectedIndexChanged">
                        <asp:ListItem Value="">-- Todos os Locais --</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <!-- AUTO UPDATE PANEL -->
        <asp:UpdatePanel ID="upParticipacoes" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <!-- Auto refresh timer -->
                <div class="text-end mb-2">
                    <small class="text-muted">Atualização automática a cada 20 segundos</small>
                </div>

                <asp:Timer ID="timerRefresh" runat="server" Interval="20000" OnTick="timerRefresh_Tick" />

                <!-- Estatísticas -->
                <div class="row mb-4">
                    <div class="col-md-3">
                        <div class="stats-card" style="background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);">
                            <h3 class="display-6 mb-0"><asp:Label ID="lblPendentes" runat="server" Text="0" /></h3>
                            <small>Pendentes</small>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="stats-card" style="background: linear-gradient(135deg, #28a745 0%, #218838 100%);">
                            <h3 class="display-6 mb-0"><asp:Label ID="lblResolvidas" runat="server" Text="0" /></h3>
                            <small>Resolvidas</small>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="stats-card" style="background: linear-gradient(135deg, #ffc107 0%, #e0a800 100%);">
                            <h3 class="display-6 mb-0"><asp:Label ID="lblUrgentes" runat="server" Text="0" /></h3>
                            <small>Urgentes</small>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="stats-card" style="background: linear-gradient(135deg, #17a2b8 0%, #138496 100%);">
                            <h3 class="display-6 mb-0"><asp:Label ID="lblTotal" runat="server" Text="0" /></h3>
                            <small>Total</small>
                        </div>
                    </div>
                </div>

                <!-- GridView -->
                <div class="section-card">
                    <asp:GridView ID="gridParticipacoes" runat="server"
                        CssClass="table table-striped table-hover"
                        AutoGenerateColumns="false"
                        EmptyDataText="Nenhuma participação encontrada."
                        DataKeyNames="IDParticipacao"
                        AllowPaging="true" PageSize="15"
                        OnPageIndexChanging="gridParticipacoes_PageIndexChanging"
                        OnSelectedIndexChanged="gridParticipacoes_SelectedIndexChanged">
                        <Columns>
                            <asp:CommandField ShowSelectButton="true" SelectText="Selecionar" HeaderText="Selecionar" />
                            <asp:BoundField DataField="IDParticipacao" HeaderText="ID" Visible="false" />
                            <asp:BoundField DataField="Numero" HeaderText="Equipamento" />
                            <asp:BoundField DataField="LocalNome" HeaderText="Local" />
                            <asp:BoundField DataField="DescricaoProblema" HeaderText="Problema" />
                            <asp:BoundField DataField="DataParticipacao" HeaderText="Data Criação"
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <span class='<%# Eval("DataResolucao") == DBNull.Value ? "badge bg-danger" : "badge bg-success" %>'>
                                        <%# Eval("DataResolucao") == DBNull.Value ? "Pendente" : "Resolvida" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Urgente">
                                <ItemTemplate>
                                    <span class='<%# Convert.ToBoolean(Eval("Urgente")) ? "badge bg-warning text-dark" : "badge bg-secondary" %>'>
                                        <%# Convert.ToBoolean(Eval("Urgente")) ? "SIM" : "NÃO" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:BoundField DataField="NivelGravidade" HeaderText="Gravidade" />
                            <asp:BoundField DataField="NomeTecnico" HeaderText="Técnico" />
                            
                            <asp:TemplateField HeaderText="Ações">
                                <ItemTemplate>
                                    <asp:Button ID="btnResolver" runat="server" 
                                        Text="Resolver" 
                                        CssClass="btn btn-sm btn-success"
                                        CommandName="Resolver" 
                                        CommandArgument='<%# Eval("IDParticipacao") %>'
                                        OnClick="btnResolver_Click"
                                        Visible='<%# Eval("DataResolucao") == DBNull.Value %>' />
                                    <span class='<%# Eval("DataResolucao") != DBNull.Value ? "badge bg-success" : "" %>'>
                                        <%# Eval("DataResolucao") != DBNull.Value ? "✓ Resolvida" : "" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <SelectedRowStyle CssClass="grid-selected fw-bold" />
                        <HeaderStyle CssClass="table-primary" />
                    </asp:GridView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="timerRefresh" EventName="Tick" />
                <asp:PostBackTrigger ControlID="gridParticipacoes" />
            </Triggers>
        </asp:UpdatePanel>
        
        <!-- Botões de Ação -->
        <div class="section-card mt-3">
            <div class="row">
                <div class="col text-end">
                    <asp:Button ID="btnNovaParticipacao" runat="server" Text="+ Criar" 
                        CssClass="btn btn-success" OnClick="btnNovaParticipacao_Click" />
                    <asp:Button ID="btnAlterar" runat="server" Text="Alterar" 
                        CssClass="btn btn-primary ms-2" OnClick="btnAlterar_Click" />
                    <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" 
                        CssClass="btn btn-danger ms-2" OnClick="btnEliminar_Click" 
                        OnClientClick="return confirm('Tem certeza que deseja eliminar esta participação?');" />
                </div>
            </div>
        </div>

        <!-- Formulário Nova Participação (Oculto inicialmente) -->
        <asp:Panel ID="pnlNovaParticipacao" runat="server" Visible="false">
            <div class="section-card mt-3">
                <h5 class="mb-3">Criar Nova Participação</h5>
                
                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Local *</label>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="ddlLocal" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Equipamento *</label>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="ddlEquipamento" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Descrição do Problema *</label>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="500"></asp:TextBox>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Nível de Gravidade</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlGravidade" runat="server" CssClass="form-select" Enabled="false" style="background-color: #e9ecef;">
                            <asp:ListItem Value="1">1 - Baixa</asp:ListItem>
                            <asp:ListItem Value="2">2 - Média</asp:ListItem>
                            <asp:ListItem Value="3" Selected="True">3 - Alta</asp:ListItem>
                            <asp:ListItem Value="4">4 - Crítica</asp:ListItem>
                            <asp:ListItem Value="5">5 - Muito Crítica</asp:ListItem>
                        </asp:DropDownList>
                        <small class="text-muted">Definido automaticamente pelo local</small>
                    </div>
                    <div class="col-sm-2 text-end">
                        <asp:CheckBox ID="chkUrgente" runat="server" Text="Urgente" CssClass="form-check-input me-2" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 col-form-label text-end">Técnico</label>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="ddlTecnico" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Não atribuído --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col text-end">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary ms-2" OnClick="btnCancelar_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
