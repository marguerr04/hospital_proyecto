window.dashboardCharts = (function () {
    const charts = {};

    const PALETTE = [
        "#4e79a7", "#f28e2b", "#e15759", "#76b7b2", "#59a14f",
        "#edc948", "#b07aa1", "#ff9da7", "#9c755f", "#bab0ab"
    ];

    function buildPalette(count) {
        const colors = [];
        for (let i = 0; i < count; i++) {
            colors.push(PALETTE[i % PALETTE.length]);
        }
        return colors;
    }

    function destroyChart(id) {
        if (charts[id]) {
            charts[id].destroy();
            charts[id] = null;
        }
    }

    function renderBar(id, labels, data, colors) {
        const ctx = document.getElementById(id);
        if (!ctx) {
            console.warn(`Canvas con id '${id}' no encontrado`);
            return;
        }

        const bg = colors || buildPalette(data.length);
        destroyChart(id);

        const canvasContainer = ctx.parentElement;
        if (canvasContainer) {
            canvasContainer.style.position = 'relative';
            canvasContainer.style.width = '100%';
            canvasContainer.style.height = '100%';
        }

        charts[id] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels.slice(0, 6), // Limitar etiquetas para mejor visualización
                datasets: [{
                    label: 'Casos',
                    data: data.slice(0, 6),
                    backgroundColor: bg.slice(0, 6),
                    borderRadius: 4,
                    hoverBackgroundColor: 'rgba(0,0,0,0.1)',
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'x',
                layout: {
                    padding: { top: 5, bottom: 5, left: 5, right: 5 }
                },
                scales: { 
                    y: { 
                        beginAtZero: true,
                        max: Math.max(...data.slice(0, 6)) + 1,
                        ticks: { font: { size: 10 } }
                    },
                    x: {
                        ticks: { font: { size: 9 } }
                    }
                },
                plugins: {
                    legend: { 
                        display: true, 
                        position: 'top',
                        labels: { font: { size: 10 }, padding: 10 }
                    },
                    tooltip: { 
                        callbacks: { label: (ctx) => `${ctx.dataset.label}: ${ctx.parsed.y}` },
                        titleFont: { size: 10 },
                        bodyFont: { size: 9 }
                    }
                }
            }
        });
    }

    function renderDonut(id, labels, data, colors) {
        const ctx = document.getElementById(id);
        if (!ctx) {
            console.warn(`Canvas con id '${id}' no encontrado`);
            return;
        }

        const bg = colors || buildPalette(data.length);
        destroyChart(id);

        const canvasContainer = ctx.parentElement;
        if (canvasContainer) {
            canvasContainer.style.position = 'relative';
            canvasContainer.style.width = '100%';
            canvasContainer.style.height = '100%';
        }

        charts[id] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels,
                datasets: [{
                    label: 'Estados',
                    data,
                    backgroundColor: bg,
                    borderColor: '#fff',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                layout: {
                    padding: { top: 5, bottom: 5, left: 5, right: 5 }
                },
                plugins: {
                    legend: { 
                        position: 'right',
                        labels: { font: { size: 10 }, padding: 12, boxWidth: 12 }
                    },
                    tooltip: { 
                        callbacks: { label: (ctx) => `${ctx.label}: ${ctx.parsed}` },
                        titleFont: { size: 10 },
                        bodyFont: { size: 9 }
                    }
                }
            }
        });
    }

    function renderLine(id, labels, data, options) {
        const ctx = document.getElementById(id);
        if (!ctx) {
            console.warn(`Canvas con id '${id}' no encontrado`);
            return;
        }

        const defaultOptions = options || {};
        const label = defaultOptions.label || 'Datos';
        destroyChart(id);

        const canvasContainer = ctx.parentElement;
        if (canvasContainer) {
            canvasContainer.style.position = 'relative';
            canvasContainer.style.width = '100%';
            canvasContainer.style.height = '100%';
        }

        charts[id] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels.slice(0, 12), // Limitar a últimos 12 meses
                datasets: [{
                    label: label,
                    data: data.slice(0, 12),
                    borderColor: '#4e79a7',
                    backgroundColor: 'rgba(78, 121, 167, 0.1)',
                    borderWidth: 2,
                    tension: 0.4,
                    fill: true,
                    pointRadius: 3,
                    pointHoverRadius: 5,
                    pointBackgroundColor: '#4e79a7',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 1.5
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                layout: {
                    padding: { top: 5, bottom: 5, left: 5, right: 5 }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        max: Math.max(...data.slice(0, 12)) + 1,
                        ticks: { font: { size: 10 } }
                    },
                    x: {
                        ticks: { font: { size: 9 } }
                    }
                },
                plugins: {
                    legend: { 
                        display: true, 
                        position: 'top',
                        labels: { font: { size: 10 }, padding: 10 }
                    },
                    tooltip: { 
                        mode: 'index', 
                        intersect: false,
                        titleFont: { size: 10 },
                        bodyFont: { size: 9 }
                    }
                }
            }
        });
    }

    function renderPie(id, labels, data, colors) {
        const ctx = document.getElementById(id);
        if (!ctx) {
            console.warn(`Canvas con id '${id}' no encontrado`);
            return;
        }

        const bg = colors || buildPalette(data.length);
        destroyChart(id);

        const canvasContainer = ctx.parentElement;
        if (canvasContainer) {
            canvasContainer.style.position = 'relative';
            canvasContainer.style.width = '100%';
            canvasContainer.style.height = '100%';
        }

        charts[id] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels,
                datasets: [{
                    label: 'Causales',
                    data,
                    backgroundColor: bg,
                    borderColor: '#fff',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                layout: {
                    padding: { top: 5, bottom: 5, left: 5, right: 5 }
                },
                plugins: {
                    legend: { 
                        position: 'bottom',
                        labels: { font: { size: 10 }, padding: 12, boxWidth: 12 }
                    },
                    tooltip: { 
                        callbacks: { label: (ctx) => `${ctx.label}: ${ctx.parsed}` },
                        titleFont: { size: 10 },
                        bodyFont: { size: 9 }
                    }
                }
            }
        });
    }

    function showDetail(label, value, sexoFilter, gesFilter) {
        console.log(`Detalle: ${label} = ${value} (Sexo: ${sexoFilter}, GES: ${gesFilter})`);
    }

    return {
        renderBar,
        renderDonut,
        renderLine,
        renderPie,
        showDetail,
        destroyChart
    };
})();
