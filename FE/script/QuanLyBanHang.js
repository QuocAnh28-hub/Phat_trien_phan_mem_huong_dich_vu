const app = angular.module("dahApp", []);

app.controller("BanHangCtrl", function ($scope, $http) {
  console.log("AngularJS Quản lý bán hàng đã khởi tạo!");

  const API_BASE = "https://localhost:7107/api-thungan/QuanLyBanHang";
  const token = localStorage.getItem("token");
  const headers = token ? { Authorization: `Bearer ${token}` } : {};
  const money = n => new Intl.NumberFormat("vi-VN").format(n || 0);

  // ================== BIẾN KHỞI TẠO ==================
  $scope.khachHang = {};
  $scope.sanPham = {};
  $scope.danhSachCT = [];
  $scope.tongTien = 0;

  // Biến điều khiển popup thanh toán
  $scope.showPaymentModal = false;
  $scope.thanhToan = { phuongThuc: "Tiền mặt" };

  // ================== 1️⃣ KIỂM TRA KHÁCH HÀNG ==================
  $scope.kiemTraKhachHang = function () {
    const maKH = $scope.khachHang.ma?.trim();
    if (!maKH) return;

    console.log("🔍 Đang kiểm tra khách hàng:", maKH);
    $http.get(`${API_BASE}/get-byid-khachhang?maKH=${maKH}`, { headers })
      .then(res => {
        if (res.data && res.data.success && res.data.data) {
          const kh = res.data.data;
          $scope.khachHang.ten = kh.TenKH;
          $scope.khachHang.sdt = kh.SDT;
          $scope.khachHang.diachi = kh.DiaChi;
          $scope.khachHang.moi = false;
          console.log("✅ Đã tải khách hàng:", kh);
        } else {
          alert("⚠️ Khách hàng chưa tồn tại! Sẽ thêm mới khi lưu hóa đơn.");
          $scope.khachHang.ten = "";
          $scope.khachHang.sdt = "";
          $scope.khachHang.diachi = "";
          $scope.khachHang.moi = true;
        }
      })
      .catch(err => {
        if (err.status === 404) {
          alert("⚠️ Khách hàng chưa tồn tại! Sẽ thêm mới khi lưu hóa đơn.");
          $scope.khachHang.ten = "";
          $scope.khachHang.sdt = "";
          $scope.khachHang.diachi = "";
          $scope.khachHang.moi = true;
        } else {
          console.error("❌ Lỗi lấy KH:", err);
          alert("Không thể kết nối API khách hàng!");
        }
      });
  };

  // ================== 2️⃣ THÊM SẢN PHẨM ==================
  $scope.themSanPham = function () {
    const maSP = $scope.sanPham.ma?.trim();
    const sl = parseInt($scope.sanPham.sl) || 1;
    if (!maSP) return alert("Vui lòng nhập mã sản phẩm!");

    console.log("🔍 Đang lấy thông tin sản phẩm:", maSP);
    $http.get(`${API_BASE}/get-sanpham-by-id?id=${maSP}`, { headers })
      .then(res => {
        if (res.data && res.data.length > 0) {
          const sp = res.data[0];
          const gia = Number(sp.dongia);
          const tt = gia * sl;
          $scope.danhSachCT.push({
            masp: sp.masp,
            tensp: sp.tensp,
            soluong: sl,
            dongia: gia,
            tongtien: tt
          });
          $scope.capNhatTongTien();
        } else {
          alert("❌ Không tìm thấy sản phẩm!");
        }
      })
      .catch(err => {
        if (err.status === 404) {
          alert("⚠️ Sản phẩm không tồn tại!");
        } else {
          console.error("❌ Lỗi lấy sản phẩm:", err);
          alert("Không thể kết nối API sản phẩm!");
        }
      });
  };

  // ================== 3️⃣ XÓA SẢN PHẨM ==================
  $scope.xoaSanPham = function (i) {
    $scope.danhSachCT.splice(i, 1);
    $scope.capNhatTongTien();
  };

  // ================== 4️⃣ TÍNH TỔNG TIỀN ==================
  $scope.capNhatTongTien = function () {
    $scope.tongTien = $scope.danhSachCT.reduce((s, x) => s + x.tongtien, 0);
  };

  // ================== 5️⃣ LƯU HÓA ĐƠN ==================
  $scope.luuHoaDon = async function () {
    const maKH = $scope.khachHang.ma?.trim();
    if (!maKH) return alert("Vui lòng nhập mã khách hàng!");
    if ($scope.danhSachCT.length === 0) return alert("Chưa có sản phẩm nào!");

    // Nếu khách hàng mới => thêm mới
    if ($scope.khachHang.moi) {
      const newKH = {
        maKH: maKH,
        tenKH: $scope.khachHang.ten || "Khách hàng mới",
        sdt: $scope.khachHang.sdt || "",
        diaChi: $scope.khachHang.diachi || ""
      };
      try {
        await $http.post(`${API_BASE}/insert-khachhang`, newKH, { headers });
        console.log("🆕 Đã thêm khách hàng mới:", newKH);
        $scope.khachHang.moi = false;
      } catch (err) {
        console.error("❌ Lỗi thêm khách hàng:", err);
        return alert("Không thể thêm khách hàng mới!");
      }
    }

    // Tạo dữ liệu hóa đơn
    const maHDBan = "HD" + Date.now().toString().slice(-6);
    const tong = $scope.tongTien;
    const payload = {
      MAHDBAN: maHDBan,
      MANV: "NV001",
      MAKH: maKH,
      NGAYLAP: new Date().toISOString(),
      TONGTIENHANG: tong,
      THUEVAT: Math.round(tong * 0.1),
      GIAMGIA: 0,
      listjson_chitietban: $scope.danhSachCT.map(x => ({
        MAHDBAN: maHDBan,
        MASP: x.masp,
        SOLUONG: x.soluong,
        DONGIA: x.dongia,
        TONGTIEN: x.tongtien
      }))
    };

    try {
      const res = await $http.post(`${API_BASE}/insert-hoadonban`, payload, { headers });
      console.log("📦 Payload gửi đi:", payload);

      if (res.data && res.data.success) {
        alert("✅ Lưu hóa đơn thành công!");
        $scope.danhSachCT = [];
        $scope.capNhatTongTien();

        console.log("✅ Hóa đơn lưu thành công, mở popup thanh toán!");
        $scope.showPaymentModal = true;

        // 👉 Mở popup chọn phương thức thanh toán
        $scope.thanhToan = {
          maHDBan: maHDBan,
          soTienThanhToan: tong,
          phuongThuc: "Tiền mặt"
        };
        $scope.showPaymentModal = true;
      } else {
        alert("❌ Lưu hóa đơn thất bại!");
      }
    } catch (err) {
      console.error("❌ Lỗi lưu hóa đơn:", err);
      alert("Không thể lưu hóa đơn!");
    }
  };

  // ================== 6️⃣ XỬ LÝ POPUP THANH TOÁN ==================
  $scope.dongModal = function () {
    $scope.showPaymentModal = false;
  };

  $scope.xacNhanThanhToan = async function () {
    const maThanhToan = "TT" + Date.now().toString().slice(-5);
    const pt = $scope.thanhToan.phuongThuc;
    const thanhToan = {
      maThanhToan: maThanhToan,
      maHDBan: $scope.thanhToan.maHDBan,
      phuongThuc: pt,
      soTienThanhToan: pt === "Ghi nợ" ? 0 : $scope.thanhToan.soTienThanhToan,
      ngayThanhToan: new Date().toISOString(),
      trangThai: pt === "Ghi nợ" ? "Chưa thanh toán" : "Đã thanh toán"
    };

    try {
      const resTT = await $http.post(`${API_BASE}/insert-thanhtoan`, thanhToan, { headers });
      console.log("💰 Đã tạo thanh toán:", resTT.data);

      if (resTT.data && resTT.data.success) {
        alert(
          pt === "Ghi nợ"
            ? "🧾 Đã ghi công nợ, khách hàng chưa thanh toán!"
            : "💵 Hóa đơn đã được thanh toán thành công!"
        );
      } else {
        alert("❌ Lỗi khi tạo thanh toán!");
      }
    } catch (err) {
      console.error("❌ Lỗi thêm thanh toán:", err);
      alert("Không thể tạo bản ghi thanh toán!");
    } finally {
      $scope.showPaymentModal = false;
    }
  };
});
