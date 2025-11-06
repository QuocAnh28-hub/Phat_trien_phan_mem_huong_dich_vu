(() => {
  const API_BASE = "https://localhost:7107/api-ketoan/QuanLyCongNo";
  const token = localStorage.getItem("token");
  const authHeaders = token ? { Authorization: `Bearer ${token}` } : {};
  const money = n => new Intl.NumberFormat('vi-VN').format(n || 0);

  // === API ===
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
      alert("Lỗi cập nhật: " + err.message);
      return { success: false };
    }
  }

  async function fetchUnpaidInvoices() {
    return await apiGet(`${API_BASE}/get-hoadon-chuathanhtoan`);
  }

  async function updateInvoiceStatus(maHDBan, phuongThuc) {
    const url = `${API_BASE}/update-trangthai-thanhtoan?maHDBan=${encodeURIComponent(maHDBan)}&phuongThuc=${encodeURIComponent(phuongThuc)}`;
    return await apiPut(url);
  }

  // === HIỂN THỊ DANH SÁCH KHÁCH HÀNG CÒN NỢ ===
  async function renderUnpaidInvoices() {
    const invoices = await fetchUnpaidInvoices();
    const tbody = document.querySelector("#debt-table tbody");
    const detail = document.getElementById("detail-area");

    if (!tbody) return;

    if (!invoices.length) {
      tbody.innerHTML = `<tr><td colspan="6" class="muted">Không có khách hàng còn nợ.</td></tr>`;
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

    // Hiển thị danh sách khách hàng
    tbody.innerHTML = "";
    Object.keys(grouped).forEach(kh => {
      const hoadons = grouped[kh];
      const tongNo = hoadons.reduce((a, h) => a + (h.SoTienPhaiTra || 0), 0);
      const sdt = hoadons[0].SoDienThoai || "—";

      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${kh}</td>
        <td>${sdt}</td>
        <td>${hoadons.length}</td>
        <td>${money(tongNo)} ₫</td>
        <td>${money(tongNo)} ₫</td>
        <td><button class="btn ghost" style="background-color: #acacacff" data-view="${kh}">Xem</button></td>
      `;
      tbody.appendChild(tr);
    });

    // Sự kiện xem chi tiết hóa đơn
    tbody.querySelectorAll("[data-view]").forEach(btn => {
      btn.addEventListener("click", () => showInvoicesOf(btn.getAttribute("data-view"), grouped));
    });
  }

  // === HIỂN THỊ CHI TIẾT CỦA 1 KHÁCH HÀNG ===
  function showInvoicesOf(khach, grouped) {
    const area = document.getElementById("detail-area");
    const list = grouped[khach];
    if (!list || !list.length) return;

    const sdt = list[0].SoDienThoai || "—";

    let html = `
      <h3>Hóa đơn của khách hàng: ${khach} - SĐT: ${sdt}</h3>
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
          <td><button class="btn primary" data-id="${hd.MaHoaDon}">Thanh toán</button></td>
        </tr>`;
    });

    html += `</tbody></table>`;
    area.innerHTML = html;

    // Gán nút thanh toán
    area.querySelectorAll("[data-id]").forEach(btn => {
      btn.addEventListener("click", async () => {
        const id = btn.getAttribute("data-id");
        const phuongThuc = prompt(
          `Nhập phương thức thanh toán cho hóa đơn ${id} (VD: Tiền mặt, Chuyển khoản):`,
          "Tiền mặt"
        );

        if (!phuongThuc) {
          alert("Bạn đã hủy thao tác thanh toán. Hóa đơn vẫn giữ nguyên trạng thái 'Chưa thanh toán'.");
          return;
        }

        if (!confirm(`Xác nhận đánh dấu hóa đơn ${id} là 'Đã thanh toán' bằng ${phuongThuc}?`)) return;

        const res = await updateInvoiceStatus(id, phuongThuc);
        if (res && res.success) {
          alert(`Hóa đơn ${id} đã được cập nhật thành 'Đã thanh toán'.`);
          renderUnpaidInvoices();
        } else {
          alert("Cập nhật thất bại. Vui lòng thử lại!");
        }
      });
    });
  }

  document.addEventListener("DOMContentLoaded", renderUnpaidInvoices);
})();
