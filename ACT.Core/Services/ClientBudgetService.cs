using System;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientBudgetService : BaseService<ClientBudget>, IDisposable
    {
        public ClientBudgetService()
        {

        }

        public ClientBudget GetByPSPId(int id)
        {
            return context.ClientBudgets.FirstOrDefault(b => b.ClientId == id);
        }
    }
}
