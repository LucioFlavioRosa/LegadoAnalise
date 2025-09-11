var Evolucao = function () {

    var Radar = function () {

        var json = $('input[id$=hfJsonRadar]').val();
        var obj = JSON.parse(json);

        var ctx = document.getElementById("radarassociado").getContext('2d');

        const listCores = ['azul1', 'azul2', 'azul3', 'azul4', 'azul5', 'azul6', 'azul7'];
        var color = Chart.helpers.color;
        window.chartColors = {
            azul1: 'rgb(0, 22, 72)',
            azul2: 'rgb(0, 41, 132)',
            azul3: 'rgb(0, 59, 192)',
            azul4: 'rgb(33, 102, 255)',
            azul5: 'rgb(113, 157, 255)',
            azul6: 'rgb(179, 203, 255)',
            azul7: 'rgb(221, 232, 255)'
        };
                      
        var chart = new Chart(ctx, {
            type: 'radar',
            data: {
                labels: obj.labels,
            },
            options: {
                responsive: true,
                layout: {
                    padding: 10
                },
                scale: {
                    angleLines: {
                        display: true,
                        color: "rgba(0,0,0,0.9)"
                    },
                    ticks: {
                        beginAtZero: true,
                        max: Math.floor(200 / Number(obj.stepsize)) * Number(obj.stepsize),
                        min: 0,
                        stepSize: obj.stepsize,
                        display: false,
                        fontSize: 20
                    },
                    pointLabels: {
                        fontSize: 15,
                        position: "top",
                    }
                },
                title: {
                    display: false
                },
                legend: {
                    display: true,
                    position: "bottom",
                    fontSize: 22
                },
                labels: {
                    fontSize: 22
                }
            }

        });               

        for (var d in obj.datasets) {
            var pavaliado = obj.datasets[d];
            addData(chart, pavaliado.periodo, color(window.chartColors.white).alpha(0.2).rgbString(), window.chartColors[listCores[d]], pavaliado.dataset);
        }
    }


    function addData(radar, label, backgroundColor, borderColor, data) {
        radar.data.datasets.push({
            data: data,
            label: label,
            borderColor: borderColor,
            backgroundColor: backgroundColor
        });

        radar.update();
    }

    return {

        init: function () {
            Radar();
        }

    };

}();

jQuery(document).ready(function () {
    Evolucao.init();
});