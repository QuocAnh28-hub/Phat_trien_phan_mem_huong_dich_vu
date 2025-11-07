const API_URL = "https://localhost:7107/api-thukho/QuanLyTonKho";
const token = localStorage.getItem("token");
const tableBody = document.getElementById("product-list");


let currentPage = 1;
const itemsPerPage = 5;
let allSanPham = [];

// Hàm làm mới lại danh sách
function loadAllSanPham() {
  $.ajax({
    url: `${API_URL}/get-all-sanpham`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      allSanPham = response;
      renderTable();
    },
    error: function (xhr) {
      console.error("Lỗi khi tải lại sản phẩm:", xhr);
    }
  });
}


// Lấy sản phẩm khi tải trang
window.onload = function () {
  $.ajax({
    url: `${API_URL}/get-all-sanpham`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      allSanPham = response;
      console.log("Sản phẩm:", allSanPham);

      const tableBody = $("#product-list");
      tableBody.empty();

      renderTable();
    },
    error: function (xhr) {
      console.error("Lỗi khi tải sản phẩm:", xhr);
      alert("Không thể tải sản phẩm. Kiểm tra token hoặc API.");
    }
  });
}



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
            
            <td>${sp.dongia}</td>
            <td>${sp.thuoctinh.trim() || ""}</td>
            <td>${sp.thue}</td>
            <td>${sp.soluongton}</td>
            <td>
              <button class="btn-edit" onclick="dieuchitonkho('${sp.masp.trim()}')">Điều chỉnh</button>
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

function dieuchitonkho(masanpham) {
  document.getElementById("adjustModal").style.display = "flex";

  const sanpham = allSanPham.find(sp => sp.masp.trim() === masanpham.trim());
  if (!sanpham) {
    alert("Không tìm thấy sản phẩm cần điều chỉnh số lượng!");
    return;
  }

  document.getElementById("adjustProductId").value = sanpham.masp.trim();
  document.getElementById("adjustProductName").value = sanpham.tensp.trim();
}
function closeEditTonKho() {
  document.getElementById("adjustModal").style.display = "none";
  document.getElementById("adjustForm").reset();
}

function apdungdieuchinh(event){
  event.preventDefault();
  const masp = document.getElementById("adjustProductId").value.trim();
  const sanpham = allSanPham.find(sp => sp.masp.trim() === masp.trim());
  
  const soluongcu = Number(sanpham.soluongton);
  let soluongcapnhat = 0;
  
  const soluongmoi = Number(document.getElementById("adjustQuantity").value);
  const loaidieuchinh = document.getElementById("adjustType").value.trim();

  if(loaidieuchinh === "increase"){
    soluongcapnhat = soluongcu + soluongmoi;
  }
  else if (loaidieuchinh === "decrease"){
    if(soluongcu < soluongmoi){
      alert("Không thể giảm nhiều hơn số lượng hiện tại!");
      return;
    }
    soluongcapnhat = soluongcu - soluongmoi;
  }
  console.log("Dữ liệu gửi lên API:", soluongcapnhat);
  $.ajax({
    url: `${API_URL}/update-soluong-sanpham?maSP=${masp}&soLuongMoi=${soluongcapnhat}`,
    type: "POST",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      alert("Cập nhật tồn kho thành công!");
      allSanPham = response;
      closeEditTonKho();
      renderTable();
    },
    error: function (xhr) {
      console.error("Lỗi khi cập nhật số lượng sản phẩm:", xhr);
      alert("Cập nhật thất bại!");
    }
  });
}