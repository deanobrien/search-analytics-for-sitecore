using Sitecore.Analytics;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeanOBrien.Feature.SearchAnalytics.Utilities
{
    public class TrackSearch : ITrackSearch
    {
        public const string SearchEvent = "0C179613-2073-41AB-992E-027D03D523BF";
        public const string SearchRankingEvent = "79944B76-002E-4423-BF49-96F0455F6814";
        public const string SearchClickThroughEvent = "D6C11602-065A-43ED-BB15-3A6C6B6B0278";

        public virtual void Track(Item pageEventItem, string query, string top20Results)
        {
            string newQuery = query;
            if (top20Results != null && !string.IsNullOrWhiteSpace(query))
            {
                newQuery += "&top20Results=" + top20Results;
            }
            RegisterEvent(pageEventItem, newQuery, "Search Ranking", SearchRankingEvent);
            Track(pageEventItem, query);
        }
        public virtual void Track(Item pageEventItem, string query)
        {
            RegisterEvent(pageEventItem, query, "Search", SearchEvent);
        }
        public virtual void TrackSiteSearchClick(Item pageEventItem, string query)
        {
            RegisterEvent(pageEventItem, query, "Search Click Through", SearchClickThroughEvent);
        }

        private static void RegisterEvent(Item pageEventItem, string text, string eventName, string eventDefintionID)
        {
            // These events are added to the contacts tracker so that they can be processed when the session ends

            if (Tracker.IsActive)
            {
                if (Tracker.Current == null) Tracker.StartTracking();
                Assert.ArgumentNotNull(pageEventItem, nameof(pageEventItem));
                Assert.IsNotNull(pageEventItem, $"Cannot find page event: {pageEventItem}");
                if (Tracker.Current != null && Tracker.Current.IsActive)
                {
                    var pageEventData = new Sitecore.Analytics.Data.PageEventData(eventName, new Guid(eventDefintionID))
                    {
                        ItemId = pageEventItem.ID.ToGuid(),
                        Data = text,
                        DataKey = text,
                        Text = text
                    };
                    var interaction = Tracker.Current.Session.Interaction;
                    if (interaction != null)
                    {
                        interaction.CurrentPage.Register(pageEventData);
                    }
                }
            }
        }
    }
}
