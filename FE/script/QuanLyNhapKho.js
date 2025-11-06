const API_URL = "https://localhost:7107/api-thukho/QuanLyNhapKho";
const API_URL_admin = "https://localhost:7107/api-admin/QuanLyNhanVien";
const API_URL_SANPHAM = "https://localhost:7107/api-thukho/QuanLySanPham";
const token = localStorage.getItem("token");
const tableBody = document.getElementById("receipt-list");


let allPhieuNhap = [];
let allCTPhieuNhap = [];
let allNhaCungCap = [];
let allSanPham = [];

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
              <button class="btn-view" onclick="viewCTPhieuNhap('${pn.maphieunhap.trim()}')">Xem</button>
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
              <button class="btn-view" onclick="viewCTPhieuNhap('${pn.maphieunhap}')">Xem</button>
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

      renderTable();
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
  const h3 = document.getElementById("tieudechitiet");
  h3.innerHTML = "Chi tiết phiếu nhập " + maphieunhap;
  const tableBody = document.getElementById("detail-table-body");
  tableBody.innerHTML = "";
  const chitiet = phieunhap.listjson_chitietnhap;

  if (!chitiet.length || chitiet.length === 0) {
    alert("Không có chi tiết phiếu nhập!");
    closeEditPhieuNhap();
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

function collectChiTietNhap() {
  const rows = document.querySelectorAll("#chiTietTable tr");
  const maphieunhap = document.getElementById("PhieuNhapID").value.trim();
  const chitiet = [];

  rows.forEach(row => {
    const masp = row.querySelector(".maSP")?.value.trim();
    const soluong = Number(row.querySelector(".soLuong")?.value.trim());
    const dongia = Number(row.querySelector(".donGia")?.value.trim());
    const thanhtien = soluong * dongia;

    if (masp) { // chỉ lấy nếu có mã sản phẩm
      chitiet.push({
        maphieunhap,
        masp,
        soluong,
        dongianhap: dongia,
        thanhtien
      });
    }
  });

  return chitiet;
}
function loadNhaCungCap() {
  $.ajax({
    url: `${API_URL}/get-all-nhacungcap`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      const select = document.getElementById("NhaCungCapID");
      const selectEdit = document.getElementById("editNhaCungCapID");
      select.innerHTML = '<option value="">-- Chọn nhà cung cấp --</option>';
      selectEdit.innerHTML = '<option value="">-- Chọn nhà cung cấp --</option>';
      //console.log(response.data);
      response.data.forEach(ncc => {
        const option = document.createElement("option");
        option.value = ncc.mancc;
        option.textContent = ncc.tenncc;
        select.appendChild(option);
        selectEdit.appendChild(option.cloneNode(true));
      });
    },
    error: function (xhr) {
      console.error("Lỗi khi tải nhà cung cấp:", xhr.responseText);
    }
  });
}

function loadNhanVien() {
  $.ajax({
    url: `${API_URL_admin}/get-all-nhanvien`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      const select = document.getElementById("NguoiNhap");
      select.innerHTML = '<option value="">-- Chọn người nhập --</option>';
      const selectedit = document.getElementById("editNguoiNhap");
      selectedit.innerHTML = '<option value="">-- Chọn người nhập --</option>';
      response.data.forEach(nv => {
        const option = document.createElement("option");
        option.value = nv.manv;
        option.textContent = nv.tennv;
        select.appendChild(option);
        selectedit.appendChild(option.cloneNode(true));
      });
    },
    error: function (xhr) {
      console.error("Lỗi khi tải nhân viên:", xhr.responseText);
    }
  });
}
// Khai báo một biến toàn cục mới để lưu chuỗi HTML options
let sanPhamOptionsHTML = '<option value="">-- Chọn sản phẩm --</option>';
function loadAllSanPham() {
  $.ajax({
    url: `${API_URL_SANPHAM}/get-all-sanpham`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      // Dữ liệu trả về có thể nằm trong response hoặc response.data
      allSanPham = response.data || response;
      console.log("Sản phẩm: ", allSanPham);
      // Tạo danh sách <option> một lần
      let options = '<option value="">-- Chọn sản phẩm --</option>';
      allSanPham.forEach(sp => {
        const masp = (sp.masp || "").trim();
        const tensp = (sp.tensp || "").trim();
        if (masp !== "") {
          options += `<option value="${masp}">${masp} - ${tensp}</option>`;
        }
      });
      sanPhamOptionsHTML = options;
    },
    error: function (xhr) {
      console.error("Lỗi khi tải sản phẩm:", xhr.responseText);
    }
  });
}
function loadChiTietPhieu() {
  $.ajax({
    url: `${API_URL}/get-all-chitietnhap`,
    type: "GET",
    headers: { Authorization: `Bearer ${token}` },
    success: function (response) {
      allCTPhieuNhap = response.data;
      console.log("Phiếu nhập chi tiết:", allCTPhieuNhap);
    },
    error: function (xhr) {
      console.error("Lỗi khi tải chi tiết phiếu:", xhr.responseText);
    }
  });
}

// Gọi khi trang load
$(document).ready(function () {
  loadNhaCungCap();
  loadNhanVien();
  loadAllSanPham();
  loadChiTietPhieu();
});


//thêm phiếu nhập
function addPhieuNhap(event) {
  event.preventDefault();

  const maphieunhap = document.getElementById("PhieuNhapID").value.trim();
  const manhacungcap = document.getElementById("NhaCungCapID").value.trim();
  const manhanvien = document.getElementById("NguoiNhap").value.trim();
  const ngaynhap = document.getElementById("NgayNhap").value.trim();

  const listChiTiet = collectChiTietNhap();
  //chuyển ngày sang ISO dạng bên backend nhận
  const ngaylapISO = `${ngaynhap}T00:00:00.000Z`; // yyyy-mm-dd

  const newPhieuNhap = {
    maphieunhap: maphieunhap,
    mancc: manhacungcap,
    manv: manhanvien,
    ngaylap: ngaylapISO,
    listjson_chitietnhap: listChiTiet
  };
  //nếu không thêm chi tiết thì bỏ qua

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
      //console.log(response)
      if (response.success === false) {
        alert("Thêm phiếu nhập thất bại!");
        return;
      }
      alert("Thêm phiếu nhập thành công!");
      closeAddPhieuNhap();
      //location.reload();
      loadAllPhieuNhap(); // tải lại danh sách thay vì reload toàn trang
    },
    error: function (xhr) {
      console.error("Lỗi khi thêm phiếu nhập:", xhr.responseText);
      alert("Thêm phiếu nhập thất bại! Kiểm tra console để xem chi tiết lỗi.");
    }
  })
}


function addChiTietRow() {
  const tbody = document.getElementById('chiTietBody');
  const newRow = document.createElement('tr');
  newRow.innerHTML = `
    <td>
      <select class="maSP" required>
        ${sanPhamOptionsHTML}
      </select>
    </td>
    <td><input type="number" class="soLuong" min="1" required></td>
    <td><input type="number" class="donGia" min="0" required></td>
    <td>
      <button type="button" class="btn-delete" onclick="removeChiTietRow(this)">Xóa</button>
    </td>
  `;

  tbody.appendChild(newRow);
}

function removeChiTietRow(button) {
  button.closest('tr').remove();
}

function addEditChiTietRow() {
  const tbody = document.getElementById('editChiTietBody');
  const newRow = document.createElement('tr');
  newRow.innerHTML = `
    <td>
      <select class="maSP" required>
        ${sanPhamOptionsHTML}
      </select>
    </td>
    <td><input type="number" class="soLuong" min="1" required></td>
    <td><input type="number" class="donGia" min="0" required></td>
    <td>
      <button type="button" class="btn-delete" onclick="removeChiTietRow(this)">Xóa</button>
    </td>
  `;

  tbody.appendChild(newRow);
}


function openEditPhieuNhap(maphieuNhap) {
  document.getElementById("editPhieuNhap").style.display = "flex";

  const phieunhap = allPhieuNhap.find(pn => pn.maphieunhap.trim() === maphieuNhap.trim());
  if (!phieunhap) {
    alert("Không tìm thấy phiếu nhập cần sửa!");
    return;
  }

  document.getElementById("originalPhieuId").value = phieunhap.maphieunhap.trim();
  document.getElementById("editNhaCungCapID").value = phieunhap.mancc.trim();
  document.getElementById("editNguoiNhap").value = phieunhap.manv.trim();

  //chuyển ngày ISO: cắt, chỉ lấy phần ngày
  const ngaylapBT = phieunhap.ngaylap.split('T')[0];
  document.getElementById("editNgayNhap").value = ngaylapBT;

  //lấy các chi tiết phiếu
  const dschitietnhap = allCTPhieuNhap.filter(ctpn => ctpn.maphieunhap.trim() === maphieuNhap.trim());
  //thêm các row chi tiết cho phiếu (nếu có)
  const tableEdit = document.getElementById("editChiTietBody");
  tableEdit.innerHTML = "";
  if (dschitietnhap.length > 0) {
    dschitietnhap.forEach(ctpn => {

      let optionsHTML = '<option value="">-- Chọn sản phẩm --</option>';
      allSanPham.forEach(sp => {
        const masp = (sp.masp || "").trim();
        const tensp = (sp.tensp || "").trim();

        const selected = (masp === ctpn.masp.trim()) ? 'selected' : '';

        optionsHTML += `<option value="${masp}" ${selected}>${masp} - ${tensp}</option>`;
      });
      const newRowHTML = `
            <tr>
                <td>
                    <select class="maSP" required>
                        ${optionsHTML} 
                      </select>
                </td>
                <td><input type="number" value="${ctpn.soluong}" class="soLuong" min="1" required></td>
                <td><input type="number" value="${ctpn.dongianhap}" class="donGia" min="0" required></td>
                <td>
                    <button type="button" class="btn-delete" onclick="removeChiTietRow(this)">Xóa</button>
                </td>
            </tr>
            `;
      tableEdit.innerHTML += newRowHTML;
    });
  }
}
function closeEditPhieuNhap() {
  document.getElementById("editPhieuNhap").style.display = "none";
  document.getElementById("editPhieuNhapForm").reset();
  return;
}

//xoá
function deleteReceipt(maphieunhap) {
  if (!confirm(`Bạn có chắc muốn xoá phiếu nhập '${maphieunhap}' này không?`)) return;

  $.ajax({
    url: `${API_URL}/delete-phieunhapkho?maphieunhap=${maphieunhap}`,
    type: "DELETE",
    contentType: "application/json",
    headers: {
      Authorization: `Bearer ${token}`
    },
    success: function (response) {
      //console.log(response.data)
      alert("Xoá phiếu nhập thành công!");
      location.reload(); // tải lại danh sách
    },
    error: function (xhr) {
      alert(`Không thể thêm xoá phiếu nhập!.`);
    }
  });
}
