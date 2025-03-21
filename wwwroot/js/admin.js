document.addEventListener('DOMContentLoaded', function() {
    // Toggle sidebar
    const sidebarToggle = document.getElementById('sidebar-toggle');
    const sidebar = document.querySelector('.sidebar');
    const mainContent = document.querySelector('.main-content');
    
    sidebarToggle.addEventListener('click', function() {
        sidebar.classList.toggle('collapsed');
        mainContent.classList.toggle('expanded');
    });
    
    // Highlight active nav item
    const currentUrl = window.location.pathname;
    const navLinks = document.querySelectorAll('.sidebar .nav-link');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (currentUrl === href || (href !== '#' && currentUrl.includes(href))) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });
    
    // Initialize any chart libraries or other components here
});
