using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using ACT.Data.Models;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Data.Entity;
using System.Data.SqlClient;

namespace ACT.Core.Services
{
    public class AuditLogService : BaseService<AuditLog>, IDisposable
    {
        private ACTEntities _context = new ACTEntities();

        public AuditLogService()
        {
        }

        /// <summary>
        /// Gets a total count of audits matching the search filters
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public override int Total( PagingModel pm, CustomSearchModel csm )
        {
            string[] qs = ( csm.Query ?? "" ).Split( ' ' );

            List<int> pspIds = CurrentUser.PSPs.Select( s => s.Id ).ToList();
            List<int> clientIds = CurrentUser.Clients.Select( s => s.Id ).ToList();

            return ( from a in context.AuditLogs
                     join user in context.Users on a.UserId equals user.Id into temp
                     from u in temp.DefaultIfEmpty()

                         #region WHERE

                     where
                     (
                         // WHERE
                         (
                            ( CurrentUser.RoleType == RoleType.PSP ? context.PSPUsers.Any( pu => pu.UserId == a.UserId && pspIds.Contains( pu.PSPId ) ) : true ) &&
                            ( CurrentUser.RoleType == RoleType.Client ? context.ClientUsers.Any( cu => cu.UserId == a.UserId && clientIds.Contains( cu.ClientId ) ) : true )
                         ) &&

                         // CUSTOM SEARCH
                         ( ( csm.UserId != 0 ) ? a.UserId == csm.UserId : true ) &&
                         ( ( csm.ObjectId != 0 ) ? a.ObjectId == csm.ObjectId : true ) &&
                         ( ( !string.IsNullOrEmpty( csm.TableName ) ) ? a.ActionTable == csm.TableName : true ) &&
                         ( ( !string.IsNullOrEmpty( csm.ControllerName ) ) ? a.Controller == csm.ControllerName : true ) &&
                         ( ( csm.FromDate.HasValue ) ? DbFunctions.TruncateTime( a.CreatedOn ) >= DbFunctions.TruncateTime( csm.FromDate ) : true ) &&
                         ( ( csm.ToDate.HasValue ) ? DbFunctions.TruncateTime( a.CreatedOn ) <= DbFunctions.TruncateTime( csm.ToDate ) : true ) &&
                         ( ( csm.ActivityType != ActivityTypes.All ) ? a.Type == ( int ) csm.ActivityType : true ) &&

                         // NORMAL QUERY
                         (
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( a.Parameters ) || qs.All( q => a.Parameters.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( a.BeforeImage ) || qs.All( q => a.BeforeImage.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( a.AfterImage ) || qs.All( q => a.AfterImage.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( a.Action ) || qs.All( q => a.Action.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( a.Comments ) || qs.All( q => a.Comments.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( a.Controller ) || qs.All( q => a.Controller.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( a.ActionTable ) || qs.All( q => a.ActionTable.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Name ) || qs.All( q => u.Name.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Surname ) || qs.All( q => u.Surname.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                             ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Email ) || qs.All( q => u.Email.ToLower().Contains( q.ToLower() ) ) ) : true )
                         )
                       )

                     #endregion

                     select a ).Count();
        }

        /// <summary>
        /// Gets a list of Audit Logs matching the search filters
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public new List<AuditLogCustomModel> List( PagingModel pm, CustomSearchModel csm )
        {
            string[] qs = ( csm.Query ?? "" ).Split( ' ' );

            List<int> pspIds = CurrentUser.PSPs.Select( s => s.Id ).ToList();
            List<int> clientIds = CurrentUser.Clients.Select( s => s.Id ).ToList();

            List<AuditLogCustomModel> list = ( from a in context.AuditLogs
                                               join user in context.Users on a.UserId equals user.Id into temp
                                               from u in temp.DefaultIfEmpty()

                                                   #region WHERE

                                               where
                                               (
                                                   // WHERE
                                                   (
                                                       ( CurrentUser.RoleType == RoleType.PSP ? context.PSPUsers.Any( pu => pu.UserId == a.UserId && pspIds.Contains( pu.PSPId ) ) : true ) &&
                                                       ( CurrentUser.RoleType == RoleType.Client ? context.ClientUsers.Any( cu => cu.UserId == a.UserId && clientIds.Contains( cu.ClientId ) ) : true )
                                                   ) &&

                                                   // CUSTOM SEARCH

                                                   ( ( csm.UserId != 0 ) ? a.UserId == csm.UserId : true ) &&
                                                   ( ( csm.ObjectId != 0 ) ? a.ObjectId == csm.ObjectId : true ) &&
                                                   ( ( !string.IsNullOrEmpty( csm.TableName ) ) ? a.ActionTable == csm.TableName : true ) &&
                                                   ( ( !string.IsNullOrEmpty( csm.ControllerName ) ) ? a.Controller == csm.ControllerName : true ) &&
                                                   ( ( csm.FromDate.HasValue ) ? DbFunctions.TruncateTime( a.CreatedOn ) >= DbFunctions.TruncateTime( csm.FromDate ) : true ) &&
                                                   ( ( csm.ToDate.HasValue ) ? DbFunctions.TruncateTime( a.CreatedOn ) <= DbFunctions.TruncateTime( csm.ToDate ) : true ) &&
                                                   ( ( csm.ActivityType != ActivityTypes.All ) ? a.Type == ( int ) csm.ActivityType : true ) &&

                                                   // NORMAL QUERY

                                                   (
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( a.Parameters ) || qs.All( q => a.Parameters.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( a.BeforeImage ) || qs.All( q => a.BeforeImage.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( a.AfterImage ) || qs.All( q => a.AfterImage.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( a.Action ) || qs.All( q => a.Action.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( a.Comments ) || qs.All( q => a.Comments.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( a.Controller ) || qs.All( q => a.Controller.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( a.ActionTable ) || qs.All( q => a.ActionTable.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Name ) || qs.All( q => u.Name.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Surname ) || qs.All( q => u.Surname.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                                                       ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Email ) || qs.All( q => u.Email.ToLower().Contains( q.ToLower() ) ) ) : true )
                                                   )
                                               )

                                               #endregion

                                               select new AuditLogCustomModel
                                               {
                                                   #region Properties

                                                   Id = a.Id,
                                                   Type = a.Type,
                                                   UserId = a.UserId,
                                                   Action = a.Action,
                                                   ObjectId = a.ObjectId,
                                                   Comments = a.Comments,
                                                   CreatedOn = a.CreatedOn,
                                                   Controller = a.Controller,
                                                   ModifiedOn = a.ModifiedOn,
                                                   Parameters = a.Parameters,
                                                   ActionTable = a.ActionTable,
                                                   IsAjaxRequest = a.IsAjaxRequest,

                                                   User = u

                                                   #endregion
                                               } ).OrderBy( CreateOrderBy( pm.SortBy, pm.Sort ) )
                                            .Skip( pm.Skip )
                                            .Take( pm.Take )
                                            .ToList();

            return list;
        }

        /// <summary>
        /// Gets a total count of Client Load Audit Log Per User matching the search criteria
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public int ClientLoadAuditLogPerUserTotal( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmUserId", csm.UserId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmVehicleId", csm.VehicleId ) },
                { new SqlParameter( "csmPODCommentId", csm.PODCommentId ) },
                { new SqlParameter( "csmTransporterId", csm.TransporterId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            // Query

            #region Query

            string query = @"SELECT
                                COUNT(al.[Id]) AS [Total]
                            FROM
	                            [dbo].[AuditLog] al
	                            LEFT OUTER JOIN [dbo].[User] u1 ON u1.[Id]=al.[UserId]
	                            LEFT OUTER JOIN [dbo].[UserRole] ur ON ur.[UserId]=u1.[Id]
	                            LEFT OUTER JOIN [dbo].[Role] r ON r.[Id]=ur.[RoleId]
	                            LEFT OUTER JOIN [dbo].[ClientLoad] cl ON cl.[Id]=al.[ObjectId]
                                LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]
	                            LEFT OUTER JOIN [dbo].[ExtendedClientLoad] ecl ON ecl.[ClientLoadId]=cl.[Id]
                                LEFT OUTER JOIN [dbo].[Vehicle] v ON v.[Id]=cl.[VehicleId]
                                LEFT OUTER JOIN [dbo].[Transporter] t ON t.[Id]=cl.[TransporterId]
                                LEFT OUTER JOIN [dbo].[PODComment] pc ON pc.[Id]=cl.[PODCommentId]
                                LEFT OUTER JOIN [dbo].[ClientSite] cs1 ON cs1.[Id]=cl.[ClientSiteId]
                                LEFT OUTER JOIN [dbo].[ClientSite] cs2 ON cs2.[Id]=cl.[ToClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s1 ON s1.[Id]=cs1.[SiteId]
                                LEFT OUTER JOIN [dbo].[Site] s2 ON s2.[Id]=cs2.[SiteId]
                                LEFT OUTER JOIN [dbo].[Region] r1 ON r1.[Id]=s1.[RegionId]
                                LEFT OUTER JOIN [dbo].[Region] r2 ON r2.[Id]=s2.[RegionId]
                                LEFT OUTER JOIN [dbo].[ClientAuthorisation] ca ON ca.[ClientLoadId]=cl.[Id]
	                            LEFT OUTER JOIN [dbo].[User] u2 ON u2.[Id]=ca.[UserId]";

            #endregion

            // WHERE

            #region WHERE

            query = $"{query} WHERE (al.[ActionTable]='ClientLoad')";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.UserId > 0 )
            {
                query = $"{query} AND (u1.[Id]=@csmUserId)";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.[Id]=@csmClientId)";
            }
            if ( csm.VehicleId > 0 )
            {
                query = $"{query} AND (v.[Id]=@csmVehicleId)";
            }
            if ( csm.TransporterId > 0 )
            {
                query = $"{query} AND (t.[Id]=@csmTransporterId)";
            }
            if ( csm.PODCommentId > 0 )
            {
                query = $"{query} AND (pc.[Id]=@csmPODCommentId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.[PODCommentDate] >= @csmFromDate AND cl.[PODCommentDate] <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.[PODCommentDate]>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.[PODCommentDate]<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (pc.[Comment] LIKE '%{1}%' OR
                                                  t.[Name] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  ca.[Code] LIKE '%{1}%' OR
                                                  s1.[Name] LIKE '%{1}%' OR
                                                  s2.[Name] LIKE '%{1}%' OR
                                                  r1.[Code] LIKE '%{1}%' OR
                                                  r2.[Code] LIKE '%{1}%' OR
                                                  r1.[Description] LIKE '%{1}%' OR
                                                  r2.[Description] LIKE '%{1}%' OR
                                                  v.[FleetNumber] LIKE '%{1}%' OR
                                                  v.[Registration] LIKE '%{1}%' OR
                                                  cs1.[ClientCustomerNumber] LIKE '%{1}%' OR
                                                  t.[ClientTransporterCode] LIKE '%{1}%' OR
                                                  cs1.[AccountingCode] LIKE '%{1}%' OR
                                                  u1.[Name] LIKE '%{1}%' OR
                                                  u2.[Name] LIKE '%{1}%' OR
                                                  cl.[LoadNumber] LIKE '%{1}%' OR
                                                  cl.[DeliveryNote] LIKE '%{1}%' OR
                                                  cl.[ReceiverNumber] LIKE '%{1}%' OR
                                                  cl.[PODNumber] LIKE '%{1}%' OR
                                                  cl.[PCNNumber] LIKE '%{1}%' OR
                                                  cl.[THAN] LIKE '%{1}%' OR
                                                  cl.[DebriefDocketNo] LIKE '%{1}%' OR
                                                  ecl.[DocketNumber] LIKE '%{1}%' OR
                                                  ecl.[OriginalDocketNumber] LIKE '%{1}%' OR
                                                  ecl.[LocationId] LIKE '%{1}%' OR
                                                  ecl.[OtherPartyId] LIKE '%{1}%' OR
                                                  ecl.[EquipmentCode] LIKE '%{1}%' OR
                                                  ecl.[Ref] LIKE '%{1}%' OR
                                                  ecl.[OtherRef] LIKE '%{1}%' OR
                                                  ecl.[PalletReturnSlipNo] LIKE '%{1}%' OR
                                                  ecl.[ChepCustomerThanDocNo] LIKE '%{1}%' OR
                                                  ecl.[WarehouseTransferDocNo] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Client Load Audit Log Per User matching the search criteria
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<ClientLoadAuditLogModel> ListClientLoadAuditLogPerUser( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmUserId", csm.UserId ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmVehicleId", csm.VehicleId ) },
                { new SqlParameter( "csmPODCommentId", csm.PODCommentId ) },
                { new SqlParameter( "csmTransporterId", csm.TransporterId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            // Query

            #region Query

            string query = @"SELECT
                                s2.[Id] AS [ToSiteId],
	                            al.[Id] AS [AuditLogId],
                                s1.[Id] AS [FromSiteId],
                                r2.[Code] AS [ToRegionCode],
                                r.[Name] AS [AuditUserRole],
                                pc.[Comment] AS [PODComment],
                                t.[Name] AS [TransporterName],
                                r1.[Code] AS [FromRegionCode],
                                c.[CompanyName] AS [ClientName],
	                            ca.[Code] AS [AuthorizationCode],
                                s2.[Description] AS [ToSiteName],
                                s1.[Description] AS [FromSiteName],
                                r2.[Description] AS [ToRegionName],
                                r1.[Description] AS [FromRegionName],
	                            al.[CreatedOn] AS [AuditLogCreatedOn],
                                v.[FleetNumber] AS [VehicleFleetNumber],
                                v.[Registration] AS [VehicleRegistration],
	                            cs1.[ClientCustomerNumber] AS [DebtorsCode],
	                            ca.[AuthorisationDate] AS [AuthorizationDate],
	                            t.ClientTransporterCode AS [TransporterNumber],
	                            cs1.[AccountingCode] AS [CustomerAccountNumber],
                                u1.[Name] + ' ' + u1.[Surname] AS [AuditUserName],
                                u2.[Name] + ' ' + u2.[Surname] AS [AuthorizerName],
	                            al.[AfterImage] AS [ClientLoadAfterImage],
	                            (SELECT al1.[AfterImage] FROM [dbo].[AuditLog] al1 WHERE al1.[ActionTable]='ExtendedClientLoad' AND al1.[ObjectId]=ecl.[Id] AND FORMAT(al.[CreatedOn], 'yyyyMMddHHmmss')=FORMAT(al1.[CreatedOn], 'yyyyMMddHHmmss')) AS [ExtendedClientLoadAfterImage],
	                            al.[CreatedOn] AS [ClientLoadAuditDateTime],
	                            (SELECT al1.[CreatedOn] FROM [dbo].[AuditLog] al1 WHERE al1.[ActionTable]='ExtendedClientLoad' AND al1.[ObjectId]=ecl.[Id] AND FORMAT(al.[CreatedOn], 'yyyyMMddHHmmss')=FORMAT(al1.[CreatedOn], 'yyyyMMddHHmmss')) AS [ExtendedClientLoadAuditDateTime]
                            FROM
	                            [dbo].[AuditLog] al
	                            LEFT OUTER JOIN [dbo].[User] u1 ON u1.[Id]=al.[UserId]
	                            LEFT OUTER JOIN [dbo].[UserRole] ur ON ur.[UserId]=u1.[Id]
	                            LEFT OUTER JOIN [dbo].[Role] r ON r.[Id]=ur.[RoleId]
	                            LEFT OUTER JOIN [dbo].[ClientLoad] cl ON cl.[Id]=al.[ObjectId]
                                LEFT OUTER JOIN [dbo].[Client] c ON c.[Id]=cl.[ClientId]
	                            LEFT OUTER JOIN [dbo].[ExtendedClientLoad] ecl ON ecl.[ClientLoadId]=cl.[Id]
                                LEFT OUTER JOIN [dbo].[Vehicle] v ON v.[Id]=cl.[VehicleId]
                                LEFT OUTER JOIN [dbo].[Transporter] t ON t.[Id]=cl.[TransporterId]
                                LEFT OUTER JOIN [dbo].[PODComment] pc ON pc.[Id]=cl.[PODCommentId]
                                LEFT OUTER JOIN [dbo].[ClientSite] cs1 ON cs1.[Id]=cl.[ClientSiteId]
                                LEFT OUTER JOIN [dbo].[ClientSite] cs2 ON cs2.[Id]=cl.[ToClientSiteId]
                                LEFT OUTER JOIN [dbo].[Site] s1 ON s1.[Id]=cs1.[SiteId]
                                LEFT OUTER JOIN [dbo].[Site] s2 ON s2.[Id]=cs2.[SiteId]
                                LEFT OUTER JOIN [dbo].[Region] r1 ON r1.[Id]=s1.[RegionId]
                                LEFT OUTER JOIN [dbo].[Region] r2 ON r2.[Id]=s2.[RegionId]
                                LEFT OUTER JOIN [dbo].[ClientAuthorisation] ca ON ca.[ClientLoadId]=cl.[Id]
	                            LEFT OUTER JOIN [dbo].[User] u2 ON u2.[Id]=ca.[UserId]";

            #endregion

            // WHERE

            #region WHERE

            query = $"{query} WHERE (al.[ActionTable]='ClientLoad')";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=cl.[ClientId] AND pu.[UserId]=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.[ClientId]=cl.[ClientId] AND cu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.UserId > 0 )
            {
                query = $"{query} AND (u1.[Id]=@csmUserId)";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.[Id]=@csmClientId)";
            }
            if ( csm.VehicleId > 0 )
            {
                query = $"{query} AND (v.[Id]=@csmVehicleId)";
            }
            if ( csm.TransporterId > 0 )
            {
                query = $"{query} AND (t.[Id]=@csmTransporterId)";
            }
            if ( csm.PODCommentId > 0 )
            {
                query = $"{query} AND (pc.[Id]=@csmPODCommentId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cl.[PODCommentDate] >= @csmFromDate AND cl.[PODCommentDate] <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cl.[PODCommentDate]>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cl.[PODCommentDate]<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (pc.[Comment] LIKE '%{1}%' OR
                                                  t.[Name] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  ca.[Code] LIKE '%{1}%' OR
                                                  s1.[Name] LIKE '%{1}%' OR
                                                  s2.[Name] LIKE '%{1}%' OR
                                                  r1.[Code] LIKE '%{1}%' OR
                                                  r2.[Code] LIKE '%{1}%' OR
                                                  r1.[Description] LIKE '%{1}%' OR
                                                  r2.[Description] LIKE '%{1}%' OR
                                                  v.[FleetNumber] LIKE '%{1}%' OR
                                                  v.[Registration] LIKE '%{1}%' OR
                                                  cs1.[ClientCustomerNumber] LIKE '%{1}%' OR
                                                  t.[ClientTransporterCode] LIKE '%{1}%' OR
                                                  cs1.[AccountingCode] LIKE '%{1}%' OR
                                                  u1.[Name] LIKE '%{1}%' OR
                                                  u2.[Name] LIKE '%{1}%' OR
                                                  cl.[LoadNumber] LIKE '%{1}%' OR
                                                  cl.[DeliveryNote] LIKE '%{1}%' OR
                                                  cl.[ReceiverNumber] LIKE '%{1}%' OR
                                                  cl.[PODNumber] LIKE '%{1}%' OR
                                                  cl.[PCNNumber] LIKE '%{1}%' OR
                                                  cl.[THAN] LIKE '%{1}%' OR
                                                  cl.[DebriefDocketNo] LIKE '%{1}%' OR
                                                  ecl.[DocketNumber] LIKE '%{1}%' OR
                                                  ecl.[OriginalDocketNumber] LIKE '%{1}%' OR
                                                  ecl.[LocationId] LIKE '%{1}%' OR
                                                  ecl.[OtherPartyId] LIKE '%{1}%' OR
                                                  ecl.[EquipmentCode] LIKE '%{1}%' OR
                                                  ecl.[Ref] LIKE '%{1}%' OR
                                                  ecl.[OtherRef] LIKE '%{1}%' OR
                                                  ecl.[PalletReturnSlipNo] LIKE '%{1}%' OR
                                                  ecl.[ChepCustomerThanDocNo] LIKE '%{1}%' OR
                                                  ecl.[WarehouseTransferDocNo] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = $"{query} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ";

            List<ClientLoadAuditLogModel> model = context.Database.SqlQuery<ClientLoadAuditLogModel>( query, parameters.ToArray() ).ToList();

            if ( model.NullableAny() )
            {
                foreach ( ClientLoadAuditLogModel item in model )
                {
                    try
                    {
                        item.ClientLoad = !string.IsNullOrEmpty( item.ClientLoadAfterImage ) ? new JavaScriptSerializer().Deserialize<ClientLoadCustomModel>( item.ClientLoadAfterImage ) : new ClientLoadCustomModel();
                    }
                    catch ( Exception ex )
                    {
                        item.ClientLoad = new ClientLoadCustomModel();
                    }

                    try
                    {
                        item.ExtendedClientLoad = !string.IsNullOrEmpty( item.ExtendedClientLoadAfterImage ) ? new JavaScriptSerializer().Deserialize<ExtendedClientLoadCustomModel>( item.ExtendedClientLoadAfterImage ) : new ExtendedClientLoadCustomModel();
                    }
                    catch ( Exception ex )
                    {
                        item.ExtendedClientLoad = new ExtendedClientLoadCustomModel();
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Gets a list of audit logs using the specified action table
        /// </summary>
        /// <param name="actionTable"></param>
        /// <returns></returns>
        public List<AuditLog> ListByActionTable( string actionTable )
        {
            return context.AuditLogs.Where( a => a.ActionTable == actionTable )
                                    .OrderByDescending( o => o.CreatedOn )
                                    .ToList();
        }

        /// <summary>
        /// Gets an activity using the specified id
        /// </summary>
        /// <param name="id">Id of the activity to be fetched</param>
        /// <returns></returns>
        public new AuditLogCustomModel GetById( int id )
        {
            return ( from a in _context.AuditLogs
                     join user in _context.Users on a.UserId equals user.Id into temp
                     from u in temp.DefaultIfEmpty()

                     where
                     (
                         a.Id == id
                     )

                     select new AuditLogCustomModel
                     {
                         #region Properties

                         Id = a.Id,
                         Type = a.Type,
                         UserId = a.UserId,
                         Action = a.Action,
                         Browser = a.Browser,
                         ObjectId = a.ObjectId,
                         Comments = a.Comments,
                         CreatedOn = a.CreatedOn,
                         Controller = a.Controller,
                         AfterImage = a.AfterImage,
                         ModifiedBy = a.ModifiedBy,
                         ModifiedOn = a.ModifiedOn,
                         Parameters = a.Parameters,
                         BeforeImage = a.BeforeImage,
                         ActionTable = a.ActionTable,
                         IsAjaxRequest = a.IsAjaxRequest,

                         User = u

                         #endregion
                     } ).FirstOrDefault();
        }

        /// <summary>
        /// Creates a new audit log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activity"></param>
        /// <param name="newItem"></param>
        /// <param name="oldItem"></param>
        public bool Create<T>( ActivityTypes activity, T newItem, T oldItem = null, int userId = 0 ) where T : class
        {
            try
            {
                dynamic oldObj = new ExpandoObject(),
                        newObj = new ExpandoObject();

                var oldDic = oldObj as IDictionary<string, object>;
                var newDic = newObj as IDictionary<string, object>;

                if ( oldItem != null )
                {
                    var oldprops = oldItem.GetType().GetProperties();

                    foreach ( var item in oldprops )
                    {
                        if (
                            ( item.PropertyType != null ) &&
                            ( item.PropertyType == typeof( string ) ||
                              item.PropertyType == typeof( int ) ||
                              item.PropertyType == typeof( int? ) ||
                              item.PropertyType == typeof( decimal ) ||
                              item.PropertyType == typeof( decimal? ) ||
                              item.PropertyType == typeof( DateTime ) ||
                              item.PropertyType == typeof( DateTime? ) ||
                              item.PropertyType == typeof( double ) ||
                              item.PropertyType == typeof( double? ) ||
                              item.PropertyType == typeof( TimeSpan ) ||
                              item.PropertyType == typeof( TimeSpan? ) ||
                              item.PropertyType == typeof( bool ) ||
                              item.PropertyType == typeof( bool? ) ||
                              item.PropertyType == typeof( byte ) ||
                              item.PropertyType == typeof( byte? ) )
                           )
                        {
                            oldDic[ item.Name ] = item.GetValue( oldItem );
                        }
                    }
                }

                var props = newItem.GetType().GetProperties();

                foreach ( var item in props )
                {
                    if (
                        ( item.PropertyType != null ) &&
                        ( item.PropertyType == typeof( string ) ||
                            item.PropertyType == typeof( int ) ||
                            item.PropertyType == typeof( int? ) ||
                            item.PropertyType == typeof( decimal ) ||
                            item.PropertyType == typeof( decimal? ) ||
                            item.PropertyType == typeof( DateTime ) ||
                            item.PropertyType == typeof( DateTime? ) ||
                            item.PropertyType == typeof( double ) ||
                            item.PropertyType == typeof( double? ) ||
                            item.PropertyType == typeof( TimeSpan ) ||
                            item.PropertyType == typeof( TimeSpan? ) ||
                            item.PropertyType == typeof( bool ) ||
                            item.PropertyType == typeof( bool? ) ||
                            item.PropertyType == typeof( byte ) ||
                            item.PropertyType == typeof( byte? ) )
                      )
                    {
                        newDic[ item.Name ] = item.GetValue( newItem );
                    }
                }

                string actionTable = newItem.GetType().BaseType.Name;

                if ( actionTable.ToLower() == "object" )
                {
                    actionTable = newItem.GetType().Name;
                }

                string before = ( oldItem != null ) ? Newtonsoft.Json.JsonConvert.SerializeObject( oldObj ) : string.Empty;
                string after = ( newObj != null ) ? Newtonsoft.Json.JsonConvert.SerializeObject( newObj ) : string.Empty;

                //if ( before == after ) return false;

                HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;

                string b = string.Format( "Type={1} {0} Name={2} {0} Version={3} {0} Platform={4} {0} Supports JavaScript={5}", Environment.NewLine,
                                           browser.Type, browser.Browser, browser.Version, browser.Platform, browser.EcmaScriptVersion.ToString() );

                if ( userId == 0 )
                {
                    userId = ( CurrentUser?.Id > 0 ) ? CurrentUser.Id : 0;
                }

                AuditLog log = new AuditLog()
                {
                    Browser = b,
                    UserId = userId,
                    AfterImage = after,
                    BeforeImage = before,
                    Type = ( int ) activity,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ActionTable = actionTable,
                    Comments = string.Format( "Created/Updated a {0}", actionTable ),
                    ModifiedBy = ( ( !string.IsNullOrEmpty( CurrentUser?.Email ) ) ? CurrentUser.Email : "System" ),
                    IsAjaxRequest = ( HttpContext.Current.Request.Headers[ "X-Requested-With" ] == "XMLHttpRequest" ),
                    Action = ( HttpContext.Current.Request.RequestContext.RouteData.Values[ "action" ] as string ) ?? string.Empty,
                    Controller = ( HttpContext.Current.Request.RequestContext.RouteData.Values[ "controller" ] as string ) ?? string.Empty,
                    ObjectId = ( int ) newItem.GetType().GetProperties().FirstOrDefault( x => x.Name == "Id" ).GetValue( newItem ),
                    Parameters = string.Empty //new JavaScriptSerializer().Serialize( HttpContext.Current.Request.RequestContext.RouteData.Values )
                };

                _context.AuditLogs.Add( log );
                _context.SaveChanges();
            }
            catch ( Exception ex )
            {

            }

            return true;
        }
    }
}
