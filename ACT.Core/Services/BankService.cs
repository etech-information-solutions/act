using System;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class BankService : BaseService<Bank>, IDisposable
    {
        public BankService()
        {

        }

        /// <summary>
        /// Checks if a bank with the same name already exists...?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exist( string name )
        {
            return context.Banks.Any( b => b.Name.ToLower() == name.ToLower() );
        }

        /// <summary>
        /// Gets a bank by bank name
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        public Bank GetByName( string name )
        {
            return context.Banks.FirstOrDefault( b => b.Name.ToLower().Replace( " ", "" ) == name.ToLower().Replace( " ", "" ) );
        }
    }
}
