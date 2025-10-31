const API_URL = "https://localhost:7107/api-thukho/QuanLySanPham";
const token = localStorage.getItem("token");
const tableBody = document.getElementById("product-list");

let currentPage = 1;
const itemsPerPage = 8;
let allSanPham = [];

function openAdd() {
  document.getElementById("addProduct").style.display = "flex";
}
function closeAdd() {
  document.getElementById("addProduct").style.display = "none";
  document.getElementById("addProductForm").reset();
}



// Đóng popup khi click ra ngoài
window.onclick = function(e) {
  const addProduct = document.getElementById("addProduct");
  const editProduct = document.getElementById("editProduct");
  if (e.target === addProduct) closeAddProduct();
  if (e.target === editProduct) closeEditProduct();
};

// Hàm làm mới lại danh sách
function loadAllSanPham() {
  axios.get(`${API_URL}/get-all-sanpham`, {
    headers: { Authorization: `Bearer ${token}` }
  })
  .then(function (response) {
    allSanPham = response.data;
    renderTable();
    document.getElementById("searchInput").value = "";
  })
  .catch(function (error) {
    console.error("Lỗi khi tải lại sản phẩm:", error);
  });
}

function searchByMaSanPham() {
  const masp = document.getElementById("searchInput").value.trim();
  if (!masp) {
    alert("Vui lòng nhập mã sản phẩm cần tìm!");
    return;
  }

  axios.get(`${API_URL}/get-sanpham-by-id?id=${masp}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
  .then(function (response) {
    

    // Vì API trả về MẢNG, nên lấy phần tử đầu tiên
    const sp = response.data[0];
    //console.log("Kết quả API:", response.data);
    if (sp) {
      const tableBody = document.getElementById("product-list");
      tableBody.innerHTML = `
        <tr>
            <td>${sp.masp.trim()}</td>
            <td>${sp.tensp.trim()}</td>
            <td>${sp.mavach.trim()}</td>
            <td>${sp.mota.trim() || ""}</td>
            <td>${sp.madanhmuc.trim()}</td>
            <td>${sp.dongia}</td>
            <td>${sp.thuoctinh.trim() || ""}</td>
            <td>${sp.thue}</td>
            <td>${sp.soluongton}</td>
            <td>
              <button class="btn-edit" onclick="openEditProduct('${sp.masp.trim()}')">Sửa</button>
              <button class="btn-delete" onclick="deleteProduct('${sp.masp.trim()}')">Xóa</button>
            </td>
          </tr>`;
    }
  })
  .catch(function (error) {
    console.error("Lỗi khi tìm sản phẩm:", error);
    alert("Không tìm thấy sản phẩm hoặc API bị lỗi.");
  });
}

//lấy sản phẩm khi tải trang
window.onload = function() {
  axios.get(`${API_URL}/get-all-sanpham`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
  .then(function (response) {
    allSanPham = response.data;
    console.log("Danh sách sản phẩm:", allSanPham);
    const tableBody = document.getElementById("product-list");
    if (tableBody) {
      tableBody.innerHTML = "";
      allSanPham.forEach(sp => {
        const row = `
          <tr>
            <td>${sp.masp.trim()}</td>
            <td>${sp.tensp.trim()}</td>
            <td>${sp.mavach.trim()}</td>
            <td>${sp.mota.trim() || ""}</td>
            <td>${sp.madanhmuc.trim()}</td>
            <td>${sp.dongia}</td>
            <td>${sp.thuoctinh.trim() || ""}</td>
            <td>${sp.thue}</td>
            <td>${sp.soluongton}</td>
            <td>
              <button class="btn-edit" onclick="openEditProduct('${sp.masp.trim()}')">Sửa</button>
              <button class="btn-delete" onclick="deleteProduct('${sp.masp.trim()}')">Xóa</button>
            </td>
          </tr>`;
        tableBody.innerHTML += row;
      });
    renderTable();
    }
  })
  .catch(function (error) {
    console.error("Lỗi khi tải sản phẩm:", error);
    alert("Không thể tải sản phẩm. Kiểm tra token hoặc API.");
  });
};

function renderTable() {
  const start = (currentPage - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  const dataToShow = allSanPham.slice(start, end);

  const tableBody = document.getElementById("product-list");
  tableBody.innerHTML = "";

  dataToShow.forEach(sp => {
    const row = `
        <tr>
            <td>${sp.masp.trim()}</td>
            <td>${sp.tensp.trim()}</td>
            <td>${sp.mavach.trim()}</td>
            <td>${sp.mota.trim() || ""}</td>
            <td>${sp.madanhmuc.trim()}</td>
            <td>${sp.dongia}</td>
            <td>${sp.thuoctinh.trim() || ""}</td>
            <td>${sp.thue}</td>
            <td>${sp.soluongton}</td>
            <td>
              <button class="btn-edit" onclick="openEditProduct('${sp.masp.trim()}')">Sửa</button>
              <button class="btn-delete" onclick="deleteProduct('${sp.masp.trim()}')">Xóa</button>
            </td>
          </tr>`;
    tableBody.innerHTML += row;
  });
  renderPagination();
}

function renderPagination() {
  //tính số trang
  const totalPages = Math.ceil(allSanPham.length / itemsPerPage);
  const pagination = document.getElementById("pagination");
  pagination.innerHTML = "";

  for (let i = 1; i <= totalPages; i++) {
    const btn = document.createElement("button");
    btn.innerText = i;
    //gán class active cho trang hiện tại
    btn.className = i === currentPage ? "active" : "";
    btn.onclick = function () {
      currentPage = i;
      renderTable();
    };
    pagination.appendChild(btn);
  }
}
