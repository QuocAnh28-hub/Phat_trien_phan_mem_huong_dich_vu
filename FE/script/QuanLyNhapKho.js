const API_URL = 'https://localhost:7095/api/QuanLyNhapKho/get-byid-phieunhapkho?maphieunhap';
// Toggle hiển thị chi tiết phiếu nhập
function toggleDetails(receiptId) {
    const detailsDiv = document.getElementById(`receipt-details-${receiptId}`);
    if (detailsDiv.style.display === 'none') {
        // Ẩn tất cả các chi tiết khác
        const allDetails = document.querySelectorAll('.receipt-details');
        allDetails.forEach(detail => detail.style.display = 'none');
        
        // Hiển thị chi tiết được chọn
        detailsDiv.style.display = 'block';
        
        // Load chi tiết phiếu nhập
        loadReceiptDetails(receiptId);
    } else {
        detailsDiv.style.display = 'none';
    }
}



// Hiển thị chi tiết phiếu nhập
function displayReceiptDetails(receiptId, details) {
    const detailsContainer = document.getElementById(`receipt-details-${receiptId}`);
    let html = `
        <h3>Chi tiết phiếu nhập ${receiptId}</h3>
        <table class="detail-table">
            <thead>
                <tr>
                    <th>Mã SP</th>
                    <th>Tên sản phẩm</th>
                    <th>Số lượng</th>
                    <th>Đơn giá</th>
                    <th>Thành tiền</th>
                </tr>
            </thead>
            <tbody>
    `;

    details.forEach(item => {
        html += `
            <tr>
                <td>${item.productId}</td>
                <td>${item.productName}</td>
                <td>${item.quantity}</td>
                <td>${formatCurrency(item.price)}</td>
                <td>${formatCurrency(item.quantity * item.price)}</td>
            </tr>
        `;
    });

    html += '</tbody></table>';
    detailsContainer.innerHTML = html;
}

// Format tiền tệ
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}




function addChiTietRow() {
  const tbody = document.getElementById('chiTietBody');
  const newRow = `
    <tr>
      <td><input type="text" class="maSP" required></td>
      <td><input type="text" class="tenSP" required></td>
      <td><input type="number" class="soLuong" min="1" required onchange="tinhThanhTien(this)"></td>
      <td><input type="number" class="donGia" min="0" required onchange="tinhThanhTien(this)"></td>
      <td class="thanhTien">0₫</td>
      <td><button type="button" class="btn-delete" onclick="removeChiTietRow(this)">Xóa</button></td>
    </tr>
  `;
  tbody.insertAdjacentHTML('beforeend', newRow);
}

function removeChiTietRow(button) {
  button.closest('tr').remove();
}


function openEditPhieuNhap(id) {
          // try receipts array first
          let data = (typeof receipts !== 'undefined') ? receipts.find(r => r.id === id) : null;

          if (!data) {
            // fallback: tìm row trong bảng
            const rows = document.querySelectorAll('#receipt-list tr');
            for (const r of rows) {
              if (r.cells && r.cells[0] && r.cells[0].textContent.trim() === id) {
                data = {
                  id: id,
                  ncc: r.cells[1].textContent.trim(),
                  nguoi: r.cells[2].textContent.trim(),
                  vat: parseFloat(r.cells[3].textContent.replace('%','')) || 0,
                  ngay: r.cells[4].textContent.trim(),
                  chiTiet: []
                };
                // nếu có div chi tiết tương ứng, lấy dữ liệu từ đó
                const detailDiv = document.getElementById('receipt-details-' + id);
                if (detailDiv) {
                  const trList = detailDiv.querySelectorAll('table tbody tr');
                  trList.forEach(tr => {
                    const tds = tr.querySelectorAll('td');
                    data.chiTiet.push({
                      maSP: tds[0].textContent.trim(),
                      tenSP: tds[1].textContent.trim(),
                      soLuong: parseFloat(tds[2].textContent.trim()) || 0,
                      donGia: parseFloat(tds[3].textContent.replace(/[^0-9.-]+/g,"")) || 0
                    });
                  });
                }
                break;
              }
            }
          }

          if (!data) return alert('Không tìm thấy phiếu ' + id);

          // fill form
          document.getElementById('originalPhieuId').value = data.id;
          document.getElementById('editPhieuNhapID').value = data.id;
          document.getElementById('editNhaCungCapID').value = data.ncc || '';
          document.getElementById('editNguoiNhap').value = data.nguoi || '';
          document.getElementById('editThueVAT').value = data.vat || 0;
          // nếu định dạng ngày khác có thể cần chuyển đổi
          document.getElementById('editNgayNhap').value = data.ngay || '';

          // fill chi tiết
          const body = document.getElementById('editChiTietBody');
          body.innerHTML = '';
          if (data.chiTiet && data.chiTiet.length) {
            data.chiTiet.forEach(i => {
              const tr = document.createElement('tr');
              tr.innerHTML = `
                <td><input type="text" class="maSP" value="${i.maSP || ''}" required></td>
                <td><input type="text" class="tenSP" value="${i.tenSP || ''}" required></td>
                <td><input type="number" class="soLuong" min="1" value="${i.soLuong || 0}" required onchange="tinhThanhTienEdit(this)"></td>
                <td><input type="number" class="donGia" min="0" value="${i.donGia || 0}" required onchange="tinhThanhTienEdit(this)"></td>
                <td class="thanhTien">${new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format((i.soLuong||0)*(i.donGia||0))}</td>
                <td><button type="button" class="btn-delete" onclick="removeEditChiTietRow(this)">Xóa</button></td>
              `;
              body.appendChild(tr);
            });
          } else {
            // 1 dòng tr mặc định
            body.innerHTML = `<tr>
              <td><input type="text" class="maSP" required></td>
              <td><input type="text" class="tenSP" required></td>
              <td><input type="number" class="soLuong" min="1" value="1" required onchange="tinhThanhTienEdit(this)"></td>
              <td><input type="number" class="donGia" min="0" value="0" required onchange="tinhThanhTienEdit(this)"></td>
              <td class="thanhTien">0₫</td>
              <td><button type="button" class="btn-delete" onclick="removeEditChiTietRow(this)">Xóa</button></td>
            </tr>`;
          }

          document.getElementById('editPhieuNhap').style.display = 'block';
        }

        function closeEditPhieuNhap() {
          document.getElementById('editPhieuNhap').style.display = 'none';
        }

        function addEditChiTietRow() {
          const tbody = document.getElementById('editChiTietBody');
          const newRow = document.createElement('tr');
          newRow.innerHTML = `
            <td><input type="text" class="maSP" required></td>
            <td><input type="text" class="tenSP" required></td>
            <td><input type="number" class="soLuong" min="1" value="1" required onchange="tinhThanhTienEdit(this)"></td>
            <td><input type="number" class="donGia" min="0" value="0" required onchange="tinhThanhTienEdit(this)"></td>
            <td class="thanhTien">0₫</td>
            <td><button type="button" class="btn-delete" onclick="removeEditChiTietRow(this)">Xóa</button></td>
          `;
          tbody.appendChild(newRow);
        }

        function removeEditChiTietRow(btn) {
          btn.closest('tr').remove();
        }

        function tinhThanhTienEdit(input) {
          const row = input.closest('tr');
          const soLuong = parseFloat(row.querySelector('.soLuong').value) || 0;
          const donGia = parseFloat(row.querySelector('.donGia').value) || 0;
          const thanhTien = soLuong * donGia;
          row.querySelector('.thanhTien').textContent = new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(thanhTien);
        }

        // submit xử lý lưu sửa
        document.getElementById('editPhieuNhapForm').addEventListener('submit', function(e){
          e.preventDefault();
          const originalId = document.getElementById('originalPhieuId').value;
          const id = document.getElementById('editPhieuNhapID').value.trim();
          const ncc = document.getElementById('editNhaCungCapID').value.trim();
          const nguoi = document.getElementById('editNguoiNhap').value.trim();
          const vat = parseFloat(document.getElementById('editThueVAT').value) || 0;
          const ngay = document.getElementById('editNgayNhap').value;

          const chiTiet = [];
          document.querySelectorAll('#editChiTietBody tr').forEach(r => {
            chiTiet.push({
              maSP: r.querySelector('.maSP').value.trim(),
              tenSP: r.querySelector('.tenSP').value.trim(),
              soLuong: parseFloat(r.querySelector('.soLuong').value) || 0,
              donGia: parseFloat(r.querySelector('.donGia').value) || 0
            });
          });

          // cập nhật mảng receipts nếu tồn tại
          if (typeof receipts !== 'undefined') {
            const idx = receipts.findIndex(x => x.id === originalId);
            if (idx !== -1) {
              receipts[idx] = { id, ncc, nguoi, vat, ngay, chiTiet };
            }
          }

          // cập nhật DOM bảng (tìm row có mã originalId)
          const rows = document.querySelectorAll('#receipt-list tr');
          for (const r of rows) {
            if (r.cells && r.cells[0] && r.cells[0].textContent.trim() === originalId) {
              r.cells[0].textContent = id;
              if (r.cells[1]) r.cells[1].textContent = ncc;
              if (r.cells[2]) r.cells[2].textContent = nguoi;
              if (r.cells[3]) r.cells[3].textContent = vat + '%';
              if (r.cells[4]) r.cells[4].textContent = ngay;
            }
          }

          // cập nhật phần chi tiết DOM nếu có
          const detailDivOld = document.getElementById('receipt-details-' + originalId);
          if (detailDivOld) {
            const newDiv = document.createElement('div');
            newDiv.id = 'receipt-details-' + id;
            newDiv.className = 'receipt-details';
            let html = `<h3>Chi tiết phiếu nhập ${id}</h3><table class="detail-table"><thead><tr><th>Mã SP</th><th>Tên sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead><tbody>`;
            chiTiet.forEach(i => {
              html += `<tr>
                <td>${i.maSP}</td>
                <td>${i.tenSP}</td>
                <td>${i.soLuong}</td>
                <td>${new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(i.donGia)}</td>
                <td>${new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(i.donGia * i.soLuong)}</td>
              </tr>`;
            });
            html += '</tbody></table>';
            newDiv.innerHTML = html;
            detailDivOld.parentNode.replaceChild(newDiv, detailDivOld);
          }

          // nếu mã thay đổi, cập nhật nút onclick trên hàng (nếu cần)
          // đóng popup
          closeEditPhieuNhap();
          alert('Cập nhật phiếu nhập thành công');
        });

function openAddPhieuNhap() {
  document.getElementById('addPhieuNhap').style.display = 'block';
}

function closeAddPhieuNhap() {
  document.getElementById('addPhieuNhap').style.display = 'none';
  document.getElementById('addPhieuNhapForm').reset();
}


function openEditPhieuNhap(id) {
  document.getElementById("editPhieuNhap").style.display = "flex";
}
function closeEditPhieuNhap() {
  document.getElementById("editPhieuNhap").style.display = "none";
  document.getElementById("editPhieuNhapForm").reset();
}

// Đóng popup khi click ra ngoài
window.onclick = function(e) {
  const add = document.getElementById("addPhieuNhap");
  const edit = document.getElementById("editPhieuNhap");
  if (e.target === add) closeAddPhieuNhap();
  if (e.target === edit) closeEditPhieuNhap();
};