using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Models.Simple;
using ACT.Core.Services;

namespace ACT.UI.Controllers.api
{
    public class PalletController : ApiController
    {
        BaseController baseC = new BaseController();

        // GET api/Pallet/GetOutstandingPalletsPerClient
        [ HttpGet]
        public List<SimpleOutstandingPallets> ListOutstandingPalletsPerClient( string email, string apikey )
        {
            CustomSearchModel csm = new CustomSearchModel() { };

            List<SimpleOutstandingPallets> response = new List<SimpleOutstandingPallets>();

            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                clservice.CurrentUser = clservice.GetUser( email );

                response = clservice.ListOutstandingPalletsPerClient( csm );

                if ( !response.NullableAny() )
                {
                    return response;
                }

                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, 1 );
                //csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                List<SimpleOutstandingPallets> response30 = clservice.ListOutstandingPalletsPerClient( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Past30 = response30?.FirstOrDefault( r => r.ClientId == s.ClientId )?.Total ?? 0;
                }
            }

            return response;
        }

        // GET api/Pallet/GetOutstandingPalletsPerSite
        [HttpGet]
        public List<SimpleOutstandingPallets> ListOutstandingPalletsPerSite( string email, string apikey )
        {
            List<SimpleOutstandingPallets> response = new List<SimpleOutstandingPallets>();

            CustomSearchModel csm = new CustomSearchModel() { };

            using ( ClientLoadService clservice = new ClientLoadService() )
            {
                clservice.CurrentUser = clservice.GetUser( email );

                response = clservice.ListOutstandingPalletsPerSite( csm );

                if ( !response.NullableAny() )
                {
                    return response;
                }

                // 0-3 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                List<SimpleOutstandingPallets> rep = clservice.ListOutstandingPalletsPerSite( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Month1 = rep?.FirstOrDefault( r => r.SiteId == s.SiteId )?.Total ?? 0;
                }

                // 4-6 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month ) );

                rep = clservice.ListOutstandingPalletsPerSite( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Month2 = rep?.FirstOrDefault( r => r.SiteId == s.SiteId )?.Total ?? 0;
                }

                // 7-12 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month ) );

                rep = clservice.ListOutstandingPalletsPerSite( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Month3 = rep?.FirstOrDefault( r => r.SiteId == s.SiteId )?.Total ?? 0;
                }

                // +12 Months
                csm.FromDate = null;
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month ) );

                rep = clservice.ListOutstandingPalletsPerSite( csm );

                foreach ( SimpleOutstandingPallets s in response )
                {
                    s.Month4 = rep?.FirstOrDefault( r => r.SiteId == s.SiteId )?.Total ?? 0;
                }
            }

            return response;
        }


        [HttpGet]
        public List<SiteAuditCustomModel> SiteAudits( string email, string apikey )
        {
            using ( SiteAuditService saservice = new SiteAuditService() )
            {
                saservice.CurrentUser = saservice.GetUser( email );

                List<SiteAuditCustomModel> response = saservice.List1( new PagingModel() { Take = int.MaxValue }, new CustomSearchModel() );

                return response;
            }
        }
    }
}
