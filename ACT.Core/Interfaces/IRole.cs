using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACT.Core.Interfaces
{
    public interface IRole
    {
        string[] GetAspNetRoles();

        IEnumerable<IPermission> Permissions { get; }
    }
}
