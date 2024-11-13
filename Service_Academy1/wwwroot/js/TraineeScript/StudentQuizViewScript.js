document.addEventListener('DOMContentLoaded', function () {
    const quizForm = document.getElementById('quizForm');
    const submitQuizButton = document.getElementById('submitQuizButton');
    const confirmSubmitButton = document.getElementById('confirmSubmitButton');
    const answerFields = document.querySelectorAll('.answer-field');

    submitQuizButton.addEventListener('click', function () {
        let isValid = true;

        // Check each answer field for validity
        answerFields.forEach(function (field) {
            if (field.value.trim() === '') {
                isValid = false;
                field.style.borderColor = 'red';  // Highlight empty fields
            } else {
                field.style.borderColor = '';  // Reset border color if field is not empty
            }
        });

        if (isValid) {
            // Show the confirmation modal if all fields are filled
            $('#submitQuizModal').modal('show');
        } else {
            alert('Please fill out all answer fields.');
        }
    });

    confirmSubmitButton.addEventListener('click', function () {
        // Hide the modal and submit the form
        $('#submitQuizModal').modal('hide');
        quizForm.submit();
    });
});
