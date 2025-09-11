var Feedback = function () {


    var Radar = function () {

        var json = $('input[id$=hfJsonRadar]').val();
        var obj = JSON.parse(json);

        var ctx = document.getElementById("radarprojeto").getContext('2d');
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

        var chart = new Chart(ctx, {
            type: 'radar',
            data: {
                labels: obj[0].labels,
                datasets: [
                    {
                        label: 'Avaliação Gestor',     
                        backgroundColor: 'rgba(255, 255, 255, 255)',
                        borderColor: window.chartColors.blue,
                        pointBackgroundColor: window.chartColors.blue,
                        data: obj[0].datasetgestor
                    },
                    {
                        label: 'Auto Avaliação',   
                        backgroundColor: 'rgba(255, 255, 255, 255)',
                        borderColor: window.chartColors.yellow,
                        pointBackgroundColor: window.chartColors.yellow,
                        data: obj[0].datasetavaliado
                    },                    
                    {
                        label: 'Nível Atual',    
                        backgroundColor: 'rgba(255, 255, 255, 255)',
                        data: obj[0].datasetatual
                    },
                    {
                        label: 'Próximo Nível',                     
                        backgroundColor: 'rgba(255, 255, 255, 255)',
                        borderColor: window.chartColors.grey,
                        pointBackgroundColor: window.chartColors.grey,
                        data: obj[0].datasetproximonivel
                    }
                ]
            },
            options: {
                responsive: true,
                scale: {
                    angleLines: {
                        display: false
                    },
                    ticks: {
                        suggestedMin: 0,
                        suggestedMax: 200,
                        maxTicksLimit: 1,
                        display: false
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
            Radar();
        }

    };

}();

jQuery(document).ready(function () {
    Feedback.init();
});
