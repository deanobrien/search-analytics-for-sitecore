window.onload = function () {
    var DailySearchCtx = document.getElementById("DailySearchCanvas").getContext("2d");

    window.DailySearch = new Chart(DailySearchCtx, {
        type: 'bar',
        data: DailySearchData,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    var VisitsCtx = document.getElementById("VisitsCanvas").getContext("2d");

    window.Visits = new Chart(VisitsCtx, {
        type: 'polarArea',
        data: VisitsData,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });

    var DurationCtx = document.getElementById("DurationCanvas").getContext("2d");

    window.Duration = new Chart(DurationCtx, {
        type: 'polarArea',
        data: DurationData,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });
    updateSearchTerm(searchTerm, true)
};