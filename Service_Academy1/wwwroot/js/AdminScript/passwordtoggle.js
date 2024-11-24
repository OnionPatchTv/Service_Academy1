function togglePasswordVisibility(passwordId) {
    var passwordField = document.getElementById(passwordId);
    var eyeIcon = document.getElementById(passwordId === 'password' ? 'eyeIcon' : 'eyeIcon2');

    if (passwordField.type === "password") {
        passwordField.type = "text";
        eyeIcon.classList.remove('fa-eye');
        eyeIcon.classList.add('fa-eye-slash');
    } else {
        passwordField.type = "password";
        eyeIcon.classList.remove('fa-eye-slash');
        eyeIcon.classList.add('fa-eye');
    }
}