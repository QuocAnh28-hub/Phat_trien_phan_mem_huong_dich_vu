
function openAddModal() {
  document.getElementById("addModal").style.display = "flex";
}
function closeAddModal() {
  document.getElementById("addModal").style.display = "none";
  document.getElementById("addCategoryForm").reset();
}

function openEditModal(id) {
  document.getElementById("editModal").style.display = "flex";
}
function closeEditModal() {
  document.getElementById("editModal").style.display = "none";
  document.getElementById("editCategoryForm").reset();
}

// Đóng popup khi click ra ngoài
window.onclick = function(e) {
  const addModal = document.getElementById("addModal");
  const editModal = document.getElementById("editModal");
  if (e.target === addModal) closeAddModal();
  if (e.target === editModal) closeEditModal();
};

window.onload = function() {
  const token = localStorage.getItem("token");

  axios.get(`https://localhost:7107/api-thukho/QuanLyDanhMuc/get-all-danhmuc`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
  .then(function (response) {
    const data = response.data;
    console.log("Danh sách danh mục:", data);


    //lấy danh mục từ localStorage và đưa vào bảng
    const tableBody = document.getElementById("category-list");
    if (tableBody) {
      tableBody.innerHTML = "";
      data.forEach(danhmuc => {
        const row = `
          <tr>
            <td>${danhmuc.madanhmuc}</td>
            <td>${danhmuc.tendanhmuc}</td>
            <td>${danhmuc.mota || ""}</td>
            <td>
              <button class="btn-edit" onclick="openEditModal('${danhmuc.madanhmuc}')">Sửa</button>
              <button class="btn-delete" onclick="deleteCategory('${danhmuc.madanhmuc}')">Xóa</button>
            </td>
          </tr>`;
        tableBody.innerHTML += row;
      });
    }

  })
  .catch(function (error) {
    console.error("Lỗi khi tải danh mục:", error);
    alert("Không thể tải danh mục. Kiểm tra token hoặc API.");
  });
};


