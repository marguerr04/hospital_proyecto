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

    function renderBar(id, labels, data) {
        const ctx = document.getElementById(id);
        if (!ctx) return;

        const bg = buildPalette(data.length);

        if (charts[id]) {
            charts[id].data.labels = labels;
            charts[id].data.datasets = [{
                label: 'Casos',
                data: data,
                backgroundColor: bg
            }];
            charts[id].update();
            return;
        }

        charts[id] = new Chart(ctx.getContext('2d'), {
            type: 'bar',
            data: {
                labels,
                datasets: [{
                    label: 'Casos',
                    data: data,
                    backgroundColor: bg
                }]
            },
            options: {
                responsive: true,
                scales: { y: { beginAtZero: true } }
            }
        });
    }

    function renderDonut(id, labels, data) {
        const ctx = document.getElementById(id);
        if (!ctx) return;

        const bg = buildPalette(data.length);

        if (charts[id]) {
            charts[id].data.labels = labels;
            charts[id].data.datasets = [{
                label: 'Estados',
                data,
                backgroundColor: bg
            }];
            charts[id].update();
            return;
        }

        charts[id] = new Chart(ctx.getContext('2d'), {
            type: 'doughnut',
            data: {
                labels,
                datasets: [{
                    label: 'Estados',
                    data,
                    backgroundColor: bg
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: 'right' }
                }
            }
        });
    }

    return {
        renderBar,
        renderDonut
    };
})();
