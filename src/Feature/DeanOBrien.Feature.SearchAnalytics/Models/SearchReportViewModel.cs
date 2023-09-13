using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeanOBrien.Feature.SearchAnalytics.Models
{
    public class SearchReportViewModel
    {
        public string ItemId { get; set; }
        public string ItemDisplayName { get; set; }
        public string SearchTerm { get; set; }
        public List<Tuple<int, string>> TotalSearchesForAllTerms { get; set; }

        public List<Tuple<string, int, DateTime>> SearchRankings { get; set; }
        public List<Tuple<string, int>> TotalClickThroughs { get; set; }
        public List<Tuple<decimal, int, string, decimal>> AverageDurations { get; set; }
        public List<Tuple<string, int, int>> SearchTermsByEngagement { get; internal set; }
        public string Display { get; internal set; }
    }
}
