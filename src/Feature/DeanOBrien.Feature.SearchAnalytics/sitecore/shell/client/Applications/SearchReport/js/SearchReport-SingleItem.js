window.onload = function () {
    var ctx = document.getElementById("canvas").getContext("2d");
    window.SearchRanking = new Chart(ctx, {
        type: 'line',
        data: searchRankingData,
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    reverse: true,
                    min: 0,
                    max: 20
                }
            }
        }
    });

    var clickThroughCtx = document.getElementById("clickThroughCanvas").getContext("2d");
    window.ClickThrough = new Chart(clickThroughCtx, {
        type: 'bar',
        data: clickThroughData,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    updateSearchTerm(searchTerm, false)
};