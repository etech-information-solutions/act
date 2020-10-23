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
    public class SiteAuditService : BaseService<SiteAudit>, IDisposable
    {
        public SiteAuditService()
        {

        }

        /// <summary>
        /// Gets a Site Audit using the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override SiteAudit GetById( int id )
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById( id );
        }

        /// <summary>
        /// Gets the total number of Site Audits matching the specified params
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
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmSact", Status.Active ) },
                { new SqlParameter( "csmStatus", csm.Status ) },
            };

            #endregion

            string query = @"SELECT
	                            COUNT(sa.[Id]) AS [Total]
                             FROM
	                            [dbo].[SiteAudit] sa
                                INNER JOIN [dbo].[Site] s ON s.[Id]=sa.[SiteId]
                                LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=sa.[ClientId]
                                LEFT OUTER JOIN [dbo].[PSPClient] pc ON pc.[ClientId]=c.[Id]
                                LEFT OUTER JOIN [dbo].[PSP] p ON pc.[PSPId]=p.[Id]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[Region] r WHERE (r.[Id]=s.[RegionId]) AND (pu.UserId=@userid) AND (r.[PSPId]=pu.[PSPId])) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE (cu.UserId=@userid) AND (EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[SiteId]=s.[Id]))) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND (sa.SiteId=@csmSiteId) ";
            }
            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND (sa.ClientId=@csmClientId) ";
            }
            if ( csm.PSPId != 0 )
            {
                query = $"{query} AND (sa.PSPId=@csmPSPId) ";
            }

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (sa.Status=@csmStatus) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (sa.CreatedOn >= @csmFromDate AND sa.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (sa.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (sa.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (s.[Name] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[ContactPerson] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[ContactPerson] LIKE '%{1}%' OR
                                                  sa.[Equipment] LIKE '%{1}%' OR
                                                  sa.[CustomerName] LIKE '%{1}%' OR
                                                  sa.[RepName] LIKE '%{1}%' OR
                                                  sa.[PalletAuditor] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Site Audits matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<SiteAuditCustomModel> List1( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmSact", Status.Active ) },
                { new SqlParameter( "csmStatus", csm.Status ) },
            };

            #endregion

            string query = @"SELECT
	                            sa.*,
	                            s.Name AS [SiteName],
	                            p.CompanyName AS [PSPName],
	                            c.CompanyName AS [ClientName],
	                            (SELECT TOP 1 d.Id FROM [dbo].[Document] d WHERE d.ObjectId=sa.Id AND d.ObjectType='SiteAudit' AND d.Status=@csmSact) AS [ReportDocumentId]
                             FROM
	                            [dbo].[SiteAudit] sa
                                INNER JOIN [dbo].[Site] s ON s.[Id]=sa.[SiteId]
                                LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=sa.[ClientId]
                                LEFT OUTER JOIN [dbo].[PSPClient] pc ON pc.[ClientId]=c.[Id]
                                LEFT OUTER JOIN [dbo].[PSP] p ON pc.[PSPId]=p.[Id]";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show Disputes for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[Region] r WHERE (r.[Id]=s.[RegionId]) AND (pu.UserId=@userid) AND (r.[PSPId]=pu.[PSPId])) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE (cu.UserId=@userid) AND (EXISTS(SELECT 1 FROM [dbo].[ClientSite] cs WHERE cs.[SiteId]=s.[Id]))) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.SiteId != 0 )
            {
                query = $"{query} AND (sa.SiteId=@csmSiteId) ";
            }
            if ( csm.ClientId != 0 )
            {
                query = $"{query} AND (sa.ClientId=@csmClientId) ";
            }
            if ( csm.PSPId != 0 )
            {
                query = $"{query} AND (sa.PSPId=@csmPSPId) ";
            }

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (sa.Status=@csmStatus) ";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (sa.CreatedOn >= @csmFromDate AND sa.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (sa.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (sa.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (s.[Name] LIKE '%{1}%' OR
                                                  s.[Description] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[ContactPerson] LIKE '%{1}%' OR
                                                  p.[CompanyName] LIKE '%{1}%' OR
                                                  p.[ContactPerson] LIKE '%{1}%' OR
                                                  sa.[Equipment] LIKE '%{1}%' OR
                                                  sa.[CustomerName] LIKE '%{1}%' OR
                                                  sa.[RepName] LIKE '%{1}%' OR
                                                  sa.[PalletAuditor] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<SiteAuditCustomModel>( query, parameters.ToArray() ).ToList();
        }
    }
}
