// ...existing code...
window.dashboardCharts = (function () {
  const charts = {};

  function normalizeColors(colors, dataLength) {
    if (!colors || !colors.length) return Array(dataLength).fill('rgba(75,192,192,0.8)');
    if (!Array.isArray(colors)) return Array(dataLength).fill(colors);
    if (colors.length === 1) return Array(dataLength).fill(colors[0]);
    if (colors.length < dataLength) {
      const out = [];
      for (let i = 0; i < dataLength; i++) out.push(colors[i % colors.length]);
      return out;
    }
    return colors;
  }

  function renderBar(id, labels, data, colors) {
    console.log('dashboardCharts.renderBar', id, labels?.length, data?.length, colors);
    const ctx = document.getElementById(id);
    if (!ctx) { console.warn('canvas not found', id); return; }
    const bg = normalizeColors(colors, data?.length || 0);
    if (charts[id]) {
      charts[id].data.labels = labels;
      charts[id].data.datasets = [{ label: 'Casos', data: data, backgroundColor: bg }];
      charts[id].update();
      return;
    }
    charts[id] = new Chart(ctx.getContext('2d'), {
      type: 'bar',
      data: { labels: labels, datasets: [{ label: 'Casos', data: data, backgroundColor: bg, borderColor: bg, borderWidth: 1 }] },
      options: { responsive: true, scales: { y: { beginAtZero: true } }, plugins: { legend: { display: false } } }
    });
  }

  function renderDonut(id, labels, data, colors) {
    console.log('dashboardCharts.renderDonut', id, labels?.length, data?.length, colors);
    const ctx = document.getElementById(id);
    if (!ctx) { console.warn('canvas not found', id); return; }
    const bg = normalizeColors(colors, data?.length || 0);
    if (charts[id]) {
      charts[id].data.labels = labels;
      charts[id].data.datasets = [{ data: data, backgroundColor: bg }];
      charts[id].update();
      return;
    }
    charts[id] = new Chart(ctx.getContext('2d'), {
      type: 'doughnut',
      data: { labels: labels, datasets: [{ data: data, backgroundColor: bg, borderColor: '#fff', borderWidth: 1 }] },
      options: { responsive: true, plugins: { legend: { position: 'right' } } }
    });
  }

  function renderLine(id, labels, data, options = {}) {
    console.log('dashboardCharts.renderLine', id, labels?.length, data?.length, options);
    const ctx = document.getElementById(id);
    if (!ctx) { console.warn('canvas not found', id); return; }
    const ds = { label: options.label || 'Serie', data: data, borderColor: options.borderColor || 'rgb(75,192,192)', backgroundColor: options.backgroundColor || 'rgba(75,192,192,0.2)', fill: options.fill ?? true, tension: options.tension ?? 0.1, pointRadius: options.pointRadius ?? 3 };
    if (charts[id]) {
      charts[id].data.labels = labels;
      charts[id].data.datasets = [ds];
      charts[id].update();
      return;
    }
    charts[id] = new Chart(ctx.getContext('2d'), {
      type: 'line',
      data: { labels: labels, datasets: [ds] },
      options: { responsive: true, scales: { y: { beginAtZero: true } }, plugins: { legend: { display: options.showLegend ?? true } } }
    });
  }

  function renderPie(id, labels, data, colors) {
    console.log('dashboardCharts.renderPie', id, labels?.length, data?.length, colors);
    const ctx = document.getElementById(id);
    if (!ctx) { console.warn('canvas not found', id); return; }
    const bg = normalizeColors(colors, data?.length || 0);
    if (charts[id]) {
      charts[id].data.labels = labels;
      charts[id].data.datasets = [{ data: data, backgroundColor: bg }];
      charts[id].update();
      return;
    }
    charts[id] = new Chart(ctx.getContext('2d'), {
      type: 'pie',
      data: { labels: labels, datasets: [{ data: data, backgroundColor: bg, borderColor: '#fff', borderWidth: 1 }] },
      options: { responsive: true, plugins: { legend: { position: 'right' } } }
    });
  }

  return { renderBar, renderDonut, renderLine, renderPie };
})();