using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeanOBrien.Feature.SearchAnalytics.Models
{
    public class SearchRankingViewModel
    {
        public string SearchTerm { get; set; }
        public List<Tuple<string, int, DateTime>> SearchRankingResults { get; set; }
        public IEnumerable<string> Labels
        {
            get
            {
                var result = new List<string>();
                var firstDay = SearchRankingResults.OrderBy(x => x.Item3).Take(1).FirstOrDefault().Item3;
                var lastDay = SearchRankingResults.OrderByDescending(x => x.Item3).Take(1).FirstOrDefault().Item3;
                var currentDay = firstDay;

                result.Add(firstDay.ToShortDateString());

                while (currentDay < lastDay)
                {
                    currentDay = currentDay.AddDays(1);
                    result.Add(currentDay.ToShortDateString());
                }
                return result;
            }
        }
        public IEnumerable<string> Values
        {
            get
            {
                var result = new List<string>();
                var firstDay = SearchRankingResults.OrderBy(x => x.Item3).Take(1).FirstOrDefault().Item3;
                var latestResult = SearchRankingResults.OrderBy(x => x.Item3).Take(1).FirstOrDefault().Item2.ToString();
                var lastDay = SearchRankingResults.OrderByDescending(x => x.Item3).Take(1).FirstOrDefault().Item3;
                var currentDay = firstDay;
                foreach (var item in SearchRankingResults.OrderBy(x => x.Item3))
                {
                    if (item.Item3 == currentDay)
                    {

                    }
                    else
                    {
                        var daysDifference = (item.Item3 - currentDay).Days;
                        for (int i = 0; i < daysDifference; i++)
                        {
                            result.Add(latestResult);
                        }
                    }
                    result.Add(item.Item2.ToString());
                    latestResult = item.Item2.ToString();
                    currentDay = item.Item3.AddDays(1);
                }
                return result;
            }
        }
    }
}
