

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
    const systemUsageCanvas = document.getElementById('system-usage-chart');
    if (systemUsageCanvas) {
        const labels = JSON.parse(systemUsageCanvas.dataset.labels);
        const logins = JSON.parse(systemUsageCanvas.dataset.logins);
        const quizSubmissions = JSON.parse(systemUsageCanvas.dataset.quizSubmissions);
        const activitySubmissions = JSON.parse(systemUsageCanvas.dataset.activitySubmissions);
        const programEnrollments = JSON.parse(systemUsageCanvas.dataset.programEnrollments);

        const ctx = systemUsageCanvas.getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Logins',
                        data: logins,
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Quiz Submissions',
                        data: quizSubmissions,
                        backgroundColor: 'rgba(153, 102, 255, 0.2)',
                        borderColor: 'rgba(153, 102, 255, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Activity Submissions',
                        data: activitySubmissions,
                        backgroundColor: 'rgba(255, 159, 64, 0.2)',
                        borderColor: 'rgba(255, 159, 64, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Program Enrollments',
                        data: programEnrollments,
                        backgroundColor: 'rgba(255, 99, 132, 0.2)',
                        borderColor: 'rgba(255, 99, 132, 1)',
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
});

document.getElementById('generate-report-form').addEventListener('submit', async function (e) {
    e.preventDefault(); // Prevent form submission

    // Get user input
    const reportType = document.getElementById('report-type').value;
    const startDate = document.getElementById('start-date').value;
    const endDate = document.getElementById('end-date').value;

    if (reportType === 'pdf') {
        await generatePDFReport(startDate, endDate);
    } else {
        alert('Spreadsheet export is under construction.');
    }
});

async function generatePDFReport(startDate, endDate) {
    const { jsPDF } = window.jspdf;
    const pdf = new jsPDF({ orientation: 'landscape', unit: 'px', format: 'a4' });

    let margin = 40; // Margin for spacing
    let yPosition = margin; // Start position for content

    // Add Title and Filters
    pdf.setFontSize(18);
    pdf.setFont('helvetica', 'bold');
    pdf.text('Analytics Report', margin, yPosition);
    yPosition += 30;

    pdf.setFontSize(12);
    pdf.setFont('helvetica', 'normal');
    pdf.text(`Start Date: ${startDate || 'N/A'}`, margin, yPosition);
    yPosition += 15;
    pdf.text(`End Date: ${endDate || 'N/A'}`, margin, yPosition);
    yPosition += 15;

    // Add Recommendations
    const recommendation = document.querySelector('.chart-container-ai p').innerText;
    pdf.setFontSize(14);
    pdf.setFont('helvetica', 'bold');
    pdf.text('Arli AI Recommendations:', margin, yPosition);
    yPosition += 20;

    pdf.setFontSize(12);
    pdf.setFont('helvetica', 'normal');
    const lines = pdf.splitTextToSize(recommendation, 720); // Wrap text to fit width
    pdf.text(lines, margin, yPosition);
    yPosition += lines.length * 15 + 20;

    // Capture and Add Charts
    const chartIds = [
        'activity-performance-program-chart',
        'quiz-performance-program-chart',
        'completion-rate-chart',
        'program-performance-chart',
        'overall-program-progress-chart',
        'system-usage-chart',
    ];

    for (const chartId of chartIds) {
        const canvas = document.getElementById(chartId);
        if (canvas) {
            const chartImage = await html2canvas(canvas).then((canvas) => canvas.toDataURL('image/png'));
            pdf.addImage(chartImage, 'PNG', margin, yPosition, 340, 150); // Scale and position
            yPosition += 320; // Add spacing for the next chart

            if (yPosition > 550) { // Start a new page if content overflows
                pdf.addPage();
                yPosition = margin;
            }
        }
    }

    // Save the PDF
    pdf.save('Analytics_Report.pdf');
}
