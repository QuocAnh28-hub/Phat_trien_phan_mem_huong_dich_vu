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
  $scope.tongVAT = 0;
  $scope.hoaDonDaThanhToan = null;

  // Popup thanh toán
  $scope.showPaymentModal = false;
  $scope.thanhToan = { maHDBan: null, soTienThanhToan: 0, phuongThuc: "Tiền mặt" };


  // ================== 1️⃣ KIỂM TRA KHÁCH HÀNG ==================
  $scope.kiemTraKhachHang = function () {
    const maKH = ($scope.khachHang.ma || "").trim();
    if (!maKH) return;

    $http.get(`${API_BASE}/get-byid-khachhang?maKH=${encodeURIComponent(maKH)}`, { headers })
      .then(res => {
        if (res.data && res.data.success && res.data.data) {
          const kh = res.data.data;
          $scope.khachHang.ma = kh.MaKH;
          $scope.khachHang.ten = kh.TenKH;
          $scope.khachHang.sdt = kh.SDT;
          $scope.khachHang.diachi = kh.DiaChi;
          $scope.khachHang.moi = false;
        } else {
          alert("⚠️ Khách hàng chưa tồn tại! Sẽ thêm mới khi lưu hóa đơn.");
          $scope.khachHang.moi = true;
          $scope.khachHang.ten = "";
          $scope.khachHang.sdt = "";
          $scope.khachHang.diachi = "";
        }
      })
      .catch(() => {
        alert("Không thể kết nối API khách hàng!");
      });
  };


  // ================== 2️⃣ THÊM SẢN PHẨM ==================
  $scope.themSanPham = function () {
    const maSP = ($scope.sanPham.ma || "").trim();
    const sl = parseInt($scope.sanPham.sl) || 1;
    if (!maSP) return alert("Vui lòng nhập mã sản phẩm!");
    if (sl <= 0) return alert("Số lượng phải lớn hơn 0!");

    $http.get(`${API_BASE}/get-sanpham-by-id?id=${encodeURIComponent(maSP)}`, { headers })
      .then(res => {
        const sp = Array.isArray(res.data) ? res.data[0] : res.data.data;

        if (!sp) return alert("❌ Không tìm thấy sản phẩm!");

        const masp = sp.masp;
        const gia  = Number(sp.dongia);
        const vat  = Number(sp.thue || 0);

        const existing = $scope.danhSachCT.find(item => item.masp === masp);
        if (existing) {
          existing.soluong += sl;
          existing.tongtien = existing.dongia * existing.soluong;
        } else {
          $scope.danhSachCT.push({
            masp: masp,
            tensp: sp.tensp,
            soluong: sl,
            dongia: gia,
            vat: vat,
            tongtien: gia * sl
          });
        }

        $scope.capNhatTongTien();
        $scope.sanPham.ma = "";
        $scope.sanPham.sl = 1;
      })
      .catch(() => {
        alert("Không thể kết nối API sản phẩm!");
      });
  };


  // ================== 3️⃣ XÓA SẢN PHẨM ==================
  $scope.xoaSanPham = function (i) {
    $scope.danhSachCT.splice(i, 1);
    $scope.capNhatTongTien();
  };


  // ================== 4️⃣ TÍNH TỔNG TIỀN + VAT ==================
  $scope.capNhatTongTien = function () {
    $scope.danhSachCT.forEach(sp => {
      sp.tongtien = sp.dongia * sp.soluong;
    });

    $scope.tongTien = $scope.danhSachCT.reduce((s, x) => s + x.tongtien, 0);

    // VAT theo DB (giá trị tuyệt đối / sản phẩm)
    $scope.tongVAT = $scope.danhSachCT.reduce((s, x) => s + (x.vat * x.soluong), 0);
  };


  // ================== Helper: Gộp các dòng trùng MASP ==================
  function mergeItems(list) {
    const map = {};
    list.forEach(it => {
      if (!map[it.masp]) {
        map[it.masp] = angular.copy(it);
      } else {
        map[it.masp].soluong += it.soluong;
        map[it.masp].tongtien = map[it.masp].soluong * map[it.masp].dongia;
      }
    });
    return Object.values(map);
  }


  // ================== Helper: cập nhật tồn kho ==================
  async function updateStockForProduct(masp, soldQty) {
    try {
      const res = await $http.get(`${API_BASE}/get-sanpham-by-id?id=${masp}`, { headers });
      const sp = Array.isArray(res.data) ? res.data[0] : res.data.data;

      const curQty = Number(sp.soluongton || 0);
      const newQty = Math.max(0, curQty - soldQty);

      await $http.patch(
        `${API_BASE}/update-soluong-sanpham?maSP=${masp}&soLuongMoi=${newQty}`,
        null,
        { headers }
      );

      return { ok: true };
    } catch (err) {
      return { ok: false };
    }
  }


  // ================== 5️⃣ LƯU HÓA ĐƠN ==================
  $scope.luuHoaDon = async function () {
    const maKH = ($scope.khachHang.ma || "").trim();
    if (!maKH) return alert("Vui lòng nhập mã khách hàng!");
    if ($scope.danhSachCT.length === 0) return alert("Chưa có sản phẩm nào!");

    if ($scope.khachHang.moi) {
      await $http.post(`${API_BASE}/insert-khachhang`, {
        MaKH: maKH,
        TenKH: $scope.khachHang.ten,
        SDT: $scope.khachHang.sdt,
        DiaChi: $scope.khachHang.diachi
      }, { headers });
      $scope.khachHang.moi = false;
    }

    const maHDBan = 'HD' + Math.floor(Math.random() * 90000000 + 10000000);
<<<<<<< HEAD
    const tong = $scope.tongTien || 0;
    
    // Gộp các dòng trùng MASP trước khi gửi
=======

>>>>>>> 30f2a18e557c76f84c093d72833ac7e48ef19319
    const mergedList = mergeItems($scope.danhSachCT);

    const payload = {
      MAHDBAN: maHDBan,
      MANV: "NV001",
      MAKH: maKH,
      NGAYLAP: new Date().toISOString(),
      TONGTIENHANG: $scope.tongTien,
      THUEVAT: $scope.tongVAT,
      GIAMGIA: 0,
      listjson_chitietban: mergedList.map(x => ({
        MAHDBAN: maHDBan,
        MASP: x.masp,
        TenSP: x.tensp,
        SOLUONG: x.soluong,
        DONGIA: x.dongia,
        TONGTIEN: x.tongtien,
        THUE: x.vat
      }))
    };

    const res = await $http.post(`${API_BASE}/insert-hoadonban`, payload, { headers });
    const tongThanhToan = res.data.data?.TongThanhToan 
                   ?? ($scope.tongTien + $scope.tongVAT);

    if (res.data.success) {
      alert("Lưu hóa đơn thành công!");

      await Promise.all(mergedList.map(it => updateStockForProduct(it.masp, it.soluong)));

      // LƯU HÓA ĐƠN ĐỂ HIỂN THỊ
      $scope.hoaDonDaThanhToan = {
        maHDBan: maHDBan,
        khachhang: angular.copy($scope.khachHang),
        sanpham: angular.copy(mergedList),
        VAT: $scope.tongVAT,
        TongTien: $scope.tongTien
      };

      // 👉 LƯU TỔNG TIỀN TRƯỚC KHI RESET
        const tongThanhToan = $scope.tongTien + $scope.tongVAT;

        // Reset danh sách sau khi tính xong
        $scope.danhSachCT = [];
        $scope.capNhatTongTien();

        $scope.$applyAsync(() => {
          $scope.thanhToan = {
            maHDBan: maHDBan,
            soTienThanhToan: tongThanhToan,
            phuongThuc: "Tiền mặt"
          };
          $scope.showPaymentModal = true;
        });

    }
  };


  // ================== 6️⃣ XÁC NHẬN THANH TOÁN ==================
  $scope.xacNhanThanhToan = async function () {
    $scope.showPaymentModal = false;
    try {
      const payload = {
        MaThanhToan: "TT" + Date.now(),
        MaHDBan: $scope.thanhToan.maHDBan,
        PhuongThuc: $scope.thanhToan.phuongThuc,
        SoTienThanhToan: $scope.thanhToan.soTienThanhToan,
        NgayThanhToan: new Date().toISOString(),
        TrangThai: "Đã thanh toán"
      };

      const res = await $http.post(`${API_BASE}/insert-thanhtoan`, payload, { headers });

      if (res.data.success) {

        // CHUYỂN VIEW -> HIỂN THỊ HÓA ĐƠN
        document.querySelector("#tao-hoa-don").classList.remove("is-active");
        document.querySelector("#chi-tiet").classList.add("is-active");

        alert("Thanh toán thành công!");
      }

    } finally {
      $scope.showPaymentModal = false;
    }
  };

});
