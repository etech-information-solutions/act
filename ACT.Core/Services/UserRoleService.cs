using System;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class UserRoleService : BaseService<UserRole>, IDisposable
    {
        public UserRoleService()
        {

        }

        /// <summary>
        /// Checks if a UserRole with the same name already exists...?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int Count( int roleId = 0, int userId = 0 )
        {
            return context.UserRoles.Count( ur => ( roleId > 0 ? ur.RoleId == roleId : true ) && ( userId > 0 ? ur.UserId == userId : true ) );
        }
    }
}
