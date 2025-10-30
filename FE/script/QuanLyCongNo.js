const db = {
HoaDonBan: [
    { id: 'HD001', ngay: '2025-10-01', khach: 'Nguyễn A', tong: 1500000, paid: 500000 },
    { id: 'HD002', ngay: '2025-10-05', khach: 'Đỗ B', tong: 3200000, paid: 3200000 },
    { id: 'HD003', ngay: '2025-10-15', khach: 'Nguyễn A', tong: 700000, paid: 0 },
    { id: 'HD004', ngay: '2025-10-20', khach: 'Phạm C', tong: 1200000, paid: 200000 }
],
ThanhToan: [
    { id: 'TT001', hoaDonId: 'HD001', ngay: '2025-10-02', soTien: 500000, method: 'Tiền mặt' },
    { id: 'TT002', hoaDonId: 'HD002', ngay: '2025-10-06', soTien: 3200000, method: 'Chuyển khoản' }
]
};

const money = n => new Intl.NumberFormat('vi-VN').format(n || 0);

function groupDebts() {
const map = {};
db.HoaDonBan.forEach(h => {
    const owe = Math.max(0, (h.tong || 0) - (h.paid || 0));
    if (!map[h.khach]) map[h.khach] = { khach: h.khach, invoices: [], total: 0, owed: 0 };
    map[h.khach].invoices.push(Object.assign({}, h, { owed: owe }));
    map[h.khach].total += (h.tong || 0);
    map[h.khach].owed += owe;
});
return Object.values(map).sort((a,b)=>b.owed - a.owed);
}

function renderDebts() {
const tbody = document.querySelector('#debt-table tbody');
const list = groupDebts();
if (!list.length) {
    tbody.innerHTML = '<tr><td colspan="5" class="muted">Không có khách hàng còn nợ.</td></tr>';
    return;
}
tbody.innerHTML = '';
list.forEach((c) => {
    const tr = document.createElement('tr');
    tr.innerHTML = `
    <td>${c.khach}</td>
    <td>${c.invoices.length}</td>
    <td>${money(c.total)}</td>
    <td>${money(c.owed)}</td>
    <td class="inline">
        <button class="btn ghost" data-act="view" data-k="${c.khach}">Xem hóa đơn</button>
        ${c.owed > 0 ? `<button class="btn primary" data-act="pay" data-k="${c.khach}">Thanh toán</button>` : `<span class="small muted">Đã thu sạch</span>`}
    </td>
    `;
    tbody.appendChild(tr);
});
}

function renderInvoicesFor(khach) {
const invoices = db.HoaDonBan.filter(h => h.khach === khach);
if (!invoices.length) {
    document.getElementById('detail-area').innerHTML = '<div class="muted">Không tìm thấy hóa đơn.</div>';
    return;
}
let html = `<div class="invoices"><h3 style="margin:8px 0 6px 0">Hóa đơn của ${khach}</h3><table class="debt-table"><thead><tr><th>ID</th><th>Ngày</th><th>Tổng (₫)</th><th>Đã trả (₫)</th><th>Còn lại (₫)</th><th>Hành động</th></tr></thead><tbody>`;
invoices.forEach(h => {
    const owed = Math.max(0, (h.tong||0) - (h.paid||0));
    html += `<tr>
    <td>${h.id}</td>
    <td>${h.ngay}</td>
    <td>${money(h.tong)}</td>
    <td>${money(h.paid || 0)}</td>
    <td class="${owed ? 'unpaid' : 'paid'}">${money(owed)}</td>
    <td>
        ${owed > 0 
        ? `<button class="btn primary" data-pay="${h.id}">Thanh toán</button>`
        : `<span class="small muted">Đã thanh toán</span>
            <button class="btn ghost" data-del="${h.id}" style="margin-left:6px;">Xóa</button>`}
    </td>
    </tr>`;
});
html += `</tbody></table></div>`;
document.getElementById('detail-area').innerHTML = html;

document.querySelectorAll('[data-pay]').forEach(b=>{
    b.addEventListener('click', () => payInvoice(b.getAttribute('data-pay')));
});
document.querySelectorAll('[data-del]').forEach(b=>{
    b.addEventListener('click', () => deleteInvoice(b.getAttribute('data-del')));
});
}

function payInvoice(invoiceId) {
const inv = db.HoaDonBan.find(h => h.id === invoiceId);
if (!inv) return alert('Hóa đơn không tìm thấy');
const owed = Math.max(0, (inv.tong||0) - (inv.paid||0));
const str = prompt(`Hóa đơn ${inv.id} còn ${money(owed)}. Nhập số tiền thanh toán (bỏ trống = toàn bộ):`);
if (str === null) return;
let amt = Number(String(str).replace(/[^\d.-]/g,'')) || 0;
if (amt <= 0) amt = owed;
if (amt > owed) amt = owed;
const method = prompt('Phương thức thanh toán (Tiền mặt / Chuyển khoản / Thẻ):', 'Tiền mặt') || 'Tiền mặt';
const ttId = 'TT' + Math.floor(Math.random()*900000 + 100000);
db.ThanhToan.push({ id: ttId, hoaDonId: inv.id, ngay: new Date().toISOString().slice(0,10), soTien: amt, method });
inv.paid = (inv.paid || 0) + amt;
if (inv.paid > inv.tong) inv.paid = inv.tong;
alert(`Thanh toán ${money(amt)} cho ${inv.id} thành công.`);
renderDebts();
renderInvoicesFor(inv.khach);
document.getElementById('detail-area').scrollIntoView({behavior:'smooth'});
}

function deleteInvoice(invoiceId) {
if (!confirm('Bạn có chắc muốn xóa hóa đơn này không?')) return;
const idx = db.HoaDonBan.findIndex(h => h.id === invoiceId);
if (idx === -1) return alert('Không tìm thấy hóa đơn cần xóa.');
const kh = db.HoaDonBan[idx].khach;
db.HoaDonBan.splice(idx, 1);
alert('Đã xóa hóa đơn ' + invoiceId);
renderDebts();
renderInvoicesFor(kh);
}

document.addEventListener('DOMContentLoaded', () => {
renderDebts();
document.getElementById('debt-table').addEventListener('click', e => {
    const v = e.target;
    if (v.matches('[data-act="view"]')) {
    renderInvoicesFor(v.getAttribute('data-k'));
    document.getElementById('detail-area').scrollIntoView({behavior:'smooth'});
    } else if (v.matches('[data-act="pay"]')) {
    renderInvoicesFor(v.getAttribute('data-k'));
    document.getElementById('detail-area').scrollIntoView({behavior:'smooth'});
    }
});
});