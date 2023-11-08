using DeanOBrien.Feature.SearchAnalytics.Utilities;
using DeanOBrien.Foundation.DataAccess.SearchAnalytics;
using Sitecore;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeanOBrien.Feature.SearchAnalytics.Controllers
{
    public class ExampleSearchController : Controller
    {
        private ISearchService _SearchService;
        private string _term;
        private ITrackSearch _trackSearch;
        private readonly ISearchStore _searchStore;

        public ExampleSearchController(ISearchService SearchService, ITrackSearch trackSearch, ISearchStore searchStore)
        {
            _SearchService = SearchService;
            _trackSearch = trackSearch;
            _searchStore = searchStore;
        }
        public ActionResult SiteSearchResults()
        {
            var model = new RenderingModel();

            //
            // This is not intended to be a used as a component. 
            // Rather an example of how you might integrate with TrackSearch() given a result from a similar SearchService
            //

            _term = Request["q"];
            var searchResults = _SearchService.SiteSearch(_term);
            var top20Items = searchResults.Take(20).Select(x => x.ItemId.ToString());

            if (!string.IsNullOrWhiteSpace(_term))
            {
                _trackSearch.Track(Context.Item, _term, string.Join("|", top20Items));
            }

            //
            // Add the search results to a custom view model and pass to view
            //

            return View(model);
        }
        [Route("api/autocompletesearch/sitesearch/{contactId}")]
        public IEnumerable<string> GetEnhancedResults(string queryString, string contactId = null)
        {
            //
            // This is not intended to be a used as a component. 
            // Rather an example of how you might take a mixture of individual (searches are stored together with contactId) and general results
            //

            var result = new List<string>();
            if (contactId != null)
            {
                var individualSearchStoreResults = _searchStore.GetTotalSingleSearchesForSimilarTermsForContact(queryString, contactId, DateTime.Now.AddDays(-90)).OrderByDescending(x => x.Item1).Take(2).ToList();
                if (individualSearchStoreResults != null) result.AddRange(individualSearchStoreResults.Select(y => y.Item2));
            }
            var requiredFromGeneral = 5 - (result.Count());

            var generalSearchStoreResults = _searchStore.GetTotalSingleSearchesForSimilarTerms(queryString, DateTime.Now.AddDays(-90)).OrderByDescending(x => x.Item1).Where(x => !result.Contains(x.Item2)).Take(requiredFromGeneral).Select(y => y.Item2).ToList();
            if (generalSearchStoreResults != null) result.AddRange(generalSearchStoreResults);

            return result;
        }
    }
    public interface ISearchService
    {
        IEnumerable<SearchResultItem> SiteSearch(string term);
        IEnumerable<SearchResultItem> AutoCompleteSiteSearch(string term);
    }
}
