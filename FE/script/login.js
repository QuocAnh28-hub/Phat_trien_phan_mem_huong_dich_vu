(function () {
  'use strict';

  angular.module('dahApp')
    .constant('AUTH_API', 'https://localhost:7107/api-common/Login')

    .factory('AuthService', function ($http, AUTH_API, $window, $q) {

      function persistLogin(payload, usernameFallback) {
        localStorage.setItem('token', payload.token || '');
        localStorage.setItem('isLoggedIn', 'true');
        const userObj = payload.user || { username: usernameFallback };
        localStorage.setItem('user', JSON.stringify(userObj));
      }

      function routeByRole(role) {
        switch (role) {
          case 4: $window.location.href = '../pages/QuanLyTaiKhoan.html'; break; // Admin
          case 3: $window.location.href = '../pages/QuanLyDanhMuc.html';  break; // Quản lý/Kho
          case 2: $window.location.href = '../pages/QuanLyBanHang.html';  break; // Thu ngân
          case 1: $window.location.href = '../pages/QuanLyCongNo.html';   break; // Kế toán
          default: $window.location.href = '../pages/index.html';
        }
      }

      function fetchRoleAndRoute(username) {
        const u = encodeURIComponent(username);
        return $http.get(`${AUTH_API}/get-role?username=${u}`).then((res) => {
          const data = res?.data?.data || res?.data || {};
          const role = parseInt(data.quyen, 10);
          if (Number.isNaN(role)) throw new Error('Không xác định được quyền.');
          localStorage.setItem('role', String(role));
          routeByRole(role);
        });
      }

      function login(username, password) {
        if (!username || !password) return $q.reject(new Error('Vui lòng nhập tài khoản và mật khẩu!'));
        const u = encodeURIComponent(username);
        const p = encodeURIComponent(password);

        return $http.post(`${AUTH_API}/login?username=${u}&pass=${p}`)
          .then((res) => {
            const d = res?.data || {};
            if (!d.success) throw new Error(d.message || 'Sai thông tin đăng nhập!');
            persistLogin(d, username);
            return fetchRoleAndRoute(username);
          });
      }

      return { login };
    })

    .controller('LoginCtrl', function (AuthService) {
      const vm = this;
      vm.form = { username: '', password: '' };
      vm.showPwd = false;
      vm.isBusy = false;

      vm.submit = function (e) {
        e?.preventDefault?.();
        if (!vm.form.username || !vm.form.password) {
          alert('Vui lòng nhập đầy đủ tài khoản và mật khẩu!');
          return;
        }
        vm.isBusy = true;
        AuthService.login(vm.form.username, vm.form.password)
          .catch((err) => {
            console.error('Login error:', err);
            alert(err?.message || 'Không thể kết nối đến API hoặc tài khoản không hợp lệ!');
          })
          .finally(() => { vm.isBusy = false; });
      };
    });

})();
