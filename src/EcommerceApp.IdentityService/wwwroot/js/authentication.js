document.addEventListener('DOMContentLoaded', function () {
    // Make form inputs have placeholder to work with label animation
    const formInputs = document.querySelectorAll('.form-input');
    formInputs.forEach(input => {
        input.setAttribute('placeholder', ' ');
    });

    // Tab switching functionality
    const tabBtns = document.querySelectorAll('.tab-btn');
    const authForms = document.querySelectorAll('.auth-form');

    tabBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            // Update active tab
            tabBtns.forEach(b => b.classList.remove('active'));
            this.classList.add('active');

            // Show corresponding form
            const formId = this.getAttribute('data-form');
            authForms.forEach(form => {
                form.classList.remove('active');
                if (form.id === `${form.id}Form`) {
                    form.classList.remove('active');
                }
                if (form.id === `${formId}Form`) {
                    form.classList.add('active');
                }
            });
        });
    });

    // Password toggle functionality
    const togglePasswordBtns = document.querySelectorAll('.toggle-password');

    togglePasswordBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            const passwordId = this.getAttribute('data-for');
            const passwordInput = document.getElementById(passwordId);
            const toggleIcon = this.querySelector('.toggle-icon');

            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                toggleIcon.classList.add('show');
            } else {
                passwordInput.type = 'password';
                toggleIcon.classList.remove('show');
            }
        });
    });

    // Login form validation
    const loginForm = document.getElementById('loginForm');

    //loginForm.addEventListener('submit', function (e) {
    //    e.preventDefault();

    //    simulateSubmit(this.querySelector('.submit-btn'));
    //});

    // Register form validation
    const registerForm = document.getElementById('registerForm');

    //registerForm.addEventListener('submit', function (e) {
    //    e.preventDefault();

    //    simulateSubmit(this.querySelector('.submit-btn'));

    //});

    function simulateSubmit(button) {
        const btnText = button.querySelector('.btn-text');
        const btnIcon = button.querySelector('.btn-icon');
        const originalText = btnText.textContent;

        // Show loading state
        btnText.textContent = 'PROCESSING';
        btnIcon.style.opacity = '0';
        button.disabled = true;

        // Add loading animation
        button.classList.add('loading');

        // Simulate API call with timeout
        setTimeout(() => {
            // Success animation
            button.classList.remove('loading');
            button.classList.add('success');
            btnText.textContent = 'SUCCESS';

            // Reset after showing success
            setTimeout(() => {
                button.classList.remove('success');
                btnText.textContent = originalText;
                btnIcon.style.opacity = '';
                button.disabled = false;
            }, 1500);
        }, 1500);
    }

    // Add subtle parallax effect to visual elements
    const visualElements = document.querySelectorAll('.editorial-element');
    document.addEventListener('mousemove', function (e) {
        if (window.innerWidth <= 1024) return;

        const moveX = (e.clientX - window.innerWidth / 2) * 0.02;
        const moveY = (e.clientY - window.innerHeight / 2) * 0.02;

        visualElements.forEach((element, index) => {
            const depth = (index + 1) * 0.5;
            element.style.transform = `translate(${moveX * depth}px, ${moveY * depth}px)`;
        });
    });

    // Add dramatic entrance animation for visual elements
    visualElements.forEach((element, index) => {
        element.style.opacity = '0';
        element.style.transform = 'scale(0.8)';
        element.style.transition = `opacity 1s ease, transform 1s ease`;
        element.style.transitionDelay = `${0.2 * (index + 1)}s`;

        setTimeout(() => {
            element.style.opacity = '1';
            element.style.transform = 'scale(1)';
        }, 300);
    });

    // Add focus effect for form inputs
    formInputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

        input.addEventListener('blur', function () {
            this.parentElement.classList.remove('focused');
        });
    });
});