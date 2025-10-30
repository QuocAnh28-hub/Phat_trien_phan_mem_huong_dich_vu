function dangNhap(event) {
  event.preventDefault(); // Ngăn reload trang

  const user = document.getElementById('username').value.trim();
  const pass = document.getElementById('password').value.trim();

  if (!user || !pass) {
    alert("Vui lòng nhập đầy đủ tài khoản và mật khẩu!");
    return;
  }

  // Gọi API đăng nhập qua Gateway
  axios.post(`https://localhost:7107/api-common/Login/login?username=${user}&pass=${pass}`)
    .then(res => {
      console.log("Kết quả API:", res.data);

      if (res.data.success) {
        alert(res.data.message); // "Đăng nhập thành công!"

        // Lưu token và trạng thái
      localStorage.setItem("token", res.data.token);
      localStorage.setItem("isLoggedIn", "true");

      // Lưu user: nếu API không trả user -> vẫn lưu tối thiểu username để hiện lên UI
      const userObj = res.data.user || { username: user };
      localStorage.setItem("user", JSON.stringify(userObj));

        // Gọi thêm API lấy quyền
        return axios.get(`https://localhost:7107/api-common/Login/get-role?username=${user}`);
      } else {
        throw new Error(res.data.message || "Sai thông tin đăng nhập!");
      }
    })
    .then(roleRes => {
      console.log("Dữ liệu quyền:", roleRes.data);

      // Lấy quyền đúng cấu trúc
      const role = parseInt(roleRes.data.data.quyen);

      // Lưu quyền vào localStorage
      localStorage.setItem("role", role);

      // Chuyển hướng theo quyền
      if (role === 4) {
        window.location.href = "../pages/QuanLyTaiKhoan.html";
      } else if (role === 3) {
        window.location.href = "../pages/QuanLyDanhMuc.html";
      } else if (role === 2) {
        window.location.href = "../pages/QuanLyBanHang.html";
      } else if (role === 1) {
        window.location.href = "../pages/QuanLyCongNo.html";
      } else {
        window.location.href = "../pages/index.html";
      }
    })
    .catch(err => {
      console.error("Lỗi khi đăng nhập hoặc lấy quyền:", err);
      alert("Không thể kết nối đến API hoặc tài khoản không hợp lệ!");
    });
}

document.addEventListener("DOMContentLoaded", () => {
  const logoutBtn = document.querySelector(".logout a");

  if (logoutBtn) {
    logoutBtn.addEventListener("click", (event) => {
      event.preventDefault();

      if (confirm("Bạn có chắc muốn đăng xuất không?")) {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        localStorage.removeItem("role");
        localStorage.removeItem("isLoggedIn");
        window.location.href = "../pages/index.html";
      } else {
      }
    });
  }
});

document.addEventListener("DOMContentLoaded", () => {
  // Lấy thông tin người dùng từ localStorage
  const user = JSON.parse(localStorage.getItem("user"));

  // Kiểm tra có user không
  if (user && (user.userName || user.username)) {
    const userName = user.userName || user.username;

    // Tìm đến thẻ <strong> trong quick-stats và thay nội dung
    const strongTag = document.querySelector(".quick-stats .row strong");
    if (strongTag) {
      strongTag.textContent = userName;
    }
  }
});