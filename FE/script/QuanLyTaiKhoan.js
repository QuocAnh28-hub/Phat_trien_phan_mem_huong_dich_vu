(function () {
  "use strict";

  angular.module("dahApp", [])
    /* Tự gắn Bearer token */
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

    /* Service gọi API + chuẩn hoá dữ liệu */
    .factory("TaiKhoanService", ["$http", function ($http) {
      const API_URL = "https://localhost:7107/api-admin/QuanLyTaiKhoan";
      const EP = {
        ALL: "/get-all-taikhoan",
        BY_ID: "/get-byid-taikhoan",       // ?maTaiKhoan=
        CREATE: "/create-taikhoan",
        UPDATE: "/update-byID-taikhoan",
        DELETE: "/del-byID-taikhoan"       // ?maTaiKhoan=
      };

      const ensureList = (res) => {
        const d = res && res.data;
        if (Array.isArray(d?.data)) return d.data;
        if (Array.isArray(d)) return d;
        return [];
      };

      function getAll() { return $http.get(API_URL + EP.ALL).then(ensureList); }

      function getById(id) {
        return $http.get(API_URL + EP.BY_ID, { params: { maTaiKhoan: id } }).then(ensureList);
      }

      async function create(base) {
        // BE có thể nhận 'pass' hoặc 'password'
        try {
          return await $http.post(API_URL + EP.CREATE, base);
        } catch (e) {
          const alt = { ...base };
          if ("pass" in alt) { alt.password = alt.pass; delete alt.pass; }
          else if ("password" in alt) { alt.pass = alt.password; delete alt.password; }
          return $http.post(API_URL + EP.CREATE, alt);
        }
      }

      async function update(base) {
        try {
          return await $http.post(API_URL + EP.UPDATE, base);
        } catch (e) {
          const alt = { ...base };
          if ("pass" in alt) { alt.password = alt.pass; delete alt.pass; }
          else if ("password" in alt) { alt.pass = alt.password; delete alt.password; }
          return $http.post(API_URL + EP.UPDATE, alt);
        }
      }

      async function remove(id) {
        try {
          return await $http.delete(API_URL + EP.DELETE, { params: { maTaiKhoan: id } });
        } catch (e) {
          // fallback: DELETE với body
          return $http.delete(API_URL + EP.DELETE, { data: { maTaiKhoan: id } });
        }
      }

      return { getAll, getById, create, update, remove };
    }])

    /* Controller UI */
    .controller("TaiKhoanCtrl", ["$scope", "TaiKhoanService", function ($scope, svc) {
      const vm = this;

      // Username hiển thị
      vm.currentUser = (JSON.parse(localStorage.getItem("user") || "{}").userName)
        || (JSON.parse(localStorage.getItem("user") || "{}").username)
        || localStorage.getItem("username")
        || "DAH";

      // State
      vm.list = [];
      vm.searchText = "";
      vm.page = 1;
      vm.perPage = 8;

      vm.modalAdd = false;
      vm.modalEdit = false;
      vm.add = {};
      vm.edit = {};

      // Utilities
      vm.roleName = function (q) {
        const r = parseInt(q, 10);
        if (r === 4) return "Admin";
        if (r === 3) return "Thủ Kho";
        if (r === 2) return "Thu Ngân";
        if (r === 1) return "Kế toán";
        return q ?? "";
      };

      vm.searchFilter = function (x) {
        if (!vm.searchText) return true;
        const q = vm.searchText.toLowerCase();
        return (x.maTaiKhoan || "").toLowerCase().includes(q)
            || (x.userName || "").toLowerCase().includes(q);
      };

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
        svc.getAll()
          .then(list => {
            vm.list = list;
            vm.page = 1;
          })
          .catch(err => {
            console.error("Load error:", err);
            alert("Không thể tải danh sách tài khoản.");
          });
      };

      // Add
      vm.openAdd = function () {
        vm.add = { maTaiKhoan: "", userName: "", pass: "", quyen: "2" };
        vm.modalAdd = true;
      };
      vm.closeAdd = function () {
        vm.modalAdd = false; vm.add = {};
      };
      vm.create = function () {
        if (!vm.add.userName || !vm.add.pass) {
          alert("Vui lòng nhập USERNAME và PASS!");
          return;
        }
        const payload = {
          maTaiKhoan: vm.add.maTaiKhoan?.trim(),
          userName: vm.add.userName?.trim(),
          pass: vm.add.pass?.trim(),
          quyen: vm.add.quyen
        };
        svc.create(payload).then(() => {
          alert("Thêm tài khoản thành công!");
          vm.closeAdd(); vm.load();
        }).catch(e => {
          console.error("Create error:", e);
          alert("Không thể thêm tài khoản!");
        });
      };

      // Edit
      vm.openEdit = function (x) {
        vm.edit = angular.copy(x);
        vm.edit.pass = ""; // để trống nếu không đổi
        vm.modalEdit = true;
      };
      vm.closeEdit = function () {
        vm.modalEdit = false; vm.edit = {};
      };
      vm.update = function () {
        if (!vm.edit.userName) {
          alert("USERNAME không được trống!");
          return;
        }
        const payload = {
          maTaiKhoan: vm.edit.maTaiKhoan?.trim(),
          userName: vm.edit.userName?.trim(),
          quyen: vm.edit.quyen
        };
        if (vm.edit.pass) payload.pass = vm.edit.pass?.trim();

        svc.update(payload).then(() => {
          alert("Sửa tài khoản thành công!");
          vm.closeEdit(); vm.load();
        }).catch(e => {
          console.error("Update error:", e);
          alert("Không thể sửa tài khoản!");
        });
      };

      // Delete
      vm.confirmDelete = function (x) {
        if (!confirm(`Bạn có chắc muốn xóa tài khoản '${x.maTaiKhoan}' không?`)) return;
        svc.remove(x.maTaiKhoan).then(() => {
          alert("Xoá tài khoản thành công!");
          vm.load();
        }).catch(e => {
          console.error("Delete error:", e);
          alert("Không thể xoá tài khoản!");
        });
      };

      // Init
      vm.load();
    }]);
})();
