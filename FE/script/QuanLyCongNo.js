(() => {
  const CONGNO_API = "https://localhost:7107/api-ketoan/QuanLyCongNo";  // công nợ / thanh toán
  const token = localStorage.getItem("token");
  const authHeaders = token ? { Authorization: `Bearer ${token}` } : {};
  const money = n => new Intl.NumberFormat('vi-VN').format(n || 0);

  async function apiGetUrl(url) {
    console.log('[API GET]', url);
    try {
      const res = await fetch(url, { headers: authHeaders });
      console.log('[API] status', res.status, url);
      if (res.status === 404) {
        console.warn('[API] 404 Not Found:', url);
        return [];
      }
      if (!res.ok) throw new Error(`HTTP ${res.status} ${res.statusText}`);
      const json = await res.json();
      return (json && typeof json === 'object' && 'success' in json) ? (json.data || []) : json;
    } catch (err) {
      console.error('[API GET] error', url, err);
      return [];
    }
  }

  async function apiPostUrl(url, payload) {
    console.log('[API POST]', url, payload);
    try {
      const res = await fetch(url, {
        method: 'POST',
        headers: Object.assign({ 'Content-Type': 'application/json' }, authHeaders),
        body: JSON.stringify(payload)
      });
      const txt = await res.text().catch(()=>null);
      if (!res.ok) {
        // return object with message so caller can inspect
        return { success: false, message: txt || `HTTP ${res.status}` , status: res.status };
      }
      try {
        const json = JSON.parse(txt);
        return (json && typeof json === 'object' && 'success' in json) ? json : json;
      } catch (e) {
        return txt;
      }
    } catch (err) {
      console.error('[API POST] error', url, err);
      return { success: false, message: err.message || String(err) };
    }
  }

  async function apiPatchUrl(url, payload = null) {
    console.log('[API PATCH]', url, payload);
    try {
      const opts = { method: 'PATCH', headers: Object.assign({}, authHeaders) };
      if (payload) {
        opts.headers['Content-Type'] = 'application/json';
        opts.body = JSON.stringify(payload);
      }
      const res = await fetch(url, opts);
      if (!res.ok) {
        const txt = await res.text().catch(()=>null);
        throw new Error(`HTTP ${res.status} ${txt || res.statusText}`);
      }
      const json = await res.json();
      return (json && typeof json === 'object' && 'success' in json) ? json : json;
    } catch (err) {
      console.error('[API PATCH] error', url, err);
      return { success: false, message: err.message || String(err) };
    }
  }

  // endpoints
  async function getAllInvoices() {
    return await apiGetUrl(`${CONGNO_API}/get-all-hoadonban`);
  }
  async function getAllPayments() {
    return await apiGetUrl(`${CONGNO_API}/get-all-thanhtoan`);
  }

  // try multiple candidate POST endpoints (fallback to avoid 404)
  async function tryPostEndpoints(list, payload) {
    for (const path of list) {
      const url = `${CONGNO_API}/${path}`;
      try {
        console.log('[API POST try]', url, payload);
        const res = await fetch(url, {
          method: 'POST',
          headers: Object.assign({ 'Content-Type': 'application/json' }, authHeaders),
          body: JSON.stringify(payload)
        });
        const txt = await res.text().catch(()=>null);
        if (!res.ok) {
          console.warn('[API POST] failed', url, res.status, txt);
          continue;
        }
        try { return JSON.parse(txt); } catch(e){ return txt; }
      } catch (err) {
        console.error('[API POST] error try', url, err);
      }
    }
    return { success: false, message: 'All POST endpoints failed' };
  }

  // Improved insertPayment:
  // 1) try direct known endpoint /insert-thanhtoan using apiPostUrl (gives clear status/message)
  // 2) if failed, try same endpoint without MaThanhToan (backend may generate it)
  // 3) fallback to candidate route names
  async function insertPayment(payload) {
    // try primary endpoint with full payload
    try {
      const res = await apiPostUrl(`${CONGNO_API}/insert-thanhtoan`, payload);
      console.log('[API POST] primary insert-thanhtoan response', res);
      if (res && (res.success === true || res.data || (res.status && Number(res.status) < 400))) return res;
    } catch (e) { console.error('[API POST] primary error', e); }

    // try without MaThanhToan (if backend auto-generates)
    const payloadNoId = Object.assign({}, payload);
    delete payloadNoId.MaThanhToan;
    try {
      const res2 = await apiPostUrl(`${CONGNO_API}/insert-thanhtoan`, payloadNoId);
      console.log('[API POST] insert-thanhtoan without id response', res2);
      if (res2 && (res2.success === true || res2.data || (res2.status && Number(res2.status) < 400))) return res2;
    } catch (e) { console.error('[API POST] try without id error', e); }

    // final fallback list (adjust if your backend uses different names)
    const candidates = [
      'insert-thanh-toan',
      'insertThanhToan',
      'create-thanhtoan',
      'createThanhToan',
      'update-thanhtoan'
    ];
    return await tryPostEndpoints(candidates, payloadNoId);
  }

  async function updateInvoiceStatus(maHoaDon, trangThai) {
    const q = `?maHoaDon=${encodeURIComponent(maHoaDon)}&trangThai=${encodeURIComponent(trangThai)}`;
    const patchUrl = `${CONGNO_API}/update-trangthai-hoadon${q}`;
    const patchRes = await apiPatchUrl(patchUrl);
    if (patchRes && (patchRes.success === true || patchRes.message === undefined)) {
      console.log('Cập nhật trạng thái hóa đơn qua PATCH OK', maHoaDon, trangThai);
      return patchRes;
    }
    const postUrl = `${CONGNO_API}/update-trangthai-hoadon`;
    const postRes = await apiPostUrl(postUrl, { maHoaDon, trangThai });
    if (postRes && (postRes.success === true || postRes.message === undefined)) {
      console.log('Cập nhật trạng thái hóa đơn qua POST OK', maHoaDon, trangThai);
      return postRes;
    }
    console.warn('Không cập nhật được trạng thái hóa đơn', maHoaDon, trangThai);
    return { success: false, message: 'Không cập nhật trạng thái hóa đơn' };
  }

  // utilities: extract invoice id from payment reliably
  function extractInvoiceIdFromPayment(p) {
    if (!p || typeof p !== 'object') return null;
    const tries = [
      'MaHDBan','MAHDBAN','mahdban','maHDBan','mahdBan','MaHD','mahd','maHoaDon','mahoadon','MAHDBan'
    ];
    for (const k of tries) if (k in p && p[k] != null && String(p[k]).trim() !== '') return String(p[k]).trim();
    for (const k of Object.keys(p)) {
      const lk = k.toLowerCase();
      if (lk.includes('ma') && (lk.includes('hd') || lk.includes('hdb') || lk.includes('hoadon'))) return String(p[k]).trim();
    }
    return null;
  }

  // try extract invoice total from invoice object
  function extractInvoiceTotalFromInvoice(inv) {
    if (!inv || typeof inv !== 'object') return 0;
    const keys = ['TONGTIENHANG','tongtienhang','TONGTIEN','tongtien','tong','TONG','tongTien','TongTien'];
    for (const k of keys) {
      if (k in inv && inv[k] != null && inv[k] !== '') {
        const v = Number(inv[k]);
        if (!isNaN(v)) return v;
      }
    }
    for (const k of Object.keys(inv)) {
      const v = Number(inv[k]);
      if (!isNaN(v) && v > 0) return v;
    }
    return 0;
  }

  // Lấy IDs hóa đơn có thanh toán với trangThai = "Chưa thanh toán"
  function getUnpaidInvoiceIdsFromPayments(payments) {
    const set = new Set();
    (Array.isArray(payments) ? payments : []).forEach(p => {
      const status = (p.TrangThai || p.trangThai || p.trangthai || p.TRANGTHAI || '').toString().toLowerCase();
      const id = extractInvoiceIdFromPayment(p);
      if (!id) return;
      if (status.includes('chưa') || status.includes('chua')) set.add(String(id));
    });
    return set;
  }

  // compose invoices + payments, attach paid / owed / hasUnpaid
  async function fetchInvoicesWithPayments() {
    const invoicesRaw = await getAllInvoices();
    const paymentsRaw = await getAllPayments();

    const payments = Array.isArray(paymentsRaw) ? paymentsRaw : (paymentsRaw?.data || []);
    const invoices = Array.isArray(invoicesRaw) ? invoicesRaw : (invoicesRaw?.data || []);

    if (payments && payments.length) console.log('[CONGNO] sample payment:', payments[0]);
    if (invoices && invoices.length) console.log('[CONGNO] sample invoice:', invoices[0]);

    // build maps
    const paidMap = {};
    const unpaidFlagMap = {};
    (payments || []).forEach(p => {
      const id = extractInvoiceIdFromPayment(p);
      const status = (p.TrangThai || p.trangThai || p.trangthai || p.TRANGTHAI || '').toString().toLowerCase();
      const amt = Number(p.SoTienThanhToan || p.sotienthanhtoan || p.soTien || p.sotien || p.SOTIENTHANHTOAN || 0) || 0;
      if (!id) return;
      const sid = String(id);
      paidMap[sid] = (paidMap[sid] || 0) + (isNaN(amt) ? 0 : amt);
      if (status.includes('chưa') || status.includes('chua')) unpaidFlagMap[sid] = true;
    });

    const normInv = (Array.isArray(invoices) ? invoices : []).map(h => {
      const id = h.MAHDBAN || h.mahdban || h.maHDBan || h.id || h.MaHoaDon || h.mahoadon || h.madh || h.madoc;
      const cus = h.TENKH || h.tenkh || h.TenKH || h.KHACH || h.khach || h.Ten || h.kh || '';
      const ngay = h.NGAYLAP || h.ngaylap || h.ngay || h.NgayLap || (h.ngayTao || '');
      const tong = extractInvoiceTotalFromInvoice(h);
      const paid = paidMap[id] || 0;
      const hasUnpaid = !!unpaidFlagMap[id];
      const owed = Math.max(0, tong - paid);
      return { id, cus, ngay, tong, paid, owed, hasUnpaid };
    }).filter(x => x.id);

    // keep invoices that either have a 'chưa' payment record or still owe > 0
    const filtered = normInv.filter(inv => inv.hasUnpaid || inv.owed > 0);

    return { invoices: filtered, payments };
  }

  function groupDebts(invList) {
    const map = {};
    invList.forEach(h => {
      const owed = Math.max(0, (h.owed || 0));
      const key = (h.cus && String(h.cus).trim()) ? h.cus : 'Khách lẻ';
      if (!map[key]) map[key] = { khach: key, invoices: [], total: 0, owed: 0 };
      map[key].invoices.push(Object.assign({}, h, { owed, cus: key }));
      map[key].total += (h.tong || 0);
      map[key].owed += owed;
    });
    return Object.values(map).sort((a, b) => b.owed - a.owed);
  }

  async function renderDebts() {
    const { invoices: invs } = await fetchInvoicesWithPayments();
    const grouped = groupDebts(invs);
    const tbody = document.querySelector('#debt-table tbody');
    if (!tbody) return;
    if (!grouped.length) {
      tbody.innerHTML = '<tr><td colspan="5" class="muted">Không có khách hàng còn nợ (theo điều kiện "Chưa thanh toán").</td></tr>';
      return;
    }
    tbody.innerHTML = '';
    grouped.forEach(c => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${c.khach}</td>
        <td>${c.invoices.length}</td>
        <td>${money(c.total)}</td>
        <td>${money(c.owed)}</td>
        <td class="inline">
          <button class="btn ghost" data-act="view" data-k="${encodeURIComponent(c.khach)}">Xem hóa đơn</button>
          ${c.owed > 0 ? `<button class="btn primary" data-act="pay" data-k="${encodeURIComponent(c.khach)}">Thanh toán</button>` : `<span class="small muted">Đã thu sạch</span>`}
        </td>
      `;
      tbody.appendChild(tr);
    });
  }

  async function renderInvoicesFor(khachEncoded) {
    const khach = decodeURIComponent(khachEncoded);
    const { invoices: invs } = await fetchInvoicesWithPayments();
    const invoices = invs.filter(h => ((h.cus && String(h.cus).trim()) ? String(h.cus) : 'Khách lẻ') === khach);
    const area = document.getElementById('detail-area');
    if (!area) return;
    if (!invoices.length) {
      area.innerHTML = '<div class="muted">Không tìm thấy hóa đơn có trạng thái "Chưa thanh toán".</div>';
      return;
    }
    let html = `<div class="invoices"><h3>Hóa đơn chưa thanh toán của ${khach}</h3>
      <table class="debt-table"><thead><tr><th>ID</th><th>Ngày</th><th>Tổng (₫)</th><th>Đã trả (₫)</th><th>Còn lại (₫)</th><th>Hành động</th></tr></thead><tbody>`;
    invoices.forEach(h => {
      const owed = Math.max(0, (h.owed || 0));
      const showPay = owed > 0 || !!h.hasUnpaid;
      const statusLabel = h.hasUnpaid ? 'Chưa thanh toán' : (owed > 0 ? 'Chưa thanh toán' : 'Đã thanh toán');
      html += `<tr>
        <td>${h.id}</td>
        <td>${h.ngay || ''}</td>
        <td>${money(h.tong)}</td>
        <td>${money(h.paid || 0)}</td>
        <td class="${owed ? 'unpaid' : 'paid'}">${money(owed)}</td>
        <td>
          ${showPay ? `<button class="btn primary" data-pay="${h.id}">Thanh toán</button>` : `<span class="small muted">${statusLabel}</span>`}
        </td>
      </tr>`;
    });
    html += `</tbody></table></div>`;
    area.innerHTML = html;

    area.querySelectorAll('[data-pay]').forEach(b => {
      b.addEventListener('click', () => openPayForm(b.getAttribute('data-pay')));
    });
  }

  // open pay form: allow payment if hasUnpaid even when owed==0 (use invoice total as default)
  async function openPayForm(hoaDonId) {
    const { invoices: invs } = await fetchInvoicesWithPayments();
    const inv = invs.find(i => i.id === hoaDonId);
    if (!inv) return alert('Không tìm thấy hóa đơn.');
    const owed = Math.max(0, (inv.owed || 0));
    const maxAmt = Math.max(owed, inv.tong || 0);
    let defaultAmt = owed > 0 ? owed : (inv.tong || 0);
    if (defaultAmt <= 0 && maxAmt > 0) defaultAmt = maxAmt;
    let raw = prompt(`Hóa đơn ${hoaDonId} (Tổng ${money(inv.tong)}). Nhập số tiền thanh toán (mặc định ${money(defaultAmt)}):`, String(defaultAmt));
    if (raw === null) return;
    raw = String(raw).replace(/[^\d.-]/g, '');
    let amt = Number(raw) || 0;
    if (amt <= 0) amt = defaultAmt;
    if (maxAmt > 0 && amt > maxAmt) amt = maxAmt;
    const method = prompt('Phương thức thanh toán (Tiền mặt / Chuyển khoản / Thẻ):', 'Tiền mặt') || 'Tiền mặt';
    const trangThai = amt >= maxAmt ? 'Đã thanh toán' : 'Chưa thanh toán';

    // do not force MaThanhToan by default (backend may auto-generate)
    const payload = {
      MaHDBan: hoaDonId,
      PhuongThuc: method,
      SoTienThanhToan: amt,
      NgayThanhToan: new Date().toISOString(),
      TrangThai: trangThai
    };

    const res = await insertPayment(payload);
    console.log('[API POST] insertPayment response', res);
    if (res && (res.success === true || res.data || (res.status && Number(res.status) < 400))) {
      const upd = await updateInvoiceStatus(hoaDonId, trangThai);
      if (upd && (upd.success === true || upd.message === undefined)) {
        console.log('Trạng thái hóa đơn đã được cập nhật:', hoaDonId, trangThai);
      } else {
        console.warn('Không cập nhật được trạng thái hóa đơn qua API:', hoaDonId, upd);
      }
      alert(`Thanh toán ${money(amt)} cho ${hoaDonId} thành công.`);
      await renderDebts();
      await renderInvoicesFor(inv.cus);
    } else {
      console.error('Lỗi thanh toán', res);
      alert('Thanh toán thất bại. Kiểm tra Network/Console để xem response và endpoint.');
    }
  }

  document.addEventListener('DOMContentLoaded', async () => {
    if (!document.querySelector('#debt-table')) {
      document.body.insertAdjacentHTML('afterbegin', `
        <section class="card">
          <h2>Quản lý công nợ</h2>
          <div class="table-wrap">
            <table id="debt-table" class="table">
              <thead><tr><th>Khách hàng</th><th>Số hóa đơn</th><th>Tổng nợ (₫)</th><th>Còn nợ (₫)</th><th></th></tr></thead>
              <tbody></tbody>
            </table>
          </div>
          <div id="detail-area" style="margin-top:16px"></div>
        </section>
      `);
    }

    await renderDebts();

    document.getElementById('debt-table').addEventListener('click', e => {
      const btn = e.target;
      if (btn.matches('[data-act="view"]')) {
        renderInvoicesFor(btn.getAttribute('data-k'));
        document.getElementById('detail-area').scrollIntoView({ behavior: 'smooth' });
      } else if (btn.matches('[data-act="pay"]')) {
        renderInvoicesFor(btn.getAttribute('data-k'));
        document.getElementById('detail-area').scrollIntoView({ behavior: 'smooth' });
      }
    });
  });
})();