using DeanOBrien.Feature.SearchAnalytics.Utilities;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Pipelines.ProcessItem;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DeanOBrien.Feature.SearchAnalytics.Pipeline
{
    public class RegisterCustomPageEvents : ProcessItemProcessor
    {
        private ITrackSearch _trackSearch;

        public RegisterCustomPageEvents(ITrackSearch trackSearch)
        {
            _trackSearch = trackSearch;
        }
        public override void Process(ProcessItemArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));

            if (!Tracker.Enabled || Tracker.Current == null || !Tracker.Current.IsActive)
            {
                return;
            }
            FireCustomEvent(args);
        }

        private void FireCustomEvent(ProcessItemArgs args)
        {
            if (args.Item == null)
            {
                return;
            }
            string searchPageUrlPartial = Sitecore.Configuration.Settings.GetSetting("SearchPageUrlPartial");
            string searchPageParameterName = Sitecore.Configuration.Settings.GetSetting("SearchPageParameterName");


            var previousPage = HttpContext.Current?.Request?.UrlReferrer?.AbsoluteUri;
            if (!string.IsNullOrWhiteSpace(previousPage) && previousPage.Contains(searchPageUrlPartial))
            {
                Uri previousUri = new Uri(previousPage);
                string query = HttpUtility.ParseQueryString(previousUri.Query).Get(searchPageParameterName);
                if (!string.IsNullOrWhiteSpace(query))
                {
                    _trackSearch.TrackSiteSearchClick(Context.Item, query);
                }
            }
        }
    }
}
