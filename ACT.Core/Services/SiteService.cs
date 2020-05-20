using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class SiteService : BaseService<Site>, IDisposable
    {
        public SiteService()
        {

        }

        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v, int regionId = 0 )
        {
            Dictionary<int, string> siteOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "regionId", regionId ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = string.Empty;

            query = $"SELECT s.Id AS [TKey], s.Name AS [TValue] FROM [dbo].[Site] s WHERE (1=1)";

            if ( regionId > 0 )
            {
                query = $"{query} AND (s.[RegionId]=@regionId)";
            }

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[Region] r WHERE r.Id=s.RegionId AND r.PSPId=pu.PSPId AND pu.UserId=@userid)";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu, [dbo].[PSPClient] pc, [dbo].[Region] r WHERE r.Id=s.RegionId AND r.PSPId=pc.PSPId AND cu.ClientId=pc.ClientId AND cu.UserId=@userid)";
            }

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( siteOptions.Keys.Any( x => x == k.TKey ) )
                        continue;

                    siteOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return siteOptions;
        }

        /// <summary>
        /// Gets a list of PSPs matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<Site> ListCSM( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmProductId", csm.ProductId ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmPSPClientStatus", ( int ) csm.PSPClientStatus ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmSiteId", csm.SiteId ) },
                { new SqlParameter( "csmRegion", csm.RegionId ) },

            };

            #endregion

            string query = @"SELECT
                                p.*
                                FROM
                                [dbo].[Site] p";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( !CurrentUser.IsAdmin )
            {
                /*
                 *     siteList = (from p in context.PSPClients
                        join c in context.ClientSites
                        on p.ClientId equals c.ClientId
                        join e in context.Sites
                        on c.SiteId equals e.Id
                        where p.PSPId == pspId
                        select e).ToList();
                 */
                //add to mke sure only correct sites are seen            
                query = $"{query} AND EXISTS(SELECT 1 FROM[dbo].[PSPUser] pu LEFT JOIN[dbo].[PSPClient] pc ON pc.PSPId = pu.PSPId LEFT JOIN [dbo].[ClientSite] cs ON cs.ClientId=pc.ClientId WHERE pu.UserId = @userid) ";
                //query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE p.Id=pu.PSPId AND pu.UserId=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.ClientId != 0 )
            {
                //query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE p.Id=pc.PSPId AND pc.ClientId=@csmClientId) ";
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientSite] pc WHERE p.Id=pc.SiteId AND pc.ClientId=@csmClientId) ";
            }
            if ( csm.SiteId != 0 )
            {
                //query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE p.Id=pc.PSPId AND pc.ClientId=@csmClientId) ";
                query = $"{query} AND p.Id=@csmSiteId ";
            }
            if ( csm.RegionId != 0 )
            {
                //query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPClient] pc WHERE p.Id=pc.PSPId AND pc.ClientId=@csmClientId) ";
                query = $"{query} AND p.RegionId=@csmRegion ";
            }


            if ( !string.IsNullOrEmpty( csm.SitePlanningPoint ) )
            {
                query = string.Format( @"{0} AND (p.PlanningPoint LIKE '%{1}%' OR p.[XCord] LIKE '%{1}%' OR p.[YCord] LIKE '%{1}%')", query, csm.SitePlanningPoint );
            }

            if ( !string.IsNullOrEmpty( csm.ContactName ) )
            {
                query = string.Format( @"{0} AND (p.[ContactName] LIKE '%{1}%' OR p.[ReceivingContact] LIKE '%{1}%' OR p.[FinanceContact] LIKE '%{1}%') ", query, csm.ContactName );
            }

            if ( !string.IsNullOrEmpty( csm.ContactNumber ) )
            {
                query = string.Format( @"{0} AND (p.[ContactNo] LIKE '%{1}%' OR p.[FinanceContactNo] LIKE '%{1}%' OR p.[ReceivingContactNo] LIKE '%{1}%') ", query, csm.ContactNumber );
            }

            if ( !string.IsNullOrEmpty( csm.ReferenceNumber ) )
            {
                query = string.Format( @"{0} AND (p.[AccountCode] LIKE '%{1}%' OR p.[SiteCodeChep] LIKE '%{1}%')", query, csm.ReferenceNumber );
            }

            //if (csm.ProductId != 0)
            //{
            //    query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPProduct] pp WHERE p.Id=pp.PSPId AND pp.ProductId=@csmProductId) ";
            //}
            //if (csm.PSPClientStatus != Enums.PSPClientStatus.All)
            //{
            //    query = $"{query} AND (p.Status=@csmPSPClientStatus) ";
            //}

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (p.CreatedOn >= @csmFromDate AND p.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (p.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (p.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (p.[Name] LIKE '%{1}%' OR
                                                  p.[Description] LIKE '%{1}%' OR
                                                  p.[XCord] LIKE '%{1}%' OR
                                                  p.[YCord] LIKE '%{1}%' OR
                                                  p.[Address] LIKE '%{1}%' OR
                                                  p.[AccountCode] LIKE '%{1}%' OR
                                                  p.[ContactNo] LIKE '%{1}%' OR
                                                  p.[ContactName] LIKE '%{1}%' OR
                                                  p.[Depot] LIKE '%{1}%' OR
                                                  p.[SiteCodeChep] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<Site> model = context.Database.SqlQuery<Site>( query, parameters.ToArray() ).ToList();

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

        public List<Site> GetSitesByClient( int clientId )
        {
            List<Site> siteList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            siteList = ( from p in context.ClientSites
                         join e in context.Sites
                         on p.SiteId equals e.Id
                         where p.ClientId == clientId
                         select e ).ToList();

            return siteList;
        }

        public List<Site> GetSubSitesBySiteClient( int clientId, int siteId )
        {
            List<Site> siteList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            siteList = ( from p in context.ClientSites
                         join e in context.Sites
                         on p.SiteId equals e.Id
                         where p.ClientId == clientId
                         where e.SiteId == siteId
                         select e ).ToList();

            return siteList;
        }

        public List<Site> GetSitesByClientsOfPSP( int pspId )
        {
            List<Site> siteList;
            siteList = ( from p in context.PSPClients
                         join c in context.ClientSites
                         on p.ClientId equals c.ClientId
                         join e in context.Sites
                         on c.SiteId equals e.Id
                         where p.PSPId == pspId
                         select e ).ToList();

            return siteList;
        }

        public List<Site> GetSitesByClients( int clientId )
        {
            List<Site> siteList;
            siteList = ( from c in context.ClientSites
                         join e in context.Sites
                         on c.SiteId equals e.Id
                         where c.ClientId == clientId
                         select e ).ToList();

            return siteList;
        }

        public List<Site> GetSitesByClients( int clientId, int siteId )
        {
            List<Site> siteList;
            siteList = ( from c in context.ClientSites
                         join e in context.Sites
                         on c.SiteId equals e.Id
                         where c.ClientId == clientId
                         where c.SiteId == siteId
                         select e ).ToList();

            return siteList;
        }

        public int GetClientBySite( int siteId )
        {
            int clientId;
            clientId = ( from c in context.ClientSites
                         where c.SiteId == siteId
                         where c.Status == ( int ) Status.Active
                         select c.ClientId ).FirstOrDefault();


            return clientId;
        }



        public List<Site> GetSitesByClientsIncluded( int clientId, int siteId )
        {
            List<Site> siteList;
            siteList = ( from c in context.ClientSites
                         join e in context.Sites
                         on c.SiteId equals e.Id
                         where c.ClientId == clientId
                         where e.SiteId == siteId
                         select e ).ToList();


            return siteList;
        }
        public List<Site> GetSitesByClientIncluded( int clientId )
        {
            List<Site> siteList;
            siteList = ( from s in context.Sites
                         join e in context.ClientSites
                         on s.Id equals e.SiteId
                         where e.ClientId == clientId
                         select s ).ToList();
            return siteList;
        }


        public List<Site> GetSitesByClientsExcluded( int clientId, int siteId )
        {
            List<Site> siteList;
            List<int> exclList = new List<int>();
            exclList.Add( siteId );

            siteList = ( from c in context.ClientSites
                         join e in context.Sites
                         on c.SiteId equals e.Id
                         where c.ClientId == clientId
                         where e.SiteId == null
                         where !exclList.Contains( c.SiteId )
                         select e ).ToList();

            return siteList;
        }

        public int CountSubSitesBySite( int siteId )
        {

            //return 0;
            //me_employees.Count(me => me.me_login_name == this.me_login_name && me.me_pkey != this.me_pkey);
            //return context.Sites.Count(s => s.SiteId == siteId && s.Status = (int)Status.Active);
            int siteCount = ( from c in context.Sites
                              where c.SiteId == siteId
                              where c.Status == ( int ) Status.Active
                              select c ).Count();

            return siteCount;
        }


        public bool ExistByAccountCode( string accCode )
        {
            return context.Sites.Any( c => c.AccountCode == accCode );
        }

        public bool ExistByXYCoords( string xcoord, string ycoord )
        {
            bool bExists = false;
            bExists = context.Sites.Any( c => c.XCord == xcoord && c.YCord == ycoord );
            return bExists;
        }
    }
}
