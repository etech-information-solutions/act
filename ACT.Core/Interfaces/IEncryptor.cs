using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Interfaces
{
    public interface IEncryptor
    {
        string Encrypt( string text );

        string Decrypt( string encryptedText );
    }
}
