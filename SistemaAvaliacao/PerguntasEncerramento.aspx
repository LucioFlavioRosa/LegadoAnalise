<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PerguntasEncerramento.aspx.cs" Inherits="SistemaAvaliacao.PerguntasEncerramento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-breadcrumb border-bottom">
                <div class="row">
                    <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                        <h5 class="font-medium text-uppercase mb-0">Perguntas de Encerramento</h5>
                    </div>
                    <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                        <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                            <ol class="breadcrumb mb-0 justify-content-end p-0">
                                <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro Básico de Projetos</a></li>
                                <li class="breadcrumb-item active" aria-current="page">Perguntas de Encerramento</li>
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
                
				
				<div class="card">
                    <div class="card-body">		
						<h4 class="card-title">Dados de Perguntas de Encerramento</h4>					
						<form action="#">
							<div class="form-body">
								<div class="row">
									<div class="col-md-3">
										<div class="form-group">
											<label>Código</label>
											<input type="text" class="form-control">
										</div>
									</div>
									<div class="col-md-6">
										<div class="form-group">
											<label>Pergunta de Encerramento</label>
											<input type="text" class="form-control">
										</div>
									</div>
									<div class="col-md-3">
										<div class="form-group">
											<label>Possui comentário?</label>											
											<select class="form-control p-0">
												<option></option>
												<option value="S">SIM</option>
												<option value="N">NÃO</option>
											</select>
										</div>
									</div>
								</div>
							</div>
							<div class="form-actions">
								<div class="text-right">
									<button type="submit" class="btn btn-info">Cadastrar/Editar</button>
									<button type="reset" class="btn btn-dark">Limpar</button>
								</div>
							</div>
						</form>
                    </div>
				</div>
				
				<div class="card">

					<div class="card-body">
						<h4 class="card-title">Lista de Perguntas de Encerramento</h4>
						<h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code> pergunta de encerramento código</code>. </h6>
						<div class="table-responsive">
							<table class="table table-striped border dtInit">
								<thead>
									<tr>
										<th>Código</th>
										<th>Pergunta de Encerramento</th>
										<th>Possui comentário?</th>
										<th data-sort="0">Ação</th>
									</tr>
								</thead>
								<tbody>
									<tr>
										<td>1</td>
										<td>Teste 1</td>
										<td>SIM</td>
										<td> <button type="button" class="btn btn-info btn-outline btn-circle btn-lg m-r-5"><i class="ti-pencil-alt"></i></button> <button type="button" class="btn btn-dark btn-outline btn-circle btn-lg m-r-5"><i class="ti-trash"></i></button> </td>
									</tr>
									<tr>
										<td>2</td>
										<td>Teste 2</td>
										<td>NÃO</td>
										<td> <button type="button" class="btn btn-info btn-outline btn-circle btn-lg m-r-5"><i class="ti-pencil-alt"></i></button> <button type="button" class="btn btn-dark btn-outline btn-circle btn-lg m-r-5"><i class="ti-trash"></i></button> </td>
									</tr>
								</tbody>
								<tfoot>
									<tr>
										<th>Código</th>
										<th>Pergunta de Encerramento</th>
										<th>Possui comentário?</th>
										<th>Ação</th>
									</tr>
								</tfoot>
							</table>
						</div>
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
    <!--<script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
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
