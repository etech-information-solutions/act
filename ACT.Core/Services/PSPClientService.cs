using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;
using System.Linq;

namespace ACT.Core.Services
{
    public class PSPClientService : BaseService<PSPClient>, IDisposable
    {
        public PSPClientService()
        {

        }
        /// <summary>
        /// Gets a total count of PSP Client matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public int Total1(PagingModel pm, CustomSearchModel csm)
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
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmPSPSId", csm.PSPId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(cp.Id)
                             FROM
                                [dbo].[PSPClient] cp
                                INNER JOIN [dbo].[PSP] p ON p.Id = cp.PSPId
                                INNER JOIN [dbo].[Client] c ON c.Id = cp.ClientId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show clients for logged in PSP
            if (CurrentUser.RoleType == RoleType.PSP)
            {
                query = $@"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId WHERE pc.ClientId=c.Id AND pu.UserId=@userid ) ";
            }
            else if (CurrentUser.RoleType == RoleType.Client)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.UserId=@userid AND cu.ClientId=c.Id)";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if (csm.ClientId > 0)
            {
                query = $"{query} AND cp.ClientId=@csmClientId ";
            }
            if (csm.PSPId > 0)
            {
                query = $"{query} AND cp.PSPId=@csmPSPId ";
            }

            if (csm.FromDate.HasValue && csm.ToDate.HasValue)
            {
                query = $"{query} AND (cp.CreatedOn >= @csmFromDate AND cp.CreatedOn <= @csmToDate) ";
            }
            else if (csm.FromDate.HasValue || csm.ToDate.HasValue)
            {
                if (csm.FromDate.HasValue)
                {
                    query = $"{query} AND (cp.CreatedOn>=@csmFromDate) ";
                }
                if (csm.ToDate.HasValue)
                {
                    query = $"{query} AND (cp.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if (!string.IsNullOrEmpty(csm.Query))
            {
                query = string.Format(@"{0} AND (cp.[AccountingCode] LIKE '%{1}%' OR
                                                  cp.[ProductDescription] LIKE '%{1}%' OR
                                                  p.[Name] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim());
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>(query, parameters.ToArray()).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Client Products matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<PSPClientCustomModel> List1(PagingModel pm, CustomSearchModel csm)
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
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                cp.*,
                                p.[CompanyName] AS [PspName],
                                c.[CompanyName] AS [ClientName]
                             FROM
                                [dbo].[PSPProduct] cp
                                INNER JOIN [dbo].[PSP] p ON p.Id = cp.PSPId
                                INNER JOIN [dbo].[Client] c ON c.Id = cp.ClientId";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show clients for logged in PSP
            if (CurrentUser.RoleType == RoleType.PSP)
            {
                query = $@"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu INNER JOIN [dbo].[PSPClient] pc ON pc.PSPId=pu.PSPId WHERE pc.ClientId=c.Id AND pu.UserId=@userid ) ";
            }
            else if (CurrentUser.RoleType == RoleType.Client)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.UserId=@userid AND cu.ClientId=c.Id)";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if (csm.ClientId > 0)
            {
                query = $"{query} AND cp.ClientId = @csmClientId ";
            }
            if (csm.PSPId > 0)
            {
                query = $"{query} AND cp.PSPId=@csmPSPId ";
            }

            if (csm.FromDate.HasValue && csm.ToDate.HasValue)
            {
                query = $"{query} AND (cp.CreatedOn >= @csmFromDate AND cp.CreatedOn <= @csmToDate) ";
            }
            else if (csm.FromDate.HasValue || csm.ToDate.HasValue)
            {
                if (csm.FromDate.HasValue)
                {
                    query = $"{query} AND (cp.CreatedOn>=@csmFromDate) ";
                }
                if (csm.ToDate.HasValue)
                {
                    query = $"{query} AND (cp.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if (!string.IsNullOrEmpty(csm.Query))
            {
                query = string.Format(@"{0} AND (cp.[AccountingCode] LIKE '%{1}%' OR
                                                  cp.[ProductDescription] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim());
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format("{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query);

            return context.Database.SqlQuery<PSPClientCustomModel>(query, parameters.ToArray()).ToList();
        }

        /// <summary>
        /// Gets a list of products using the specified client id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<PSPClient> ListByClient(int clientId)
        {
            return context.PSPClients.Include("Product").Where(cp => cp.ClientId == clientId).ToList();
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPClient> PSPClientList()
        {
            var tt = (from d in context.PSPClients select d);
            if (tt.Count() > 0)
            {
                List<PSPClient> records = (from s in context.PSPClients where s.Status == 1 select s).ToList();
                if (records != null)
                {
                    return records;
                }
                else return null;
            }
            else
                return null;
        }

    }
}