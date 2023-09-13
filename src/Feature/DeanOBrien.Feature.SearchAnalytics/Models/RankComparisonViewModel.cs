using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeanOBrien.Feature.SearchAnalytics.Models
{
    public class RankComparisonViewModel
    {
        public string SearchTerm { get; set; }
        public List<RankComparison> RankComparisons { get; set; }
        public IEnumerable<string> Backgrounds
        {
            get
            {
                var availableBacks = new List<string>() {
                  "rgba(255, 99, 132, 0.8)",
                  "rgba(255, 159, 64, 0.8)",
                  "rgba(255, 205, 86, 0.8)",
                  "rgba(75, 192, 192, 0.8)",
                  "rgba(54, 162, 235, 0.8)",
                  "rgba(153, 102, 255, 0.8)",
                  "rgba(201, 203, 207, 0.8)",
                  "rgba(154, 162, 235, 0.8)",
                  "rgba(153, 102, 155, 0.8)",
                  "rgba(201, 103, 207, 0.8)"
                };
                int countBeingReturned = 0;
                if (RankComparisons != null && RankComparisons.Count > 0)
                {
                    countBeingReturned = RankComparisons.Take(10).Count();
                }
                return availableBacks.Take(countBeingReturned);
            }
        }
        public IEnumerable<string> VisitsLabels
        {
            get
            {
                var result = new List<string>();
                if (RankComparisons != null && RankComparisons.Where(x => x.Visits > 0).ToList().Count > 0)
                {
                    foreach (var item in RankComparisons.Where(x => x.Visits > 0).ToList().OrderByDescending(x => x.Visits).Take(10))
                    {
                        if (item.DisplayName.Length > 50)
                        {
                            result.Add(item.DisplayName.Truncate(50) + "...");
                        }
                        else
                        {
                            result.Add(item.DisplayName);
                        }
                    }
                }
                return result;
            }
        }
        public IEnumerable<string> VisitsValues
        {
            get
            {
                var result = new List<string>();
                if (RankComparisons != null && RankComparisons.Where(x => x.Visits > 0).ToList().Count > 0)
                {
                    foreach (var item in RankComparisons.Where(x => x.Visits > 0).ToList().OrderByDescending(x => x.Visits).Take(10))
                    {
                        result.Add(item.Visits.ToString());
                    }
                }
                return result;
            }
        }
        public IEnumerable<string> DurationLabels
        {
            get
            {
                var result = new List<string>();
                if (RankComparisons != null && RankComparisons.Where(x => Convert.ToDecimal(x.TotalDuration) > 0).ToList().Count > 0)
                {
                    foreach (var item in RankComparisons.Where(x => Convert.ToDecimal(x.TotalDuration) > 0).ToList().OrderByDescending(x => x.TotalDuration).Take(10))
                    {
                        if (item.DisplayName.Length > 50)
                        {
                            result.Add(item.DisplayName.Truncate(50) + "...");
                        }
                        else
                        {
                            result.Add(item.DisplayName);
                        }
                    }
                }
                return result;
            }
        }
        public IEnumerable<string> DurationValues
        {
            get
            {
                var result = new List<string>();
                if (RankComparisons != null && RankComparisons.Count > 0)
                {
                    foreach (var item in RankComparisons.OrderByDescending(x => x.TotalDuration).Take(10))
                    {
                        result.Add(item.TotalDuration.ToString());
                    }
                }
                return result;
            }
        }
    }
    public class RankComparison
    {
        public string ItemId { get; set; }
        public string DisplayName { get; set; }
        public int ExistingRank { get; set; }
        public int TotalDurationRank { get; set; }
        public int VisitRank { get; set; }

        public int TotalDurationGain
        {
            get
            {
                return ExistingRank - TotalDurationRank;
            }
        }
        public int VisitGain
        {
            get
            {
                return ExistingRank - VisitRank;
            }
        }

        public string TotalDurationMovementImage
        {
            get
            {
                if (TotalDurationGain > 0)
                {
                    return "up.jpg";
                }
                else if (TotalDurationGain < 0)
                {
                    return "down.jpg";
                }
                else
                {
                    return "blank.gif";
                }
            }
        }
        public string VisitMovementImage
        {
            get
            {
                if (VisitGain > 0)
                {
                    return "up.jpg";
                }
                else if (VisitGain < 0)
                {
                    return "down.jpg";
                }
                else
                {
                    return "blank.gif";
                }
            }
        }
        public Decimal AverageDuration
        {
            get
            {
                if (Visits == 0) { return 0; }
                return Convert.ToDecimal(TotalDuration) / Visits;
            }
        }
        public string TotalDuration { get; set; }
        public int Visits { get; set; }

    }
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
