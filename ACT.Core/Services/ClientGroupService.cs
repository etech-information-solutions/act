using System;
using System.Collections.Generic;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientGroupService : BaseService<ClientGroup>, IDisposable
    {
        public ClientGroupService()
        {

        }

        public List<ClientGroup> GetClientGroupsByPSP(int pspId)
        {
            List<ClientGroup> grouptList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            grouptList = (from g in context.ClientGroups
                          where g.PSPClient.PSPId == pspId
                          where g.Status == (int)Status.Active
                          select g).ToList();

            return grouptList;
        }

        public List<ClientGroup> GetClientGroupsByClientGroup(int groupId, int clientId)
        {
            List<ClientGroup> grouptList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            grouptList = (from g in context.ClientGroups
                          where g.ClientId == clientId
                          where g.GroupId == groupId
                          //where g.Status == (int)Status.Active
                          select g).ToList();

            return grouptList;
        }
    }
}
