using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;
using System.Data.SqlClient;
using ACT.Core.Models.Simple;

namespace ACT.Core.Services
{
    public class UserService : BaseService<User>, IDisposable
    {
        public UserService()
        {

        }

        /// <summary>
        /// Gets a user using the specified id
        /// </summary>
        /// <param name="id">Id of the user to be fetched</param>
        /// <returns></returns>
        public override User GetById( int id )
        {
            OldObject = context.Users.AsNoTracking().FirstOrDefault( u => u.Id == id );

            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            User user = context.Users.FirstOrDefault( u => u.Id == id );

            return user;
        }

        /// <summary>
        /// Gets a user using the specified Email Address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetByEmail( string email )
        {
            return context.Users.FirstOrDefault( u => u.Email == email );
        }

        /// <summary>
        /// Gets a user using the specified id
        /// </summary>
        /// <param name="id">Id of the user to be fetched</param>
        /// <returns></returns>
        public SimpleUserModel GetSimpleById( int id )
        {
            SimpleUserModel user = ( SimpleUserModel ) ContextExtensions.GetCachedUserData( "simpu_" + id );

            if ( user != null )
            {
                return user;
            }

            // Parameters
            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "uid", id ) }
            };

            string query = "SELECT u.Id, u.Name, u.Surname, u.Email, u.IdNumber, u.Branch, u.Name + ' ' + u.Surname AS DisplayName FROM [dbo].[Users] u WHERE u.Id=@uid";

            user = context.Database.SqlQuery<SimpleUserModel>( query.Trim(), parameters.ToArray() ).FirstOrDefault();

            ContextExtensions.CacheUserData( "simpu_" + id, user );

            return user;
        }

        /// <summary>
        /// Gets a total list of users using the provided filters
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public override int Total( PagingModel pm, CustomSearchModel csm )
        {
            string[] qs = ( csm.Query ?? "" ).Split( ' ' );

            return ( from u in context.Users

                         #region Where

                     where
                     (
                         // Where
                         ( CurrentUser.RoleType == RoleType.PSP ? u.PSPUsers.Any( pu => CurrentUser.PSPs.Any( p => p.Id == pu.PSPId ) ) : true ) &&
                         ( CurrentUser.RoleType == RoleType.Client ? u.ClientUsers.Any( cu => CurrentUser.PSPs.Any( c => c.Id == cu.ClientId ) ) : true ) &&



                         // Custom Search
                         (
                            ( ( csm.Status != Status.All ) ? u.Status == ( int ) csm.Status : true ) &&
                            ( ( csm.RoleType != RoleType.All ) ? u.UserRoles.Any( ur => ur.Role.Type == ( int ) csm.RoleType ) : true )
                         ) &&



                         // Normal Search
                         (
                            ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Name ) || qs.All( q => u.Name.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                            ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Cell ) || qs.All( q => u.Cell.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                            ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Email ) || qs.All( q => u.Email.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                            ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Surname ) || qs.All( q => u.Surname.ToLower().Contains( q.ToLower() ) ) ) : true )
                        )
                     )

                     #endregion

                     select u ).Count();
        }

        /// <summary>
        /// Gets a list of users using the provided filters
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public override List<User> List( PagingModel pm, CustomSearchModel csm )
        {
            string[] qs = ( csm.Query ?? "" ).Split( ' ' );

            base.context.Configuration.LazyLoadingEnabled = true;
            base.context.Configuration.ProxyCreationEnabled = true;

            return ( from u in context.Users

                    #region Where

                     where
                     (
                         // Where
                         ( CurrentUser.RoleType == RoleType.PSP ? u.PSPUsers.Any( pu => CurrentUser.PSPs.Any( p => p.Id == pu.PSPId ) ) : true ) &&
                         ( CurrentUser.RoleType == RoleType.Client ? u.ClientUsers.Any( cu => CurrentUser.PSPs.Any( c => c.Id == cu.ClientId ) ) : true ) &&



                         // Custom Search
                         (
                            ( ( csm.Status != Status.All ) ? u.Status == ( int ) csm.Status : true ) &&
                            ( ( csm.RoleType != RoleType.All ) ? u.UserRoles.Any( ur => ur.Role.Type == ( int ) csm.RoleType ) : true )
                         ) &&



                         // Normal Search
                         (
                            ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Name ) || qs.All( q => u.Name.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                            ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Cell ) || qs.All( q => u.Cell.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                            ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Email ) || qs.All( q => u.Email.ToLower().Contains( q.ToLower() ) ) ) : true ) ||
                            ( qs.Any( q => q != "" ) ? ( qs.Contains( u.Surname ) || qs.All( q => u.Surname.ToLower().Contains( q.ToLower() ) ) ) : true )
                        )
                     )

                     #endregion
                     select u ).OrderBy( CreateOrderBy( pm.SortBy, pm.Sort ) )
                               .Skip( pm.Skip )
                               .Take( pm.Take )
                               .ToList();
        }

        /// <summary>
        /// Gets a simple list of users, usually useful in a custom search
        /// </summary>
        /// <param name="simple"></param>
        /// <returns></returns>
        public List<SimpleUserModel> List( bool simple = true )
        {
            return ( from u in context.Users
                     where
                     (
                        u.Status == ( int ) Status.Active
                     )
                     select new SimpleUserModel
                     {
                         Id = u.Id,
                         Name = u.Name,
                         Email = u.Email,
                         Surname = u.Surname,
                         DisplayName = u.Name + " " + u.Surname,
                         Status = ( Status ) u.Status
                     } ).ToList();
        }

        /// <summary>
        /// Gets a simple list of users, usually useful in a custom search
        /// </summary>
        /// <param name="simple"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool simple = true, RoleType roleType = RoleType.All )
        {
            Dictionary<int, string> userOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>();

            string query = string.Empty;

            query = $"SELECT u.Id AS [TKey], u.Name + ' ' + u.Surname AS [TValue] FROM [dbo].[User] u WHERE u.Status={( int ) Status.Active}";

            if ( roleType != RoleType.All )
            {
                query = $" {query} AND u.Type={( int ) roleType} ";
            }

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( userOptions.Keys.Any( x => x == k.TKey ) )
                        continue;

                    userOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return userOptions;
        }

        /// <summary>
        /// Gets a list of leads record for the specified params
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

            CountModel count;

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmRoleType", ( int ) csm.RoleType ) },
                { new SqlParameter( "csmProvince", ( int ) csm.Province ) }
            };

            #endregion

            string query = string.Empty;

            query = string.Format( @"SELECT
	                                   COUNT(1) AS [Total]
                                     FROM 
	                                    [dbo].[User] u", query );

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1) ";

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.RoleType != RoleType.All )
            {
                query = $"{query} AND (u.Type=@csmRoleType) ";
            }
            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (u.Status=@csmAgentStatus) ";
            }
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (u.CreatedOn >= @csmFromDate AND u.CreatedOn <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (u.CreatedOn>=@csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (u.CreatedOn<=@csmToDate) ";
                }
            }
            if ( csm.Province != Province.All )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[Address] a WHERE a.ObjectId=u.Id AND a.ObjectType='User' AND a.Province=@csmProvince) ";
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (u.Name LIKE '%{1}%' OR
                                                  u.Surname LIKE '%{1}%' OR
                                                  u.IdNumber LIKE '%{1}%' OR
                                                  u.TaxNumber LIKE '%{1}%' OR
                                                  u.Email LIKE '%{1}%' OR
                                                  u.Cell LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            count = context.Database.SqlQuery<CountModel>( query.Trim(), parameters.ToArray() ).FirstOrDefault();

            return count.Total;
        }

        /// <summary>
        /// Checks if a user with the specified email already exists?
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ExistByEmail( string email )
        {
            return context.Users.Any( u => u.Email == email && u.Status == ( int ) Status.Active );
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">An instance of a user to be created</param>
        /// <returns></returns>
        public User Create( User user, int roleId )
        {
            #region Role

            if ( roleId != 0 )
            {
                // Add the new selected
                UserRole role = new UserRole()
                {
                    RoleId = roleId,
                    UserId = user.Id,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    Status = ( int ) Status.Active,
                    ModifiedBy = ( ( CurrentUser != null && CurrentUser.Email != null ) ? CurrentUser.Email : "System" )
                };

                user.UserRoles.Add( role );
            }

            #endregion

            return base.Create( user );
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="user">An instance of a user to be updated</param>
        /// <returns></returns>
        public User Update( User user, int roleId = 0 )
        {
            #region Role

            if ( roleId != 0 && !user.UserRoles.Any( ur => ur.RoleId == roleId ) )
            {
                if ( user.UserRoles != null && user.UserRoles.Any() && !user.UserRoles.Any( ur => ur.RoleId == roleId ) )
                {
                    // Quickly Remove current role (s)
                    string query = string.Format( "DELETE FROM [UserRole] WHERE [Id] IN ({0});", string.Join( ",", user.UserRoles.Select( ur => ur.Id ) ) );

                    bool success = Query( query );
                }

                // Add the new selected
                UserRole role = new UserRole()
                {
                    RoleId = roleId,
                    UserId = user.Id,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    Status = ( int ) Status.Active,
                    ModifiedBy = ( ( CurrentUser != null && CurrentUser.Email != null ) ? CurrentUser.Email : "System" )
                };

                user.UserRoles = user.UserRoles ?? new List<UserRole>();

                user.UserRoles.Add( role );
            }

            #endregion

            return base.Update( user );
        }
    }
}
