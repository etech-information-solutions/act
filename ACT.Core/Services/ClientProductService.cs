using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientProductService : BaseService<ClientProduct>, IDisposable
    {
            public ClientProductService()
            {

            }
            /// <summary>
            /// Gets a list of PSPs matching the specified search params
            /// </summary>
            /// <param name="pm"></param>
            /// <param name="csm"></param>
            /// <returns></returns>
            public List<ProductCustomModel> ListCSM(PagingModel pm, CustomSearchModel csm)
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
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                //{ new SqlParameter( "clientid", clientId > 0 ? clientId : 0 ) },
            };

                #endregion

                string query = @"SELECT
                                p.*,
                                (SELECT COUNT(1) FROM [dbo].[ProductPrice] pp WHERE pp.ProductId=p.Id) AS [ProductPriceCount],
                                (SELECT COUNT(1) FROM [dbo].[Document] d WHERE p.Id=d.ObjectId AND d.ObjectType='Product') AS [DocumentCount]
                             FROM
                                [dbo].[ClientProduct] p";

                // WHERE

                #region WHERE

                query = $"{query} WHERE (1=1)";

                #endregion

                #region WHERE IF CLIENT
                if (csm.ClientId > 0)
                {
                    query = $"{query} AND p.ClientId = @clientid ";
                }

                #endregion


                // Custom Search

                #region Custom Search

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
                    query = string.Format(@"{0} AND (p.[Name] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim());
                }

                #endregion

                // ORDER

                query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

                // SKIP, TAKE

                query = string.Format("{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query);

                List<ProductCustomModel> model = context.Database.SqlQuery<ProductCustomModel>(query, parameters.ToArray()).ToList();

                if (model.NullableAny(p => p.DocumentCount > 0))
                {
                    using (DocumentService dservice = new DocumentService())
                    {
                        foreach (ProductCustomModel item in model.Where(p => p.DocumentCount > 0))
                        {
                            item.Documents = dservice.List(item.Id, "Product");
                        }
                    }
                }

                return model;
            }

        
    }
}
