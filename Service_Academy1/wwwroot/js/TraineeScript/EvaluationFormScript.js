$(document).ready(function () {
    $('#confirmSubmit').click(function () {
        // Trigger form submission after confirmation
        $('#evaluationForm').submit();
        $('#submitModal').modal('hide');
    });
});

document.addEventListener('DOMContentLoaded', () => {
    const chartCanvas = document.getElementById('evaluationChart');
    if (chartCanvas) {
        const labels = JSON.parse(chartCanvas.dataset.labels);
        const values = JSON.parse(chartCanvas.dataset.values);

        const ctx = chartCanvas.getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Average Ratings',
                    data: values,
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
});
