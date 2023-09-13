using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace DeanOBrien.Feature.SearchAnalytics.Pipelines
{
    public class RegisterCustomRoute
    {
        public virtual void Process(PipelineArgs args)
        {
            Register();
        }

        public static void Register()
        {
            RouteTable.Routes.MapRoute("SearchReport", "sitecore/shell/sitecore/client/applications/searchreport/", new { controller = "SearchReport", action = "SearchReport" });
            RouteTable.Routes.MapRoute("SearchReportById", "sitecore/shell/sitecore/client/applications/searchreport/{id}", new { controller = "SearchReport", action = "SearchReport" });
            RouteTable.Routes.MapRoute("GetSearchRankingRecordsForItemAndTermOverTime", "sitecore/shell/sitecore/client/applications/searchreport/GetSearchRankingRecordsForItemAndTermOverTime/{itemId}/{searchterm}", new { controller = "SearchReport", action = "GetSearchRankingRecordsForItemAndTermOverTime" });
            RouteTable.Routes.MapRoute("GetClickThroughsForItemAndTermOverTime", "sitecore/shell/sitecore/client/applications/searchreport/GetClickThroughsForItemAndTermOverTime/{itemId}/{searchterm}", new { controller = "SearchReport", action = "GetClickThroughsForItemAndTermOverTime" });
            RouteTable.Routes.MapRoute("GetAllItemsEngagementForTerm", "sitecore/shell/sitecore/client/applications/searchreport/GetAllItemsEngagementForTerm/{searchterm}", new { controller = "SearchReport", action = "GetAllItemsEngagementForTerm" });
            RouteTable.Routes.MapRoute("GetAllItemsEngagementForTermOrderByTotal", "sitecore/shell/sitecore/client/applications/searchreport/GetAllItemsEngagementForTermOrderByTotal/{searchterm}", new { controller = "SearchReport", action = "GetAllItemsEngagementForTermOrderByTotal" });
            RouteTable.Routes.MapRoute("GetAllItemsEngagementForTermOrderByVisit", "sitecore/shell/sitecore/client/applications/searchreport/GetAllItemsEngagementForTermOrderByVisit/{searchterm}", new { controller = "SearchReport", action = "GetAllItemsEngagementForTermOrderByVisit" });
            RouteTable.Routes.MapRoute("GetSearchTermsByTotalDurationVariation", "sitecore/shell/sitecore/client/applications/searchreport/GetSearchTermsByTotalDurationVariation/{searchterm}", new { controller = "SearchReport", action = "GetSearchTermsByTotalDurationVariation" });
            RouteTable.Routes.MapRoute("GetDailySearchesForTermOverTime", "sitecore/shell/sitecore/client/applications/searchreport/GetDailySearchesForTermOverTime/{searchterm}", new { controller = "SearchReport", action = "GetDailySearchesForTermOverTime" });
        }
    }
}
