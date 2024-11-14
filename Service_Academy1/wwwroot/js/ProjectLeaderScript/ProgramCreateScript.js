//PROGRAM CREATE SCRIPT

// Handle photo upload and preview
const photoInput = document.getElementById('photoInput');
const photoPreview = document.getElementById('photoPreview');

photoInput.addEventListener('change', function () {
    const file = photoInput.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            photoPreview.style.backgroundImage = `url(${e.target.result})`;
            photoPreview.style.backgroundSize = 'cover';
            photoPreview.style.backgroundPosition = 'center';
            photoPreview.textContent = ''; // Hide the text once a photo is uploaded
        }
        reader.readAsDataURL(file);
    } else {
        photoPreview.style.backgroundImage = 'none';
        photoPreview.textContent = 'Add Photo'; // Reset text if no photo
    }
});

// Make the photo-preview box clickable to trigger file input
photoPreview.addEventListener('click', function () {
    photoInput.click();
});

// Handle form submission
const programForm = document.getElementById('programForm');

programForm.addEventListener('submit', function (e) {
    //e.preventDefault();

    const title = document.getElementById('programTitle').value;
    const instructor = document.getElementById('programInstructor').value;
    const description = document.getElementById('programDescription').value;

    if (title && instructor && description) {
        alert(`Program Added: \nTitle: ${title} \nInstructor: ${instructor} \nDescription: ${description}`);

        // Optionally clear form after submission
        programForm.reset();
        photoPreview.style.backgroundImage = 'none';
        photoPreview.textContent = 'No photo uploaded';
    } else {
        alert('Please fill out all fields.');
    }
});

$(document).ready(function () {
    // Optional: If you'd like to keep auto-hiding the alert after a few seconds while still allowing manual close
    setTimeout(function () {
        $(".alert").fadeOut("slow");
    }, 5000);
});

