// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/**
 * Main JavaScript file for the website
 */
document.addEventListener('DOMContentLoaded', function () {
    // Handle navigation active state
    const setActiveNavLink = () => {
        const currentPage = window.location.pathname;
        const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
        
        navLinks.forEach(link => {
            const href = link.getAttribute('href');
            if (href === currentPage || (currentPage.startsWith(href) && href !== '/')) {
                link.classList.add('active');
                link.style.color = 'var(--primary-color)';
            }
        });
    };

    // Back to top button - simplified
    const createBackToTopButton = () => {
        const button = document.createElement('button');
        button.innerHTML = '<i class="bi bi-arrow-up"></i>';
        button.className = 'back-to-top-btn';
        button.setAttribute('aria-label', 'Back to top');
        
        document.body.appendChild(button);
        
        // Throttle scroll event to improve performance
        let scrollTimeout;
        window.addEventListener('scroll', () => {
            if (scrollTimeout) return;
            
            scrollTimeout = setTimeout(() => {
                if (window.pageYOffset > 300) {
                    button.classList.add('visible');
                } else {
                    button.classList.remove('visible');
                }
                scrollTimeout = null;
            }, 100);
        });
        
        button.addEventListener('click', () => {
            // Use simpler scrolling for better performance
            window.scrollTo(0, 0);
        });
    };

    // Execute only essential functions
    setActiveNavLink();
    createBackToTopButton();
    
    // Lazy loading images (using native browser support if available)
    if (!('loading' in HTMLImageElement.prototype)) {
        // Only implement if browser doesn't support lazy loading
        const lazyImages = document.querySelectorAll('img[loading="lazy"]');
        if (lazyImages.length > 0) {
            // Simple lazy loading implementation
            const lazyLoadObserver = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.dataset.src || img.src;
                        lazyLoadObserver.unobserve(img);
                    }
                });
            });
            
            lazyImages.forEach(img => {
                lazyLoadObserver.observe(img);
            });
        }
    }
});

// Smooth scrolling only for anchor links - optimized
document.addEventListener('click', function(e) {
    // Use event delegation instead of attaching to each anchor
    const anchor = e.target.closest('a[href^="#"]');
    if (anchor) {
        const targetId = anchor.getAttribute('href');
        if (targetId !== '#') {
            e.preventDefault();
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                const header = document.querySelector('.navbar');
                const headerOffset = header ? header.offsetHeight : 0;
                const targetPosition = targetElement.getBoundingClientRect().top + window.pageYOffset - headerOffset;
                
                window.scrollTo({
                    top: targetPosition,
                    behavior: 'smooth'
                });
            }
        }
    }
});
