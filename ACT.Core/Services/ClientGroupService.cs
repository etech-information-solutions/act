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

        /// <summary>
        /// Gets a list of PSPs matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<ClientGroupCustomModel> ListCSM(PagingModel pm, CustomSearchModel csm)
        {
            if (csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date)
            {
                csm.ToDate = csm.ToDate?.AddDays(1);
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmClientId", csm.ClientId ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                //{ new SqlParameter( "clientid", clientId > 0 ? clientId : 0 ) },
            };

            #endregion

            string query = @"SELECT
                            p.*,gr.*
                             FROM
                                [dbo].[ClientGroup] p 
                                LEFT JOIN [dbo].[Group] gr ON p.ProducId = gr.Id";//  LEFT JOIN[dbo].[Client] cc ON cc.Id = cg.ClientId"

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            #endregion

            #region WHERE IF CLIENT
            if (csm.ClientId > 0)
            {
                query = $"{query} AND p.ClientId = @csmClientId ";
            }

            #endregion


            // Custom Search

            #region Custom Search

            if (csm.FromDate.HasValue && csm.ToDate.HasValue)
            {
                query = $"{query} AND (p.CreatedOn >= @csmFromDate AND p.CreatedOn <= @csmToDate) ";
            }
            else if (csm.FromDate.HasValue || csm.ToDate.HasValue)
            {
                if (csm.FromDate.HasValue)
                {
                    query = $"{query} AND (p.CreatedOn>=@csmFromDate) ";
                }
                if (csm.ToDate.HasValue)
                {
                    query = $"{query} AND (p.CreatedOn<=@csmToDate) ";
                }
            }

            #endregion

            // Normal Search

            #region Normal Search

            if (!string.IsNullOrEmpty(csm.Query))
            {
                query = string.Format(@"{0} AND (gr.[Name] LIKE '%{1}%' OR
                                                  gr.[Description] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim());
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {pm.SortBy} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format("{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query);

            return context.Database.SqlQuery<ClientGroupCustomModel>(query, parameters.ToArray()).ToList();
        }

        //public List<ClientGroup> GetClientGroupsByPSP(int pspId)
        //{
        //    List<ClientGroup> grouptList;
        //    //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        //    grouptList = (from g in context.ClientGroups
        //                  where g.PSPClient.PSPId == pspId
        //                  where g.Status == (int)Status.Active
        //                  select g).ToList();

        //    return grouptList;
        //}

        public List<ClientGroup> GetClientGroupsByClient(int clientId)
        {
            List<ClientGroup> grouptList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            grouptList = (from g in context.ClientGroups
                          where g.ClientId == clientId
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
