// auth.js
document.addEventListener("DOMContentLoaded", () => {
  const isLoggedIn = localStorage.getItem("isLoggedIn");
  const token = localStorage.getItem("token");
  const role = Number.parseInt(localStorage.getItem("role"), 10);

  // 0) Chặn chưa đăng nhập
  if (!isLoggedIn || !token || !Number.isInteger(role)) {
    if (!/index\.html?$/i.test(window.location.pathname)) {
      window.location.href = "../pages/index.html";
    }
    return;
  }

  // 1) WHITELIST trang theo vai trò (không cần sửa HTML, dựa vào tên file .html)
  const allowByRole = {
    4: ["QuanLyTaiKhoan", "QuanLyNhanVien", "QuanLyKhuyenMai", "QuanLyCongNo", "BaoCaoThongKe", "QuanLyBanHang", "XuLyDoiTra", "QuanLyDanhMuc", "QuanLySanPham", "QuanLyNhapKho", "QuanLyTonKho"],
    3: ["QuanLyDanhMuc", "QuanLySanPham", "QuanLyNhapKho", "QuanLyTonKho"],
    2: ["QuanLyBanHang", "XuLyDoiTra"],
    1: ["QuanLyCongNo", "BaoCaoThongKe"] // chỉ kế toán
  };
  const defaultPageByRole = {
    4: "../pages/QuanLyTaiKhoan.html",
    3: "../pages/QuanLyDanhMuc.html",
    2: "../pages/QuanLyBanHang.html",
    1: "../pages/QuanLyCongNo.html"
  };

  const allowedSlugs = allowByRole[role] || [];

  const currentPath = window.location.pathname;
  const currentSlug = (() => {
    const m = currentPath.match(/([^/]+)\.html?$/i);
    return m ? m[1] : ""; // ví dụ "QuanLyTaiKhoan"
  })();

  // 2) Lọc sidebar: ẩn link không thuộc quyền + dọn section-title trống
  const nav = document.querySelector(".sidebar .nav");
  if (nav) {
    const links = nav.querySelectorAll("a[href$='.html'], a[href$='.htm']");
    links.forEach(a => {
      const href = a.getAttribute("href") || "";
      const m = href.match(/([^/]+)\.html?$/i);
      const slug = m ? m[1] : "";
      const canSee = allowedSlugs.some(s => slug.includes(s));
      a.style.display = canSee ? "" : "none";
    });

    // Ẩn section-title không còn link visible
    const titles = nav.querySelectorAll(".section-title");
    titles.forEach(title => {
      let node = title.nextElementSibling;
      let hasVisible = false;
      while (node && !node.classList.contains("section-title")) {
        if (node.matches?.("a[href]") && node.style.display !== "none") { hasVisible = true; break; }
        node = node.nextElementSibling;
      }
      title.style.display = hasVisible ? "" : "none";
    });
  }

  // 3) Chặn truy cập trực tiếp: nếu file hiện tại không thuộc whitelist -> redirect
  const isIndex = /index\.html?$/i.test(currentPath);
  const isAllowedPage = allowedSlugs.some(s => currentSlug.includes(s));
  if (!isIndex && !isAllowedPage) {
    const target = defaultPageByRole[role] || "../pages/index.html";
    if (!currentPath.endsWith(target.split("/").pop())) {
      window.location.href = target;
      return;
    }
  }

    const sectionAllowByRole = {
    4: ["ke-toan", "ban-hang", "kho", "admin"],
    3: ["kho"],
    2: ["ban-hang"],
    1: ["ke-toan"]
  };
  const allowedSections = sectionAllowByRole[role] || [];
  document.querySelectorAll("[data-section]").forEach(sec => {
    const name = sec.getAttribute("data-section");
    sec.style.display = allowedSections.includes(name) ? "" : "none";
  });
});