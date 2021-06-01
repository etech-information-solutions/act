using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;
using ACT.Core.Models.Custom;

namespace ACT.Core.Services
{
    public class ClientGroupService : BaseService<ClientGroup>, IDisposable
    {
        public ClientGroupService()
        {

        }
    }
}
