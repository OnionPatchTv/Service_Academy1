//account management
let programs = [
    { name: "BSc Computer Science", type: "Degree", status: "Active", enrolledStudents: 350 },
    { name: "Web Development Bootcamp", type: "Non-Degree", status: "Active", enrolledStudents: 150 },
];

// Function to switch between tabs
document.getElementById('overviewTab').addEventListener('click', function () {
    document.getElementById('programOverview').style.display = 'block';
    document.getElementById('createProgram').style.display = 'none';
    this.classList.add('active');
    document.getElementById('createProgramTab').classList.remove('active');
});

document.getElementById('createProgramTab').addEventListener('click', function () {
    document.getElementById('programOverview').style.display = 'none';
    document.getElementById('createProgram').style.display = 'block';
    this.classList.add('active');
    document.getElementById('overviewTab').classList.remove('active');
});

// Function to update the program table
function updateProgramTable() {
    const programList = document.getElementById('programList');
    programList.innerHTML = '';

    programs.forEach(program => {
        const row = `
            <tr>
                <td>${program.name}</td>
                <td>${program.type}</td>
                <td>${program.status}</td>
                <td>${program.enrolledStudents}</td>
                <td>
                    <button class="btn btn-primary btn-sm" onclick="editProgram('${program.name}')">Edit</button>
                    <button class="btn btn-danger btn-sm" onclick="deleteProgram('${program.name}')">Delete</button>
                </td>
            </tr>
        `;
        programList.innerHTML += row;
    });
}

// Function to filter programs
function filterPrograms() {
    const searchValue = document.getElementById('searchProgram').value.toLowerCase();
    const filteredPrograms = programs.filter(program =>
        program.name.toLowerCase().includes(searchValue) ||
        program.type.toLowerCase().includes(searchValue)
    );

    const programList = document.getElementById('programList');
    programList.innerHTML = '';

    filteredPrograms.forEach(program => {
        const row = `
            <tr>
                <td>${program.name}</td>
                <td>${program.type}</td>
                <td>${program.status}</td>
                <td>${program.enrolledStudents}</td>
                <td>
                    <button class="btn btn-primary btn-sm" onclick="editProgram('${program.name}')">Edit</button>
                    <button class="btn btn-danger btn-sm" onclick="deleteProgram('${program.name}')">Delete</button>
                </td>
            </tr>
        `;
        programList.innerHTML += row;
    });
}

// Function to create a new program
function createProgram(event) {
    event.preventDefault();
    const newProgram = {
        name: document.getElementById('programName').value,
        type: document.getElementById('programType').value,
        status: 'Active',
        enrolledStudents: 0
    };
    programs.push(newProgram);
    updateProgramTable();
    alert('Program created successfully!');
}

// Initialize program table on page load
document.addEventListener('DOMContentLoaded', updateProgramTable);

function filterAccounts() {
    var searchText = document.getElementById("searchAccount").value.toLowerCase();
    var roleFilter = document.getElementById("filterRole").value.toLowerCase();

    var tableRows = document.getElementById("accountList").getElementsByTagName("tr");

    for (var i = 0; i < tableRows.length; i++) {
        var username = tableRows[i].getElementsByTagName("td")[0].textContent.toLowerCase();
        var email = tableRows[i].getElementsByTagName("td")[1].textContent.toLowerCase();
        var fullName = tableRows[i].getElementsByTagName("td")[2].textContent.toLowerCase();
        var role = tableRows[i].getElementsByTagName("td")[3].textContent.toLowerCase();

        if (
            (username.indexOf(searchText) > -1 ||
                email.indexOf(searchText) > -1 ||
                fullName.indexOf(searchText) > -1) &&
            (roleFilter === "" || role === roleFilter)
        ) {
            tableRows[i].style.display = "";
        } else {
            tableRows[i].style.display = "none";
        }
    }
}

// JavaScript for handling the Edit button and populating the modal
$('#editAccountModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    var userId = button.data('userid');
    var username = button.data('username');
    var email = button.data('email');
    var fullName = button.data('fullname');

    // Update the modal's content
    var modal = $(this);
    modal.find('#editUserId').val(userId);
    modal.find('#editUserName').val(username);
    modal.find('#editEmail').val(email);
    modal.find('#editFullName').val(fullName);
    // You might need additional logic to pre-select the correct role in the 'editRole' dropdown
});

