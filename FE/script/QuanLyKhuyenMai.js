(function () {
  "use strict";

  angular.module("dahApp", [])
    .factory("AuthInterceptor", function () {
      return {
        request: function (config) {
          const t = localStorage.getItem("token");
          if (t) config.headers.Authorization = "Bearer " + t;
          return config;
        }
      };
    })
    .config(["$httpProvider", function ($httpProvider) {
      $httpProvider.interceptors.push("AuthInterceptor");
    }])
    .factory("KhuyenMaiService", ["$http", function ($http) {
      const KM_API = "https://localhost:7107/api-admin/QuanLyKhuyenMai";
      const EP = {
        ALL:    "/get-all-khuyenmai",
        BY_ID:  "/get-byid-khuyenmai",
        CREATE: "/create-khuyenmai",
        UPDATE: "/update-khuyenmai",
        DELETE: "/del-khuyenmai"
      };

      const norm = (x) => ({
        maKM:  (x.maKM ?? x.makm ?? "").trim(),
        tenKM: (x.tenKM ?? x.tenkm ?? "").trim(),
        maSP:  (x.maSP ?? x.masp ?? "").trim(),
        ngayBD: x.ngayBD ?? x.ngaybatdau ?? x.ngayBatDau,
        ngayKT: x.ngayKT ?? x.ngayketthuc ?? x.ngayKetThuc,
      });

      function unwrapList(res) {
        const d = res && res.data;
        if (Array.isArray(d?.data)) return d.data;
        if (Array.isArray(d)) return d;
        return [];
      }

      function getAll()    { return $http.get(KM_API + EP.ALL).then(unwrapList); }
      function getById(ma) { return $http.get(KM_API + EP.BY_ID, { params: { ma } }).then(unwrapList); }
      function create(b)   { return $http.post(KM_API + EP.CREATE, b); }
      function update(b)   { return $http.post(KM_API + EP.UPDATE, b); }
      function remove(ma)  {
        return $http.delete(KM_API + EP.DELETE, { params: { ma } })
          .catch(err => {
            if ([400,415,422].includes(err?.status)) {
              return $http.delete(KM_API + EP.DELETE, { data: { ma } });
            }
            throw err;
          });
      }

      return { norm, getAll, getById, create, update, remove };
    }])
    .controller("KhuyenMaiCtrl", ["$scope", "KhuyenMaiService", function ($scope, svc) {
      const vm = this;

      // Hiển thị username ở sidebar
      vm.currentUser = (JSON.parse(localStorage.getItem("user") || "{}").userName)
                    || (JSON.parse(localStorage.getItem("user") || "{}").username)
                    || localStorage.getItem("username")
                    || "DAH";

      vm.list = [];
      vm.searchText = "";
      vm.page = 1;
      vm.kmPerPage = 8;

      vm.modalAdd = false;
      vm.modalEdit = false;
      vm.add = {};
      vm.edit = {};

      vm.toISOZ = (dateObj, endOfDay=false) => {
        if (!dateObj) return null;
        const d = new Date(dateObj);
        if (endOfDay) { d.setHours(23,59,59,999); } else { d.setHours(0,0,0,0); }
        return d.toISOString();
      };
      vm.toVN = (iso) => {
        if (!iso) return "";
        const d = new Date(iso);
        return isNaN(d) ? iso : new Intl.DateTimeFormat("vi-VN").format(d);
      };

      vm.searchFilter = function (item) {
        if (!vm.searchText) return true;
        const q = vm.searchText.toLowerCase();
        return (item.maKM || "").toLowerCase().includes(q)
            || (item.tenKM || "").toLowerCase().includes(q)
            || (item.maSP || "").toLowerCase().includes(q);
      };

      vm.totalPages = function () {
        if (!vm.filtered) return 1;
        return Math.max(1, Math.ceil(vm.filtered.length / vm.kmPerPage));
      };
      vm.pages = function () {
        const t = vm.totalPages();
        return Array.from({length: t}, (_,i)=> i+1);
      };

      vm.load = function () {
        svc.getAll().then(list => {
          vm.list = list.map(svc.norm).sort((a,b)=> new Date(b.ngayBD) - new Date(a.ngayBD));
          vm.page = 1;
        }).catch(err => {
          console.error("Load KM error", err);
          alert("Không thể tải danh sách khuyến mãi!");
        });
      };

      vm.openAdd = function () {
        vm.add = { maKM:"", tenKM:"", maSP:"", _start:null, _end:null };
        vm.modalAdd = true;
      };
      vm.closeAdd = function () { vm.modalAdd = false; vm.add = {}; };

      vm.openEdit = function (k) {
        vm.edit = angular.copy(k);
        vm.edit._start = k.ngayBD ? new Date(k.ngayBD) : null;
        vm.edit._end   = k.ngayKT ? new Date(k.ngayKT) : null;
        vm.modalEdit = true;
      };
      vm.closeEdit = function () { vm.modalEdit = false; vm.edit = {}; };

      vm.create = function () {
        const body = {
          maKM:  (vm.add.maKM || "").trim(),
          tenKM: (vm.add.tenKM || "").trim(),
          maSP:  (vm.add.maSP || "").trim(),
          ngayBD: vm.toISOZ(vm.add._start, false),
          ngayKT: vm.toISOZ(vm.add._end,   true),
        };
        if (!body.maKM || !body.tenKM || !body.maSP) {
          alert("Vui lòng nhập maKM, tenKM, maSP!");
          return;
        }
        svc.create(body).then(()=> {
          alert("Thêm khuyến mãi thành công!");
          vm.modalAdd = false; vm.add = {};
          vm.load();
        }).catch(err=>{
          console.error("Create KM error", err);
          alert("Không thể thêm khuyến mãi!");
        });
      };

      vm.update = function () {
        const body = {
          maKM:  (vm.edit.maKM || "").trim(),
          tenKM: (vm.edit.tenKM || "").trim(),
          maSP:  (vm.edit.maSP || "").trim(),
          ngayBD: vm.toISOZ(vm.edit._start, false),
          ngayKT: vm.toISOZ(vm.edit._end,   true),
        };
        if (!body.tenKM || !body.maSP) {
          alert("tenKM và maSP không được trống!");
          return;
        }
        svc.update(body).then(()=> {
          alert("Cập nhật khuyến mãi thành công!");
          vm.modalEdit = false; vm.edit = {};
          vm.load();
        }).catch(err=>{
          console.error("Update KM error", err);
          alert("Không thể cập nhật khuyến mãi!");
        });
      };

      vm.confirmDelete = function (k) {
        if (!confirm(`Bạn có chắc muốn xóa khuyến mãi '${k.maKM}' không?`)) return;
        svc.remove(k.maKM).then(()=> {
          alert("Xóa khuyến mãi thành công!");
          vm.load();
        }).catch(err=>{
          console.error("Delete KM error", err);
          alert("Không thể xóa khuyến mãi!");
        });
      };

      vm.load();
    }]);
})();
