// Handle photo upload and preview
const photoPreview = document.getElementById('photoPreview');
const profilePicInput = document.getElementById('profilePicInput');

photoPreview.addEventListener('click', function () {
    profilePicInput.click();
});

profilePicInput.addEventListener('change', function () {
    const file = profilePicInput.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            photoPreview.style.backgroundImage = `url(${e.target.result})`;
            photoPreview.textContent = ''; // Hide 'Add Photo' text when a photo is selected
        };
        reader.readAsDataURL(file);
    }
});
$("#editProfileButton").click(function () {
    if (!$("#profilePicInput")[0].files.length) {
        // Ensure the profilePath remains null if no file is uploaded
        $("input[name='ProfilePath']").val(null);
    }
});