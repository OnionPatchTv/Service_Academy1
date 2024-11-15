$(document).ready(function () {
    $('#confirmSubmit').click(function () {
        // Trigger form submission after confirmation
        $('#evaluationForm').submit();
        $('#submitModal').modal('hide');
    });
});