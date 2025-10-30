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

        // Lưu token và user
        localStorage.setItem("token", res.data.token);
        localStorage.setItem("user", JSON.stringify(res.data.user));

        localStorage.setItem("isLoggedIn", "true");  // trạng thái đăng nhập

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
        // Đổi nội dung trong thẻ strong
        //document.querySelector(".quick-stats .row strong").textContent = "Administrator";
      } else if (role === 3) {
        window.location.href = "../pages/QuanLyDanhMuc.html";
        // Đổi nội dung trong thẻ strong
        //document.querySelector(".quick-stats .row strong").textContent = "Thủ kho";
      } else if (role === 2) {
        window.location.href = "../pages/QuanLyBanHang.html";
        // Đổi nội dung trong thẻ strong
        //document.querySelector(".quick-stats .row strong").textContent = "Thu ngân";
      } else if (role === 1) {
        window.location.href = "../pages/QuanLyCongNo.html";
        // Đổi nội dung trong thẻ strong
        //document.querySelector(".quick-stats .row strong").textContent = "Kế toán";
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
  const logoutBtn = document.getElementsByClassName("logout");

  if (logoutBtn.length > 0) {
    logoutBtn[0].addEventListener("click", () => {
      if (confirm("Bạn có chắc muốn đăng xuất không?")) {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        localStorage.removeItem("role");
        localStorage.removeItem("isLoggedIn"); // xóa trạng thái đăng nhập

        window.location.href = "../pages/index.html";
      }
    });
  }
});
