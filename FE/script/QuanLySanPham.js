const API_URL = "https://localhost:7107/api-thukho/QuanLySanPham";
const token = localStorage.getItem("token");
const tableBody = document.getElementById("product-list");

let currentPage = 1;
const itemsPerPage = 8;
let allSanPham = [];
let allDanhMuc = [];

function openAdd() {
  document.getElementById("addProduct").style.display = "flex";
}
function closeAdd() {
  document.getElementById("addProduct").style.display = "none";
  document.getElementById("addProductForm").reset();
}


// Hàm làm mới lại danh sách
function loadAllSanPham() {
  $.ajax({
    url: `${API_URL}/get-all-sanpham`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      allSanPham = response;
      renderTable();
      $("#searchInput").val("");
    },
    error: function (xhr) {
      console.error("Lỗi khi tải lại sản phẩm:", xhr);
    }
  });
}

// Tìm sản phẩm theo mã
function searchByMaSanPham() {
  const masp = $("#searchInput").val().trim();
  if (!masp) {
    alert("Vui lòng nhập mã sản phẩm cần tìm!");
    return;
  }

  $.ajax({
    url: `${API_URL}/get-sanpham-by-id?id=${masp}`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      const sp = response[0]; // API trả về mảng
      if (sp) {
        const row = `
          <tr>
            <td>${sp.masp.trim()}</td>
            <td>${sp.tensp.trim()}</td>
            <td>${sp.mavach?.trim() || ""}</td>
            <td>${sp.mota?.trim() || ""}</td>
            <td>${sp.madanhmuc.trim()}</td>
            <td>${sp.dongia}</td>
            <td>${sp.thuoctinh?.trim() || ""}</td>
            <td>${sp.thue}</td>
            <td>${sp.soluongton}</td>
            <td>
              <button class="btn-edit" onclick="openEditProduct('${sp.masp.trim()}')">Sửa</button>
              <button class="btn-delete" onclick="deleteProduct('${sp.masp.trim()}')">Xóa</button>
            </td>
          </tr>`;
        $("#product-list").html(row);
      } else {
        alert("Không tìm thấy sản phẩm!");
      }
    },
    error: function (xhr) {
      console.error("Lỗi khi tìm sản phẩm:", xhr);
      alert("Không tìm thấy sản phẩm hoặc API bị lỗi.");
    }
  });
}

// Lấy sản phẩm khi tải trang
window.onload = function () {
  // Tải danh mục trước
  $.ajax({
    url: `https://localhost:7107/api-thukho/QuanLyDanhMuc/get-all-danhmuc`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (data) {
      allDanhMuc = data;

      const selectAdd = $("#CategoryID");
      const selectEdit = $("#editCategoryID");

      selectAdd.empty().append(`<option value="">-- Chọn danh mục --</option>`);
      selectEdit.empty().append(`<option value="">-- Chọn danh mục --</option>`);

      data.forEach(dm => {
        const option = `<option value="${dm.madanhmuc.trim()}">${dm.tendanhmuc.trim()}</option>`;
        selectAdd.append(option);
        selectEdit.append(option);
      });


      // Sau khi có danh mục thì mới tải sản phẩm
      $.ajax({
        url: `${API_URL}/get-all-sanpham`,
        type: "GET",
        headers: { Authorization: `Bearer ${token}` },
        success: function (response) {
          allSanPham = response;
          console.log("Danh mục:", allDanhMuc);
          console.log("Sản phẩm:", allSanPham);

          const tableBody = $("#product-list");
          tableBody.empty();

          allSanPham.forEach(sp => {
            const dm = allDanhMuc.find(d => d.madanhmuc.trim() === sp.madanhmuc.trim());
            const tenDanhMuc = dm ? dm.tendanhmuc.trim() : "Không xác định";
            const row = `
              <tr>
                <td>${sp.masp.trim()}</td>
                <td>${sp.tensp.trim()}</td>
                <td>${tenDanhMuc}</td>
                <td>${sp.mavach?.trim() || ""}</td>
                <td>${sp.mota?.trim() || ""}</td>
                
                <td>${sp.dongia}</td>
                <td>${sp.thuoctinh?.trim() || ""}</td>
                <td>${sp.thue}</td>
                <td>${sp.soluongton}</td>
                <td>
                  <button class="btn-edit" onclick="openEditProduct('${sp.masp.trim()}')">Sửa</button>
                  <button class="btn-delete" onclick="deleteProduct('${sp.masp.trim()}')">Xóa</button>
                </td>
              </tr>`;
            tableBody.append(row);
          });

          renderTable();
        },
        error: function (xhr) {
          console.error("Lỗi khi tải sản phẩm:", xhr);
          alert("Không thể tải sản phẩm. Kiểm tra token hoặc API.");
        }
      });
    },
    error: function (xhr) {
      console.error("Lỗi khi tải danh mục:", xhr);
      alert("Không thể tải danh mục!");
    }
  });
};



function renderTable() {
  const start = (currentPage - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  const dataToShow = allSanPham.slice(start, end);

  const tableBody = document.getElementById("product-list");
  tableBody.innerHTML = "";

  dataToShow.forEach(sp => {
    const dm = allDanhMuc.find(d => d.madanhmuc.trim() === sp.madanhmuc.trim());
    const tenDanhMuc = dm ? dm.tendanhmuc.trim() : "Không xác định";
    const row = `
        <tr>
            <td>${sp.masp.trim()}</td>
            <td>${sp.tensp.trim()}</td>
            <td>${tenDanhMuc.trim()}</td>
            <td>${sp.mavach.trim()}</td>
            <td>${sp.mota.trim() || ""}</td>
            
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


//thêm sản phẩm
function addSanPham(event) {
  event.preventDefault();

  const masanpham = document.getElementById("ProductID").value.trim();
  const tensanpham = document.getElementById("ProductName").value.trim();
  const mavach = document.getElementById("Barcode").value.trim();
  const mota = document.getElementById("ProductDesc").value.trim();
  const madanhmuc = $("#CategoryID").val().trim();
  const dongia = Number(document.getElementById("Price").value);
  const thuoctinh = document.getElementById("Attributes").value.trim();
  const thueVAT = Number(document.getElementById("VAT").value);
  const soluongton = Number(document.getElementById("Stock").value);


  const newProduct = {
    masp: masanpham,     
    tensp: tensanpham,
    mavach: mavach,
    mota: mota,
    madanhmuc: madanhmuc,
    dongia: dongia,
    thuoctinh: thuoctinh,
    thue: thueVAT,     
    soluongton: soluongton
  };
  console.log("Dữ liệu gửi lên API:", newProduct);
  $.ajax({
    url: `${API_URL}/insert-sanpham`,
    type: "POST",
    data: JSON.stringify(newProduct),
    contentType: "application/json",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function () {
      alert("Thêm sản phẩm thành công!");
      closeAdd();
      location.reload();
    },
    error: function () {
      alert("Thêm sản phẩm thất bại!");
    }
  })
}

//sửa
function openEditProduct(masanpham) {
  
  document.getElementById("editProduct").style.display = "flex";
  const sanpham = allSanPham.find(sp => sp.masp.trim() === masanpham.trim());
  if (!sanpham) {
    alert("Không tìm thấy sản phẩm cần sửa!");
    return;
  }
  document.getElementById("editProductId").value = sanpham.masp.trim();
  document.getElementById("editProductName").value = sanpham.tensp.trim();
  document.getElementById("editBarcode").value = sanpham.mavach.trim();
  document.getElementById("editProductDesc").value= sanpham.mota.trim();
  $("#editCategoryID").val(sanpham.madanhmuc.trim());
  document.getElementById("editPrice").value = sanpham.dongia;
  document.getElementById("editAttributes").value = sanpham.thuoctinh.trim();
  document.getElementById("editVAT").value = sanpham.thue;
  document.getElementById("editStock").value = sanpham.soluongton;
}

function closeEditProduct() {
  document.getElementById("editProduct").style.display = "none";
  document.getElementById("editProductForm").reset();
}
//SỬA
function editSanPham(event) {
  event.preventDefault();

  const masanpham = document.getElementById("editProductId").value.trim();
  const tensanpham = document.getElementById("editProductName").value.trim();
  const mavach = document.getElementById("editBarcode").value.trim();
  const mota = document.getElementById("editProductDesc").value.trim();
  const madanhmuc = $("#editCategoryID").val().trim();
  const dongia = Number(document.getElementById("editPrice").value);
  const thuoctinh = document.getElementById("editAttributes").value.trim();
  const thueVAT = Number(document.getElementById("editVAT").value);
  const soluongton = Number(document.getElementById("editStock").value);

  const editProduct = {
    masp: masanpham,     
    tensp: tensanpham,
    mavach: mavach,
    mota: mota,
    madanhmuc: madanhmuc,
    dongia: dongia,
    thuoctinh: thuoctinh,
    thue: thueVAT,     
    soluongton: soluongton
  };
  //console.log("Dữ liệu gửi lên API:", editProduct);
  if (!confirm(`Bạn có chắc muốn sửa sản phẩm '${masanpham}' không?`)) return;
  $.ajax({
    url: `${API_URL}/update-sanpham`,
    type: "PUT",
    data: JSON.stringify(editProduct),
    contentType: "application/json",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function () {
      alert("Sửa sản phẩm thành công!");
      closeEditProduct();
      location.reload();
    },
    error: function () {
      alert("Sửa sản phẩm thất bại!");
    }
  })
}

//xoá sản phẩm 
function deleteProduct(masanpham) {
  if (!confirm(`Bạn có chắc muốn xoá sản phẩm '${masanpham}' không?`)) return;

  $.ajax({
    url: `${API_URL}/delete-sanpham?masp=${masanpham}`,
    type: "DELETE",
    contentType: "application/json",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function (response) {
      alert("Xoá sản phẩm thành công!");
      location.reload(); // tải lại danh sách
    },
    error: function (xhr) {
      alert(`Không thể thêm xoá sản phẩm!.`);
    }
  });
}
