using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ClientService : BaseService<Client>, IDisposable
    {
        public ClientService()
        {

        }

        /// <summary>
        /// Gets a list of clients
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Dictionary<int, string> List( bool v )
        {
            Dictionary<int, string> clientOptions = new Dictionary<int, string>();
            List<IntStringKeyValueModel> model = new List<IntStringKeyValueModel>();

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
            };

            string query = string.Empty;

            query = $"SELECT c.Id AS [TKey], c.CompanyName AS [TValue] FROM [dbo].[Client] c WHERE (1=1)";

            if ( !CurrentUser.IsAdmin )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[ClientUser] cu WHERE cu.UserId=@userid AND cu.UserId=c.Id)";
            }

            model = context.Database.SqlQuery<IntStringKeyValueModel>( query.Trim(), parameters.ToArray() ).ToList();

            if ( model != null && model.Any() )
            {
                foreach ( var k in model )
                {
                    if ( clientOptions.Keys.Any( x => x == k.TKey ) )
                        continue;

                    clientOptions.Add( k.TKey, ( k.TValue ?? "" ).Trim() );
                }
            }

            return clientOptions;
        }

        public List<Client> GetClientsByPSP(int pspId) 
        {
            List<Client> clientList;
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            clientList = (from p in context.PSPClients
                              join e in context.Clients
                              on p.ClientId equals e.Id
                              where p.PSPId == pspId
                              select e).ToList();
                              
                              


            return clientList;
        }


        public List<Client> GetClientsByPSPAwaitingActivation(int pspId)
        {
            List<Client> clientList;
            List<int> statusList = new List<int>();
            statusList.Add((int)Status.Pending);
            statusList.Add((int)Status.Inactive);            
            //context.Roles.FirstOrDefault(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            clientList = (from p in context.PSPClients
                          join e in context.Clients
                          on p.ClientId equals e.Id
                          where p.PSPId == pspId
                          where statusList.Contains(e.Status)
                          select e).ToList();

            return clientList;
        }

        public List<Client> GetClientsByPSPIncludedGroup(int pspId, int groupId)
        {
            List<object> parameters = new List<object>();

            //return clientList;
            string query = string.Format("SELECT DISTINCT(Client.Id) as DClientId, Client.* FROM Client LEFT JOIN PSPClient ON Client.Id = PSPClient.ClientID LEFT JOIN ClientGroup ON ClientGroup.ClientId = PSPClient.ClientId WHERE GroupId = {0} AND PSPClient.PSPId = {1}", groupId, pspId);

            return context.Database.SqlQuery<Client>(query, parameters.ToArray()).ToList();
        }

        public List<Client> GetClientsByPSPExcludedGroup(int pspId, int groupId)
        {
            List<object> parameters = new List<object>();
            string query = string.Format("SELECT DISTINCT(Client.Id) as DClientId,Client.* FROM Client LEFT JOIN PSPClient ON Client.Id = PSPClient.ClientID WHERE NOT EXISTS (SELECT ClientID FROM ClientGroup WHERE GroupId = {0} AND ClientGroup.ClientId = PSPClient.ClientId) AND PSPClient.PSPId = {1}", groupId, pspId);

            return context.Database.SqlQuery<Client>(query, parameters.ToArray()).ToList();
        }

        public bool ExistByCompanyRegistrationNumber( string registrationNumber )
        {
            return context.Clients.Any( c => c.CompanyRegistrationNumber == registrationNumber );
        }
    }
}
