using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Services
{
  public  class BillingInvoiceService : BaseService<BillingInvoice>, IDisposable
    {
        public BillingInvoiceService()
        {

        }

        /// <summary>
        /// Gets a billing invoice the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override BillingInvoice GetById(int id)
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById(id);
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
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            COUNT(d.[Id]) AS [Total]
                             FROM
	                            [dbo].[BillingInvoice] d
                                INNER JOIN [dbo].[Client] c ON c.[Id]=d.[ClientId]
                                LEFT OUTER JOIN [dbo].[PSP] s ON s.[Id]=d.[PSPId]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if (CurrentUser.RoleType == RoleType.PSP)
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE pc.[PSPId]=pu.[PSPId] AND pc.[ClientId]=c.[Id])
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
                                                (cu.ClientId=c.Id)
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if (csm.ClientId != 0)
            {
                query = $"{query} AND (d.[ClientId]=@csmClientId) ";
            }
            if (csm.PSPId != 0)
            {
                query = $"{query} AND (s.Id=@csmPSPId) ";
            }

            if (csm.FromDate.HasValue && csm.ToDate.HasValue)
            {
                query = $"{query} AND (d.CreatedOn >= @csmFromDate AND d.CreatedOn <= @csmToDate) ";
            }
            else if (csm.FromDate.HasValue || csm.ToDate.HasValue)
            {
                if (csm.FromDate.HasValue)
                {
                    query = $"{query} AND (d.CreatedOn>=@csmFromDate) ";
                }
                if (csm.ToDate.HasValue)
                {
                    query = $"{query} AND (d.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if (!string.IsNullOrEmpty(csm.Query))
            {
                query = string.Format(@"{0} AND (d.[ContactPerson] LIKE '%{1}%' OR
                                                  d.[InvoiceAmount] LIKE '%{1}%' OR
                                                  d.[InvoiceNumber] LIKE '%{1}%' OR
                                                  d.[DueDate] LIKE '%{1}%' OR
                                                  d.[StatementDate] LIKE '%{1}%' OR
                                                  d.[EmailAddress] LIKE '%{1}%' OR
                                                  d.[ContactNumber] LIKE '%{1}%' OR
                                                  d.[ContactEmail] LIKE '%{1}%' OR
                                                  d.[InvoiceType] LIKE '%{1}%' OR
                                                  d.[PSPId] LIKE '%{1}%' OR
                                                  d.[ClientId] LIKE '%{1}%' OR
                                                  d.[Total] LIKE '%{1}%' OR
                                                  d.[AccountNumber] LIKE '%{1}%' OR
                                                  s.[CompanyName] LIKE '%{1}%' OR
                                                  c.[TradingAs] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim());
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>(query, parameters.ToArray()).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Delivery Notes matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<BillingInvoiceCustomModel> List1(PagingModel pm, CustomSearchModel csm)
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
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
	                            d.*,
	                            s.[CompanyName] AS [PSPName],
	                            c.[CompanyName] AS [ClientName]
	                             FROM
	                            [dbo].[BillingInvoice] d
                                INNER JOIN [dbo].[Client] c ON c.[Id]=d.[ClientId]
                                LEFT OUTER JOIN [dbo].[PSP] s ON s.[Id]=d.[PSPId]";
                                

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if (CurrentUser.RoleType == RoleType.PSP)
            {
                query = $@"{query} AND EXISTS(SELECT
                                                1
                                              FROM
                                                [dbo].[PSPUser] pu
                                              WHERE
                                                (pu.UserId=@userid) AND
                                                EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE pc.[PSPId]=pu.[PSPId] AND pc.[ClientId]=c.[Id])
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
                                                (cu.ClientId=c.Id)
                                             ) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if (csm.ClientId != 0)
            {
                query = $"{query} AND (d.[ClientId]=@csmClientId) ";
            }
            if (csm.PSPId != 0)
            {
                query = $"{query} AND (s.Id=@csmPSPId) ";
            }

            if (csm.FromDate.HasValue && csm.ToDate.HasValue)
            {
                query = $"{query} AND (d.CreatedOn >= @csmFromDate AND d.CreatedOn <= @csmToDate) ";
            }
            else if (csm.FromDate.HasValue || csm.ToDate.HasValue)
            {
                if (csm.FromDate.HasValue)
                {
                    query = $"{query} AND (d.CreatedOn>=@csmFromDate) ";
                }
                if (csm.ToDate.HasValue)
                {
                    query = $"{query} AND (d.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if (!string.IsNullOrEmpty(csm.Query))
            {
                query = string.Format(@"{0} AND (d.[ContactPerson] LIKE '%{1}%' OR
                                                  d.[InvoiceAmount] LIKE '%{1}%' OR
                                                  d.[InvoiceNumber] LIKE '%{1}%' OR
                                                  d.[DueDate] LIKE '%{1}%' OR
                                                  d.[StatementDate] LIKE '%{1}%' OR
                                                  d.[EmailAddress] LIKE '%{1}%' OR
                                                  d.[ContactNumber] LIKE '%{1}%' OR
                                                  d.[ContactEmail] LIKE '%{1}%' OR
                                                  d.[InvoiceType] LIKE '%{1}%' OR
                                                  d.[PSPId] LIKE '%{1}%' OR
                                                  d.[ClientId] LIKE '%{1}%' OR
                                                  d.[Total] LIKE '%{1}%' OR
                                                  d.[AccountNumber] LIKE '%{1}%' OR
                                                  s.[CompanyName] LIKE '%{1}%' OR
                                                  c.[TradingAs] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim());
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format("{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query);

            return context.Database.SqlQuery<BillingInvoiceCustomModel>(query, parameters.ToArray()).ToList();
        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public bool CheckBillingInvoice(BillingInvoice model)
        {
            var tt = (from d in context.BillingInvoices select d);
            if (tt.Count() > 0)
            {
                var checkRecord = (from s in context.BillingInvoices where s.InvoiceNumber == model.InvoiceNumber
                                   && s.InvoiceType == model.InvoiceType select s);

                if (checkRecord.Count() > 0)
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
        public BillingInvoice GetBillingInvoice(int id)
        {
            var tt = (from d in context.BillingInvoices select d);
            if (tt.Count() > 0)
            {
                BillingInvoice checkRecord = (from s in context.BillingInvoices  where s.Id == id select s).FirstOrDefault();

                if (checkRecord!=null)
                {
                    return checkRecord;
                }
                else { return null; }
            }
            else
                return null;

        }
    }
}
