document.addEventListener('DOMContentLoaded', function () {
    const ctx = document.getElementById('roleDistributionChart');
    if (!ctx) return;

    if (typeof roleData === 'undefined' || roleData.length === 0) {
        return;
    }

    const labels = roleData.map(r => r.RoleName);
    const data = roleData.map(r => r.UserCount);

    // Kurumsal Renk Paleti
    const backgroundColors = [
        '#0A192F', // Primary Dark
        '#0F9F8F', // Accent Teal
        '#f59e0b', // Warning Orange
        '#6366f1', // Indigo
        '#10b981', // Emerald
        '#ef4444', // Red
        '#8b5cf6', // Purple
    ];

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: backgroundColors.slice(0, data.length),
                borderWidth: 2,
                borderColor: '#ffffff',
                hoverOffset: 10
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right',
                    labels: {
                        font: {
                            family: "'Inter', sans-serif",
                            size: 13
                        },
                        usePointStyle: true,
                        padding: 20
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(10, 25, 47, 0.9)',
                    titleFont: { family: "'Inter', sans-serif", size: 14 },
                    bodyFont: { family: "'Inter', sans-serif", size: 13 },
                    padding: 12,
                    cornerRadius: 8,
                    displayColors: true,
                    callbacks: {
                        label: function (context) {
                            let label = context.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed !== null) {
                                label += context.parsed + ' Kullanıcı';
                            }
                            return label;
                        }
                    }
                }
            },
            cutout: '65%', // Ortadaki boşluk
            animation: {
                animateScale: true,
                animateRotate: true
            }
        }
    });
});
