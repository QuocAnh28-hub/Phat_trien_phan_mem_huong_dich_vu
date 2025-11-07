
const API_URL = "https://localhost:7107/api-thukho/QuanLyDanhMuc";
const token = localStorage.getItem("token");


let currentPage = 1;
const itemsPerPage = 5;
let allCategories = [];

function openAddModal() {
  document.getElementById("addModal").style.display = "flex";
}
function closeAddModal() {
  document.getElementById("addModal").style.display = "none";
  document.getElementById("addCategoryForm").reset();
}



//lấy danh mục khi tải trang
window.onload = function() {

  $.ajax({
    url: `${API_URL}/get-all-danhmuc`,
    type: 'GET',
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function(data) {
      allCategories = data;
      console.log("Danh sách danh mục:", allCategories);
      renderTable(); 
    },
    error: function() {
      alert("Không thể tải danh mục. Kiểm tra token hoặc API.");
    }
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

// Thêm danh mục
function addCategory(event) {
  event.preventDefault();

  const maDanhmuc = $("#categoryID").val().trim();
  const tenDanhmuc = $("#categoryName").val().trim();
  const moTa = $("#categoryDesc").val().trim();

  const danhmucNew = {
    madanhmuc: maDanhmuc,
    tendanhmuc: tenDanhmuc,
    mota: moTa
  };

  $.ajax({
    url: `${API_URL}/insert-danhmuc`,
    type: "POST",
    data: JSON.stringify(danhmucNew),
    contentType: "application/json",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function () {
      alert("Thêm danh mục thành công!");
      closeAddModal();
      location.reload(); // tải lại danh sách
    },
    error: function (xhr) {
      console.error("Lỗi khi thêm mới danh mục:", xhr);
      alert(`Không thể thêm danh mục.`);
    }
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
  if (!confirm(`Bạn có chắc muốn sửa danh mục '${maDanhmuc}' không?`)) return;
  $.ajax({
    url: `${API_URL}/update-danhmuc`,
    type: "PUT",
    data: JSON.stringify(danhmucUpdate),
    contentType: "application/json",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function (response) {
      alert("Sửa danh mục thành công!");
      closeEditModal();
      location.reload(); // tải lại danh sách
    },
    error: function (xhr) {
      alert(`Không thể thêm sửa danh mục!.`);
    }
  });
  
}
//xoá danh mục 
function deleteCategory(maDanhmuc) {
  if (!confirm(`Bạn có chắc muốn xóa danh mục '${maDanhmuc}' không?`)) return;

  $.ajax({
    url: `${API_URL}/delete-danhmuc?maDanhMuc=${maDanhmuc}`,
    type: "DELETE",
    contentType: "application/json",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function (response) {
      alert("Xoá danh mục thành công!");
      location.reload(); // tải lại danh sách
    },
    error: function (xhr) {
      alert(`Không thể thêm xoá danh mục!.`);
    }
  });
}




function searchByMaDanhMuc() {
  const maDanhMuc = $("#searchInput").val().trim();
  if (!maDanhMuc) {
    alert("Vui lòng nhập mã danh mục cần tìm!");
    return;
  }

  $.ajax({
    url: `${API_URL}/get-byID-danhmuc?madanhmuc=${maDanhMuc}`,
    type: "GET",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function (response) {
      const danhmuc = response[0]; // API trả về mảng
      if (danhmuc) {
        const row = `
          <tr>
            <td>${danhmuc.madanhmuc.trim()}</td>
            <td>${danhmuc.tendanhmuc.trim()}</td>
            <td>${danhmuc.mota?.trim() || ""}</td>
            <td>
              <button class="btn-edit" onclick="openEditModal('${danhmuc.madanhmuc.trim()}')">Sửa</button>
              <button class="btn-delete" onclick="deleteCategory('${danhmuc.madanhmuc.trim()}')">Xóa</button>
            </td>
          </tr>`;
        $("#category-list").html(row);
      } else {
        alert("Không tìm thấy danh mục!");
      }
    },
    error: function (xhr) {
      console.error("Lỗi khi tìm danh mục:", xhr);
      alert("Không tìm thấy danh mục hoặc API bị lỗi.");
    }
  });
}

// Hàm làm mới lại danh sách
function loadAllCategories() {
  $.ajax({
    url: `${API_URL}/get-all-danhmuc`,
    type: "GET",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function (response) {
      allCategories = response;
      renderTable();
      $("#searchInput").val("");
    },
    error: function (xhr) {
      console.error("Lỗi khi tải lại danh mục:", xhr);
    }
  });
}
