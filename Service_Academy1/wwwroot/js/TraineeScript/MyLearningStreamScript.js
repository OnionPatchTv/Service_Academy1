document.addEventListener("DOMContentLoaded", function () {
    const dropdownButtons = document.querySelectorAll(".dropdown-btn");
    const descriptionContainer = document.getElementById('programDescriptionContainer'); // Get description container
    const showDescriptionModalButton = document.getElementById('showDescriptionModal'); // Get show button
    const modalDescriptionContent = document.getElementById('modalDescriptionContent');  // Get modal content area

    showDescriptionModalButton.addEventListener('click', () => {
        const modalBody = document.getElementById('modalDescriptionContent');
        modalBody.innerHTML = currentModuleDescription || "No description available."; //Safeguard against null
        $('#moduleDescriptionModal').modal('show');
    });

    const descriptionContainers = document.querySelectorAll('.description-container');
    descriptionContainers.forEach(container => {
        container.addEventListener('click', function (event) {
            const description = this.querySelector('.description');
            if (description) {
                description.classList.toggle('expanded');
                const collapsedHeight = description.scrollHeight; // Get the actual height of the description in its expanded state
                this.style.maxHeight = description.classList.contains('expanded') ? 'max-content' : collapsedHeight + 'px';
            }
        });
    });


    dropdownButtons.forEach((button) => {
        button.addEventListener("click", function (event) {
            event.stopPropagation(); // Prevent bubbling

            this.classList.toggle("active"); // Still toggle the active class for styling

            const content = this.nextElementSibling;
            content.style.display = content.style.display === "block" ? "none" : "block";
        }); // Removed the code to close other dropdowns
    });

    // Close dropdowns only if clicking OUTSIDE the sidebar
    document.addEventListener('click', function (event) {

        const sidebar = document.querySelector('.sidebar');
        if (!sidebar.contains(event.target)) {
            dropdownButtons.forEach(button => {
                if (button.classList.contains('active')) {
                    button.classList.remove('active');
                    button.nextElementSibling.style.display = "none";
                }
            });

        }


    });
    // When the form is submitted
    const submitActivityForm = document.getElementById('submitActivityForm');
    submitActivityForm.addEventListener('submit', function (event) {
        const submissionLinkInput = document.getElementById('submissionLink');
        let submissionLink = submissionLinkInput.value.trim();

        // Check if the link field is empty or contains the default prompt ("No Link Pasted")
        if (!submissionLink || submissionLink === "No Link Pasted") {
            submissionLink = "No Link Pasted";  // Set the value to the default prompt if no link is provided
        }

        // Define regex patterns for allowed links
        const googleDrivePattern = /^https:\/\/(drive\.google\.com|docs\.google\.com|sheets\.google\.com|slides\.google\.com)/;
        const youtubePattern = /^https:\/\/(www\.youtube\.com|youtu\.be)/;
        const canvaPattern = /^https:\/\/(www\.canva\.com)/;

        // If a link is provided and doesn't match any of the valid patterns, show an alert and prevent form submission
        if (submissionLink !== "No Link Pasted" && !googleDrivePattern.test(submissionLink) &&
            !youtubePattern.test(submissionLink) && !canvaPattern.test(submissionLink)) {
            alert("Please enter a valid link. Only Google Drive, YouTube, Google Docs, Sheets, Slides, or Canva links are allowed.");
            event.preventDefault(); // Prevent form submission
            return false;
        }

        // If everything is valid, the form can be submitted
        // You can set the submissionLink value to "No Link Pasted" before submitting if necessary
        submissionLinkInput.value = submissionLink;
    });
    const moduleDescriptionButtons = document.querySelectorAll('[data-bs-target="#moduleDescriptionModal"]'); // Select buttons targeting the modal
    moduleDescriptionButtons.forEach(button => {
        button.addEventListener('click', function (event) {
            const description = this.dataset.description; // Get description from data attribute
            $('#moduleDescriptionModal').on('show.bs.modal', function (event) {
                $('#modalDescriptionContent').html(description); // Populate modal on show
            });
        });
    });
});
function markAsRead(url, element) {
    fetch(url, { method: 'POST' })
        .then(response => {
            if (response.ok) {
                // Update the icon color to green on success
                element.style.color = 'green';
            } else {
                // Handle error responses
                response.text().then(text => console.error('Error:', text));
            }
        })
        .catch(error => console.error('Network error:', error));
}

let currentModuleDescription = ""; // Store the description

function loadModuleContent(filePath, moduleTitle, linkPath, moduleDescription) {
    // Update the iframe source
    document.getElementById("moduleContentFrame").src = filePath;

    // Update the Module Viewer title
    document.getElementById("moduleViewerTitle").textContent = moduleTitle || "Module Viewer";

    // Update the video icon state
    const videoIcon = document.getElementById("videoIcon");
    const moduleVideoLink = document.getElementById("moduleVideoLink");

    if (linkPath && linkPath !== "No Link Available") {
        videoIcon.style.color = "orange"; // Highlight the icon
        videoIcon.title = "View Module Video";
        moduleVideoLink.href = linkPath; // Set the link
        moduleVideoLink.style.pointerEvents = "auto"; // Enable clicking
    } else {
        videoIcon.style.color = "grey"; // Grey out the icon
        videoIcon.title = "No Link Available";
        moduleVideoLink.href = "#"; // Remove the link
        moduleVideoLink.style.pointerEvents = "none"; // Disable clicking
    }
    currentModuleDescription = moduleDescription; // Store the description

    // Display the module description
    const moduleDescriptionElement = document.getElementById("moduleDescription");
    if (moduleDescription && moduleDescription !== "") {
        moduleDescriptionElement.textContent = moduleDescription;
    } else {
        moduleDescriptionElement.textContent = "No description available.";
    }
}
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

const modalBackdrop = document.querySelector('.modal-backdrop'); // Get the backdrop element

$('#moduleDescriptionModal').on('show.bs.modal', function () {
    if (modalBackdrop) {
        modalBackdrop.classList.add('show'); // Add the "show" class
    }
});

$('#moduleDescriptionModal').on('hidden.bs.modal', function () {
    if (modalBackdrop) {
        modalBackdrop.classList.remove('show'); // Remove the "show" class
    }
});
