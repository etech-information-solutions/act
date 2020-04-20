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

     

        #endregion

        #region Uploads and Imports


        public ActionResult CompanyFiles(string objId, string objType)
        {
            ObjectDocumentsViewModel model = new ObjectDocumentsViewModel();
            if (!string.IsNullOrEmpty(objType) && !string.IsNullOrEmpty(objId))
            {

                using (DocumentService docservice = new DocumentService())
                {
                    List<Document> docList = new List<Document>();
                    int oId = int.Parse(objId);
                    switch (objType.ToLower())
                    {
                        case "client":
                            docList = docservice.List(oId, "Client");


                            break;
                        default:
                            break;
                    }
                    model.objDocuments = docList;
                    model.objId = oId;
                    model.objType = objType;
                }
            }
            return PartialView("_ListDocuments", model);
        }


        public JsonResult Upload(int? clientId = null)
        {
            FileViewModel nfv = new FileViewModel();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                //file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root

                // int clientid = 0;//clientId

                if (fileName != null)
                {
                    // Create folder
                    string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/{clientId}/");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string now = DateTime.Now.ToString("yyyyMMddHHmmss");
                    using (DocumentService dservice = new DocumentService())
                    {
                        Document doc = new Document()
                        {
                            ObjectId = clientId,//should be null for client adds, so we can update it later
                            ObjectType = "Client",
                            Status = (int)Status.Active,
                            Name = fileName,
                            Category = "Company Document",
                            Title = "Company Document",
                            Size = fileSize,
                            Description = "Company Document",
                            Type = mimeType,
                            Location = $"Client/{clientId}/{now}-{clientId}-{fileName}"
                        };

                        dservice.Create(doc);

                        string fullpath = Path.Combine(path, $"{now}-{clientId}-{fileName}");
                        file.SaveAs(fullpath);

                        nfv.Description = doc.Description;
                        nfv.Extension = mimeType;
                        nfv.Location = doc.Location;
                        nfv.Name = fileName;
                        nfv.Id = doc.Id;

                    }
                }
            }

            //return Json("Uploaded " + Request.Files.Count + " files");
            return Json(nfv, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        // GET: /Client/ImportChepLoad
        public ActionResult ImportChepLoad(HttpPostedFileBase postedFile)
        {
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

                    using (var sreader = new StreamReader(postedFile.InputStream))
                    using (SiteService siteService = new SiteService())
                    using (ChepLoadService service = new ChepLoadService())
                    using (TransactionScope scope = new TransactionScope())
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(',');
                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            int siteType = 1;

                            string strLatY = rows[3].ToString();
                            decimal latitudeY = 0;
                            try
                            {
                                decimal.TryParse(rows[3].ToString(), out latitudeY);
                                latitudeY = decimal.Round(latitudeY, 4);
                                strLatY = (latitudeY != 0 ? latitudeY.ToString() : strLatY);
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = string.Concat(rows[3].ToString(), " ", ex.Message);
                            }
                            string strLngX = rows[2].ToString();
                            decimal longitudeX = 0;
                            try
                            {
                                decimal.TryParse(rows[2].ToString(), out longitudeX);
                                longitudeX = decimal.Round(longitudeX, 4);
                                strLngX = (longitudeX != 0 ? longitudeX.ToString() : strLngX);
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = string.Concat(rows[3].ToString(), " ", ex.Message);
                            }

                            Site existingSite = null;
                            #region Validation
                            if (!string.IsNullOrEmpty(strLngX) && siteService.ExistByXYCoords(strLngX, strLatY))
                            {
                                //Notify($"Sorry, a Site with the same X Y Coordinates already exists \"{model.XCord}\" already exists!", NotificationType.Error);
                                //return View(model);

                                //rather than pass back to view, we will create the new site as a subsite of the existing site. 
                                //Get the existing site first
                                existingSite = siteService.GetByColumnsWhere("XCord", strLngX, "YCord", strLatY);
                   //             SiteID = existingSite.Id;//This is the existing site retrieved by mapping same X and Y coord, read that into the model.SiteId which makes the new site a child site
                                siteType = 2;//Mark teh site as a subsite by default
                            }

                            int regionId = 0;
                            if (!string.IsNullOrEmpty(rows[16].ToString()))
                            {
                                regionId = int.Parse(rows[16].ToString());
                            }
                            int provinceId = 0;
                            string provinceName = "";
                            if (!string.IsNullOrEmpty(rows[8].ToString()))
                            {
                                int.TryParse(rows[8].ToString(), out provinceId);
                                try
                                {
                                    if (provinceId > 0)
                                    {
                                        provinceName = ((Province)provinceId).GetDisplayText();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = string.Concat(rows[8].ToString(), " ", ex.Message);
                                }
                            }
                            #region Create Site
                            Site site = new Site()
                            {
                                Name = rows[0].ToString(),
                                Description = rows[1].ToString(),
                                XCord = strLngX,
                                YCord = strLatY,
                                Address = rows[4].ToString() + " " + rows[5].ToString() + " " + rows[6].ToString() + " " + rows[7].ToString() + " " + provinceName,
                                PostalCode = rows[7].ToString(),
                                ContactName = rows[10].ToString(),
                                ContactNo = rows[9].ToString(),
                                PlanningPoint = rows[11].ToString(),
                                SiteType = siteType,
                                AccountCode = rows[13].ToString(),
                                Depot = rows[14].ToString(),
                                SiteCodeChep = rows[15].ToString(),
                                Status = (int)Status.Pending,
                                RegionId = regionId,
                                FinanceContact = rows[17].ToString(),
                                FinanceContactNo = rows[18].ToString(),
                                ReceivingContact = rows[19].ToString(),
                                ReceivingContactNo = rows[20].ToString(),
                            };
                            //For Subsites
                   //         if (SiteID > 0)
                    //        {
                       //         site.SiteId = SiteID;
                    //        }
                   //         site = siteService.Create(site);
                            #endregion

                            #region Create Address (s)

                            if (!string.IsNullOrEmpty(rows[3].ToString()))
                            {
                                Address address = new Address()
                                {
                                    ObjectId = site.Id,
                                    ObjectType = "Site",
                                    Town = rows[6].ToString(),
                                    Status = (int)Status.Active,
                                    PostalCode = rows[7].ToString(),
                                    Type = (int)AddressType.Postal,
                                    Addressline1 = rows[4].ToString(),
                                    Addressline2 = rows[5].ToString(),
                                    Province = provinceId,
                                };
                                aservice.Create(address);
                            }

                            #endregion

                            //tie Client in Session to New Site
                            #region Add ClientSite
                            ClientSite csSite = new ClientSite()
                            {
                                ClientId = clientID,
                                SiteId = site.Id,
                                AccountingCode = site.AccountCode,
                                Status = (int)Status.Active
                            };
                            csService.Create(csSite);
                            #endregion

                        }
                        #endregion
                        scope.Complete();
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
            return RedirectToAction("ManageSites", "Client");
        }


        [HttpPost]
        // GET: /Client/ImportClientLoad
        public ActionResult ImportClientLoad(HttpPostedFileBase postedFile)
        {
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
                    //for subsites to load tehse sites under a main site
                    string sessSiteId = (Session["SiteId"] != null ? Session["SiteId"].ToString() : null);
                    int SiteID = (!string.IsNullOrEmpty(sessSiteId) ? int.Parse(sessSiteId) : 0);

                    using (var sreader = new StreamReader(postedFile.InputStream))
                    using (SiteService siteService = new SiteService())
                    using (ClientSiteService csService = new ClientSiteService())
                    using (TransactionScope scope = new TransactionScope())
                    using (AddressService aservice = new AddressService())
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(',');
                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            int siteType = 1;

                            string strLatY = rows[3].ToString();
                            decimal latitudeY = 0;
                            try
                            {
                                decimal.TryParse(rows[3].ToString(), out latitudeY);
                                latitudeY = decimal.Round(latitudeY, 4);
                                strLatY = (latitudeY != 0 ? latitudeY.ToString() : strLatY);
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = string.Concat(rows[3].ToString(), " ", ex.Message);
                            }
                            string strLngX = rows[2].ToString();
                            decimal longitudeX = 0;
                            try
                            {
                                decimal.TryParse(rows[2].ToString(), out longitudeX);
                                longitudeX = decimal.Round(longitudeX, 4);
                                strLngX = (longitudeX != 0 ? longitudeX.ToString() : strLngX);
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = string.Concat(rows[3].ToString(), " ", ex.Message);
                            }

                            Site existingSite = null;
                            #region Validation
                            if (!string.IsNullOrEmpty(strLngX) && siteService.ExistByXYCoords(strLngX, strLatY))
                            {
                                //Notify($"Sorry, a Site with the same X Y Coordinates already exists \"{model.XCord}\" already exists!", NotificationType.Error);
                                //return View(model);

                                //rather than pass back to view, we will create the new site as a subsite of the existing site. 
                                //Get the existing site first
                                existingSite = siteService.GetByColumnsWhere("XCord", strLngX, "YCord", strLatY);
                                SiteID = existingSite.Id;//This is the existing site retrieved by mapping same X and Y coord, read that into the model.SiteId which makes the new site a child site
                                siteType = 2;//Mark teh site as a subsite by default
                            }

                            int regionId = 0;
                            if (!string.IsNullOrEmpty(rows[16].ToString()))
                            {
                                regionId = int.Parse(rows[16].ToString());
                            }
                            int provinceId = 0;
                            string provinceName = "";
                            if (!string.IsNullOrEmpty(rows[8].ToString()))
                            {
                                int.TryParse(rows[8].ToString(), out provinceId);
                                try
                                {
                                    if (provinceId > 0)
                                    {
                                        provinceName = ((Province)provinceId).GetDisplayText();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = string.Concat(rows[8].ToString(), " ", ex.Message);
                                }
                            }
                            #region Create Site
                            Site site = new Site()
                            {
                                Name = rows[0].ToString(),
                                Description = rows[1].ToString(),
                                XCord = strLngX,
                                YCord = strLatY,
                                Address = rows[4].ToString() + " " + rows[5].ToString() + " " + rows[6].ToString() + " " + rows[7].ToString() + " " + provinceName,
                                PostalCode = rows[7].ToString(),
                                ContactName = rows[10].ToString(),
                                ContactNo = rows[9].ToString(),
                                PlanningPoint = rows[11].ToString(),
                                SiteType = siteType,
                                AccountCode = rows[13].ToString(),
                                Depot = rows[14].ToString(),
                                SiteCodeChep = rows[15].ToString(),
                                Status = (int)Status.Pending,
                                RegionId = regionId,
                                FinanceContact = rows[17].ToString(),
                                FinanceContactNo = rows[18].ToString(),
                                ReceivingContact = rows[19].ToString(),
                                ReceivingContactNo = rows[20].ToString(),
                            };
                            //For Subsites
                            if (SiteID > 0)
                            {
                                site.SiteId = SiteID;
                            }
                            site = siteService.Create(site);
                            #endregion

                            #region Create Address (s)

                            if (!string.IsNullOrEmpty(rows[3].ToString()))
                            {
                                Address address = new Address()
                                {
                                    ObjectId = site.Id,
                                    ObjectType = "Site",
                                    Town = rows[6].ToString(),
                                    Status = (int)Status.Active,
                                    PostalCode = rows[7].ToString(),
                                    Type = (int)AddressType.Postal,
                                    Addressline1 = rows[4].ToString(),
                                    Addressline2 = rows[5].ToString(),
                                    Province = provinceId,
                                };
                                aservice.Create(address);
                            }

                            #endregion

                            //tie Client in Session to New Site
                            #region Add ClientSite
                            ClientSite csSite = new ClientSite()
                            {
                                ClientId = clientID,
                                SiteId = site.Id,
                                AccountingCode = site.AccountCode,
                                Status = (int)Status.Active
                            };
                            csService.Create(csSite);
                            #endregion

                        }
                        #endregion
                        scope.Complete();
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
            return RedirectToAction("ManageSites", "Client");
        }



        #endregion
    }
}
