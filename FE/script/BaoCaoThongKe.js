// =================== HÀM XỬ LÝ NGÀY ===================
const parseLocalDate = (s) => {
  if (!s) return null;
  return new Date(`${s}T00:00:00`); // Giữ đúng múi giờ local
};
const startOfDay = d => new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0, 0);
const endOfDay   = d => new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59, 999);

// =================== HÀM CHUNG ===================
const money = n => new Intl.NumberFormat('vi-VN').format(n || 0);
const API_BASE = "https://localhost:7107/api-ketoan/BaoCaoThongKe";

// Hàm gọi API có kèm token
async function fetchApi(url) {
  try {
    const token = localStorage.getItem("token");
    const res = await axios.get(url, {
      headers: { Authorization: `Bearer ${token}` }
    });
    return res.data;
  } catch (err) {
    console.error("Lỗi khi gọi API:", err);
    alert("Không thể tải dữ liệu từ máy chủ!");
    return [];
  }
}

// =================== FORM BÁO CÁO ===================
document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("report-form");
  const result = document.getElementById("report-results");

  form.addEventListener("submit", async e => {
    e.preventDefault();

    const type = form.type.value;
    const fromStr = form.from.value;
    const toStr   = form.to.value;

    // ====== Kiểm tra ngày ======
    if (!fromStr || !toStr) {
      result.innerHTML = `<p style="color:red;font-weight:600;">Vui lòng chọn đầy đủ "Ngày bắt đầu" và "Ngày kết thúc"!</p>`;
      return;
    }

    const fromDate = parseLocalDate(fromStr);
    const toDate   = parseLocalDate(toStr);
    if (isNaN(fromDate) || isNaN(toDate)) {
      result.innerHTML = `<p style="color:red;font-weight:600;">Ngày không hợp lệ. Vui lòng chọn lại!</p>`;
      return;
    }
    if (fromDate > toDate) {
      result.innerHTML = `<p style="color:red;font-weight:600;">"Ngày bắt đầu" không được lớn hơn "Ngày kết thúc".</p>`;
      return;
    }

    const today = startOfDay(new Date());
    if (fromDate > today || toDate > today) {
      result.innerHTML = `<p style="color:red;font-weight:600;">Không được chọn ngày trong tương lai.</p>`;
      return;
    }

    let html = `<p><strong>Thời gian thống kê:</strong> ${fromStr} → ${toStr}</p>`;

    if (type === "Doanh thu") {
      html += await renderRevenue(fromDate, toDate);
    } else if (type === "Sản phẩm bán chạy") {
      html += await renderTopProducts(fromDate, toDate);
    } else if (type === "Sản phẩm tồn nhiều") {
      html += await renderStockedProducts(fromDate, toDate);
    }

    result.innerHTML = html;
  });
});

// =================== BÁO CÁO DOANH THU ===================
async function renderRevenue(fromDate, toDate) {
  const data = await fetchApi(`${API_BASE}/get-all-hoadonban`);
  const from = startOfDay(fromDate);
  const to   = endOfDay(toDate);

  const filtered = data.filter(h => {
    const d = new Date(h.ngaylap);
    return d >= from && d <= to;
  });

  if (!filtered.length) {
    return `<p class="muted">Không có dữ liệu trong khoảng thời gian này.</p>`;
  }

  let html = `<table>
    <thead>
      <tr><th>Mã hóa đơn</th><th>Ngày</th><th>Tổng tiền hàng (₫)</th><th>Thuế VAT</th><th>Giảm giá</th></tr>
    </thead><tbody>`;
  let total = 0;

  filtered.forEach(h => {
    html += `<tr>
      <td>${h.mahdban}</td>
      <td>${new Date(h.ngaylap).toLocaleDateString("vi-VN")}</td>
      <td>${money(h.tongtienhang)}</td>
      <td>${money(h.thuevat)}</td>
      <td>${money(h.giamgia)}</td>
    </tr>`;
    total += (h.tongtienhang || 0);
  });

  html += `</tbody>
    <tfoot><tr><td colspan="2">Tổng cộng</td><td>${money(total)}</td><td></td><td></td></tr></tfoot>
  </table>`;
  return html;
}

// =================== TOP 5 SẢN PHẨM BÁN CHẠY ===================
async function renderTopProducts(fromDate, toDate) {
  const from = startOfDay(fromDate);
  const to = endOfDay(toDate);

  const invoices = await fetchApi(`${API_BASE}/get-all-hoadonban`);
  const filteredInvoices = invoices.filter(h => {
    const d = new Date(h.ngaylap);
    return d >= from && d <= to;
  });

  // Gộp sản phẩm theo mã
  const salesMap = {};
  filteredInvoices.forEach(hd => {
    if (hd.listjson_chitietban && hd.listjson_chitietban.length > 0) {
      hd.listjson_chitietban.forEach(ct => {
        if (!salesMap[ct.masp]) {
          salesMap[ct.masp] = { masp: ct.masp, soluong: 0, tongtien: 0 };
        }
        salesMap[ct.masp].soluong += ct.soluong;
        salesMap[ct.masp].tongtien += ct.tongtien;
      });
    }
  });

  const arr = Object.values(salesMap);
  if (!arr.length) return `<p class="muted">Không có dữ liệu bán hàng trong khoảng thời gian này.</p>`;

  const sorted = arr.sort((a, b) => b.soluong - a.soluong).slice(0, 5);

  // Lấy tên SP từ API sản phẩm (để hiển thị)
  const products = await fetchApi(`${API_BASE}/get-all-sanpham`);
  const nameMap = {};
  products.forEach(sp => nameMap[sp.masp] = sp.tensp);

  let html = `<table>
    <thead><tr><th>Mã SP</th><th>Tên sản phẩm</th><th>Số lượng bán</th><th>Doanh thu (₫)</th></tr></thead><tbody>`;

  sorted.forEach(sp => {
    html += `<tr>
      <td>${sp.masp}</td>
      <td>${nameMap[sp.masp] || "(Không rõ)"}</td>
      <td>${sp.soluong}</td>
      <td>${money(sp.tongtien)}</td>
    </tr>`;
  });

  html += `</tbody></table>`;
  return html;
}

// =================== TOP 5 SẢN PHẨM TỒN NHIỀU ===================
async function renderStockedProducts(fromDate, toDate) {
  const data = await fetchApi(`${API_BASE}/get-all-sanpham`);

  // Nếu có "ngaynhap" thì lọc theo ngày nhập, nếu không thì lấy toàn bộ
  const from = startOfDay(fromDate);
  const to = endOfDay(toDate);
  const filtered = data.filter(sp => {
    if (!sp.ngaynhap) return true;
    const d = new Date(sp.ngaynhap);
    return d >= from && d <= to;
  });

  const sorted = [...filtered].sort((a, b) => (b.soluongton || 0) - (a.soluongton || 0)).slice(0, 5);

  if (!sorted.length)
    return `<p class="muted">Không có dữ liệu tồn kho trong khoảng thời gian này.</p>`;

  let html = `<table>
    <thead><tr><th>Mã SP</th><th>Tên sản phẩm</th><th>Tồn kho</th><th>Đơn giá (₫)</th></tr></thead><tbody>`;

  sorted.forEach(sp => {
    html += `<tr>
      <td>${sp.masp}</td>
      <td>${sp.tensp}</td>
      <td>${sp.soluongton ?? 0}</td>
      <td>${money(sp.dongia)}</td>
    </tr>`;
  });

  html += `</tbody></table>`;
  return html;
}
