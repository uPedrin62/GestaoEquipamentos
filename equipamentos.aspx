<%@ Page Title="" Language="C#" MasterPageFile="~/administrador/modelo.Master"
    AutoEventWireup="true" CodeBehind="equipamentos.aspx.cs" Inherits="esoEquipamentos26.administrador.equipamentos" %>

<%@ MasterType VirtualPath="~/administrador/modelo.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* Estilo para as grids */
        .grid-equipamentos {
            border-collapse: collapse;
            width: 100%;
            background-color: white;
            border: 1px solid #ddd;
        }
        
        .grid-equipamentos th {
            background-color: #b8d4ea;
            color: #333;
            font-weight: 600;
            padding: 12px 15px;
            text-align: center;
            border: 1px solid #a0c4dd;
            font-size: 14px;
        }
        
        .grid-equipamentos td {
            padding: 10px 15px;
            border: 1px solid #ddd;
            text-align: center;
            background-color: white;
        }
        
        .grid-equipamentos tr:nth-child(even) td {
            background-color: #f9f9f9;
        }
        
        .grid-equipamentos tr:hover td {
            background-color: #e8f4f8;
        }
        
        .grid-equipamentos tr.selected-row td {
            background-color: #d0e8f5 !important;
            font-weight: 500;
        }
        
        .grid-equipamentos input[type="submit"] {
            background-color: transparent;
            color: #007bff;
            border: none;
            padding: 5px 10px;
            cursor: pointer;
            font-size: 13px;
            text-decoration: underline;
        }
        
        .grid-equipamentos input[type="submit"]:hover {
            color: #0056b3;
        }
        
        .grid-equipamentos .btn-selecionar {
            color: #007bff;
            text-decoration: underline;
            font-size: 13px;
            border: none;
            background: none;
            padding: 0;
            cursor: pointer;
        }
        
        .grid-equipamentos .btn-selecionar:hover {
            color: #0056b3;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">
        <div class="main-header" style="background: linear-gradient(135deg, #17a2b8 0%, #138496 100%); color: white; padding: 20px; border-radius: 8px; margin-bottom: 30px;">
            <h1 class="mb-0">Equipamentos</h1>
        </div>
    </div>

    <div class="container">

        <!-- lista de locais - filtro para preencher Grid de equipamentos -->
        <div class="row py-2 my-4">
            <div class="col-12">
                <span class="d-inline-block">Local</span>

                <asp:DropDownList runat="server" ID="listLocais" AutoPostBack="true"
                    CssClass="form-select ms-2 border-secondary d-inline-block" Width="250"
                    OnSelectedIndexChanged="listLocais_SelectedIndexChanged">
                    <asp:ListItem Text="Selecione uma sala" Value="" />
                </asp:DropDownList>
            </div>
        </div>

        <!-- Grid - equipamentos que se encontram no local -->
        <asp:GridView runat="server" ID="gridEquipamentos" DataKeyNames="IDEquipamento"
            GridLines="None" CssClass="grid-equipamentos" AutoGenerateColumns="false">
            <RowStyle Height="40" CssClass="grid-row" />
            <HeaderStyle CssClass="grid-header" />
            <Columns>
                <asp:CommandField ShowSelectButton="True" SelectText="Selecionar" ButtonType="Link" />

                <asp:BoundField DataField="IDEquipamento" HeaderText="Id" Visible="false" />
                <asp:BoundField DataField="Tipo" HeaderText="Tipo de equipamento" />
                <asp:BoundField DataField="Marca" HeaderText="Marca" />
                <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
                <asp:BoundField DataField="Numero" HeaderText="Número de série" />
                <asp:BoundField DataField="NumeroESO" HeaderText="Número ESO" />
                <asp:BoundField DataField="LocalAtual" HeaderText="Local Atual" />
                <asp:BoundField DataField="Estado" HeaderText="Estado" />
            </Columns>
            <SelectedRowStyle BackColor="#d0e8f5" Font-Bold="true" />
        </asp:GridView>

        <!-- botões- criar, editar e ver -->
        <div class="row mt-4">
            <div class="col-12">
                <asp:Button Text="Criar equipamento" runat="server" ID="buttonCriarEquipamento"
                    CssClass="btn btn-secondary d-inline-block" Width="200"
                    OnClick="buttonCriarEquipamento_Click" />

                <asp:Button Text="Editar equipamento" runat="server" ID="buttonEditarEquipamento"
                    CssClass="btn btn-secondary d-inline-block ms-3" Width="200"
                    OnClick="buttonEditarEquipamento_Click" />

                <asp:Button Text="Ver equipamento" runat="server" ID="buttonVerEquipamento"
                    CssClass="btn btn-info d-inline-block ms-3" Width="200"
                    OnClick="buttonVerEquipamento_Click" />
            </div>
        </div>

        <!-- formulário equipamento e locais -->
        <div id="formularioEquipamento" runat="server" visible="false" style="margin-top: 40px;">

            <div class="container py-3 border rounded">

                <asp:Label ID="lblMsg" runat="server" CssClass="text-danger fw-semibold d-block mb-3"></asp:Label>

                <!-- modo VER (Labels) -->
                <div id="painelVerEquipamento" runat="server" visible="false">
                    <div class="d-flex align-items-center justify-content-between mb-3">
                        <span class="fw-semibold fs-5">Detalhes do Equipamento</span>
                    </div>

                    <div class="row g-3">
                        <div class="col-6">
                            <div>
                                <span class="fw-semibold">Tipo:</span>
                                <asp:Label ID="lblVerTipo" runat="server" />
                            </div>
                            <div>
                                <span class="fw-semibold">Marca:</span>
                                <asp:Label ID="lblVerMarca" runat="server" />
                            </div>
                            <div>
                                <span class="fw-semibold">Modelo:</span>
                                <asp:Label ID="lblVerModelo" runat="server" />
                            </div>
                            <div class="mt-2">
                                <span class="fw-semibold">Local atual:</span>
                                <asp:Label ID="lblVerLocal" runat="server" />
                            </div>
                        </div>

                        <div class="col-6">
                            <div>
                                <span class="fw-semibold">Número:</span>
                                <asp:Label ID="lblVerNumero" runat="server" />
                            </div>
                            <div>
                                <span class="fw-semibold">Número ESO:</span>
                                <asp:Label ID="lblVerNumeroESO" runat="server" />
                            </div>
                            <div>
                                <span class="fw-semibold">Número CMO Odivelas:</span>
                                <asp:Label ID="lblVerNumeroCMO" runat="server" />
                            </div>
                        </div>

                        <div class="col-6">
                            <div>
                                <span class="fw-semibold">Data entrada:</span>
                                <asp:Label ID="lblVerDataEntrada" runat="server" />
                            </div>
                            <div>
                                <span class="fw-semibold">Garantia até:</span>
                                <asp:Label ID="lblVerGarantiaAte" runat="server" />
                            </div>
                            <div>
                                <span class="fw-semibold">Estado:</span>
                                <asp:Label ID="lblVerEstado" runat="server" />
                            </div>
                        </div>

                        <div class="col-6">
                            <div id="rowVerDataAbate" runat="server" visible="false">
                                <span class="fw-semibold">Data abate:</span>
                                <asp:Label ID="lblVerDataAbate" runat="server" />
                            </div>
                        </div>

                        <div class="col-12">
                            <div class="fw-semibold">Descrição</div>
                            <asp:Label ID="lblVerDescricao" runat="server" CssClass="d-block"></asp:Label>
                        </div>
                    </div>

                    <div class="mt-4">
                        <asp:Button Text="Fechar" runat="server" ID="buttonFecharVer"
                            CssClass="btn btn-warning" OnClick="buttonFechar_Click" CausesValidation="false" />
                    </div>

                    <hr class="my-4" />
                </div>

                <!-- modo CRIAR / EDITRA (inputs) -->
                <div id="painelEditarEquipamento" runat="server" visible="false">

                    <!-- tipo de equipamento -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblTipoEquipamento" runat="server" AssociatedControlID="ddlTipoEquipamento"
                            CssClass="col-2 col-form-label text-end" Text="Tipo de equipamento:" />
                        <div class="col">
                            <asp:DropDownList ID="ddlTipoEquipamento" runat="server"
                                CssClass="form-select border-secondary" Width="350"
                                OnSelectedIndexChanged="ddlTipoEquipamento_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- marca -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblMarca" runat="server" AssociatedControlID="ddlMarca"
                            CssClass="col-2 col-form-label text-end" Text="Marca:" />
                        <div class="col">
                            <asp:DropDownList ID="ddlMarca" runat="server"
                                CssClass="form-select border-secondary" Width="350"
                                OnSelectedIndexChanged="ddlMarca_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- modelo -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblModelo" runat="server" AssociatedControlID="ddlModelo"
                            CssClass="col-2 col-form-label text-end" Text="Modelo:" />
                        <div class="col">
                            <asp:DropDownList ID="ddlModelo" runat="server"
                                CssClass="form-select border-secondary w-75">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- número de série -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblNumero" runat="server" AssociatedControlID="txtNumero"
                            CssClass="col-2 col-form-label text-end" Text="Número:" />
                        <div class="col">
                            <asp:TextBox ID="txtNumero" runat="server"
                                CssClass="form-control border-secondary" MaxLength="25" Width="200" />
                        </div>
                    </div>

                    <!-- número ESO -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblNumeroESO" runat="server" AssociatedControlID="txtNumeroESO"
                            CssClass="col-2 col-form-label text-end" Text="Número ESO:" />
                        <div class="col">
                            <asp:TextBox ID="txtNumeroESO" runat="server"
                                CssClass="form-control border-secondary" MaxLength="25" Width="200" />
                        </div>
                    </div>

                    <!-- número CM Odivelas -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblNumeroCMOdivelas" runat="server" AssociatedControlID="txtNumeroCMOdivelas"
                            CssClass="col-2 col-form-label text-end" Text="Número CMO Odivelas:" />
                        <div class="col">
                            <asp:TextBox ID="txtNumeroCMOdivelas" runat="server"
                                CssClass="form-control border-secondary" MaxLength="25" Width="200" />
                        </div>
                    </div>

                    <!-- data de entrada -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblDataEntrada" runat="server" AssociatedControlID="txtDataEntrada"
                            CssClass="col-2 col-form-label text-end" Text="Data Entrada:" />
                        <div class="col">
                            <asp:TextBox ID="txtDataEntrada" runat="server" TextMode="Date"
                                CssClass="form-control border-secondary" Width="200" />
                        </div>
                    </div>

                    <!-- data de abate -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblDataAbate" runat="server" AssociatedControlID="txtDataAbate"
                            CssClass="col-2 col-form-label text-end" Text="Data Abate:" />
                        <div class="col">
                            <asp:TextBox ID="txtDataAbate" runat="server" TextMode="Date"
                                CssClass="form-control border-secondary" Width="200" />
                        </div>
                    </div>

                    <!-- garantia até -->
                    <div class="row align-items-center mb-3">
                        <asp:Label ID="lblGarantiaAte" runat="server" AssociatedControlID="txtGarantiaAte"
                            CssClass="col-2 col-form-label text-end" Text="Garantia Até:" />
                        <div class="col">
                            <asp:TextBox ID="txtGarantiaAte" runat="server" TextMode="Date"
                                CssClass="form-control border-secondary" Width="200" />
                        </div>
                    </div>

                    <!-- descrição -->
                    <div class="row align-items-start mb-3">
                        <asp:Label ID="lblDescricao" runat="server" AssociatedControlID="txtDescricao"
                            CssClass="col-2 col-form-label text-end" Text="Descrição:" />
                        <div class="col">
                            <asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="4"
                                CssClass="form-control border-secondary w-75" MaxLength="2500" />
                        </div>
                    </div>

                    <!-- estado -->
                    <div class="row align-items-center mb-4">
                        <asp:Label ID="lblEstado" runat="server" AssociatedControlID="ddlEstado"
                            CssClass="col-2 col-form-label text-end" Text="Estado:" />
                        <div class="col">
                            <asp:DropDownList ID="ddlEstado" runat="server"
                                CssClass="form-select border-secondary w-25">
                                <asp:ListItem Text="Operacional" Value="Operacional"></asp:ListItem>
                                <asp:ListItem Text="Reparação" Value="Reparação"></asp:ListItem>
                                <asp:ListItem Text="Inutilizado" Value="Inutilizado"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <hr />

                    <!-- LOCAIS -->
                    <div class="row mt-4">
                        <!-- HISTÓRICO - GRID -->
                        <div class="col-7">
                            <span class="fw-semibold">Histórico de Locais</span>

                            <asp:GridView ID="gridHistoricoLocais" runat="server"
                                GridLines="None"
                                CssClass="table table-bordered table-striped mt-2"
                                AutoGenerateColumns="False">
                                <Columns>

                                    <asp:BoundField DataField="LocalNome" HeaderText="Local" />
                                    <asp:BoundField DataField="DataEntrada" HeaderText="Data Entrada" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="DataSaida" HeaderText="Data Saída" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="TecnicoEntrada" HeaderText="Téc. Entrada" />
                                    <asp:BoundField DataField="TecnicoSaida" HeaderText="Téc. Saída" />

                                    <asp:TemplateField HeaderText="Atual">
                                        <ItemTemplate>
                                            <%# Eval("DataFimProp") == DBNull.Value ? "Sim" : "" %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>

                        </div>

                        <!-- definir local -->
                        <div class="col-5">
                            <span class="fw-semibold d-block mb-2">Definir Local Atual</span>

                            <div class="mb-3">
                                <asp:Label ID="lblLocalAtual" runat="server" AssociatedControlID="ddlLocalAtual"
                                    CssClass="form-label" Text="Local:" />
                                <asp:DropDownList ID="ddlLocalAtual" runat="server"
                                    CssClass="form-select border-secondary">
                                </asp:DropDownList>
                            </div>

                            <div class="mb-3">
                                <asp:Label ID="lblDataEntradaLocal" runat="server" AssociatedControlID="txtDataEntradaLocal"
                                    CssClass="form-label" Text="Data de entrada nesse local:" />
                                <asp:TextBox ID="txtDataEntradaLocal" runat="server" TextMode="Date"
                                    CssClass="form-control border-secondary" />
                            </div>

                            <div class="text-muted">
                                (Ao guardar, se o local mudar, fecha o local anterior e cria uma nova linha em LocalEquipamento.)
                       
                            </div>
                        </div>
                    </div>

                    <!-- botões GUARDAR e CANCELAR (criação e edição de equipamentos + locais) -->
                    <div class="row mt-5">
                        <div class="col-2"></div>
                        <div class="col-8">
                            <asp:Button Text="Guardar" runat="server" CssClass="btn btn-primary d-inline-block"
                                ID="buttonGuardarEquipamento" OnClick="buttonGuardarEquipamento_Click" />
                            <asp:Button Text="Cancelar" runat="server" CssClass="btn btn-warning d-inline-block ms-4"
                                ID="buttonCancelar" OnClick="buttonFechar_Click" CausesValidation="false" />
                        </div>
                    </div>

                </div>

            </div>
        </div>

    </div>

</asp:Content>