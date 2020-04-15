using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.Data.Models;
using ACT.UI.Models;
using ACT.UI.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json;
namespace ACT.UI.Controllers
{
    public class PalletController : BaseController
    {
        // GET: Pallet
        public ActionResult Index()
        {
            return View();
        }

        #region Pallet PoolingAgentData
        //
        // GET: /Pallet/PoolingAgentData
        public ActionResult PoolingAgentData(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "PoolingAgentData";

                return PartialView("_PoolingAgentDataCustomSearch", new CustomSearchModel("PoolingAgentData"));
            }
            ViewBag.ViewName = "PoolingAgentData";

            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_PoolingAgentData", paging);
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet ClientData
        //
        // GET: /Pallet/ClientData
        public ActionResult ClientData(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "ClientData";

                return PartialView("_ClientDataCustomSearch", new CustomSearchModel("ClientData"));
            }
            ViewBag.ViewName = "ClientData";

            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_ClientData", paging);
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet ReconcileLoads
        //
        // GET: /Pallet/ReconcileLoads
        public ActionResult ReconcileLoads(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "ReconcileLoads";

                return PartialView("_ReconcileLoadsCustomSearch", new CustomSearchModel("ReconcileLoads"));
            }
            ViewBag.ViewName = "ReconcileLoads";

            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_ReconcileLoads", paging);
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet ReconcileInvoice
        //
        // GET: /Pallet/ReconcileInvoice
        public ActionResult ReconcileInvoice(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "ReconcileInvoice";

                return PartialView("_ReconcileInvoiceCustomSearch", new CustomSearchModel("ReconcileInvoice"));
            }
            ViewBag.ViewName = "ReconcileInvoice";

            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_ReconcileInvoice", paging);
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet Exceptions
        //
        // GET: /Pallet/Exceptions
        public ActionResult Exceptions(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "Exceptions";

                return PartialView("_ExceptionsCustomSearch", new CustomSearchModel("Exceptions"));
            }
            ViewBag.ViewName = "Exceptions";

            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_Exceptions", paging);
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet GenerateDeliveryNote
        //
        // GET: /Pallet/GenerateDeliveryNote
        public ActionResult GenerateDeliveryNote(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "GenerateDeliveryNote";

                return PartialView("_GenerateDeliveryNoteCustomSearch", new CustomSearchModel("GenerateDeliveryNote"));
            }
            ViewBag.ViewName = "GenerateDeliveryNote";

            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_GenerateDeliveryNote", paging);
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region Pallet Disputes
        //
        // GET: /Pallet/Disputes
        public ActionResult Disputes(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "Disputes";

                return PartialView("_DisputesCustomSearch", new CustomSearchModel("Disputes"));
            }
            ViewBag.ViewName = "Disputes";

            int total = 0;

            List<Site> model = new List<Site>();
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_Disputes", paging);
        }
        #endregion

        //-------------------------------------------------------------------------------------

        #region General


        #endregion

        //-------------------------------------------------------------------------------------

        #region APIs


        #endregion
    }
}
