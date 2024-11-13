document.addEventListener('DOMContentLoaded', function () {
    var showMoreButton = document.getElementById('show-more');
    var showLessButton = document.getElementById('show-less-item');
    var sidebarItems = document.querySelectorAll('.sidebar ul > li');

    var visibleItems = 5; // Number of initially visible items

    // Hide all items after the visible ones
    sidebarItems.forEach(function (item, index) {
        if (index >= visibleItems && !item.id.startsWith("show")) {  // Avoid hiding the Show More/Less items themselves
            item.classList.add('hidden');
        }
    });

    // If there are more items than visibleItems, show the Show More button
    if (sidebarItems.length > visibleItems) {
        showMoreButton.style.display = 'block';
        showLessButton.style.display = 'none';
    } else {
        showMoreButton.style.display = 'none';
        showLessButton.style.display = 'none';
    }

    // Show More button event
    showMoreButton.addEventListener('click', function () {
        sidebarItems.forEach(function (item) {
            item.classList.remove('hidden');
        });
        showMoreButton.style.display = 'none';
        showLessButton.style.display = 'block';
    });

    // Show Less button event
    showLessButton.addEventListener('click', function () {
        sidebarItems.forEach(function (item, index) {
            if (index >= visibleItems && !item.id.startsWith("show")) {  // Avoid hiding the Show More/Less items themselves
                item.classList.add('hidden');
            }
        });
        showLessButton.style.display = 'none';
        showMoreButton.style.display = 'block';
    });
});
