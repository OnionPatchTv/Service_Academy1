//MYLEARNINGSCRIPT

function filterPrograms() {
    // Get values from search input and filter dropdowns
    var searchText = document.querySelector(".search-container input").value.toLowerCase();
    var filterAgenda = document.querySelector("#filterAgenda").value.toLowerCase(); // Agenda filter

    // Get all the program cards
    var programCards = document.querySelectorAll(".program-card");

    // Loop through each program card and apply filters
    programCards.forEach(function (card) {
        // Extract program details (e.g., title, instructor name, agenda)
        var programTitle = card.querySelector("h3").textContent.toLowerCase();
        var programAgenda = card.getAttribute("data-agenda").toLowerCase(); // Get the agenda from the data attribute

        // Apply search text and agenda filter
        if (
            (programTitle.includes(searchText)) &&
            (filterAgenda === "all" || programAgenda === filterAgenda)
        ) {
            card.style.display = ""; // Show the card
        } else {
            card.style.display = "none"; // Hide the card
        }
    });
}

// Add event listeners for the search input and filter changes
document.querySelector(".search-container input").addEventListener("input", filterPrograms);
document.querySelector("#filterAgenda").addEventListener("change", filterPrograms);
function openSubmitActivityModal(activityId, title, description, totalScore) {
    $('#activityIdInput').val(activityId);
    $('#activityTitle').val(title);
    $('#activityDirection').val(description);
    $('#totalScore').text(totalScore);

    // Clear previous inputs
    $('#submissionLink').val('');
    $('#submissionFileDisplay').text('No file uploaded'); // Display text for the uploaded file

    // Fetch existing submission details
    $.get('/Assessment/GetSubmissionDetails', { activitiesId: activityId }, function (data) {
        if (data) {
            if (data.filePath) {
                // Display the file name or path in the modal
                $('#submissionFileDisplay').text(data.filePath);
            }
            if (data.linkPath) {
                $('#submissionLink').val(data.linkPath);
            }
        }
    }).fail(function () {
        console.log("No existing submission found.");
    });

    // Fetch raw and computed scores
    $.get('/Assessment/GetScores', { activitiesId: activityId }, function (data) {
        if (data) {
            $('#rawScore').text('Raw Score: ' + data.rawScore);
            $('#computedScore').text('Computed Score: ' + data.computedScore);
        } else {
            $('#rawScore').text('Raw Score: 0');
            $('#computedScore').text('Computed Score: 0');
        }
    }).fail(function () {
        console.log("No scores found.");
    });

    $('#submitActivityModal').modal('show');
}


$(document).ready(function () {
    // Optional: If you'd like to keep auto-hiding the alert after a few seconds while still allowing manual close
    setTimeout(function () {
        $(".alert").fadeOut("slow");
    }, 5000);
});

function toggleDescription(element) {
    const container = element.closest('.description-container'); // Get the parent container
    element.classList.toggle('expanded');

    // Adjust container height after transition (important for smooth collapse)
    container.style.maxHeight = element.classList.contains('expanded') ? element.scrollHeight + 'px' : 'calc(1.5em * 4)';
}