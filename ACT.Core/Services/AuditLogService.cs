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
