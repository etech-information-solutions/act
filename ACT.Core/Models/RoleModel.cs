using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACT.Core.Interfaces;

namespace ACT.Core.Models
{
    public class RoleModel : IRole
    {
        #region Properties

        public string Name { get; set; }

        public List<PermissionModel> Permissions { get; set; }

        #endregion

        #region IRole Members

        public string[] GetAspNetRoles()
        {

            if ( this.Permissions == null )
            {
                throw new Exception( "Permissions must be populated in order to implement IRole" );
            }

            List<string> list = new List<string>();

            Permissions.NullableForEach( p =>
            {
                list.AddRange( p.ToAspNetRoles() );
            } );

            return list.Distinct().ToArray();
        }

        IEnumerable<IPermission> IRole.Permissions
        {
            get
            {
                return this.Permissions;
            }
        }

        #endregion
    }
}
