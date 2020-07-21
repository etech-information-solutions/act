
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.UI.Mvc;

namespace ACT.UI.Controllers
{
    [Requires( PermissionTo.View, PermissionContext.Finance )]
    public class FinanceController : BaseController
    {
        // GET: Finance
        public ActionResult Index()
        {
            return View();
        }


        #region Exports

        //
        // GET: /Finance/Export
        public FileContentResult Export( PagingModel pm, CustomSearchModel csm, string type = "billing" )
        {
            string csv = "";
            string filename = string.Format( "{0}-{1}.csv", type.ToUpperInvariant(), DateTime.Now.ToString( "yyyy_MM_dd_HH_mm" ) );

            pm.Skip = 0;
            pm.Take = int.MaxValue;

            switch ( type )
            {
                case "billing":

                    #region Billing

                    csv = string.Format( "Statement Date,Statement Number,PSP Name,Product Name,Statement Amount,Invoice Amount,Tax Amount,Payment Date,Reference Number,Nominated Account {0}", Environment.NewLine );

                    using ( PSPBillingService bservice = new PSPBillingService() )
                    {
                        List<PSPBillingCustomModel> billing = bservice.List1( pm, csm );

                        if ( billing != null && billing.Any() )
                        {
                            foreach ( PSPBillingCustomModel item in billing )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} {12}",
                                                    csv,
                                                    item.StatementDate,
                                                    item.StatementNumber,
                                                    item.CreatedOn,
                                                    item.PSPName,
                                                    item.ProductName,
                                                    item.PaymentAmount,
                                                    item.InvoiceAmount,
                                                    item.TaxAmount,
                                                    item.PaymentDate,
                                                    item.ReferenceNumber,
                                                    item.NominatedAccount,
                                                    Environment.NewLine );
                            }
                        }
                    }

                    #endregion

                    break;
            }

            return File( new System.Text.UTF8Encoding().GetBytes( csv ), "text/csv", filename );
        }

        #endregion

        #region Partial Views

        //
        // GET: /Finance/Billing
        public ActionResult Billing( PagingModel pm, CustomSearchModel csm, bool givecsm = false )
        {
            using ( PSPBillingService bservice = new PSPBillingService() )
            {
                if ( givecsm )
                {
                    ViewBag.ViewName = "Billing";

                    return PartialView( "_BillingCustomSearch", new CustomSearchModel( "Billing" ) );
                }

                List<PSPBillingCustomModel> model = bservice.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : bservice.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_Billing", paging );
            }
        }

        #endregion
    }
}
