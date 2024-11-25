document.addEventListener("DOMContentLoaded", function () {
    const dropdownButtons = document.querySelectorAll(".dropdown-btn");
    const descriptionContainer = document.getElementById('programDescriptionContainer');
    const showDescriptionModalButton = document.getElementById('showDescriptionModal');
    const modalDescriptionContent = document.getElementById('modalDescriptionContent');

    showDescriptionModalButton.addEventListener('click', () => {
        const modalBody = document.getElementById('modalDescriptionContent');
        modalBody.innerHTML = currentModuleDescription || "No description available.";
        $('#moduleDescriptionModal').modal('show');
    });

    const descriptionContainers = document.querySelectorAll('.description-container');
    descriptionContainers.forEach(container => {
        container.addEventListener('click', function () {
            const description = this.querySelector('.description');
            if (description) {
                description.classList.toggle('expanded');
                this.style.maxHeight = description.classList.contains('expanded') ? 'max-content' : `${description.scrollHeight}px`;
            }
        });
    });

    dropdownButtons.forEach((button) => {
        button.addEventListener("click", function (event) {
            event.stopPropagation();
            this.classList.toggle("active");

            const content = this.nextElementSibling;
            content.style.display = content.style.display === "block" ? "none" : "block";
        });
    });

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

    const submitActivityForm = document.getElementById('submitActivityForm');
    submitActivityForm.addEventListener('submit', handleSubmitActivityForm);

    const moduleDescriptionButtons = document.querySelectorAll('[data-bs-target="#moduleDescriptionModal"]');
    moduleDescriptionButtons.forEach(button => {
        button.addEventListener('click', function () {
            // Get the description from the button's dataset
            currentModuleDescription = this.dataset.description || "No description available.";

            // Update the modal content when the modal is triggered
            $('#moduleDescriptionModal').on('show.bs.modal', function () {
                $('#modalDescriptionContent').html(currentModuleDescription);
            });
        });
    });
});

function handleSubmitActivityForm(event) {
    const submissionLinkInput = document.getElementById('submissionLink');
    let submissionLink = submissionLinkInput.value.trim();

    if (!submissionLink || submissionLink === "No Link Pasted") {
        submissionLink = "No Link Pasted";
    }

    const validLinkPatterns = [
        /^https:\/\/(drive\.google\.com|docs\.google\.com|sheets\.google\.com|slides\.google\.com)/,
        /^https:\/\/(www\.youtube\.com|youtu\.be)/,
        /^https:\/\/(www\.canva\.com)/
    ];

    if (submissionLink !== "No Link Pasted" && !validLinkPatterns.some(pattern => pattern.test(submissionLink))) {
        alert("Please enter a valid link. Only Google Drive, YouTube, Google Docs, Sheets, Slides, or Canva links are allowed.");
        event.preventDefault();
        return false;
    }

    submissionLinkInput.value = submissionLink;
}

function loadModuleContent(filePath, moduleTitle, linkPath, moduleDescription) {
    const iframe = document.getElementById('moduleContentFrame');
    const titleElement = document.getElementById('moduleViewerTitle');
    const videoIcon = document.getElementById('videoIcon');
    const moduleVideoLink = document.getElementById('moduleVideoLink');
    const descriptionElement = document.getElementById('moduleDescription');

    iframe.src = filePath;
    titleElement.textContent = moduleTitle || 'Module Viewer';

    if (linkPath && linkPath !== 'No Link Available') {
        videoIcon.style.color = 'orange';
        videoIcon.title = 'View Module Video';
        moduleVideoLink.href = linkPath;
        moduleVideoLink.style.pointerEvents = 'auto';
    } else {
        videoIcon.style.color = 'grey';
        videoIcon.title = 'No Link Available';
        moduleVideoLink.href = '#';
        moduleVideoLink.style.pointerEvents = 'none';
    }

    const modalDescriptionContent = $('#moduleDescriptionModal .modal-body');
    modalDescriptionContent.html(moduleDescription && moduleDescription !== "" ? moduleDescription : "No description available.");
}

function openSubmitActivityModal(activityId, title, description, totalScore) {
    $('#activityIdInput').val(activityId);
    $('#activityTitle').val(title);
    $('#activityDirection').val(description);
    $('#totalScore').text(totalScore);

    $('#submissionLink').val('');
    $('#submissionFileDisplay').text('No file uploaded');

    $.get('/Assessment/GetSubmissionDetails', { activitiesId: activityId }, function (data) {
        if (data) {
            if (data.filePath) {
                $('#submissionFileDisplay').text(data.filePath);
            }
            if (data.linkPath) {
                $('#submissionLink').val(data.linkPath);
            }
        }
    }).fail(function () {
        console.log("No existing submission found.");
    });

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
function markAsRead(url, element) {
    console.log("URL:", url); // Log to check if URL is passed correctly
    fetch(url, { method: 'POST' })
        .then(response => {
            if (response.ok) {
                element.style.color = 'green';
            } else {
                return response.text().then(text => console.error('Error:', text));
            }
        })
        .catch(error => console.error('Network error:', error));
}