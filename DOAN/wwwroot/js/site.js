// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Global JavaScript for E-commerce Store

$(document).ready(function() {
    // Initialize cart count
    updateCartCount();
    
    // Add to cart functionality
    $('.add-to-cart').click(function(e) {
        e.preventDefault();
        
        var productId = $(this).data('product-id');
        var quantity = $('#quantity').val() || 1;
        
        // Show loading state
        var originalText = $(this).html();
        $(this).html('<i class="fas fa-spinner fa-spin me-2"></i>Đang thêm...');
        $(this).prop('disabled', true);
        
        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: {
                productId: productId,
                quantity: quantity
            },
            success: function(response) {
                if (response.success) {
                    showAlert('success', response.message);
                    updateCartCount(response.cartCount);
                } else {
                    showAlert('danger', response.message);
                }
            },
            error: function() {
                showAlert('danger', 'Có lỗi xảy ra. Vui lòng thử lại.');
            },
            complete: function() {
                // Restore button state
                $('.add-to-cart[data-product-id="' + productId + '"]').html(originalText);
                $('.add-to-cart[data-product-id="' + productId + '"]').prop('disabled', false);
            }
        });
    });
    
    // Search functionality
    $('.search-form').on('submit', function(e) {
        var searchTerm = $(this).find('input[name="search"]').val().trim();
        if (searchTerm.length < 2) {
            e.preventDefault();
            showAlert('warning', 'Vui lòng nhập ít nhất 2 ký tự để tìm kiếm.');
        }
    });
    
    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function(event) {
        var target = $(this.getAttribute('href'));
        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 100
            }, 1000);
        }
    });
    
    // Add fade-in animation to elements
    $('.product-card, .category-card').addClass('fade-in');
});

// Update cart count in header
function updateCartCount(count) {
    if (count !== undefined) {
        $('.cart-count').text(count);
    } else {
        // Fetch current cart count
        $.ajax({
            url: '/Cart/GetCartCount',
            type: 'GET',
            success: function(response) {
                $('.cart-count').text(response.count || 0);
            }
        });
    }
}

// Show alert messages
function showAlert(type, message) {
    var alertHtml = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
        '<i class="fas fa-' + getAlertIcon(type) + ' me-2"></i>' +
        message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>';
    
    // Remove existing alerts
    $('.alert').remove();
    
    // Add new alert
    $('main').prepend(alertHtml);
    
    // Auto hide after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut();
    }, 5000);
}

// Get appropriate icon for alert type
function getAlertIcon(type) {
    switch (type) {
        case 'success':
            return 'check-circle';
        case 'danger':
            return 'exclamation-triangle';
        case 'warning':
            return 'exclamation-circle';
        case 'info':
            return 'info-circle';
        default:
            return 'info-circle';
    }
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

// Format number with thousand separators
function formatNumber(number) {
    return new Intl.NumberFormat('vi-VN').format(number);
}

// Debounce function for search
function debounce(func, wait, immediate) {
    var timeout;
    return function() {
        var context = this, args = arguments;
        var later = function() {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
}

// Lazy loading for images
function lazyLoadImages() {
    const images = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });

    images.forEach(img => imageObserver.observe(img));
}

// Initialize lazy loading when DOM is ready
$(document).ready(function() {
    lazyLoadImages();
});

