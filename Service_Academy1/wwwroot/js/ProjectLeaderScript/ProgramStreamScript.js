// Toggle the display of the announcement input field
function toggleAnnouncementInput() {
    const inputField = document.getElementById('announcement-input');
    const isHidden = inputField.style.display === 'none';
    inputField.style.display = isHidden ? 'block' : 'none';

    if (isHidden) {
        document.getElementById('announcement-textarea').focus();
    }
}

// Post a new announcement
function postAnnouncement() {
    const announcementText = document.getElementById('announcement-textarea').innerText.trim();
    if (!announcementText) return;

    const postsContainer = document.getElementById('posts');
    const newPost = document.createElement('div');
    newPost.className = 'post';
    newPost.innerHTML = `
        <div class="post-header">
            <div class="post-user">
                <img src="/images/profile.png" alt="User Profile Picture" class="post-user-img">
                <span class="post-user-name">Leigh Smith</span>
            </div>
            <span class="post-timestamp">Just now</span>
        </div>
        <p class="post-content">${announcementText}</p>
    `;
    postsContainer.prepend(newPost);

    document.getElementById('announcement-textarea').innerText = '';
    toggleAnnouncementInput();
}

// Cancel the announcement post
function cancelPost() {
    document.getElementById('announcement-textarea').innerText = '';
    toggleAnnouncementInput();
}

// Apply formatting to the selected text
function applyFormat(formatType) {
    const textarea = document.getElementById('announcement-textarea');
    const selection = window.getSelection();

    if (selection.rangeCount > 0) {
        const range = selection.getRangeAt(0);
        const selectedText = range.toString();

        if (selectedText.length > 0) {
            const span = document.createElement('span');
            span.style.fontWeight = formatType === 'bold' ? 'bold' : 'normal';
            span.style.fontStyle = formatType === 'italic' ? 'italic' : 'normal';
            span.style.textDecoration = formatType === 'underline' ? 'underline' : 'none';
            span.textContent = selectedText;

            range.deleteContents();
            range.insertNode(span);

            selection.removeAllRanges();
            selection.addRange(range);
        }
    }
    textarea.focus();
}

// Handle dropdown toggling
function setupDropdowns() {
    const dropdownButtons = document.querySelectorAll('.dropdown-btn');

    dropdownButtons.forEach((button) => {
        button.addEventListener('click', function (event) {
            event.stopPropagation();
            this.classList.toggle('active');

            const content = this.nextElementSibling;
            content.style.display = content.style.display === 'block' ? 'none' : 'block';
        });
    });

    document.addEventListener('click', (event) => {
        const sidebar = document.querySelector('.sidebar');
        if (!sidebar.contains(event.target)) {
            dropdownButtons.forEach((button) => {
                if (button.classList.contains('active')) {
                    button.classList.remove('active');
                    button.nextElementSibling.style.display = 'none';
                }
            });
        }
    });
}

// Load module content into the viewer
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

    // Populate the modal description
    const modalDescriptionContent = $('#moduleDescriptionModal .modal-body');  // Select the modal body
    if (moduleDescription && moduleDescription !== "") {
        modalDescriptionContent.html(moduleDescription); // Use html() to preserve formatting
    } else {
        modalDescriptionContent.html("No description available.");
    }
}

// Validate Google Drive or YouTube links
function validateLink(input) {
    const value = input.value.trim();
    const googleDrivePattern = /^https:\/\/(drive\.google\.com|docs\.google\.com)/;
    const youtubePattern = /^https:\/\/(www\.youtube\.com|youtu\.be)/;

    if (value && !googleDrivePattern.test(value) && !youtubePattern.test(value)) {
        alert('Please enter a valid Google Drive or YouTube link.');
        return false;
    }
    return true;
}

// Set up module forms with link validation
function setupModuleForms() {
    const forms = [
        document.querySelector('#uploadModuleModal form'),
        document.querySelector('#updateModuleModal form')
    ];

    forms.forEach((form) => {
        if (form) {
            form.addEventListener('submit', (event) => {
                const linkInput = form.querySelector('#linkPath');
                if (!validateLink(linkInput)) {
                    event.preventDefault();
                }
            });
        }
    });
}

// Initialize the application
document.addEventListener('DOMContentLoaded', () => {
    setupDropdowns();
    setupModuleForms();

    setTimeout(() => {
        document.querySelectorAll('.alert').forEach(alert => alert.style.display = 'none');
    }, 5000);

    document.addEventListener('keydown', (event) => {
        if ((event.ctrlKey || event.metaKey) && (event.key === 'p' || event.key === 's')) {
            event.preventDefault();
            alert('Printing and saving are disabled for this content.');
        }
    });
});

// Toggle the description view
function toggleDescription(element) {
    const container = element.closest('.description-container');
    const isExpanded = element.classList.toggle('expanded');
    container.style.maxHeight = isExpanded ? `${element.scrollHeight}px` : 0;
}

// Open the Update Module modal and populate its fields
function openUpdateModuleModal(moduleId, moduleTitle, currentLinkPath, moduleDescription, currentFilePath) {
    console.log("Module ID:", moduleId);
    console.log("Module Title:", moduleTitle);
    console.log("Link Path:", currentLinkPath);
    console.log("Module Description:", moduleDescription);
    console.log("Current File Path:", currentFilePath);

    // Splitting title
    const [prefix, titleWithoutPrefix] = moduleTitle.split(': ');

    // Set modal fields
    $('#updateModuleModalLabel').text(prefix);
    $('#moduleIdInput').val(moduleId);
    $('#moduleTitleInput').val(titleWithoutPrefix);
    $('#moduleDescriptionInput').val(moduleDescription || '');
    $('#linkPath').val(currentLinkPath || '');

    // Set current file name text
    if (currentFilePath) {
        $('#currentFileName').text(`Current file: ${currentFilePath}`);
    } else {
        $('#currentFileName').text('No file uploaded');
    }

    // Reinitialize modal and show it (Bootstrap's modal requires manual triggering in some cases)
    $('#updateModuleModal').modal('dispose');  // Dispose of any old modal instances
    $('#updateModuleModal').modal('show');     // Show the modal
}



// Open the Delete Module modal and set its values
function openDeleteModuleModal(moduleId, moduleTitle) {
    $('#deleteModuleModal').find('input[name="moduleId"]').val(moduleId);
    $('#deleteModuleModal').find('.modal-title').text('Delete Module: ' + moduleTitle);
    $('#deleteModuleModal').modal('show');
}

// Open the Update Activity modal and populate its fields
function openUpdateActivityModal(activityId, activityTitle, activityDescription, activityScore) {
    $('#activitiesIdInput').val(activityId);
    $('#activityTitleInput').val(activityTitle);
    $('#activityDescriptionInput').val(activityDescription); // Description textarea
    $('#activityScoreInput').val(activityScore);

    $('#updateActivityModal').modal('show');
}

// Open the Delete Activity modal and set its values
function openDeleteActivityModal(activityId, activityTitle) {
    $('input[name="activitiesId"]').val(activityId);
    $('#deleteActivityModalLabel').text('Delete Activity: ' + activityTitle);
    $('#deleteActivityModal').modal('show');
}

// Close modals when close button is clicked
$('.close-btn').on('click', function () {
    $('#uploadModuleModal').modal('hide');
    $('#createAssessmentModal').modal('hide');
    $('#updateModuleModal').modal('hide');
    $('#deleteModuleModal').modal('hide');
    $('#updateActivityModal').modal('hide');
    $('#deleteActivityModal').modal('hide');
});
