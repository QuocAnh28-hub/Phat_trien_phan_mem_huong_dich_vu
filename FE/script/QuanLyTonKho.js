// Nỗ lực lấy sản phẩm từ API, nếu không có dùng demo
      const API_PRODUCTS = '/api/products'; // thay đổi khi có API thực
      let products = [
        { id: 'SP001', name: 'iPhone 14', barcode: '8934571234', desc: 'Điện thoại thông minh cao cấp', category: 'DM001', price: 25000000, attrs: '128GB - Đen', vat: 10, stock: 15 },
        { id: 'SP002', name: 'Asus Vivobook', barcode: '7896541230', desc: 'Laptop hiệu năng tốt', category: 'DM002', price: 15000000, attrs: '8GB/512GB', vat: 10, stock: 8 },
        { id: 'SP003', name: 'Tai nghe Bluetooth', barcode: '111222333', desc: 'Tai nghe không dây', category: 'DM003', price: 800000, attrs: 'Màu trắng', vat: 10, stock: 50 }
      ];

      function formatCurrency(v){ return new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(v||0); }

      async function loadProducts() {
        try {
          const res = await fetch(API_PRODUCTS);
          if (res.ok) {
            const data = await res.json();
            if (Array.isArray(data) && data.length) { products = data; }
          }
        } catch(e){
          // dùng dữ liệu demo
          console.warn('Không thể lấy từ API, dùng dữ liệu demo.');
        }
        renderProducts();
      }

      function refreshProducts(){ loadProducts(); }

      function renderProducts(){
        const tbody = document.getElementById('product-list');
        tbody.innerHTML = '';
        products.forEach(p => {
          const tr = document.createElement('tr');
          tr.innerHTML = `
            <td>${p.id}</td>
            <td>${p.name}</td>
            <td>${p.barcode || ''}</td>
            <td>${p.desc || ''}</td>
            <td>${p.category || ''}</td>
            <td>${formatCurrency(p.price)}</td>
            <td>${p.attrs || ''}</td>
            <td>${p.vat ? p.vat + '%' : ''}</td>
            <td>${p.stock}</td>
            <td>
              <button class="btn-view" onclick="toggleProductDetails('${p.id}')">Xem</button>
              <button class="btn-primary" onclick="openAdjustModal('${p.id}')">Điều chỉnh</button>
            </td>
          `;
          tbody.appendChild(tr);
        });
        renderAllDetails(); // chuẩn bị div chi tiết
      }

      function renderAllDetails(){
        const container = document.getElementById('product-details-container');
        container.innerHTML = '';
        products.forEach(p => {
          const div = document.createElement('div');
          div.id = 'product-details-' + p.id;
          div.className = 'receipt-details';
          div.style.display = 'none';
          div.innerHTML = `
            <h3>Chi tiết ${p.id} - ${p.name}</h3>
            <table class="detail-table">
              <thead><tr><th>Mã SP</th><th>Tên</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead>
              <tbody>
                <tr>
                  <td>${p.id}</td>
                  <td>${p.name}</td>
                  <td>${p.stock}</td>
                  <td>${formatCurrency(p.price)}</td>
                  <td>${formatCurrency(p.stock * (p.price||0))}</td>
                </tr>
              </tbody>
            </table>
          `;
          container.appendChild(div);
        });
      }

      function toggleProductDetails(id){
        document.querySelectorAll('.receipt-details').forEach(d => d.style.display = 'none');
        const el = document.getElementById('product-details-' + id);
        if (!el) return;
        el.style.display = el.style.display === 'block' ? 'none' : 'block';
        if (el.style.display === 'block') el.scrollIntoView({behavior:'smooth', block:'nearest'});
      }

      // Adjust modal
      function openAdjustModal(productId){
        const modal = document.getElementById('adjustModal');
        const idEl = document.getElementById('adjustProductId');
        const nameEl = document.getElementById('adjustProductName');
        if (productId){
          const p = products.find(x => x.id === productId);
          if (p){
            idEl.value = p.id;
            nameEl.value = p.name;
          } else {
            idEl.value = productId;
            nameEl.value = '';
          }
        } else {
          idEl.value = '';
          nameEl.value = '';
        }
        modal.style.display = 'block';
      }

      function closeAdjustModal(){
        document.getElementById('adjustModal').style.display = 'none';
        document.getElementById('adjustForm').reset();
      }

      document.getElementById('adjustForm').addEventListener('submit', function(e){
        e.preventDefault();
        const id = document.getElementById('adjustProductId').value.trim();
        const type = document.getElementById('adjustType').value;
        const qty = parseInt(document.getElementById('adjustQuantity').value,10) || 0;
        if (!id || qty <= 0) return alert('Chọn sản phẩm và số lượng hợp lệ.');
        const idx = products.findIndex(p => p.id === id);
        if (idx === -1) return alert('Không tìm thấy sản phẩm.');
        if (type === 'increase') products[idx].stock += qty;
        else products[idx].stock = Math.max(0, products[idx].stock - qty);
        renderProducts();
        closeAdjustModal();
        alert('Cập nhật tồn kho thành công.');
      });

      // khởi tạo
      document.addEventListener('DOMContentLoaded', () => {
        loadProducts();
        // đóng modal khi click ngoài nội dung
        window.addEventListener('click', function(e){
          const modal = document.getElementById('adjustModal');
          if (e.target === modal) closeAdjustModal();
        });
      });