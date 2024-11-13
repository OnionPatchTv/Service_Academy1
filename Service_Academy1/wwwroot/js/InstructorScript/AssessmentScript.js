//ASSESSMENT SCRIPT
function openDeleteQuizModal(quizId, quizTitle) {
    // Set the hidden input field to the quizId
    document.getElementById('quizId').value = quizId;
    // Optionally, update the modal message to include the quiz title
    document.getElementById('deleteQuizModalLabel').textContent = `Delete Quiz: ${quizTitle}`;
    // Show the modal
    $('#deleteQuizModal').modal('show');
}

$('.close, .btnNo').on('click', function () {
    $('#deleteModuleModal').modal('hide'); // Close the modal manually
    $('#deleteQuizModal').modal('hide'); // Close quiz modal manually
});
