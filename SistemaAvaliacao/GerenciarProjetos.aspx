<%@ Page Title="Gerenciar Projetos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GerenciarProjetos.aspx.cs" Inherits="SistemaAvaliacao.GerenciarProjetos" 
    EnableEventValidation="false" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler1" />
    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Gerenciar Projetos</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active">Gerenciar Projetos</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>
    <!-- ============================================================== -->
    <!-- End Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->

    <!-- ============================================================== -->
    <!-- Container fluid  -->
    <!-- ============================================================== -->
    <div class="page-content container-fluid">
        <!-- ============================================================== -->
        <!-- Start Page Content -->
        <!-- ============================================================== -->

        <div style="overflow: hidden;">

            <%--ASSOCIADOS NO PROJETOS--%>
            <div class="card collapsed" id="cardDadosProjeto">
                <div title="HIDDEN HELPERS" hidden>
                    <asp:TextBox ID="txtDummy" runat="server" Text="" />
                    <asp:CheckBox ID="cboxDummy" runat="server" Style="display: none" Text="HiddenButton" AutoPostBack="false" />
                    <asp:TextBox ID="txtLiderDummy" runat="server" Text="" />
                    <asp:TextBox ID="txtAddLideradoDummy" runat="server" Text="" />
                    <asp:TextBox ID="txtExcluiLideradoDummy" runat="server" Text="" />
                    <asp:TextBox ID="txtExcluiAvaliadorDummy" runat="server" Text="" />
                    <asp:Button ID="btnAddLideradoDummy" runat="server" Text="" OnClick="txtDummy_TextChanged" />
                    <asp:Button ID="btnFinalizaAdicaoDummy" runat="server" Text="" OnClick="btnFinalizaAdicaoDummy_Click" />
                    <asp:Button ID="btnExcluirLideradoDummy" runat="server" Text="" OnClick="btnExcluirLideradoDummy_Click" />
                    <asp:TextBox ID="txt_Alocacao_IdAssociado" runat="server" Text=""></asp:TextBox>
                    <asp:TextBox ID="txt_Alocacao_Data" runat="server" Text=""></asp:TextBox>
                    <asp:TextBox ID="txt_Alocacao_Avaliador" runat="server" Text=""></asp:TextBox>
                    <asp:TextBox ID="txt_Alocacao_Target" runat="server" Text=""></asp:TextBox>
                    <asp:Button ID="btn_Alocacao_AlteraInicio" runat="server" Text="" OnClick="btn_Alocacao_AlteraInicio_Click" />
                    <asp:TextBox ID="txt_Alocacao_Sinalizador" runat="server" Text=""></asp:TextBox>
                    <asp:Button ID="btn_Alocacao_AlteraSinalizador" runat="server" Text="" OnClick="btn_Alocacao_AlteraSinalizador_Click" />
                    <asp:TextBox ID="txt_Fatores_IdFatorProjeto" runat="server"></asp:TextBox>
                    <asp:TextBox ID="txt_Fatores_ValorSlider" runat="server"></asp:TextBox>
                    <asp:Button ID="btn_Fatores_AtualizaFatoreProjeto" runat="server" OnClick="btn_Fatores_AtualizaFatoreProjeto_Click" />
                    <asp:TextBox ID="txt_LimpaAssociado_IdAssociado" runat="server"></asp:TextBox>
                    <asp:Button ID="btn_LimpaAssociado_Limpar" runat="server" OnClick="btn_LimpaAssociado_Limpar_Click" />
                    <asp:Button ID="btn_AlteraAssociado_Finaliza" runat="server" OnClick="btn_AlteraAssociado_Finaliza_Click" />
                    <asp:TextBox ID="txt_Drag_IdAssociadoDraged" runat="server"></asp:TextBox>
                    <asp:TextBox ID="txt_Drag_IdAssociadoTarget" runat="server"></asp:TextBox>
                    <asp:Button ID="btn_Drag_Finaliza" runat="server" OnClick="btn_Drag_Finaliza_Click" />
                    <asp:Button ID="btn_Drag_Novo" runat="server" OnClick="btn_Drag_Novo_Click" />
                    <asp:TextBox ID="txt_AtualizaValid_DivParent" runat="server" Text="" />
                    <asp:Button ID="btn_AtualizaValid_Item" runat="server" OnClick="btn_AtualizaValid_Item_Click" />
                </div>
                <div class="card-body">

                    <asp:Repeater runat="server" ID="repeaterNovoAvaliado">
                        <ItemTemplate>
                            <div class="customFlexBox" ondrop="dropNovo(event)" ondragover="allowDrop(event)">
                                <div>
                                    <div style="border-color: black; border-left: 2px solid">
                                        <div class="divContentHorizontal">
                                            <div class="divContent">
                                                <div class="customFlexBoxItem">
                                                    <label style="font-size: 20px">→ </label>
                                                    <asp:DropDownList runat="server" ID="ddlNovoAvaliado" CssClass="ddlNovoAvaliado" ClientIDMode="Static"
                                                        onchange="alteraAddLiderado(this); return false;">
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="divAcoes">
                                                    <div class="divContainerCboxes">
                                                        <div class="divCbox">
                                                            <asp:Button runat="server" ID="btnFinalizarAdicao" Text="Adicionar" CssClass="btn-dark" OnClientClick="finalizaAdicao(); return false;" />
                                                        </div>
                                                        <div class="divCbox">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Repeater runat="server" ID="parentRepeater" OnItemDataBound="mainRepeater_ItemDataBound">
                        <ItemTemplate>
                            <div class="customFlexBox" data-id="<%#Eval("IdAssociado") %>">
                                <div>
                                    <div style="<%#Eval("LeftBorder") %>">
                                        <div id="holdDiv<%#Eval("IdAssociado") %>" ondrop="drop(event)" ondragover="allowDrop(event)" data-id="<%#Eval("IdAssociado") %>">
                                            <div id="dragDiv<%#Eval("IdAssociado") %>" draggable="true" ondragstart="drag(event)" data-id="<%#Eval("IdAssociado") %>" data-projeto="<%#Eval("IdProjeto") %>">
                                                <div style="<%#Eval("LeftBorderLast") %>" class="divContentHorizontal" draggable="false">
                                                    <div class="divImage">
                                                        <img src="<%# Eval("FotoNome") %>" class="associadoFoto" draggable="false">
                                                    </div>
                                                    <div class="divContent">
                                                        <div class="customFlexBoxItem">
                                                            <label style="font-size: 20px">→ </label>
                                                            <asp:Label runat="server" Text='<%#Eval("Associado") %>' Visible='<%#Eval("ExibeNome") %>'></asp:Label>
                                                            <div <%#Eval("ExibeSelecionar") %>>
                                                                <asp:DropDownList runat="server" ID="ddlAlteraAvaliado" CssClass="ddlNovoAvaliado" ClientIDMode="Static"
                                                                    DataSource='<%#Eval("AssociadosAlterar") %>' DataTextField="Nome" DataValueField="IdAssociado"
                                                                    onchange='<%# string.Format("alteraAssociado({0}, this); return false;", Eval("IdProjeto")) %>'>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="divAcoes" <%#Eval("ExibeAcoes") %>>
                                                            <div class="divContainerCboxes">
                                                                <div class="divCbox">
                                                                    <asp:CheckBox runat="server" ID="cboxRespondeDesempenho" CssClass="customCheckBox" Checked='<%#Eval("RespondeDesempenho") %>' />
                                                                    <asp:Label runat="server"> Responde avaliação de DESEMPENHO sobre SI MESMO</asp:Label>
                                                                </div>
                                                                <div class="divCbox">
                                                                    <asp:CheckBox runat="server" ID="cboxRespondeLideranca" CssClass="customCheckBox" Checked='<%#Eval("RespondeLideranca") %>' />
                                                                    <asp:Label runat="server"> Responde avaliação de LIDERANÇA sobre o LÍDER</asp:Label>
                                                                </div>
                                                            </div>
                                                            <div class="divContainerDatas">
                                                                <div class="divCbox">
                                                                    <asp:Label runat="server">Alocação:</asp:Label>
                                                                    <asp:TextBox runat="server" Text='<%#Eval("InicioAlocacao") %>' CssClass="txtboxDataAlocacao" TextMode="Date" ID="alteraDataInicio"
                                                                        onblur=<%# string.Format("alteraData({0}, {1}, {2}, this, 'inicio'); return false;", Eval("IdAssociado"), Eval("IdProjeto"), Eval("IdAvaliador")) %>></asp:TextBox>
                                                                    <asp:Label runat="server">à</asp:Label>
                                                                    <asp:TextBox runat="server" Text='<%#Eval("TerminoAlocacao") %>' CssClass="txtboxDataAlocacao" TextMode="Date" ID="alteraDataFim"
                                                                        onblur=<%# string.Format("alteraData({0}, {1}, {2}, this, 'termino'); return false;", Eval("IdAssociado"), Eval("IdProjeto"), Eval("IdAvaliador")) %>></asp:TextBox>
                                                                </div>
                                                                <div class="divCbox">
                                                                    <asp:Label runat="server">Ciclo de avaliação:</asp:Label>
                                                                    <asp:DropDownList runat="server" ID="ddlCicloAvaliacao" CssClass="ddlCicloAvaliacao"
                                                                        onchange='<%# string.Format("alteraSinalizador({0}, {1}, {2}, this); return false;", Eval("IdAssociado"), Eval("IdProjeto"), Eval("IdAvaliador")) %>'>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                            <div class="divContainerBotoes">
                                                                <div class="botaoTooltip">
                                                                    <asp:Button runat="server" CssClass="buttonAcoes" Text="✎" Enabled='<%#Eval("HabilitaLimpar") %>'
                                                                        OnClientClick='<%# string.Format("limpaAssociado({0}, {1}); return false;", Eval("IdAssociado"), Eval("IdProjeto")) %>' />
                                                                    <span class="tooltiptext"><%#Eval("TextoLimpar") %></span>
                                                                </div>
                                                                <div class="botaoTooltip">
                                                                    <asp:Button runat="server" CssClass="buttonAcoes" Text="X" Enabled='<%#Eval("HabilitaExcluir") %>'
                                                                        OnClientClick='<%# string.Format("excluiLiderado({0}, {1}, {2}); return false;", Eval("IdAssociado"), Eval("IdAvaliador"), Eval("IdProjeto")) %>' />
                                                                    <span class="tooltiptext"><%#Eval("TextoExcluir") %></span>
                                                                </div>
                                                                <div class="botaoTooltip">
                                                                    <asp:Button runat="server" CssClass="buttonAcoes" Text="+" ID="btnAddLiderado" Enabled='<%# Eval("HabilitarAdicionarLiderado") %>'
                                                                        OnClientClick='<%# string.Format("test({0}, {1}); return false;", Eval("IdProjeto"), Eval("IdAssociado")) %>' />
                                                                    <span class="tooltiptext"><%#Eval("TextoAdicionarLiderado") %></span>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div style="margin-left: 40px">
                                            <asp:Repeater runat="server" ID="ChildRepeater" OnItemDataBound="mainRepeater_ItemDataBound" Visible="false">
                                            </asp:Repeater>
                                            <asp:Repeater runat="server" ID="NovoAvaliadoRepeater" Visible="false" OnItemDataBound="repeaterNovoAvaliado_ItemDataBound">
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>


                    <asp:UpdatePanel runat="server" ID="updPanelTeste" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="rptProjetos" />
                            <asp:AsyncPostBackTrigger ControlID="btnAddLideradoDummy" />
                            <asp:AsyncPostBackTrigger ControlID="btnFinalizaAdicaoDummy" />
                            <asp:AsyncPostBackTrigger ControlID="btnExcluirLideradoDummy" />
                            <asp:AsyncPostBackTrigger ControlID="btn_Alocacao_AlteraInicio" />
                            <asp:AsyncPostBackTrigger ControlID="btn_Alocacao_AlteraSinalizador" />
                            <asp:AsyncPostBackTrigger ControlID="btn_LimpaAssociado_Limpar" />
                            <asp:AsyncPostBackTrigger ControlID="btn_AlteraAssociado_Finaliza" />
                            <asp:AsyncPostBackTrigger ControlID="btn_Drag_Finaliza" />
                            <asp:AsyncPostBackTrigger ControlID="btn_Drag_Novo" />
                        </Triggers>
                        <ContentTemplate>
                            <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler2" />
                            <%--DADOS PROJETO--%>
                            <h4 class="card-title">Dados do Projeto</h4>
                            <div class="form-body">
                                <div class="row">
                                    <div class="col-sm-1">
                                        <div class="form-group">
                                            <label>Código</label>
                                            <asp:Label ID="txtCodigo" class="form-control" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-sm">
                                        <div class="form-group">
                                            <label>Projeto</label>
                                            <asp:Label ID="txtProjeto" class="form-control" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-sm">
                                        <div class="form-group">
                                            <label>Empresa / Cliente</label>
                                            <asp:Label ID="txtCliente" runat="server" class="form-control"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm">
                                        <div class="form-group">
                                            <label>MD</label>
                                            <asp:Label ID="ddlResponsavel" runat="server" CssClass="form-control">
                                            </asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-sm">
                                        <div class="form-group">
                                            <label>Status</label>
                                            <asp:Label ID="ddlStatus" runat="server" CssClass="form-control">
                                            </asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-sm">
                                        <div class="form-group">
                                            <label>Tipo de Projeto</label>
                                            <asp:Label ID="ddlTipoProjeto" runat="server" CssClass="form-control">
                                            </asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-sm">
                                        <div class="form-group">
                                            <label>Complexidade</label>
                                            <asp:UpdatePanel runat="server">
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btn_Fatores_AtualizaFatoreProjeto" />
                                                    <asp:AsyncPostBackTrigger ControlID="btn_AtualizaValid_Item" />
                                                </Triggers>
                                                <ContentTemplate>
                                                    <asp:Label ID="ddlComplexidade" runat="server" CssClass="form-control"></asp:Label>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-2">
                                        <div class="form-group">
                                            <label>Início</label>
                                            <asp:Label ID="txtDataInicio" runat="server" class="form-control"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-md-2">
                                        <div class="form-group">
                                            <label>Término</label>
                                            <asp:Label ID="txtDataTermino" runat="server" class="form-control"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <br />

                            <%--PILLS BOTÕES--%>
                            <div class="row">
                                <ul class="nav nav-pills mb-3" id="pills-tab" role="tablist">
                                    <li class="nav-item">
                                        <a class="nav-link active btn btn-facebook" id="tab-hierqaruia-tab" data-toggle="pill" href="#tab-hierarquia" role="tab" aria-controls="tab-hierarquia" aria-selected="true">Hierarquia</a>
                                    </li>
                                    <li class="nav-item">
                                        <a class="nav-link btn btn-danger" id="tab-complexidade-tab" data-toggle="pill" href="#tab-complexidade" role="tab" aria-controls="tab-complexidade" aria-selected="false">Complexidade</a>
                                    </li>
                                </ul>
                            </div>

                            <%--PILLS CARDS--%>
                            <div class="tab-content" id="pills-tabContent">

                                <%--HIERARQUIA AVALIAÇÕES--%>
                                <div class="tab-pane fade active in show" id="tab-hierarquia" role="tabpanel" aria-labelledby="tab-hierarquia-tab">
                                    <h3>Hierarquia de Avaliações</h3>
                                    <h4 runat="server" id="labelTituloHierarquia" class="card-title"></h4>
                                    <div class="form-body">
                                        <asp:ListView runat="server" ID="listaHierarquia">
                                            <LayoutTemplate>
                                                <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <asp:Repeater runat="server" ID="mainRepeater" DataSource='<%#Eval("Associados") %>' OnItemDataBound="mainRepeater_ItemDataBound">
                                                    <ItemTemplate>
                                                        <div style="<%#Eval("MarginLeftText") %>">
                                                            <div class="topDiv">
                                                                <asp:Label runat="server" Text='<%#Eval("Associado") %>'></asp:Label>
                                                            </div>
                                                            <asp:Repeater runat="server" ID="ChildRepeater" OnItemDataBound="mainRepeater_ItemDataBound" Visible="false">
                                                            </asp:Repeater>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
                                </div>

                                <%--COMPLEXIDADE PROJETO--%>
                                <div class="tab-pane fade" id="tab-complexidade" role="tabpanel" aria-labelledby="tab-hierarquia-tab">
                                    <h3>Fatores de complexidade do projeto</h3>
                                    <h5 style="font-style:italic">Segure e arraste os sliders azuis</h5>
                                    <br />
                                    <asp:Repeater ID="rptFatoresProjeto" runat="server">
                                        <ItemTemplate>
                                            <div style="margin-bottom: 20px; background-color: #f5f8fa">
                                                <asp:HiddenField runat="server" Value='<%# Eval("IdFatorProjeto") %>' />
                                                <asp:Label runat="server" Text='<%# Eval("Fator") %>' Font-Size="1.2em"></asp:Label>
                                                <div id="divContainerBlocoFator" style="display:flex;flex-direction:row;gap:2em">
                                                    <div class="divContainerFatores" style="flex-grow:1">
                                                        <asp:Label runat="server" Text='<%# Eval("NotaFator") %>' Visible="false"></asp:Label>
                                                        <div class="divContainerSliderNotas">
                                                            <div class="divContainerNotasDescricoes">
                                                                <asp:Label CssClass="descricaoFator" runat="server" ID="fatorDescBaixa" Text='<%# Eval("FatorDescBaixa") %>'></asp:Label>
                                                                <asp:Label CssClass="descricaoFator" runat="server" ID="fatorDescMedia" Text='<%# Eval("FatorDescMedia") %>'></asp:Label>
                                                                <asp:Label CssClass="descricaoFator" runat="server" ID="fatorDescAlta" Text='<%# Eval("FatorDescAlta") %>'></asp:Label>
                                                            </div>
                                                            <div class="slidecontainer">
                                                                <input type="range" min="1" max="3" value='<%# Eval("ValorSlider") %>' class="slider" id="rangeNotaFator"
                                                                    onchange="atualizaFatorProjeto(<%#  Eval("IdFatorProjeto") %>, this); return false;">
                                                            </div>
                                                            <div class="divContainerNotasDescricoes">
                                                                <asp:Label CssClass="descricaoFator" runat="server" ID="Label1" Text='BAIXA COMPLEXIDADE'></asp:Label>
                                                                <asp:Label CssClass="descricaoFator" runat="server" ID="Label2" Text='MÉDIA COMPLEXIDADE'></asp:Label>
                                                                <asp:Label CssClass="descricaoFator" runat="server" ID="Label3" Text='ALTA COMPLEXIDADE'></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div id="divContainerValid" runat="server" style="display:flex;flex-direction:column;align-items:center;justify-content:center;gap:1px;min-width:110px;max-width:110px">
                                                        <h5>Validação MD</h5>
                                                        <input type="checkbox" class="checkboxValid" id="cboxValid2"  <%# Eval("Checked") %>
                                                            onchange='<%# string.Format("atualizaValidItem({0}, this, \"{1}\"); return false;", Eval("IdFatorProjeto"), Eval("ValidadorPessoa")) %>'/>
                                                        <label ID="lblValidTexto" style="font-size:0.75em"><%# Eval("ValidadoTexto") %></label>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <%--LISTA PROJETOS--%>
        <br />
        <div class="card" id="cardListaProjetos">
            <div class="card-body">
                <h4 class="card-title">Projetos sob minha gestão</h4>
                <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>projetos status datas</code>. </h6>
                <div class="table-responsive">

                    <asp:Repeater ID="rptProjetos" runat="server">
                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Projeto</th>
                                        <th>Data Inicio</th>
                                        <th>Data Término</th>
                                        <th>Nº Alocados Atualmente</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DataInicio", "{0: dd/MM/yyyy}") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DataTermino", "{0: dd/MM/yyyy}") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "ContagemAlocados") %></td>
                                <td>
                                    <asp:Button Text="Gerenciar" runat="server" ID="btnGerenciarProjeto" class="btn btn-dark"
                                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdProjeto") %>'
                                        OnClick="btnGerenciarProjeto_Click"></asp:Button></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>	           
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>
            </div>
        </div>
    </div>


    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <!--<script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- apps-->
    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app.init.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
    <!--<script src="dist/js/sidebarmenu.js"></script>-->
    <!--Custom JavaScript -->
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/ckeditor/ckeditor.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
    <script>
        CKEDITOR.replace('editor1', {
            height: 150
        });

        function alertTeste() {
            alert("teste");
        }

        function expandContract() {
            const el = document.getElementById("cardDadosProjeto")
            el.classList.add('expanded')
            el.classList.remove('collapsed')

        }

        function scrollTop() {
            $("html, body").animate({ scrollTop: 0 }, "slow");
        }

        function test(IdProjeto, IdAssociado) {
            try {
                document.getElementById('<%=txtLiderDummy.ClientID%>').value = -1;
                document.getElementById('<%=txtLiderDummy.ClientID%>').value = IdAssociado;
                document.getElementById('<%=txtDummy.ClientID%>').value = -1;
                document.getElementById('<%=txtDummy.ClientID%>').value = IdProjeto;

                var clickButton = document.getElementById("<%= btnAddLideradoDummy.ClientID %>");
                clickButton.click();
            }
            catch (error) {
                alert(error);
            }
        }

        function alteraAddLiderado(dropdown) {
            document.getElementById('<%=txtAddLideradoDummy.ClientID%>').value = dropdown.value;
        }

        function finalizaAdicao() {
            try {
                var clickButton = document.getElementById("<%= btnFinalizaAdicaoDummy.ClientID %>");
                clickButton.click();
            }
            catch (error) {
                alert(error);
            }
        }

        function excluiLiderado(IdLiderado, IdAvaliador, IdProjeto) {
            document.getElementById('<%=txtExcluiLideradoDummy.ClientID%>').value = IdLiderado;
            document.getElementById('<%=txtExcluiAvaliadorDummy.ClientID%>').value = IdAvaliador;
            document.getElementById('<%=txtDummy.ClientID%>').value = IdProjeto;

            var clickButton = document.getElementById("<%= btnExcluirLideradoDummy.ClientID %>");
            clickButton.click();
        }

        function alteraData(IdAssociado, IdProjeto, IdAvaliador, dropDownData, targetData) {
            document.getElementById('<%=txt_Alocacao_IdAssociado.ClientID%>').value = IdAssociado;
            document.getElementById('<%=txtDummy.ClientID%>').value = IdProjeto;
            document.getElementById('<%=txt_Alocacao_Avaliador.ClientID%>').value = IdAvaliador;
            document.getElementById('<%=txt_Alocacao_Data.ClientID%>').value = dropDownData.value;
            document.getElementById('<%=txt_Alocacao_Target.ClientID%>').value = targetData;
            var clickButtonAlocacao = document.getElementById("<%= btn_Alocacao_AlteraInicio.ClientID %>");
            clickButtonAlocacao.click();
        }

        function alteraSinalizador(IdAssociado, IdProjeto, IdAvaliador, ddlSinalizador) {
            document.getElementById('<%=txt_Alocacao_IdAssociado.ClientID%>').value = IdAssociado;
            document.getElementById('<%=txtDummy.ClientID%>').value = IdProjeto;
            document.getElementById('<%=txt_Alocacao_Avaliador.ClientID%>').value = IdAvaliador;
            document.getElementById('<%=txt_Alocacao_Sinalizador.ClientID%>').value = ddlSinalizador.value;
            var clickButtonAlocacao = document.getElementById("<%= btn_Alocacao_AlteraSinalizador.ClientID %>");
            clickButtonAlocacao.click();
        }

        function limpaAssociado(IdAssociado, IdProjeto) {
            document.getElementById('<%=txt_LimpaAssociado_IdAssociado.ClientID%>').value = IdAssociado;
            document.getElementById('<%=txtDummy.ClientID%>').value = IdProjeto;
            var clickButtonAlocacao = document.getElementById("<%= btn_LimpaAssociado_Limpar.ClientID %>");
            clickButtonAlocacao.click();
        }

        function alteraAssociado(IdProjeto, ddl) {
            document.getElementById('<%=txtDummy.ClientID%>').value = IdProjeto;
            document.getElementById('<%=txt_LimpaAssociado_IdAssociado.ClientID%>').value = ddl.value;
            var clickButtonAlocacao = document.getElementById("<%= btn_AlteraAssociado_Finaliza.ClientID %>");
            clickButtonAlocacao.click();
        }

    </script>
    <style>
        #cardDadosProjeto {
            margin-top: -100%;
            transition: all 0.65s;
        }

            #cardDadosProjeto.expanded {
                margin-top: 0;
            }

        .topDiv {
            display: flex;
            align-items: center;
            height: max-content;
            font-size: 1.4em;
            background-color: #E4F419;
            padding-left: 1em;
        }

        .associadoFoto {
            height: 83px;
            aspect-ratio: 1/ 1;
            margin-top: 5px;
        }

        .divContent {
            flex-grow: 1;
            align-self: flex-start;
        }

        .divContentHorizontal {
            display: flex;
            flex-direction: row;
            align-items: center;
            background: linear-gradient( to bottom, #ffffff 0%, #ffffff 7%, #E8F2F7 7%, #E8F2F7 100% )
        }

        .customFlexBox {
            display: flex;
            flex-direction: column;
        }

        .customFlexBoxItem {
            display: flex;
            align-items: center;
            height: max-content;
            font-size: 1.25em;
            background: linear-gradient( to bottom, #ffffff 0%, #ffffff 15%, #D0E4EE 15%, #D0E4EE 100% );
        }

        .divAcoes {
            padding-left: 1em;
            font-size: 14px;
            display: flex;
            justify-content: flex-start;
            align-items: center;
            flex-direction: row;
            background-color: #E8F2F7;
        }

        .divContainerCboxes {
            display: flex;
            justify-content: center;
            flex-direction: column;
            flex-grow: 1;
        }

        .divContainerDatas {
            display: flex;
            align-items: flex-start;
            flex-direction: column;
            flex-grow: 1;
        }

        .divCbox {
            display: flex;
            align-items: center;
            flex-direction: row;
            gap: 5px;
        }

        .customCheckBox {
        }

            .customCheckBox input {
                width: 20px;
                height: 20px;
                accent-color: #E5F419;
                padding-top: 4px;
            }

        .divContainerBotoes {
            display: flex;
            justify-content: center;
            align-items: center;
            margin-right: 60px;
        }

        .buttonAcoes {
            width: 40px;
            height: 40px;
            background-color: #424D70;
            color: white;
            border-radius: 6px;
        }

        .botaoTooltip {
            font-size: 22px;
            margin-right: 1em;
            position: relative;
        }

            .botaoTooltip:hover .tooltiptext {
                visibility: visible;
            }

        .tooltiptext {
            visibility: hidden;
            font-size: 14px;
            background-color: black;
            color: #fff;
            text-align: center;
            padding: 5px;
            border-radius: 6px;
            position: absolute;
            max-width: 250px;
            top: 40px;
            width: max-content;
            z-index: 4;
        }

        .botaoTooltip {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
        }

        .ddlNovoAvaliado {
            height: 100%;
            line-height: 100%;
            font-size: 1.05em;
            border: none;
            margin-top: 6px;
            background: none;
            color: #021240;
            max-width: 350px;
        }

        .ddlCicloAvaliacao {
            height: 100%;
            line-height: 100%;
            font-size: 1em;
            font-style: italic;
            border: none;
            background: rgb(250, 250, 250);
            color: #021240;
            max-width: 350px;
        }

        .txtboxDataAlocacao {
            line-height: 100%;
            font-size: 1em;
            font-style: italic;
            color: #021240;
            border: none;
            background: rgb(250, 250, 250);
            max-width: 120px;
            text-align: center;
        }
    </style>
    <style>
        .slidecontainer {
            width: 100%; /* Width of the outside container */
        }

        /* The slider itself */
        .slider {
            -webkit-appearance: none; /* Override default CSS styles */
            appearance: none;
            width: 100%; /* Full-width */
            height: 25px; /* Specified height */
            background: #d3d3d3; /* Grey background */
            outline: none; /* Remove outline */
            opacity: 0.7; /* Set transparency (for mouse-over effects on hover) */
            -webkit-transition: .2s; /* 0.2 seconds transition on hover */
            transition: opacity .2s;
        }

            /* Mouse-over effects */
            .slider:hover {
                opacity: 1; /* Fully shown on mouse-over */
            }

            /* The slider handle (use -webkit- (Chrome, Opera, Safari, Edge) and -moz- (Firefox) to override default look) */
            .slider::-webkit-slider-thumb {
                -webkit-appearance: none; /* Override default look */
                appearance: none;
                width: 25px; /* Set a specific slider handle width */
                height: 25px; /* Slider handle height */
                background: #021240; /* Azul Peers background */
                cursor: pointer; /* Cursor on hover */
            }

            .slider::-moz-range-thumb {
                width: 25px; /* Set a specific slider handle width */
                height: 25px; /* Slider handle height */
                background: #04AA6D; /* Green background */
                cursor: pointer; /* Cursor on hover */
            }

        .divContainerFatores {
            display: flex;
            flex-direction: row;
            gap: 7px
        }

        .divContainerSliderNotas {
            flex-direction: column;
            gap: 1px;
            flex-grow: 1;
            display: flex;
        }

        .divContainerNotasDescricoes {
            flex-direction: row;
            display: flex;
            justify-content: space-between;
            flex-grow: 1;
        }

        .descricaoFator {
            max-width: 300px;
            text-align: center;
            display: flex;
            align-items: flex-end;
            color: grey;
            font-style: italic
        }
    </style>
    <style>
        .checkboxValid{
            width:1.7em;
            height:1.7em;
            accent-color:#003150;
        }
    </style>
    <script>
        function atualizaFatorProjeto(idFatorProjeto, sliderEle) {
            document.getElementById('<%=txt_Fatores_IdFatorProjeto.ClientID%>').value = idFatorProjeto;
            document.getElementById('<%=txt_Fatores_ValorSlider.ClientID%>').value = sliderEle.value;
            var clickButton = document.getElementById("<%= btn_Fatores_AtualizaFatoreProjeto.ClientID %>");
            clickButton.click();
        }

        function atualizaValidItem(idFatorProjeto, cbxEle, validadorPessoa) {
            document.getElementById('<%=txt_Fatores_IdFatorProjeto.ClientID%>').value = idFatorProjeto;
            document.getElementById('<%=txt_Fatores_ValorSlider.ClientID%>').value = cbxEle.checked;
            document.getElementById('<%=txt_AtualizaValid_DivParent.ClientID%>').value = cbxEle.parentElement.id;
            var children = cbxEle.parentElement.children;
            for (var i = 0; i < children.length; i++) {
                var tableChild = children[i];
                if (tableChild.id == "lblValidTexto") {
                    var currentdate = new Date();
                    var datetime = currentdate.getDate() + "/"
                        + (("0" + (currentdate.getMonth() + 1)).slice(-2)) + "/"
                        + currentdate.getFullYear() + " "
                        + currentdate.getHours() + ":"
                        + currentdate.getMinutes();
                    tableChild.innerHTML = "Atualizado pelo " + validadorPessoa + " em: " + datetime;
                }
            }
            var clickButton = document.getElementById("<%= btn_AtualizaValid_Item.ClientID %>");
            clickButton.click();
        }

        function hold() {
        }
    </script>
    <script>
        function allowDrop(ev) {
            ev.preventDefault();
        }
        function drag(ev) {
            ev.dataTransfer.setData("text", ev.target.id);
        }

        function drop(ev) {
            ev.preventDefault();
            var data = ev.dataTransfer.getData("text");
            var dragedElement = document.getElementById(data);
            var dragedId = dragedElement.dataset.id;
            var dragedProjeto = dragedElement.dataset.projeto;

            //var targetElement = $("#" + ev.target);
            var findParentDataId = "";
            var targetParent = ev.target.parentElement;
            while (findParentDataId == "")
            {
                if (targetParent.dataset.id == undefined) {
                    targetParent = targetParent.parentElement;
                }
                else {
                    findParentDataId = targetParent.dataset.id;
                }
            }
            document.getElementById('<%=txtDummy.ClientID%>').value = dragedProjeto;
            document.getElementById('<%=txt_Drag_IdAssociadoDraged.ClientID%>').value = dragedId;
            document.getElementById('<%=txt_Drag_IdAssociadoTarget.ClientID%>').value = findParentDataId;
            var clickButton = document.getElementById("<%= btn_Drag_Finaliza.ClientID %>");
            clickButton.click();
        }

        function dropNovo(ev) {
            ev.preventDefault();
            var data = ev.dataTransfer.getData("text");
            var dragedElement = document.getElementById(data);
            var dragedId = dragedElement.dataset.id;
            var dragedProjeto = dragedElement.dataset.projeto;

            //var targetElement = $("#" + ev.target);
            var findParentDataId = "";
            var targetParent = ev.target.parentElement;
            while (findParentDataId == "") {
                if (targetParent.dataset.id == undefined) {
                    targetParent = targetParent.parentElement;
                }
                else {
                    findParentDataId = targetParent.dataset.id;
                }
            }
            document.getElementById('<%=txtDummy.ClientID%>').value = dragedProjeto;
            document.getElementById('<%=txt_Drag_IdAssociadoDraged.ClientID%>').value = dragedId;
            document.getElementById('<%=txt_Drag_IdAssociadoTarget.ClientID%>').value = findParentDataId;
            var clickButton = document.getElementById("<%= btn_Drag_Novo.ClientID %>");
            clickButton.click();
        }
    </script>

</asp:Content>
