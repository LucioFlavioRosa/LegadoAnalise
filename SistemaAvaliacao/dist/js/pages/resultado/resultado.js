var ResultadoComite = function () {

    var color = Chart.helpers.color;
    window.chartColors = {
        yellow: 'rgb(252, 176, 25)',
        blue: 'rgb(0, 51, 92)',
        red: 'rgb(255, 99, 132)',
        orange: 'rgb(255, 159, 64)',
        green: 'rgb(75, 192, 192)',
        purple: 'rgb(153, 102, 255)',
        grey: 'rgb(231,233,237)'
    };

    var Radar = function () {

        var json = $('input[id$=hfJsonRadar]').val();
        var obj = JSON.parse(json);
        var index = 0;
        for (var d in obj) {

            var ctx = document.getElementById('radar_'+ index +'_' + obj[d].idprojeto).getContext('2d');
            var chart = new Chart(ctx, {
                type: 'radar',
                data: {
                    labels: obj[d].labels,
                    datasets: [{
                        label: 'Projeto',
                        backgroundColor: color(window.chartColors.blue).alpha(0.2).rgbString(),
                        borderColor: window.chartColors.blue,
                        pointBackgroundColor: window.chartColors.blue,
                        data: obj[d].datasetcompetencias
                    }, {
                        label: 'Nível Atual',
                        data: obj[d].datasetnivelatual,

                    }]
                },
                options: {
                    responsive: true,
                    scale: {
                        angleLines: {
                            display: false
                        },
                        ticks: {
                            suggestedMin: 50,
                            suggestedMax: 900
                        },
                        pointLabels: {
                            fontSize: 8,
                        }
                    },
                    title: {
                        display: false
                    },
                    legend: {
                        display: true,
                        position: "bottom",
                        "labels": {
                            "fontSize": 8,
                        }
                    }
                }

            });

            index++;
        }

    }

    var ModalSomaRadar = function () {
        $(".btnExpandSomaRadar").click(function () {
           
            var json = $('input[id$=hfJsonSomaRadar]').val();
            var obj = JSON.parse(json);
                      
            var ctx = document.getElementById('radar_modal').getContext('2d');
            var chart = new Chart(ctx, {
                type: 'radar',
                data: {
                    labels: obj.labels,
                    datasets: [{
                        label: 'Projeto',
                        backgroundColor: color(window.chartColors.blue).alpha(0.2).rgbString(),
                        borderColor: window.chartColors.blue,
                        pointBackgroundColor: window.chartColors.blue,
                        data: obj.datasetcompetencias
                    }, {
                        label: 'Nível Atual',
                        data: obj.datasetnivelatual,

                    }]
                },
                options: {
                    responsive: true,
                    scale: {
                        angleLines: {
                            display: false
                        },
                        ticks: {
                            suggestedMin: 50,
                            suggestedMax: 900
                        },
                        pointLabels: {
                            fontSize: 12,
                        }
                    },
                    title: {
                        display: false
                    },
                    legend: {
                        display: true,
                        position: "bottom",
                        "labels": {
                            "fontSize": 12,
                        }
                    }
                }

            });

            $('#modalRadar').modal();
        });
    }

    var SomaRadar = function () {

        var json = $('input[id$=hfJsonSomaRadar]').val();
        var obj = JSON.parse(json);

        var ctx = document.getElementById('radar_comite').getContext('2d');
        var chart = new Chart(ctx, {
            type: 'radar',
            data: {
                labels: obj.labels,
                datasets: [{
                    label: 'Projeto',
                    backgroundColor: color(window.chartColors.blue).alpha(0.2).rgbString(),
                    borderColor: window.chartColors.blue,
                    pointBackgroundColor: window.chartColors.blue,
                    data: obj.datasetcompetencias
                }, {
                    label: 'Nível Atual',
                        data: obj.datasetproximonivel,

                }]
            },
            options: {
                responsive: true,
                scale: {
                    angleLines: {
                        display: false
                    },
                    ticks: {
                        suggestedMin: 50,
                        suggestedMax: 900
                    },
                    pointLabels: {
                        fontSize: 8,
                    }
                },
                title: {
                    display: false
                },
                legend: {
                    display: true,
                    position: "bottom",
                    "labels": {
                        "fontSize": 8,
                    }
                }
            }

        });

    }

    var ModalRadar = function () {
        $(".btnExpandRadar").click(function () {
            var idprojeto = $(this).data("id");

            var json = $('input[id$=hfJsonRadar]').val();
            var data = JSON.parse(json);

            var obj = data.filter(function (data) { return data.idprojeto == idprojeto });

            var ctx = document.getElementById('radar_modal').getContext('2d');
            var chart = new Chart(ctx, {
                type: 'radar',
                data: {
                    labels: obj[0].labels,
                    datasets: [{
                        label: 'Projeto',
                        backgroundColor: color(window.chartColors.blue).alpha(0.2).rgbString(),
                        borderColor: window.chartColors.blue,
                        pointBackgroundColor: window.chartColors.blue,
                        data: obj[0].datasetcompetencias
                    }, {
                        label: 'Nível Atual',
                        data: obj[0].datasetnivelatual,

                    }]
                },
                options: {
                    responsive: true,
                    scale: {
                        angleLines: {
                            display: false
                        },
                        ticks: {
                            suggestedMin: 50,
                            suggestedMax: 900
                        },
                        pointLabels: {
                            fontSize: 12,
                        }
                    },
                    title: {
                        display: false
                    },
                    legend: {
                        display: true,
                        position: "bottom",
                        "labels": {
                            "fontSize": 12,
                        }
                    }
                }

            });

            $('#modalRadar').modal();
        });
    }

    var CollapseProjetos = function () {

        $(".btncollapse").click(function () {


            if ($(this).children("i").hasClass("fa-plus-circle")) {
                $(this).children("i").removeClass("fa-plus-circle");
                $(this).children("i").addClass("fa-minus-circle");

                $('table tr.hidelinha').addClass('showlinha');
                $('table tr.hidelinha').removeClass('hidelinha');

                $('#divajuste').removeClass('closeTable');
                $('#divajuste').addClass('openTable');               
                
            }
            else {
                $(this).children("i").removeClass("fa-minus-circle");
                $(this).children("i").addClass("fa-plus-circle");

                $('table tr.showlinha').addClass('hidelinha');
                $('table tr.showlinha').removeClass('showlinha');

                $('#divajuste').removeClass('openTable');
                $('#divajuste').addClass('closeTable');
            }
        });

    }

    var Complexidade = function () {
        $(".btncomplexidade").click(function () {
            var idprojeto = $(this).data("id");
            $("#hdIdProjeto").val(idprojeto);

            $.ajax({
                url: "avalizacao_resultado.aspx/ComplexidadeProjeto",
                type: "POST",
                data: '{id:"' + idprojeto + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (data) {

                    if (data != null && data.d != null) {

                        var json = $.parseJSON(data.d);

                        var $dropdown = $("#ddlComplexidade");
                        $dropdown.empty();
                        $.each(json.listcomplexidade, function () {
                            $dropdown.append($("<option />").val(this.IdComplexidade).text(this.Complexidade));
                        });

                        $dropdown.val(json.idprojetocomplexidade); 

                        $("#modalComplexidadeLabel").empty();
                        $("#modalComplexidadeLabel").html("Projeto " + json.nomeprojeto);
                        
                    }
                },
                error: function (data) {

                    if (data != null && data.d != null) {
                        var json = $.parseJSON(data.d);
                        alert(json.erro);
                    }
                    else {
                        alert('Erro ao tentar carregar a combo complexidade.');
                    }
                }
            });
    

            $('#modalComplexidade').modal();
        });


        $("#btnSalvarcomplexidade").click(function () {
            var idprojeto = $("#hdIdProjeto").val();

            if (idprojeto != '') {

                var idcomplexidade = $("#ddlComplexidade").children("option:selected").val();

                $.ajax({
                    url: "avalizacao_resultado.aspx/SalvarComplexidadeProjeto",
                    type: "POST",
                    data: '{idprojeto:"' + idprojeto + '",idcomplexidade:"' + idcomplexidade + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    success: function (data) {

                        if (data != null && data.d != null) {

                            var json = $.parseJSON(data.d);

                            if (json.atualizado) {
                                $('#modalComplexidade').hide();
                                location.reload();
                            }

                            $('#modalComplexidade').hide();
                        }
                    },
                    error: function (data) {

                        if (data != null && data.d != null) {
                            var json = $.parseJSON(data.d);
                            alert(json.erro);
                        }
                        else {
                            alert('Erro ao tentar carregar a combo complexidade.');
                        }
                    }
                });

            }
            else {
                alert('Projeto não encontrado.')
            }
            
        });
        

    }

    return {

        init: function () {
            CollapseProjetos();
            ModalRadar();
            ModalSomaRadar();
            Radar();
            SomaRadar();
            Complexidade();
        }

    };

}();

jQuery(document).ready(function () {
    ResultadoComite.init();
});




