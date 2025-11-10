(function () {
  'use strict';

  /* ================== MODULE & CONFIG ================== */
  angular.module('DAHApp', [])
    .constant('API_BASE', 'https://localhost:7107/api-thungan/QuanLyDoiTra') // đổi nếu gateway khác
    .factory('AuthInterceptor', function () {
      return {
        request: function (cfg) {
          const t = localStorage.getItem('token');
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

      function tryParams(path, paramsList) {
        const url = API_BASE + path;
        return new Promise(async (resolve, reject) => {
          let lastErr = null;
          for (const p of paramsList) {
            try { const res = await $http.get(url, { params: p }); return resolve(res); }
            catch (e) {
              lastErr = e;
              const code = e.status;
              if (![400,404,415,422].includes(code)) return reject(e);
            }
          }
          reject(lastErr || new Error('Không tìm thấy'));
        });
      }

      function unwrapList(res) {
        const d = res && res.data;
        if (Array.isArray(d?.data)) return d.data;
        if (Array.isArray(d)) return d;
        return [];
      }

      return {
        getAllInvoices: () => $http.get(API_BASE + '/get-all-hoadonban'),
        getInvoiceById: (id) => tryParams('/get-hoadonban-by-id', [
          { maHoaDon: id }, { maHDB: id }, { id }, { ma: id }
        ]),
        getDetailsByInvoice: (invoiceId) => tryParams('/get-chitietban-by-IDhoadon', [
          { maHDB: invoiceId }, { maHoaDon: invoiceId }, { id: invoiceId }, { ma: invoiceId }
        ]),
        getAllDetails: () => $http.get(API_BASE + '/get-all-chitietban'),
        unwrapList
      };
    })

  /* ================== CONTROLLER ================== */
    .controller('XuLyDoiTraCtrl', function ($scope, Gateway) {
      const vm = this;

      // user
      (function setUser() {
        const u = JSON.parse(localStorage.getItem('user') || '{}');
        vm.currentUser = u.userName || u.username || localStorage.getItem('username') || 'thungan';
      })();

      // state
      vm.search = { maHD: '', from: null, to: null };
      vm.invoice = null;
      vm.details = []; // normalized
      vm.retQty = {};  // { productId: number }
      vm.retReason = {};
      vm.exchangeItems = [];
      vm.newEx = { code: '', qty: null, price: null };
      vm.sumReturn = 0; vm.sumExchange = 0; vm.sumDiff = 0;
      vm.history = [];

      // helpers normalize
      function pickInvoiceId(x) {
        return x?.mahdban || x?.mahdBan || x?.maHoaDon || x?.mahoadon || x?.maHD || x?.ma || x?.id || null;
      }
      function normalizeInvoice(x) {
        if (!x) return null;
        const id = pickInvoiceId(x);
        return {
          id,
          customerId: x.makh ?? x.maKH ?? '',
          createdAt: x.ngaylap ?? x.ngayLap ?? null,
          goodsTotal: Number(x.tongtienhang ?? x.tongTienHang ?? 0)
        };
      }
      function normalizeDetail(d) {
        return {
          invoiceId: d.mahdban ?? d.maHoaDon ?? d.maHDB ?? d.id ?? '',
          productId: d.masp ?? d.maSP ?? d.code ?? '',
          productName: d.tensp ?? d.tenSP ?? null,
          qty: Number(d.soluong ?? d.soLuong ?? 0),
          price: Number(d.dongia ?? d.donGia ?? 0),
          lineTotal: Number(
            d.tongtien ?? d.tongTien ?? d.thanhtien ?? d.thanhTien ??
            (Number(d.soluong ?? 0) * Number(d.dongia ?? 0))
          )
        };
      }

      // actions
      vm.resetAll = function (e) {
        if (e) e.preventDefault();
        vm.search = { maHD: '', from: null, to: null };
        vm.invoice = null;
        vm.details = [];
        vm.retQty = {};
        vm.retReason = {};
        vm.exchangeItems = [];
        vm.newEx = { code: '', qty: null, price: null };
        vm.sumReturn = vm.sumExchange = vm.sumDiff = 0;
      };

      vm.fetchInvoice = async function (e) {
        if (e) e.preventDefault();
        vm.invoice = null;
        vm.details = []; vm.retQty = {}; vm.retReason = {};
        vm.sumReturn = vm.sumExchange = vm.sumDiff = 0;

        try {
          let invRaw = null;

          if (vm.search.maHD) {
            const res = await Gateway.getInvoiceById(vm.search.maHD);
            invRaw = (res.data?.data || res.data || [])[0] || null;
          } else {
            const res = await Gateway.getAllInvoices();
            const list = Gateway.unwrapList(res);
            if (!vm.search.from && !vm.search.to) {
              invRaw = list[0] || null;
            } else {
              const from = vm.search.from ? new Date(vm.search.from) : null;
              const to = vm.search.to ? new Date(vm.search.to + 'T23:59:59') : null;
              invRaw = list.find(x => {
                const d = new Date(x.ngaylap ?? x.ngayLap);
                return (from ? d >= from : true) && (to ? d <= to : true);
              }) || null;
            }
          }

          if (!invRaw) return alert('Không tìm thấy hóa đơn phù hợp.');
          vm.invoice = normalizeInvoice(invRaw);

          // details
          let details = [];
          try {
            const resCT = await Gateway.getDetailsByInvoice(vm.invoice.id);
            details = Gateway.unwrapList(resCT).map(normalizeDetail);
          } catch {
            // fallback
            const resAll = await Gateway.getAllDetails();
            const all = Gateway.unwrapList(resAll).map(normalizeDetail);
            details = all.filter(x => x.invoiceId === vm.invoice.id);
          }
          vm.details = details;
          vm.recalc();
          $scope.$applyAsync();
        } catch (err) {
          console.error('Fetch invoice error:', err);
          alert('Không thể tải hóa đơn.');
        }
      };

      vm.onQtyChange = function (productId, max) {
        const v = Math.max(0, Math.min(Number(vm.retQty[productId] || 0), Number(max)));
        vm.retQty[productId] = v;
        vm.recalc();
      };

      vm.toggleReturn = function (productId) {
        vm.retQty[productId] = vm.retQty[productId] > 0 ? 0 : 1;
        vm.recalc();
      };

      vm.addExchange = function (e) {
        if (e) e.preventDefault();
        const code = (vm.newEx.code || '').trim();
        const qty = Number(vm.newEx.qty || 0);
        const price = Number(vm.newEx.price || 0);
        if (!code || qty <= 0 || price < 0) return alert('Nhập Mã/Tên, SL (>0) và Giá hợp lệ.');
        vm.exchangeItems.push({ code, qty, price });
        vm.newEx = { code: '', qty: null, price: null };
        vm.recalc();
      };

      vm.removeExchange = function (i) {
        vm.exchangeItems.splice(i, 1);
        vm.recalc();
      };

      vm.recalc = function () {
        const priceMap = Object.fromEntries(vm.details.map(x => [x.productId, x.price]));
        vm.sumReturn = Object.entries(vm.retQty)
          .reduce((a, [sp, q]) => a + (Number(q || 0) * (priceMap[sp] || 0)), 0);
        vm.sumExchange = vm.exchangeItems.reduce((a, x) => a + x.qty * x.price, 0);
        vm.sumDiff = vm.sumExchange - vm.sumReturn;
      };

      vm.createVoucher = function (e) {
        if (e) e.preventDefault();
        if (!vm.invoice) return alert('Chưa có hóa đơn.');
        const hasRet = Object.values(vm.retQty).some(q => Number(q) > 0);
        const hasEx = vm.exchangeItems.length > 0;
        if (!hasRet && !hasEx) return alert('Bạn chưa chọn hàng đổi/trả.');

        const hangTra = Object.entries(vm.retQty)
          .filter(([_, q]) => Number(q) > 0)
          .map(([productId, soLuong]) => {
            const it = vm.details.find(r => r.productId === productId);
            return { masp: productId, soLuong: Number(soLuong), donGia: it?.price || 0 };
          });

        const hangDoi = vm.exchangeItems.map(x => ({
          masp: x.code, soLuong: Number(x.qty), donGia: Number(x.price)
        }));

        const payload = {
          hoaDonGoc: vm.invoice.id,
          hangTra, hangDoi,
          hinhThuc: vm.paymentMethod || 'Tiền mặt',
          ghiChu: vm.note || ''
        };

        console.log('Payload tạo phiếu đổi/trả:', payload);
        alert('Đã tạo payload (mở console để xem). Khi có endpoint tạo phiếu, gọi Gateway tương ứng là xong.');
        // Ví dụ sau này:
        // $http.post(API_BASE + '/insert-phieu-doi-tra', payload).then(...);
      };

      vm.pickHistory = function (raw) {
        vm.search.maHD = pickInvoiceId(raw);
        vm.fetchInvoice();
      };

      // recent
      (async function init() {
        try {
          const res = await Gateway.getAllInvoices();
          vm.history = Gateway.unwrapList(res).slice(0, 10);
          $scope.$applyAsync();
        } catch (e) {
          vm.history = [];
        }
      })();
    });

})();
