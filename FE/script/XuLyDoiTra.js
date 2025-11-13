(function () {
  'use strict';

  /* ================== MODULE & CONFIG ================== */
  angular.module('DAHApp', [])
    .constant('API_BASE', 'https://localhost:7107/api-thungan/QuanLyDoiTra')
    .factory('AuthInterceptor', function () {
      return {
        request: function (cfg) {
          var t = localStorage.getItem('token');
          if (t) cfg.headers.Authorization = 'Bearer ' + t;
          return cfg;
        }
      };
    })
    .config(function ($httpProvider) {
      $httpProvider.interceptors.push('AuthInterceptor');
    })

    /* ================== SERVICE: Gateway ================== */
    .factory('Gateway', function ($http, API_BASE) {

      // Thử GET với nhiều bộ params cho 1 path
      function tryParams(path, paramsList) {
        var url = API_BASE + path;
        return new Promise(function (resolve, reject) {
          var i = 0, lastErr = null;

          (function next() {
            if (i >= paramsList.length) {
              return reject(lastErr || new Error('Không tìm thấy'));
            }
            $http.get(url, { params: paramsList[i++] })
              .then(function (res) { resolve(res); })
              .catch(function (e) {
                lastErr = e;
                var code = e.status;
                // các mã 4xx do sai param thì thử tiếp biến thể khác
                if ([400, 404, 415, 422].indexOf(code) === -1) {
                  return reject(e);
                }
                next();
              });
          })();
        });
      }

      function unwrapList(res) {
        var d = res && res.data;
        // { success: true, data: [...] }
        if (d && Array.isArray(d.data)) return d.data;
        if (Array.isArray(d)) return d;
        if (d && Array.isArray(d.items)) return d.items;
        return [];
      }

      // ===== HÓA ĐƠN =====
      var getInvoiceById = function (id) {
        return tryParams('/get-hoadonban-by-id', [
          { maHoaDon: id }, { id: id }, { maHDB: id }, { ma: id }
        ]);
      };

      // ===== CHI TIẾT BÁN =====
      var getDetailsByInvoice = function (invoiceId) {
        return tryParams('/get-chitietban-by-IDhoadon', [
          { maHDB: invoiceId }, { id: invoiceId }, { maHoaDon: invoiceId }, { ma: invoiceId }
        ]);
      };

      // Xoá chi tiết: backend cho 2 kiểu: theo id CT hoặc theo cặp {maHDB, maSP}
      var deleteChiTietBan = function (params) {
        return $http.delete(API_BASE + '/delete-chitietban', { params: params });
      };

      // ===== THANH TOÁN =====
      var getPaymentsByInvoice = function (invoiceId) {
        return $http.get(API_BASE + '/get-thanhtoan-by-mahdban', {
          params: { maHDBan: invoiceId }
        });
      };

      // 🔹 RESET SỐ TIỀN THANH TOÁN THEO MÃ HÓA ĐƠN
      // Gọi store SP_RESET_SOTIENTHANHTOAN_BY_MAHDBAN qua API:
      // POST api-thungan/QuanLyDoiTra/reset-sotienthanhtoan-by-mahdban?maHDBan=HD001&soTienMoi=0
      var resetPaymentsByInvoice = function (invoiceId, amount) {
        return $http.post(API_BASE + '/reset-sotienthanhtoan-by-mahdban', null, {
          params: {
            maHDBan: invoiceId,
            soTienMoi: amount || 0
          }
        });
      };

      // (Hàm update từng thanh toán cũ, giờ không còn dùng nữa nhưng giữ lại nếu sau này cần)
      var updatePaymentAmountToZero = function (payment) {
        var body = {
          mathanhtoan: payment.id,
          mahdBan: payment.maHoaDon,
          phuongThuc: payment.phuongThuc,
          soTienThanhToan: 0,
          ngaythanhtoan: payment.ngayThanhToan,
          trangThai: payment.trangThai
        };
        return $http.post(API_BASE + '/update-thanhtoan', body);
      };

      // Xoá toàn bộ chi tiết bán của 1 hóa đơn
      async function deleteAllDetailsOfInvoice(invoiceId) {
        var res = await getDetailsByInvoice(invoiceId);
        var list = unwrapList(res) || [];
        if (!list.length) return 0;

        var ok = 0;
        for (var i = 0; i < list.length; i++) {
          var d = list[i];
          var idCT = d.id || d.maCT || d.maChiTiet;
          var maSP = d.masp || d.maSP || d.productId;
          try {
            if (idCT) {
              await deleteChiTietBan({ id: idCT });
            } else {
              await deleteChiTietBan({ maHDB: invoiceId, maSP: maSP });
            }
            ok++;
          } catch (e) {
            console.warn('Xoá chi tiết lỗi:', e, d);
          }
        }
        return ok;
      }

      return {
        unwrapList: unwrapList,
        getInvoiceById: getInvoiceById,
        getDetailsByInvoice: getDetailsByInvoice,
        deleteChiTietBan: deleteChiTietBan,
        deleteAllDetailsOfInvoice: deleteAllDetailsOfInvoice,
        getPaymentsByInvoice: getPaymentsByInvoice,
        updatePaymentAmountToZero: updatePaymentAmountToZero, // optional
        resetPaymentsByInvoice: resetPaymentsByInvoice        // 👈 dùng cái này cho đổi trả
      };
    })

    /* ================== CONTROLLER ================== */
    .controller('XuLyDoiTraCtrl', function ($scope, Gateway) {
      var vm = this;

      /* ----- USER ----- */
      (function setUser() {
        var u = {};
        try { u = JSON.parse(localStorage.getItem('user') || '{}'); } catch (e) { }
        vm.currentUser = u.userName || u.username || localStorage.getItem('username') || 'thungan';
      })();

      /* ----- STATE ----- */
      vm.loading = false;
      vm.search = { maHD: '', from: null, to: null };

      vm.invoice = null;
      vm.details = [];
      vm.retQty = {};   // { productId: number }
      vm.retReason = {};
      vm.exchangeItems = [];
      vm.newEx = { code: '', qty: null, price: null };

      vm.sumReturn = 0;
      vm.sumExchange = 0;
      vm.sumDiff = 0;

      vm.payments = [];

      // MODAL state
      vm.modal = {
        editInvoice: { show: false, data: {} },
        editDetail: { show: false, data: {} },
        editPayment: { show: false, isNew: true, data: {} }
      };

      vm.closeModals = function () {
        vm.modal.editInvoice.show = false;
        vm.modal.editDetail.show = false;
        vm.modal.editPayment.show = false;
      };

      /* ----- HELPERS ----- */
      function pickInvoiceId(x) {
        return (x && (x.mahdban || x.mahdBan || x.maHoaDon || x.mahoadon || x.maHD || x.ma || x.id)) || null;
      }

      function normalizeInvoice(x) {
        if (!x) return null;
        var id = pickInvoiceId(x);
        return {
          id: id,
          customerId: x.makh || x.maKH || '',
          createdAt: x.ngaylap || x.ngayLap || null,
          goodsTotal: Number(x.tongtienhang || x.tongTienHang || 0)
        };
      }

      function normalizeDetail(d) {
        return {
          invoiceId: d.mahdban || d.maHoaDon || d.maHDB || d.id || '',
          productId: d.masp || d.maSP || d.code || '',
          productName: d.tensp || d.tenSP || d.tenSanPham || d.productName || d.name || d.ten || d['ten_sp'] || '',
          qty: Number(d.soluong || d.soLuong || 0),
          price: Number(d.dongia || d.donGia || 0),
          lineTotal: Number(
            d.tongtien || d.tongTien || d.thanhtien || d.thanhTien ||
            (Number(d.soluong || 0) * Number(d.dongia || 0))
          )
        };
      }

      // LOAD THANH TOÁN
      function loadPayments(invoiceId) {
        vm.payments = [];
        Gateway.getPaymentsByInvoice(invoiceId)
          .then(function (res) {
            var list = Gateway.unwrapList(res);
            vm.payments = list.map(function (p, idx) {
              return {
                id: p.mathanhtoan || p.maThanhToan || ('TMP_' + idx),
                maHoaDon: p.mahdBan || p.maHDBan,
                soTien: Number(p.soTienThanhToan || p.SoTienThanhToan || 0),
                phuongThuc: p.phuongThuc || p.PhuongThuc || '',
                ngayThanhToan: p.ngaythanhtoan || p.NgayThanhToan,
                trangThai: p.trangThai || p.TrangThai || ''
              };
            });
          })
          .catch(function (err) {
            console.error('Load payments error:', err);
            vm.payments = [];
          });
      }

      function resetReturnExchangeState() {
        vm.retQty = {};
        vm.retReason = {};
        vm.exchangeItems = [];
        vm.newEx = { code: '', qty: null, price: null };
        vm.sumReturn = 0;
        vm.sumExchange = 0;
        vm.sumDiff = 0;
      }

      function recalc() {
        var priceMap = {};
        for (var i = 0; i < vm.details.length; i++) {
          var it = vm.details[i];
          priceMap[it.productId] = it.price;
        }

        vm.sumReturn = 0;
        var keys = Object.keys(vm.retQty);
        for (var j = 0; j < keys.length; j++) {
          var sp = keys[j];
          var q = Number(vm.retQty[sp] || 0);
          var price = priceMap[sp] || 0;
          vm.sumReturn += q * price;
        }

        vm.sumExchange = 0;
        for (var k = 0; k < vm.exchangeItems.length; k++) {
          var x = vm.exchangeItems[k];
          vm.sumExchange += (x.qty || 0) * (x.price || 0);
        }

        vm.sumDiff = vm.sumExchange - vm.sumReturn;
      }

      /* ----- UI ACTIONS: CHI TIẾT ĐỔI/TRẢ ----- */
      vm.onQtyChange = function (productId, max) {
        var v = Math.max(0, Math.min(Number(vm.retQty[productId] || 0), Number(max)));
        vm.retQty[productId] = v;
        recalc();
      };

      vm.toggleReturn = function (productId, max) {
        var cur = Number(vm.retQty[productId] || 0);
        var next = cur > 0 ? 0 : 1;
        vm.retQty[productId] = Math.min(next, Number(max) || 0);
        recalc();
      };

      vm.removeExchange = function (i) {
        vm.exchangeItems.splice(i, 1);
        recalc();
      };

      vm.resetAll = function () {
        vm.invoice = null;
        vm.details = [];
        resetReturnExchangeState();
        vm.payments = [];
      };

      /* ----- LOAD HÓA ĐƠN ----- */
      vm.fetchInvoice = async function (e) {
        if (e && e.preventDefault) e.preventDefault();
        if (!vm.search.maHD) {
          alert('Vui lòng nhập mã hóa đơn.');
          return;
        }

        vm.loading = true;
        vm.resetAll();

        try {
          var res = await Gateway.getInvoiceById(vm.search.maHD);
          var invRaw =
            (res.data && res.data.data) ? res.data.data[0] :
              (Array.isArray(res.data) ? res.data[0] : res.data);

          if (!invRaw) {
            alert('Không tìm thấy hóa đơn.');
            return;
          }

          vm.invoice = normalizeInvoice(invRaw);

          var resCT = await Gateway.getDetailsByInvoice(vm.invoice.id);
          vm.details = Gateway.unwrapList(resCT).map(normalizeDetail);

          loadPayments(vm.invoice.id);
          recalc();
        } catch (err) {
          console.error('Fetch invoice error:', err);
          alert('Không có hóa đơn đổi.');
        } finally {
          vm.loading = false;
          $scope.$applyAsync();
        }
      };

      /* ----- XÁC NHẬN ĐỔI/TRẢ ----- */
      vm.createVoucher = async function (e) {
        if (e && e.preventDefault) e.preventDefault();
        if (!vm.invoice) {
          alert('Chưa có hóa đơn.');
          return;
        }

        if (!confirm('Xác nhận đổi / trả cho HĐ ' + vm.invoice.id + ' ?')) {
          return;
        }

        vm.loading = true;

        // cờ để biết có lỗi thật sự nghiêm trọng hay không
        var anySuccess = false;
        var deleteError = null;
        var resetError = null;

        // 1) Xoá chi tiết bán
        try {
          var deleted = await Gateway.deleteAllDetailsOfInvoice(vm.invoice.id);
          console.log('Đã xoá', deleted, 'chi tiết bán');
          anySuccess = true;
        } catch (err1) {
          console.error('Lỗi xoá chi tiết bán:', err1);
          deleteError = err1;
        }

        // 2) Reset số tiền thanh toán theo mã hoá đơn
        try {
        await Gateway.resetPaymentsByInvoice(vm.invoice.id, 0);
        vm.payments.forEach(function (p) { p.soTien = 0; });
          console.log('Đã reset SOTIENTHANHTOAN về 0 cho HĐ', vm.invoice.id);
          anySuccess = true;
        } catch (err2) {
          console.error('Lỗi reset thanh toán:', err2);
          resetError = err2;
        }

        // 3) Cập nhật UI nếu có ít nhất 1 bước thành công
        if (anySuccess) {
          // Cập nhật bảng chi tiết & tổng kết trên UI
          vm.details = [];
          resetReturnExchangeState();

          // Cập nhật bảng thanh toán: số tiền = 0
          vm.payments.forEach(function (p) { p.soTien = 0; });

          // Thông báo
          if (deleteError || resetError) {
            alert('Đổi / trả đã thực hiện.');
          } else {
            alert('Đổi / trả xong.');
          }
        } else {
          // cả hai bước đều lỗi → thất bại thật sự
          alert('Xử lý đổi trả thất bại.');
        }

        vm.loading = false;
        $scope.$applyAsync();
      };

      /* ----- MODAL: THANH TOÁN (simple) ----- */
      vm.openAddPayment = function () {
        if (!vm.invoice) {
          alert('Vui lòng tải hóa đơn trước.');
          return;
        }

        vm.modal.editPayment.isNew = true;
        vm.modal.editPayment.data = {
          id: null,
          maHoaDon: vm.invoice.id,
          soTien: vm.sumDiff > 0 ? vm.sumDiff : 0,
          phuongThuc: vm.paymentMethod || 'Tiền mặt',
          ghiChu: vm.note || ''
        };
        vm.modal.editPayment.show = true;
      };

      vm.savePayment = function () {
        var p = vm.modal.editPayment.data;
        if (!p || !p.maHoaDon) {
          alert('Thiếu mã hóa đơn.');
          return;
        }
        if (!p.soTien || p.soTien <= 0) {
          alert('Số tiền phải > 0');
          return;
        }

        if (vm.modal.editPayment.isNew) {
          var newId = p.id || ('TT_' + Date.now());
          vm.payments.push({
            id: newId,
            maHoaDon: p.maHoaDon,
            soTien: Number(p.soTien || 0),
            phuongThuc: p.phuongThuc || '',
            ngayThanhToan: new Date(),
            trangThai: 'Đã thanh toán'
          });
        }

        vm.modal.editPayment.show = false;
      };

      // stub để tránh lỗi nếu HTML có gọi
      vm.saveInvoice = function () { vm.modal.editInvoice.show = false; };
      vm.saveDetail = function () { vm.modal.editDetail.show = false; };

    });

})();
