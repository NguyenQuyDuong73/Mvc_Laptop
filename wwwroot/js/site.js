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
  const total = quantity * price; // Tính tổng giá
  document.getElementById(`total-${productId}`).innerText =
    "$" + total.toFixed(2); // Cập nhật tổng giá vào table
}
function updateQuantity(productId) {
  let input = document.getElementById(`quantity-${productId}`);
  let quantity = input.value;

  // Gửi yêu cầu cập nhật số lượng qua AJAX
  fetch(
    `http://localhost:5187/Cart/UpdateQuantity?id=${productId}&quantity=${quantity}`,
    {
      method: "GET",
    }
  )
    .then((response) => response.json())
    .then((data) => {
      if (data.success) {
        console.log("Giỏ hàng đã được cập nhật.");
      }
    })
    .catch((error) => {
      console.error("Có lỗi xảy ra khi cập nhật giỏ hàng:", error);
    });
}
// Hàm để lấy số lượng giỏ hàng từ backend và cập nhật UI
function updateCartCount() {
  $.ajax({
    url: "/Cart/GetCartItemCount", // URL của action trong controller
    type: "GET",
    success: function (data) {
      // Cập nhật số lượng giỏ hàng trong span
      $(".js-cart-count").text(data.totalQuantity); // Cập nhật số lượng giỏ hàng
    },
    error: function (error) {
      console.error("Có lỗi xảy ra khi lấy số lượng giỏ hàng:", error);
    },
  });
}
// Gọi hàm updateCartCount sau khi thay đổi giỏ hàng (thêm sản phẩm, xóa sản phẩm, cập nhật số lượng)
updateCartCount(); // Gọi hàm khi trang được tải
// Khi thêm sản phẩm vào giỏ hàng
function addToCart(productId, quantity) {
  $.ajax({
    url: "/Cart/AddToCart", // Thay đổi URL tương ứng với hành động trong controller
    type: "POST",
    data: { id: productId, quantity: quantity },
    success: function () {
      updateCartCount(); // Cập nhật số lượng sau khi thêm sản phẩm
    },
    error: function (error) {
      console.error("Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng:", error);
    },
  });
}
// Khi xóa sản phẩm khỏi giỏ hàng
function removeFromCart(productId) {
  $.ajax({
    url: "/Cart/RemoveFromCart", // Thay đổi URL tương ứng với hành động trong controller
    type: "POST",
    data: { id: productId },
    success: function () {
      updateCartCount(); // Cập nhật số lượng sau khi xóa sản phẩm
    },
    error: function (error) {
      console.error("Có lỗi xảy ra khi xóa sản phẩm khỏi giỏ hàng:", error);
    },
  });
}
function closeModal() {
  const modal = document.getElementById('loginModal');
  if (modal) {
      modal.style.display = 'none';
  }
}
document.addEventListener("DOMContentLoaded", function () {
  const slides = document.querySelectorAll(".hero-carousel-slide");
  const prevButton = document.querySelector(".hero-carousel-prev");
  const nextButton = document.querySelector(".hero-carousel-next");
  const dotsContainer = document.querySelector(".hero-carousel-dots");

  let currentIndex = 0;

  // Create dots dynamically
  slides.forEach((_, index) => {
      const dot = document.createElement("button");
      dot.dataset.index = index;
      if (index === currentIndex) dot.classList.add("active");
      dotsContainer.appendChild(dot);
  });

  const dots = dotsContainer.querySelectorAll("button");

  function showSlide(index) {
      slides.forEach((slide, i) => {
          slide.style.display = i === index ? "block" : "none";
      });
      dots.forEach((dot, i) => {
          dot.classList.toggle("active", i === index);
      });
      currentIndex = index;
  }

  // Next slide
  nextButton.addEventListener("click", () => {
      const nextIndex = (currentIndex + 1) % slides.length;
      showSlide(nextIndex);
  });

  // Previous slide
  prevButton.addEventListener("click", () => {
      const prevIndex = (currentIndex - 1 + slides.length) % slides.length;
      showSlide(prevIndex);
  });

  // Dot navigation
  dots.forEach(dot => {
      dot.addEventListener("click", (e) => {
          const index = parseInt(e.target.dataset.index);
          showSlide(index);
      });
  });

  // Auto-slide
  setInterval(() => {
      const nextIndex = (currentIndex + 1) % slides.length;
      showSlide(nextIndex);
  }, 5000);
});

/*Register*/
document.getElementById("registerForm").onsubmit = function (event) {
  var password = document.getElementById("Password").value;
  var confirmPassword = document.getElementById("ConfirmPassword").value;
  var confirmPasswordError = document.getElementById("ConfirmPasswordError");
  if (password !== confirmPassword) {
      event.preventDefault();
      confirmPasswordError.textContent = "Mật khẩu và xác nhận mật khẩu không khớp.";
  } else {
      confirmPasswordError.textContent = ""; // Xóa lỗi nếu mật khẩu khớp
  }
};
