// =================== HÀM XỬ LÝ NGÀY ===================
const parseLocalDate = (s) => {
  if (!s) return null;
  return new Date(`${s}T00:00:00`);
};
const startOfDay = d => new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0, 0);
const endOfDay   = d => new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59, 999);

// =================== HÀM CHUNG ===================
const money = n => new Intl.NumberFormat('vi-VN').format(n || 0);
const API_BASE = "https://localhost:7107/api-ketoan/BaoCaoThongKe";

async function fetchApi(url, method = 'get', data = null) {
  try {
    const token = localStorage.getItem("token");
    const headers = token ? { Authorization: `Bearer ${token}` } : {};

    // 1) Nếu Angular đã bootstrap, lấy $http từ injector
    if (window.angular && angular.element && angular.element(document.body).injector) {
      try {
        const $injector = angular.element(document.body).injector();
        if ($injector) {
          const $http = $injector.get('$http');
          if (method.toLowerCase() === 'get') {
            const res = await $http.get(url, { headers });
            return res.data;
          } else if (method.toLowerCase() === 'post') {
            const res = await $http.post(url, data, { headers });
            return res.data;
          } else {
            const res = await $http({ method, url, data, headers });
            return res.data;
          }
        }
      } catch (e) {
        // tiếp tục fallback nếu injector/get thất bại
        console.warn('Không lấy được $http từ injector, sẽ dùng fallback', e);
      }
    }

    // 2) Fallback: axios nếu có
    if (window.axios) {
      if (method.toLowerCase() === 'get') {
        const res = await axios.get(url, { headers });
        return res.data;
      } else {
        const res = await axios({ method, url, data, headers });
        return res.data;
      }
    }

    // 3) Cuối cùng fallback sang fetch
    const fetchHeaders = Object.assign({ 'Content-Type': 'application/json' }, headers);
    const opts = { method: method.toUpperCase(), headers: fetchHeaders };
    if (data) opts.body = JSON.stringify(data);
    const resp = await fetch(url, opts);
    if (!resp.ok) throw new Error(`HTTP ${resp.status} ${resp.statusText}`);
    const json = await resp.json();
    return json;
  } catch (err) {
    console.error("Lỗi khi gọi API:", err);
    // Hiển thị thông báo ngắn gọn
    try { alert("Không thể tải dữ liệu từ máy chủ! Kiểm tra console."); } catch(e){}
    return [];
  }
}

// =================== FORM BÁO CÁO ===================
document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("report-form");
  const result = document.getElementById("report-results");

  if (!form || !result) {
    // nếu không tìm thấy form/result thì không làm gì
    console.warn("Form báo cáo hoặc vùng kết quả không tồn tại trên trang.");
    return;
  }

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

  if (!Array.isArray(data) || data.length === 0) {
    return `<p class="muted">Không có dữ liệu trong khoảng thời gian này.</p>`;
  }

  const filtered = data.filter(h => {
    const d = new Date(h.ngaylap || h.NGAYLAP || h.NgayLap);
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
    const mahd = h.mahdban || h.MAHDBAN || h.MaHDBan || h.MAHDBAN;
    const ngay = new Date(h.ngaylap || h.NGAYLAP || h.NgayLap);
    const tong = Number(h.tongtienhang || h.TONGTIENHANG || h.TongTienHang || 0);
    const thue = Number(h.thuevat || h.THUEVAT || 0);
    const giam = Number(h.giamgia || h.GIAMGIA || 0);

    html += `<tr>
      <td>${mahd}</td>
      <td>${isNaN(ngay) ? '' : ngay.toLocaleDateString("vi-VN")}</td>
      <td>${money(tong)}</td>
      <td>${money(thue)}</td>
      <td>${money(giam)}</td>
    </tr>`;
    total += tong;
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
  if (!Array.isArray(invoices) || invoices.length === 0) {
    return `<p class="muted">Không có dữ liệu bán hàng trong khoảng thời gian này.</p>`;
  }

  const filteredInvoices = invoices.filter(h => {
    const d = new Date(h.ngaylap || h.NGAYLAP || h.NgayLap);
    return d >= from && d <= to;
  });

  // Gộp sản phẩm theo mã
  const salesMap = {};
  filteredInvoices.forEach(hd => {
    const list = hd.listjson_chitietban || hd.listjson_chitietban || hd.ListJson_ChiTietBan || [];
    if (Array.isArray(list)) {
      list.forEach(ct => {
        const masp = ct.masp || ct.MASP || ct.MASP;
        const soluong = Number(ct.soluong || ct.SOLUONG || 0);
        const tongtien = Number(ct.tongtien || ct.TONGTIEN || 0);
        if (!salesMap[masp]) salesMap[masp] = { masp, soluong: 0, tongtien: 0 };
        salesMap[masp].soluong += soluong;
        salesMap[masp].tongtien += tongtien;
      });
    }
  });

  const arr = Object.values(salesMap);
  if (!arr.length) return `<p class="muted">Không có dữ liệu bán hàng trong khoảng thời gian này.</p>`;

  const sorted = arr.sort((a, b) => b.soluong - a.soluong).slice(0, 5);

  // Lấy tên SP từ API sản phẩm (để hiển thị)
  const products = await fetchApi(`${API_BASE}/get-all-sanpham`);
  const nameMap = {};
  if (Array.isArray(products)) products.forEach(sp => {
    const id = sp.masp || sp.MASP || sp.MaSP;
    const name = sp.tensp || sp.TENSP || sp.TenSP || sp.Ten;
    if (id) nameMap[id] = name;
  });

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

  if (!Array.isArray(data) || data.length === 0) {
    return `<p class="muted">Không có dữ liệu tồn kho trong khoảng thời gian này.</p>`;
  }

  // Nếu có "ngaynhap" thì lọc theo ngày nhập, nếu không thì lấy toàn bộ
  const from = startOfDay(fromDate);
  const to = endOfDay(toDate);
  const filtered = data.filter(sp => {
    if (!sp.ngaynhap && !sp.NGAYNHAP) return true;
    const d = new Date(sp.ngaynhap || sp.NGAYNHAP || sp.NgayNhap);
    return d >= from && d <= to;
  });

  const sorted = [...filtered].sort((a, b) => (b.soluongton || b.SOLUONGTON || 0) - (a.soluongton || a.SOLUONGTON || 0)).slice(0, 5);

  if (!sorted.length)
    return `<p class="muted">Không có dữ liệu tồn kho trong khoảng thời gian này.</p>`;

  let html = `<table>
    <thead><tr><th>Mã SP</th><th>Tên sản phẩm</th><th>Tồn kho</th><th>Đơn giá (₫)</th></tr></thead><tbody>`;

  sorted.forEach(sp => {
    const id = sp.masp || sp.MASP || sp.MaSP;
    const name = sp.tensp || sp.TENSP || sp.TenSP || sp.Ten;
    const qty = sp.soluongton ?? sp.SOLUONGTON ?? 0;
    const price = sp.dongia || sp.DONGIA || sp.DonGia || 0;
    html += `<tr>
      <td>${id}</td>
      <td>${name}</td>
      <td>${qty}</td>
      <td>${money(price)}</td>
    </tr>`;
  });

  html += `</tbody></table>`;
  return html;
}