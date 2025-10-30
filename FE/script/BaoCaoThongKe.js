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
    console.error("❌ Lỗi khi gọi API:", err);
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
    const from = form.from.value;
    const to = form.to.value;

    let html = `<p><strong>Thời gian thống kê:</strong> ${from || "---"} → ${to || "---"}</p>`;

    if (type === "Doanh thu") {
      html += await renderRevenue();
    } else if (type === "Sản phẩm bán chạy") {
      html += await renderTopProducts();
    } else if (type === "Sản phẩm tồn nhiều") {
      html += await renderStockedProducts();
    }

    result.innerHTML = html;
  });
});

// =================== BÁO CÁO DOANH THU ===================
async function renderRevenue() {
  const data = await fetchApi(`${API_BASE}/get-all-hoadonban`);
  let html = `<table><thead><tr><th>Mã hóa đơn</th><th>Ngày</th><th>Tổng tiền hàng (₫)</th><th>Thuế VAT</th><th>Giảm giá</th></tr></thead><tbody>`;
  let total = 0;

  data.forEach(h => {
    html += `<tr>
      <td>${h.mahdban}</td>
      <td>${new Date(h.ngaylap).toLocaleDateString("vi-VN")}</td>
      <td>${money(h.tongtienhang)}</td>
      <td>${money(h.thuevat)}</td>
      <td>${money(h.giamgia)}</td>
    </tr>`;
    total += h.tongtienhang;
  });

  html += `</tbody><tfoot><tr><td colspan="2">Tổng cộng</td><td>${money(total)}</td><td></td><td></td></tr></tfoot></table>`;
  return html;
}

// =================== TOP 5 SẢN PHẨM BÁN CHẠY ===================
async function renderTopProducts() {
  const data = await fetchApi(`${API_BASE}/get-all-sanpham`);

  // Sắp xếp theo số lượng bán (nếu chưa có trường riêng, có thể thay bằng "soluongton" tạm)
  const sorted = [...data].sort((a, b) => (b.soluongban || 0) - (a.soluongban || 0)).slice(0, 5);

  let html = `<table>
    <thead><tr><th>Mã SP</th><th>Tên sản phẩm</th><th>Giá bán (₫)</th><th>Số lượng bán</th></tr></thead><tbody>`;

  sorted.forEach(sp => {
    html += `<tr>
      <td>${sp.masp}</td>
      <td>${sp.tensp}</td>
      <td>${money(sp.dongia)}</td>
      <td>${sp.soluongban ?? 0}</td>
    </tr>`;
  });

  html += `</tbody></table>`;
  return html;
}

// =================== TOP 5 SẢN PHẨM TỒN NHIỀU ===================
async function renderStockedProducts() {
  const data = await fetchApi(`${API_BASE}/get-all-sanpham`);

  // Sắp xếp theo số lượng tồn giảm dần
  const sorted = [...data].sort((a, b) => (b.soluongton || 0) - (a.soluongton || 0)).slice(0, 5);

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
