let reports = [];
let filtered = [];
let currentPage = 1;
const pageSize = 10;
let sortField = null;
let sortDir = 1;

function escapeHtml(str) {
  if (str == null) return '';
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;');
}

function methodPill(method) {
  const m = (method || '').toUpperCase();
  const cls = {
    GET: 'pill-get',
    POST: 'pill-post',
    PUT: 'pill-put',
    DELETE: 'pill-delete'
  }[m] || 'pill-other';
  return `<span class="pill ${cls}">${m}</span>`;
}

function statusLabel(response) {
  if (!response) return '<span class="muted">No response</span>';
  const s = response.status;
  const cls = s >= 200 && s < 400 ? 'status-ok' : 'status-bad';
  return `<span class="${cls}">${s}</span>`;
}

async function loadReports() {
  // ⭐ THIS is where PostgreSQL data is loaded
  const res = await fetch('/reports');
  reports = await res.json();
  filtered = [...reports];
  renderTable();
}

function applyFilters() {
  const search = document.getElementById('search').value.toLowerCase();
  const method = document.getElementById('filter-method').value;

  filtered = reports.filter(r => {
    const matchesSearch =
      r.url.toLowerCase().includes(search) ||
      r.method.toLowerCase().includes(search) ||
      (r.response && String(r.response.status).includes(search));

    const matchesMethod = method ? r.method === method : true;

    return matchesSearch && matchesMethod;
  });

  currentPage = 1;
  renderTable();
}

function sortBy(field) {
  if (sortField === field) sortDir *=  -1;
  else {
    sortField = field;
    sortDir = 1;
  }

  filtered.sort((a, b) => {
    const av = field === 'status' ? (a.response?.status || 0) : a[field];
    const bv = field === 'status' ? (b.response?.status || 0) : b[field];
    return av > bv ? sortDir : -sortDir;
  });

  renderTable();
}

function renderTable() {
  const tbody = document.getElementById('reports-body');
  tbody.innerHTML = '';

  const start = (currentPage - 1) * pageSize;
  const pageItems = filtered.slice(start, start + pageSize);

  if (pageItems.length === 0) {
    tbody.innerHTML = `<tr><td colspan="5" class="muted">No results.</td></tr>`;
    return;
  }

  pageItems.forEach((item, index) => {
    const rowId = `details-${start + index}`;

    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${escapeHtml(item.created_at)}</td>
      <td>${methodPill(item.method)}</td>
      <td>${escapeHtml(item.url)}</td>
      <td>${statusLabel(item.response)}</td>
      <td><button class="btn" data-target="${rowId}">Toggle</button></td>
    `;
    tbody.appendChild(tr);

    const detailsTr = document.createElement('tr');
    detailsTr.innerHTML = `
      <td colspan="5">
        <div class="details" id="${rowId}">
          <strong>Request Headers</strong>
          <pre>${escapeHtml(JSON.stringify(item.headers, null, 2))}</pre>

          <strong>Request Body</strong>
          <pre>${escapeHtml(item.body || '(empty)')}</pre>

          <strong>Response Headers</strong>
          <pre>${escapeHtml(JSON.stringify(item.response?.headers || {}, null, 2))}</pre>

          <strong>Response Body</strong>
          <pre>${escapeHtml(item.response?.body || '(empty)')}</pre>
        </div>
      </td>
    `;
    tbody.appendChild(detailsTr);
  });

  document.getElementById('page-info').textContent =
    `Page ${currentPage} of ${Math.ceil(filtered.length / pageSize)}`;
}

document.getElementById('search').addEventListener('input', applyFilters);
document.getElementById('filter-method').addEventListener('change', applyFilters);

document.getElementById('prev-page').addEventListener('click', () => {
  if (currentPage > 1) {
    currentPage--;
    renderTable();
  }
});

document.getElementById('next-page').addEventListener('click', () => {
  if (currentPage < Math.ceil(filtered.length / pageSize)) {
    currentPage++;
    renderTable();
  }
});

document.querySelectorAll('th[data-sort]').forEach(th => {
  th.addEventListener('click', () => sortBy(th.dataset.sort));
});

document.getElementById('dark-toggle').addEventListener('click', () => {
  document.body.classList.toggle('dark');
});

document.addEventListener('click', e => {
  const btn = e.target.closest('.btn');
  if (!btn) return;
  const id = btn.dataset.target;
  const el = document.getElementById(id);
  el.style.display = el.style.display === 'block' ? 'none' : 'block';
});

loadReports();
