
const API_URL = "https://localhost:7107/api-thukho/QuanLyDanhMuc";
const token = localStorage.getItem("token");


let currentPage = 1;
const itemsPerPage = 8;
let allCategories = [];

function openAddModal() {
  document.getElementById("addModal").style.display = "flex";
}
function closeAddModal() {
  document.getElementById("addModal").style.display = "none";
  document.getElementById("addCategoryForm").reset();
}



// Đóng popup khi click ra ngoài
window.onclick = function(e) {
  const addModal = document.getElementById("addModal");
  const editModal = document.getElementById("editModal");
  if (e.target === addModal) closeAddModal();
  if (e.target === editModal) closeEditModal();
};




//lấy danh mục khi tải trang
window.onload = function() {

  axios.get(`${API_URL}/get-all-danhmuc`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
  .then(function (response) {
    allCategories = response.data;
    console.log("Danh sách danh mục:", allCategories);

    const tableBody = document.getElementById("category-list");
    if (tableBody) {
      tableBody.innerHTML = "";
      allCategories.forEach(danhmuc => {
        const row = `
          <tr>
            <td>${danhmuc.madanhmuc.trim()}</td>
            <td>${danhmuc.tendanhmuc.trim()}</td>
            <td>${danhmuc.mota.trim() || ""}</td>
            <td>
              <button class="btn-edit" onclick="openEditModal('${danhmuc.madanhmuc.trim()}')">Sửa</button>
              <button class="btn-delete" onclick="deleteCategory('${danhmuc.madanhmuc.trim()}')">Xóa</button>
            </td>
          </tr>`;
        tableBody.innerHTML += row;
      });
    renderTable();
    }
  })
  .catch(function (error) {
    console.error("Lỗi khi tải danh mục:", error);
    alert("Không thể tải danh mục. Kiểm tra token hoặc API.");
  });
};

function renderTable() {
  const start = (currentPage - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  const dataToShow = allCategories.slice(start, end);

  const tableBody = document.getElementById("category-list");
  tableBody.innerHTML = "";

  dataToShow.forEach(danhmuc => {
    const row = `
      <tr>
        <td>${danhmuc.madanhmuc.trim()}</td>
        <td>${danhmuc.tendanhmuc.trim()}</td>
        <td>${danhmuc.mota.trim() || ""}</td>
        <td>
          <button class="btn-edit" onclick="openEditModal('${danhmuc.madanhmuc.trim()}')">Sửa</button>
          <button class="btn-delete" onclick="deleteCategory('${danhmuc.madanhmuc.trim()}')">Xóa</button>
        </td>
      </tr>`;
    tableBody.innerHTML += row;
  });
  renderPagination();
}

function renderPagination() {
  //tính số trang
  const totalPages = Math.ceil(allCategories.length / itemsPerPage);
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

//thêm danh mục
function addCategory(event) {
  event.preventDefault();

  const maDanhmuc = document.getElementById("categoryID").value.trim();
  const tenDanhmuc = document.getElementById("categoryName").value.trim();
  const moTa = document.getElementById("categoryDesc").value.trim();

  const danhmucNew = {
    madanhmuc: maDanhmuc,
    tendanhmuc: tenDanhmuc,
    mota: moTa
  };

  axios.post(`${API_URL}/insert-danhmuc`, danhmucNew, {
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json"
    }
  })
  .then(function (response) {
    alert("Thêm danh mục thành công!");
    closeAddModal();
    window.location.reload(); // tải lại danh sách
  })
  .catch(function (error) {
    console.error("Lỗi khi thêm mới danh mục:", error);
    alert(`Không thể thêm danh mục do trùng mã '${maDanhmuc.trim()}'.`);
  });
}

//sửa
function openEditModal(maDanhmuc) {
  document.getElementById("editModal").style.display = "flex";
  const category = allCategories.find(dm => dm.madanhmuc.trim() === maDanhmuc.trim());
  if (!category) {
    alert("Không tìm thấy danh mục cần sửa!");
    return;
  }

  document.getElementById("editCategoryId").value = category.madanhmuc.trim();
  document.getElementById("editCategoryName").value = category.tendanhmuc.trim();
  document.getElementById("editCategoryDesc").value = category.mota?.trim() || "";
}

function closeEditModal() {
  document.getElementById("editModal").style.display = "none";
  document.getElementById("editCategoryForm").reset();
}


//sửa danh mục
function editCategory(event) {
  event.preventDefault();

  const maDanhmuc = document.getElementById("editCategoryId").value.trim();
  const tenDanhmuc = document.getElementById("editCategoryName").value.trim();
  const moTa = document.getElementById("editCategoryDesc").value.trim();

  const danhmucUpdate = {
    madanhmuc: maDanhmuc,
    tendanhmuc: tenDanhmuc,
    mota: moTa
  };

  axios.put(`${API_URL}/update-danhmuc`, danhmucUpdate, {
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json"
    }
  })
  .then(function (response) {
    alert("Sửa thông tin danh mục thành công!");
    closeEditModal();
    window.location.reload(); // tải lại danh sách
  })
  .catch(function (error) {
    console.error("Lỗi khi thay đổi thông tin danh mục:", error);
    alert("Không thể sửa danh mục. Kiểm tra token hoặc API.");
  });
  
}
//xoá danh mục
function deleteCategory(maDanhmuc) {
  if (!confirm(`Bạn có chắc muốn xóa danh mục '${maDanhmuc}' không?`)) return;

  axios.delete(`${API_URL}/delete-danhmuc?maDanhMuc=${maDanhmuc}`, {
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json"
    }
  })
  .then(function (response) {
    alert("Xóa danh mục thành công!");
    window.location.reload(); // tải lại danh sách
  })
  .catch(function (error) {
    console.error("Lỗi khi xóa danh mục:", error);
    alert("Không thể xóa danh mục. Kiểm tra token hoặc API.");
  });
}




function searchByMaDanhMuc() {
  const maDanhMuc = document.getElementById("searchInput").value.trim();
  if (!maDanhMuc) {
    alert("Vui lòng nhập mã danh mục cần tìm!");
    return;
  }

  axios.get(`${API_URL}/get-byID-danhmuc?madanhmuc=${maDanhMuc}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
  .then(function (response) {
    

    // Vì API trả về MẢNG, nên lấy phần tử đầu tiên
    const danhmuc = response.data[0];
    //console.log("Kết quả API:", response.data);
    if (danhmuc) {
      const tableBody = document.getElementById("category-list");
      tableBody.innerHTML = `
        <tr>
          <td>${danhmuc.madanhmuc.trim()}</td>
          <td>${danhmuc.tendanhmuc.trim()}</td>
          <td>${danhmuc.mota.trim() || ""}</td>
          <td>
            <button class="btn-edit" onclick="openEditModal('${danhmuc.madanhmuc.trim()}')">Sửa</button>
            <button class="btn-delete" onclick="deleteCategory('${danhmuc.madanhmuc.trim()}')">Xóa</button>
          </td>
        </tr>`;
    }
  })
  .catch(function (error) {
    console.error("Lỗi khi tìm danh mục:", error);
    alert("Không tìm thấy danh mục hoặc API bị lỗi.");
  });
}

// Hàm làm mới lại danh sách
function loadAllCategories() {
  axios.get(`${API_URL}/get-all-danhmuc`, {
    headers: { Authorization: `Bearer ${token}` }
  })
  .then(function (response) {
    allCategories = response.data;
    renderTable();
    document.getElementById("searchInput").value = "";
  })
  .catch(function (error) {
    console.error("Lỗi khi tải lại danh mục:", error);
  });
}