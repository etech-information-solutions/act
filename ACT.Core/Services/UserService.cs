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
using ACT.Core.Models.Custom;

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
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "csmRoleType", ( int ) csm.RoleType ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = string.Empty;

            query = string.Format( @"SELECT
	                                   COUNT(1) AS [Total]
                                     FROM 
	                                    [dbo].[User] u", query );

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE u.Id=pu.UserId AND pu.UserId=@userid) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE u.Id=cu.UserId AND cu.UserId=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (u.Status=@csmStatus) ";
            }
            if ( csm.RoleType != RoleType.All )
            {
                query = $"{query} AND (u.Type=@csmRoleType) ";
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

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (u.[Name] LIKE '%{1}%' OR
                                                  u.[Surname] LIKE '%{1}%' OR
                                                  u.[Email] LIKE '%{1}%' OR
                                                  u.[Cell] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            count = context.Database.SqlQuery<CountModel>( query.Trim(), parameters.ToArray() ).FirstOrDefault();

            return count.Total;
        }

        /// <summary>
        /// Gets a list of users using the provided filters
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<UserCustomModel> ListCSM( PagingModel pm, CustomSearchModel csm )
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
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "csmRoleType", ( int ) csm.RoleType ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                u.*,
                                (SELECT TOP 1 ur.RoleId FROM [dbo].[UserRole] ur WHERE u.Id=ur.UserId) AS [RoleId],
                                (SELECT TOP 1 r.Name FROM [dbo].[UserRole] ur, [dbo].[Role] r WHERE u.Id=ur.UserId AND r.Id=ur.RoleId) AS [RoleName]
                             FROM
                                [dbo].[User] u";

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            // Limit to only show PSP for logged in user
            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu WHERE u.Id=pu.UserId AND pu.PSPId IN({string.Join( ",", CurrentUser.PSPs.Select( s => s.Id ) )})) ";
            }
            else if ( CurrentUser.RoleType == RoleType.Client )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE u.Id=cu.UserId AND cu.ClientId IN({string.Join( ",", CurrentUser.Clients.Select( s => s.Id ) )})) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.Status != Status.All )
            {
                query = $"{query} AND (u.Status=@csmStatus) ";
            }
            if ( csm.RoleType != RoleType.All )
            {
                query = $"{query} AND (u.Type=@csmRoleType) ";
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

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (u.[Name] LIKE '%{1}%' OR
                                                  u.[Surname] LIKE '%{1}%' OR
                                                  u.[Email] LIKE '%{1}%' OR
                                                  u.[Cell] LIKE '%{1}%'
                                             ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            return context.Database.SqlQuery<UserCustomModel>( query, parameters.ToArray() ).ToList();
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
