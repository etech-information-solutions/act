using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.UI.Mvc;

namespace ACT.UI.Controllers
{
    [Requires( PermissionTo.View, PermissionContext.DashBoard )]
    public class DashBoardController : BaseController
    {
        // GET: DashBoard
        public ActionResult Index()
        {
            return View();
        }



        #region Partial Views

        //
        // POST || GET: /DashBoard/AgeOfOutstandingPallets
        public ActionResult AgeOfOutstandingPallets( CustomSearchModel csm, bool givedata = false )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                csm.ReconciliationStatus = ReconciliationStatus.Unreconciled;

                if ( givedata )
                {
                    List<ClientLoadCustomModel> loads = service.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_AgeOfOutstandingPalletsData", loads );
                }

                List<AgeOfOutstandingPallets> reps = new List<AgeOfOutstandingPallets>();

                // 0-3 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                AgeOfOutstandingPallets rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 03;
                rep.DurationName = "0-3 Months";

                reps.Add( rep );

                // 4-6 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month ) );

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 06;
                rep.DurationName = "4-6 Months";

                reps.Add( rep );

                // 7-12 Months
                csm.FromDate = new DateTime( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month, 1 );
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -6 ).Year, DateTime.Now.AddMonths( -6 ).Month ) );

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 12;
                rep.DurationName = "7-12 Months";

                reps.Add( rep );

                // +12 Months
                csm.FromDate = null;
                csm.ToDate = new DateTime( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month, DateTime.DaysInMonth( DateTime.Now.AddMonths( -12 ).Year, DateTime.Now.AddMonths( -12 ).Month ) );

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 13;
                rep.DurationName = "> 12 Months";

                reps.Add( rep );

                reps = reps.OrderBy( o => o.DurationNumber ).ToList();

                return PartialView( "_AgeOfOutstandingPallets", reps );
            }
        }

        //
        // POST || GET: /DashBoard/LoadsPerMonth
        public ActionResult LoadsPerMonth( CustomSearchModel csm, bool givedata = false )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                DateTime fromDate = csm.FromDate ?? new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, 1 );
                DateTime toDate = csm.ToDate ?? new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                if ( givedata )
                {
                    csm.ToDate = toDate;
                    csm.FromDate = fromDate;

                    List<ClientLoadCustomModel> loadsPerMonth = service.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_LoadsPerMonthData", loadsPerMonth );
                }

                List<LoadsPerMonth> loads = new List<LoadsPerMonth>();

                int months = ( ( toDate.Year - fromDate.Year ) * 12 ) + toDate.Month - fromDate.Month;

                months = months <= 0 ? 1 : months;

                LoadsPerMonth load;

                for ( int i = 0; i < months; i++ )
                {
                    csm.FromDate = fromDate.AddMonths( months - i );
                    csm.ToDate = new DateTime( csm.FromDate.Value.Year, csm.FromDate.Value.Month, DateTime.DaysInMonth( csm.FromDate.Value.Year, csm.FromDate.Value.Month ) );

                    load = service.LoadsPerMonth( csm );

                    load.MonthNumber = csm.FromDate.Value.Month;
                    load.MonthName = csm.FromDate.Value.ToString( "MMMM" );

                    loads.Add( load );
                }

                //Yearly cumulative

                csm.FromDate = new DateTime( DateTime.Now.Year, 1, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                load = service.LoadsPerMonth( csm );

                load.MonthNumber = int.MaxValue;
                load.MonthName = csm.FromDate.Value.ToString( "yyyy" );

                loads.Add( load );

                loads = loads.OrderBy( o => o.MonthNumber ).ToList();

                return PartialView( "_LoadsPerMonth", loads );
            }
        }

        //
        // POST || GET: /DashBoard/NumberOfPalletsManaged
        public ActionResult NumberOfPalletsManaged( CustomSearchModel csm, bool givedata = false )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                if ( givedata )
                {
                    List<ClientLoadCustomModel> numberOfPalletsManaged = service.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_NumberOfPalletsManagedData", numberOfPalletsManaged );
                }

                List<NumberOfPalletsManaged> loads = new List<NumberOfPalletsManaged>();

                DateTime fromDate = csm.FromDate ?? new DateTime( DateTime.Now.Year, 1, 1 );
                DateTime toDate = csm.ToDate ?? new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                int months = ( ( toDate.Year - fromDate.Year ) * 12 ) + toDate.Month - fromDate.Month;

                months = months <= 0 ? 1 : months;

                NumberOfPalletsManaged load;

                for ( int i = 0; i < ( months + 1 ); i++ )
                {
                    csm.FromDate = fromDate.AddMonths( months - i );
                    csm.ToDate = new DateTime( csm.FromDate.Value.Year, csm.FromDate.Value.Month, DateTime.DaysInMonth( csm.FromDate.Value.Year, csm.FromDate.Value.Month ) );

                    load = service.NumberOfPalletsManaged( csm );

                    load.MonthNumber = csm.FromDate.Value.Month;
                    load.MonthName = csm.FromDate.Value.ToString( "MMMM" );

                    loads.Add( load );
                }

                loads = loads.OrderBy( o => o.MonthNumber ).ToList();

                return PartialView( "_NumberOfPalletsManaged", loads );
            }
        }

        //
        // POST || GET: /DashBoard/AuthorisationCodes
        public ActionResult AuthorisationCodes( CustomSearchModel csm, bool givedata = false )
        {
            using ( ClientLoadService clservice = new ClientLoadService() )
            using ( ClientAuthorisationService aservice = new ClientAuthorisationService() )
            {
                DateTime fromDate = csm.FromDate ?? new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, 1 );
                DateTime toDate = csm.ToDate ?? new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                if ( givedata )
                {
                    csm.ToDate = toDate;
                    csm.FromDate = fromDate;

                    List<ClientAuthorisationCustomModel> authCodes = aservice.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_AuthorisationCodesData", authCodes );
                }

                List<AuthorisationCodes> auths = new List<AuthorisationCodes>();

                int months = ( ( toDate.Year - fromDate.Year ) * 12 ) + toDate.Month - fromDate.Month;

                months = months <= 0 ? 1 : months;

                AuthorisationCodes auth;

                for ( int i = 0; i < months; i++ )
                {
                    csm.FromDate = fromDate.AddMonths( months - i );
                    csm.ToDate = new DateTime( csm.FromDate.Value.Year, csm.FromDate.Value.Month, DateTime.DaysInMonth( csm.FromDate.Value.Year, csm.FromDate.Value.Month ) );

                    auth = clservice.AuthorisationCodesPerMonth( csm );

                    auth.MonthNumber = csm.FromDate.Value.Month;
                    auth.MonthName = csm.FromDate.Value.ToString( "MMMM" );

                    auths.Add( auth );
                }

                //Yearly cumulative

                csm.FromDate = new DateTime( DateTime.Now.Year, 1, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                auth = clservice.AuthorisationCodesPerMonth( csm );

                auth.MonthNumber = int.MaxValue;
                auth.MonthName = csm.FromDate.Value.ToString( "yyyy" );

                auths.Add( auth );

                auths = auths.OrderBy( o => o.MonthNumber ).ToList();

                return PartialView( "_AuthorisationCodes", auths );
            }
        }

        //
        // POST || GET: /DashBoard/KPIMeasurement
        public ActionResult KPIMeasurement( CustomSearchModel csm, bool givedata = false )
        {

            return PartialView( "_KPIMeasurement" );
        }

        //
        // POST || GET: /DashBoard/NumberOfDisputes
        public ActionResult NumberOfDisputes( CustomSearchModel csm, bool givedata = false )
        {
            DateTime fromDate = csm.FromDate ?? new DateTime( DateTime.Now.AddMonths( -3 ).Year, DateTime.Now.AddMonths( -3 ).Month, 1 );
            DateTime toDate = csm.ToDate ?? new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

            using ( DisputeService service = new DisputeService() )
            {
                if ( givedata )
                {
                    csm.ToDate = toDate;
                    csm.FromDate = fromDate;

                    List<DisputeCustomModel> authCodes = service.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_NumberOfDisputesData", authCodes );
                }

                List<NumberOfDisputes> disputes = new List<NumberOfDisputes>();

                int months = ( ( toDate.Year - fromDate.Year ) * 12 ) + toDate.Month - fromDate.Month;

                months = months <= 0 ? 1 : months;

                NumberOfDisputes dispute;

                for ( int i = 0; i < months; i++ )
                {
                    csm.FromDate = fromDate.AddMonths( months - i );
                    csm.ToDate = new DateTime( csm.FromDate.Value.Year, csm.FromDate.Value.Month, DateTime.DaysInMonth( csm.FromDate.Value.Year, csm.FromDate.Value.Month ) );

                    dispute = service.NumberOfDisputes( csm );

                    dispute.MonthNumber = csm.FromDate.Value.Month;
                    dispute.MonthName = csm.FromDate.Value.ToString( "MMMM" );

                    disputes.Add( dispute );
                }

                //Yearly cumulative

                csm.FromDate = new DateTime( DateTime.Now.Year, 1, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                dispute = service.NumberOfDisputes( csm );

                dispute.MonthNumber = int.MaxValue;
                dispute.MonthName = csm.FromDate.Value.ToString( "yyyy" );

                disputes.Add( dispute );

                disputes = disputes.OrderBy( o => o.MonthNumber ).ToList();

                return PartialView( "_NumberOfDisputes", disputes );
            }
        }

        #endregion
    }
}
