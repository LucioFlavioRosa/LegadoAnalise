var Consolidacao = function () {


    var Competencia = function () {

        $(".ddlCompetencia").change(function () {

            var nota = $(this).children("option:selected").val();
            var idavaliacaocompetencia = $(this).data("id");
            var nivel = $(this).data("nivel");

            $.ajax({
                url: "avalizacao_consolidacao.aspx/SaveCompetencia",
                type: "POST",
                data: '{id:"' + idavaliacaocompetencia + '",nivel:"' + nivel + '",nota:"' + nota + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (data) {
                                        
                    if (data != null && data.d != null) {

                       var json = $.parseJSON(data.d);

                        if (nivel == 1) {
                            $("#tdcomiten1_" + idavaliacaocompetencia).empty();
                            $("#tdcomiten1_" + idavaliacaocompetencia).html(json.nota);
                        }
                        else if (nivel == 2) {
                            $("#tdcomiten2_" + idavaliacaocompetencia).empty();
                            $("#tdcomiten2_" + idavaliacaocompetencia).html(json.nota);
                        }
                    }
                },
                error: function (data) {

                    if (data != null && data.d != null) {
                        var json = $.parseJSON(data.d);
                        alert(json.erro);
                    }
                    else {
                        alert('Erro ao tentar salvar a nota do comitê.');
                    }
                }
            });
        });

    }

    var Performance = function () {

        $(".ddlPerformance").change(function () {

            var nota = $(this).children("option:selected").val();
            var idavaliacaoperformance = $(this).data("id");
           
            $.ajax({
                url: "avalizacao_consolidacao.aspx/SavePerformance",
                type: "POST",
                data: '{id:"' + idavaliacaoperformance + '",nota:"' + nota + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (data) {

                    if (data != null && data.d != null) {

                        var json = $.parseJSON(data.d);

                        $("#tdperformance_" + idavaliacaoperformance).empty();
                        $("#tdperformance_" + idavaliacaoperformance).html(json.nota);
                    }
                },
                error: function (data) {

                    if (data != null && data.d != null) {
                        var json = $.parseJSON(data.d);
                        alert(json.erro);
                    }
                    else {
                        alert('Erro ao tentar salvar a nota do comitê.');
                    }
                }
            });
        });

    }

    var Radar = function () {

        var json = $('input[id$=hfJsonRadar]').val();
        var obj = JSON.parse(json);

        var ctx = document.getElementById("radarprojeto").getContext('2d');
        var color = Chart.helpers.color;
        window.chartColors = {
            red: 'rgb(255, 99, 132)',
            orange: 'rgb(255, 159, 64)',
            yellow: 'rgb(255, 205, 86)',
            green: 'rgb(75, 192, 192)',
            blue: 'rgb(54, 162, 235)',
            purple: 'rgb(153, 102, 255)',
            grey: 'rgb(231,233,237)'
        };

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
                    }
                },
                title: {
                    display: false
                },
                legend: {
                    display: true,
                    position: "bottom",
                    fontSize: 5,
                }
            }

        });
    }

    return {

        init: function () {
            Competencia(); 
            Performance();
            //Radar();
        }

    };

}();

jQuery(document).ready(function () {
    Consolidacao.init();
});




