$(document).ready(function () {
    $('#verifyForm').submit(function (event) {
        event.preventDefault(); // Prevent default form submission

        var certificateId = $('#certificateId').val();

        // Your existing fetch code here...
        fetch('/Home/VerifyCertificates?certificateId=' + certificateId, {
            method: 'GET'
        })
            .then(response => response.json())
            .then(data => {
                $('#verificationMessage').text(data.message);
                $('#verificationResultModal').modal('show');
            })
            .catch(error => {
                console.error('Error:', error);
                $('#verificationMessage').text("An error occurred. Please try again later.");
                $('#verificationResultModal').modal('show');
            });
    });
});