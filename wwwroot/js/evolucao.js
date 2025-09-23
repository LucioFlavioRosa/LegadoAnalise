// Função para renderizar o gráfico radar da evolução do associado
window.renderRadarChart = (canvasId, jsonData) => {
    try {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error('Canvas não encontrado:', canvasId);
            return;
        }

        const ctx = canvas.getContext('2d');
        const data = JSON.parse(jsonData);

        // Destruir gráfico existente se houver
        if (window.radarChart) {
            window.radarChart.destroy();
        }

        // Configurar cores para os datasets
        const colors = [
            'rgba(54, 162, 235, 0.6)',   // Azul
            'rgba(255, 99, 132, 0.6)',   // Vermelho
            'rgba(75, 192, 192, 0.6)',   // Verde
            'rgba(255, 206, 86, 0.6)',   // Amarelo
            'rgba(153, 102, 255, 0.6)',  // Roxo
            'rgba(255, 159, 64, 0.6)'    // Laranja
        ];

        const borderColors = [
            'rgba(54, 162, 235, 1)',
            'rgba(255, 99, 132, 1)',
            'rgba(75, 192, 192, 1)',
            'rgba(255, 206, 86, 1)',
            'rgba(153, 102, 255, 1)',
            'rgba(255, 159, 64, 1)'
        ];

        // Preparar datasets para o Chart.js
        const chartDatasets = data.datasets.map((dataset, index) => ({
            label: dataset.periodo,
            data: dataset.dataset,
            backgroundColor: colors[index % colors.length],
            borderColor: borderColors[index % borderColors.length],
            borderWidth: 2,
            pointBackgroundColor: borderColors[index % borderColors.length],
            pointBorderColor: '#fff',
            pointHoverBackgroundColor: '#fff',
            pointHoverBorderColor: borderColors[index % borderColors.length]
        }));

        // Configuração do gráfico
        const config = {
            type: 'radar',
            data: {
                labels: data.labels,
                datasets: chartDatasets
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    title: {
                        display: true,
                        text: 'Evolução das Competências',
                        font: {
                            size: 16,
                            weight: 'bold'
                        }
                    },
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            usePointStyle: true
                        }
                    }
                },
                scales: {
                    r: {
                        beginAtZero: true,
                        max: data.stepsize || 200,
                        ticks: {
                            stepSize: (data.stepsize || 200) / 5,
                            font: {
                                size: 10
                            }
                        },
                        grid: {
                            color: 'rgba(0, 0, 0, 0.1)'
                        },
                        angleLines: {
                            color: 'rgba(0, 0, 0, 0.1)'
                        },
                        pointLabels: {
                            font: {
                                size: 11,
                                weight: 'bold'
                            },
                            color: '#333'
                        }
                    }
                },
                elements: {
                    line: {
                        borderWidth: 2
                    },
                    point: {
                        radius: 4,
                        hoverRadius: 6
                    }
                },
                interaction: {
                    intersect: false,
                    mode: 'point'
                },
                animation: {
                    duration: 1000,
                    easing: 'easeInOutQuart'
                }
            }
        };

        // Criar o gráfico
        window.radarChart = new Chart(ctx, config);

        console.log('Gráfico radar renderizado com sucesso');
    } catch (error) {
        console.error('Erro ao renderizar gráfico radar:', error);
    }
};

// Função para limpar o gráfico
window.clearRadarChart = () => {
    if (window.radarChart) {
        window.radarChart.destroy();
        window.radarChart = null;
    }
};

// Função para redimensionar o gráfico
window.resizeRadarChart = () => {
    if (window.radarChart) {
        window.radarChart.resize();
    }
};

// Event listener para redimensionamento da janela
window.addEventListener('resize', () => {
    setTimeout(() => {
        window.resizeRadarChart();
    }, 100);
});