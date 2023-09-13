using Sitecore.Data.Items;

namespace DeanOBrien.Feature.SearchAnalytics.Utilities
{
    public interface ITrackSearch
    {
        void Track(Item pageEventItem, string query, string top20Results);
        void Track(Item pageEventItem, string query);
        void TrackSiteSearchClick(Item pageEventItem, string query);
    }
}
