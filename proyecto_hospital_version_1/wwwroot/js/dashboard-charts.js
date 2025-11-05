window.dashboardCharts = (function () {
    const charts = {};

    function buildColors(colors) {
        if (!colors || !colors.length) {
            return ['rgba(75,192,192,0.8)'];
        }
        return colors;
    }

    function renderBar(id, labels, data, colors) {
        const ctx = document.getElementById(id);
        if (!ctx) return;
        const bg = buildColors(colors);
        if (charts[id]) {
            charts[id].data.labels = labels;
            charts[id].data.datasets = [{ label: 'Casos', data: data, backgroundColor: bg }];
            charts[id].update();
            return;
        }
        charts[id] = new Chart(ctx.getContext('2d'), {
            type: 'bar',
            data: { labels, datasets: [{ data, backgroundColor: bg }] },
            options: { responsive: true, scales: { y: { beginAtZero: true } } }
        });
    }

    function renderDonut(id, labels, data, colors) {
        const ctx = document.getElementById(id);
        if (!ctx) return;
        const bg = buildColors(colors);
        if (charts[id]) {
            charts[id].data.labels = labels;
            charts[id].data.datasets = [{ data, backgroundColor: bg }];
            charts[id].update();
            return;
        }
        charts[id] = new Chart(ctx.getContext('2d'), {
            type: 'doughnut',
            data: { labels, datasets: [{ data, backgroundColor: bg }] },
            options: { responsive: true, plugins: { legend: { position: 'right' } } }
        });
    }

    return {
        renderBar,
        renderDonut
    };
})();
