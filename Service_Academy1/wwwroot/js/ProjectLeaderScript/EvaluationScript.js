function loadEvaluationCharts(programId) {
    $.ajax({
        url: `/Evaluation/GetEvaluationResults?programId=${programId}`,
        method: 'GET',
        success: function (data) {
            // Define color schemes
            const chartColors = {
                performance: { background: 'rgba(54, 162, 235, 0.2)', border: 'rgba(54, 162, 235, 1)' },
                satisfaction: { background: 'rgba(75, 192, 192, 0.2)', border: 'rgba(75, 192, 192, 1)' },
                quality: { background: 'rgba(255, 206, 86, 0.2)', border: 'rgba(255, 206, 86, 1)' },
                continuity: { background: 'rgba(153, 102, 255, 0.2)', border: 'rgba(153, 102, 255, 1)' }
            };

            // Iterate through the data to create charts
            data.forEach(item => {
                // Check if 'category' is defined and not null
                if (item.category) {
                    let categoryKey = item.category.toLowerCase();
                    let chartId = `${categoryKey}Chart`;
                    let colors = chartColors[categoryKey];

                    // Only create a chart if colors are defined for this category
                    if (colors) {
                        createChart(chartId, item.category, item.averageRating, colors);
                    } else {
                        console.warn(`No color scheme defined for category: ${item.category}`);
                    }
                } else {
                    console.warn("Encountered null or undefined category in data:", item);
                }
            });
        },
        error: function () {
            console.error("Error loading evaluation data.");
        }
    });
}

function createChart(chartId, category, averageRating, color) {
    var ctx = document.getElementById(chartId)?.getContext('2d');
    if (ctx) {
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: [category],
                datasets: [{
                    label: 'Average Rating',
                    data: [averageRating],
                    backgroundColor: color.background,
                    borderColor: color.border,
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        max: 5
                    }
                }
            }
        });
    } else {
        console.warn(`Chart element with ID '${chartId}' not found in the DOM.`);
    }
}
