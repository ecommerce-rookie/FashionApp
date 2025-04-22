document.addEventListener('DOMContentLoaded', function () {
    // Form data object to store all user inputs
    const formData = {
        firstName: '',
        lastName: '',
        dateOfBirth: '',
        stylePreferences: [],
        contact: {
            phoneNumber: '',
            province: '',
            provinceCode: '',
            district: '',
            districtCode: '',
            ward: '',
            wardCode: '',
            streetAddress: ''
        },
        profilePhoto: null
    };

    // Current step tracking
    let currentStep = 1;
    const totalSteps = 4; // Reduced from 5 to 4 steps

    // API URLs for Vietnam address
    const API_PROVINCE_URL = 'https://provinces.open-api.vn/api';

    // Prevent enter for form
    document.getElementById("onboarding-form").addEventListener("keydown", (e) => {
        if (e.key == "Enter") {
            e.preventDefault();

            if (currentStep < totalSteps) {
                const nextBtn = document.getElementById(`step${currentStep}Next`);
                if (nextBtn) {
                    nextBtn.click();
                }
            }

        }
    });

    // Update progress bar and step indicator
    function updateProgress() {
        const progressBar = document.getElementById('progressBar');
        const currentStepText = document.getElementById('currentStepText');

        const progressPercentage = (currentStep / totalSteps) * 100;
        progressBar.style.width = `${progressPercentage}%`;

        if (currentStep <= totalSteps) {
            currentStepText.textContent = `STEP ${currentStep} OF ${totalSteps}`;
        } else {
            currentStepText.textContent = 'COMPLETE';
        }
    }

    // Show a specific step
    function showStep(stepNumber) {
        // Hide all steps
        const steps = document.querySelectorAll('.onboarding-step');
        steps.forEach(step => {
            step.classList.remove('active');
        });

        // Show the current step
        const currentStepElement = document.getElementById(stepNumber === 5 ? 'completion' : `step${stepNumber}`);
        currentStepElement.classList.add('active');

        // Update progress
        currentStep = stepNumber;
        updateProgress();

        // Scroll to top
        window.scrollTo(0, 0);
    }

    // Show error message
    function showError(elementId, message) {
        const errorElement = document.getElementById(elementId);
        errorElement.textContent = message;
        errorElement.classList.add('show');
    }

    // Clear error message
    function clearError(elementId) {
        const errorElement = document.getElementById(elementId);
        errorElement.textContent = '';
        errorElement.classList.remove('show');
    }

    // Step 1: Personal Details
    const step1NextBtn = document.getElementById('step1Next');
    step1NextBtn.addEventListener('click', function () {
        console.log('Step 1 Next button clicked');
        const firstName = document.getElementById('firstName').value;
        const lastName = document.getElementById('lastName').value;

        let isValid = true;

        // Validate first name
        if (!firstName) {
            showError('firstNameError', 'FIRST NAME REQUIRED');
            isValid = false;
        } else {
            clearError('firstNameError');
        }

        // Validate last name
        if (!lastName) {
            showError('lastNameError', 'LAST NAME REQUIRED');
            isValid = false;
        } else {
            clearError('lastNameError');
        }

        if (isValid) {
            // Save data
            formData.firstName = firstName;
            formData.lastName = lastName;

            // Move to next step
            showStep(2);

            console.log('Step 1 Next button clicked next');
        }

        console.log('Step 1 Next button clicked completed');
    });

    // Step 2: Style Preferences
    const styleOptions = document.querySelectorAll('.style-option');
    styleOptions.forEach(option => {
        option.addEventListener('click', function () {
            const styleId = this.getAttribute('data-style');

            if (this.classList.contains('selected')) {
                // Deselect
                this.classList.remove('selected');
                formData.stylePreferences = formData.stylePreferences.filter(style => style !== styleId);
            } else {
                // Select
                this.classList.add('selected');
                formData.stylePreferences.push(styleId);
            }

            // Clear any error
            clearError('styleError');
        });
    });

    const step2BackBtn = document.getElementById('step2Back');
    step2BackBtn.addEventListener('click', function () {
        showStep(1);
    });

    const step2NextBtn = document.getElementById('step2Next');
    step2NextBtn.addEventListener('click', function () {
        if (formData.stylePreferences.length === 0) {
            showError('styleError', 'PLEASE SELECT AT LEAST ONE STYLE');
        } else {
            showStep(3);
        }
    });

    // Step 3: Address and Phone Information
    // Fetch provinces from API
    async function fetchProvinces() {
        try {
            const response = await fetch(`${API_PROVINCE_URL}/p/`);
            if (!response.ok) throw new Error('Failed to fetch provinces');
            const data = await response.json();
            return data;
        } catch (error) {
            console.error('Error fetching provinces:', error);
            return [];
        }
    }

    // Fetch districts from API
    async function fetchDistricts(provinceCode) {
        try {
            const response = await fetch(`${API_PROVINCE_URL}/p/${provinceCode}?depth=2`);
            if (!response.ok) throw new Error('Failed to fetch districts');
            const data = await response.json();
            return data.districts || [];
        } catch (error) {
            console.error('Error fetching districts:', error);
            return [];
        }
    }

    // Fetch wards from API
    async function fetchWards(districtCode) {
        try {
            const response = await fetch(`${API_PROVINCE_URL}/d/${districtCode}?depth=2`);
            if (!response.ok) throw new Error('Failed to fetch wards');
            const data = await response.json();
            return data.wards || [];
        } catch (error) {
            console.error('Error fetching wards:', error);
            return [];
        }
    }

    // Populate dropdown with items
    function populateDropdown(dropdown, items, inputField, nextInputField, codeProperty, nameProperty, callback) {
        dropdown.innerHTML = '';

        if (items.length === 0) {
            const noResults = document.createElement('div');
            noResults.className = 'dropdown-item';
            noResults.textContent = 'No results found';
            dropdown.appendChild(noResults);
            return;
        }

        items.forEach(item => {
            const dropdownItem = document.createElement('div');
            dropdownItem.className = 'dropdown-item';
            dropdownItem.textContent = item[nameProperty || 'name'];
            dropdownItem.addEventListener('click', () => {
                inputField.value = item[nameProperty || 'name'];

                // Store the selected code
                if (inputField === document.getElementById('province')) {
                    formData.contact.province = item[nameProperty || 'name'];
                    formData.contact.provinceCode = item[codeProperty || 'code'];

                    // Reset district and ward
                    document.getElementById('district').value = '';
                    document.getElementById('ward').value = '';
                    formData.contact.district = '';
                    formData.contact.districtCode = '';
                    formData.contact.ward = '';
                    formData.contact.wardCode = '';

                    // Enable district input
                    document.getElementById('district').disabled = false;
                } else if (inputField === document.getElementById('district')) {
                    formData.contact.district = item[nameProperty || 'name'];
                    formData.contact.districtCode = item[codeProperty || 'code'];

                    // Reset ward
                    document.getElementById('ward').value = '';
                    formData.contact.ward = '';
                    formData.contact.wardCode = '';

                    // Enable ward input
                    document.getElementById('ward').disabled = false;
                } else if (inputField === document.getElementById('ward')) {
                    formData.contact.ward = item[nameProperty || 'name'];
                    formData.contact.wardCode = item[codeProperty || 'code'];
                }

                // Hide dropdown
                dropdown.classList.remove('show');

                // Clear any error
                clearError(inputField.id + 'Error');

                // Execute callback if provided
                if (callback) callback(item[codeProperty || 'code']);
            });
            dropdown.appendChild(dropdownItem);
        });
    }

    // Initialize address dropdowns
    async function initAddressDropdowns() {
        const provinceInput = document.getElementById('province');
        const provinceDropdown = document.getElementById('provinceDropdown');
        const districtInput = document.getElementById('district');
        const districtDropdown = document.getElementById('districtDropdown');
        const wardInput = document.getElementById('ward');
        const wardDropdown = document.getElementById('wardDropdown');

        // Fetch provinces on page load
        const provinces = await fetchProvinces();

        // Province dropdown
        provinceInput.addEventListener('focus', () => {
            populateDropdown(provinceDropdown, provinces, provinceInput, districtInput, 'code', 'name', async (provinceCode) => {
                const districts = await fetchDistricts(provinceCode);
                // Pre-populate districts for the selected province
                populateDropdown(districtDropdown, districts, districtInput, wardInput, 'code', 'name', async (districtCode) => {
                    const wards = await fetchWards(districtCode);
                    // Pre-populate wards for the selected district
                    populateDropdown(wardDropdown, wards, wardInput, null, 'code', 'name');
                });
            });
            provinceDropdown.classList.add('show');
        });

        provinceInput.addEventListener('input', async () => {
            const searchTerm = provinceInput.value.toLowerCase();
            const filteredProvinces = provinces.filter(province =>
                province.name.toLowerCase().includes(searchTerm)
            );
            populateDropdown(provinceDropdown, filteredProvinces, provinceInput, districtInput, 'code', 'name', async (provinceCode) => {
                const districts = await fetchDistricts(provinceCode);
                populateDropdown(districtDropdown, districts, districtInput, wardInput, 'code', 'name', async (districtCode) => {
                    const wards = await fetchWards(districtCode);
                    populateDropdown(wardDropdown, wards, wardInput, null, 'code', 'name');
                });
            });
            provinceDropdown.classList.add('show');
        });

        // District dropdown
        districtInput.addEventListener('focus', async () => {
            if (!formData.contact.provinceCode) {
                showError('districtError', 'PLEASE SELECT A PROVINCE FIRST');
                return;
            }

            const districts = await fetchDistricts(formData.contact.provinceCode);
            populateDropdown(districtDropdown, districts, districtInput, wardInput, 'code', 'name', async (districtCode) => {
                const wards = await fetchWards(districtCode);
                populateDropdown(wardDropdown, wards, wardInput, null, 'code', 'name');
            });
            districtDropdown.classList.add('show');
        });

        districtInput.addEventListener('input', async () => {
            if (!formData.contact.provinceCode) return;

            const searchTerm = districtInput.value.toLowerCase();
            const districts = await fetchDistricts(formData.contact.provinceCode);
            const filteredDistricts = districts.filter(district =>
                district.name.toLowerCase().includes(searchTerm)
            );
            populateDropdown(districtDropdown, filteredDistricts, districtInput, wardInput, 'code', 'name', async (districtCode) => {
                const wards = await fetchWards(districtCode);
                populateDropdown(wardDropdown, wards, wardInput, null, 'code', 'name');
            });
            districtDropdown.classList.add('show');
        });

        // Ward dropdown
        wardInput.addEventListener('focus', async () => {
            if (!formData.contact.districtCode) {
                showError('wardError', 'PLEASE SELECT A DISTRICT FIRST');
                return;
            }

            const wards = await fetchWards(formData.contact.districtCode);
            populateDropdown(wardDropdown, wards, wardInput, null, 'code', 'name');
            wardDropdown.classList.add('show');
        });

        wardInput.addEventListener('input', async () => {
            if (!formData.contact.districtCode) return;

            const searchTerm = wardInput.value.toLowerCase();
            const wards = await fetchWards(formData.contact.districtCode);
            const filteredWards = wards.filter(ward =>
                ward.name.toLowerCase().includes(searchTerm)
            );
            populateDropdown(wardDropdown, filteredWards, wardInput, null, 'code', 'name');
            wardDropdown.classList.add('show');
        });

        // Close dropdowns when clicking outside
        document.addEventListener('click', (e) => {
            if (!e.target.closest('.dropdown-wrapper')) {
                provinceDropdown.classList.remove('show');
                districtDropdown.classList.remove('show');
                wardDropdown.classList.remove('show');
            }
        });
    }

    // Phone number validation
    function isValidPhoneNumber(phoneNumber) {
        // Vietnamese phone number format: +84xxxxxxxxx or 0xxxxxxxxx (10-11 digits)
        const phoneRegex = /^(\+84|0)[3-9][0-9]{8,9}$/;
        return phoneRegex.test(phoneNumber);
    }

    const step3BackBtn = document.getElementById('step3Back');
    step3BackBtn.addEventListener('click', function () {
        showStep(2);
    });

    const step3NextBtn = document.getElementById('step3Next');
    step3NextBtn.addEventListener('click', function () {
        const phoneNumber = document.getElementById('phoneNumber').value;
        const province = document.getElementById('province').value;
        const district = document.getElementById('district').value;
        const ward = document.getElementById('ward').value;
        const streetAddress = document.getElementById('streetAddress').value;

        let isValid = true;

        // Validate phone number
        if (!phoneNumber) {
            showError('phoneNumberError', 'PHONE NUMBER REQUIRED');
            isValid = false;
        } else if (!isValidPhoneNumber(phoneNumber)) {
            showError('phoneNumberError', 'INVALID PHONE NUMBER FORMAT');
            isValid = false;
        } else {
            clearError('phoneNumberError');
            formData.contact.phoneNumber = phoneNumber;
        }

        // Validate province
        if (!province) {
            showError('provinceError', 'PROVINCE REQUIRED');
            isValid = false;
        } else {
            clearError('provinceError');
        }

        // Validate district
        if (!district) {
            showError('districtError', 'DISTRICT REQUIRED');
            isValid = false;
        } else {
            clearError('districtError');
        }

        // Validate ward
        if (!ward) {
            showError('wardError', 'WARD REQUIRED');
            isValid = false;
        } else {
            clearError('wardError');
        }

        // Validate street address
        if (!streetAddress) {
            showError('streetAddressError', 'STREET ADDRESS REQUIRED');
            isValid = false;
        } else {
            clearError('streetAddressError');
            formData.contact.streetAddress = streetAddress;
        }

        if (isValid) {
            // Move to next step (now step 4 - Profile Photo)
            showStep(4);
        }
    });

    // Step 4: Profile Photo (previously Step 5)
    const photoPreview = document.getElementById('photoPreview');
    const photoUpload = document.getElementById('photoUpload');
    const selectPhotoBtn = document.getElementById('selectPhotoBtn');
    const removePhotoBtn = document.getElementById('removePhotoBtn');

    selectPhotoBtn.addEventListener('click', function () {
        photoUpload.click();
    });

    photoPreview.addEventListener('click', function () {
        photoUpload.click();
    });

    photoUpload.addEventListener('change', function () {
        if (this.files && this.files[0]) {
            const reader = new FileReader();

            reader.onload = function (e) {
                // Create image element
                const img = document.createElement('img');
                img.src = e.target.result;

                // Clear preview and add image
                photoPreview.innerHTML = '';
                photoPreview.appendChild(img);

                // Save photo data
                formData.profilePhoto = e.target.result;

                // Show remove button
                removePhotoBtn.classList.remove('hidden');

                // Update button text
                selectPhotoBtn.textContent = 'CHANGE PHOTO';
            };

            reader.readAsDataURL(this.files[0]);
        }
    });

    removePhotoBtn.addEventListener('click', function () {
        // Clear preview
        photoPreview.innerHTML = `
            <div class="photo-placeholder">
                <span class="plus-icon">+</span>
                <span class="upload-text">ADD PHOTO</span>
            </div>
        `;

        // Reset file input
        photoUpload.value = '';

        // Clear saved photo
        formData.profilePhoto = null;

        // Hide remove button
        this.classList.add('hidden');

        // Reset button text
        selectPhotoBtn.textContent = 'SELECT PHOTO';
    });

    const step4BackBtn = document.getElementById('step4Back');
    step4BackBtn.addEventListener('click', function () {
        showStep(3);
    });

    const skipPhotoBtn = document.getElementById('skipPhotoBtn');
    skipPhotoBtn.addEventListener('click', function () {
        formData.profilePhoto = null;
        completeOnboarding();
    });

    const step4NextBtn = document.getElementById('step4Next');
    step4NextBtn.addEventListener('click', function () {
        completeOnboarding();
    });

    // Complete onboarding and show summary
    function completeOnboarding() {
        // Update user name in completion screen
        document.getElementById('userFirstName').textContent = formData.firstName;

        // Update style summary
        const styleSummary = document.getElementById('styleSummary');
        if (formData.stylePreferences.length > 0) {
            styleSummary.textContent = formData.stylePreferences
                .map(style => style.charAt(0).toUpperCase() + style.slice(1))
                .join(', ');
        }

        // Update contact summary
        const contactSummary = document.getElementById('contactSummary');
        contactSummary.innerHTML = `
            Address: ${formData.contact.streetAddress}, ${formData.contact.ward}, ${formData.contact.district}, ${formData.contact.province}<br>
            Phone: ${formData.contact.phoneNumber}
        `;

        // Update profile photo in summary
        const profilePhotoSummary = document.getElementById('profilePhotoSummary');
        if (formData.profilePhoto) {
            profilePhotoSummary.innerHTML = `<img src="${formData.profilePhoto}" alt="Profile Photo">`;
        } else {
            profilePhotoSummary.innerHTML = `
                <div class="profile-photo-placeholder">
                    <span>No photo selected</span>
                </div>
            `;
        }

        // Show completion screen
        showStep(5);
    }

    // Initialize
    updateProgress();
    initAddressDropdowns();

    // Add subtle parallax effect to visual elements
    const visualElements = document.querySelectorAll('.floating-element');
    document.addEventListener('mousemove', function (e) {
        if (window.innerWidth <= 768) return;

        const moveX = (e.clientX - window.innerWidth / 2) * 0.02;
        const moveY = (e.clientY - window.innerHeight / 2) * 0.02;

        visualElements.forEach((element, index) => {
            const depth = (index + 1) * 0.5;
            element.style.transform = `translate(${moveX * depth}px, ${moveY * depth}px)`;
        });
    });
});