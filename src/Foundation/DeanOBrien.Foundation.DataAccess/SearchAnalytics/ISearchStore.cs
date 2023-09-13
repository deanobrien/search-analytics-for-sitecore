using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeanOBrien.Foundation.DataAccess.SearchAnalytics
{
    public interface ISearchStore
    {
        bool PopulateTempAdjustedRank();

        bool AddSingleSearchRecord(string contactId, string searchTerm, DateTime date);
        bool AddSearchClickThroughRecord(double duration, string itemId, string searchTerm, DateTime date);
        bool AddSearchRankingRecord(string itemId, string searchTerm, int rank, DateTime date);
        List<Tuple<string, int, DateTime>> GetFirstSearchRankingRecord(string searchTerm, DateTime date);

        List<Tuple<int, string>> GetTotalSingleSearchesForSimilarTerms(string searchTerm, DateTime date);
        List<Tuple<int, string>> GetTotalSingleSearchesForSimilarTermsForContact(string searchTerm, string contactId, DateTime date);

        List<Tuple<string, int, DateTime>> GetLatestSearchRankingRecordsForItem(string itemId);
        List<Tuple<string, int>> GetTotalClickThroughsForItem(string itemId, DateTime date);
        List<Tuple<decimal, int, string, decimal>> GetAllTermsEngagementForItem(string itemId, DateTime date);
        List<Tuple<string, int, DateTime>> GetSearchRankingRecordsForItemAndTermOverTime(string itemId, string searchTerm, DateTime date);
        List<Tuple<int, DateTime>> GetClickThroughsForItemAndTermOverTime(string itemId, string searchTerm, DateTime date);

        List<Tuple<string, int, int>> GetSearchTermsByEngagement(DateTime dateTime);
        List<Tuple<string, int, decimal, decimal, int, int, int>> GetRankingSummaryForTerm(string searchTerm);
        List<Tuple<int, DateTime>> GetDailySearchesForTermOverTime(string searchTerm, DateTime dateTime);
        List<Tuple<int, string>> GetTotalSearchesForAllTerms(DateTime dateTime);
    }
}
