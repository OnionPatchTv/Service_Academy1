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
                $('#verificationResultModal').modal('show'); // Show modal on success or failure

                // Close modal after 3 seconds (optional)
                setTimeout(function () {
                    $('#verificationResultModal').modal('hide'); // Hide the modal after 3 seconds
                }, 3000); // Adjust the time as needed
            })
            .catch(error => {
                console.error('Error:', error);
                $('#verificationMessage').text("An error occurred. Please try again later.");
                $('#verificationResultModal').modal('show');
            });
    });

    // Close modal when the close button is clicked (optional)
    $('#closeModalButton').click(function () {
        $('#verificationResultModal').modal('hide');
    });
});
