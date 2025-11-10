(function () {
  "use strict";

  angular.module("dahApp", [])
    // Interceptor tự gắn Bearer token
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

    // Service gọi API
    .factory("NhanVienService", ["$http", function ($http) {
      const NV_API = "https://localhost:7107/api-admin/QuanLyNhanVien";
      const EP = {
        ALL:    "/get-all-nhanvien",
        BY_ID:  "/get-byid-nhanvien",   // ?manv=
        CREATE: "/create-nhanvien",     // body: { manv, tennv, sdt, diachi }
        UPDATE: "/update-nhanvien",
        DELETE: "/delete-nhanvien"      // ?manv=
      };

      const norm = (x) => ({
        manv:   (x.manv   || "").trim(),
        tennv:  (x.tennv  || "").trim(),
        sdt:    (x.sdt    || "").trim(),
        diachi: (x.diachi || "").trim(),
      });

      const unwrapList = (res) => {
        const d = res && res.data;
        if (Array.isArray(d?.data)) return d.data;
        if (Array.isArray(d)) return d;
        return [];
      };

      function getAll()      { return $http.get(NV_API + EP.ALL).then(unwrapList); }
      function getById(id)   { return $http.get(NV_API + EP.BY_ID, { params: { manv: id } }).then(unwrapList); }
      function create(body)  { return $http.post(NV_API + EP.CREATE, body); }
      function update(body)  { return $http.post(NV_API + EP.UPDATE, body); }
      function remove(id)    { return $http.delete(NV_API + EP.DELETE, { params: { manv: id } }); }

      return { norm, getAll, getById, create, update, remove };
    }])

    // Controller UI
    .controller("NhanVienCtrl", ["$scope", "NhanVienService", function ($scope, svc) {
      const vm = this;

      // Hiển thị username ở sidebar
      vm.currentUser =
        (JSON.parse(localStorage.getItem("user") || "{}").userName) ||
        (JSON.parse(localStorage.getItem("user") || "{}").username) ||
        localStorage.getItem("username") ||
        "DAH";

      // State
      vm.list = [];
      vm.searchText = "";
      vm.page = 1;
      vm.perPage = 8;

      vm.modalAdd = false;
      vm.modalEdit = false;
      vm.add = {};
      vm.edit = {};

      // Search filter
      vm.searchFilter = function (x) {
        if (!vm.searchText) return true;
        const q = vm.searchText.toLowerCase();
        return (x.manv || "").toLowerCase().includes(q)
            || (x.tennv || "").toLowerCase().includes(q)
            || (x.sdt || "").toLowerCase().includes(q);
      };

      // Pagination
      vm.totalPages = function () {
        if (!vm.filtered) return 1;
        return Math.max(1, Math.ceil(vm.filtered.length / vm.perPage));
      };
      vm.pages = function () {
        const t = vm.totalPages();
        return Array.from({ length: t }, (_, i) => i + 1);
      };

      // Load
      vm.load = function () {
        svc.getAll().then(list => {
          vm.list = list.map(svc.norm);
          vm.page = 1;
        }).catch(err => {
          console.error("Load NV error", err);
          alert("Không thể tải danh sách nhân viên!");
        });
      };

      // Add
      vm.openAdd = function () {
        vm.add = { manv:"", tennv:"", sdt:"", diachi:"" };
        vm.modalAdd = true;
      };
      vm.closeAdd = function () { vm.modalAdd = false; vm.add = {}; };
      vm.create = function () {
        const b = {
          manv:   (vm.add.manv || "").trim(),
          tennv:  (vm.add.tennv || "").trim(),
          sdt:    (vm.add.sdt || "").trim(),
          diachi: (vm.add.diachi || "").trim(),
        };
        if (!b.manv || !b.tennv) { alert("Vui lòng nhập MANV và TENNV!"); return; }
        svc.create(b).then(() => {
          alert("Thêm nhân viên thành công!");
          vm.closeAdd(); vm.load();
        }).catch(e => {
          console.error("Create NV error", e);
          alert("Không thể thêm nhân viên!");
        });
      };

      // Edit
      vm.openEdit = function (nv) {
        vm.edit = angular.copy(nv);
        vm.modalEdit = true;
      };
      vm.closeEdit = function () { vm.modalEdit = false; vm.edit = {}; };
      vm.update = function () {
        const b = {
          manv:   (vm.edit.manv || "").trim(),
          tennv:  (vm.edit.tennv || "").trim(),
          sdt:    (vm.edit.sdt || "").trim(),
          diachi: (vm.edit.diachi || "").trim(),
        };
        if (!b.tennv) { alert("TENNV không được trống!"); return; }
        svc.update(b).then(() => {
          alert("Cập nhật nhân viên thành công!");
          vm.closeEdit(); vm.load();
        }).catch(e => {
          console.error("Update NV error", e);
          alert("Không thể cập nhật nhân viên!");
        });
      };

      // Delete
      vm.confirmDelete = function (nv) {
        if (!confirm(`Bạn có chắc muốn xóa nhân viên '${nv.manv}' không?`)) return;
        svc.remove(nv.manv).then(() => {
          alert("Xóa nhân viên thành công!");
          vm.load();
        }).catch(e => {
          console.error("Delete NV error", e);
          alert("Không thể xóa nhân viên!");
        });
      };

      // Init
      vm.load();
    }]);
})();
