var Dashboard = function () {

    var ChartLinePerformance = function () {

        var json = $('input[id$=hfPerformance]').val();

        if (json != '') {
            var obj = JSON.parse(json);

            var ctx = document.getElementById("linechart").getContext('2d');
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
                type: 'line',
                data: {
                    labels: obj.labels,
                    datasets: [
                        {                            
                            backgroundColor: 'rgba(255, 255, 255, 255)',
                            borderColor: window.chartColors.blue,
                            pointBackgroundColor: window.chartColors.blue,                            
                            fill: false,
                            label: 'Performance Geral',                           
                            lineTension: 0,
                            data: obj.dataset
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        yAxes: [{
                            ticks: {
                                max: 11,
                                beginAtZero: true
                            },
                            stacked: true
                        }]
                    }
                }

            });
        }
    }

    var RadarMediaConsolidado = function () {

        var json = $('input[id$=hfCompetencia]').val();

        if (json != '') {
            var obj = JSON.parse(json);

            var ctx = document.getElementById("radarn1").getContext('2d');
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
                    labels: obj.labels,
                    datasets: [
                        {
                            label: 'Média',
                            backgroundColor: 'rgba(255, 255, 255, 255)',
                            borderColor: window.chartColors.blue,
                            pointBackgroundColor: window.chartColors.blue,
                            data: obj.datasetconsolidado
                        },
                        {
                            label: 'Próximo Nível',
                            backgroundColor: 'rgba(255, 255, 255, 255)',
                            data: obj.datasetnivel
                        }
                    ]
                },
                options: {
                    responsive: true,
                    showAllTooltips : false,
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
                        position: "left",
                        fontSize: 5,
                    },
                    plugins: {
                        datalabels: {
                            display:false                           
                        }
                    }
                }

            });
        }
    }

    var AvaliadosPorPeriodo = function () {

        var json = $('input[id$=hfAvaliadosPeriodo]').val();
        var obj = JSON.parse(json);

        var ctx = document.getElementById("baravaliadosperiodo").getContext('2d');

        colors = [];

        for (let i = 0; i < obj.dataset.length; i++) {
            this.colors.push('#' + Math.floor(Math.random() * 16777215).toString(16));
        }

        var chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: obj.labels,
                datasets: [
                    {
                        data: obj.dataset,
                        backgroundColor: this.colors
                    }
                ]
            },
            options: {
                responsive: true,
                legend: {
                    display: false
                },
                scales: {
                    xAxes: [{
                        stacked: true
                    }],
                    yAxes: [{
                        stacked: true
                    }]
                }

            }

        });
    }

    var AndamentoAvaliacoes = function () {

        var json = $('input[id$=hfAndamentoAvaliacoes]').val();
        var obj = JSON.parse(json);

        var ctx = document.getElementById("pieandamentoavaliacoes").getContext('2d');

        colors = [];

        for (let i = 0; i < obj.dataset.length; i++) {
            this.colors.push('#' + Math.floor(Math.random() * 16777215).toString(16));
        }

        var chart = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: obj.labels,
                datasets: [
                    {
                        data: obj.dataset,
                        backgroundColor: this.colors
                    }
                ]
            },
            options: {
                responsive: true,
                legend: { position: 'right' },
                plugins: {
                    datalabels: {
                        formatter: (value, ctx) => {
                            let sum = 0;
                            let dataArr = ctx.chart.data.datasets[0].data;
                            dataArr.map(data => {
                                sum += data;
                            });
                            let percentage = Math.round(value * 100 / sum) + "%";
                            return percentage;
                        },
                        color: '#fff',
                    }
                }

            }

        });
    }

    return {

        init: function () {
            AndamentoAvaliacoes();
            AvaliadosPorPeriodo();
            RadarMediaConsolidado();
            ChartLinePerformance();
           
        }

    };

}();

jQuery(document).ready(function () {
    Dashboard.init();
});
