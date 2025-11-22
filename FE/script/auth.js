/*
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
*/

// auth.js
(function () {
  'use strict';

  // Hàm decode JWT token
  function parseJwt(token) {
    try {
      var base64Url = token.split('.')[1];
      if (!base64Url) return null;
      
      var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      var jsonPayload = decodeURIComponent(
        atob(base64).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join('')
      );
      return JSON.parse(jsonPayload);
    } catch (e) {
      console.error('Không thể decode token:', e);
      return null;
    }
  }

  // Hàm logout và xóa dữ liệu
  function performLogout(message) {
    if (message) {
      alert(message);
    }
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('role');
    localStorage.removeItem('isLoggedIn');
    window.location.href = '../pages/index.html';
  }

  // Hàm kiểm tra token hợp lệ
  function validateToken(token, localRole) {
    // Kiểm tra token tồn tại và có định dạng đúng
    if (!token || typeof token !== 'string') {
      console.error('Token không tồn tại hoặc không hợp lệ');
      return { valid: false, reason: 'Token không hợp lệ' };
    }

    // Kiểm tra định dạng JWT (phải có 3 phần)
    if (token.split('.').length !== 3) {
      console.error('Token không đúng định dạng JWT');
      return { valid: false, reason: 'Token không đúng định dạng' };
    }

    // Decode token
    var payload = parseJwt(token);
    if (!payload) {
      console.error('Không thể decode token');
      return { valid: false, reason: 'Token không thể giải mã' };
    }

    // Kiểm tra token hết hạn
    if (payload.exp) {
      var now = Date.now();
      var expireTime = payload.exp * 1000;
      
      if (expireTime < now) {
        var expiredMinutes = Math.floor((now - expireTime) / 60000);
        console.error('Token đã hết hạn ' + expiredMinutes + ' phút trước');
        return { 
          valid: false, 
          reason: 'expired',
          message: 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.' 
        };
      }

      // Cảnh báo nếu token sắp hết hạn (còn < 5 phút)
      var remainingMinutes = Math.floor((expireTime - now) / 60000);
      if (remainingMinutes < 5 && remainingMinutes > 0) {
        console.warn('Token sắp hết hạn sau ' + remainingMinutes + ' phút');
      }
    }

    // Kiểm tra role khớp với token
    var tokenRole = parseInt(payload.QUYEN, 10);
    if (!Number.isInteger(tokenRole)) {
      console.error('Token không chứa thông tin quyền hợp lệ');
      return { valid: false, reason: 'Token thiếu thông tin quyền' };
    }

    if (tokenRole !== localRole) {
      console.error('PHÁT HIỆN FAKE ROLE!');
      console.error('   Role trong localStorage:', localRole);
      console.error('   Role thật trong token:', tokenRole);
      return { 
        valid: false, 
        reason: 'fake_role',
        message: 'Phát hiện hành vi bất thường. Vui lòng đăng nhập lại.' 
      };
    }

    console.log('Token hợp lệ - Role:', tokenRole);
    return { valid: true, payload: payload };
  }

  // Chặn truy cập khi chưa đăng nhập
  document.addEventListener('DOMContentLoaded', function () {
    var isLoggedIn = localStorage.getItem('isLoggedIn');
    var token = localStorage.getItem('token');
    var role = parseInt(localStorage.getItem('role'), 10);

    // Kiểm tra cơ bản
    if (!isLoggedIn || !token || !Number.isInteger(role)) {
      if (!/index\.html?$/i.test(window.location.pathname)) {
        window.location.href = '../pages/index.html';
      }
      return;
    }

    // Validate token (kiểm tra hết hạn và fake role)
    var validation = validateToken(token, role);
    if (!validation.valid) {
      performLogout(validation.message);
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
          performLogout();
        }
      });
    }

    // Kiểm tra token định kỳ mỗi 30 giây
    setInterval(function() {
      var currentToken = localStorage.getItem('token');
      var currentRole = parseInt(localStorage.getItem('role'), 10);
      
      if (!currentToken) {
        performLogout('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.');
        return;
      }

      var validation = validateToken(currentToken, currentRole);
      if (!validation.valid) {
        performLogout(validation.message);
      }
    }, 30000); // Kiểm tra mỗi 30 giây
  });
})();