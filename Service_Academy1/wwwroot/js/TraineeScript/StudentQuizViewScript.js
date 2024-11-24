document.addEventListener('DOMContentLoaded', function () {
    const quizForm = document.getElementById('quizForm');
    const submitQuizButton = document.getElementById('submitQuizButton');
    const answerFields = document.querySelectorAll('.answer-field');

    submitQuizButton.addEventListener('click', function () {
        let isValid = true;
        let emptyQuestions = []; // Array to store indices of empty questions

        // Check each answer field for validity
        answerFields.forEach(function (field, index) {
            if (field.value.trim() === '') {
                isValid = false;
                field.style.borderColor = 'red';
                emptyQuestions.push(index + 1); // Store question number (index + 1)
            } else {
                field.style.borderColor = '';
            }
        });

        if (isValid) {
            $('#submitQuizModal').modal('show');
        } else {
            let errorMessage = "Please answer the following questions:\n";
            emptyQuestions.forEach(questionNumber => {
                errorMessage += `- Question ${questionNumber}\n`;
            });
            alert(errorMessage);
        }
    })

    confirmSubmitButton.addEventListener('click', function () {
        // Hide the modal and submit the form
        $('#submitQuizModal').modal('hide');
        quizForm.submit();
    });
});
