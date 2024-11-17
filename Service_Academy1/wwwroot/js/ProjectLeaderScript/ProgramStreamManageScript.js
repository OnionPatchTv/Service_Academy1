
//PROGRAMSTREAMMANAGESCRIPT
function openDenyModal(enrollmentId) {
    $('#enrollmentId').val(enrollmentId);  // Set enrollment ID in the hidden input
    $('#denyModal').modal('show');         // Show the modal
}
$(document).ready(function () {
    $('#viewGradeModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var enrolleeId = button.data('enrollee-id'); // Extract enrollment ID
        var programId = button.data('program-id'); // Extract program ID
        var traineeName = button.data('trainee-name'); // Extract trainee name
        var modal = $(this);

        // Set the trainee name dynamically
        modal.find('.modal-title #traineeName').text(traineeName);

        // Fetch the grades data using AJAX
        $.ajax({
            url: '/ProjectLeader/GetGrades', // Action URL where you'll fetch the grades
            type: 'GET',
            data: { enrollmentId: enrolleeId, programId: programId },
            success: function (data) {
                console.log('Program ID:', programId);
                console.log('Enrollment ID:', enrolleeId);
                console.log('Received Data:', data);
                console.log(data);  // Log the raw data to the console

                // Check if data is undefined or null
                if (!data || data === undefined || data === null) {
                    console.error('No data received or data is undefined.');
                    return;
                }

                var gradesTableBody = modal.find('#gradesTableBody');
                gradesTableBody.empty(); // Clear any existing rows

                // Check if data is an array and process accordingly
                if (Array.isArray(data)) {
                    data.forEach(function (grade) {
                        var row = '<tr>' +
                            '<td>' + grade.quizTitle + '</td>' +
                            '<td>' + grade.rawScore + '</td>' +
                            '<td>' + grade.totalScore + '</td>' +
                            '<td>' + grade.retries + '</td>' +
                            '<td>' + grade.computedScore + '</td>' +
                            '<td>' + grade.remarks + '</td>' +
                            '</tr>';
                        gradesTableBody.append(row);
                    });
                } else {
                    console.error('Data is not in the expected format.');
                }
            },
            error: function () {
                alert('Error fetching grade data.');
            }
        });
        $.ajax({
            url: '/ProjectLeader/GetTraineeActivities', // Action URL for activities
            type: 'GET',
            data: { enrollmentId: enrolleeId, programId: programId },
            success: function (data) {
                var activitiesTableBody = modal.find('#activitiesTableBody');
                activitiesTableBody.empty(); // Clear existing rows

                if (Array.isArray(data)) {
                    data.forEach(function (activity) {
                        var row = `<tr>
                            <td>${activity.activityTitle}</td>
                            <td>${activity.rawScore}</td>
                            <td>${activity.totalScore}</td>
                            <td>${activity.computedScore}</td>
                        </tr>`;
                        activitiesTableBody.append(row);
                    });
                }
            },
            error: function () {
                alert('Error fetching activities data.');
            }
        });
    });
});

let traineeSortAsc = true;

function toggleSortTrainees() {
    const traineeList = $('.trainee-list');
    const trainees = traineeList.children('.trainee-item').get();

    trainees.sort((a, b) => {
        const nameA = $(a).data('name').toUpperCase();
        const nameB = $(b).data('name').toUpperCase();
        return traineeSortAsc ? nameA.localeCompare(nameB) : nameB.localeCompare(nameA);
    });

    traineeList.append(trainees);
    traineeSortAsc = !traineeSortAsc;
    $('#sortIconTrainees').toggleClass('fa-arrow-up-a-z fa-arrow-down-z-a');
}

let requestSortAsc = true;

function toggleSortRequests() {
    const requestList = $('.request-list');
    const requests = requestList.children('.trainee-request').get();

    requests.sort((a, b) => {
        const nameA = $(a).data('name').toUpperCase();
        const nameB = $(b).data('name').toUpperCase();
        return requestSortAsc ? nameA.localeCompare(nameB) : nameB.localeCompare(nameA);
    });

    requestList.append(requests);
    requestSortAsc = !requestSortAsc;
    $('#sortIconRequests').toggleClass('fa-arrow-up-a-z fa-arrow-down-z-a');
}

$('#statusFilter').on('change', function () {
    const selectedStatus = $(this).val();
    $('.trainee-item').each(function () {
        const itemStatus = $(this).data('status');
        if (selectedStatus === 'all' || selectedStatus === itemStatus.toLowerCase()) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
});
