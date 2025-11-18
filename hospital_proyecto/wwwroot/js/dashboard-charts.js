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
      options: { responsive: true, scales: { y: { beginAtZero: true } }, plugins: { legend: { display: false } }, onClick: function (evt, elements) { if (elements && elements.length) { const el = elements[0]; const idx = el.index; const lbl = this.data.labels[idx]; const val = this.data.datasets[0].data[idx]; window.dashboardCharts.showDetail(lbl, val); } } }
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
      options: { responsive: true, plugins: { legend: { position: 'right' } }, onClick: function(evt, elements) { if (elements && elements.length) { const el = elements[0]; const idx = el.index; const lbl = this.data.labels[idx]; const val = this.data.datasets[0].data[idx]; window.dashboardCharts.showDetail(lbl, val); } } }
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
      options: { responsive: true, scales: { y: { beginAtZero: true } }, plugins: { legend: { display: options.showLegend ?? true } }, onClick: function(evt, elements) { if (elements && elements.length) { const el = elements[0]; const idx = el.index; const lbl = this.data.labels[idx]; const val = this.data.datasets[0].data[idx]; window.dashboardCharts.showDetail(lbl, val); } } }
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
      options: { responsive: true, plugins: { legend: { position: 'right' } }, onClick: function(evt, elements) { if (elements && elements.length) { const el = elements[0]; const idx = el.index; const lbl = this.data.labels[idx]; const val = this.data.datasets[0].data[idx]; window.dashboardCharts.showDetail(lbl, val); } } }
    });
  }

  function showDetail(label, value, sexo, ges) {
    try {
      // crear modal básico
      const existing = document.getElementById('dc-modal-root');
      if (existing) existing.remove();
      const root = document.createElement('div');
      root.id = 'dc-modal-root';
      root.style.position = 'fixed';
      root.style.left = '0';
      root.style.top = '0';
      root.style.width = '100vw';
      root.style.height = '100vh';
      root.style.background = 'rgba(0,0,0,0.5)';
      root.style.display = 'flex';
      root.style.alignItems = 'center';
      root.style.justifyContent = 'center';
      root.style.zIndex = 9999;

      const panel = document.createElement('div');
      panel.style.background = '#fff';
      panel.style.borderRadius = '8px';
      panel.style.padding = '16px';
      panel.style.width = '80%';
      panel.style.maxWidth = '900px';
      panel.style.boxShadow = '0 8px 24px rgba(0,0,0,0.2)';

      const title = document.createElement('h3');
      title.innerText = `Detalle: ${label} (${value})`;
      panel.appendChild(title);

      const info = document.createElement('div');
      info.style.fontSize = '0.9rem';
      info.style.marginBottom = '8px';
      info.innerText = `Filtros — Sexo: ${sexo || 'Todos'}, GES: ${ges || 'Todos'}`;
      panel.appendChild(info);

      const canvas = document.createElement('canvas');
      canvas.id = 'dc-detail-chart';
      canvas.style.width = '100%';
      canvas.style.height = '320px';
      panel.appendChild(canvas);

      const close = document.createElement('button');
      close.innerText = 'Cerrar';
      close.style.marginTop = '12px';
      close.onclick = () => { root.remove(); };
      panel.appendChild(close);

      root.appendChild(panel);
      document.body.appendChild(root);

      // render simple chart con el valor (puede reemplazarse por fetch a API para detalle real)
      const ctxD = canvas.getContext('2d');
      new Chart(ctxD, {
        type: 'bar',
        data: { labels: [label], datasets: [{ label: 'Valor', data: [value], backgroundColor: ['rgba(33,150,243,0.8)'] }] },
        options: { responsive: true, scales: { y: { beginAtZero: true } }, plugins: { legend: { display: false } } }
      });
    }
    catch (e) { console.error('showDetail error', e); }
  }

  return { renderBar, renderDonut, renderLine, renderPie, showDetail };
})();