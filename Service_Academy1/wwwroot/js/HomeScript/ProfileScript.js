// Initially hide the profile picture input and make it disabled
document.getElementById('profilePic').disabled = true;
document.getElementById('profilePic').style.display = 'none';

// Toggle Edit Profile
document.getElementById('editProfileButton').addEventListener('click', function () {
    const formFields = document.querySelectorAll('.form input');
    const profilePicInput = document.getElementById('profilePic');
    const editableAbout = document.querySelector('.editable-about');

    // Check if in edit mode
    if (document.body.classList.contains('edit-mode')) {
        // Validate before allowing save
        if (validateForm()) {
            saveProfileData();
            toggleEditMode(false);
        } else {
            // Replaced alert with the error toast
            showErrorNotification("Please correct the highlighted errors before saving.");
        }
    } else {
        toggleEditMode(true);
    }
});

// Function to toggle edit mode
function toggleEditMode(enable) {
    const formFields = document.querySelectorAll('.form input');
    const profilePicInput = document.getElementById('profilePic');
    const editableAbout = document.querySelector('.editable-about');
    const button = document.getElementById('editProfileButton');

    if (enable) {
        // Enable editing
        document.body.classList.add('edit-mode');
        formFields.forEach(field => field.removeAttribute('readonly'));
        profilePicInput.style.display = 'block';
        profilePicInput.disabled = false;
        editableAbout.contentEditable = "true";
        button.textContent = "Save Changes";
    } else {
        // Disable editing
        document.body.classList.remove('edit-mode');
        formFields.forEach(field => field.setAttribute('readonly', 'readonly'));
        profilePicInput.style.display = 'none';
        profilePicInput.disabled = true;
        editableAbout.contentEditable = "false";
        button.textContent = "Edit Profile";
    }
}

// Save Profile Data
function saveProfileData() {
    const fullName = document.getElementById('fullName').value;
    const email = document.getElementById('email').value;

    const profileFullNameElement = document.getElementById('profileFullName');
    const profileEmailElement = document.getElementById('profileEmail');

    // Update profile section with the saved full name and email
    profileFullNameElement.textContent = fullName;
    profileEmailElement.textContent = email;

    // Log saved data for demonstration (could send to a server)
    console.log('Saved Data:', { fullName, email });

    // Show the save notification
    showSaveNotification();
}

// Handle Profile Picture Change
document.getElementById('profilePic').addEventListener('change', function (e) {
    const file = e.target.files[0];

    // Allowed image formats (MIME types)
    const allowedFormats = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];

    if (file) {
        // Check if the file is of an allowed format
        if (!allowedFormats.includes(file.type)) {
            showErrorNotification('Invalid file format. Please upload a JPEG, PNG, GIF, or WEBP image.');
            return; // Exit the function early if the format is not allowed
        }

        const reader = new FileReader();
        reader.onload = function (event) {
            // Set the new profile picture as a background image
            document.querySelector('.profile-pic-label i').style.backgroundImage = `url(${event.target.result})`;
            document.querySelector('.profile-pic-label i').style.fontSize = "0"; // Hide icon when picture is set
        };
        reader.onerror = function () {
            showErrorNotification('Error reading file. Please try again.'); // Show toast for file read error
        };
        reader.readAsDataURL(file);
    }
});

// Event listener to limit characters in editable-about
document.querySelector('.editable-about').addEventListener('textInput', function (e) {
    const editableAbout = document.querySelector('.editable-about');
    const maxLength = 160;

    // Get the current content and check its length
    const currentText = editableAbout.textContent;

    // If length exceeds the max length, prevent input and show error
    if (currentText.length >= maxLength) {
        // Prevent the default action (inserting the new character)
        e.preventDefault();

        // Show the error notification
        showErrorNotification("You have reached the maximum character limit of 160.");
    }
});



// Function to show the notification
function showSaveNotification() {
    const notification = document.getElementById('save-notification');
    notification.classList.add('show');

    // Hide the notification after 3 seconds
    setTimeout(() => {
        notification.classList.remove('show');
    }, 3000);
}

// Show Error Notification
function showErrorNotification(message) {
    const errorNotification = document.getElementById('error-notification');
    errorNotification.textContent = message; // Set the error message
    errorNotification.classList.add('show');

    // Hide the error notification after 3 seconds
    setTimeout(() => {
        errorNotification.classList.remove('show');
    }, 3000);
}

// Validate Form with Error Notification
function validateForm() {
    let isValid = true;
    let errorMessage = "Please correct the highlighted errors.";

    // Validate Full Name
    const fullNameField = document.getElementById('fullName');
    if (fullNameField.value.trim() === "") {
        fullNameField.classList.add('error');
        isValid = false;
    } else {
        fullNameField.classList.remove('error');
    }

    // Validate Email
    const emailField = document.getElementById('email');
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailPattern.test(emailField.value)) {
        emailField.classList.add('error');
        isValid = false;
    } else {
        emailField.classList.remove('error');
    }

    // Validate Phone Number
    const phoneField = document.getElementById('phone');
    const phonePattern = /^\d{11}$/;
    if (!phonePattern.test(phoneField.value)) {
        phoneField.classList.add('error');
        isValid = false;
    } else {
        phoneField.classList.remove('error');
    }

    // Validate Age
    const ageField = document.getElementById('age');
    if (ageField.value < 1 || ageField.value > 120) {
        ageField.classList.add('error');
        isValid = false;
    } else {
        ageField.classList.remove('error');
    }

    if (!isValid) {
        showErrorNotification(errorMessage); // Show toast with error message
    }

    return isValid;
}
