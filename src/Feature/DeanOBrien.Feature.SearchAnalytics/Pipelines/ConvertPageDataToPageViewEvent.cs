using Sitecore.Analytics.Model;
using Sitecore.Analytics.XConnect.DataAccess.Pipelines.ConvertToXConnectEventPipeline;
using Sitecore.Framework.Conditions;
using Sitecore.XConnect.Collection.Model;
using System;
using SitecoreXConnect = Sitecore.XConnect;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using DeanOBrien.Foundation.DataAccess.SearchAnalytics;
using Sitecore.Diagnostics;
using System.Web;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DeanOBrien.Feature.SearchAnalytics.Pipelines
{
    public class ConvertPageDataToPageViewEvent : ConvertToXConnectEventProcessorBase<PageData>
    {
        public const string SearchEvent = "0C179613-2073-41AB-992E-027D03D523BF";
        public const string SearchRankingEvent = "79944B76-002E-4423-BF49-96F0455F6814";
        public const string SearchClickThroughEvent = "D6C11602-065A-43ED-BB15-3A6C6B6B0278";
        private ISearchStore _searchStore;

        public ConvertPageDataToPageViewEvent(ISearchStore searchStore)
        {
            _searchStore = searchStore;
        }
        protected override SitecoreXConnect.Event ConvertToEvent(PageData pageData)
        {
            Condition.Requires<DateTime>(pageData.DateTime, nameof(pageData)).IsUtc("{0}.DateTime should be of kind UTC");
            PageViewEvent pageViewEvent = CreatePageViewEvent(pageData);
            FillPageViewEvent(pageViewEvent, pageData);
            return (SitecoreXConnect.Event)pageViewEvent;
        }

        private PageViewEvent CreatePageViewEvent(PageData pageData)
        {
            var isProcessed = MemoryCache.Default.Get(pageData.DateTime.Ticks.ToString() + pageData.Item.Id.ToString()) == null ? false : true;

            PageViewEvent pageViewEvent = new PageViewEvent(pageData.DateTime, pageData.Item.Id, pageData.Item.Version, pageData.Item.Language);
            pageViewEvent.Duration = TimeSpan.FromMilliseconds((double)pageData.Duration);

            pageViewEvent.Url = pageData.Url.ToString();


            // These are registered while generating search listings
            var searchEvent = pageData.PageEvents.FindLast(ev => ev.PageEventDefinitionId.ToString().ToUpper() == SearchEvent);
            if (searchEvent != null && !string.IsNullOrWhiteSpace(searchEvent.Text) && !isProcessed)
            {
                var contactId = Sitecore.Analytics.Tracker.Current?.Contact?.ContactId;
                if (_searchStore != null)
                {
                    if (contactId != null) { _searchStore.AddSingleSearchRecord(contactId.ToString(), searchEvent.Text, DateTime.Now); }
                    else { _searchStore.AddSingleSearchRecord("not available", searchEvent.Text, DateTime.Now); }
                }
            }
            // These are registered while generating search listings
            var searchRankingEvent = pageData.PageEvents.FindLast(ev => ev.PageEventDefinitionId.ToString().ToUpper() == SearchRankingEvent);
            if (searchRankingEvent != null && !string.IsNullOrWhiteSpace(searchRankingEvent.Text) && searchRankingEvent.Text.Contains("&top20Results"))
            {
                int index = searchRankingEvent.Text.LastIndexOf("&");
                if (!isProcessed)
                {
                    try
                    {
                        var top20Results = searchRankingEvent.Text.Substring(index + 14, searchRankingEvent.Text.Length - index - 14).Split('|');

                        if (index >= 0)
                            searchRankingEvent.Text = searchRankingEvent.Text.Substring(0, index);

                        var firstRecord = _searchStore.GetFirstSearchRankingRecord(searchRankingEvent.Text, DateTime.Now.Date);
                        if (firstRecord.Count == 0)
                        {
                            var i = 1;
                            foreach (var item in top20Results)
                            {
                                _searchStore.AddSearchRankingRecord(item, searchRankingEvent.Text, i, DateTime.Now.Date);
                                i++;
                            }
                        }
                        else
                        {
                            // double check if this is now used and remove if not (searches recorded in searchEvent now)
                            _searchStore.AddSearchRankingRecord("register additional search", searchRankingEvent.Text, 0, DateTime.Now.Date);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"ConvertPageDataToPageViewEvent: Failed addinging rankings for {searchRankingEvent.Text}/n {ex.Message}", this);
                    }

                }
            }

            // These are only registered on pages directly after search listings
            var searchClickThroughEvent = pageData.PageEvents.FindLast(ev => ev.PageEventDefinitionId.ToString().ToUpper() == SearchClickThroughEvent);
            if (searchClickThroughEvent != null && !string.IsNullOrWhiteSpace(searchClickThroughEvent.Text))
            {
                pageViewEvent.Url = pageViewEvent.Url + "?q=" + HttpUtility.UrlEncode(searchClickThroughEvent.Text);

                if (!isProcessed)
                {
                    _searchStore.AddSearchClickThroughRecord(pageViewEvent.Duration.Seconds, pageData.Item.Id.ToString(), searchClickThroughEvent.Text, DateTime.Now.Date);
                }
            }

            MemoryCache.Default.Set(pageData.DateTime.Ticks.ToString() + pageData.Item.Id.ToString(), "1", DateTimeOffset.Now.AddMinutes(10080));
            return pageViewEvent;
        }

        private void FillPageViewEvent(PageViewEvent pageViewEvent, PageData pageData)
        {
            if (pageData.SitecoreDevice == null)
                return;

            pageViewEvent.SitecoreRenderingDevice = new SitecoreXConnect.Collection.Model.SitecoreDeviceData(pageData.SitecoreDevice.Id, pageData.SitecoreDevice.Name);
        }
    }
}
