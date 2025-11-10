// auth.js
(function () {
  'use strict';

  // Chặn truy cập khi chưa đăng nhập
  document.addEventListener('DOMContentLoaded', function () {
    var isLoggedIn = localStorage.getItem('isLoggedIn');
    var token = localStorage.getItem('token');
    var role = parseInt(localStorage.getItem('role'), 10);

    if (!isLoggedIn || !token || !Number.isInteger(role)) {
      if (!/index\.html?$/i.test(window.location.pathname)) {
        window.location.href = '../pages/index.html';
      }
      return;
    }

    // Hiển thị tên người dùng ở sidebar
    try {
      var user = JSON.parse(localStorage.getItem('user') || '{}');
      var name = user.userName || user.username || 'DAH';
      var strong = document.querySelector('.quick-stats .row strong');
      if (strong) strong.textContent = name;
    } catch (_) {}

    // Quyền -> whitelist
    var allowByRole = {
      4: ['QuanLyTaiKhoan','QuanLyNhanVien','QuanLyKhuyenMai','QuanLyCongNo','BaoCaoThongKe','QuanLyBanHang','XuLyDoiTra','QuanLyDanhMuc','QuanLySanPham','QuanLyNhapKho','QuanLyTonKho'],
      3: ['QuanLyDanhMuc','QuanLySanPham','QuanLyNhapKho','QuanLyTonKho'],
      2: ['QuanLyBanHang','XuLyDoiTra'],
      1: ['QuanLyCongNo','BaoCaoThongKe']
    };
    var defaultPageByRole = {
      4: '../pages/QuanLyTaiKhoan.html',
      3: '../pages/QuanLyDanhMuc.html',
      2: '../pages/QuanLyBanHang.html',
      1: '../pages/QuanLyCongNo.html'
    };
    var allowed = allowByRole[role] || [];

    var currentPath = window.location.pathname;
    var m = currentPath.match(/([^/]+)\.html?$/i);
    var currentSlug = m ? m[1] : '';

    // Lọc sidebar (ẩn những link không có quyền)
    var nav = document.querySelector('.sidebar .nav');
    if (nav) {
      var links = nav.querySelectorAll("a[href$='.html'], a[href$='.htm']");
      links.forEach(function (a) {
        var href = a.getAttribute('href') || '';
        var mm = href.match(/([^/]+)\.html?$/i);
        var slug = mm ? mm[1] : '';
        var canSee = allowed.some(function (s) { return slug.indexOf(s) !== -1; });
        a.style.display = canSee ? '' : 'none';
      });

      // Ẩn heading trống
      var titles = nav.querySelectorAll('.section-title');
      titles.forEach(function (title) {
        var node = title.nextElementSibling, hasVisible = false;
        while (node && !node.classList.contains('section-title')) {
          if (node.matches && node.matches('a[href]') && node.style.display !== 'none') { hasVisible = true; break; }
          node = node.nextElementSibling;
        }
        title.style.display = hasVisible ? '' : 'none';
      });
    }

    // Chặn truy cập trực tiếp
    var isIndex = /index\.html?$/i.test(currentPath);
    var isAllowedPage = allowed.some(function (s) { return currentSlug.indexOf(s) !== -1; });
    if (!isIndex && !isAllowedPage) {
      var target = defaultPageByRole[role] || '../pages/index.html';
      if (!currentPath.endsWith(target.split('/').pop())) {
        window.location.href = target;
        return;
      }
    }

    // Ẩn/hiện section theo role (nếu dùng data-section)
    var sectionAllowByRole = { 4:['ke-toan','ban-hang','kho','admin'], 3:['kho'], 2:['ban-hang'], 1:['ke-toan'] };
    var allowedSections = sectionAllowByRole[role] || [];
    document.querySelectorAll('[data-section]').forEach(function (sec) {
      var name = sec.getAttribute('data-section');
      sec.style.display = allowedSections.indexOf(name) !== -1 ? '' : 'none';
    });

    // Bắt sự kiện logout ở mọi trang (giữ hành vi cũ)
    var logoutBtn = document.querySelector('.logout a');
    if (logoutBtn) {
      logoutBtn.addEventListener('click', function (e) {
        e.preventDefault();
        if (confirm('Bạn có chắc muốn đăng xuất không?')) {
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          localStorage.removeItem('role');
          localStorage.removeItem('isLoggedIn');
          window.location.href = '../pages/index.html';
        }
      });
    }
  });
})();
