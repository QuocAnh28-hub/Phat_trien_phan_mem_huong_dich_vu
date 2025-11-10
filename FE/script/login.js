// login.js
(function () {
  'use strict';

  // Tạo (hoặc lấy) module
  var app;
  try { app = angular.module('dahApp'); }
  catch (e) { app = angular.module('dahApp', []); }

  // Gắn token tự động cho $http
  app.factory('AuthInterceptor', function () {
    return {
      request: function (cfg) {
        var t = localStorage.getItem('token');
        if (t) cfg.headers.Authorization = 'Bearer ' + t;
        return cfg;
      }
    };
  });
  app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('AuthInterceptor');
  });

  // ========= AuthService: login, lấy role, route, logout =========
  app.constant('AUTH_API', 'https://localhost:7107/api-common/Login');

  app.factory('AuthService', function ($http, $window, $q, AUTH_API) {
    function persistLogin(payload, usernameFallback) {
      localStorage.setItem('token', payload.token || '');
      localStorage.setItem('isLoggedIn', 'true');
      // đảm bảo có 'username'
      var userObj = payload.user || { username: usernameFallback };
      localStorage.setItem('user', JSON.stringify(userObj));
    }
    function renderUserName() {
      try {
        var user = JSON.parse(localStorage.getItem('user') || '{}');
        var name = user.userName || user.username || 'DAH';
        var strong = document.querySelector('.quick-stats .row strong');
        if (strong) strong.textContent = name;
      } catch (_) {}
    }
    function routeByRole(role) {
      role = parseInt(role, 10);
      localStorage.setItem('role', String(role));
      if (role === 4)      $window.location.href = '../pages/QuanLyTaiKhoan.html';
      else if (role === 3) $window.location.href = '../pages/QuanLyDanhMuc.html';
      else if (role === 2) $window.location.href = '../pages/QuanLyBanHang.html';
      else if (role === 1) $window.location.href = '../pages/QuanLyCongNo.html';
      else                 $window.location.href = '../pages/index.html';
    }
    function getRoleAndRoute(username) {
      return $http.get(AUTH_API + '/get-role', { params: { username: username } })
        .then(function (res) {
          var data = (res && res.data && res.data.data) || {};
          if (data.quyen == null) throw new Error('Không xác định được quyền.');
          routeByRole(data.quyen);
          renderUserName();
        });
    }

    function login(username, password) {
      if (!username || !password) return $q.reject(new Error('Vui lòng nhập tài khoản và mật khẩu!'));
      return $http.post(AUTH_API + '/login', null, { params: { username: username, pass: password } })
        .then(function (res) {
          var d = (res && res.data) || {};
          if (!d.success) throw new Error(d.message || 'Sai thông tin đăng nhập!');
          alert(d.message || 'Đăng nhập thành công!');
          persistLogin(d, username);
          return getRoleAndRoute(username);
        })
        .catch(function (err) {
          console.error('Login error:', err);
          alert('Không thể kết nối đến API hoặc tài khoản không hợp lệ!');
        });
    }

    function logout() {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      localStorage.removeItem('role');
      localStorage.removeItem('isLoggedIn');
      $window.location.href = '../pages/index.html';
    }

    return { login: login, logout: logout, renderUserName: renderUserName };
  });

  // ========= Giữ nguyên cách gọi cũ: window.dangNhap(event) =========
  window.dangNhap = function (event) {
    event && event.preventDefault && event.preventDefault();
    var injector = angular.element(document.body).injector() || angular.injector(['ng', 'dahApp']);
    var AuthService = injector.get('AuthService');

    var user = document.getElementById('username').value.trim();
    var pass = document.getElementById('password').value.trim();
    if (!user || !pass) { alert('Vui lòng nhập đầy đủ tài khoản và mật khẩu!'); return false; }

    AuthService.login(user, pass);
    return false;
  };

  // ========= Gắn logout cho nút .logout a nếu có =========
  document.addEventListener('DOMContentLoaded', function () {
    var el = document.querySelector('.logout a');
    if (el) {
      el.addEventListener('click', function (e) {
        e.preventDefault();
        if (confirm('Bạn có chắc muốn đăng xuất không?')) {
          var injector = angular.element(document.body).injector() || angular.injector(['ng', 'dahApp']);
          injector.get('AuthService').logout();
        }
      });
    }
  });

})();
