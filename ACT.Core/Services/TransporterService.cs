using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class TransporterService : BaseService<Transporter>, IDisposable
    {
        public TransporterService()
        {

        }

        /// <summary>
        /// Gets a list of Campaign Purchases using the specified params
        /// </summary>
        /// <param name="pm"></param> 
        /// <param name="csm"></param> 
        /// <returns></returns>
        public List<TransporterCustomModel> ListCSM(PagingModel pm, CustomSearchModel csm)
        {
            if (csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date)
            {
                csm.ToDate = csm.ToDate?.AddDays(1);
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = string.Format(@"SELECT
	                                         v.*
                                           FROM
	                                         [dbo].[Transporter] v");

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            // Custom Search

            #region Custom Search

            if (csm.FromDate.HasValue && csm.ToDate.HasValue)
            {
                query = $"{query} AND (v.CreatedOn >= @csmFromDate AND v.CreatedOn <= @csmToDate) ";
            }
            else if (csm.FromDate.HasValue || csm.ToDate.HasValue)
            {
                if (csm.FromDate.HasValue)
                {
                    query = $"{query} AND (v.CreatedOn>=@csmFromDate) ";
                }
                if (csm.ToDate.HasValue)
                {
                    query = $"{query} AND (v.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if (!string.IsNullOrEmpty(csm.Query))
            {
                query = string.Format(@"{0} AND (LOWER(REPLACE(v.TradingName, ' ', '')) LIKE '%{1}%' OR LOWER(REPLACE(v.Name, ' ', '')) LIKE '%{1}%' OR LOWER(REPLACE(v.RegistrationNumber, ' ', '')) LIKE '%{1}%') ", query, csm.Query.Trim().ToLower().Replace(" ", ""));
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format("{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query);

            return context.Database.SqlQuery<TransporterCustomModel>(query.Trim(), parameters.ToArray()).ToList();
        }
    }
}
