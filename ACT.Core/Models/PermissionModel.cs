using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACT.Data.Models;
using ACT.Core.Interfaces;
using ACT.Core.Enums;
using ACT.Core.Helpers;

namespace ACT.Core.Models
{
    public class PermissionModel : IPermission
    {
        #region Properties

        public int Id { get; set; }

        public int RoleId { get; set; }

        public PermissionContext PermissionContext { get; set; }

        public PermissionTo PermissionTo { get; set; }

        #endregion

        #region IPermission Members

        public string[] ToAspNetRoles()
        {
            List<string> list = new List<string>();

            string sc = PermissionContext.GetStringValue();

            foreach ( PermissionTo pt in EnumHelper.GetOptions<PermissionTo>() )
            {
                if ( PermissionTo.MatchesFilter( pt ) )
                {
                    list.Add( string.Format( "{0}_{1}", PermissionContext.GetStringValue(), pt.GetStringValue() ) );
                }
            }

            return list.ToArray();
        }

        #endregion

        public override string ToString()
        {
            return ToAspNetRoles().Delimit();
        }
    }
}