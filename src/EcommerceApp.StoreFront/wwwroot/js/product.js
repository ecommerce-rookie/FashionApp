$(document).ready(function () {
    const cartIcon = $(".user-nav__icon_4");

    $(".add-to-cart-btn").on("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        const item = $(this).closest(".short-item");
        const productId = item.data("product-id");
        const quantity = 1;

        $.ajax({
            type: "POST",
            url: "/checkout?handler=AddToCart",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                productId: productId,
                quantity: quantity
            }),
            success: function (res) {
                if (res === true) {
                    const cartCount = cartIcon.next();
                    cartCount.text(parseInt(cartCount.text()) + quantity);

                    cartIcon.addClass("cart-shake");
                    setTimeout(() => {
                        cartIcon.removeClass("cart-shake");
                    }, 800);

                    //createNotification('success', 'Thank You!', 'Add Product Success');
                } else {
                    createNotification('error', 'Sorry! Try Again!', 'Add Product Fail');
                }
            },
            error: function () {
                createNotification('error', 'Oop! Try again!', 'Network Error!');
            }
        });
    });
});
