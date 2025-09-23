// Dashboard Chart.js Integration
let dashboardCharts = {};

// Initialize all dashboard charts
window.initializeDashboardCharts = () => {
    // Initialize empty charts that will be populated later
    console.log('Dashboard charts initialized');
};

// Update pie chart
window.updatePieChart = (canvasId, jsonData) => {
    try {
        const data = JSON.parse(jsonData);
        const ctx = document.getElementById(canvasId);
        
        if (dashboardCharts[canvasId]) {
            dashboardCharts[canvasId].destroy();
        }
        
        dashboardCharts[canvasId] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: data.labels,
                datasets: [{
                    data: data.dataset,
                    backgroundColor: [
                        '#007bff',
                        '#28a745',
                        '#ffc107',
                        '#dc3545',
                        '#17a2b8',
                        '#6f42c1',
                        '#fd7e14'
                    ]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error updating pie chart:', error);
    }
};

// Update bar chart
window.updateBarChart = (canvasId, jsonData) => {
    try {
        const data = JSON.parse(jsonData);
        const ctx = document.getElementById(canvasId);
        
        if (dashboardCharts[canvasId]) {
            dashboardCharts[canvasId].destroy();
        }
        
        dashboardCharts[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Quantidade',
                    data: data.dataset,
                    backgroundColor: '#007bff',
                    borderColor: '#0056b3',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error updating bar chart:', error);
    }
};

// Update radar chart
window.updateRadarChart = (canvasId, jsonData) => {
    try {
        const data = JSON.parse(jsonData);
        const ctx = document.getElementById(canvasId);
        
        if (dashboardCharts[canvasId]) {
            dashboardCharts[canvasId].destroy();
        }
        
        dashboardCharts[canvasId] = new Chart(ctx, {
            type: 'radar',
            data: {
                labels: data.labels,
                datasets: [
                    {
                        label: 'Consolidado',
                        data: data.datasetconsolidado,
                        backgroundColor: 'rgba(0, 123, 255, 0.2)',
                        borderColor: '#007bff',
                        borderWidth: 2
                    },
                    {
                        label: 'Nível Máximo',
                        data: data.datasetnivel,
                        backgroundColor: 'rgba(40, 167, 69, 0.1)',
                        borderColor: '#28a745',
                        borderWidth: 1,
                        borderDash: [5, 5]
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    r: {
                        beginAtZero: true,
                        max: 200
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error updating radar chart:', error);
    }
};

// Update line chart
window.updateLineChart = (canvasId, jsonData) => {
    try {
        const data = JSON.parse(jsonData);
        const ctx = document.getElementById(canvasId);
        
        if (dashboardCharts[canvasId]) {
            dashboardCharts[canvasId].destroy();
        }
        
        dashboardCharts[canvasId] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Performance',
                    data: data.dataset,
                    backgroundColor: 'rgba(23, 162, 184, 0.2)',
                    borderColor: '#17a2b8',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error updating line chart:', error);
    }
};

// Cleanup function
window.destroyDashboardCharts = () => {
    Object.values(dashboardCharts).forEach(chart => {
        if (chart) {
            chart.destroy();
        }
    });
    dashboardCharts = {};
};