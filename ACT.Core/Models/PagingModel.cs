using System;

namespace ACT.Core.Models
{
    public class PagingModel
    {
        /// <summary>
        /// The number of records to Skip
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// The number of records to Fetch
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// The page number e.g. Page 1
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The enter search string
        /// </summary>
        public string Query { get; set; }

        public string Sort { get; set; }

        public string SortBy { get; set; }

        public PagingModel()
        {
            this.Sort = "DESC";
            this.SortBy = "CreatedOn";
            this.Take = ConfigSettings.PagingTake;
        }
    }
}