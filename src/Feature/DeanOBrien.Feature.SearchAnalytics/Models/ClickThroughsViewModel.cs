using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeanOBrien.Feature.SearchAnalytics.Models
{
    public class ClickThroughsViewModel
    {
        public string ItemId { get; set; }
        public string SearchTerm { get; set; }
        public List<Tuple<int, DateTime>> ClickThroughs { get; set; }
        public IEnumerable<string> Labels
        {
            get
            {
                var result = new List<string>();
                var firstDay = ClickThroughs.OrderBy(x => x.Item2).Take(1).FirstOrDefault().Item2;
                var lastDay = ClickThroughs.OrderByDescending(x => x.Item2).Take(1).FirstOrDefault().Item2;
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
        public IEnumerable<int> Values
        {
            get
            {
                var result = new List<int>();
                var firstDay = ClickThroughs.OrderBy(x => x.Item2).Take(1).FirstOrDefault().Item2;
                var lastDay = ClickThroughs.OrderByDescending(x => x.Item2).Take(1).FirstOrDefault().Item2;
                var currentDay = firstDay;
                foreach (var item in ClickThroughs.OrderBy(x => x.Item2))
                {
                    if (item.Item2 == currentDay)
                    {

                    }
                    else
                    {
                        var daysDifference = (item.Item2 - currentDay).Days;
                        for (int i = 0; i < daysDifference; i++)
                        {
                            result.Add(0);
                        }
                    }
                    result.Add(item.Item1);
                    currentDay = item.Item2.AddDays(1);
                }
                return result;
            }
        }
    }
}
