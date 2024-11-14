//PROGRAM LIST SCRIPT
function enrollInProgram(programId) {
    $.ajax({
        url: '@Url.Action("Enroll", "ProgramList")',
        type: 'POST',
        data: { programId: programId },
        success: function (response) {
            if (response.success) {
                alert(response.message); // Display success message
                // Optionally, update the UI to reflect enrollment
            } else {
                alert("Failed to enroll. Please try again.");
            }
        },
        error: function () {
            alert("Error occurred during enrollment.");
        }
    });
}

