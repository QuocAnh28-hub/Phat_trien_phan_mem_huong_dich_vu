const db = {
  HoaDonBan: [
    { id:'HD001', ngay:'2025-10-01', tong:1500000, paid:1500000 },
    { id:'HD002', ngay:'2025-10-03', tong:800000, paid:800000 },
    { id:'HD003', ngay:'2025-10-10', tong:1200000, paid:900000 },
    { id:'HD004', ngay:'2025-10-12', tong:2000000, paid:2000000 },
    { id:'HD005', ngay:'2025-10-15', tong:500000, paid:500000 }
  ],
  SanPham: [
    { ma:'SP01', ten:'Nước ngọt Coca', soLuongBan:120, tonKho:40 },
    { ma:'SP02', ten:'Bánh Oreo', soLuongBan:95, tonKho:25 },
    { ma:'SP03', ten:'Snack Oishi', soLuongBan:150, tonKho:15 },
    { ma:'SP04', ten:'Sữa Vinamilk', soLuongBan:80, tonKho:60 },
    { ma:'SP05', ten:'Trà Xanh 0 độ', soLuongBan:100, tonKho:50 },
    { ma:'SP06', ten:'Mì Hảo Hảo', soLuongBan:70, tonKho:200 },
    { ma:'SP07', ten:'Kẹo Alpenliebe', soLuongBan:60, tonKho:180 }
  ]
};

const money = n => new Intl.NumberFormat('vi-VN').format(n || 0);

// =================== LẮNG NGHE SỰ KIỆN FORM ===================
document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById('report-form');
  const result = document.getElementById('report-results');

  if (!form || !result) return;

  form.addEventListener('submit', e => {
    e.preventDefault();

    const type = form.type.value;
    const from = form.from.value;
    const to = form.to.value;

    let html = `<p><strong>Thời gian thống kê:</strong> ${from || '---'} → ${to || '---'}</p>`;

    if (type === 'Doanh thu') {
      html += renderRevenue();
    } else if (type === 'Sản phẩm bán chạy') {
      html += renderTopProducts();
    } else if (type === 'Sản phẩm tồn nhiều') {
      html += renderStockedProducts();
    }

    result.innerHTML = html;
  });
});

// =================== HÀM XỬ LÝ BÁO CÁO ===================
function renderRevenue() {
  let html = `<table><thead><tr><th>Mã hóa đơn</th><th>Ngày</th><th>Số tiền thanh toán (₫)</th></tr></thead><tbody>`;
  let total = 0;
  db.HoaDonBan.forEach(h => {
    html += `<tr><td>${h.id}</td><td>${h.ngay}</td><td>${money(h.paid)}</td></tr>`;
    total += h.paid;
  });
  html += `</tbody><tfoot><tr><td colspan="2">Tổng cộng</td><td>${money(total)}</td></tr></tfoot></table>`;
  return html;
}

function renderTopProducts() {
  const sorted = [...db.SanPham].sort((a,b)=>b.soLuongBan - a.soLuongBan).slice(0,5);
  let html = `<table><thead><tr><th>Mã SP</th><th>Tên sản phẩm</th><th>Số lượng bán</th></tr></thead><tbody>`;
  sorted.forEach(sp => {
    html += `<tr><td>${sp.ma}</td><td>${sp.ten}</td><td>${sp.soLuongBan}</td></tr>`;
  });
  html += `</tbody></table>`;
  return html;
}

function renderStockedProducts() {
  const sorted = [...db.SanPham].sort((a,b)=>b.tonKho - a.tonKho).slice(0,5);
  let html = `<table><thead><tr><th>Mã SP</th><th>Tên sản phẩm</th><th>Tồn kho</th><th>Lượt bán</th></tr></thead><tbody>`;
  sorted.forEach(sp => {
    html += `<tr><td>${sp.ma}</td><td>${sp.ten}</td><td>${sp.tonKho}</td><td>${sp.soLuongBan}</td></tr>`;
  });
  html += `</tbody></table>`;
  return html;
}