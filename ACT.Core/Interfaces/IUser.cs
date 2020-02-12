using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACT.Core.Interfaces
{

    public interface IUser
    {
        string Username { get; }

        IRole Role { get; }
    }

    public static class IUserExtensions
    {
        public static bool Owns( this IUser _this, IOwnable ownable )
        {
            return ownable.BelongsTo( _this );
        }
    }
}
