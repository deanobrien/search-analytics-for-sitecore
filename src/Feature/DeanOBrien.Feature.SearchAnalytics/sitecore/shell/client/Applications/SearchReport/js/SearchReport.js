
// CHART INITIAL DATASETS

// GLOBAL
var DailySearchData = {
    labels: "tbc",
    datasets: [
        {
            label: "tbc",
            fillColor: "rgba(151,187,205,0.2)",
            strokeColor: "rgba(151,187,205,1)",
            pointColor: "rgba(151,187,205,1)",
            pointStrokeColor: "#fff",
            pointHighlightFill: "#fff",
            pointHighlightStroke: "rgba(151,187,205,1)",
            data: "tbc"
        }
    ]
}
var VisitsData = {
    datasets: [
        {
            backgroundColor: "tbc",
            data: "tbc",
            hoverOffset: 4
        }
    ]
}
var DurationData = {
    datasets: [
        {
            //label: "label tbc",
            backgroundColor: "tbc",
            data: "tbc",
            hoverOffset: 4
        }
    ]
}
// SINGLE ITEM
var searchRankingData = {
    labels: "tbc",
    datasets: [
        {
            label: "tbc",
            fillColor: "rgba(151,187,205,0.2)",
            strokeColor: "rgba(151,187,205,1)",
            pointColor: "rgba(151,187,205,1)",
            pointStrokeColor: "#fff",
            pointHighlightFill: "#fff",
            pointHighlightStroke: "rgba(151,187,205,1)",
            data: "tbc"
        }
    ]
}
var clickThroughData = {
    labels: "tbc",
    datasets: [
        {

            label: "tbc",
            fillCoModelor: "rgba(151,187,205,0.2)",
            strokeColor: "rgba(151,187,205,1)",
            pointColor: "rgba(151,187,205,1)",
            pointStrokeColor: "#fff",
            pointHighlightFill: "#fff",
            pointHighlightStroke: "rgba(151,187,205,1)",
            data: "tbc"
        }
    ]
}

// COMMON FUNCTIONS

function showSection(sectionName) {
    $(".searchterm-section").hide();
    $(".searchterm-button").parent().removeClass("active");
    $("#" + sectionName).show();
    $("#" + sectionName + "-button").addClass("active");
}
function show(sectionName, global) {
    $(".engagement-section").hide();
    $(".engagement-button").removeClass("btn-primary");
    if (global) {
        $(".engagement-chart").hide();
    }
    if (sectionName == "AdjustedDurationRankingsForTerm") {
        $("#duration-button").addClass("btn-primary");
        showAdjustedDurationRankingsForTerm(window.Duration, searchTerm, global);
        if (global) {
            $("#duration").show();
        }
    } else {
        $("#visit-button").addClass("btn-primary");
        showAdjustedVisitsRankingsForTerm(window.Visits, searchTerm, global);
        if (global) {
            $("#visits").show();
        }
    }
    $("#" + sectionName).show();
}

// DATA RETRIEVAL FUNCTIONS

// SINGLE ITEM
function getSearchRankingData(chart, itemId, searchTerm) {
    searchRankingData.datasets[0].label = searchTerm;
    var api = "/sitecore/shell/sitecore/client/applications/searchreport/GetSearchRankingRecordsForItemAndTermOverTime/" + itemId + "/" + searchTerm + "/";
    $.getJSON(api, function (data) {
    }).then(function (data) {
        searchRankingData.labels = data.Labels;
        searchRankingData.datasets[0].data = data.Values;
        chart.data = searchRankingData;
        chart.update();
    });
}
function getClickThroughData(chart, itemId, searchTerm) {
    clickThroughData.datasets[0].label = searchTerm;
    var api = "/sitecore/shell/sitecore/client/applications/searchreport/GetClickThroughsForItemAndTermOverTime/" + itemId + "/" + searchTerm + "/";
    $.getJSON(api, function (data) {
    }).then(function (data) {
        clickThroughData.labels = data.Labels;
        clickThroughData.datasets[0].data = data.Values;
        chart.data = clickThroughData;
        chart.update();
    });
}
// GLOBAL
function getDailySearchData(chart, searchTerm) {
    DailySearchData.datasets[0].label = searchTerm;
    $.getJSON("/sitecore/shell/sitecore/client/applications/searchreport/GetDailySearchesForTermOverTime/" + searchTerm + "/", function (data) {
    }).then(function (data) {
        DailySearchData.labels = data.Labels;
        DailySearchData.datasets[0].data = data.Values;
        chart.data = DailySearchData;
        chart.update();
    });
}
function showAdjustedDurationRankingsForTerm(chart, searchTerm, global) {
    var adjustedDurationRankingsForTerm = $("#AdjustedDurationRankingsForTerm");
    adjustedDurationRankingsForTerm.empty();
    $.getJSON("/sitecore/shell/sitecore/client/applications/searchreport/GetAllItemsEngagementForTermOrderByTotal/" + searchTerm, function (data) {
        var rankComparisons = data.RankComparisons;
        $.each(rankComparisons, function (index, elem) {
            var template = $('#AdjustedDurationRankingsForTermTpl').html();
            var html = Mustache.to_html(template, elem);
            adjustedDurationRankingsForTerm.append(html);
        });
    }).then(function (data) {
        if (global) {
            DurationData.datasets[0].label = searchTerm;
            DurationData.labels = data.DurationLabels;
            DurationData.datasets[0].backgroundColor = data.Backgrounds;
            DurationData.datasets[0].data = data.DurationValues;
            chart.data = DurationData;
            chart.update();
        }
    });
}
function showAdjustedVisitsRankingsForTerm(chart, searchTerm, global) {
    var adjustedVisitRankingsForTerm = $("#AdjustedVisitRankingsForTerm");
    adjustedVisitRankingsForTerm.empty();
    $.getJSON("/sitecore/shell/sitecore/client/applications/searchreport/GetAllItemsEngagementForTermOrderByVisit/" + searchTerm, function (data) {
        var rankComparisons = data.RankComparisons;
        $.each(rankComparisons, function (index, elem) {
            var template = $('#AdjustedVisitRankingsForTermTpl').html();
            var html = Mustache.to_html(template, elem);
            adjustedVisitRankingsForTerm.append(html);
        });
    }).then(function (data) {
        if (global) {
            VisitsData.datasets[0].label = searchTerm;
            VisitsData.labels = data.VisitsLabels;
            VisitsData.datasets[0].backgroundColor = data.Backgrounds;
            VisitsData.datasets[0].data = data.VisitsValues;
            chart.data = VisitsData;
            chart.update();
        }
    });
}