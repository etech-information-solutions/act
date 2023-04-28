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
    public class PSPBillingService : BaseService<PSPBilling>, IDisposable
    {
        public PSPBillingService()
        {

        }

        /// <summary>
        /// Gets a total count of Billing items matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public int Total1( PagingModel pm, CustomSearchModel csm )
        {
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date )
            {
                csm.ToDate = csm.ToDate?.AddDays( 1 );
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                { new SqlParameter( "csmPSPProductId", csm.PSPProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            COUNT(b.[Id]) AS [Total]
                             FROM
	                            [dbo].[PSPBilling] b
                                INNER JOIN [dbo].[PSP] p ON p.[Id]=b.[PSPId]
                                INNER JOIN [dbo].[PSPProduct] p1 ON p1.[Id]=b.[PSPProductId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                (b.[PSPId]=pu.[PSPId])
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
                                                            (pc.[PSPId]=b.[PSPId])
                                                       )
                                                )
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.PSPId != 0 )
            {
                query = $"{query} AND (p.Id=@csmPSPId) ";
            }
            if ( csm.PSPProductId != 0 )
            {
                query = $"{query} AND (p1.Id=@csmPSPProductId) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (b.CreatedOn >= @csmFromDate AND b.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (b.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (b.[StatementNumber] LIKE '%{1}%' OR
                                                  b.[ReferenceNumber] LIKE '%{1}%' OR
                                                  b.[NominatedAccount] LIKE '%{1}%' OR
                                                  p.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[TradingAs] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  p.[VATNumber] LIKE '%{1}%' OR
                                                  p.[ContactNumber] LIKE '%{1}%' OR
                                                  p.[ContactPerson] LIKE '%{1}%' OR
                                                  p.[FinancialPerson] LIKE '%{1}%' OR
                                                  p.[Email] LIKE '%{1}%' OR
                                                  p.[AdminEmail] LIKE '%{1}%' OR
                                                  p1.[Name] LIKE '%{1}%' OR
                                                  p1.[Description] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Billing items matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<PSPBillingCustomModel> List1( PagingModel pm, CustomSearchModel csm )
        {
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date )
            {
                csm.ToDate = csm.ToDate?.AddDays( 1 );
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "csmPSPId", csm.PSPId ) },
                {new SqlParameter( "csmClientId", csm.ClientId) },
                { new SqlParameter( "csmPSPProductId", csm.PSPProductId ) },
               // { new SqlParameter( "csmPSPProductId", csm.PSPProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion          

        
            // WHERE
            string query = @"SELECT
	                            b.*,
	                            p.CompanyName AS [PSPName],
                                c.CompanyName As [ClientName],
	                            p1.Name AS [ProductName]
                             FROM
	                            [dbo].[PSPBilling] b
                                INNER JOIN [dbo].[Client]c on c.[Id] = b.[ClientId] 
                                INNER JOIN [dbo].[PSP] p ON p.[Id]=b.[PSPId]
                                INNER JOIN [dbo].[Product] p1 ON p1.[Id]=b.[PSPProductId]"; 

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $@"{query} AND EXISTS (SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                (b.[PSPId]=pu.[PSPId])
                                             ) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $@"{query} AND EXISTS (SELECT
                                                1
                                              FROM
                                                [dbo].[ClientUser] cu 
                                              WHERE
                                                (cu.UserId=@userid) AND
                                                (EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc
                                                        WHERE
                                                            (pc.[ClientId]=cu.[ClientId]) AND
                                                            (pc.[PSPId]=b.[PSPId])
                                                       )
                                                )
                                             ) ";
            }

            #endregion

            // Limit to only show PSP for logged in user
            if (!CurrentUser.IsAdmin)
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId AND pu.UserId=@userid) ";
            }

//#endregion

            // Custom Search

            #region Custom Search

            if (csm.PSPId != 0)
            {
               // query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPBilling] pc WHERE pc.Id=@csmPSPId) ";
                query = $"{query} AND p.Id=@csmPSPId ";
            }
            if (csm.PSPProductId != 0)
            {
               // query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId AND pp.ProductId=@csmProductId) ";
                query = $"{query} AND b.PSPProductId=@csmPSPProductId ";

            }
            if (csm.ClientId != 0)
            {
               // query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPClient] pp WHERE p.Id=pp.ClientId AND pp.ProductId=@csmProductId) ";
               query = $"{query} AND p.Id=pp.ClientId AND b.PSPProductId=@csmProductId ";
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

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format(@"{0} AND (b.[StatementNumber] LIKE '%{1}%' OR
                                                  b.[ReferenceNumber] LIKE '%{1}%' OR
                                                  b.[NominatedAccount] LIKE '%{1}%' OR
                                                  p.[StatementDate] LIKE '%{1}%' OR
                                                  p.[Rate] LIKE '%{1}%' OR
                                                  p.[Units] LIKE '%{1}%' OR
                                                  p.[InvoiceAmount] LIKE '%{1}%' OR
                                                  p.[PaymentAmount] LIKE '%{1}%' OR
                                                  p.[PaymentDate] LIKE '%{1}%' OR
                                                  p.[Status] LIKE '%{1}%' OR
                                                  p.[TaxAmount] LIKE '%{1}%' OR                                                 
                                                    p1.[PSPId] LIKE '%{1}%' OR,
                                                    p1.[PSPProductId] LIKE '%{1}%' OR
                                                    p1.[ClientId] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion
	 
	

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

           query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<PSPBillingCustomModel> model = context.Database.SqlQuery<PSPBillingCustomModel>( query, parameters.ToArray() ).ToList();
            return model;
        }

        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List(bool v)
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = string.Empty;

            query = $"SELECT p.Id AS [TKey], p.Name AS [TValue] FROM [dbo].[PSPBilling] p WHERE (1=1)";

            #region WHERE

            if (CurrentUser.RoleType == RoleType.PSP)
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
            else if (CurrentUser.RoleType == RoleType.Client)
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

            model = context.Database.SqlQuery<IntStringKeyValueModel>(query.Trim(), parameters.ToArray()).ToList();

            if (model != null && model.Any())
            {

                foreach (var k in model)
                {
                    if (clientOptions.Keys.Any(x => x == k.TKey))
                        continue;

                    clientOptions.Add(k.TKey, (k.TValue ?? "").Trim());
                }
            }

            return clientOptions;
        }


        /// <summary>
        /// Gets a role using the specified id
        /// </summary>
        /// <param name="id">Id of the role to be fetched</param>
        /// <returns></returns>
        public override PSPBilling GetById(int id)
        {
            return context.PSPBillings                        
                          .FirstOrDefault(c => c.ClientId == id);
        }

        public PSPBilling GetPSPDetails(int id)
        {
            var returnItem = (from d in context.PSPBillings where d.Id == id select d).FirstOrDefault();

            return returnItem;
        }

        public PSPBilling GetClientDetails(int id)
        {
            var returnItem = (from d in context.PSPBillings where d.ClientId == id select d).FirstOrDefault();

            return returnItem;
        }

        public  List<PSPBilling> GetPSPClientDetails(int psp, int client)
        {
            var returnItem = (from d in context.PSPBillings where d.ClientId == client && d.PSPId == psp
                              && d.PaymentAmount == null && d.PaymentDate == null select d).ToList();

            return returnItem;
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public bool CheckPSPBilling(PSPBilling model)
        {
            var tt = (from d in context.PSPBillings select d);
            if (tt.Count() > 0)
            {
                var checkRecord = (from s in context.PSPBillings where s.PSPProductId == model.PSPProductId && s.PSPId== model.PSPId && s.StatementDate>DateTime.Now  select s);

                if (checkRecord.Count()> 0)
                {
                    return true;
                }
                else { return false; }
            }
            else
                    return true;                
                      
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPBilling> GetPSPBillingList(int psp)
        {
            var tt = (from d in context.PSPBillings select d);
            if (tt.Count() > 0)
            {
                var checkRecord = (from s in context.PSPBillings where s.PSPId == psp && s.PaymentAmount==null && s.PaymentDate==null select s).ToList();

                if (checkRecord.Count() > 0)
                {
                    return checkRecord;
                }
                else { return null; }
            }
            else
                return null;

        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<int> GetDistinctPSPBillingList()
        {
            var tt = (from d in context.PSPBillings select d);
            if (tt.Count() > 0)
            {
                var checkRecord = (from s in context.PSPBillings where s.PaymentAmount == null && s.PaymentDate == null select s.PSPId).Distinct().ToList();

                if (checkRecord.Count() > 0)
                {
                    return checkRecord;
                }
                else { return null; }
            }
            else
                return null;

        }


        /// </summary>      
        /// 
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPBilling> PSPBillingList()
        {
            var tt = (from d in context.PSPBillings select d);
            if (tt.Count() > 0)
            {
                var checkRecord = (from s in context.PSPBillings where  s.PaymentAmount == null && s.PaymentDate == null select s).ToList();

                if (checkRecord.Count() > 0)
                {
                    return checkRecord;
                }
                else { return null; }
            }
            else
                return null;

        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public List<PSPBilling> GetClientBillingList(int client, int psp)
        {
            var tt = (from d in context.PSPBillings select d);
            if (tt.Count() > 0)
            {
                var checkRecord = (from s in context.PSPBillings where s.ClientId == client && s.PSPId == psp
                                   && s.PaymentDate==null && s.PaymentAmount==null select s).ToList();

                if (checkRecord.Count() > 0)
                {
                    return checkRecord;
                }
                else { return null; }
            }
            else
                return null;

        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public string GenerateReference(int id)
        {
            var Reference = ""; int red;
            var spp = ""; var spp1 = "";
            var tt = (from d in context.PSPBillings where d.PSPId == id orderby d.Id descending select d).FirstOrDefault();
            
            if (tt != null)
            {
                int len = tt.ReferenceNumber.Length;
                if (len == 10)
                {
                    spp = tt.ReferenceNumber.Substring(0, 2);
                    spp1 = tt.ReferenceNumber.Substring(2, 8);
                }
                else
                {
                    spp = tt.ReferenceNumber.Substring(0, 1);
                    spp1 = tt.ReferenceNumber.Substring(1, 8);
                }
                
                if (int.TryParse(spp, out red))
                {
                    if (red == id)
                    {
                       // var stest = tt.ReferenceNumber.Substring(2, (len - 1));
                        int.TryParse(spp1, out red);
                        red++;
                        string s = red.ToString().PadLeft(8, '0');
                        Reference = (id + s).ToString();

                        return Reference;
                    }
                    else { Reference = (id + "00000001").ToString(); }
                }
                else { Reference = (id + "00000001").ToString(); }
            }

            else
                Reference = (id + "00000001").ToString();
            return Reference;

        }
    }
}
