
function openAddProduct() {
  document.getElementById("addProduct").style.display = "flex";
}
function closeAddProduct() {
  document.getElementById("addProduct").style.display = "none";
  document.getElementById("addProductForm").reset();
}

function openEditProduct(id) {
  document.getElementById("editProduct").style.display = "flex";
}
function closeEditProduct() {
  document.getElementById("editProduct").style.display = "none";
  document.getElementById("editProductForm").reset();
}

// Đóng popup khi click ra ngoài
window.onclick = function(e) {
  const addProduct = document.getElementById("addProduct");
  const editProduct = document.getElementById("editProduct");
  if (e.target === addProduct) closeAddProduct();
  if (e.target === editProduct) closeEditProduct();
};
