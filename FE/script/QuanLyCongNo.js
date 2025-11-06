(() => {
  const API_BASE = "https://localhost:7107/api-ketoan/QuanLyCongNo";
  const token = localStorage.getItem("token");
  const authHeaders = token ? { Authorization: `Bearer ${token}` } : {};
  const money = n => new Intl.NumberFormat('vi-VN').format(n || 0);

  // === API CALLS ===
  async function apiGet(url) {
    try {
      const res = await fetch(url, { headers: authHeaders });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      const json = await res.json();
      return json.data || [];
    } catch (err) {
      console.error("[API GET ERROR]", err);
      return [];
    }
  }

  async function apiPut(url) {
    try {
      const res = await fetch(url, { method: "PUT", headers: authHeaders });
      const json = await res.json().catch(() => ({}));
      if (!res.ok) throw new Error(json.message || res.statusText);
      return json;
    } catch (err) {
      console.error("[API PUT ERROR]", err);
      alert("❌ Lỗi cập nhật: " + err.message);
      return { success: false };
    }
  }

  async function fetchUnpaidInvoices() {
    return await apiGet(`${API_BASE}/get-hoadon-chuathanhtoan`);
  }

  async function updateInvoiceStatus(maHDBan) {
    return await apiPut(`${API_BASE}/update-trangthai-thanhtoan?maHDBan=${encodeURIComponent(maHDBan)}`);
  }

  // === HIỂN THỊ DANH SÁCH CÔNG NỢ ===
  async function renderUnpaidInvoices() {
    const invoices = await fetchUnpaidInvoices();
    const tbody = document.querySelector("#debt-table tbody");
    const detail = document.getElementById("detail-area");

    if (!tbody) return;

    if (!invoices.length) {
      tbody.innerHTML = `<tr><td colspan="5" class="muted">Không có khách hàng còn nợ.</td></tr>`;
      if (detail) detail.innerHTML = "";
      return;
    }

    // Gom nhóm hóa đơn theo khách hàng
    const grouped = {};
    invoices.forEach(inv => {
      const kh = inv.TenKhachHang || "Khách lẻ";
      if (!grouped[kh]) grouped[kh] = [];
      grouped[kh].push(inv);
    });

    // Hiển thị nhóm khách hàng
    tbody.innerHTML = "";
    Object.keys(grouped).forEach(kh => {
      const hoadons = grouped[kh];
      const tongNo = hoadons.reduce((a, h) => a + (h.SoTienPhaiTra || 0), 0);
      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${kh}</td>
        <td>${hoadons.length}</td>
        <td>${money(tongNo)} ₫</td>
        <td>${money(tongNo)} ₫</td>
        <td>
          <button class="btn ghost" data-view="${kh}">Xem</button>
        </td>`;
      tbody.appendChild(tr);
    });

    // Gán sự kiện "Xem hóa đơn"
    tbody.querySelectorAll("[data-view]").forEach(btn => {
      btn.addEventListener("click", () => showInvoicesOf(btn.getAttribute("data-view"), grouped));
    });
  }

  // === HIỂN THỊ CHI TIẾT HÓA ĐƠN CỦA 1 KHÁCH ===
  function showInvoicesOf(khach, grouped) {
    const area = document.getElementById("detail-area");
    const list = grouped[khach];
    if (!list || !list.length) return;

    let html = `
      <h3>Hóa đơn của khách hàng: ${khach}</h3>
      <table class="detail-table">
        <thead>
          <tr>
            <th>Mã hóa đơn</th>
            <th>Ngày lập</th>
            <th>Số tiền phải trả (₫)</th>
            <th>Trạng thái</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
    `;

    list.forEach(hd => {
      html += `
        <tr>
          <td>${hd.MaHoaDon}</td>
          <td>${new Date(hd.NgayLap).toLocaleDateString("vi-VN")}</td>
          <td>${money(hd.SoTienPhaiTra)} ₫</td>
          <td><span class="status unpaid">${hd.TrangThai || "Chưa thanh toán"}</span></td>
          <td>
            <button class="btn primary" data-id="${hd.MaHoaDon}">Đánh dấu đã thanh toán</button>
          </td>
        </tr>`;
    });

    html += `</tbody></table>`;
    area.innerHTML = html;

    area.querySelectorAll("[data-id]").forEach(btn => {
      btn.addEventListener("click", async () => {
        const id = btn.getAttribute("data-id");
        if (!confirm(`Xác nhận đánh dấu hóa đơn ${id} là "Đã thanh toán"?`)) return;
        const res = await updateInvoiceStatus(id);
        if (res && res.success) {
          alert(`✅ Hóa đơn ${id} đã được cập nhật thành "Đã thanh toán".`);
          renderUnpaidInvoices();
        }
      });
    });
  }

  // === CHẠY KHI TRANG LOAD ===
  document.addEventListener("DOMContentLoaded", renderUnpaidInvoices);
})();
