using System;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class PSPBudgetService : BaseService<PSPBudget>, IDisposable
    {
        public PSPBudgetService()
        {

        }

        public PSPBudget GetByPSPId( int id )
        {
            return context.PSPBudgets.FirstOrDefault( b => b.PSPId == id );
        }
    }
}
