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
  $scope.thanhToan = { maHDBan: null, soTienThanhToan: 0, phuongThuc: "Tiền mặt" };

  $scope.dongModal = function () {
    $scope.showPaymentModal = false;
  };

  $scope.xacNhanThanhToan = async function () {
    try {
      const maThanhToan = `TT${Date.now()}${Math.floor(Math.random() * 9000 + 1000)}`;
      const pt = $scope.thanhToan.phuongThuc || "Tiền mặt";
      const soTien = $scope.thanhToan.soTienThanhToan || 0;
      const thanhToanPayload = {
        MaThanhToan: maThanhToan,
        MaHDBan: $scope.thanhToan.maHDBan,
        PhuongThuc: pt,
        SoTienThanhToan: pt === "Ghi nợ" ? 0 : soTien,
        NgayThanhToan: new Date().toISOString(),
        TrangThai: pt === "Ghi nợ" ? "Chưa thanh toán" : "Đã thanh toán"
      };

      console.log("Gửi thanh toán:", thanhToanPayload);
      const resTT = await $http.post(`${API_BASE}/insert-thanhtoan`, thanhToanPayload, { headers });
      console.log("Phản hồi thanh toán:", resTT.data);

      if (resTT.data && resTT.data.success) {
        alert(pt === "Ghi nợ"
          ? "🧾 Đã ghi công nợ, khách hàng chưa thanh toán!"
          : "💵 Hóa đơn đã được thanh toán thành công!");
      } else {
        console.error("Lưu thanh toán không thành công:", resTT.data);
        alert("❌ Lỗi khi tạo thanh toán!");
      }
    } catch (err) {
      console.error("❌ Lỗi thêm thanh toán:", err);
      const serverMsg = err?.data?.message || err?.statusText || err?.message || JSON.stringify(err);
      alert(`Không thể tạo bản ghi thanh toán! Chi tiết: ${serverMsg}`);
    } finally {
      $scope.showPaymentModal = false;
      // ensure digest
      if (!$scope.$$phase) $scope.$applyAsync();
    }
  };

  // ================== 1️⃣ KIỂM TRA KHÁCH HÀNG ==================
  $scope.kiemTraKhachHang = function () {
    const maKH = ($scope.khachHang.ma || "").trim();
    if (!maKH) return;

    console.log("🔍 Đang kiểm tra khách hàng:", maKH);
    $http.get(`${API_BASE}/get-byid-khachhang?maKH=${encodeURIComponent(maKH)}`, { headers })
      .then(res => {
        if (res.data && res.data.success && res.data.data) {
          const kh = res.data.data;
          $scope.khachHang.ma = kh.MaKH || kh.maKH || maKH;
          $scope.khachHang.ten = kh.TenKH || kh.tenKH || "";
          $scope.khachHang.sdt = kh.SDT || kh.sdt || "";
          $scope.khachHang.diachi = kh.DiaChi || kh.diaChi || "";
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
        if (err && err.status === 404) {
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
    const maSP = ($scope.sanPham.ma || "").trim();
    const sl = parseInt($scope.sanPham.sl) || 1;
    if (!maSP) return alert("Vui lòng nhập mã sản phẩm!");
    if (sl <= 0) return alert("Số lượng phải lớn hơn 0!");

    console.log("🔍 Đang lấy thông tin sản phẩm:", maSP);
    $http.get(`${API_BASE}/get-sanpham-by-id?id=${encodeURIComponent(maSP)}`, { headers })
      .then(res => {
        // backend may return array or object
        const data = res.data;
        const sp = Array.isArray(data) ? data[0] : (data && data.data) ? data.data : data;
        if (sp) {
          const gia = Number(sp.dongia || sp.DONGIA || sp.DonGia || 0);
          const tt = gia * sl;
          $scope.danhSachCT.push({
            masp: sp.masp || sp.MASP || maSP,
            tensp: sp.tensp || sp.TENSP || sp.TenSP || "",
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
        if (err && err.status === 404) {
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
    $scope.tongTien = $scope.danhSachCT.reduce((s, x) => s + (Number(x.tongtien) || 0), 0);
  };

  // ================== 5️⃣ LƯU HÓA ĐƠN ==================
  $scope.luuHoaDon = async function () {
    const maKH = ($scope.khachHang.ma || "").trim();
    if (!maKH) return alert("Vui lòng nhập mã khách hàng!");
    if ($scope.danhSachCT.length === 0) return alert("Chưa có sản phẩm nào!");

    // Nếu khách hàng mới => thêm mới
    if ($scope.khachHang.moi) {
      const newKH = {
        MaKH: maKH,
        TenKH: $scope.khachHang.ten || "Khách hàng mới",
        SDT: $scope.khachHang.sdt || "",
        DiaChi: $scope.khachHang.diachi || ""
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

    // Sinh mã dạng HD + 8 chữ số ngẫu nhiên (ví dụ: HD12345678)
    const maHDBan = 'HD' + Math.floor(Math.random() * 90000000 + 10000000);

    const tong = $scope.tongTien || 0;
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
      console.log("Gửi payload lưu hóa đơn:", payload);
      const res = await $http.post(`${API_BASE}/insert-hoadonban`, payload, { headers });
      console.log("Phản hồi lưu hóa đơn:", res.data);

      if (res.data && res.data.success) {
        alert("✅ Lưu hóa đơn thành công!");
        $scope.danhSachCT = [];
        $scope.capNhatTongTien();

        // chuẩn bị dữ liệu thanh toán rồi mở modal
        const paymentPayload = {
          maHDBan: maHDBan,
          soTienThanhToan: tong,
          phuongThuc: "Tiền mặt"
        };

        $scope.$applyAsync(() => {
          $scope.thanhToan = paymentPayload;
          $scope.showPaymentModal = true;
        });
      } else {
        console.error("Lưu hóa đơn thất bại, phản hồi API:", res.data);
        const msg = (res.data && (res.data.message || JSON.stringify(res.data))) || "Lưu hóa đơn thất bại!";
        alert(`❌ ${msg}`);
      }
    } catch (err) {
      console.error("❌ Lỗi lưu hóa đơn:", err);
      const serverMsg = err?.data?.message || err?.statusText || err?.message || JSON.stringify(err);
      alert(`Không thể lưu hóa đơn! Chi tiết: ${serverMsg}`);
    }
  };
});