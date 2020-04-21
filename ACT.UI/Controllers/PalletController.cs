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
            if (!string.IsNullOrEmpty(msg)) {
                ViewBag.Message = msg;
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

        // GET: Pallet/AddChepLoad
        [Requires(PermissionTo.Create)]
        public ActionResult AddChepLoad()
        {
            ChepLoadCustomModel model = new ChepLoadCustomModel();
            //Posting type is the type or method it is entered into 1 Email, 2 Imported,  3 Added
            model.PostingType = 3;

             return View(model);
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
            }
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            ViewBag.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
       //     model.ContextualMode = (clientId > 0 ? true : false); //Whether a client is specific or not and the View can know about it
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
        public ActionResult AddClientLoad()
        {
            ClientLoadCustomModel model = new ClientLoadCustomModel();
            return View(model);
        }

        //// POST: Client/AddGroup
        //[HttpPost]
        //[Requires(PermissionTo.Create)]
        //public ActionResult AddGroup(GroupViewModel model)
        //{
        //    try
        //    {

        //        if (!ModelState.IsValid)
        //        {
        //            Notify("Sorry, the Group was not created. Please correct all errors and try again.", NotificationType.Error);

        //            return View(model);
        //        }

        //        using (GroupService gService = new GroupService())
        //        using (ClientGroupService cgservice = new ClientGroupService())
        //        using (TransactionScope scope = new TransactionScope())
        //        {
        //            #region Create Group
        //            Group group = new Group()
        //            {
        //                Name = model.Name,
        //                Description = model.Description,
        //                Status = (int)Status.Active
        //            };
        //            group = gService.Create(group);
        //            #endregion

        //            #region Create Group Client Links
        //            if (!string.IsNullOrEmpty(model.GroupClientList))
        //            {
        //                String[] clientList = model.GroupClientList.Split(',');
        //                string lastId = "0";
        //                foreach (string itm in clientList)
        //                {
        //                    //test to see if its not been added before
        //                    ClientGroup checkCG = cgservice.GetByColumnsWhere("ClientId", int.Parse(itm), "GroupId", group.Id);

        //                    if (!string.IsNullOrEmpty(itm) && itm != lastId && checkCG == null)
        //                    {
        //                        ClientGroup client = new ClientGroup()
        //                        {
        //                            ClientId = int.Parse(itm),
        //                            GroupId = group.Id,
        //                            Status = (int)Status.Active
        //                        };
        //                        cgservice.Create(client);
        //                    }
        //                    lastId = itm;
        //                }
        //            }
        //            #endregion

        //            scope.Complete();
        //        }

        //        Notify("The Group was successfully created.", NotificationType.Success);
        //        return RedirectToAction("ClientGroups");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        return View();
        //    }
        //}


        //// GET: Client/EditGroupGet/5
        //[Requires(PermissionTo.Edit)]
        //public ActionResult EditGroupGet(int id)
        //{
        //    Group group;

        //    using (GroupService service = new GroupService())
        //    {
        //        group = service.GetById(id);


        //        if (group == null)
        //        {
        //            Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

        //            return PartialView("_AccessDenied");
        //        }

        //        GroupViewModel model = new GroupViewModel()
        //        {
        //            Id = group.Id,
        //            Name = group.Name,
        //            Description = group.Description,
        //            Status = (int)group.Status,
        //            EditMode = true
        //        };
        //        return View("EditGroup", model);
        //    }
        //}

        //// POST: Client/EditGroup/5
        //[Requires(PermissionTo.Edit)]
        //public ActionResult EditGroup(GroupViewModel model, PagingModel pm, bool isstructure = false)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            Notify("Sorry, the selected Group was not updated. Please correct all errors and try again.", NotificationType.Error);

        //            return View(model);
        //        }

        //        Group group;

        //        using (GroupService service = new GroupService())
        //        using (TransactionScope scope = new TransactionScope())
        //        {
        //            group = service.GetById(model.Id);

        //            #region Update Group

        //            // Update Group
        //            //group.Id = model.Id;
        //            group.Name = model.Name;
        //            group.Description = model.Description;
        //            group.Status = (int)model.Status;

        //            service.Update(group);

        //            #endregion


        //            scope.Complete();
        //        }

        //        Notify("The selected Group details were successfully updated.", NotificationType.Success);

        //        return RedirectToAction("ClientGroups");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        return View();
        //    }
        //}

        // POST: Pallet/DeleteClientLoad/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeleteClientLoad(ClientLoadCustomModel model)
        {
            ClientLoadCustomModel activeLoad;
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
        public ActionResult GenerateDeliveryNote(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "GenerateDeliveryNote";

                return PartialView("_GenerateDeliveryNoteCustomSearch", new CustomSearchModel("GenerateDeliveryNote"));
            }
            ViewBag.ViewName = "GenerateDeliveryNote";
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
        public JsonResult SetSite(string SiteId)
        {
            if (SiteId != null)
            {
                Session["SiteId"] = SiteId;
                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetClientSite(string SiteId)
        {
            if (SiteId != null)
            {
                int clientId = 0;
                if (int.Parse(SiteId) > 0)
                {
                    using (SiteService service = new SiteService())
                    {
                        clientId = service.GetClientBySite(int.Parse(SiteId));
                    }
                    Session["ClientId"] = clientId;
                }
                Session["SiteId"] = SiteId;
                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
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
                            string[] rows = sreader.ReadLine().Split(',');

                            ChepLoadCustomModel model = new ChepLoadCustomModel();
                            
                            if (string.IsNullOrEmpty(rows[0]))
                            {
                                model.LoadDate = DateTimeExtensions.formatImportDate(rows[0]);
                                model.EffectiveDate = DateTimeExtensions.formatImportDate(rows[1]);
                                model.NotifyDate = DateTimeExtensions.formatImportDate(rows[2]);
                                model.DocketNumber = rows[3];
                                model.PostingType = 2;//import - Transfer Customer to Customer - Out
                                model.ClientDescription = rows[5];
                                model.ReferenceNumber = rows[6];
                                model.ReceiverNumber = rows[7];
                                model.Equipment = rows[9];
                                model.OriginalQuantity = decimal.Parse(rows[10]);

                                service.Create(model);
                                importMessage += " Trading Partner: " + model.ClientDescription + " created at Id " + model.Id + "\r\n";
                                cntCreated++;
                            }
                            cnt++;
                        }
                        importMessage += " Records To Process: " + cnt + "\r\n";
                        importMessage += " Records Processed: " + cntCreated + "\r\n";
                        scope.Complete();
                        Session["ImportMessage"] = importMessage;
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {

            }
            return RedirectToAction("PoolingAgentData", "Pallet");
        }


        [HttpPost]
        // GET: /Client/ImportClientLoad
        public ActionResult ImportClientLoad(HttpPostedFileBase postedFile)
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
                            string[] rows = sreader.ReadLine().Split(',');

                            ChepLoadCustomModel model = new ChepLoadCustomModel();

                            if (string.IsNullOrEmpty(rows[0]))
                            {
                                model.LoadDate = DateTimeExtensions.formatImportDate(rows[0]);
                                model.EffectiveDate = DateTimeExtensions.formatImportDate(rows[1]);
                                model.NotifyDate = DateTimeExtensions.formatImportDate(rows[2]);
                                model.DocketNumber = rows[3];
                                model.PostingType = 2;//import - Transfer Customer to Customer - Out
                                model.ClientDescription = rows[5];
                                model.ReferenceNumber = rows[6];
                                model.ReceiverNumber = rows[7];
                                model.Equipment = rows[9];
                                model.OriginalQuantity = decimal.Parse(rows[10]);

                                service.Create(model);
                                importMessage += " Trading Partner: " + model.ClientDescription + " created at Id " + model.Id + "\r\n";
                                cntCreated++;
                            }
                            cnt++;
                        }
                        importMessage += " Records To Process: " + cnt + "\r\n";
                        importMessage += " Records Processed: " + cntCreated + "\r\n";
                        scope.Complete();
                        Session["ImportMessage"] = importMessage;
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {

            }
            return RedirectToAction("PoolingAgentData", "Pallet");
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
