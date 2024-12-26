// function decreaseQuantity(productId) {
//     const input = document.getElementById(`quantity-${productId}`);
//     if (input && input.value > 1) {
//         input.value--;
//     }
// }

// function increaseQuantity(productId) {
//     const input = document.getElementById(`quantity-${productId}`);
//     if (input) {
//         input.value++;
//     }
// }
function decreaseQuantity(productId) {
    const input = document.getElementById(`quantity-${productId}`); // Sử dụng id để tìm input
    let quantity = parseInt(input.value); // Lấy giá trị của input
    if (quantity > 1) {
        input.value = quantity - 1; // Giảm giá trị
        updateQuantity(productId);
    }
}

function increaseQuantity(productId) {
    const input = document.getElementById(`quantity-${productId}`); // Sử dụng id để tìm input
    let quantity = parseInt(input.value); // Lấy giá trị của input
    const maxQuantity = parseInt(input.max); // Lấy số lượng tối đa từ thuộc tính max
    if (quantity < maxQuantity) {
        input.value = quantity + 1; // Tăng giá trị
        updateQuantity(productId); // Cập nhật số lượng cho AJAX
    }
}
function updateTotal(productId, price) {
    const input = document.getElementById(`quantity-${productId}`);
    const quantity = parseInt(input.value);
    const total = quantity * price;  // Tính tổng giá
    document.getElementById(`total-${productId}`).innerText = "$" + total.toFixed(2); // Cập nhật tổng giá vào table
}
function updateQuantity(productId) {
    let input = document.getElementById(`quantity-${productId}`);
    let quantity = input.value;

    // Gửi yêu cầu cập nhật số lượng qua AJAX
    fetch(`http://localhost:5187/Cart/UpdateQuantity?id=${productId}&quantity=${quantity}`, {
        method: 'GET'
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            console.log("Giỏ hàng đã được cập nhật.");
        }
    })
    .catch(error => {
        console.error("Có lỗi xảy ra khi cập nhật giỏ hàng:", error);
    });
}