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

        // Destruir gráfico anterior si existe
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
                labels,
                datasets: [{
                    label: 'Casos',
                    data: data,
                    backgroundColor: bg,
                    borderRadius: 4,
                    hoverBackgroundColor: 'rgba(0,0,0,0.1)'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'x',
                scales: { 
                    y: { 
                        beginAtZero: true,
                        max: Math.max(...data) + 2
                    }
                },
                plugins: {
                    legend: { display: true, position: 'top' },
                    tooltip: { callbacks: { label: (ctx) => `${ctx.dataset.label}: ${ctx.parsed.y}` } }
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

        // Destruir gráfico anterior si existe
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
                plugins: {
                    legend: { position: 'right' },
                    tooltip: { callbacks: { label: (ctx) => `${ctx.label}: ${ctx.parsed}` } }
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

        // Destruir gráfico anterior si existe
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
                labels,
                datasets: [{
                    label: label,
                    data: data,
                    borderColor: '#4e79a7',
                    backgroundColor: 'rgba(78, 121, 167, 0.1)',
                    borderWidth: 3,
                    tension: 0.4,
                    fill: true,
                    pointRadius: 5,
                    pointHoverRadius: 7,
                    pointBackgroundColor: '#4e79a7',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        max: Math.max(...data) + 1
                    }
                },
                plugins: {
                    legend: { display: true, position: 'top' },
                    tooltip: { mode: 'index', intersect: false }
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

        // Destruir gráfico anterior si existe
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
                plugins: {
                    legend: { position: 'bottom' },
                    tooltip: { callbacks: { label: (ctx) => `${ctx.label}: ${ctx.parsed}` } }
                }
            }
        });
    }

    function showDetail(label, value, sexoFilter, gesFilter) {
        console.log(`Detalle: ${label} = ${value} (Sexo: ${sexoFilter}, GES: ${gesFilter})`);
        // Aquí puedes agregar lógica adicional si es necesario
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
