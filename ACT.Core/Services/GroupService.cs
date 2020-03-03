using System;
using System.Collections.Generic;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class GroupService : BaseService<Group>, IDisposable
    {
        public GroupService()
        {

        }

        public List<Group> GetGroupsByPSP(int pspId)
        {
            List<Group> groupList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            groupList = (from g in context.ClientGroups
                         join e in context.Groups
                         on g.GroupId equals e.Id
                      where g.PSPClient.PSPId == pspId
                      //where e.Status == (int)Status.Active
                      //where g.Status == (int)Status.Active
                         select e).ToList();

            return groupList;
        }
    }
}
