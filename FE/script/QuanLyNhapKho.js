const API_URL = "https://localhost:7107/api-thukho/QuanLyNhapKho";
const token = localStorage.getItem("token");
const tableBody = document.getElementById("receipt-list");


let allPhieuNhap = [];
let allCTPhieuNhap = [];
let allNhaCungCap = [];
let currentPage = 1;
const itemsPerPage = 8;
// Format tiền tệ
function formatCurrency(amount) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount);
}

// Hàm làm mới lại danh sách
function loadAllPhieuNhap() {
  $.ajax({
    url: `${API_URL}/get-all-phieunhapkho`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      allPhieuNhap = response.data;
      $("#searchInput").val("");
      renderTable();
    },
    error: function (xhr) {
      console.error("Lỗi khi tải lại phiếu nhập:", xhr);
    }
  });
}

function renderTable() {
  const start = (currentPage - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  const dataToShow = allPhieuNhap.slice(start, end);

  const tableBody = document.getElementById("receipt-list");
  tableBody.innerHTML = "";

  dataToShow.forEach(pn => {
    const row = `
          <tr>
            <td>${pn.maphieunhap.trim()}</td>
            <td>${pn.mancc.trim()}</td>
            <td>${pn.manv.trim()}</td>
            <td>${new Date(pn.ngaylap).toLocaleDateString("vi-VN")}</td>
            <td>
              <button class="btn-view" onclick="viewCTPhieuNhap('${pn.maphieunhap.trim()}')">Xem chi tiết</button>
              <button class="btn-edit" onclick="openEditPhieuNhap('${pn.maphieunhap.trim()}')">Sửa</button>
              <button class="btn-delete" onclick="deleteReceipt('${pn.maphieunhap.trim()}')">Xóa</button>
            </td>
          </tr>`;
    tableBody.innerHTML += row;
  });
  renderPagination();
}

function renderPagination() {
  //tính số trang
  const totalPages = Math.ceil(allPhieuNhap.length / itemsPerPage);
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


// Tìm phiếu nhập theo mã
function searchByMaPhieuNhap() {
  const maphieunhap = $("#searchInput").val().trim();
  if (!maphieunhap) {
    alert("Vui lòng nhập mã phiếu nhập cần tìm!");
    return;
  }

  $.ajax({
    url: `${API_URL}/get-byid-phieunhapkho?maphieunhap=${maphieunhap}`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      const phieunhap = response.data;

      if (!phieunhap || phieunhap.length === 0) {
        alert("Không tìm thấy phiếu nhập.");
        return;
      }

      let rows = "";
      phieunhap.forEach(pn => {
        rows += `
          <tr>
            <td>${pn.maphieunhap}</td>
            <td>${pn.mancc}</td>
            <td>${pn.manv}</td>
            <td>${new Date(pn.ngaylap).toLocaleDateString("vi-VN")}</td>
            <td>
              <button class="btn-view" onclick="viewCTPhieuNhap('${pn.maphieunhap}')">Xem chi tiết</button>
              <button class="btn-edit" onclick="openEditPhieuNhap('${pn.maphieunhap}')">Sửa</button>
              <button class="btn-delete" onclick="deleteReceipt('${pn.maphieunhap}')">Xóa</button>
            </td>
          </tr>`;
      });

      $("#receipt-list").html(rows);
    },
    error: function (xhr) {
      console.error("Lỗi khi tìm phiếu nhập:", xhr);
      alert("Không tìm thấy phiếu nhập hoặc API bị lỗi.");
    }
  });
}


window.onload = function () {
  $.ajax({
    url: `${API_URL}/get-all-phieunhapkho`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      allPhieuNhap = response.data;
      console.log("Phiếu nhập:", allPhieuNhap);
      const tableBody = $("#receipt-list");
      tableBody.empty();

      allPhieuNhap.forEach(pn => {
        const row = `
          <tr>
            <td>${pn.maphieunhap.trim()}</td>
            <td>${pn.mancc.trim()}</td>
            <td>${pn.manv.trim()}</td>
            <td>${new Date(pn.ngaylap).toLocaleDateString("vi-VN")}</td>
            <td>
              <button class="btn-view" onclick="viewCTPhieuNhap('${pn.maphieunhap.trim()}')">Xem chi tiết</button>
              <button class="btn-edit" onclick="openEditPhieuNhap('${pn.maphieunhap.trim()}')">Sửa</button>
              <button class="btn-delete" onclick="deleteReceipt('${pn.maphieunhap.trim()}')">Xóa</button>
            </td>
          </tr>`;

        tableBody.append(row);
        renderTable();
      });
    },
    error: function (xhr) {
      console.error("Lỗi khi tải Phiếu nhập:", xhr);
      alert("Không thể tải Phiếu nhập. Kiểm tra token hoặc API.");
    }
  });
}




function viewCTPhieuNhap(maphieunhap) {
  const phieunhap = allPhieuNhap.find(pn => pn.maphieunhap.trim() === maphieunhap)
  if (!phieunhap) {
    alert("Không tìm thấy phiếu nhập cần xem chi tiết");
    return;
  }
  const tableBody = document.getElementById("detail-table-body");
  tableBody.innerHTML = "";
  const chitiet = phieunhap.listjson_chitietnhap;

  if (!chitiet.length || chitiet.length === 0) {
    alert("Không có chi tiết phiếu nhập!");
    return;
  }
  else {
    chitiet.forEach(pn => {
      const row = `
                <tr>
                  <td>${pn.masp}</td>
                  <td>${pn.soluong}</td>
                  <td>${formatCurrency(pn.dongianhap)}</td>
                  <td>${formatCurrency(pn.thanhtien)}</td>
                  <td><button class="btn-secondary" onclick="closeviewCTPhieuNhap('${pn.maphieunhap.trim()}')">Đóng</button></td>
                </tr>`;
      tableBody.innerHTML += row;
    });
  }


  document.getElementById('receipt-details').style.display = 'block';


}

function closeviewCTPhieuNhap() {
  document.getElementById('receipt-details').style.display = 'none';
}


function openAddPhieuNhap() {
  document.getElementById('addPhieuNhap').style.display = 'block';
}
function closeAddPhieuNhap() {
  document.getElementById('addPhieuNhap').style.display = 'none';
  document.getElementById('addPhieuNhapForm').reset();
}
//thêm phiếu nhập
function addPhieuNhap(event) {
  event.preventDefault();

  const maphieunhap = document.getElementById("PhieuNhapID").value.trim();
  const manhacungcap = document.getElementById("NhaCungCapID").value.trim();
  const manhanvien = document.getElementById("NguoiNhap").value.trim();
  const ngaynhap = document.getElementById("NgayNhap").value.trim();

  const masanpham = document.getElementById("maSP").value.trim();
  const soluong = Number(document.getElementById("soLuong").value.trim());
  const dongia = Number(document.getElementById("donGia").value.trim());
  const thanhtien = soluong * dongia;

  //chuyển ngày sang ISO dạng bên backend nhận
  const ngaylapISO = `${ngaynhap}T00:00:00.000Z`; // yyyy-mm-dd

  const newPhieuNhap = {
    maphieunhap: maphieunhap,
    mancc: manhacungcap,
    manv: manhanvien,
    ngaylap: ngaylapISO,
    listjson_chitietnhap: [
      {
        maphieunhap: maphieunhap,
        masp: masanpham,
        soluong: soluong,
        dongianhap: dongia,
        thanhtien: thanhtien
      }
    ]
  };
  console.log("Dữ liệu gửi lên API:", newPhieuNhap);
  $.ajax({
    url: `${API_URL}/create-phieunhapkho`,
    type: "POST",
    data: JSON.stringify(newPhieuNhap),
    contentType: "application/json",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function (response) {
      console.log(response)
      if (response.success === false) {
        alert("Thêm phiếu nhập thất bại: " + (response.message || "Lỗi không xác định."));
        return;
      }
      alert("Thêm phiếu nhập thành công!");
      closeAddPhieuNhap();
      //location.reload();
      loadAllPhieuNhap(); // tải lại danh sách thay vì reload toàn trang
    },
    error: function (xhr) {
      console.error("Lỗi khi thêm phiếu nhập:", xhr.responseText);
      alert("Thêm phiếu nhập thất bại! Kiểm tra console để xem chi tiết.");
    }
  })
}


function addChiTietRow() {
  const tbody = document.getElementById('chiTietBody');
  const newRow = `
    <tr>
      <td><input type="text" class="maSP" required></td>
      <td><input type="text" class="tenSP" required></td>
      <td><input type="number" class="soLuong" min="1" required"></td>
      <td><input type="number" class="donGia" min="0" required"></td>
      <td><button type="button" class="btn-delete" onclick="removeChiTietRow(this)">Xóa</button></td>
    </tr>
  `;
  tbody.insertAdjacentHTML('beforeend', newRow);
}

function removeChiTietRow(button) {
  button.closest('tr').remove();
}

function openEditPhieuNhap(id) {
  document.getElementById("editPhieuNhap").style.display = "flex";
}
function closeEditPhieuNhap() {
  document.getElementById("editPhieuNhap").style.display = "none";
  document.getElementById("editPhieuNhapForm").reset();
}

