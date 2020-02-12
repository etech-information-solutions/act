using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACT.Core.Helpers;
using ACT.Core.Models;
using ACT.Core.Services;

namespace ACT.UI.Providers
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        public RoleProvider()
        {
        }


        public override void AddUsersToRoles( string[] usernames, string[] roleNames )
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }


        public override void CreateRole( string roleName )
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole( string roleName, bool throwOnPopulatedRole )
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole( string roleName, string usernameToMatch )
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using ( RoleService service = new RoleService() )
            {
                return service.GetAllAspNetRoles();
            }
        }

        public override string[] GetRolesForUser( string username )
        {
            using ( UserService service = new UserService() )
            {
                UserModel user = service.GetUser( username );

                if ( user != null )
                {
                    return user.GetAspNetRoles();
                }
            }

            return new string[] { };
        }

        public override string[] GetUsersInRole( string roleName )
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole( string username, string roleName )
        {
            using ( UserService service = new UserService() )
            {
                UserModel user = service.GetUser( username );

                string[] roles = user.GetAspNetRoles();

                return roles.Contains( roleName );
            }
        }

        public override void RemoveUsersFromRoles( string[] usernames, string[] roleNames )
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists( string roleName )
        {
            throw new NotImplementedException();
        }
    }
}