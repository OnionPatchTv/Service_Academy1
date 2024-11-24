

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
    console.log("Form submission prevented.");
    console.log("Form submission prevented.");
    const reportType = document.getElementById('report-type').value;
    const reportDate = document.getElementById('report-date').value || 'N/A';
    console.log(`Report Type: ${reportType}, Report Date: ${reportDate}`);
    if (reportType === 'pdf') {
        try {
            await generatePDFReport(reportDate);
            console.log("PDF report generation complete.");
        } catch (err) {
            console.error("Error during PDF generation:", err);
        }
    } else {
        alert('Spreadsheet export is under construction.');
    }
});
document.getElementById('generate-report-form').addEventListener('submit', async function (e) {
    e.preventDefault(); // Prevent the default form submission
    console.log("Form submission prevented.");

    const reportType = document.getElementById('report-type').value;
    const reportDate = document.getElementById('report-date').value || 'N/A';
    console.log(`Report Type: ${reportType}, Report Date: ${reportDate}`);

    if (reportType === 'pdf') {
        try {
            await generatePDFReport(reportDate);
            console.log("PDF report generation complete.");
        } catch (err) {
            console.error("Error during PDF generation:", err);
        }
    } else {
        alert('Spreadsheet export is under construction.');
    }

    return false; // Prevent any further form action
});

async function generatePDFReport(reportDate) {
    try {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF({ orientation: 'landscape', unit: 'px', format: 'a4' });
        const margin = 40;
        const pageWidth = doc.internal.pageSize.getWidth();
        const pageHeight = doc.internal.pageSize.getHeight();

        // Title and Date (First Page)
        let yPosition = margin;
        doc.setFontSize(18);
        doc.setFont('helvetica', 'bold');
        doc.text('Analytics Report', pageWidth / 2, yPosition, { align: 'center' });
        yPosition += 30;
        doc.setFontSize(12);
        doc.text(`Report Date: ${reportDate}`, margin, yPosition);
        yPosition += 20;

        // Recommendations
        const recommendationElement = document.querySelector('.chart-container-ai p');
        if (recommendationElement) {
            const recommendation = recommendationElement.innerText;
            doc.setFontSize(14);
            doc.text('Arli AI Recommendations:', margin, yPosition);
            yPosition += 20;
            const lines = doc.splitTextToSize(recommendation, pageWidth - 2 * margin);
            doc.text(lines, margin, yPosition);
        } else {
            console.warn("Recommendation text not found.");
        }

        // Add Charts on Separate Pages
        const chartIds = [
            'activity-performance-program-chart',
            'quiz-performance-program-chart',
            'completion-rate-chart',
            'program-performance-chart',
            'overall-program-progress-chart',
            'system-usage-chart'
        ];

        for (const chartId of chartIds) {
            doc.addPage(); // Start a new page for each chart
            yPosition = margin; // Reset yPosition for the new page
            const canvas = document.getElementById(chartId);
            if (canvas) {
                try {
                    const imgData = await html2canvas(canvas).then(canvas => canvas.toDataURL('image/png'));
                    const imgProps = doc.getImageProperties(imgData);
                    const imgWidth = pageWidth - 2 * margin;
                    const imgHeight = (imgProps.height * imgWidth) / imgProps.width;

                    const xPosition = margin;
                    const centeredYPosition = (pageHeight - imgHeight) / 2;
                    doc.addImage(imgData, 'PNG', xPosition, centeredYPosition, imgWidth, imgHeight);
                } catch (error) {
                    console.error(`Error capturing chart ${chartId}:`, error);
                    doc.setFontSize(10);
                    doc.setTextColor(255, 0, 0);
                    doc.text(`Error capturing chart ${chartId}`, margin, yPosition);
                    yPosition += 15;
                    doc.setTextColor(0, 0, 0);
                }
            } else {
                console.warn(`Chart with ID ${chartId} not found.`);
            }
        }

        doc.save('Analytics_Report.pdf');
    } catch (err) {
        console.error("Error in generatePDFReport:", err);
    }
}