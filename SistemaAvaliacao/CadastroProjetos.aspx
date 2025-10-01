<%@ Page Async="true" Title="Projeto" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CadastroProjetos.aspx.cs" Inherits="SistemaAvaliacao.CadastroProjetos" ValidateRequest="false" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <link rel="stylesheet" href="dist/css/sliderAvaliacao.css?" + DateTime.Now.ToString("ddMMHHmmss")">

    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->

    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Cargos</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro Básico de Usuários</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Cargos</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>

    <!-- ============================================================== -->
    <!-- End Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->


    <div class="page-content container-fluid">
        <!-- ============================================================== -->
        <!-- Start Page Content -->
        <!-- ============================================================== -->
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Dados do Projeto</h4>
                <div class="form-body">

                    <div class="row">
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Código</label>
                                <asp:TextBox ID="txtCodigo" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Projeto</label>
                                <asp:TextBox ID="txtProjeto" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Empresa / Cliente</label>
                                <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-control p-0 select2" DataValueField="0" DataTextField="Selecione" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Status</label>
                                <asp:DropDownList ID="ddlStatusProjeto" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>

                    </div>
                    <div class="row">

                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Início</label>
                                <asp:TextBox ID="txtDataInicio" TextMode="Date" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Término</label>
                                <asp:TextBox ID="txtDataTermino" TextMode="Date" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Responsável <a href="javascript:void(0)" data-toggle="modal" data-target="#responsavel-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                            <asp:DropDownList ID="ddlResponsavel" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                            </asp:DropDownList>
                        </div>



                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Gestor <a href="javascript:void(0)" data-toggle="modal" data-target="#gestor-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                <asp:DropDownList ID="ddlGestor" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Tipo de Projeto<a href="javascript:void(0)" data-toggle="modal" data-target="#tipo-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a> </label>
                                <asp:DropDownList ID="ddlTipoProjeto" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Complexidade <a href="javascript:void(0)" data-toggle="modal" data-target="#complexiade-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                <asp:DropDownList ID="ddlComplexidade" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Status</label>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="0">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                </div>
            </div>
        </div>
        
        <!-- HIERARQUIA DE AVALIAÇÕES -->
        <div class="card" hidden>
            <div class="card-body">
                <h4 class="card-title">Hierarquia de Avaliações</h4>
                <div class="row" style="background-color:#BFBFBF;width:100%;list-style-type: none;padding-left: 0px;float:left;border:none;height:240px" draggable="false">
                    <div class="row" style="height:35px;margin-left:10px;margin-top:5px;width:100%" draggable="false">
                        <div class="col-md-2" draggable="false">
                            <asp:DropDownList ID="ddlEsteiraSlotsAssociado" runat="server" CssClass="form-control p-0 select2" Style="width: 250px"></asp:DropDownList>
                        </div>
                        <div class="col-md-3" draggable="false">
                            <asp:Button OnClick="btnAddSlot_Click" ID="btnAddSlot" class="btn btn-danger" runat="server" Text="Adicionar Associado" AutoPostBack="true" />
                            <asp:Button OnClick="btnAtualizaEquipe_Click" ID="btnAtualizaEquipe" class="btn btn-danger" runat="server" Text="" AutoPostBack="true" Visible="false" />
                        </div>
                    </div>
                    <div id="divHoldUpdatePanel" style="float:left;list-style-type: none;padding-left: 0px">
                        <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnAddSlot" />
                            </Triggers>
                            <ContentTemplate>
                                <div class="row" style="margin-left:2px;float:left;list-style-type: none;padding-left: 0px" draggable="false">
                                    <asp:Repeater ID="rptEsteiraSlots" runat="server">
                                        <ItemTemplate>
                                            <div class="col-sm" draggable="false">
                                                <div id="drag_Add_<%# Eval("i")%>" draggable="true" ondragstart="drag(event)" style="margin: 0 auto;width:100px;height:180px;align-content:center;background-color:#002060;text-align:center;border-radius:4px">
                                                    <label class="switchRH">
                                                        <input type="checkbox" id="inputRespLid_Add_<%# Eval("i")%>" onclick="ReverterValor('inputRespLid_Add_<%# Eval("i")%>')" value="1" checked/>
                                                        <span class="sloder rounde" style="width: 60px; height: 34px;"></span>
                                                    </label>
                                                    <br/>
                                                    <div id="idAssociado_Add_<%# Eval("i")%>" hidden> <%# Eval("idAssociado")%> </div>
                                                    <div id="idAssociadoHierarquia_Add_<%# Eval("i")%>" hidden> <%# Eval("idAssociadoHierarquia")%> </div>
                                                    <div style="color:#FFFFFF;font-size:14px"> <%# Eval("nome")%> </div>
                                                    <button hidden id="btnNovoSlot_Add_<%# Eval("i")%>" style="color:white;font-size:90px;background:#002060; border:none;height:150px;width:80px" 
                                                        onclick="NovoSlot('<%# Eval("idAssociadoHierarquia")%>');" type="button">+</button>
                                                    <img draggable="false" width="80px" height="80px" style="border-radius: 50%" src="<%# Eval("FotoNome")%>" />
                                                    <br/>
                                                    <label class="switchRH">
                                                        <input type="checkbox" id="inputRespDes_Add_<%# Eval("i")%>" onclick="ReverterValor('inputRespDes_Add_<%# Eval("i")%>')" value="1" checked/>
                                                        <span class="slider round" style="width: 60px; height: 34px;"></span>
                                                    </label>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div>
                <asp:UpdatePanel ID="UpdatePanel2" UpdateMode="Conditional" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnAtualizaEquipe" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="chart"></div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:HiddenField ID="hiddenHtmlCode" runat="server" />
            </div>
            <div class="row" style="list-style-type: none;padding-left: 0px;float:left">
                <asp:Button OnClick="btnGerarAvaliacoes_Click" OnClientClick="JavaScriptFunction()" ID="btnGerarAvaliacoes" class="btn btn-info" runat="server" Text="Gerar Avaliações"
                    style="margin-left:30px;margin-bottom:10px"/>
            </div>
            <%--<button style="color:white;font-size:20px" runat="server" onclick="testCall();">+</button>--%>
            <%--<asp:Button Text="+" ID="btnNovoSlot" CommandArgument="1" OnCommand="btnNovoSlot_Command" runat="server" />--%>
        </div>

        <!-- ALOCAÇÕES DESEMPENHO -->
        <div class="card">
            <div class="card-body">
                <div class="row">
                    <h4 class="card-title">Avaliações de Desempenho<i class="fa fa-plus" title="Adicionar"></i></h4>
                </div>
                <div class="row">
                    <div class="col-md-3">
                        <div class="form-group">
                            <label>Associado alocado: <a href="javascript:void(0)" data-toggle="modal" data-target="#gestor-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                            <asp:DropDownList ID="ddlAssociadoAlocado" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdlIdProjetoAssociado" runat="server" />
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            <label>Início da Alocação:</label>
                            <asp:TextBox ID="txtInicioAlocacao" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            <label>Término da Alocação</label>
                            <asp:TextBox ID="txtTerminoAlocacao" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label>Avaliador: <a href="javascript:void(0)" data-toggle="modal" data-target="#gestor-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                            <asp:DropDownList ID="ddlAvaliadorAlocacao" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3">
                        <div class="form-group">
                            <label>Comentários</label>
                            <asp:TextBox ID="txtComentariosAlocacao" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            <br />
                            <asp:Button OnClick="btnCadastrarAlocacao_Click" ID="Button1" class="btn btn-info" runat="server" Text="Salvar Alocação" />

                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md">
                    <h4 class="card-title">Lista de associados alocados no projeto</h4>
                    <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>Nome Cargo Avaliador</code>. </h6>
                    <div class="table-responsive">
                        <asp:Repeater ID="rptAlocacoes" OnItemCommand="rptAlocacoes_ItemCommand" OnItemDataBound="rptAlocacoes_ItemDataBound" runat="server">
                            <HeaderTemplate>
                                <table class="table table-striped border dtInit">
                                    <thead>
                                        <tr>
                                            <th>Nome</th>
                                            <th>Cargo</th>
                                            <th>Início da alocação</th>
                                            <th>Término da alocação</th>
                                            <th>Avaliador</th>
                                            <th>Comentários</th>
                                            <th>Ciclo de Avaliação</th>
                                            <th>Status(Ativo/Inativo)</th>
                                            <th data-sort="0">Ação</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblIdAssociado" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "IdAssociado") %>'></asp:Label>
                                        <asp:Label ID="lblIdProjetoAssociado" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "IdProjetoAssociado") %>'></asp:Label>
                                        <%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS.Nome") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS.CARGOS.Cargo") %></td>
                                    <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DataInicio")).ToString("dd/MM/yyyy") %></td>
                                    <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DataFim")).ToString("dd/MM/yyyy") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS2.Nome") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Comentario") %></td>
                                    <td>
                                        <asp:Label ID="lblCiclo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IdPeriodoSinalizado") %>'></asp:Label></td>
                                    <td>
                                    <td>
                                        <asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                    <td>
                                        <asp:Button ID="btnEditar" class="btn btn-dark" runat="server" Text="Editar" CommandName="EditarAssociado" />
                                        <asp:Button ID="btnDeletar" class="btn btn-info" runat="server" Text="Inativar" CommandName="ExcluirAssociado" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                <br />
                <br />
            </div>
            </div>
                <div class="row" style="display:flex; justify-content:center;background-color:#E8F2F7">
                    <div style="display:flex; flex-direction:row;align-items: center;width:90%;justify-content:space-between">
                        <div style="font-size:2em">
                            <asp:Label runat="server" ID="lblValidaAlocacaoPendente" Text="Teste"></asp:Label>
                        </div>
                        <div>
                            <asp:Button runat="server" ID="btnNotificarValidaAlocacao" Text="Notificar" CssClass="btn btn-facebook" OnClick="btnNotificarValidaAlocacao_Click"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- ALOCAÇÕES LIDERANCA -->
        <div class="card">
            <div class="card-body">
                <div class="form-body">
                    <div class="col-md-12">
                        <h4 class="card-title">Avaliações de Liderança </h4>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Líder:</label>
                                <asp:DropDownList ID="ddl_AvalLideranca_Lider" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdl_AvalLideranca_Lider" runat="server" />
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Início da Alocação:</label>
                                <asp:TextBox ID="text_AvalLideranca_Inicio" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Término da Alocação</label>
                                <asp:TextBox ID="text_AvalLideranca_Termino" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Liderado Avaliador:</label>
                                <asp:DropDownList ID="ddl_AvalLideranca_Liderado" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Escopo:</label>
                                <asp:DropDownList ID="ddl_Escopo" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Comentários</label>
                                <asp:TextBox ID="text_AvalLideranca_Comentarios" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <br />
                                <asp:Button OnClick="btn_CadastrarAvaliacaoLideranca_Click" ID="btn_CadastrarAvaliacaoLideranca" class="btn btn-info" runat="server" Text="Salvar Avaliação" />

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row mt">
                <div class="col-md">
                    <div class="card-body">
                        <h4 class="card-title">Lista de avaliações de liderança</h4>
                        <div class="table-responsive">
                            <asp:Repeater ID="rpt_AvalLideranca_Avaliacoes" OnItemCommand="rptAlocacoes_ItemCommand" OnItemDataBound="rptAlocacoes_ItemDataBound" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped border dtInit">
                                        <thead>
                                            <tr>
                                                <th>Líder</th>
                                                <th>Cargo</th>
                                                <th>Início da alocação</th>
                                                <th>Término da alocação</th>
                                                <th>Liderado</th>
                                                <th>Comentários</th>
                                                <th>Status(Ativo/Inativo)</th>
                                                <th data-sort="0">Ação</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblIdAssociado" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "IdAssociado") %>'></asp:Label>
                                            <asp:Label ID="lblIdProjetoAssociado" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "IdProjetoAssociado") %>'></asp:Label>
                                            <%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS.Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS.CARGOS.Cargo") %></td>
                                        <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DataInicio")).ToString("dd/MM/yyyy") %></td>
                                        <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DataFim")).ToString("dd/MM/yyyy") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS2.Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Comentario") %></td>
                                        <td>
                                            <asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                        <td>
                                            <asp:Button ID="btnEditar" class="btn btn-dark" runat="server" Text="Editar" CommandName="EditarAssociado" />
                                            <asp:Button ID="btnDeletar" class="btn btn-info" runat="server" Text="Inativar" CommandName="ExcluirAssociado" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
									    <tfoot>
                                            <tr>
                                                <th>Nome</th>
                                                <th>Cargo</th>
                                                <th>Início da alocação</th>
                                                <th>Término da alocação</th>
                                                <th>Avaliador</th>
                                                <th>Comentários</th>
                                                <th>Status(Ativo/Inativo)</th>
                                                <th data-sort="0">Ação</th>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                    <div class="form-actions">
                        <div class="text-right">
                            <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="EFETIVAR CADASTRO " />
                        </div>
                    </div>
                    <br />
                    <br />
                </div>
            </div>
        </div>
        
        <%--EXPORT E IMPORT--%>
        <div class="card">
            <div class="card-body">
                <div class="row">
                    <h4 class="card-title">Exportar e Importar Projetos e Alocações</h4>
                </div>
                <div class="row">
                    <h6 class="card-subtitle">Utilize os modelos de arquivos exportados para alterar ou adicionar projetos e associações.
                    <br />Linhas SEM um valor na coluna IdProjeto e/ou IdAssociacao serão consideradas inserções.
                    <br />Alocações de DESEMPENHO que NÃO tiverem um valor na coluna IdAvaliador serão enviadas para validação do gestor na página de gerenciar projetos.
                    <br />As correlações de cargo, eixo, subcompetência e dimensão devem ser identificadas por seus CÓDIGOS.
                    <br />NÃO altere a ordem e quantidade das colunas, a quantidade de abas dos arquivos ou coloque linhas acima da linha de títulos.</h6>
                </div>
                <br />
                <div class="row"> 
                    <asp:Button runat="server" type="button" ID="btnExportProjetos" class="btn btn-info" OnClick="btnExportProjetos_Click" Text="Exportar Projetos"></asp:Button>
                    <asp:Button runat="server" type="button" ID="btnImportProjetos" class="btn btn-info" OnClick="btnImportProjetos_Click" Text="Importar Projetos"></asp:Button>
                    <asp:FileUpload ID="fileUploadProjetos" runat="server" AllowMultiple="false" accept=".xlsx" class="btn btn-danger" /> 
                </div>
                <br />
                <div class="row">
                    <asp:Button runat="server" type="button" ID="btnExportAssociacoes" class="btn btn-info" OnClick="btnExportAssociacoes_Click" Text="Exportar Alocações"></asp:Button>
                    <asp:Button runat="server" type="button" ID="btnImportAssociacoes" class="btn btn-info" OnClick="btnImportAssociacoes_Click" Text="Importar Alocações"></asp:Button>
                    <asp:FileUpload ID="fileUploadAssociacoes" runat="server" AllowMultiple="false" accept=".xlsx" class="btn btn-danger" /> 
                </div>
                <br />
                <div class="row">
                    <asp:Button runat="server" id="btnExportFatores" CssClass="btn btn-info" OnClick="btnExportFatores_Click" Text="Exportar Fatores de Complexidade" />
                </div>
            </div>
        </div>

        <!-- LISTA PROJETOS -->
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Lista de projetos</h4>
                <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>projtos status datas</code>. </h6>
                <div class="table-responsive">
                    <asp:HiddenField ID="hdIdProjeto" runat="server" />
                    <asp:Repeater ID="rptProjetos" runat="server" OnItemCommand="rptProjetos_ItemCommand" OnItemDataBound="rptProjetos_ItemDataBound">
                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Código</th>
                                        <th>Projeto</th>
                                        <th>Cliente</th>
                                        <th>Data Inicio</th>
                                        <th>Data Término</th>
                                        <th>Ativo</th>
                                        <th data-sort="0">Ação</th>
                                    </tr>
                                </thead>

                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Label ID="lblIdProjeto" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IdProjeto") %>'></asp:Label></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Codigo") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Projeto") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "CLIENTES.Cliente") %></td>
                                <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DataInicio")).ToString("dd/MM/yyyy") %></td>
                                <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DataFim")).ToString("dd/MM/yyyy") %></td>
                                <td>
                                    <asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                <td>
                                    <asp:Button ID="btnEditar" class="btn btn-dark" runat="server" Text="Editar" CommandName="EditarProjeto" />
                                    <asp:Button ID="btnDeletar" class="btn btn-info" runat="server" Text="Inativar" CommandName="ExcluirProjeto" />
                                </td>
                            </tr>
                        </ItemTemplate>

                        <FooterTemplate>
                            </tbody>
											<tfoot>
                                                <tr>
                                                    <th>ID</th>
                                                    <th>Código</th>
                                                    <th>Projeto</th>
                                                    <th>Cliente</th>
                                                    <th>Data Inicio</th>
                                                    <th>Data Término</th>
                                                    <th>Ativo</th>
                                                    <th data-sort="0">Ação</th>
                                                </tr>
                                            </tfoot>
                            </table>

                        </FooterTemplate>
                    </asp:Repeater>

                </div>
            </div>

        </div>
    </div>

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
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
            ev.target.innerHTML = "";
            ev.target.appendChild(document.getElementById(data));
        }
    </script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
        google.load("visualization", "1", { packages: ["orgchart"] });
        google.setOnLoadCallback(drawChart);
        function drawChart() {
            $.ajax({
                type: "POST",
                url: "CadastroProjetos.aspx/GetChartData",
                data: '{}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    var data = new google.visualization.DataTable();
                    data.addColumn('string', 'Entity');
                    data.addColumn('string', 'ParentEntity');
                    data.addColumn('string', 'ToolTip');
                    for (var i = 0; i < r.d.length; i++) {
                        var employeeId = r.d[i][0].toString();
                        var employeeName = r.d[i][1];
                        var designation = r.d[i][2];
                        var reportingManager = r.d[i][3] != null ? r.d[i][3].toString() : '';
                        var foto = r.d[i][4];
                        var respondeDesempenho = r.d[i][5];
                        var respondeLideranca = r.d[i][6];
                        var valueRespDes = r.d[i][7];
                        var valueRespLid = r.d[i][8];
                        var hideSlotAssociado = r.d[i][9];
                        var hideSlotNovo = r.d[i][10];
                        var corBackground = r.d[i][11];
                        var dragSlot = r.d[i][12];
                        var dragAssociado = r.d[i][13];
                        var temDrag = r.d[i][14];
                        data.addRows([[{
                            v: employeeId,
                            f: '<div id="div_' + i + '" ' + dragSlot + ' style="margin: 0 auto;width:100px;height:180px;align-content:center;background-color:' + corBackground + '">' +
                                (temDrag ? '<div id="drag_' + i + '" ' + dragAssociado + '>' +
                                '<label ' + hideSlotAssociado + ' class="switchRH">' +
                                '<input type="checkbox" id="inputRespLid_' + i + '" onclick="ReverterValor(&apos;inputRespLid_' + i + '&apos;)" value="' + valueRespLid + '" ' + respondeLideranca + '/>' +
                                '<span class="sloder rounde" style="width: 60px; height: 34px;"></span>' +
                                '</label>' +
                                '<br/>' +
                                '<div id="idAssociado_' + i + '" hidden>' + employeeId + "</div>" +
                                '<div id="idAssociadoHierarquia_' + i + '" hidden>' + reportingManager + "</div>" +
                                '<div style="color:#FFFFFF;font-size:14px" ' + hideSlotAssociado + '>' + employeeName + '</div>' +
                                '<button ' + hideSlotNovo + ' id="btnNovoSlot_' + i + '" style="color:white;font-size:90px;background:' + corBackground + '; border:none;height:150px;width:80px" onclick="NovoSlot(&apos;' + reportingManager + '&apos;);" type="button">+</button>' +
                                '<img ' + hideSlotAssociado + ' draggable="false" width=80px height=80px style="border-radius: 50%" src="' + foto + '" /> ' +
                                '<br/>' +
                                '<label ' + hideSlotAssociado + ' class="switchRH">' +
                                '<input type="checkbox" id="inputRespDes_' + i + '" onclick="ReverterValor(&apos;inputRespDes_' + i + '&apos;)" value="' + valueRespDes + '" ' + respondeDesempenho + '/>' +
                                '<span class="slider round" style="width: 60px; height: 34px;"></span>' +
                                '</label>' +
                                '</div>' : '') +
                                '</div>',
                            p: { style: 'background:' + corBackground + ' !important; background-image:none' }
                        }, reportingManager, '']]);
                        data.setRowProperty(i, 'style', 'background:' + corBackground + ' !important; background-image:none; border:none')
                    }

                    var chart = new google.visualization.OrgChart($("#chart")[0]);
                    chart.draw(data, { allowHtml: true });
                },
                failure: function (r) {
                    alert(r.d);
                },
                error: function (r) {
                    alert(r.d);
                }
            });
        }
    </script>
    <script type="text/javascript">
        function JavaScriptFunction() {
            document.getElementById('<%= hiddenHtmlCode.ClientID %>').value = document.getElementById('chart').innerHTML;
        }
    </script>
    <script type="text/javascript">
        function ReverterValor(ele) {
            if (document.getElementById(ele).value === "1") {
                document.getElementById(ele).value = "0";
            }
            else {
                document.getElementById(ele).value = "1";
            }
        }
    </script>
    <script type="text/javascript">
        function NovoSlot(idAssociadoHierarquia) {
            $.ajax({
                type: "POST",
                url: "CadastroProjetos.aspx/AddNovoSlot",
                data: '{ idAssociadoHierarquia: ' + idAssociadoHierarquia + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function () {
                    drawChart();

                    var UpdatePanel1 = '<%=UpdatePanel1.ClientID%>';
                    __doPostBack(UpdatePanel1, '');
                    return false;
                }
            });
            //document.getElementById('<%= hiddenHtmlCode.ClientID %>').value = document.getElementById('chart').innerHTML;
            //document.getElementById('<%= btnAtualizaEquipe.ClientID%>').click();
        }
    </script>


</asp:Content>