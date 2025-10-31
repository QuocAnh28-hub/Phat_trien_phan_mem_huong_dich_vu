document.addEventListener("DOMContentLoaded", () => {
  console.log("✅ JS QuanLyBanHang đã được tải!");

  // ==================== CẤU HÌNH CHUNG ====================
  const API_BASE = "https://localhost:7107/api-thungan/QuanLyBanHang";
  const token = localStorage.getItem("token");
  const headers = token ? { Authorization: `Bearer ${token}` } : {};
  const money = n => new Intl.NumberFormat('vi-VN').format(n || 0);

  // ==================== LẤY ELEMENT ====================
  const maKHInput = document.getElementById("ma-kh");
  const tenKHInput = document.getElementById("tenkh");
  const sdtInput = document.getElementById("sdt");
  const diachiInput = document.getElementById("diachi");
  const maSPInput = document.getElementById("ma-sp");
  const slInput = document.getElementById("sl");
  const tbody = document.querySelector("#bang-ct tbody");
  const btnThemSP = document.getElementById("btn-them-sp");
  const btnLuuHD = document.getElementById("btn-luu-hoa-don");

  // ==================== KIỂM TRA KHÁCH HÀNG ====================
  maKHInput?.addEventListener("blur", async () => {
    const maKH = maKHInput.value.trim();
    if (!maKH) return;

    console.log("🔍 Đang kiểm tra khách hàng:", maKH);
    try {
      const res = await axios.get(`${API_BASE}/get-byid-khachhang?maKH=${maKH}`, { headers });
      console.log("📥 Phản hồi khách hàng:", res.data);

      if (res.data && res.data.success && res.data.data) {
        const kh = res.data.data;
        tenKHInput.value = kh.TenKH || "";
        sdtInput.value = kh.SDT || "";
        diachiInput.value = kh.DiaChi || "";
        console.log("✅ Khách hàng đã tải:", kh);
      } else {
        alert("⚠️ Không tìm thấy khách hàng!");
        tenKHInput.value = "";
        sdtInput.value = "";
        diachiInput.value = "";
      }
    } catch (err) {
      console.error("❌ Lỗi lấy thông tin KH:", err);
      alert("Không thể kết nối API khách hàng!");
    }
  });

  // ==================== THÊM SẢN PHẨM ====================
  btnThemSP?.addEventListener("click", async () => {
    const maSP = maSPInput.value.trim();
    const sl = parseInt(slInput.value) || 1;
    if (!maSP) return alert("Vui lòng nhập mã sản phẩm!");

    console.log("🔍 Đang lấy thông tin sản phẩm:", maSP);
    try {
      const res = await axios.get(`${API_BASE}/get-sanpham-by-id?id=${maSP}`, { headers });
      console.log("📥 Phản hồi sản phẩm:", res.data);

      if (!res.data || !res.data.length) {
        alert("❌ Không tìm thấy sản phẩm!");
        return;
      }

      const sp = res.data[0];
      const gia = Number(sp.dongia);
      const tt = gia * sl;

      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${sp.masp}</td>
        <td>${sp.tensp}</td>
        <td><input type="number" min="1" value="${sl}" class="input sl-input" style="width:60px"></td>
        <td>${money(gia)}</td>
        <td>${money(tt)}</td>
        <td><button class="btn small warn btn-xoa">Xóa</button></td>
      `;
      tbody.appendChild(tr);
      capNhatTongTien();
    } catch (err) {
      console.error("❌ Lỗi lấy sản phẩm:", err);
      alert("Không thể kết nối API sản phẩm!");
    }
  });

  // ==================== XÓA SẢN PHẨM ====================
  tbody?.addEventListener("click", e => {
    if (e.target.classList.contains("btn-xoa")) {
      e.target.closest("tr").remove();
      capNhatTongTien();
    }
  });

  // ==================== TÍNH TỔNG TIỀN ====================
  function capNhatTongTien() {
    const rows = tbody.querySelectorAll("tr");
    let tong = 0;
    rows.forEach(row => {
      const sl = Number(row.querySelector(".sl-input").value);
      const gia = Number(row.cells[3].innerText.replace(/\D/g, ""));
      const tt = sl * gia;
      row.cells[4].innerText = money(tt);
      tong += tt;
    });
    let div = document.getElementById("tong-tien-hd");
    if (!div) {
      div = document.createElement("div");
      div.id = "tong-tien-hd";
      div.style = "text-align:right;margin-top:10px;font-weight:600";
      tbody.parentElement.appendChild(div);
    }
    div.innerHTML = `Tổng tiền hàng: ${money(tong)} ₫`;
    return tong;
  }

  // ==================== LƯU HÓA ĐƠN ====================
  btnLuuHD?.addEventListener("click", async () => {
    const maKH = maKHInput.value.trim();
    const tenKH = tenKHInput.value.trim();
    const sdt = sdtInput.value.trim();
    const diachi = diachiInput.value.trim();
    const maHDBan = "HD" + Date.now().toString().slice(-6);

    if (!maKH || !tenKH) return alert("Vui lòng nhập thông tin khách hàng!");
    const rows = tbody.querySelectorAll("tr");
    if (!rows.length) return alert("Vui lòng thêm ít nhất 1 sản phẩm!");

    const list = [];
    rows.forEach(r => {
      const masp = r.cells[0].innerText.trim();
      const sl = Number(r.querySelector(".sl-input").value);
      const gia = Number(r.cells[3].innerText.replace(/\D/g, ""));
      const tt = sl * gia;
      list.push({ MAHDBAN: maHDBan, MASP: masp, SOLUONG: sl, DONGIA: gia, TONGTIEN: tt });
    });

    const tong = list.reduce((sum, x) => sum + x.TONGTIEN, 0);
    const payload = {
      MAHDBAN: maHDBan,
      MANV: "NV001",
      MAKH: maKH,
      NGAYLAP: new Date().toISOString(),
      TONGTIENHANG: tong,
      THUEVAT: Math.round(tong * 0.1),
      GIAMGIA: 0,
      listjson_chitietban: list
    };

    console.log("📦 GỬI DỮ LIỆU:", payload);
    try {
      const res = await axios.post(`${API_BASE}/insert-hoadonban`, payload, { headers });
      console.log("📥 Phản hồi lưu HĐ:", res.data);
      alert(res.data.message || "Lưu hóa đơn thành công!");
      tbody.innerHTML = "";
      capNhatTongTien();
    } catch (err) {
      console.error("❌ Lỗi khi lưu hóa đơn:", err);
      alert("Không thể lưu hóa đơn!");
    }
  });
});
