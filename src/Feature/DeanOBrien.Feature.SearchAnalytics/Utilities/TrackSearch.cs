using Sitecore.Analytics;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using DeanOBrien.Foundation.DataAccess.SearchAnalytics;

namespace DeanOBrien.Feature.SearchAnalytics.Utilities
{
    public class TrackSearch : ITrackSearch
    {
        private ISearchStore _searchStore;
        private bool _isInXmMode;
        private bool _isxDBEnabled;
        private bool _isxDBTrackingEnabled;
        public const string SearchEvent = "0C179613-2073-41AB-992E-027D03D523BF";
        public const string SearchRankingEvent = "79944B76-002E-4423-BF49-96F0455F6814";
        public const string SearchClickThroughEvent = "D6C11602-065A-43ED-BB15-3A6C6B6B0278";

        public TrackSearch(ISearchStore searchStore)
        {
            _isInXmMode = false;
            if (!string.IsNullOrWhiteSpace(Sitecore.Configuration.Settings.GetSetting("XMModeForSearchAnalytics"))) _isInXmMode = Sitecore.Configuration.Settings.GetSetting("XMModeForSearchAnalytics") == "true";
            _isxDBEnabled = false;
            if (!string.IsNullOrWhiteSpace(Sitecore.Configuration.Settings.GetSetting("Xdb.Enabled"))) _isxDBEnabled = Sitecore.Configuration.Settings.GetSetting("Xdb.Enabled") == "true";
            _isxDBTrackingEnabled = false;
            if (!string.IsNullOrWhiteSpace(Sitecore.Configuration.Settings.GetSetting("Xdb.Tracking.Enabled"))) _isxDBTrackingEnabled = Sitecore.Configuration.Settings.GetSetting("Xdb.Tracking.Enabled") == "true";

            _searchStore = searchStore;
        }

        public virtual void Track(Item pageEventItem, string query, string top20Results)
        {

            if (_isInXmMode || !_isxDBEnabled || !_isxDBTrackingEnabled)
            {
                SendRankingsDirectToSql(query, top20Results, pageEventItem);
            }
            else
            {
                string newQuery = query;
                if (top20Results != null && !string.IsNullOrWhiteSpace(query))
                {
                    newQuery += "&top20Results=" + top20Results;
                }
                RegisterEvent(pageEventItem, newQuery, "Search Ranking", SearchRankingEvent);
            }

            Track(pageEventItem, query);
        }

        public virtual void Track(Item pageEventItem, string query)
        {
            if (_isInXmMode || !_isxDBEnabled || !_isxDBTrackingEnabled)
            {
                SendSearchDirectToSql(query);
            }
            else
            {
                RegisterEvent(pageEventItem, query, "Search", SearchEvent);
            }
        }

        public virtual void TrackSiteSearchClick(Item pageEventItem, string query)
        {
            if (_isInXmMode || !_isxDBEnabled || !_isxDBTrackingEnabled)
            {
                SendSiteSearchClickDirectToSql(query, pageEventItem);
            }
            else
            {
                RegisterEvent(pageEventItem, query, "Search Click Through", SearchClickThroughEvent);
            }
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
        private void SendRankingsDirectToSql(string searchTerm, string top20ResultsCombined, Item pageEventItem)
        {
            try
            {
                var top20Results = top20ResultsCombined.Split('|');

                var firstRecord = _searchStore.GetFirstSearchRankingRecord(searchTerm, DateTime.Now.Date);
                if (firstRecord.Count == 0)
                {
                    var i = 1;
                    foreach (var item in top20Results)
                    {
                        _searchStore.AddSearchRankingRecord(item, searchTerm, i, DateTime.Now.Date);
                        i++;
                    }
                }
                else
                {
                    // double check if this is now used and remove if not (searches recorded in searchEvent now)
                    _searchStore.AddSearchRankingRecord("register additional search", searchTerm, 0, DateTime.Now.Date);
                }
            }
            catch (Exception ex)
            {
                Log.Info($"TrackSearch: Failed addinging rankings for {searchTerm}/n {ex.Message}", this);
            }
        }
        private void SendSearchDirectToSql(string query)
        {
            var contactId = Sitecore.Analytics.Tracker.Current?.Contact?.ContactId;
            if (_searchStore != null)
            {
                if (contactId != null) { _searchStore.AddSingleSearchRecord(contactId.ToString(), query, DateTime.Now); }
                else { _searchStore.AddSingleSearchRecord("not available", query, DateTime.Now); }
            }
        }
        private void SendSiteSearchClickDirectToSql(string query, Item pageEventItem)
        {
            _searchStore.AddSearchClickThroughRecord(0, pageEventItem.ID.ToString(), query, DateTime.Now.Date);
        }
    }
}
