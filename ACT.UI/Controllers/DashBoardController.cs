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
            using ( ChepLoadService service = new ChepLoadService() )
            {
                csm.BalanceStatus = BalanceStatus.NotBalanced;

                if ( givedata )
                {
                    List<ChepLoadCustomModel> loads = service.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_AgeOfOutstandingPalletsData", loads );
                }

                List<AgeOfOutstandingPallets> reps = new List<AgeOfOutstandingPallets>();

                // 0-30 Days
                csm.FromDate = DateTime.Now.AddDays( -30 ).Date;
                csm.ToDate = DateTime.Now.Date;

                AgeOfOutstandingPallets rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 30;
                rep.DurationName = "0-30 Days";

                reps.Add( rep );

                // 31-60 Days
                csm.FromDate = DateTime.Now.AddDays( -60 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -31 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 60;
                rep.DurationName = "31-60 Days";

                reps.Add( rep );

                // 61-90 Days
                csm.FromDate = DateTime.Now.AddDays( -90 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -61 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 90;
                rep.DurationName = "61-90 Days";

                reps.Add( rep );

                // 91-120 Days
                csm.FromDate = DateTime.Now.AddDays( -120 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -91 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 120;
                rep.DurationName = "91-120 Days";

                reps.Add( rep );

                // 121-183 Days
                csm.FromDate = DateTime.Now.AddDays( -183 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -121 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 183;
                rep.DurationName = "121-183 Days";

                reps.Add( rep );

                // 184-270 Days
                csm.FromDate = DateTime.Now.AddDays( -270 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -184 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 270;
                rep.DurationName = "184-270 Days";

                reps.Add( rep );

                // 271-365 Days
                csm.FromDate = DateTime.Now.AddDays( -365 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -271 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 365;
                rep.DurationName = "271-365 Days";

                reps.Add( rep );

                // By Years? Get MIN YEAR
                DateTime minYear = service.MinDateTime( "ShipmentDate" ) ?? DateTime.Now;

                if ( minYear.Year == DateTime.Now.Year )
                {
                    reps = reps.OrderBy( o => o.DurationNumber ).ToList();

                    return PartialView( "_AgeOfOutstandingPallets", reps );
                }

                // -1 Year
                int minus1Year = DateTime.Now.AddYears( -1 ).Year;

                csm.FromDate = new DateTime( DateTime.Now.AddYears( -1 ).Year, 1, 1 );
                csm.ToDate = DateTime.Now.AddDays( -366 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = minus1Year;
                rep.DurationName = minus1Year.ToString();

                reps.Add( rep );

                if ( minus1Year == minYear.Year )
                {
                    reps = reps.OrderBy( o => o.DurationNumber ).ToList();

                    return PartialView( "_AgeOfOutstandingPallets", reps );
                }

                for ( int i = ( minus1Year - 1 ); i >= minYear.Year; i-- )
                {
                    csm.FromDate = new DateTime( i, 1, 1 );
                    csm.ToDate = new DateTime( i, 12, 31 );

                    rep = service.AgeOfOutstandingPallets( csm );

                    rep.DurationNumber = i;
                    rep.DurationName = i.ToString();

                    reps.Add( rep );
                }

                reps = reps.OrderBy( o => o.DurationNumber ).ToList();

                return PartialView( "_AgeOfOutstandingPallets", reps );
            }
        }

        //
        // POST || GET: /DashBoard/AgeOfOutstandingPOD
        public ActionResult AgeOfOutstandingPOD( CustomSearchModel csm, bool givedata = false )
        {
            using ( ChepLoadService service = new ChepLoadService() )
            {
                csm.IsPODOutstanding = true;
                csm.BalanceStatus = BalanceStatus.NotBalanced;
                csm.ReconciliationStatus = ReconciliationStatus.Reconciled;

                if ( givedata )
                {
                    List<ChepLoadCustomModel> loads = service.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_AgeOfOutstandingPODData", loads );
                }

                List<AgeOfOutstandingPallets> reps = new List<AgeOfOutstandingPallets>();

                // 0-30 Days
                csm.FromDate = DateTime.Now.AddDays( -30 ).Date;
                csm.ToDate = DateTime.Now.Date;

                AgeOfOutstandingPallets rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 30;
                rep.DurationName = "0-30 Days";

                reps.Add( rep );

                // 31-60 Days
                csm.FromDate = DateTime.Now.AddDays( -60 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -31 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 60;
                rep.DurationName = "31-60 Days";

                reps.Add( rep );

                // 61-90 Days
                csm.FromDate = DateTime.Now.AddDays( -90 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -61 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 90;
                rep.DurationName = "61-90 Days";

                reps.Add( rep );

                // 91-120 Days
                csm.FromDate = DateTime.Now.AddDays( -120 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -91 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 120;
                rep.DurationName = "91-120 Days";

                reps.Add( rep );

                // 121-183 Days
                csm.FromDate = DateTime.Now.AddDays( -183 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -121 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 183;
                rep.DurationName = "121-183 Days";

                reps.Add( rep );

                // 184-270 Days
                csm.FromDate = DateTime.Now.AddDays( -270 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -184 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 270;
                rep.DurationName = "184-270 Days";

                reps.Add( rep );

                // 271-365 Days
                csm.FromDate = DateTime.Now.AddDays( -365 ).Date;
                csm.ToDate = DateTime.Now.AddDays( -271 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = 365;
                rep.DurationName = "271-365 Days";

                reps.Add( rep );

                // By Years? Get MIN YEAR
                DateTime minYear = service.MinDateTime( "ShipmentDate" ) ?? DateTime.Now;

                if ( minYear.Year == DateTime.Now.Year )
                {
                    reps = reps.OrderBy( o => o.DurationNumber ).ToList();

                    return PartialView( "_AgeOfOutstandingPOD", reps );
                }

                // -1 Year
                int minus1Year = DateTime.Now.AddYears( -1 ).Year;

                csm.FromDate = new DateTime( DateTime.Now.AddYears( -1 ).Year, 1, 1 );
                csm.ToDate = DateTime.Now.AddDays( -366 ).Date;

                rep = service.AgeOfOutstandingPallets( csm );

                rep.DurationNumber = minus1Year;
                rep.DurationName = minus1Year.ToString();

                reps.Add( rep );

                if ( minus1Year == minYear.Year )
                {
                    reps = reps.OrderBy( o => o.DurationNumber ).ToList();

                    return PartialView( "_AgeOfOutstandingPOD", reps );
                }

                for ( int i = ( minus1Year - 1 ); i >= minYear.Year; i-- )
                {
                    csm.FromDate = new DateTime( i, 1, 1 );
                    csm.ToDate = new DateTime( i, 12, 31 );

                    rep = service.AgeOfOutstandingPallets( csm );

                    rep.DurationNumber = i;
                    rep.DurationName = i.ToString();

                    reps.Add( rep );
                }

                reps = reps.OrderBy( o => o.DurationNumber ).ToList();

                return PartialView( "_AgeOfOutstandingPOD", reps );
            }
        }

        //
        // POST || GET: /DashBoard/LoadsPerMonth
        public ActionResult LoadsPerMonth( CustomSearchModel csm, bool givedata = false )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                DateTime fromDate = csm.FromDate ?? new DateTime( DateTime.Now.AddMonths( -2 ).Year, DateTime.Now.AddMonths( -2 ).Month, 1 );
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

                months = months <= 0 ? 1 : months + 1;

                LoadsPerMonth load;

                for ( int i = 0; i < months; i++ )
                {
                    csm.FromDate = new DateTime( DateTime.Now.AddMonths( -i ).Year, DateTime.Now.AddMonths( -i ).Month, 1 );
                    csm.ToDate = new DateTime( csm.FromDate.Value.Year, csm.FromDate.Value.Month, DateTime.DaysInMonth( csm.FromDate.Value.Year, csm.FromDate.Value.Month ) );

                    load = service.LoadsPerMonth( csm );

                    load.MonthYear = csm.FromDate.Value.Year;
                    load.MonthNumber = csm.FromDate.Value.Month;
                    load.MonthName = $"{csm.FromDate.Value:MMMM} {csm.FromDate.Value:yyyy}";

                    loads.Add( load );
                }

                //Yearly cumulative

                csm.FromDate = new DateTime( DateTime.Now.Year, 1, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                load = service.LoadsPerMonth( csm );

                load.MonthNumber = int.MaxValue;
                load.MonthYear = int.MaxValue;
                load.MonthName = csm.FromDate.Value.ToString( "yyyy" );

                loads.Add( load );

                loads = loads.OrderBy( o => o.MonthYear ).ThenBy( o => o.MonthNumber ).ToList();

                return PartialView( "_LoadsPerMonth", loads );
            }
        }

        //
        // POST || GET: /DashBoard/NumberOfPalletsManaged
        public ActionResult NumberOfPalletsManaged( CustomSearchModel csm, bool givedata = false )
        {
            using ( ClientLoadService service = new ClientLoadService() )
            {
                DateTime fromDate = csm.FromDate ?? new DateTime( DateTime.Now.AddMonths( -2 ).Year, DateTime.Now.AddMonths( -2 ).Month, 1 );
                DateTime toDate = csm.ToDate ?? new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                if ( givedata )
                {
                    csm.ToDate = toDate;
                    csm.FromDate = fromDate;

                    List<ClientLoadCustomModel> numberOfPalletsManaged = service.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_NumberOfPalletsManagedData", numberOfPalletsManaged );
                }

                List<NumberOfPalletsManaged> loads = new List<NumberOfPalletsManaged>();

                int months = ( ( toDate.Year - fromDate.Year ) * 12 ) + toDate.Month - fromDate.Month;

                months = months <= 0 ? 1 : months + 1;

                NumberOfPalletsManaged load;

                for ( int i = 0; i < months; i++ )
                {
                    csm.FromDate = new DateTime( DateTime.Now.AddMonths( -i ).Year, DateTime.Now.AddMonths( -i ).Month, 1 );
                    csm.ToDate = new DateTime( csm.FromDate.Value.Year, csm.FromDate.Value.Month, DateTime.DaysInMonth( csm.FromDate.Value.Year, csm.FromDate.Value.Month ) );

                    load = service.NumberOfPalletsManaged( csm );

                    load.MonthYear = csm.FromDate.Value.Year;
                    load.MonthNumber = csm.FromDate.Value.Month;
                    load.MonthName = $"{csm.FromDate.Value:MMMM} {csm.FromDate.Value:yyyy}";

                    loads.Add( load );
                }

                loads = loads.OrderBy( o => o.MonthYear ).ThenBy( o => o.MonthNumber ).ToList();

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
                DateTime fromDate = csm.FromDate ?? new DateTime( DateTime.Now.AddMonths( -2 ).Year, DateTime.Now.AddMonths( -2 ).Month, 1 );
                DateTime toDate = csm.ToDate ?? new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                if ( givedata )
                {
                    csm.ToDate = toDate;
                    csm.FromDate = fromDate;

                    csm.HasAuthorisationCode = true;

                    List<ClientAuthorisationCustomModel> authCodes = aservice.List1( new PagingModel() { Take = int.MaxValue }, csm );

                    return PartialView( "_AuthorisationCodesData", authCodes );
                }

                List<AuthorisationCodes> auths = new List<AuthorisationCodes>();

                int months = ( ( toDate.Year - fromDate.Year ) * 12 ) + toDate.Month - fromDate.Month;

                months = months <= 0 ? 1 : months + 1;

                AuthorisationCodes auth;

                for ( int i = 0; i < months; i++ )
                {
                    csm.FromDate = new DateTime( DateTime.Now.AddMonths( -i ).Year, DateTime.Now.AddMonths( -i ).Month, 1 );
                    csm.ToDate = new DateTime( csm.FromDate.Value.Year, csm.FromDate.Value.Month, DateTime.DaysInMonth( csm.FromDate.Value.Year, csm.FromDate.Value.Month ) );

                    auth = clservice.AuthorisationCodesPerMonth( csm );

                    auth.MonthYear = csm.FromDate.Value.Year;
                    auth.MonthNumber = csm.FromDate.Value.Month;
                    auth.MonthName = $"{csm.FromDate.Value:MMMM} {csm.FromDate.Value:yyyy}";

                    auths.Add( auth );
                }

                //Yearly cumulative

                csm.FromDate = new DateTime( DateTime.Now.Year, 1, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                auth = clservice.AuthorisationCodesPerMonth( csm );

                auth.MonthYear = int.MaxValue;
                auth.MonthNumber = int.MaxValue;
                auth.MonthName = csm.FromDate.Value.ToString( "yyyy" );

                auths.Add( auth );

                auths = auths.OrderBy( o => o.MonthYear ).ThenBy( o => o.MonthNumber ).ToList();

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
            DateTime fromDate = csm.FromDate ?? new DateTime( DateTime.Now.AddMonths( -2 ).Year, DateTime.Now.AddMonths( -2 ).Month, 1 );
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

                months = months <= 0 ? 1 : months + 1;

                NumberOfDisputes dispute;

                for ( int i = 0; i < months; i++ )
                {
                    csm.FromDate = new DateTime( DateTime.Now.AddMonths( -i ).Year, DateTime.Now.AddMonths( -i ).Month, 1 );
                    csm.ToDate = new DateTime( csm.FromDate.Value.Year, csm.FromDate.Value.Month, DateTime.DaysInMonth( csm.FromDate.Value.Year, csm.FromDate.Value.Month ) );

                    dispute = service.NumberOfDisputes( csm );

                    dispute.MonthYear = csm.FromDate.Value.Year;
                    dispute.MonthNumber = csm.FromDate.Value.Month;
                    dispute.MonthName = $"{csm.FromDate.Value:MMMM} {csm.FromDate.Value:yyyy}";

                    disputes.Add( dispute );
                }

                //Yearly cumulative

                csm.FromDate = new DateTime( DateTime.Now.Year, 1, 1 );
                csm.ToDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month ) );

                dispute = service.NumberOfDisputes( csm );

                dispute.MonthYear = int.MaxValue;
                dispute.MonthNumber = int.MaxValue;
                dispute.MonthName = csm.FromDate.Value.ToString( "yyyy" );

                disputes.Add( dispute );

                disputes = disputes.OrderBy( o => o.MonthYear ).ThenBy( o => o.MonthNumber ).ToList();

                return PartialView( "_NumberOfDisputes", disputes );
            }
        }

        #endregion
    }
}
