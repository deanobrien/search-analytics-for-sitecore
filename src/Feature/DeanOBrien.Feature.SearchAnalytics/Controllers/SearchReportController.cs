using DeanOBrien.Feature.SearchAnalytics.Models;
using DeanOBrien.Foundation.DataAccess.SearchAnalytics;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Sitecore.Mvc.Controllers;
using System.Runtime.Caching;

namespace DeanOBrien.Feature.SearchAnalytics.Controllers
{
    public class SearchReportController : SitecoreController
    {
        private readonly ISearchStore _searchStore;

        public SearchReportController(ISearchStore searchStore)
        {
            _searchStore = searchStore;
        }
        public ActionResult SearchReport(string id = null, string searchTerm = null, string display = null)
        {
            if (!string.IsNullOrWhiteSpace(id)) id = EnsureUpperAndCurlBrackets(id);

            var isUpToDate = MemoryCache.Default.Get(DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()) == null ? false : true;
            if (!isUpToDate)
            {
                _searchStore.PopulateTempAdjustedRank();
                MemoryCache.Default.Set(DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString(), "1", DateTimeOffset.Now.AddMinutes(10080));
            }


            var model = new SearchReportViewModel();
            model.TotalSearchesForAllTerms = new List<Tuple<int, string>>();
            model.SearchRankings = new List<Tuple<string, int, DateTime>>();
            model.TotalClickThroughs = new List<Tuple<string, int>>();
            model.AverageDurations = new List<Tuple<decimal, int, string, decimal>>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model.SearchTerm = searchTerm;
            }
            if (!string.IsNullOrWhiteSpace(display))
            {
                model.Display = display;
            }
            if (id != null)
            {

                var decodedId = HttpUtility.UrlDecode(id);

                model.ItemId = id;
                var database = Sitecore.Configuration.Factory.GetDatabase("master");
                model.ItemDisplayName = database.GetItem(id).DisplayName;
                model.SearchRankings = _searchStore.GetLatestSearchRankingRecordsForItem(decodedId).OrderByDescending(x => x.Item3).ToList();
                model.TotalClickThroughs = _searchStore.GetTotalClickThroughsForItem(decodedId, DateTime.Now.AddDays(-90).Date).OrderByDescending(x => x.Item2).ToList();
                model.AverageDurations = _searchStore.GetAllTermsEngagementForItem(decodedId, DateTime.Now.AddDays(-90).Date).OrderByDescending(x => x.Item2).ToList();

                if (searchTerm == null && model.TotalClickThroughs != null && model.TotalClickThroughs.Count > 0)
                {
                    model.SearchTerm = model.TotalClickThroughs.OrderByDescending(x => x.Item2).Take(1).FirstOrDefault().Item1;
                }
            }
            else
            {
                model.TotalSearchesForAllTerms = _searchStore.GetTotalSearchesForAllTerms(DateTime.Now.AddDays(-90));
                model.SearchTermsByEngagement = _searchStore.GetSearchTermsByEngagement(DateTime.Now.AddDays(-90));

                if (searchTerm == null && model.TotalSearchesForAllTerms != null && model.TotalSearchesForAllTerms.Count > 0)
                {
                    model.SearchTerm = model.TotalSearchesForAllTerms.OrderByDescending(x => x.Item1).Take(1).FirstOrDefault().Item2;
                }
            }
            return View("~/sitecore/shell/client/Applications/SearchReport/Index.cshtml", model);
        }

        public ActionResult GetSearchRankingRecordsForItemAndTermOverTime(string itemId, string searchTerm)
        {
            var model = new SearchRankingViewModel();
            model.SearchTerm = searchTerm;
            model.SearchRankingResults = _searchStore.GetSearchRankingRecordsForItemAndTermOverTime(itemId, searchTerm, DateTime.Now.AddDays(-90));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetClickThroughsForItemAndTermOverTime(string itemId, string searchTerm)
        {
            var model = new ClickThroughsViewModel();
            model.ItemId = itemId;
            model.SearchTerm = searchTerm;
            model.ClickThroughs = _searchStore.GetClickThroughsForItemAndTermOverTime(itemId, searchTerm, DateTime.Now.AddDays(-90));
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDailySearchesForTermOverTime(string searchTerm)
        {
            var model = new DailySearchesViewModel();
            model.SearchTerm = searchTerm;
            model.DailySearches = _searchStore.GetDailySearchesForTermOverTime(searchTerm, DateTime.Now.AddDays(-90));
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllItemsEngagementForTermOrderByTotal(string searchTerm)
        {
            var model = new RankComparisonViewModel();
            model.SearchTerm = searchTerm;
            model.RankComparisons = new List<RankComparison>();

            var summaries = _searchStore.GetRankingSummaryForTerm(searchTerm);

            var database = Sitecore.Configuration.Factory.GetDatabase("master");
            foreach (var item in summaries)
            {
                var sitecoreItem = database.GetItem(item.Item1);
                if (sitecoreItem == null) continue;

                var rankComparison = new RankComparison();
                rankComparison.ItemId = RemoveUpperAndCurlBrackets(item.Item1);
                rankComparison.ExistingRank = item.Item2;
                rankComparison.DisplayName = sitecoreItem.DisplayName;
                rankComparison.TotalDuration = item.Item4.ToString("0.0");
                rankComparison.Visits = item.Item5;
                rankComparison.TotalDurationRank = item.Item6;
                rankComparison.VisitRank = item.Item7;

                model.RankComparisons.Add(rankComparison);
            }
            model.RankComparisons = model.RankComparisons.OrderBy(x => x.TotalDurationRank).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllItemsEngagementForTermOrderByVisit(string searchTerm)
        {
            var model = new RankComparisonViewModel();
            model.SearchTerm = searchTerm;
            model.RankComparisons = new List<RankComparison>();

            var summaries = _searchStore.GetRankingSummaryForTerm(searchTerm);

            var database = Sitecore.Configuration.Factory.GetDatabase("master");
            foreach (var item in summaries)
            {
                var sitecoreItem = database.GetItem(item.Item1);
                if (sitecoreItem == null) continue;

                var rankComparison = new RankComparison();
                rankComparison.ItemId = RemoveUpperAndCurlBrackets(item.Item1);
                rankComparison.ExistingRank = item.Item2;
                rankComparison.DisplayName = sitecoreItem.DisplayName;
                rankComparison.TotalDuration = item.Item4.ToString("0.0");
                rankComparison.Visits = item.Item5;
                rankComparison.TotalDurationRank = item.Item6;
                rankComparison.VisitRank = item.Item7;

                model.RankComparisons.Add(rankComparison);
            }
            model.RankComparisons = model.RankComparisons.OrderBy(x => x.VisitRank).ToList(); return Json(model, JsonRequestBehavior.AllowGet);
        }
        private static string RemoveUpperAndCurlBrackets(string itemId)
        {
            itemId = itemId.ToUpper();
            if (itemId.Contains("{")) { itemId = itemId.Replace("{",""); }
            if (itemId.Contains("}")) { itemId = itemId.Replace("}", ""); }
            return itemId;
        }
        private static string EnsureUpperAndCurlBrackets(string itemId)
        {
            itemId = itemId.ToUpper();
            if (!itemId.Contains("{")) itemId = "{" + itemId + "}";
            return itemId;
        }
    }
}
