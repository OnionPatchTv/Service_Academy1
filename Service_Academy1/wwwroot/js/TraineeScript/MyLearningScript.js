
//MYLEARNINGSCRIPT
function loadModuleContent(filePath) {
    document.getElementById("moduleContentFrame").src = filePath;
}
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
