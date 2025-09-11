$('.gridResultado').DataTable({
	"order": [],
	"columnDefs": [{
		"targets": 'no-sort',
		"orderable": false,
	}],
	dom: "<'row'<'col-md-6'l><'col-md-5'f><'col-md-1'B>>" +
		"<'row'<'col-md-12'tr>>" +
		"<'row'<'col-md-5'i><'col-md-7'p>>",
	buttons: [
		'excel'
	],
	"lengthMenu": [
		[-1, 25, 50, 100, 150],
		["Todos",25, 50, 100, 150]
	],
	"language": {
		"decimal": ",",
		"thousands": ".",
		"sEmptyTable": "Nenhum registro encontrado",
		"sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
		"sInfoEmpty": "Sem registros",
		"sInfoFiltered": "(Filtrados de _MAX_ registros)",
		"sInfoPostFix": "",
		"sInfoThousands": ".",
		"sLengthMenu": "_MENU_ resultados por página",
		"sLoadingRecords": "Carregando...",
		"sProcessing": "Processando...",
		"sZeroRecords": "Nenhum registro compatível com o filtro",
		"sSearch": "Pesquisar",
		"oPaginate": {
			"sNext": "Próximo",
			"sPrevious": "Anterior",
			"sFirst": "Primeiro",
			"sLast": "Último"
		},
		"oAria": {
			"sSortAscending": ": Ordenar colunas de forma ascendente",
			"sSortDescending": ": Ordenar colunas de forma descendente"
		}
	}
});