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
      if (!res.ok) {
        const txt = await res.text().catch(()=>null);
        throw new Error(`HTTP ${res.status} ${txt || res.statusText}`);
      }
      const json = await res.json();
      return (json && typeof json === 'object' && 'success' in json) ? json : json;
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

  // endpoints (lấy tất cả từ cùng controller CONGNO_API)
  async function getAllInvoices() {
    return await apiGetUrl(`${CONGNO_API}/get-all-hoadonban`);
  }
  async function getAllPayments() {
    return await apiGetUrl(`${CONGNO_API}/get-all-thanhtoan`);
  }
  async function insertPayment(payload) {
    return await apiPostUrl(`${CONGNO_API}/insert-thanhtoan`, payload);
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

  // Lấy IDs hóa đơn có thanh toán với trangThai = "Chưa thanh toán"
  function getUnpaidInvoiceIdsFromPayments(payments) {
    const set = new Set();
    (Array.isArray(payments) ? payments : []).forEach(p => {
      const status = (p.TrangThai || p.trangThai || p.trangthai || p.TRANGTHAI || '').toString().toLowerCase();
      const id = p.MaHDBan || p.MAHDBAN || p.mahdban || p.maHDBan || p.maHoaDon || p.mahoadon || p.hoaDonId;
      if (!id) return;
      if (status.includes('chưa') || status.includes('chua')) {
        set.add(id);
      }
    });
    return set;
  }

  // compose invoices + payments, then filter to invoices that have payments marked 'Chưa thanh toán'
  async function fetchInvoicesWithPayments() {
    const invoicesRaw = await getAllInvoices();
    const paymentsRaw = await getAllPayments();

    const payments = Array.isArray(paymentsRaw) ? paymentsRaw : (paymentsRaw?.data || []);
    const invoices = Array.isArray(invoicesRaw) ? invoicesRaw : (invoicesRaw?.data || []);

    // IDs with payment TrangThai = "Chưa thanh toán"
    const unpaidIds = getUnpaidInvoiceIdsFromPayments(payments);

    const paidMap = {};
    payments.forEach(p => {
      const id = p.MaHDBan || p.MAHDBAN || p.mahdban || p.maHDBan || p.maHoaDon || p.mahoadon || p.hoaDonId;
      const amt = Number(p.SoTienThanhToan || p.sotienthanhtoan || p.soTien || p.sotien || 0);
      if (!id) return;
      paidMap[id] = (paidMap[id] || 0) + (isNaN(amt) ? 0 : amt);
    });

    const normInv = (Array.isArray(invoices) ? invoices : []).map(h => {
      const id = h.MAHDBAN || h.mahdban || h.maHDBan || h.id || h.MaHoaDon || h.mahoadon;
      const cus = h.TENKH || h.tenkh || h.TenKH || h.KHACH || h.khach || h.Ten || h.kh || '';
      const ngay = h.NGAYLAP || h.ngaylap || h.ngay || h.NgayLap || (h.ngayTao || '');
      const tong = Number(h.TONGTIENHANG || h.tongtien || h.tong || h.TONG || 0);
      const paid = paidMap[id] || 0;
      return { id, cus, ngay, tong, paid };
    }).filter(x => x.id);

    // filter: keep only invoices that have at least one payment record with TrangThai 'Chưa thanh toán'
    const filtered = normInv.filter(inv => unpaidIds.has(inv.id));
    return { invoices: filtered, payments };
  }

  function groupDebts(invList) {
    const map = {};
    invList.forEach(h => {
      const owed = Math.max(0, (h.tong || 0) - (h.paid || 0));
      const key = h.cus || 'Khách lẻ';
      if (!map[key]) map[key] = { khach: key, invoices: [], total: 0, owed: 0 };
      map[key].invoices.push(Object.assign({}, h, { owed }));
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
    const invoices = invs.filter(h => (h.cus || '').toString() === khach);
    const area = document.getElementById('detail-area');
    if (!area) return;
    if (!invoices.length) {
      area.innerHTML = '<div class="muted">Không tìm thấy hóa đơn có trạng thái "Chưa thanh toán".</div>';
      return;
    }
    let html = `<div class="invoices"><h3>Hóa đơn chưa thanh toán của ${khach}</h3>
      <table class="debt-table"><thead><tr><th>ID</th><th>Ngày</th><th>Tổng (₫)</th><th>Đã trả (₫)</th><th>Còn lại (₫)</th><th>Hành động</th></tr></thead><tbody>`;
    invoices.forEach(h => {
      const owed = Math.max(0, (h.tong || 0) - (h.paid || 0));
      html += `<tr>
        <td>${h.id}</td>
        <td>${h.ngay || ''}</td>
        <td>${money(h.tong)}</td>
        <td>${money(h.paid || 0)}</td>
        <td class="${owed ? 'unpaid' : 'paid'}">${money(owed)}</td>
        <td>
          ${owed > 0 ? `<button class="btn primary" data-pay="${h.id}">Thanh toán</button>` : `<span class="small muted">Đã thanh toán</span>`}
        </td>
      </tr>`;
    });
    html += `</tbody></table></div>`;
    area.innerHTML = html;

    area.querySelectorAll('[data-pay]').forEach(b => {
      b.addEventListener('click', () => openPayForm(b.getAttribute('data-pay')));
    });
  }

  async function openPayForm(hoaDonId) {
    const { invoices: invs } = await fetchInvoicesWithPayments();
    const inv = invs.find(i => i.id === hoaDonId);
    if (!inv) return alert('Không tìm thấy hóa đơn.');
    const owed = Math.max(0, (inv.tong || 0) - (inv.paid || 0));
    let raw = prompt(`Hóa đơn ${hoaDonId} còn ${money(owed)}. Nhập số tiền thanh toán (bỏ trống = tất toán):`);
    if (raw === null) return;
    raw = String(raw).replace(/[^\d.-]/g, '');
    let amt = Number(raw) || 0;
    if (amt <= 0) amt = owed;
    if (amt > owed) amt = owed;
    const method = prompt('Phương thức thanh toán (Tiền mặt / Chuyển khoản / Thẻ):', 'Tiền mặt') || 'Tiền mặt';

    const trangThai = amt >= owed ? 'Đã thanh toán' : 'Chưa thanh toán';
    const payload = {
      MaThanhToan: 'TT' + Date.now(),
      MaHDBan: hoaDonId,
      PhuongThuc: method,
      SoTienThanhToan: amt,
      NgayThanhToan: new Date().toISOString(),
      TrangThai: trangThai
    };

    const res = await insertPayment(payload);
    if (res && (res.success === true || res.data)) {
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
      alert('Thanh toán thất bại. Kiểm tra console.');
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