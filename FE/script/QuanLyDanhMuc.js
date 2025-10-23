
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
