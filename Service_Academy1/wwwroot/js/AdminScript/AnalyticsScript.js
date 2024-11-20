

// Program Performance Chart
document.addEventListener('DOMContentLoaded', () => {
    const chartCanvas = document.getElementById('program-performance-chart');
    if (chartCanvas) {
        // Fetch labels and values from data attributes
        const labels = JSON.parse(chartCanvas.dataset.labels);
        const values = JSON.parse(chartCanvas.dataset.values);

        const ctx = chartCanvas.getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Average Evaluation Rating',
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
    const completionCanvas = document.getElementById('completion-rate-chart');
    if (completionCanvas) {
        const labels = JSON.parse(completionCanvas.dataset.labels);
        const values = JSON.parse(completionCanvas.dataset.values);

        const ctx = completionCanvas.getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Program Completion Rate (%)',
                    data: values,
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 2
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        max: 100
                    }
                }
            }
        });
    }
    const programQuizCanvas = document.getElementById('quiz-performance-program-chart');
    if (programQuizCanvas) {
        const labels = JSON.parse(programQuizCanvas.dataset.labels);
        const scores = JSON.parse(programQuizCanvas.dataset.scores);
        const retries = JSON.parse(programQuizCanvas.dataset.retries);

        const ctx = programQuizCanvas.getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Average Score',
                        data: scores,
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Average Retries',
                        data: retries,
                        backgroundColor: 'rgba(255, 159, 64, 0.2)',
                        borderColor: 'rgba(255, 159, 64, 1)',
                        borderWidth: 1
                    }
                ]
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
    const activityCanvas = document.getElementById('activity-performance-program-chart');
    if (activityCanvas) {
        const labels = JSON.parse(activityCanvas.dataset.labels);
        const scores = JSON.parse(activityCanvas.dataset.scores);
        const completionRates = JSON.parse(activityCanvas.dataset.completionrates);

        const ctx = activityCanvas.getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Average Score',
                        data: scores,
                        backgroundColor: 'rgba(153, 102, 255, 0.2)',
                        borderColor: 'rgba(153, 102, 255, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Completion Rate (%)',
                        data: completionRates,
                        backgroundColor: 'rgba(255, 159, 64, 0.2)',
                        borderColor: 'rgba(255, 159, 64, 1)',
                        borderWidth: 1
                    }
                ]
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
    const progressCanvas = document.getElementById('overall-program-progress-chart');
    if (progressCanvas) {
        const labels = JSON.parse(progressCanvas.dataset.labels);
        const progress = JSON.parse(progressCanvas.dataset.progress);

        const ctx = progressCanvas.getContext('2d');
        new Chart(ctx, {
            type: 'bar', // You can change this to 'line' if you prefer a line chart
            data: {
                labels: labels,
                datasets: [{
                    label: 'Overall Program Progress',
                    data: progress,
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        max: 100 // If the progress is a percentage, you may want to cap the y-axis at 100%
                    }
                }
            }
        });
    }
});

