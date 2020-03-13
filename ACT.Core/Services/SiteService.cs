using System;
using System.Collections.Generic;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class SiteService : BaseService<Site>, IDisposable
    {
        public SiteService()
        {

        }

        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List(bool v)
        {
            Dictionary<int, string> siteOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>();

            string query = string.Empty;

            query = $"SELECT c.Id AS [TKey], c.Name AS [TValue] FROM [dbo].[Sites] c";

            model = context.Database.SqlQuery<IntStringKeyValueModel>(query.Trim(), parameters.ToArray()).ToList();

            if (model != null && model.Any())
            {
                foreach (var k in model)
                {
                    if (siteOptions.Keys.Any(x => x == k.TKey))
                        continue;

                    siteOptions.Add(k.TKey, (k.TValue ?? "").Trim());
                }
            }

            return siteOptions;
        }

        public List<Site> GetSitesByClient(int clientId)
        {
            List<Site> siteList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            siteList = (from p in context.ClientSites
                          join e in context.Sites
                          on p.SiteId equals e.Id
                          where p.ClientId == clientId
                        select e).ToList();

            return siteList;
        }

        public List<Site> GetSubSitesBySiteClient(int clientId, int siteId)
        {
            List<Site> siteList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            siteList = (from p in context.ClientSites
                        join e in context.Sites
                        on p.SiteId equals e.Id
                        where p.ClientId == clientId
                        where e.SiteId == siteId
                        select e).ToList();

            return siteList;
        }

        public List<Site> GetSitesByClientsOfPSP(int pspId)
        {
            List<Site> siteList;
            siteList = (from p in context.PSPClients
                        join c in context.ClientSites
                        on p.ClientId equals c.ClientId
                        join e in context.Sites
                        on c.SiteId equals e.Id
                        where p.PSPId == pspId
                        select e).ToList();

            return siteList;
        }

        public List<Site> GetSitesByClientsOfPSPIncluded(int pspId, int clientId, int siteId)
        {
            List<Site> siteList;
            siteList = (from p in context.PSPClients
                        join c in context.ClientSites
                        on p.ClientId equals c.ClientId
                        join e in context.Sites
                        on c.SiteId equals e.Id
                        where p.PSPId == pspId
                        where e.SiteId == siteId
                        where c.SiteId == siteId
                        select e).ToList();


            return siteList;
        }
        public List<Site> GetSitesByClientIncluded(int clientId)
        {
            List<Site> siteList;
            siteList = (from s in context.Sites
                        join e in context.ClientSites                        
                        on s.Id equals e.SiteId
                        where e.ClientId == clientId
                        select s).ToList();
            return siteList;
        }
        

        public List<Site> GetSitesByClientsOfPSPExcluded(int pspId, int clientId, int siteId)
        {
            List<Site> siteList;
            List<int> exclList = new List<int>();
            exclList.Add(clientId);

            siteList = (from p in context.PSPClients
                        join c in context.ClientSites
                        on p.ClientId equals c.ClientId
                        join e in context.Sites
                        on c.SiteId equals e.Id
                        where p.PSPId == pspId
                        where c.SiteId == siteId
                        where e.SiteId == null
                        where !exclList.Contains(p.ClientId)
                        select e).ToList();

            return siteList;
        }


        public bool ExistByAccountCode(string accCode)
        {
            return context.Sites.Any(c => c.AccountCode == accCode);
        }
    }
}
