document.addEventListener('DOMContentLoaded', function () {
    const steps = document.querySelectorAll('.checkout-step');
    const navItems = document.querySelectorAll('.checkout-nav__item');

    function showStep(stepNumber) {
        steps.forEach(step => {
            step.classList.add('hidden');
        });

        // Show the requested step
        document.getElementById('step' + stepNumber).classList.remove('hidden');

        // Update navigation indicators
        navItems.forEach(item => {
            const itemStep = parseInt(item.getAttribute('data-step'));

            // Remove current class from all items
            item.classList.remove('current');

            // Add active class to current and previous steps
            if (itemStep <= stepNumber) {
                item.classList.add('active');
            } else {
                item.classList.remove('active');
            }

            // Add current class to current step
            if (itemStep === stepNumber) {
                item.classList.add('current');
            }

        });

        // Update right column class for step 3
        const rightColumn = document.querySelector('.checkout-page__right');
        if (stepNumber === 3) {
            rightColumn.classList.add('checkout-page__right_more');
        } else {
            rightColumn.classList.remove('checkout-page__right_more');
        }
    }

    const continueToStep2 = document.getElementById('continueToStep2');
    if (continueToStep2) {
        continueToStep2.addEventListener('click', function (e) {
            e.preventDefault();
            showStep(2);
        });
    }
    const continueToStep3 = document.getElementById('continueToStep3');
    if (continueToStep3) {
        continueToStep3.addEventListener('click', function (e) {
            e.preventDefault();
            showStep(3);
        });
    }

    const backToStep1 = document.getElementById('backToStep1');
    if (backToStep1) {
        backToStep1.addEventListener('click', function (e) {
            e.preventDefault();
            showStep(1);
        });
    }

    showStep(1);

})