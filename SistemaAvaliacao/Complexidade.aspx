<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Complexidade.aspx.cs" Inherits="SistemaAvaliacao.Complexidade" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
              <div class="page-breadcrumb border-bottom">
                <div class="row">
                    <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                        <h5 class="font-medium text-uppercase mb-0">Complexidade</h5>
                    </div>
                    <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                        <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                            <ol class="breadcrumb mb-0 justify-content-end p-0">
                                <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastros Básicos de Projetos</a></li>
                                <li class="breadcrumb-item active" aria-current="page">Complexidade</li>
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
                
				<%--DIV EDITAR CAMPOS COMPLEXIDADES --%>
				<div class="card">
                    <div class="card-body">		
						<h4 class="card-title">Dados da Complexidade</h4>					
							<div class="form-body">
								<div class="row">
									<div class="col-md-6">
										<div class="form-group">
											<label>Complexidade</label>
											<asp:TextBox ID="txtComplexidade" class="form-control" runat="server"></asp:TextBox>

										</div>
									</div>
									<div class="col-md-6">
										<div class="form-group">
											<label>Status</label>
                                            <asp:DropDownList ID="ddlStatus" class="form-control" runat="server">
                                                <asp:ListItem Value="1">Ativo</asp:ListItem>
                                                <asp:ListItem Value="0">Inativo</asp:ListItem>
                                            </asp:DropDownList>
										</div>
									</div>
								</div>
								<div class="row">
									<div class="col-md-2">
										<div class="form-group">
											<label>Peso</label>
											<asp:TextBox ID="txtPeso" class="form-control" runat="server" TextMode="Number"></asp:TextBox>
										</div>
									</div>
									<div class="col-md-2">
										<div class="form-group">
											<label>Peso Ponderado</label>
											<asp:TextBox ID="txtPesoPonderado" class="form-control" runat="server" TextMode="Number"></asp:TextBox>
										</div>
									</div>
									<div class="col-md-2">
										<div class="form-group">
											<label>% Ponderação</label>
											<asp:TextBox ID="txtPonderacao" class="form-control" runat="server" TextMode="Number"></asp:TextBox>
										</div>
									</div>
									<div class="col-md-3">
										<div class="form-group">
											<label>% Faixa Inicial de Ponderação</label>
											<asp:TextBox ID="txtInicial" class="form-control" runat="server" TextMode="Number"></asp:TextBox>
										</div>
									</div>
									<div class="col-md-3">
										<div class="form-group">
											<label>% Faixa Final de Ponderação</label>
											<asp:TextBox ID="txtFinal" class="form-control" runat="server" TextMode="Number"></asp:TextBox>
										</div>
									</div>
							</div>
							<div class="form-actions">
								<div class="text-right">
									<asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />                                    
								</div>
							</div>
                        </div>
                    </div>
				</div>
				
				<%--DIV LISTA COMPLEXIDADES--%>
				<div class="card">

					<div class="card-body">
						<h4 class="card-title">Lista de Complexidades</h4>
						<h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code> complexidade código</code>. </h6>
						<div class="table-responsive">
							<asp:HiddenField ID="hdIdComplexidade" runat="server" />
							<asp:Repeater ID="rptComplexidade" OnItemCommand="rptComplexidade_ItemCommand" OnItemDataBound="rptComplexidade_ItemDataBound" runat="server">
								<HeaderTemplate>
									<table class="table table-striped border dtInit">     
										<thead>
											<tr>
												<th>Código</th>
												<th>Complexidade</th>
												<th>Mínimo Fatores</th>
												<th>Status(Ativo/Inativo)</th>
												<th data-sort="0">Ação</th>
											</tr>
										</thead>
										<tbody>
                                </HeaderTemplate>
								<ItemTemplate>
											<tr>
											<td><asp:Label ID="lblIdComplexidade" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idComplexidade") %>' Visible="false"></asp:Label>
												<%# DataBinder.Eval(Container.DataItem, "IdComplexidade") %></td>
											<td><%# DataBinder.Eval(Container.DataItem, "Complexidade") %></td>
											<td><%# DataBinder.Eval(Container.DataItem, "SomaMinimaFator") %></td>
											<td><asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
											<td>
												<asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="AlterarComplexidade" />
												<asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="ExcluirComplexidade" />
											</td>
											</tr>
								</ItemTemplate>
								<FooterTemplate>
										</tbody>
										<tfoot>
											<tr>
												<th>Código</th>
												<th>Complexidade</th>
												<th>Status(Ativo/Inativo)</th>
												<th data-sort="0">Ação</th>
											</tr>
										</tfoot>
									</table>
								</FooterTemplate>
							</asp:Repeater>
						</div>
					</div>
				</div>
				
				<%--DIV EDITAR CAMPOS FATORES--%>
				<div class="card card-body">
					<h4>Editar fatores de complexidades</h4>
					<h5>Os fatores abaixo são exibidos para mensuração de cada projeto pelo gestor na página Gerenciar Projetos.</h5>
					<div class="table-responsive">
						<asp:HiddenField runat="server" ID="idFatorSelecionado" />
					</div>
				</div>
				
                <!-- ============================================================== -->
                <!-- End PAge Content -->
                <!-- ============================================================== -->
                <!-- ============================================================== -->
                <!-- Right sidebar -->
                <!-- ============================================================== -->
                <!-- .right-sidebar -->
                <!-- ============================================================== -->
                <!-- End Right sidebar -->
                <!-- ============================================================== -->
            </div>
            <!-- ============================================================== -->
            <!-- End Container fluid  -->
            <!-- ============================================================== -->
			

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
<!--    <script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- apps -->
    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app.init.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
   <!-- <script src="dist/js/sidebarmenu.js"></script>
    <!--Custom JavaScript -->
	<script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
	

</asp:Content>
