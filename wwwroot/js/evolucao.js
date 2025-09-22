window.renderRadarChart = (jsonData) => {
    try {
        const data = JSON.parse(jsonData);
        const ctx = document.getElementById('radarassociado');
        
        if (!ctx) {
            console.error('Canvas element not found');
            return;
        }

        if (window.radarChart) {
            window.radarChart.destroy();
        }

        const datasets = data.datasets.map((item, index) => {
            const colors = [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 205, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)'
            ];
            
            const borderColors = [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 205, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)'
            ];

            return {
                label: item.periodo,
                data: item.dataset,
                backgroundColor: colors[index % colors.length],
                borderColor: borderColors[index % borderColors.length],
                borderWidth: 2,
                pointBackgroundColor: borderColors[index % borderColors.length],
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: borderColors[index % borderColors.length]
            };
        });

        const config = {
            type: 'radar',
            data: {
                labels: data.labels,
                datasets: datasets
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    r: {
                        beginAtZero: true,
                        max: 200,
                        ticks: {
                            stepSize: data.stepsize || 50
                        }
                    }
                },
                plugins: {
                    legend: {
                        position: 'top'
                    },
                    title: {
                        display: true,
                        text: 'Evolução do Associado'
                    }
                }
            }
        };

        window.radarChart = new Chart(ctx, config);
    } catch (error) {
        console.error('Erro ao renderizar gráfico radar:', error);
    }
};