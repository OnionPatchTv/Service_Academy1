// PROGRAM MANAGEMENT SCRIPT

// Function to toggle the display of date inputs for activation
function toggleDateInput(programId) {
    const dateInputs = document.getElementById(`date-inputs-${programId}`);
    dateInputs.style.display = dateInputs.style.display === 'none' || dateInputs.style.display === '' ? 'block' : 'none';
}

$(document).ready(function () {
    // Automatically hide alerts after a few seconds
    setTimeout(function () {
        $(".alert").fadeOut("slow");
    }, 5000);

    // Add event listeners for search input and filter dropdown
    document.querySelector(".search-container input").addEventListener("input", filterPrograms);
    document.querySelector("#filterAgenda").addEventListener("change", filterPrograms);

    // Attach modal open/close handlers for activation and deactivation
    const buttons = document.querySelectorAll('.toggle-button');
    buttons.forEach(button => {
        button.addEventListener('click', function () {
            const programId = this.id.split('-')[1];
            const isActive = this.classList.contains('activated');

            // Toggle the button text and state
            this.classList.toggle('activated');
            this.classList.toggle('deactivated');
            this.textContent = isActive ? 'Deactivate' : 'Activate';

            // Open the corresponding modal
            const modalId = isActive ? `#deactivateModal-${programId}` : `#activateModal-${programId}`;
            $(modalId).modal('show');
        });
    });
});

// Filter function to show/hide programs based on search and selected agenda
function filterPrograms() {
    // Get values from search input and filter dropdown
    var searchText = document.querySelector(".search-container input").value.toLowerCase();
    var filterAgenda = document.querySelector("#filterAgenda").value.toLowerCase();

    // Get all the program cards
    var programCards = document.querySelectorAll(".program-card");

    // Loop through each program card and apply filters
    programCards.forEach(function (card) {
        // Extract program title and agenda from data attributes
        var programTitle = card.getAttribute("data-title").toLowerCase();
        var programAgenda = card.getAttribute("data-agenda").toLowerCase();

        // Apply search text and agenda filter
        if (
            (programTitle.includes(searchText)) && // Matches search text
            (filterAgenda === "all" || programAgenda === filterAgenda) // Matches selected agenda or "all"
        ) {
            card.style.display = ""; // Show the card
        } else {
            card.style.display = "none"; // Hide the card
        }
    });
}

// Optional handler for modal close actions, if needed
$('#deactivateModal, #activateModal').on('hidden.bs.modal', function () {
    // Additional actions after modal close, if required
});

