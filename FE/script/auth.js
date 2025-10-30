document.addEventListener("DOMContentLoaded", () => {
  const isLoggedIn = localStorage.getItem("isLoggedIn");
  const token = localStorage.getItem("token");

  if (!isLoggedIn || !token) {
    window.location.href = "../pages/index.html";
    return;
  }

  const role = parseInt(localStorage.getItem("role"));
  const currentPage = window.location.pathname;

  // Quy định phân quyền
  const roleAccess = {
    4: ["QuanLyTaiKhoan", "QuanLyNhanVien", "QuanLyKhuyenMai", "QuanLyCongNo", "BaoCaoThongKe", "QuanLyBanHang", "XuLyDoiTra", "QuanLyDanhMuc", "QuanLySanPham", "QuanLyNhapKho", "QuanLyTonKho"],
    3: ["QuanLyDanhMuc", "QuanLySanPham", "QuanLyNhapKho", "QuanLyTonKho"],
    2: ["QuanLyBanHang", "XuLyDoiTra"],
    1: ["QuanLyCongNo", "BaoCaoThongKe"]
  };

  let hasAccess = false;
  Object.keys(roleAccess).forEach(r => {
    if (parseInt(r) === role) {
      roleAccess[r].forEach(page => {
        if (currentPage.includes(page)) hasAccess = true;
      });
    }
  });

  if (!hasAccess && !currentPage.includes("index")) {
    alert("Bạn không có quyền truy cập trang này!");
    window.location.href = "../pages/index.html";
  }
});
