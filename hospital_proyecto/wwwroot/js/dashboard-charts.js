window.dashboardCharts = (function () {
  const charts = {};

  function buildColors(colors) {
    // acepta colores hex o rgba, si falta genera tonos grises
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
      data: {
        labels: labels,
        datasets: [{
          label: 'Casos',
          data: data,
          backgroundColor: bg,
          borderColor: bg,
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        scales: {
          y: { beginAtZero: true }
        },
        plugins: {
          legend: { display: false }
        }
      }
    });
  }

  function renderDonut(id, labels, data, colors) {
    const ctx = document.getElementById(id);
    if (!ctx) return;
    const bg = buildColors(colors);
    if (charts[id]) {
      charts[id].data.labels = labels;
      charts[id].data.datasets = [{ data: data, backgroundColor: bg }];
      charts[id].update();
      return;
    }
    charts[id] = new Chart(ctx.getContext('2d'), {
      type: 'doughnut',
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: bg,
          borderColor: '#111827', // match dark background
          borderWidth: 2
        }]
      },
      options: {
        responsive: true,
        plugins: { legend: { position: 'right' } }
      }
    });
  }

  return {
    renderBar: renderBar,
    renderDonut: renderDonut
  };
})();