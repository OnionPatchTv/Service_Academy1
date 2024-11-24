document.getElementById("verifyForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var certificateId = document.getElementById("certificateId").value;

    fetch('/Home/VerifyCertificates?certificateId=' + certificateId, {
        method: 'GET'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            if (data.isValid) {
                document.getElementById("verificationMessage").textContent = data.message;
            } else {
                document.getElementById("verificationMessage").textContent = data.message;
            }
            document.getElementById("verificationResultModal").style.display = "block";
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById("verificationMessage").textContent = "An error occurred. Please try again later.";
            document.getElementById("verificationResultModal").style.display = "block";
        });

});

// Close the modal
function closeModal() {
    document.getElementById("verificationResultModal").style.display = "none";
}
