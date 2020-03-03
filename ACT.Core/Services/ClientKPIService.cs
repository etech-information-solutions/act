using System;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientKPIService : BaseService<ClientKPI>, IDisposable
    {
        public ClientKPIService()
        {

        }
        public ClientKPI GetByPSPId(int id)
        {
            return context.ClientKPIs.FirstOrDefault(b => b.ClientId == id);
        }

    }
}
