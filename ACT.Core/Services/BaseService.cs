using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;
using ACT.Core.Interfaces;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ACT.Core.Services
{
    public class BaseService<T> : IDisposable, IEntity<T> where T : class
    {
        private bool disposing = false;

        internal ACTEntities context = new ACTEntities();

        public int ItemId { get; set; }

        /// <summary>
        /// Used to store Client currently selected by the logged in user
        /// </summary>
        public Client SelectedClient
        {
            get
            {
                return ( Client ) ContextExtensions.GetCachedUserData( "SEL_client" );
            }
        }

        /// <summary>
        /// Used to store the System Configuration
        /// </summary>
        public SystemConfig SystemConfig
        {
            get
            {
                SystemConfig c = ( SystemConfig ) ContextExtensions.GetCachedData( "SR_ca" );

                if ( c != null ) return c;

                c = context.SystemConfigs.FirstOrDefault();

                ContextExtensions.CacheData( "SR_ca", c );

                return c;
            }
        }

        /// <summary>
        /// Currently logged in user
        /// </summary>
        private UserModel currentUser;
        public UserModel CurrentUser
        {
            get
            {
                if ( currentUser != null ) return currentUser;

                if ( HttpContext.Current == null )
                    return null;

                string email = HttpContext.Current.User.Identity.Name;

                if ( string.IsNullOrEmpty( email ) )
                    return null;

                if ( !( ContextExtensions.GetCachedUserData( email ) is UserModel user ) )
                {
                    user = GetUser( email );
                }

                currentUser = user ?? new UserModel() { Id = 0 };

                return currentUser;
            }
            set
            {
                currentUser = value;
            }
        }

        /// <summary>
        /// Stores an entity's state before changes are made to it...
        /// </summary>
        public T OldObject { get; set; }

        public BaseService()
        {
            context.Database.CommandTimeout = 600; // Minimun set to 10 minutes

            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Gets a user using the specified Email Address and Password and populates necessary roles
        /// </summary>
        /// <param name="email">Email Address of the user to be fetched</param>
        /// <param name="password">Password of the user to be fetched</param>
        /// <returns></returns>
        public UserModel Login( string email, string password, bool resetPin = false )
        {
            if ( string.IsNullOrEmpty( email ) || string.IsNullOrEmpty( password ) ) return null;

            UserModel model = new UserModel();

            password = GetSha1Md5String( password );

            model = ( from u in context.Users
                      where
                      (
                        u.Email.Trim() == email.Trim() &&
                        u.Password == password &&
                        u.Status == ( int ) Status.Active
                      )
                      select new UserModel()
                      {
                          Id = u.Id,
                          Pin = u.Pin,
                          Cell = u.Cell,
                          Name = u.Name,
                          Email = u.Email,
                          Surname = u.Surname,
                          CreatedOn = u.CreatedOn,
                          Status = ( Status ) u.Status,
                          RoleType = ( RoleType ) u.Type,
                          DisplayName = u.Name + " " + u.Surname,

                          NiceCreatedOn = u.CreatedOn,
                          IsAdmin = u.UserRoles.Any( ur => ur.Role.Administration ),

                          Roles = u.UserRoles.Select( ur => ur.Role )
                                             .OrderByDescending( r => r.Id )
                                             .ToList(),
                          PSPs = u.PSPUsers.Select( p => p.PSP ).ToList(),
                          Clients = u.ClientUsers.Select( c => c.Client ).ToList(),
                          SelfieUrl = context.Images
                                             .Where( a => a.ObjectId == u.Id && a.ObjectType == "User" && a.Name.ToLower() == "selfie" )
                                             .Select( s => SystemConfig.ImagesLocation + "//" + s.Location ).FirstOrDefault(),
                      } ).FirstOrDefault();

            if ( model != null )
            {
                // Get roles
                model = this.ConfigRoles( model );

                User user = context.Users.FirstOrDefault( u => u.Id == model.Id );

                if ( resetPin )
                {
                    user.Pin = null;
                }

                user.LastLogin = DateTime.Now;

                context.Entry( user ).State = EntityState.Modified;
                context.SaveChanges();

                ContextExtensions.CacheUserData( model.Email, model );
            }

            return model;
        }

        /// <summary>
        /// Gets a user using the specified ID and populates the necessary roles
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public UserModel GetUser( string email )
        {
            if ( string.IsNullOrEmpty( email ) ) return null;

            UserModel model = new UserModel();

            model = ( from u in context.Users
                      where
                      (
                        u.Email == email &&
                        u.Status == ( int ) Status.Active
                      )
                      select new UserModel()
                      {
                          Id = u.Id,
                          Pin = u.Pin,
                          Cell = u.Cell,
                          Name = u.Name,
                          Email = u.Email,
                          Surname = u.Surname,
                          CreatedOn = u.CreatedOn,
                          Status = ( Status ) u.Status,
                          RoleType = ( RoleType ) u.Type,
                          DisplayName = u.Name + " " + u.Surname,

                          NiceCreatedOn = u.CreatedOn,
                          IsAdmin = u.UserRoles.Any( ur => ur.Role.Administration ),

                          Roles = u.UserRoles.Select( ur => ur.Role )
                                             .OrderByDescending( r => r.Id )
                                             .ToList(),
                          PSPs = u.PSPUsers.Select( p => p.PSP ).ToList(),
                          Clients = u.ClientUsers.Select( c => c.Client ).ToList(),
                          SelfieUrl = context.Images
                                             .Where( a => a.ObjectId == u.Id && a.ObjectType == "User" && a.Name.ToLower() == "selfie" )
                                             .Select( s => SystemConfig.ImagesLocation + "//" + s.Location ).FirstOrDefault(),
                      } ).FirstOrDefault();


            if ( model != null )
            {
                // Get roles
                model = this.ConfigRoles( model );

                ContextExtensions.CacheUserData( model.Email, model );
            }

            return model;
        }

        /// <summary>
        /// Configures the specified user's roles
        /// </summary>
        /// <param name="user"></param> 
        /// <returns></returns>
        public UserModel ConfigRoles( UserModel model )
        {
            // Role
            Role r = model.Roles.FirstOrDefault();

            model.Role = r;
            model.RoleType = ( RoleType ) r.Type;
            model.IsAdmin = ( r.Type == ( int ) RoleType.SuperAdmin );
            model.IsPSPAdmin = ( r.Type == ( int ) RoleType.PSP );

            RoleModel role = new RoleModel()
            {
                Name = r.Name,
                Permissions = new List<PermissionModel>()
            };

            Dictionary<string, object> permissions = r.GetType()
                                                      .GetProperties()
                                                      .ToDictionary( prop => prop.Name, prop => prop.GetValue( r, null ) );

            // permissions.Add( "DashBoard", true );

            int count = 1;

            foreach ( string key in permissions.Keys )
            {
                // If this cannot be parsed, then it's another [Role].[Property]
                // We're only look for: 
                // [DashBoard], [PaymentRequisition], [Authorisation], [Finance], [Supplier], [Report], [Administration]
                if ( !Enum.TryParse( key, out PermissionContext pContext ) ) continue;

                if ( permissions[ key ] == null ) continue;

                bool.TryParse( permissions[ key ].ToString(), out bool hasAccess );

                if ( !hasAccess ) continue;

                role.Permissions.Add( new PermissionModel() { Id = count, PermissionContext = pContext, PermissionTo = PermissionTo.View, RoleId = r.Id } );

                role.Permissions.Add( new PermissionModel() { Id = count, PermissionContext = pContext, PermissionTo = PermissionTo.Create, RoleId = r.Id } );
                role.Permissions.Add( new PermissionModel() { Id = count, PermissionContext = pContext, PermissionTo = PermissionTo.Edit, RoleId = r.Id } );
                role.Permissions.Add( new PermissionModel() { Id = count, PermissionContext = pContext, PermissionTo = PermissionTo.Delete, RoleId = r.Id } );

                count++;
            }

            model.RoleModel = role;

            return model;
        }

        /// <summary>
        /// Gets a user using the specified pin
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public UserModel GetByPin( string pin, string email )
        {
            pin = GetSha1Md5String( pin );

            User user = context.Users.FirstOrDefault( u => u.Pin == pin && u.Email == email );

            return ( user != null ) ? GetUser( user.Email ) : null;
        }


        /// <summary>
        /// Determines if a logged in user has Admin Rights...?
        /// </summary>
        /// <returns></returns>
        public bool UserHasAdminRights()
        {
            return ( CurrentUser.RoleType == RoleType.SuperAdmin );
        }

        /// <summary>
        /// Applies SHA1 and then MD5 to a given string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetSha1Md5String( string input )
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] result = sha1.ComputeHash( Encoding.UTF8.GetBytes( input ) );

            using ( MD5 md5 = MD5.Create() )
            {
                // Create a new Stringbuilder to collect the bytes 
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data  
                // and format each one as a hexadecimal string. 
                for ( int i = 0; i < result.Length; i++ )
                {
                    sBuilder.Append( result[ i ].ToString( "x2" ) );
                }

                input = sBuilder.ToString();
            }

            return input;
        }

        /// <summary>
        /// Adds tracking information/properties to an entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Track( T item, bool isUpdate = false )
        {
            var properties = item.GetType().GetProperties();

            for ( int i = 0; i < properties.Length; i++ )
            {
                if ( isUpdate && properties[ i ].Name == "CreatedOn" ) continue;

                //if ( !isUpdate && properties[ i ].GetValue( item ) != null ) continue;

                if ( !isUpdate && properties[ i ].Name == "CreatedOn" )
                {
                    properties[ i ].SetValue( item, DateTime.Now, null );
                }
                if ( properties[ i ].Name == "ModifiedOn" )
                {
                    properties[ i ].SetValue( item, DateTime.Now, null );
                }
                if ( properties[ i ].Name == "ModifiedBy" )
                {
                    properties[ i ].SetValue( item, ( ( CurrentUser != null && CurrentUser.Email != null ) ? CurrentUser.Email : "System" ), null );
                }

                // TRIM all strings
                if ( properties[ i ].PropertyType == typeof( string ) )
                {
                    string value = ( string ) properties[ i ].GetValue( item );

                    properties[ i ].SetValue( item, value?.Trim(), null );
                }
            }

            ItemId = ( int ) properties.FirstOrDefault( i => i.Name == "Id" ).GetValue( item );

            return item;
        }

        /// <summary>
        /// Creates a Contains in the Where Clause
        /// </summary>
        /// <param name="csm"></param>
        /// <returns></returns>
        public virtual Expression<Func<T, bool>> ColumnContains( CustomSearchModel csm )
        {
            ParameterExpression item = Expression.Parameter( typeof( T ), "item" );

            Expression body = Expression.NotEqual( Expression.Property( item, "Id" ), Expression.Constant( 0 ) );

            List<System.Reflection.PropertyInfo> properties = typeof( T ).GetProperties().Where( p => p.PropertyType == typeof( string ) ).ToList();

            if ( !properties.Any() && !string.IsNullOrEmpty( ( csm.Query ?? "" ) ) )
            {
                body = Expression.AndAlso( body, properties.Select( p => ( Expression ) Expression.Call(
                          Expression.Property( item, p ), "Contains", Type.EmptyTypes, Expression.Constant( ( csm.Query ?? "" ) ) )
                       ).Aggregate( Expression.OrElse ) );
            }

            return Expression.Lambda<Func<T, bool>>( body, item );
        }

        /// <summary>
        /// Applies a generic column where clause in the specified table T
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Expression<Func<T, bool>> ColumnWhere( string column, object value )
        {
            ParameterExpression itemParameter = Expression.Parameter( typeof( T ), "item" );

            Expression<Func<T, bool>> whereExpression = null;

            if ( value != null && typeof( T ).GetProperties().Any( p => p.Name == column ) )
            {
                whereExpression = Expression.Lambda<Func<T, bool>>
                    (
                    Expression.Equal( Expression.Property( itemParameter, column ), Expression.Constant( value ) ),
                    new[] { itemParameter }
                    );
            }
            else
            {
                // Fall back onto the Primary Key Id of the table. 
                // Make sure the table does have a column called Id though
                whereExpression = Expression.Lambda<Func<T, bool>>
                    (
                    Expression.NotEqual( Expression.Property( itemParameter, "Id" ), Expression.Constant( 0 ) ),
                    new[] { itemParameter }
                    );
            }

            return whereExpression;
        }

        /// <summary>
        /// Applies a generic column where clause in the specified table T
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private Expression<Func<T, bool>> ColumnsWhere( Dictionary<string, object> columns )
        {
            ParameterExpression item = Expression.Parameter( typeof( T ), "item" );

            Expression body = Expression.NotEqual( Expression.Property( item, "Id" ), Expression.Constant( 0 ) );

            if ( columns != null && columns.Any() )
            {
                foreach ( KeyValuePair<string, object> column in columns )
                {
                    if ( column.Value != null && typeof( T ).GetProperties().Any( p => p.Name == column.Key ) )
                    {
                        body = Expression.And( body, Expression.Equal( Expression.Property( item, column.Key ), Expression.Constant( column.Value ) ) );
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>( body, item );
        }

        /// <summary>
        /// Creates a custom sorting
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public string CreateOrderBy( string sortBy, string sort )
        {
            sort = string.IsNullOrEmpty( sort ) ? "ASC" : sort;
            sortBy = string.IsNullOrEmpty( sortBy ) ? "Id" : sortBy;

            if ( !sortBy.Contains( "," ) )
            {
                return string.Format( "{0} {1}", sortBy, sort );
            }

            string[] sorts = sortBy.Split( ',' );

            sortBy = string.Empty;

            foreach ( string s in sorts )
            {
                sortBy = string.Format( "{0}{1} {2},", sortBy, s, sort );
            }

            sortBy = sortBy.Remove( sortBy.Length - 1, 1 );

            return sortBy;
        }



        /// <summary>
        /// Gets an entity using the specified id
        /// </summary>
        /// <param name="value">Id of the entity to be fetched</param>
        /// <returns></returns>
        public virtual T GetById( int id )
        {
            ParameterExpression itemParameter = Expression.Parameter( typeof( T ), "item" );
            var whereExpression = Expression.Lambda<Func<T, bool>>
                (
                Expression.Equal( Expression.Property( itemParameter, "Id" ), Expression.Constant( id ) ),
                new[] { itemParameter }
                );

            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            T item = context.Set<T>().Where( whereExpression ).FirstOrDefault();

            using ( ACTEntities db = new ACTEntities() )
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                OldObject = db.Set<T>().AsNoTracking().Where( whereExpression ).FirstOrDefault();
            }

            return item;
        }

        /// <summary>
        /// Gets a total count of records
        /// </summary>
        /// <returns></returns>
        public virtual int Total()
        {
            return context.Set<T>().Count();
        }

        /// <summary>
        /// Gets a list of the entities available
        /// </summary>
        /// <returns></returns>
        public virtual List<T> List()
        {
            return context.Set<T>().ToList();
        }

        /// <summary>
        /// Gets a list of the entities available
        /// </summary>
        /// <returns></returns>
        public virtual List<T> SqlQueryList<T>( string query)
        {
            return context.Database.SqlQuery<T>( query ).ToList();
        }

        /// <summary>
        /// Gets the maximum value in the generic T in the specified column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual string MaxString( string column )
        {
            return context.Set<T>()
                          .SelectString( column )
                          .Max();
        }

        /// <summary>
        /// Gets the minimum value in the generic T in the specified column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual string MinString( string column )
        {
            return context.Set<T>()
                          .SelectString( column )
                          .Min();
        }

        /// <summary>
        /// Gets the minimum value in the generic T in the specified column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual DateTime? MinDateTime( string column )
        {
            return context.Set<T>()
                          .SelectDateTime( column )
                          .Min();
        }

        /// <summary>
        /// Gets a total count of records using the specified search filters
        /// </summary>
        /// <returns></returns>
        public virtual int Total( PagingModel pm, CustomSearchModel csm )
        {
            string[] qs = ( pm.Query ?? "" ).Split( ' ' );

            var where = ColumnContains( csm );

            // TODO: Add some generic WHERE clause e.g string properties to contain above qs/query

            return context.Set<T>().Count( where );
        }

        /// <summary>
        /// Gets a list of the entities available using the specified search filters
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public virtual List<T> List( PagingModel pm, CustomSearchModel csm )
        {
            var where = ColumnContains( csm );

            pm.Sort = ( string.IsNullOrEmpty( pm.Sort ) ) ? "ASC" : pm.Sort;
            pm.SortBy = ( string.IsNullOrEmpty( pm.SortBy ) ) ? "Id" : pm.SortBy;
            pm.Take = ( pm.Take.Equals( 0 ) ) ? ConfigSettings.PagingTake : pm.Take;

            return context.Set<T>()
                          .Where( where )
                          .OrderBy( CreateOrderBy( pm.SortBy, pm.Sort ) )
                          .Skip( pm.Skip )
                          .Take( pm.Take )
                          .ToList();
        }

        /// <summary>
        /// Gets a list of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="select"></param>
        /// <param name="select"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<string> ListByColumns( string select = "", Dictionary<string, object> columns = null )
        {
            return context.Set<T>()
                          .Where( ColumnsWhere( columns ) )
                          .SelectString( select )
                          .Distinct()
                          .ToList();
        }

        /// <summary>
        /// Gets a list of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="select"></param>
        /// <param name="select"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<string> ListByColumn( string select = "", string column = "", object value = null )
        {
            return context.Set<T>()
                          .Where( ColumnWhere( column, value ) )
                          .SelectString( select )
                          .Distinct()
                          .ToList();
        }

        /// <summary>
        /// Gets a list of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="select"></param>
        /// <param name="select"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> ListIntByColumn( string select = "", string column = "", object value = null )
        {
            return context.Set<T>()
                          .Where( ColumnWhere( column, value ) )
                          .SelectInt( select )
                          .Distinct()
                          .ToList();
        }

        /// <summary>
        /// Gets a string of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="select"></param>
        /// <param name="select"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetByColumns( string select = "", Dictionary<string, object> columns = null )
        {
            return context.Set<T>()
                          .Where( ColumnsWhere( columns ) )
                          .SelectString( select )
                          .Distinct()
                          .FirstOrDefault();
        }

        /// <summary>
        /// Gets a String of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="select"></param>
        /// <param name="select"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetByColumn( string select = "", string column = "", object value = null )
        {
            return context.Set<T>()
                          .Where( ColumnWhere( column, value ) )
                          .SelectString( select )
                          .Distinct()
                          .FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<T> ListByColumnWhere( string column = "", object value = null )
        {
            return context.Set<T>()
                          .Where( ColumnWhere( column, value ) )
                          .Distinct()
                          .ToList();
        }

        /// <summary>
        /// Gets a list of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<T> ListByColumnsWhere( string column = "", object value = null, string column2 = "", object value2 = null )
        {
            return context.Set<T>()
                            .Where( ColumnWhere( column, value ) )
                            .Where( ColumnWhere( column2, value2 ) )
                          .Distinct()
                          .ToList();
        }

        /// <summary>
        /// Gets a list of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public T GetByColumnWhere( string column = "", object value = null )
        {
            return context.Set<T>()
                          .Where( ColumnWhere( column, value ) )
                          .Distinct()
                          .FirstOrDefault();
        }

        /// <summary>
        /// A bit of a hack but relevant
        /// Gets a list of [select] for the specified table by generic T
        /// If the constraint column is defined then we'll apply it
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="column2"></param>
        /// <param name="value2"></param>/// 
        /// <returns></returns>
        public T GetByColumnsWhere( string column = "", object value = null, string column2 = "", object value2 = null )
        {
            if ( !string.IsNullOrEmpty( column ) && !string.IsNullOrEmpty( column2 ) )
            {
                return context.Set<T>()
                              .Where( ColumnWhere( column, value ) )
                              .Where( ColumnWhere( column2, value2 ) )
                              .Distinct()
                              .FirstOrDefault();
            }
            else return null;
        }


        /// <summary>
        /// Deletes an existing entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Delete( T item )
        {
            if ( !context.ChangeTracker.Entries<T>().Any( e => e.Entity == item ) )
            {
                context.Set<T>().Attach( item );
            }

            context.Set<T>().Remove( item );
            context.SaveChanges();

            using ( AuditLogService service = new AuditLogService() )
            {
                service.Create( ActivityTypes.Delete, item, OldObject );
            }

            return true;
        }

        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual T Create( T item, bool track = true )
        {
            System.Reflection.PropertyInfo[] properties = item.GetType().GetProperties();

            // Tracking
            if ( track )
            {
                item = Track( item );
            }

            context.Set<T>().Add( item );

            context.SaveChanges();

            using ( AuditLogService service = new AuditLogService() )
            {
                service.Create( ActivityTypes.Create, item );
            }

            return item;
        }

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual T Update( T item )
        {
            // Tracking
            item = Track( item, true );

            context.Entry( item ).State = EntityState.Modified;
            context.SaveChanges();

            using ( AuditLogService service = new AuditLogService() )
            {
                service.Create( ActivityTypes.Edit, item, OldObject );
            }

            return item;
        }

        /// <summary>
        /// Executes a T-SQL against the database
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual bool Query( string query, int timeout = 0 )
        {
            if ( string.IsNullOrEmpty( query ) ) return false;

            using ( ACTEntities context = new ACTEntities() )
            {
                if ( timeout > 0 )
                {
                    context.Database.CommandTimeout = timeout;
                }

                context.Database.ExecuteSqlCommand( query );
            }

            return true;
        }

        /// <summary>
        /// Formats the number depending on prefix and max
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public string FormatNumber( string prefix, int max )
        {
            string number = string.Empty;

            // Reduce
            if ( max < 10 )
            {
                number = string.Format( "{0}00000000{1}", prefix, max );
            }
            else if ( max >= 10 && max < 100 )
            {
                number = string.Format( "{0}0000000{1}", prefix, max );
            }
            else if ( max >= 100 && max < 1000 )
            {
                number = string.Format( "{0}000000{1}", prefix, max );
            }
            else if ( max >= 1000 && max < 10000 )
            {
                number = string.Format( "{0}00000{1}", prefix, max );
            }
            else if ( max >= 10000 && max < 100000 )
            {
                number = string.Format( "{0}0000{1}", prefix, max );
            }
            else if ( max >= 100000 && max < 1000000 )
            {
                number = string.Format( "{0}000{1}", prefix, max );
            }
            else if ( max >= 1000000 && max < 10000000 )
            {
                number = string.Format( "{0}00{1}", prefix, max );
            }
            else if ( max >= 10000000 && max < 100000000 )
            {
                number = string.Format( "{0}0{1}", prefix, max );
            }
            else if ( max >= 100000000 )
            {
                number = string.Format( "{0}{1}", prefix, max );
            }

            return number;
        }

        /// <summary>
        /// Makes a post request to the specified host using the specified Params
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string ApiPost( string url, string param, string login, string method = "POST", string contentType = "application/x-www-form-urlencoded", string accept = "text/json" )
        {
            HttpWebRequest req = ( HttpWebRequest ) WebRequest.Create( url );

            byte[] data = Encoding.ASCII.GetBytes( param );

            req.Accept = accept;
            req.Method = method;
            req.Timeout = 1200000;
            req.ContentType = contentType;
            req.ContentLength = data.Length;
            req.Headers[ "Authorization" ] = $"BASIC {Convert.ToBase64String( Encoding.ASCII.GetBytes( login ) )}";

            using ( Stream s = req.GetRequestStream() )
            {
                s.Write( data, 0, data.Length );
            }

            HttpWebResponse respose = ( HttpWebResponse ) req.GetResponse();

            string strResponse = ( respose != null ) ? new StreamReader( respose.GetResponseStream() ).ReadToEnd() : string.Empty;

            return strResponse;
        }

        public ICollection<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public T Get( int id )
        {
            return context.Set<T>().Find( id );
        }

        public async Task<T> GetAsync( int id )
        {
            return await context.Set<T>().FindAsync( id );
        }

        public T Find( Expression<Func<T, bool>> match )
        {
            return context.Set<T>().SingleOrDefault( match );
        }

        public async Task<T> FindAsync( Expression<Func<T, bool>> match )
        {
            return await context.Set<T>().SingleOrDefaultAsync( match );
        }

        public ICollection<T> FindAll( Expression<Func<T, bool>> match )
        {
            return context.Set<T>().Where( match ).ToList();
        }

        public async Task<ICollection<T>> FindAllAsync( Expression<Func<T, bool>> match )
        {
            return await context.Set<T>().Where( match ).ToListAsync();
        }

        public int Count()
        {
            return context.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await context.Set<T>().CountAsync();
        }

        public void Dispose()
        {
            this.disposing = true;
        }
    }
}
