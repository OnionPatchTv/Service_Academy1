﻿//PROGRAMSTREAMSCRIPT
function toggleAnnouncementInput() {
    const inputField = document.getElementById('announcement-input');
    inputField.style.display = inputField.style.display === 'none' ? 'block' : 'none';
    if (inputField.style.display === 'block') {
        document.getElementById('announcement-textarea').focus(); // Focus on the textarea
    }
}

// Function to post announcement
function postAnnouncement() {
    const announcementText = document.getElementById('announcement-textarea').innerHTML; // Get HTML content
    if (announcementText) {
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
        postsContainer.prepend(newPost); // Add new post to the top
        document.getElementById('announcement-textarea').innerHTML = ''; // Clear the textarea
        toggleAnnouncementInput(); // Hide the input field
    }
}

// Function to cancel announcement
function cancelPost() {
    document.getElementById('announcement-textarea').innerHTML = ''; // Clear the textarea
    toggleAnnouncementInput(); // Hide the input field
}

// Apply formatting to the selected text only
function applyFormat(formatType) {
    const textarea = document.getElementById('announcement-textarea');
    const selection = window.getSelection();

    if (selection.rangeCount > 0) {
        const range = selection.getRangeAt(0);
        const selectedText = range.toString();

        // Check if there is selected text before applying formatting
        if (selectedText.length > 0) {
            const span = document.createElement('span');
            span.style.fontWeight = formatType === 'bold' ? 'bold' : 'normal';
            span.style.fontStyle = formatType === 'italic' ? 'italic' : 'normal';
            span.style.textDecoration = formatType === 'underline' ? 'underline' : 'none';
            span.textContent = selectedText;

            // Replace the selected text with the formatted span
            range.deleteContents();
            range.insertNode(span);

            // Collapse the selection to the end of the inserted node to continue typing normally
            selection.removeAllRanges();
            selection.addRange(range);
        }
    }

    // Refocus on the textarea after formatting
    textarea.focus();
}
function postAnnouncement() {
    alert("This program is archived and read-only.");
}

// Function to insert bullet list
function loadModuleContent(filePath) {
    document.getElementById("moduleContentFrame").src = filePath;
}
function openUpdateModuleModal(moduleId, moduleTitle) {
    const prefix = moduleTitle.split(': ')[0]; // Extract "Module X"
    const titleWithoutPrefix = moduleTitle.split(': ')[1]; // Extract title without "Module X"

    // Set the module number in the modal header and the title in the input
    document.getElementById('updateModuleModalLabel').textContent = prefix;
    document.getElementById('moduleIdInput').value = moduleId;
    document.getElementById('moduleTitleInput').value = titleWithoutPrefix;

    // Show the modal
    $('#updateModuleModal').modal('show');
}

function openDeleteModuleModal(moduleId, moduleTitle) {
    // Set the values in the modal before showing it
    $('#deleteModuleModal').find('input[name="moduleId"]').val(moduleId);
    $('#deleteModuleModal').find('.modal-title').text('Delete Module: ' + moduleTitle);
    $('#deleteModuleModal').modal('show');
}
$('.close-btn').on('click', function () {
    $('#uploadModuleModal').modal('hide');
    $('#createAssessmentModal').modal('hide');
    $('#updateModuleModal').modal('hide');
    $('#deleteModuleModal').modal('hide');
});

$(document).ready(function () {
    // Optional: If you'd like to keep auto-hiding the alert after a few seconds while still allowing manual close
    setTimeout(function () {
        $(".alert").fadeOut("slow");
    }, 5000);
});

document.addEventListener("keydown", function (event) {
    // Block Ctrl+P (print) and Ctrl+S (save as)
    if ((event.ctrlKey || event.metaKey) && (event.key === 'p' || event.key === 's')) {
        event.preventDefault();
        alert("Printing and saving are disabled for this content.");
    }
});

