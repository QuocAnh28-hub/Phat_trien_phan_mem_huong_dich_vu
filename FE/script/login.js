function dangNhap(event) {
  event.preventDefault(); // Chặn reload trang

  const user = document.getElementById('username').value.trim();
  const pass = document.getElementById('password').value.trim();

  if (!user || !pass) {
    alert("Vui lòng nhập đầy đủ tài khoản và mật khẩu!");
    return;
  }
  //https://localhost:7107/api-common/Login/login?username=user1&pass=pass1
  // ✅ Gọi API qua Gateway
  axios.post(`https://localhost:7107/api-common/Login/login?username=${user}&pass=${pass}`)
  .then(res => {
    console.log("Kết quả API:", res.data);

    if (res.data.success) {
      alert(res.data.message); // "Đăng nhập thành công!"

      // ✅ Lưu token và thông tin người dùng
      localStorage.setItem("token", res.data.token);
      localStorage.setItem("user", JSON.stringify(res.data.user));

      // ✅ Chuyển hướng sang trang quản lý
      window.location.href = "../pages/QuanLyTaiKhoan.html";
    } else {
      alert(res.data.message || "Sai thông tin đăng nhập!");
    }
  })
  .catch(err => {
    console.error("❌ Lỗi khi gọi API:", err);
    alert("Không thể kết nối đến API hoặc lỗi mạng!");
  });
}