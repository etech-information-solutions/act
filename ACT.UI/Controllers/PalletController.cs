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
using OpenPop;
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
            //check if there are any viewbag messages from imports
            string msg = (Session["ImportMessage"] != null ? Session["ImportMessage"].ToString() : null);
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Message = msg;
                Notify(msg, NotificationType.Success);
            }
           
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<Client> clientList = new List<Client>();
            //TODO
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.ListCSM(new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;


            int total = 0;
            List<ChepLoadCustomModel> model = new List<ChepLoadCustomModel>();
            using (ChepLoadService service = new ChepLoadService())
            {
                if (clientId > 0)
                {
                    csm.ClientId = clientId;
                }

                model = service.ListCSM(pm, csm);
                        
            }
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_PoolingAgentData", paging);
        }

        // GET: Pallet/AddPoolingAgentData
        [Requires(PermissionTo.Create)]
        public ActionResult AddPoolingAgentData()
        {
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            ChepLoadViewModel model = new ChepLoadViewModel();
            model.PostingType = 3;
            return View(model);
        }

        // POST: Pallet/AddPoolingAgentData
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddPoolingAgentData(ChepLoadViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the item was not created. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                using (ChepLoadService gService = new ChepLoadService())
                using (TransactionScope scope = new TransactionScope())
                {
                    #region Create ClientLoadCustomModel
                    ChepLoad clientload = new ChepLoad()
                    {
                        LoadDate = model.LoadDate,
                        EffectiveDate = model.EffectiveDate,
                        NotifyDate = model.NotifyDate,
                        AccountNumber = model.AccountNumber,
                        ClientDescription = model.ClientDescription,
                        DeliveryNote = model.DeliveryNote,
                        ReferenceNumber = model.ReferenceNumber,
                        ReceiverNumber = model.ReceiverNumber,
                        Equipment = model.Equipment,
                        OriginalQuantity = model.OriginalQuantity,
                        NewQuantity = model.NewQuantity,
                        Status = (int)Status.Active,
                        CreatedOn = DateTime.Now,
                        ModfiedOn = DateTime.Now,
                        ModifiedBy = CurrentUser.Email
                };
                    gService.Create(clientload);
                    #endregion

                    scope.Complete();
                }

                Notify("The item was successfully created.", NotificationType.Success);
                return RedirectToAction("ClientData");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        // GET: Pallet/EditPoolingAgentData/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditPoolingAgentData(int id)
        {
            ChepLoad load;

            using (ChepLoadService service = new ChepLoadService())
            {
                load = service.GetById(id);


                if (load == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                ChepLoadViewModel model = new ChepLoadViewModel()
                {
                    LoadDate = load.LoadDate,
                    EffectiveDate = load.EffectiveDate,
                    NotifyDate = load.NotifyDate,
                    AccountNumber = load.AccountNumber,
                    ClientDescription = load.ClientDescription,
                    DeliveryNote = load.DeliveryNote,
                    ReferenceNumber = load.ReferenceNumber,
                    ReceiverNumber = load.ReceiverNumber,
                    Equipment = load.Equipment,
                    OriginalQuantity = load.OriginalQuantity,
                    NewQuantity = load.NewQuantity,
                    Status = (int)load.Status,
                    //DocumentCount = (int)load.DocumentCount,
                   // DocsList = ""
                };
                return View(model);
            }
        }

        // POST: Pallet/EditPoolingAgentData/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult EditPoolingAgentData(ChepLoadViewModel model, PagingModel pm, bool isstructure = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the selected Group was not updated. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                ChepLoad load;

                using (ChepLoadService service = new ChepLoadService())
                using (TransactionScope scope = new TransactionScope())
                {
                    load = service.GetById(model.Id);

                    #region Update Chep Load
                    load.LoadDate = model.LoadDate;
                    load.EffectiveDate = model.EffectiveDate;
                    load.NotifyDate = model.NotifyDate;
                    load.AccountNumber = model.AccountNumber;
                    load.ClientDescription = model.ClientDescription;
                    load.DeliveryNote = model.DeliveryNote;
                    load.ReferenceNumber = model.ReferenceNumber;
                    load.ReceiverNumber = model.ReceiverNumber;
                    load.Equipment = model.Equipment;
                    load.OriginalQuantity = model.OriginalQuantity;
                    load.NewQuantity = model.NewQuantity;
                    load.Status = (int)model.Status;
                    load.ModfiedOn = DateTime.Now;
                    load.ModifiedBy =  CurrentUser.Email;
                    service.Update(load);

                    #endregion


                    scope.Complete();
                }

                Notify("The selected Item details were successfully updated.", NotificationType.Success);

                return RedirectToAction("ClientData");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Pallet/DeletePoolingAgentData/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeletePoolingAgentData(ChepLoadViewModel model)
        {
            ChepLoad activeLoad;
            try
            {

                using (ChepLoadService service = new ChepLoadService())
                using (TransactionScope scope = new TransactionScope())
                {
                    activeLoad = service.GetById(model.Id);

                    if (activeLoad == null)
                    {
                        Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                        return PartialView("_AccessDenied");
                    }

                    activeLoad.Status = (((Status)activeLoad.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;
                    service.Update(activeLoad);
                    scope.Complete();

                }
                Notify("The selected item was successfully updated.", NotificationType.Success);
                return RedirectToAction("PoolingAgentData");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
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
            //check if there are any viewbag messages from imports
            string msg = (Session["ImportMessage"] != null ? Session["ImportMessage"].ToString() : null);           
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Message = msg;
                Notify(msg, NotificationType.Success);
            }
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<Client> clientList = new List<Client>();
            //TODO
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.ListCSM(new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;

            int total = 0;
            List<ClientLoadCustomModel> model = new List<ClientLoadCustomModel>();
            using (ClientLoadService service = new ClientLoadService())
            {
                if (clientId > 0)
                {
                    csm.ClientId = clientId;
                }

                model = service.ListCSM(pm, csm);

            }
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_ClientData", paging);
        }

        // GET: Pallet/AddClientLoad
        [Requires(PermissionTo.Create)]
        public ActionResult AddClientData()
        {
            ClientLoadViewModel model = new ClientLoadViewModel();
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it

            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            List<Transporter>transporterOptions = new List<Transporter>();
            using (TransporterService rservice = new TransporterService())
            {
                transporterOptions = rservice.List();
            }
            IEnumerable<SelectListItem> transporterOptionsDDL = transporterOptions.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.TradingName

            });
            ViewBag.TransporterOptions = transporterOptionsDDL;
            return View(model);
        }

        // POST: Pallet/AddClientData
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddClientData(ClientLoadViewModel model)
        {
            try
            {               
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the item was not created. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }
                string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
                int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
                ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it

                if (clientId > 0 || model.ClientId > 0)
                {
                    using (ClientLoadService gService = new ClientLoadService())
                    using (TransactionScope scope = new TransactionScope())
                    {
                        #region Create ClientLoadCustomModel
                        ClientLoad clientload = new ClientLoad()
                        {
                            ClientId = (model.ClientId>0?model.ClientId:clientId),
                            VehicleId = model.VehicleId,
                            TransporterId = model.TransporterId,
                            LoadDate = model.LoadDate,//DateTimeExtensions.formatImportDate(model.LoadDate),
                            LoadNumber = model.LoadNumber,
                            EffectiveDate = model.EffectiveDate,//DateTimeExtensions.formatImportDate(model.EffectiveDate),
                            NotifyeDate = model.NotifyeDate,//DateTimeExtensions.formatImportDate(model.NotifyeDate),
                            AccountNumber = model.AccountNumber,
                            ClientDescription = model.ClientDescription,
                            DeliveryNote = model.DeliveryNote,
                            ReferenceNumber = model.ReferenceNumber,
                            ReceiverNumber = model.ReceiverNumber,
                            Equipment = model.Equipment,
                            OriginalQuantity = model.OriginalQuantity,
                            NewQuantity = model.NewQuantity,
                            RetQuantity = model.RetQuantity,
                            //ReconcileDate = model.ReconcileDate,
                            //ReconcileInvoice = model.ReconcileInvoice,
                            PODNumber = model.PODNumber,
                            PCNNumber = model.PCNNumber,
                            PRNNumber = model.PRNNumber,
                            ARPMComments = model.ARPMComments,
                            ProvCode = model.ProvCode,
                            Status = (int)Status.Active,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = CurrentUser.Email
                        };
                        gService.Create(clientload);
                        #endregion

                        scope.Complete();
                    }

                    Notify("The item was successfully created.", NotificationType.Success);
                    return RedirectToAction("ClientData");
                } else
                {
                    ViewBag.Message = "No Client Accounted for";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        // GET: Pallet/EditClientData/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditClientData(int id)
        {
            ClientLoad load;

            using (ClientLoadService service = new ClientLoadService())
            {
                load = service.GetById(id);


                if (load == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                ClientLoadViewModel model = new ClientLoadViewModel()
                {
                    ClientId = load.ClientId,
                    VehicleId = load.VehicleId,
                    TransporterId = load.TransporterId,
                    LoadDate = load.LoadDate,
                    LoadNumber = load.LoadNumber,
                    EffectiveDate = load.EffectiveDate,
                    NotifyeDate = load.NotifyeDate,
                    AccountNumber = load.AccountNumber,
                    ClientDescription = load.ClientDescription,
                    DeliveryNote = load.DeliveryNote,
                    ReferenceNumber = load.ReferenceNumber,
                    ReceiverNumber = load.ReceiverNumber,
                    Equipment = load.Equipment,
                    OriginalQuantity = load.OriginalQuantity,
                    NewQuantity = load.NewQuantity,
                    RetQuantity = load.RetQuantity,
                    ReconcileDate = load.ReconcileDate,
                    ReconcileInvoice = load.ReconcileInvoice,
                    PODNumber = load.PODNumber,
                    PCNNumber = load.PCNNumber,
                    PRNNumber = load.PRNNumber,
                    ARPMComments = load.ARPMComments,
                    ProvCode = load.ProvCode,
                    Status = (int)load.Status
                   // DocumentCount = (int)load.DocumentCount,
                   // DocsList = ""
                };
                List<Transporter> transporterOptions = new List<Transporter>();
                using (TransporterService rservice = new TransporterService())
                {
                    transporterOptions = rservice.List();
                }
                IEnumerable<SelectListItem> transporterOptionsDDL = transporterOptions.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.TradingName

                });
                ViewBag.TransporterOptions = transporterOptionsDDL;
                List<Vehicle> vehicleOptions = new List<Vehicle>();
                using (VehicleService vservice = new VehicleService())
                {
                    vehicleOptions = vservice.ListByColumnsWhere("ObjectId", model.TransporterId, "Object", "Transporter");
                }
                IEnumerable<SelectListItem> vehicleOptionsDDL = vehicleOptions.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Registration

                });
                ViewBag.VehicleOptions = vehicleOptionsDDL;
                return View("EditClientData", model);
            }
        }

        // POST: Pallet/EditClientData/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult EditClientData(ClientLoadViewModel model, PagingModel pm, bool isstructure = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the selected Group was not updated. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                ClientLoad load;

                using (ClientLoadService service = new ClientLoadService())
                using (TransactionScope scope = new TransactionScope())
                {
                    load = service.GetById(model.Id);

                    #region Update Client Load

                    // Update Group
                    //group.Id = model.Id;
                        load.ClientId = model.ClientId;
                        load.VehicleId = model.VehicleId;
                        load.TransporterId = model.TransporterId;
                    load.LoadDate = model.LoadDate;// DateTimeExtensions.formatImportDate(model.LoadDate);
                        load.LoadNumber = model.LoadNumber;
                    load.EffectiveDate = model.EffectiveDate;// DateTimeExtensions.formatImportDate(model.EffectiveDate);
                    load.NotifyeDate = model.NotifyeDate;// DateTimeExtensions.formatImportDate(model.NotifyeDate);
                        load.AccountNumber = model.AccountNumber;
                        load.ClientDescription = model.ClientDescription;
                        load.DeliveryNote = model.DeliveryNote;
                        load.ReferenceNumber = model.ReferenceNumber;
                        load.ReceiverNumber = model.ReceiverNumber;
                        load.Equipment = model.Equipment;
                        load.OriginalQuantity = model.OriginalQuantity;
                        load.NewQuantity = model.NewQuantity;
                        load.RetQuantity = model.RetQuantity;
                        load.ReconcileDate = model.ReconcileDate;
                        load.ReconcileInvoice = model.ReconcileInvoice;
                        load.PODNumber = model.PODNumber;
                        load.PCNNumber = model.PCNNumber;
                        load.PRNNumber = model.PRNNumber;
                        load.ARPMComments = model.ARPMComments;
                        load.ProvCode = model.ProvCode;
                        load.Status = (int)model.Status;
                        load.ModifiedBy = CurrentUser.Email;
                        load.ModifiedOn = DateTime.Now;

                    service.Update(load);

                    #endregion


                    scope.Complete();
                }

                Notify("The selected Item details were successfully updated.", NotificationType.Success);

                return RedirectToAction("ClientData");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Pallet/DeleteClientData/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeleteClientData(ClientLoadCustomModel model)
        {
            ClientLoad activeLoad;
            try
            {

                using (ClientLoadService service = new ClientLoadService())
                using (TransactionScope scope = new TransactionScope())
                {
                    activeLoad = service.GetById(model.Id);

                    if (activeLoad == null)
                    {
                        Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                        return PartialView("_AccessDenied");
                    }

                    activeLoad.Status = (((Status)activeLoad.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;
                    service.Update(activeLoad);
                    scope.Complete();

                }
                Notify("The selected item was successfully updated.", NotificationType.Success);
                return RedirectToAction("ClientData");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
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
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
   //         model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<Client> clientList = new List<Client>();
            //TODO
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.ListCSM(new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;
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
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
           // model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<Client> clientList = new List<Client>();
            //TODO
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.ListCSM(new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;
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
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            //model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<Client> clientList = new List<Client>();
            //TODO
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.ListCSM(new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;
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
        public ActionResult GenerateDeliveryNote()
        {
            DeliveryNoteViewModel model = new DeliveryNoteViewModel();
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            model.ClientId = clientId;
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
           // model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<Client> clientList = new List<Client>();
            //TODO
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.ListCSM(new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;

            return PartialView("_GenerateDeliveryNote", model);
        }

        // POST: Pallet/GenerateDeliveryNote
        //[HttpPost]
        //[Requires(PermissionTo.Create)]
        //public ActionResult GenerateDeliveryNote(DeliveryNoteViewModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            Notify("Sorry, the item was not created. Please correct all errors and try again.", NotificationType.Error);

        //            return View(model);
        //        }

        //        //using (ChepLoadService gService = new ChepLoadService())
        //        //using (TransactionScope scope = new TransactionScope())
        //        //{
        //        //    #region Create ClientLoadCustomModel
        //        //    ChepLoad clientload = new ChepLoad()
        //        //    {
        //        //        LoadDate = model.LoadDate,
        //        //        EffectiveDate = model.EffectiveDate,
        //        //        NotifyDate = model.NotifyDate,
        //        //        AccountNumber = model.AccountNumber,
        //        //        ClientDescription = model.ClientDescription,
        //        //        DeliveryNote = model.DeliveryNote,
        //        //        ReferenceNumber = model.ReferenceNumber,
        //        //        ReceiverNumber = model.ReceiverNumber,
        //        //        Equipment = model.Equipment,
        //        //        OriginalQuantity = model.OriginalQuantity,
        //        //        NewQuantity = model.NewQuantity,
        //        //        Status = (int)Status.Active,
        //        //        CreatedOn = DateTime.Now,
        //        //        ModfiedOn = DateTime.Now,
        //        //        ModifiedBy = CurrentUser.Email
        //        //    };
        //        //    gService.Create(clientload);
        //        //    #endregion

        //        //    scope.Complete();
        //        //}

        //        Notify("The item was successfully created.", NotificationType.Success);
        //        return RedirectToAction("ClientData");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        return View();
        //    }
        //}


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
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
        //    model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
            List<Client> clientList = new List<Client>();
            //TODO
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.ListCSM(new PagingModel(), new CustomSearchModel() { ClientId = clientId, Status = Status.Active });
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;
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

        public JsonResult GetClientDetail(string clientId)
        {
            if (clientId != null && clientId != "")
            {
                Client client = null;

                using (ClientService bservice = new ClientService())
                {
                    client = bservice.GetById(int.Parse(clientId));
                    return Json(client, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetVehiclesForTransporter(string transId)
        {
            if (transId != null && transId != "")
            {
                List<Vehicle> sites = null;
                using (VehicleService service = new VehicleService())
                {

                    sites = service.ListByColumnsWhere("ObjectId", int.Parse(transId), "ObjectType", "Transporter");

                }
                //var jsonList = JsonConvert.SerializeObject(sites);
                //return Json(sites, JsonRequestBehavior.AllowGet);
                var data = JsonConvert.SerializeObject(sites, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }



        #endregion

        #region Uploads and Imports



        [HttpPost]
        // POSt: /Client/ImportChepLoad
        public ActionResult ImportChepLoad(HttpPostedFileBase postedFile)
        {
            string importMessage = "Pooling Agent Import Started\r\n";
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);

                //Validate uploaded file and return error.
                if (fileExtension != ".csv")
                {
                    ViewBag.Message = "Please select the csv file with .csv extension";
                    return View();
                }

                try
                {
                    string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
                    int clientID = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
                    int cnt = 0;
                    int cntCreated = 0;
                   
                    using (var sreader = new StreamReader(postedFile.InputStream))
                    using (ChepLoadService service = new ChepLoadService())
                    using (TransactionScope scope = new TransactionScope())
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(',');
                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            try { 
                                string[] rows = sreader.ReadLine().Split(',');

                                ChepLoad model = new ChepLoad();

                                if (!string.IsNullOrEmpty(rows[0]))
                                {
                                    model.LoadDate = (!string.IsNullOrEmpty(rows[0]) ? DateTimeExtensions.formatImportDate(rows[0]) : DateTime.Now);
                                    model.EffectiveDate = (!string.IsNullOrEmpty(rows[0]) ? DateTimeExtensions.formatImportDate(rows[1]) : DateTime.Now);
                                    model.NotifyDate = (!string.IsNullOrEmpty(rows[0]) ? DateTimeExtensions.formatImportDate(rows[2]) : DateTime.Now);
                                    model.DocketNumber = rows[3];
                                    model.PostingType = 2;//import - Transfer Customer to Customer - Out
                                    model.ClientDescription = rows[5];
                                    model.ReferenceNumber = rows[6];
                                    model.ReceiverNumber = rows[7];
                                    model.Equipment = rows[10];
                                    model.CreatedOn = DateTime.Now;
                                    model.ModfiedOn = DateTime.Now;
                                    model.OriginalQuantity = (Decimal.TryParse(rows[11], out decimal oQty)? oQty : 0);

                                    service.Create(model);
                                    importMessage += " Trading Partner: " + model.ClientDescription + " created at Id " + model.Id + "<br>";
                                    Notify("The PSP Configuration details were successfully updated.", NotificationType.Success);
                                    cntCreated++;
                                }
                            }              
                            catch (Exception ex)
                            {
                                importMessage += ex.Message + "<br>";
                                ViewBag.Message = ex.Message;
                            }
                            cnt++;
                        }
                        importMessage += " Records To Process: " + cnt + "<br>";
                        importMessage += " Records Processed: " + cntCreated + "<br>";
                        scope.Complete();
                        Session["ImportMessage"] = importMessage;
                    }

                }
                catch (Exception ex)
                {
                    importMessage += ex.Message + "<br>";
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {

            }
            Session["ImportMessage"] = importMessage;
            return RedirectToAction("PoolingAgentData", "Pallet");
        }


        [HttpPost]
        // GET: /Client/ImportClientLoad
        public ActionResult ImportClientLoad(HttpPostedFileBase postedFile)
        {
            string importMessage = "Client Load Import Started\r\n";
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);

                //Validate uploaded file and return error.
                if (fileExtension != ".csv")
                {
                    ViewBag.Message = "Please select the csv file with .csv extension";
                    return View();
                }

                try
                {
                    string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
                    int clientID = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
                    int cnt = 0;
                    int cntCreated = 0;

                    using (var sreader = new StreamReader(postedFile.InputStream))
                    using (ClientLoadService service = new ClientLoadService())
                    using (TransactionScope scope = new TransactionScope())
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(',');
                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            try
                            {
                                string[] rows = sreader.ReadLine().Split(',');

                            ClientLoad model = new ClientLoad();
                            int vehicleId = 0;
                            int transporterId = 0;
                            if (!string.IsNullOrEmpty(rows[12]))
                            {
                                using (VehicleService vehservice = new VehicleService())
                                {
                                    VehicleCustomModel vehicle = vehservice.ListCSM(new PagingModel(), new CustomSearchModel() { Query = rows[12], Status = Status.Active }).FirstOrDefault();
                                    if (vehicle != null)
                                    {
                                        vehicleId = vehicle.Id; //else remains 0
                                    }
                                }

                            }
                            if (!string.IsNullOrEmpty(rows[11]))
                            {
                                using (TransporterService transservice = new TransporterService())
                                {
                                    TransporterCustomModel trans = transservice.ListCSM(new PagingModel(), new CustomSearchModel() { Query = rows[11], Status = Status.Active }).FirstOrDefault();
                                    if (trans != null)
                                    {
                                        transporterId = trans.Id; //else remains 0
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(rows[0]) && vehicleId > 0 && transporterId > 0)
                            {
                                model.ClientId = clientID;
                                model.LoadDate = (!string.IsNullOrEmpty(rows[0]) ? DateTimeExtensions.formatImportDate(rows[0]) : DateTime.Now);
                                model.LoadNumber = rows[1];
                                model.AccountNumber = rows[2];                                
                                model.ClientDescription = rows[3];
                                model.ProvCode = rows[4];
                                model.PCNNumber = rows[5];
                                model.DeliveryNote = rows[6];
                                //model.PRNNumber = rows[7];
                                model.OriginalQuantity = (Decimal.TryParse(rows[7], out decimal oQty) ? oQty : 0);
                                model.RetQuantity = (Decimal.TryParse(rows[8], out decimal rQty) ? rQty : 0);
                                //model.RetQuantity = decimal.Parse(rows[9]);
                                model.ARPMComments = rows[10];
                                //some of the columns are malaligned but leaving it as is, I added 3 new columns
                                if (vehicleId>0)
                                    model.VehicleId = vehicleId;
                                if (transporterId > 0)
                                    model.TransporterId = transporterId;
                                model.CreatedOn = DateTime.Now;
                                model.ModifiedOn = DateTime.Now;
                                service.Create(model);
                                importMessage += " Customer: " + model.ClientDescription + " created at Id " + model.Id + "<br>";
                                cntCreated++;
                            } else
                                {

                                }
                            }
                            catch (Exception ex)
                            {
                                importMessage += ex.Message + "<br>";
                                ViewBag.Message = ex.Message;
                            }
                            cnt++;
                        }
                        importMessage += " Records To Process: " + cnt + "<br>";
                        importMessage += " Records Processed: " + cntCreated + "<br>";
                        scope.Complete();
                        
                    }

                }
                catch (Exception ex)
                {
                    importMessage += ex.Message + "<br>";
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {

            }
            Session["ImportMessage"] = importMessage;
            return RedirectToAction("ClientData", "Pallet");
        }


        //
        // Returns a general list of all active clients allowed in the current context to be selected from
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult ImportEmails(string pspId = null)
        {
            if (pspId != null)
            {
                using (PSPConfigService confservice = new PSPConfigService())
                {
                    PSPConfig conf = confservice.GetById(int.Parse(pspId));
                    if (conf.ImportEmailHost != null && conf.ImportEmailPort != null)
                    {
                        OpenPop.Pop3.Pop3Client pop3Client = new OpenPop.Pop3.Pop3Client();
                        pop3Client.Connect(conf.ImportEmailHost, int.Parse(conf.ImportEmailPort), true);
                        pop3Client.Authenticate(conf.ImportEmailUsername, conf.ImportEmailPassword, OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);

                        int count = pop3Client.GetMessageCount();
                        List<EmailCustomModel> Emails = new List<EmailCustomModel>();
                        int counter = 0;
                        for (int i = count; i >= 1; i--)
                        {
                            OpenPop.Mime.Message message = pop3Client.GetMessage(i);
                            EmailCustomModel email = new EmailCustomModel()
                            {
                                MessageNumber = i,
                                Subject = message.Headers.Subject,
                                DateSent = message.Headers.DateSent,
                                From = string.Format("<a href = 'mailto:{1}'>{0}</a>", message.Headers.From.DisplayName, message.Headers.From.Address),
                            };
                            OpenPop.Mime.MessagePart body = message.FindFirstHtmlVersion();
                            if (body != null)
                            {
                                email.Body = body.GetBodyAsText();
                            }
                            else
                            {
                                body = message.FindFirstPlainTextVersion();
                                if (body != null)
                                {
                                    email.Body = body.GetBodyAsText();
                                }
                            }
                            List<OpenPop.Mime.MessagePart> attachments = message.FindAllAttachments();

                            foreach (OpenPop.Mime.MessagePart attachment in attachments)
                            {
                                email.Attachments.Add(new AttachmentCustomModel
                                {
                                    FileName = attachment.FileName,
                                    ContentType = attachment.ContentType.MediaType,
                                    Content = attachment.Body
                                });
                            }
                            Emails.Add(email);
                            counter++;
                            if (counter > 2)
                            {
                                break;
                            }
                        }
                        // Session["Pop3Client"] = pop3Client;
                        //List<Client> model = new List<Client>();
                        //PagingModel pm = new PagingModel();
                        //CustomSearchModel csm = new CustomSearchModel();

                        //using (ClientService service = new ClientService())
                        //{
                        //    pm.Sort = pm.Sort ?? "ASC";
                        //    pm.SortBy = pm.SortBy ?? "Name";
                        //    csm.Status = Status.Active;
                        //    csm.Status = Status.Active;

                        //    model = service.ListCSM(pm, csm);
                        //}
                        //if (model.Any())
                        //{
                        //    IEnumerable<SelectListItem> clientDDL = model.Select(c => new SelectListItem
                        //    {
                        //        Value = c.Id.ToString(),
                        //        Text = c.CompanyName

                        //    });
                    }
                }

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }


        #endregion
    }
}
