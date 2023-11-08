using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Sitecore.Diagnostics;

namespace DeanOBrien.Foundation.DataAccess.SearchAnalytics
{
    public class SqlSearchStore : ISearchStore
    {
        private string _connectionString;

        public SqlSearchStore()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SearchAnalytics"].ConnectionString;
        }
        /*
        // Used to tabulate latest ranking data
        // Table: Ranking
        */
        public bool PopulateTempAdjustedRank()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.PopulateTempAdjustedRank", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info($"ERROR: SqlSearchStore error:{ex.Message}", this);
                return false;
            }
        }

        
        #region Used for Data Storage
        /*
        // Used to register single search
        // Table: SingleSearches
        */
        public bool AddSingleSearchRecord(string contactId, string searchTerm, DateTime date)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.InsertSingleSearchRecord", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddContactIdParameter(contactId, cmd);
                        AddSearchTermParameter(searchTerm, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info($"ERROR: SqlSearchStore error:{ex.Message}", this);
                return false;
            }
        }
        /*
        // Used to register a click through from search page and visit duration
        // Table: ClickThroughs
        */
        public bool AddSearchClickThroughRecord(double duration, string itemId, string searchTerm, DateTime date)
        {
            itemId = EnsureUpperAndCurlBrackets(itemId);
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.InsertClickThroughRecord", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddDurationParameter(duration, cmd);
                        AddItemIdParameter(itemId, cmd);
                        AddSearchTermParameter(searchTerm, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info($"ERROR: SqlSearchStore error:{ex.Message}", this);
                return false;
            }
        }
        /*
        // Used to register search rankings
        // Table: SearchRankings
        */
        public bool AddSearchRankingRecord(string itemId, string searchTerm, int rank, DateTime date)
        {
            itemId = EnsureUpperAndCurlBrackets(itemId);
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.InsertSearchRankingRecord", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddItemIdParameter(itemId, cmd);
                        AddSearchTermParameter(searchTerm, cmd);
                        AddRankParameter(rank, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info($"ERROR: SqlSearchStore error:{ex.Message}", this);
                return false;
            }
        }

        private static string EnsureUpperAndCurlBrackets(string itemId)
        {
            itemId = itemId.ToUpper();
            if (!itemId.Contains("{")) itemId = "{" + itemId + "}";
            return itemId;
        }

        /*
        // Used to check if search ranking registered on a given day
        // Table: SearchRankings
        */
        public List<Tuple<string, int, DateTime>> GetFirstSearchRankingRecord(string searchTerm, DateTime date)
        {
            var results = new List<Tuple<string, int, DateTime>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetFirstSearchRankingRecords", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddSearchTermParameter(searchTerm, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<string, int, DateTime>(reader.GetString(0), reader.GetInt32(1), reader.GetDateTime(2)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }

        #endregion Used for Data Storage

        #region Used for Auto Complete
        /*
        // Used to get totals for searches LIKE term. 
        // i.e. BUS might return | BUSiness 10 | risky BUSiness 20 |
        // Table: SingleSearches
        */
        public List<Tuple<int, string>> GetTotalSingleSearchesForSimilarTerms(string searchTerm, DateTime date)
        {
            var results = new List<Tuple<int, string>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetTotalSingleSearchesForSimilarTerms", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddSearchTermParameter(searchTerm, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<int, string>(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        /*
        // Used to get totals for searches LIKE term for a given contactId. 
        // i.e. BUS might return | BUSiness 10 | risky BUSiness 20 |
        // Table: SingleSearches
        */
        public List<Tuple<int, string>> GetTotalSingleSearchesForSimilarTermsForContact(string searchTerm, string contactId, DateTime date)
        {
            var results = new List<Tuple<int, string>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetTotalSingleSearchesForSimilarTermsForContact", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddSearchTermParameter(searchTerm, cmd);
                        AddContactIdParameter(contactId, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<int, string>(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        #endregion Used for Auto Complete

        #region Used for Single Item Reporting
        /*
        // Used to get latest ranking for search terms for ITEM. 
        // i.e. | TERM, RANK, DATE | apples 1 01/01/23 | oranges 5 05/01/23 | pears 8 21/01/23 |
        // Table: SearchRankings
        */
        public List<Tuple<string, int, DateTime>> GetLatestSearchRankingRecordsForItem(string itemId)
        {
            var results = new List<Tuple<string, int, DateTime>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetLatestSearchRankingRecordsForItem", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddItemIdParameter(itemId, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<string, int, DateTime>(reader.GetString(0), reader.GetInt32(1), reader.GetDateTime(2)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        /*
        // Used to get total click throughs for search terms for ITEM since given date
        // i.e. | TERM, TOTAL| apples 100 | oranges 63 | pears 7 |
        // Table: ClickThroughs
        */
        public List<Tuple<string, int>> GetTotalClickThroughsForItem(string itemId, DateTime date)
        {
            var results = new List<Tuple<string, int>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetTotalClickThroughsForItem", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddItemIdParameter(itemId, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<string, int>(reader.GetString(0), reader.GetInt32(1)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        /*
        // Used to get engagement data for ITEM since given date
        // i.e. | AverageDuration, Visits, Term, TotalDuration | 4.5, 17, apples, 100.1 | 6.2, 121, oranges, 630.4 | 
        // Table: ClickThroughs
        */
        public List<Tuple<decimal, int, string, decimal>> GetAllTermsEngagementForItem(string itemId, DateTime date)
        {
            var results = new List<Tuple<decimal, int, string, decimal>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetAllTermsEngagementForItem", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddItemIdParameter(itemId, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<decimal, int, string, decimal>(reader.GetDecimal(0), reader.GetInt32(1), reader.GetString(2), reader.GetDecimal(3)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        /*
        // Used to get CHART DATA showing how ITEM ranks for TERM over time.
        // i.e. | ItemId, Rank, Date | xxx, 17, 01/01/23 | xxx, 14, 02/01/23 | xxx, 9, 03/01/23 | 
        // Table: SearchRankings
        */
        public List<Tuple<string, int, DateTime>> GetSearchRankingRecordsForItemAndTermOverTime(string itemId, string searchTerm, DateTime date)
        {
            var results = new List<Tuple<string, int, DateTime>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetSearchRankingRecordsForItemAndTermOverTime", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddItemIdParameter(itemId, cmd);
                        AddSearchTermParameter(searchTerm, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<string, int, DateTime>(reader.GetString(0), reader.GetInt32(1), reader.GetDateTime(2)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        /*
        // Used to get CHART DATA showing number of click throughs TERM and ITEM over time.
        // i.e. for term=apple and item=xxx | TotalSearches, Date | 170, 01/01/23 | 4, 02/01/23 | 90, 03/01/23 | 
        // Table: ClickThroughs
        */
        public List<Tuple<int, DateTime>> GetClickThroughsForItemAndTermOverTime(string itemId, string searchTerm, DateTime date)
        {
            var results = new List<Tuple<int, DateTime>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetClickThroughsForItemAndTermOverTime", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddItemIdParameter(itemId, cmd);
                        AddSearchTermParameter(searchTerm, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<int, DateTime>(reader.GetInt32(0), reader.GetDateTime(1)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        #endregion Used for Single Item Reporting

        #region Used for Global Reporting
        /*
         // Used to get all TERMS then sorted by Variation from normal Rankings (if total duration was used to rank)
         // i.e. | Term, TotalDurationVariation, VisitsVariation | apples, 42, 37 | oranges, 39, 34 | pears 37, 21 | 
         // Using the information above, the marketer can further interogate why people are staying for longer on pages not at top of rankings
         // Table: TempAdjustedRank
         */
        public List<Tuple<string, int, int>> GetSearchTermsByEngagement(DateTime date)
        {
            var results = new List<Tuple<string, int, int>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetSearchTermsByTotalDurationVariation", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<string, int, int>(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        /*
         // Used to get ranking and engagement data for all relevant items for given TERM
         // i.e. | ItemId, Rank, AverageDuration, TotalDuration, Visits, TotalDurationRank, VisitsRank | xxx, 1, 45.4, 306.3, 42, 2, 4 | yyy, 2, 47.4, 346.3, 43, 3, 6 | 
         // Table: TempAdjustedRank
         */
        public List<Tuple<string, int, decimal, decimal, int, int, int>> GetRankingSummaryForTerm(string searchTerm)
        {
            var results = new List<Tuple<string, int, decimal, decimal, int, int, int>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetRankingSummaryForTerm", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddSearchTermParameter(searchTerm, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<string, int, decimal, decimal, int, int, int>(reader.SafeGetString(0), reader.SafeGetInt32(1), reader.SafeGetDecimal(2), reader.SafeGetDecimal(3), reader.SafeGetInt32(4), reader.SafeGetInt32(5), reader.SafeGetInt32(6)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        /*
        // Used to get CHART DATA showing total searches each day for TERM since given date
        // i.e. for term=apple| TotalSearches, Date | 64, 01/01/23 | 40, 02/01/23 | 9, 03/01/23 | 
        // TODO: THIS USES RANKING DATA TO COUNT SEARCHES RATHER THAN SINGLE SEARCHES
        // Table: SearchRankings
        */
        public List<Tuple<int, DateTime>> GetDailySearchesForTermOverTime(string searchTerm, DateTime date)
        {
            var results = new List<Tuple<int, DateTime>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetDailySearchesForTermOverTime", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddSearchTermParameter(searchTerm, cmd);
                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<int, DateTime>(reader.GetInt32(0), reader.GetDateTime(1)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        /*
         // Used to get CHART DATA showing total searches for ALL TERMS since given date
         // i.e. | TotalSearches, Term | 6400, apples | 4000, oranges | 900, pears | 
         // TODO: THIS USES RANKING DATA TO COUNT SEARCHES RATHER THAN SINGLE SEARCHES
         // Table: SearchRankings
         */
        public List<Tuple<int, string>> GetTotalSearchesForAllTerms(DateTime date)
        {
            var results = new List<Tuple<int, string>>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("dbo.GetTotalSearchesForAllTerms", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        AddDateParameter(date, cmd);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Tuple<int, string>(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        #endregion Used for Global Reporting

        #region Used to Add Parameters
        private static void AddCountParameter(int count, SqlCommand cmd)
        {
            var parameterCount = new SqlParameter("@Count", SqlDbType.Int);
            parameterCount.Value = count;
            cmd.Parameters.Add(parameterCount);
        }

        private static void AddSearchTermParameter(string searchTerm, SqlCommand cmd)
        {
            var parameterSearchTerm = new SqlParameter("@SearchTerm", SqlDbType.VarChar, 50);
            parameterSearchTerm.Value = searchTerm;
            cmd.Parameters.Add(parameterSearchTerm);
        }
        private static void AddDurationParameter(double duration, SqlCommand cmd)
        {
            var parameterDuration = new SqlParameter("@Duration", SqlDbType.Decimal);
            parameterDuration.Value = duration;
            cmd.Parameters.Add(parameterDuration);
        }
        private static void AddRankParameter(int rank, SqlCommand cmd)
        {
            var parameterRank = new SqlParameter("@Rank", SqlDbType.Int);
            parameterRank.Value = rank;
            cmd.Parameters.Add(parameterRank);
        }
        private static void AddContactIdParameter(string contactId, SqlCommand cmd)
        {
            var parameterContactId = new SqlParameter("@ContactId", SqlDbType.VarChar, 50);
            parameterContactId.Value = contactId;
            cmd.Parameters.Add(parameterContactId);
        }
        private static void AddDateParameter(DateTime date, SqlCommand cmd)
        {
            var parameterDate = new SqlParameter("@Date", SqlDbType.DateTime);
            parameterDate.Value = date.Date;
            cmd.Parameters.Add(parameterDate);
        }

        private static void AddItemIdParameter(string itemId, SqlCommand cmd)
        {
            var parameterItemId = new SqlParameter("@ItemId", SqlDbType.VarChar, 100);
            parameterItemId.Value = itemId;
            cmd.Parameters.Add(parameterItemId);
        }
        #endregion Used to Add Parameters
    }

    public static class ReaderExtensions
    {
        public static string SafeGetString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }
        public static int SafeGetInt32(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt32(colIndex);
            return 0;
        }
        public static decimal SafeGetDecimal(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetDecimal(colIndex);
            return 0;
        }
    }
}
