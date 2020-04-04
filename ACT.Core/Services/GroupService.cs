using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        //public List<Group> GetGroupsByPSP(int pspId)
        //{
        //    List<Group> groupList;
        //    //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        //    //groupList = (from g in context.ClientGroups
        //    //             join e in context.Groups
        //    //             on g.GroupId equals e.Id
        //    //          where g.PSPClient.PSPId == pspId
        //    //          //where e.Status == (int)Status.Active
        //    //          //where g.Status == (int)Status.Active
        //    //             select e).ToList();
        //    groupList = (from g in context.Groups
        //                 //where e.Status == (int)Status.Active
        //                 //where g.Status == (int)Status.Active
        //                 select g).ToList();

        //    return groupList;
        //}

        public List<Group> GetGroupsByPSP(int pspId, CustomSearchModel csm)
        {
            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmClientStatus", ( int ) csm.ClientStatus ) },
            };

            #endregion

            //return clientList;
            string query = string.Format("SELECT DISTINCT(g.Id) as DGroupId, g.* FROM [dbo].[Group] g WHERE 1=1 AND Status = 1");
            #region Custom Search

            if (csm.ClientId > 0)
            {
                query = $"{query} AND EXISTS (SELECT 1 FROM [dbo].[ClientGroup] cg WHERE cg.GroupId = g.Id AND cg.ClientId=@csmClientId AND cg.Status = 1) ";
            }

            //if (csm.ClientStatus != Status.All)
            //{
            //    query = $"{query} AND (Client.Status=@csmClientStatus) ";
            //}
            #endregion
            return context.Database.SqlQuery<Group>(query, parameters.ToArray()).ToList();
        }
    }
}
