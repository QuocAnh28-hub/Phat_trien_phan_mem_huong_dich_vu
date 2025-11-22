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
        const data = res.data;
        const sp = Array.isArray(data) ? data[0] : (data && data.data) ? data.data : data;
        if (!sp) return alert("❌ Không tìm thấy sản phẩm!");

        const masp = sp.masp || sp.MASP || maSP;
        const gia  = Number(sp.dongia || sp.DONGIA || sp.DonGia || 0);

        // Nếu đã có sản phẩm cùng mã thì chỉ cập nhật số lượng / thành tiền
        const existing = $scope.danhSachCT.find(item => item.masp === masp);
        if (existing) {
          existing.soluong = Number(existing.soluong || 0) + sl;
          existing.dongia = gia; // giữ giá hiện tại hoặc cập nhật theo backend
          existing.tongtien = Number(existing.dongia) * Number(existing.soluong);
        } else {
          $scope.danhSachCT.push({
            masp: masp,
            tensp: sp.tensp || sp.TENSP || sp.TenSP || "",
            soluong: sl,
            dongia: gia,
            tongtien: gia * sl
          });
        }

        // Cập nhật tổng và reset input nếu muốn
        $scope.capNhatTongTien();
        $scope.sanPham.ma = "";
        $scope.sanPham.sl = 1;
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
    // cập nhật tongtien cho từng dòng trước khi tính tổng (nếu user thay đổi soluong)
    $scope.danhSachCT.forEach(sp => {
      sp.tongtien = Number(sp.dongia || 0) * Number(sp.soluong || 0);
    });
    $scope.tongTien = $scope.danhSachCT.reduce((s, x) => s + (Number(x.tongtien) || 0), 0);
  };

  // ================== Helper: gộp các dòng trùng MASP ==================
  function mergeItems(list) {
    const map = {};
    list.forEach(it => {
      const id = it.masp;
      const qty = Number(it.soluong || 0);
      const price = Number(it.dongia || 0);
      if (!map[id]) {
        map[id] = { masp: id, tensp: it.tensp || "", dongia: price, soluong: qty, tongtien: price * qty };
      } else {
        map[id].soluong += qty;
        map[id].tongtien = map[id].dongia * map[id].soluong;
      }
    });
    return Object.values(map);
  }

  // ================== Helper: cập nhật tồn kho cho 1 sản phẩm ==================
  async function updateStockForProduct(masp, soldQty) {
    try {
      const res = await $http.get(`${API_BASE}/get-sanpham-by-id?id=${encodeURIComponent(masp)}`, { headers });
      const data = res.data;
      const sp = Array.isArray(data) ? data[0] : (data && data.data) ? data.data : data;
      const curQty = Number(sp?.soluongton ?? sp?.SOLUONGTON ?? sp?.soLuong ?? sp?.SoLuong ?? 0);
      const newQty = Math.max(0, curQty - Number(soldQty || 0));

      const patchUrl = `${API_BASE}/update-soluong-sanpham?maSP=${encodeURIComponent(masp)}&soLuongMoi=${encodeURIComponent(newQty)}`;
      const patchRes = await $http.patch(patchUrl, null, { headers });
      console.log(`Cập nhật tồn kho cho ${masp}: ${curQty} -> ${newQty}`, patchRes.data);
      return { ok: true, newQty };
    } catch (err) {
      console.error(`Lỗi cập nhật tồn kho cho ${masp}:`, err);
      return { ok: false, error: err };
    }
  }

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

    // Sinh mã hóa đơn (ngắn) để phù hợp DB
    const maHDBan = 'HD' + Math.floor(Math.random() * 90000000 + 10000000);
    const tong = $scope.tongTien || 0;
    
    // Gộp các dòng trùng MASP trước khi gửi
    const mergedList = mergeItems($scope.danhSachCT);

    const payload = {
      MAHDBAN: maHDBan,
      MANV: "NV001",
      MAKH: maKH,
      NGAYLAP: new Date().toISOString(),
      TONGTIENHANG: tong,
      THUEVAT: Math.round(tong * 0.1),
      GIAMGIA: 0,
      listjson_chitietban: mergedList.map(x => ({
        MAHDBAN: maHDBan,
        MASP: x.masp,
        TenSP: x.tensp || "",
        SOLUONG: x.soluong,
        DONGIA: x.dongia,
        TONGTIEN: x.tongtien
      }))
    };

    try {
      console.log("Gửi payload lưu hóa đơn (đã gộp):", payload);
      const res = await $http.post(`${API_BASE}/insert-hoadonban`, payload, { headers });
      console.log("Phản hồi lưu hóa đơn:", res.data);

      if (res.data && res.data.success) {
        alert("✅ Lưu hóa đơn thành công!");

        // Cập nhật tồn kho cho từng sản phẩm (sử dụng mergedList)
        const updateResults = await Promise.all(mergedList.map(it => updateStockForProduct(it.masp, it.soluong)));
        const failed = updateResults.filter(r => !r.ok);
        if (failed.length) {
          console.warn(`${failed.length} sản phẩm cập nhật tồn kho thất bại. Kiểm tra console.`);
          alert(`Lưu hóa đơn thành công nhưng có ${failed.length} sản phẩm cập nhật tồn kho thất bại.`);
        } else {
          console.log("✅ Tất cả sản phẩm đã cập nhật tồn kho thành công.");
        }

        // Xóa danh sách sau khi cập nhật tồn kho
        $scope.danhSachCT = [];
        $scope.capNhatTongTien();

        // chuẩn bị dữ liệu thanh toán rồi mở modal
        const paymentPayload = {
          maHDBan: maHDBan,
          soTienThanhToan: tong,
          phuongThuc: "Tiền mặt"
        };

        // set maHDBan cho thanhToan và mở modal trong $applyAsync
        $scope.$applyAsync(() => {
          $scope.thanhToan = paymentPayload;
          $scope.thanhToan.maHDBan = maHDBan;
          $scope.showPaymentModal = true;
        });
      } else {
        console.error("Lưu hóa đơn thất bại, phản hồi API:", res.data);
        const msg = (res.data && (res.data.message || JSON.stringify(res.data))) || "Lưu hóa đơn thất bại!";
        alert(`❌ ${msg}`);
      }
    } catch (err) {
      console.error("❌ Lỗi lưu hóa đơn:", err);
      // Nếu server báo lỗi duplicate PK, gợi ý nguyên nhân (dòng trùng)
      const text = err?.data?.message || err?.statusText || err?.message || JSON.stringify(err);
      if (String(text).toLowerCase().includes("primary") || String(text).toLowerCase().includes("duplicate")) {
        alert("Lỗi khi thêm hóa đơn: có thể do nhiều dòng cùng mã sản phẩm (MASP) gây trùng khoá. Hệ thống đã cố gắng gộp các dòng trước khi gửi; nếu vẫn lỗi, kiểm tra server.");
      } else {
        alert(`Không thể lưu hóa đơn! Chi tiết: ${text}`);
      }
    }
  };
});