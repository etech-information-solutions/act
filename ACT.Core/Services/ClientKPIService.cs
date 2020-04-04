using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientKPIService : BaseService<ClientKPI>, IDisposable
    {
        public ClientKPIService()
        {

        }

            /// <summary>
            /// Gets a list of PSPs matching the specified search params
            /// </summary>
            /// <param name="pm"></param>
            /// <param name="csm"></param>
            /// <returns></returns>
            public List<ClientKPI> ListCSM(PagingModel pm, CustomSearchModel csm)
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
                { new SqlParameter( "csmProductId", csm.ProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmPSPClientStatus", ( int ) csm.PSPClientStatus ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

                #endregion

                string query = @"SELECT
                                p.*
                                FROM
                                [dbo].[ClientKPI] p";

                // WHERE

                #region WHERE

                query = $"{query} WHERE (1=1)";

                // Limit to only show PSP for logged in user
                if (!CurrentUser.IsAdmin)
                {
                    //add to mke sure only correct sites are seen            
                    query = $"{query} AND EXISTS(SELECT 1 FROM[dbo].[PSPUser] pu LEFT JOIN[dbo].[PSPClient] pc ON pc.PSPId = pu.PSPId LEFT JOIN [dbo].[ClientKPI] cs ON cs.ClientId=pc.ClientId WHERE pc.ClientId = p.ClientId AND pu.UserId = @userid) ";
                    //query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId AND pu.UserId=@userid) ";
                }

                #endregion

                // Custom Search

                #region Custom Search

                if (csm.ClientId != 0)
                {
                    //query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE p.Id=pc.PSPId AND pc.ClientId=@csmClientId) ";
                    query = $"{query} AND p.ClientId=@csmClientId ";
                }
                //if (csm.ProductId != 0)
                //{
                //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId AND pp.ProductId=@csmProductId) ";
                //}
                //if (csm.PSPClientStatus != Enums.PSPClientStatus.All)
                //{
                //    query = $"{query} AND (p.Status=@csmPSPClientStatus) ";
                //}

                if (csm.FromDate.HasValue && csm.ToDate.HasValue)
                {
                    query = $"{query} AND (p.CreatedOn >= @csmFromDate AND p.CreatedOn <= @csmToDate) ";
                }
                else if (csm.FromDate.HasValue || csm.ToDate.HasValue)
                {
                    if (csm.FromDate.HasValue)
                    {
                        query = $"{query} AND (p.CreatedOn>=@csmFromDate) ";
                    }
                    if (csm.ToDate.HasValue)
                    {
                        query = $"{query} AND (p.CreatedOn<=@csmToDate) ";
                    }
                }

                #endregion

                // Normal Search

                #region Normal Search

                if (!string.IsNullOrEmpty(csm.Query))
                {
                    query = string.Format(@"{0} AND (
                                                  p.[KPIDescription] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim());
                }

                #endregion

                // ORDER

                query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

                // SKIP, TAKE

                query = string.Format("{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query);

                List<ClientKPI> model = context.Database.SqlQuery<ClientKPI>(query, parameters.ToArray()).ToList();

                //if (model.NullableAny(p => p.DocumentCount > 0))
                //{
                //    using (DocumentService dservice = new DocumentService())
                //    {
                //        foreach (PSPCustomModel item in model.Where(p => p.DocumentCount > 0))
                //        {
                //            item.Documents = dservice.List(item.Id, "PSP");
                //        }
                //    }
                //}

                return model;
            }
        
        public ClientKPI GetByPSPId(int id)
        {
            return context.ClientKPIs.FirstOrDefault(b => b.ClientId == id);
        }

    }
}
