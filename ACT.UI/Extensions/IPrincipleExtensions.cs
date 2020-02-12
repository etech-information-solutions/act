using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACT.Core.Enums;
using ACT.Core.Helpers;
using ACT.Core.Services;
using ACT.Core.Models;
using ACT.Data.Models;
using System.Reflection;

namespace System
{
    public static class IPrincipleExtensions
    {
        /// <summary>
        /// Determines whether an IPrinciple has Permission to do something within a PermissionContext.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static bool Has( this Security.Principal.IPrincipal user, PermissionTo permission, PermissionContext within )
        {
            bool isInRole = false;

            foreach ( PermissionTo pt in EnumHelper.GetOptions<PermissionTo>() )
            {
                if ( permission.MatchesFilter( pt ) )
                {
                    isInRole = user.IsInRole( string.Format( "{0}_{1}", within.GetStringValue(), pt.GetStringValue() ) );

                    if ( isInRole )
                    {
                        break;
                    }
                }
            }

            return isInRole;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static bool Has( this Security.Principal.IPrincipal user, PermissionContext within )
        {
            bool hasRole = false;

            using ( UserService service = new UserService() )
            {
                UserModel _user = service.GetUser( user.Identity.Name );

                hasRole = ( _user != null && _user.RoleModel.Name == within.GetStringValue() ) ? true : false;
            }

            return hasRole;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static UserModel Get( this Security.Principal.IPrincipal user )
        {
            UserModel _user;

            using ( UserService service = new UserService() )
            {
                _user = service.GetUser( user.Identity.Name );
            }

            return _user;
        }
    }
}