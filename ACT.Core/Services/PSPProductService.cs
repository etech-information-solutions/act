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
    public class PSPProductService : BaseService<PSPProduct>, IDisposable
    {
        public PSPProductService()
        {

        }


        /// <summary>
        /// Gets a psp product the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override PSPProduct GetById(int id)
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById(id);
        }

        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v )
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = string.Empty;

            query = $"SELECT p.Id AS [TKey], p.Name AS [TValue] FROM [dbo].[PSPProduct] p WHERE (1=1)";

            #region WHERE

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                (p.[PSPId]=pu.[PSPId])
                                                )
                                             ) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[ClientUser] cu 
                                              WHERE
                                                (cu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc
                                                        WHERE
                                                            (pc.[ClientId]=cu.[ClientId]) AND
                                                            (pc.[PSPId]=p.[PSPId])
                                                       )
                                                )
                                             ) ";
            }

            #endregion

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {

                foreach ( var k in model )
                {
                    if ( clientOptions.Keys.Any( x => x == k.TKey ) )
                        continue;

                    clientOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return clientOptions;
        }

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
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "csmProductId", csm.ProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmPSPClientStatus", ( int ) csm.PSPClientStatus ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(1) AS [Total]
                             FROM
                                [dbo].[PSP] p";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if (!CurrentUser.IsAdmin)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId AND pu.UserId=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if (csm.ClientId != 0)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSP] pc WHERE p.Id=pc.PSPId AND pc.PSPId=@csmPSPId) ";
            }
            if (csm.ProductId != 0)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId AND pp.ProductId=@csmProductId) ";
            }
            if (csm.PSPClientStatus != Enums.PSPClientStatus.All)
            {
                query = $"{query} AND (p.Status=@csmPSPClientStatus) ";
            }

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
                query = string.Format(@"{0} AND (p.[RateUnit] LIKE '%{1}%' OR
                                                  p.[Rate] LIKE '%{1}%' OR
                                                  p.[Name] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  p.[StartDate] LIKE '%{1}%' OR
                                                  p.[EndDate] LIKE '%{1}%' OR
                                                  p.[ProductId] LIKE '%{1}%' OR
                                                  p.[ProductId] LIKE '%{1}%' OR
                                                  p.[Status] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim());
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>(query, parameters.ToArray()).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of PSPs matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<PSPProductCustomModel> List1(PagingModel pm, CustomSearchModel csm)
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
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "csmProductId", csm.ProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmPSPClientStatus", ( int ) csm.PSPClientStatus ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                p.*,
                                (SELECT COUNT(1) FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId) AS [UserCount],
                                (SELECT COUNT(1) FROM [dbo].[PSPClient] pc WHERE p.Id=pc.PSPId) AS [ClientCount],
                                (SELECT COUNT(1) FROM [dbo].[PSPBilling] pi WHERE p.Id=pi.PSPId) AS [InvoiceCount],
                                (SELECT COUNT(1) FROM [dbo].[Document] d WHERE p.Id=d.ObjectId AND d.ObjectType='PSP') AS [DocumentCount]
                             FROM
                                [dbo].[PSPProduct] p";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if (!CurrentUser.IsAdmin)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId AND pu.UserId=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if (csm.PSPId != 0)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSP] pc WHERE p.Id=pc.PSPId AND pc.PSPId=@csmPSPId) ";
            }
            if (csm.ProductId != 0)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId AND pp.ProductId=@csmProductId ) ";
            }
            if (csm.PSPClientStatus != Enums.PSPClientStatus.All)
            {
                query = $"{query} AND (p.Status=@csmPSPClientStatus) ";
            }

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
                query = string.Format(@"{0} AND (p.[RateUnit] LIKE '%{1}%' OR
                                                  p.[Rate] LIKE '%{1}%' OR
                                                  p.[Name] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  p.[StartDate] LIKE '%{1}%' OR
                                                  p.[EndDate] LIKE '%{1}%' OR
                                                  p.[ProductId] LIKE '%{1}%' OR
                                                  p.[ProductId] LIKE '%{1}%' OR
                                                  p.[Status] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim());
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format("{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query);

            List<PSPProductCustomModel> model = context.Database.SqlQuery<PSPProductCustomModel>(query, parameters.ToArray()).ToList();

           return model;
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public PSPProduct CheckPSPProduct(int pspid, int productid)
        {
            var tt = (from d in context.PSPProducts select d);
            if (tt.Count() > 0)
            {
                PSPProduct checkRecord = (from s in context.PSPProducts where s.ProductId == productid && s.PSPId == pspid && s.EndDate > DateTime.Today  select s).FirstOrDefault();

                if (checkRecord != null)
                {
                    return checkRecord;
                }
                else return null;
              
            }
            else
                return null;
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPProduct> GetPSPProductList(int pspid, int productid)
        {
            var tt = (from d in context.PSPProducts select d);
            if (tt.Count() > 0)
            {
               List<PSPProduct> checkRecord = (from s in context.PSPProducts where s.ProductId == productid && s.PSPId == pspid 
                                               && s.EndDate > DateTime.Today && s.Status == 1 orderby s.PSPId, s.ProductId descending
                                               select s).ToList();
                if (checkRecord != null)
                {
                    return checkRecord;
                }
                else return null;
            }
            else
                return null;
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPProduct> MonthlyPSPProductList(int pspid)
        {
            var tt = (from d in context.PSPProducts select d);
            if (tt.Count() > 0)
            {
                List<PSPProduct> checkRecord = (from s in context.PSPProducts where s.PSPId == pspid && s.RateUnit == (int) RateUnit.Monthly
                                             && s.EndDate > DateTime.Today && s.Status == 1 orderby s.PSPId, s.ProductId descending select s).ToList();

                if (checkRecord != null)
                {
                    return checkRecord;
                }
                else return null;
            }
            else
                return null;
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPProduct> TransactionalPSPProductList(int pspid)
        {
            var tt = (from d in context.PSPProducts select d);
            if (tt.Count() > 0)
            {
                List<PSPProduct> checkRecord = (from s in context.PSPProducts where s.RateUnit ==(int) RateUnit.Weekly && s.PSPId == pspid
                                                 && s.EndDate > DateTime.Today && s.Status == 1 orderby s.PSPId, s.ProductId descending  select s).ToList();
                if (checkRecord != null)
                {
                    return checkRecord;
                }
                else return null;

            }
            else
                return null;
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPProduct> AnnualPSPProductList(int pspid)
        {
            var tt = (from d in context.PSPProducts select d);
            if (tt.Count() > 0)
            {
                List<PSPProduct> checkRecord = (from s in context.PSPProducts where s.RateUnit == (int) RateUnit.Annually && s.PSPId == pspid
                                                 && s.EndDate > DateTime.Today && s.Status == 1 orderby s.PSPId, s.ProductId descending select s).ToList();

                if (checkRecord != null)
                {
                    return checkRecord;
                }
                else return null;

            }
            else
                return null;
        }
        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPProduct> PSPProductList()
        {
            var tt = (from d in context.PSPProducts select d);
            if (tt.Count() > 0)
            {
                List<PSPProduct> records = (from s in context.PSPProducts where s.Status == 1 && s.EndDate > DateTime.Today select s).ToList();
                                        //   .GroupBy(s=>s.PSPId);
                                            

                if (records != null)
                {
                    return records;
                }
                else return null;

            }
            else
                return null;
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPProduct> PSPProductListById(int pspid)
        {
            var tt = (from d in context.PSPProducts.Where(d=>d.PSPId == pspid) select d );
            if (tt.Count() > 0)
            {
                List<PSPProduct> records = (from s in context.PSPProducts where s.Status == 1
                     && s.PSPId == pspid && s.EndDate > DateTime.Today select s).ToList();
                


                if (records != null)
                {
                    return records;
                }
                else return null;

            }
            else
                return null;
        }

        /// <summary>
        /// Updates an existing psp product
        /// </summary>
        /// <param name="pspproduct">An instance of a psp product to be updated</param>
        /// <returns></returns>
        public PSPProduct Update(PSPProduct pSPProduct)
        {
            return base.Update(pSPProduct);
        }


    }
}
