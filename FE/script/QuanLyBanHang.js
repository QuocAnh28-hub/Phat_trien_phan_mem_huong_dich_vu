// =================== DỮ LIỆU MÔ PHỎNG ===================
const db = {
  KhachHang: [
    { id: "KH001", ten: "Nguyễn Văn An", sdt: "0912345678" },
    { id: "KH002", ten: "Trần Thị B", sdt: "0987654321" },
    { id: "KH003", ten: "Phạm Văn C", sdt: "0909090909" }
  ],
  SanPham: [
    { ma: "SP001", ten: "Bánh quy", gia: 25000 },
    { ma: "SP002", ten: "Sữa Vinamilk 1L", gia: 35000 },
    { ma: "SP003", ten: "Nước ngọt Coca 330ml", gia: 12000 },
    { ma: "SP004", ten: "Snack Oishi", gia: 10000 },
    { ma: "SP005", ten: "Trà xanh 0 độ", gia: 15000 }
  ],
  HoaDonBan: [
    { id: "HD0001", khach: "Nguyễn Văn An", ngay: "2025-10-10", tong: 2350000, trangThai: "Đã thanh toán" },
    { id: "HD0002", khach: "Trần Thị B", ngay: "2025-10-12", tong: 1500000, trangThai: "Nháp" },
    { id: "HD0003", khach: "Phạm Văn C", ngay: "2025-10-14", tong: 870000, trangThai: "Đã hủy" },
    { id: "HD0004", khach: "Nguyễn Văn An", ngay: "2025-10-16", tong: 3200000, trangThai: "Đã thanh toán" }
  ]
};

// =================== HÀM TIỆN ÍCH ===================
const money = n => new Intl.NumberFormat("vi-VN").format(n || 0) + " ₫";

// =================== HIỂN THỊ DANH SÁCH HÓA ĐƠN ===================
function renderHoaDon() {
  const tbody = document.querySelector("#hoa-don table tbody");
  if (!tbody) return;
  tbody.innerHTML = "";

  db.HoaDonBan.forEach(h => {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td>${h.id}</td>
      <td>${h.khach}</td>
      <td>${h.ngay}</td>
      <td>${money(h.tong)}</td>
      <td><span class="pill">${h.trangThai}</span></td>
      <td class="table-actions">
        <a class="btn small neutral" href="#chi-tiet" data-id="${h.id}">Xem</a>
        <a class="btn small warn" href="#" data-huy="${h.id}">Hủy</a>
      </td>
    `;
    tbody.appendChild(tr);
  });

  // Gán sự kiện nút "Xem" & "Hủy"
  document.querySelectorAll('[data-id]').forEach(btn => {
    btn.addEventListener("click", e => {
      const id = e.target.getAttribute("data-id");
      renderChiTiet(id);
    });
  });

  document.querySelectorAll('[data-huy]').forEach(btn => {
    btn.addEventListener("click", e => {
      const id = e.target.getAttribute("data-huy");
      huyHoaDon(id);
    });
  });
}

// =================== HIỂN THỊ CHI TIẾT HÓA ĐƠN ===================
function renderChiTiet(maHD) {
  const hd = db.HoaDonBan.find(h => h.id === maHD);
  const section = document.querySelector("#chi-tiet");
  if (!hd || !section) return;

  // Giả lập chi tiết sản phẩm
  const chiTiet = [
    { ma: "SP001", ten: "Bánh quy", sl: 2, gia: 25000, ck: 0 },
    { ma: "SP003", ten: "Coca 330ml", sl: 5, gia: 12000, ck: 0 },
    { ma: "SP005", ten: "Trà xanh 0 độ", sl: 3, gia: 15000, ck: 0 }
  ];

  const tbody = section.querySelector("tbody");
  tbody.innerHTML = "";
  chiTiet.forEach(sp => {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td>${sp.ma}</td>
      <td>${sp.ten}</td>
      <td>${sp.sl}</td>
      <td>${money(sp.gia)}</td>
      <td>${sp.ck}</td>
      <td>${money(sp.sl * sp.gia - sp.ck)}</td>
    `;
    tbody.appendChild(tr);
  });

  // Cập nhật thông tin chi tiết
  section.querySelector("p:nth-of-type(1)").innerHTML = `<strong>Mã HĐ:</strong> ${hd.id}`;
  section.querySelector("p:nth-of-type(2)").innerHTML = `<strong>Khách hàng:</strong> ${hd.khach}`;
  section.querySelector("p:nth-of-type(3)").innerHTML = `<strong>Ngày lập:</strong> ${hd.ngay}`;

  // Chuyển view sang phần chi tiết
  document.querySelectorAll(".view").forEach(v => v.classList.remove("is-active"));
  section.classList.add("is-active");
}

// =================== HỦY HÓA ĐƠN ===================
function huyHoaDon(maHD) {
  if (!confirm("Bạn có chắc muốn hủy hóa đơn " + maHD + " không?")) return;
  const hd = db.HoaDonBan.find(h => h.id === maHD);
  if (!hd) return;
  hd.trangThai = "Đã hủy";
  alert("Hóa đơn " + maHD + " đã được hủy!");
  renderHoaDon();
}

// =================== KHỞI TẠO SAU KHI LOAD ===================
document.addEventListener("DOMContentLoaded", () => {
  renderHoaDon();
});
