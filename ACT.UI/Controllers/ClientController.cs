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
    public class ClientController : BaseController
    {
        // GET: Client
        public ActionResult Index()
        {
            return View();
        }



        #region Clients

        //
        // POST || GET: /Client/ClientList
        public ActionResult ClientList(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "ClientList";

                return PartialView("_ClientListCustomSearch", new CustomSearchModel("ClientList"));
            }
            int total = 0;

            List<Client> model = new List<Client>();
            List<ClientViewModel> viewModel = new List<ClientViewModel>();

            using (ClientService service = new ClientService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";
                if (CurrentUser.PSPs.Count > 0)
                {
                    model = service.List1(pm,csm);//service.GetClientsByPSP(CurrentUser.PSPs.FirstOrDefault().Id);
                    foreach (Client cl in model)
                    {
                        ClientViewModel vm = new ClientViewModel()
                        {
                            Id = cl.Id,
                            CompanyName = cl.CompanyName,
                            CompanyRegistrationNumber = cl.CompanyRegistrationNumber,
                            TradingAs = cl.TradingAs,
                            Description = cl.Description,
                            VATNumber = cl.VATNumber,
                            ContactNumber = cl.ContactNumber,
                            ContactPerson = cl.ContactPerson,
                            AdminEmail = cl.AdminEmail,
                            FinancialPerson = cl.FinancialPerson,
                            Email = cl.Email,
                            Status = cl.Status,
                        };
                        viewModel.Add(vm);
                    }
                }
                else
                {
                    model = null;
                }

                // var testModel = service.ListByColumn(null, "CompanyRegistrationNumber", "123456");
                total = (viewModel.Count < pm.Take && pm.Skip == 0) ? viewModel.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create(viewModel, total, pm.Skip, pm.Take, pm.Page);


            return PartialView("_ClientList", paging);
        }

        //
        // GET: /Client/ClientDetails/5
        public ActionResult ClientDetails(int id, bool layout = true)
        {
            Client model = new Client();

            using (ClientService service = new ClientService())
            using (AddressService aservice = new AddressService())
            using (DocumentService dservice = new DocumentService())
            using (ClientKPIService kservice = new ClientKPIService())
            {
                model = service.GetById(id);
                if (model == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return RedirectToAction("Index");
                }
                Session["ClientId"] = id;
                Address address = aservice.Get(model.Id, "Client");

                List<Document> documents = dservice.List(model.Id, "Client");
                List<Document> logo = dservice.List(model.Id, "ClientLogo");
                ClientKPI kpi = kservice.GetByColumnWhere("ClientId", id);

                if (address != null)
                {
                    ViewBag.Address = address;
                }
                if (documents != null)
                {
                    ViewBag.Documents = documents;
                }
                if (logo != null)
                {
                    ViewBag.Logo = logo;
                }
                if(kpi != null)
                {
                    ViewBag.KPI = kpi;
                }
            }

            if (layout)
            {
                ViewBag.IncludeLayout = true;
            }

            return View(model);
        }

        // GET: Client/AddClient
        [Requires(PermissionTo.Create)]
        public ActionResult AddClient()
        {
            ClientViewModel model = new ClientViewModel() { EditMode = true };
            return View(model);
        }

        // POST: Client/Create
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddClient(ClientViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the Client was not created. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                using (ClientService service = new ClientService())
                using (PSPClientService pspclientservice = new PSPClientService())
                using (AddressService aservice = new AddressService())
                using (TransactionScope scope = new TransactionScope())
                using (DocumentService dservice = new DocumentService())
                using (ClientKPIService kpiservice = new ClientKPIService())                    
                // using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    #region Validation
                    if (!string.IsNullOrEmpty(model.CompanyRegistrationNumber) && service.ExistByCompanyRegistrationNumber(model.CompanyRegistrationNumber.Trim()))
                    {
                        // Bank already exist!
                        Notify($"Sorry, a Client with the Registration number \"{model.CompanyRegistrationNumber}\" already exists!", NotificationType.Error);

                        return View(model);
                    }
                    #endregion
                    #region Create Client
                    Client client = new Client()
                    {
                        Email = model.Email,
                        TradingAs = model.TradingAs,
                        VATNumber = model.VATNumber,
                        AdminEmail = model.Email,
                        CompanyName = model.CompanyName,
                        Description = model.CompanyName,
                        ContactPerson = model.ContactPerson,
                        ContactNumber = model.ContactNumber,
                        FinancialPerson = model.ContactPerson,
                        Status = (int)Status.Active,//model.Status,
                        ServiceRequired = (int)ServiceType.ManageOwnPallets,
                        //ServiceRequired = (int)model.ServiceRequired,
                        CompanyRegistrationNumber = model.CompanyRegistrationNumber
                    };
                    client = service.Create(client);
                    #endregion

                    #region Create Client PSP link
                    //int pspId = Session[ "UserPSP" ];
                    int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
                    PSPClient pClient = new PSPClient()
                    {
                        PSPId = pspId,
                        ClientId = client.Id,
                        Status = (int)Status.Active
                    };
                    pClient = pspclientservice.Create(pClient);
                    #endregion

                    #region Create Client Budget
                    //Moved to  seperate API calls to get and set from view
                    #endregion

                    #region Create Address (s)

                    if (model.Address != null)
                    {
                        Address address = new Address()
                        {
                            ObjectId = client.Id,
                            ObjectType = "Client",
                            Town = model.Address.Town,
                            Status = (int)Status.Active,
                            PostalCode = model.Address.PostCode,
                            Type = (int)model.Address.AddressType,
                            Addressline1 = model.Address.AddressLine1,
                            Addressline2 = model.Address.AddressLine2,
                            Province = (int)model.Address.Province,
                        };

                        aservice.Create(address);
                    }

                    #endregion

                    #region Any Uploads
                    //moved to clientlist
                    //if (model.CompanyFile != null)
                    //{
                    //    foreach (FileViewModel file in model.CompanyFile)
                    //    {
                    //        if (file.Name != null)
                    //        {
                    //            // Create folder
                    //            string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

                    //            if (!Directory.Exists(path))
                    //            {
                    //                Directory.CreateDirectory(path);
                    //            }

                    //            string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                    //            Document doc = new Document()
                    //            {
                    //                ObjectId = client.Id,
                    //                ObjectType = "Client",
                    //                Status = (int)Status.Active,
                    //                Name = file.Name,
                    //                Category = file.Name,
                    //                Title = file.File.FileName,
                    //                Size = file.File.ContentLength,
                    //                Description = file.File.FileName,
                    //                Type = Path.GetExtension(file.File.FileName),
                    //                Location = $"Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{file.File.FileName}"
                    //            };

                    //            dservice.Create(doc);

                    //            string fullpath = Path.Combine(path, $"{now}-{file.File.FileName}");
                    //            file.File.SaveAs(fullpath);
                    //        }
                    //    }
                    //}

                    #endregion

                    #region Any Logo Uploads
                    if (model.Logo != null && model.Logo.File != null)
                    {
                        // Create folder
                        string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/Logo/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                        Document doc = new Document()
                        {
                            ObjectId = client.Id,
                            ObjectType = "Client",
                            Status = (int)Status.Active,
                            Name = model.Logo.Name,
                            Category = model.Logo.Name,
                            Title = model.Logo.File.FileName,
                            Size = model.Logo.File.ContentLength,
                            Description = model.Logo.Description,
                            Type = Path.GetExtension(model.Logo.File.FileName),
                            Location = $"Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{model.Logo.File.FileName}"
                        };

                        dservice.Create(doc);

                        string fullpath = Path.Combine(path, $"{now}-{model.Logo.File.FileName}");
                        model.Logo.File.SaveAs(fullpath);
                    }
                    //if (model.Logo != null)
                    //{
                    //    //foreach (FileViewModel logo in model.Logo)
                    //    //{
                    //    if (model.Logo.Name != null)
                    //    {
                    //        // Create folder
                    //        string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/Logo/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

                    //        if (!Directory.Exists(path))
                    //        {
                    //            Directory.CreateDirectory(path);
                    //        }

                    //        string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                    //        Document doc = new Document()
                    //        {
                    //            ObjectId = client.Id,
                    //            ObjectType = "ClientLogo",
                    //            Status = (int)Status.Active,
                    //            Name = model.Logo.Name,
                    //            Category = model.Logo.Name,
                    //            Title = model.Logo.File.FileName,
                    //            Size = model.Logo.File.ContentLength,
                    //            Description = model.Logo.File.FileName,
                    //            Type = Path.GetExtension(model.Logo.File.FileName),
                    //            Location = $"Client/Logo/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{model.Logo.File.FileName}"
                    //        };

                    //        dservice.Create(doc);

                    //        string fullpath = Path.Combine(path, $"{now}-{model.Logo.File.FileName}");
                    //        model.Logo.File.SaveAs(fullpath);
                    //    }
                    //    //}
                    //}

                    #endregion

                    #region KPI
                    if (model.KPIDisputes > 0)
                    {
  
                        ClientKPI newKPI = new ClientKPI()
                        {
                            ClientId = client.Id,
                            Disputes = model.KPIDisputes,
                            OutstandingPallets = model.KPIOutstanding,
                            Passons = 0, //TODO: Fix
                            MonthlyCost = 0, //TODO: Fix
                            ResolveDays = model.KPIDaysToResolve,
                            OutstandingDays = model.KPIDaysOutstanding,
                            Status = (int)Status.Active
                        };
                        kpiservice.Create(newKPI);
                       
                    }
                    #endregion


                    scope.Complete();
                }
                Notify("The Client was successfully created.", NotificationType.Success);
                return RedirectToAction("ClientList");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: Client/EditClient/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditClient(int id)
        {
            Client client;

            using (ClientService service = new ClientService())
            using (AddressService aservice = new AddressService())
            using (DocumentService dservice = new DocumentService())
            using (EstimatedLoadService eservice = new EstimatedLoadService())
            using (ClientKPIService kpiservice = new ClientKPIService())
            {
                client = service.GetById(id);


                if (client == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }
                Session["ClientId"] = id;
                Address address = aservice.Get(client.Id, "Client");

                List<Document> logo = dservice.List(client.Id, "ClientLogo");
                List<Document> documents = dservice.List(client.Id, "Client");

                ClientKPI kpi = kpiservice.GetByColumnWhere("ClientId", client.Id);

                EstimatedLoad load = new EstimatedLoad();

                bool unverified = (client.Status == (int)PSPClientStatus.Unverified);

                if (unverified)
                {
                    load = eservice.Get(client.Id, "Client");
                }

                ClientViewModel model = new ClientViewModel()
                {
                    Id = client.Id,
                    CompanyName = client.CompanyName,
                    CompanyRegistrationNumber = client.CompanyRegistrationNumber,
                    TradingAs = client.TradingAs,
                    Description = client.Description,
                    VATNumber = client.VATNumber,
                    ContactNumber = client.ContactNumber,
                    ContactPerson = client.ContactPerson,
                    FinancialPerson = client.ContactPerson,
                    Email = client.Email,
                    AdminEmail = client.AdminEmail,
                    DeclinedReason = client.DeclinedReason,
                    //ServiceRequired = client.ServiceRequired,
                    //Status = (Status)client.Status,
                    Status = client.Status,
                    EditMode = true,
                    Address = new AddressViewModel()
                    {
                        EditMode = true,
                        Town = address?.Town,
                        Id = address?.Id ?? 0,
                        PostCode = address?.PostalCode,
                        AddressLine1 = address?.Addressline1,
                        AddressLine2 = address?.Addressline2,
                        Province = (address != null) ? (Province)address.Province : Province.All,
                        AddressType = (address != null) ? (AddressType)address.Type : AddressType.Postal,
                    },
                    KPIDisputes = (kpi!=null?kpi.Disputes:0),
                    KPIOutstanding = (kpi != null ? kpi.OutstandingPallets : 0),
                    KPIDaysToResolve = (kpi != null ? kpi.ResolveDays : 0),
                    KPIDaysOutstanding = (kpi != null ? kpi.OutstandingDays : 0),
                };
                if (logo != null && logo.Count > 0)
                {
                    List<FileViewModel> logofvm = new List<FileViewModel>();
                    foreach (Document doc in logo)
                    {
                        FileViewModel tfvm = new FileViewModel()
                        {
                            Id = doc.Id,
                            Location = doc.Location,
                            Name = doc.Name,
                            Description = doc.Description

                        };
                        logofvm.Add(tfvm);
                        //model.CompanyFile.Add(fvm);

                    }
                    model.Logo = logofvm.LastOrDefault();
                }
                List<Document> fvm = new List<Document>();
                if (documents != null && documents.Count > 0)
                {

                    foreach (Document doc in documents)
                    {
                        Document tfvm = new Document()
                        {
                            Id = doc.Id,
                            Location = doc.Location,
                            Name = doc.Name,
                            Description = doc.Description

                        };
                        fvm.Add(tfvm);


                    }
                    model.CompanyFile = fvm;
                }


                ViewBag.CompanyUploads = fvm;
                return View(model);
            }
        }

        // POST: Client/EditClient/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult EditClient(ClientViewModel model, PagingModel pm, bool isstructure = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the selected Client was not updated. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                Client client;

                using (ClientService service = new ClientService())
                using (AddressService aservice = new AddressService())
                using (TransactionScope scope = new TransactionScope())
                using (DocumentService dservice = new DocumentService())
                using (EstimatedLoadService eservice = new EstimatedLoadService())
                using (ClientKPIService kpiservice = new ClientKPIService())
                // using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    client = service.GetById(model.Id);
                    Address address = aservice.Get(client.Id, "Client");

                    if (client == null)
                    {
                        Notify("Sorry, that Client does not exist! Please specify a valid Role Id and try again.", NotificationType.Error);

                        return View(model);
                    }

                    // Address address = aservice.Get(client.Id, "Client");

                    List<Document> documents = dservice.List(client.Id, "Client");

                    List<Document> logos = dservice.List(client.Id, "ClientLogo");

                    

                    #region Validations

                    if (!string.IsNullOrEmpty(model.CompanyRegistrationNumber) && model.CompanyRegistrationNumber.Trim().ToLower() != client.CompanyRegistrationNumber.Trim().ToLower() && service.ExistByCompanyRegistrationNumber(model.CompanyRegistrationNumber.Trim()))
                    {
                        // Role already exist!
                        Notify($"Sorry, a Client with the Registration Number \"{model.CompanyRegistrationNumber} ({model.CompanyRegistrationNumber})\" already exists!", NotificationType.Error);

                        return View(model);
                    }

                    #endregion

                    #region Update Client

                    // Update Client
                    client.Id = model.Id;
                    client.CompanyName = model.CompanyName;
                    client.CompanyRegistrationNumber = model.CompanyRegistrationNumber;
                    client.TradingAs = model.TradingAs;
                    client.Description = model.Description;
                    client.VATNumber = model.VATNumber;
                    client.ContactNumber = model.ContactNumber;
                    client.ContactPerson = model.ContactPerson;
                    client.FinancialPerson = model.ContactPerson;
                    client.Email = model.Email;
                    client.AdminEmail = model.Email;
                    //client.DeclinedReason = model.DeclinedReason;
                    //client.ServiceRequired = model.ServiceRequired;
                    client.Status = (int)model.Status;
                    client.Status = model.Status;

                    service.Update(client);

                    #endregion



                    #region Create Address (s)

                    if (model.Address != null)
                    {
                        Address clientAddress = aservice.GetById(model.Address.Id);

                        if (clientAddress == null)
                        {
                            clientAddress = new Address()
                            {
                                ObjectId = model.Id,
                                ObjectType = "Client",
                                Town = model.Address.Town,
                                Status = (int)Status.Active,
                                PostalCode = model.Address.PostCode,
                                Type = (int)model.Address.AddressType,
                                Addressline1 = model.Address.AddressLine1,
                                Addressline2 = model.Address.AddressLine2,
                                Province = (int)model.Address.Province,
                            };

                            aservice.Create(clientAddress);
                        }
                        else
                        {
                            clientAddress.Town = model.Address.Town;
                            clientAddress.PostalCode = model.Address.PostCode;
                            clientAddress.Type = (int)model.Address.AddressType;
                            clientAddress.Addressline1 = model.Address.AddressLine1;
                            clientAddress.Addressline2 = model.Address.AddressLine2;
                            clientAddress.Province = (int)model.Address.Province;

                            aservice.Update(clientAddress);
                        }
                    }

                    #endregion

                    #region Any Uploads
                    /*
                    if (model.NewCompanyFile != null)
                    {
                        //foreach (FileViewModel file in model.CompanyFile)
                        //{
                        if (model.NewCompanyFile.Name != null)
                        {
                            // Create folder
                            string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                            Document doc = dservice.GetById(model.NewCompanyFile.Id);

                            if (doc != null)
                            {
                                // Disable this file...
                                doc.Status = (int)Status.Inactive;

                                dservice.Update(doc);
                            }

                            doc = new Document()
                            {
                                ObjectId = model.Id,
                                ObjectType = "Client",
                                Status = (int)Status.Active,
                                Name = model.NewCompanyFile.Name,
                                Category = model.NewCompanyFile.Name,
                                Title = model.NewCompanyFile.File.FileName,
                                Size = model.NewCompanyFile.File.ContentLength,
                                Description = model.NewCompanyFile.File.FileName,
                                Type = Path.GetExtension(model.NewCompanyFile.File.FileName),
                                Location = $"Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{model.NewCompanyFile.File.FileName}"

                            };

                            dservice.Create(doc);

                            string fullpath = Path.Combine(path, $"{now}-{model.NewCompanyFile.File.FileName}");
                            model.NewCompanyFile.File.SaveAs(fullpath);
                        }
                        //}
                        
                    }*/

                    #endregion
                   
                    #region Any Logos
                    if (model.Logo.File != null)
                    {
                        //foreach (FileViewModel logo in model.Logo)
                        //{
                        if (model.Logo.Name != null)
                        {
                            // Create folder
                            string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/Logo/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                            Document doc = dservice.GetById(model.Logo.Id);

                            if (doc != null)
                            {
                                // Disable this file...
                                doc.Status = (int)Status.Inactive;

                                dservice.Update(doc);
                            }

                            doc = new Document()
                            {
                                ObjectId = model.Id,
                                ObjectType = "ClientLogo",
                                Status = (int)Status.Active,
                                Name = model.Logo.Name,
                                Category = model.Logo.Name,
                                Title = model.Logo.File.FileName,
                                Size = model.Logo.File.ContentLength,
                                Description = "Logo",
                                Type = Path.GetExtension(model.Logo.File.FileName),
                                Location = $"Client/Logo/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{model.Logo.File.FileName}"

                            };

                            dservice.Create(doc);

                            string fullpath = Path.Combine(path, $"{now}-{model.Logo.File.FileName}");
                            model.Logo.File.SaveAs(fullpath);
                        }
                        //}
                    }

                    #endregion

                    #region ClientKPIs
                    if (model.KPIDisputes > 0)
                    {
                        List<ClientKPI> kpi = kpiservice.ListByColumnWhere("ClientID", model.Id);
                        if (kpi != null)
                        {
                            foreach (ClientKPI k in kpi)
                            {
                                k.OutstandingPallets = model.KPIOutstanding;
                                k.Disputes = model.KPIDisputes;
                                k.OutstandingDays = model.KPIDaysOutstanding;
                                k.ResolveDays = model.KPIDaysToResolve;

                                kpiservice.Update(k);
                            }
                        }
                        else
                        {
                            ClientKPI newKPI = new ClientKPI()
                            {
                                ClientId = client.Id,
                                Disputes = model.KPIDisputes,
                                OutstandingPallets = model.KPIOutstanding,
                                Passons = 0, //TODO: Fix
                                MonthlyCost = 0, //TODO: Fix
                                Status = (int)Status.Active
                            };
                            kpiservice.Create(newKPI);
                        }
                    }

                    #endregion

                    scope.Complete();
                }

                Notify("The selected Client details were successfully updated.", NotificationType.Success);

                return RedirectToAction("ClientList");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteClient/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeleteClient(ClientViewModel model)
        {
            Client client;
            try
            {

                using (ClientService service = new ClientService())
                using (TransactionScope scope = new TransactionScope())
                {
                    client = service.GetById(model.Id);

                    if (client == null)
                    {
                        Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                        return PartialView("_AccessDenied");
                    }

                    client.Status = (((Status)client.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;

                    service.Update(client);
                    scope.Complete();

                }
                Notify("The selected Client was successfully updated.", NotificationType.Success);
                return RedirectToAction("ClientList");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetClientBudgets(string clientId)
        {
            if (clientId != null && clientId != "")
            {
                List<ClientBudget> load = null;

                using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    load = bservice.ListByColumnWhere("ClientId", int.Parse(clientId));
                    return Json(load, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetClientBudget(string Id, string ClientId, string BudgetYear, string January, string February, string March, string April, string May, string June, string July, string August, string September, string October, string November, string December)
        {
            if (Id != null)
            {
                using (ClientBudgetService bservice = new ClientBudgetService())
                using (TransactionScope scope = new TransactionScope())
                {
                    //Collection of budgets or singular?
                    if (int.Parse(Id) > 0)
                    {
                        ClientBudget budget = bservice.GetById(int.Parse(Id));

                        budget.ClientId = int.Parse(ClientId);
                        budget.Status = (int)Status.Active;
                        budget.BudgetYear = DateTime.Now.Year;
                        budget.January = int.Parse(January);
                        budget.February = int.Parse(February);
                        budget.March = int.Parse(March);
                        budget.April = int.Parse(April);
                        budget.May = int.Parse(May);
                        budget.June = int.Parse(June);
                        budget.July = int.Parse(July);
                        budget.August = int.Parse(August);
                        budget.September = int.Parse(September);
                        budget.October = int.Parse(October);
                        budget.November = int.Parse(November);
                        budget.December = int.Parse(December);



                        bservice.Update(budget);
                    }
                    else
                    {
                        ClientBudget budget = new ClientBudget();
                        budget.ClientId = int.Parse(ClientId);
                        budget.BudgetYear = DateTime.Now.Year;
                        budget.January = int.Parse(January);
                        budget.February = int.Parse(February);
                        budget.March = int.Parse(March);
                        budget.April = int.Parse(April);
                        budget.May = int.Parse(May);
                        budget.June = int.Parse(June);
                        budget.July = int.Parse(July);
                        budget.August = int.Parse(August);
                        budget.September = int.Parse(September);
                        budget.October = int.Parse(October);
                        budget.November = int.Parse(November);
                        budget.December = int.Parse(December);

                        bservice.Create(budget);
                    }
                    scope.Complete();
                }

                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Manage Sites
        //
        // GET: /Client/ManageSites
        public ActionResult ManageSites(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "ManageSites";

                return PartialView("_ManageSitesCustomSearch", new CustomSearchModel("ManageSites"));
            }
            ViewBag.ViewName = "ManageSites";

            int total = 0;

            List<Site> model = new List<Site>();
            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientID = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            //string sessSiteId = (Session["SiteId"] != null ? Session["SiteId"].ToString() : null);
            //int SiteID = (!string.IsNullOrEmpty(sessSiteId) ? int.Parse(sessSiteId) : 0);
            using (SiteService service = new SiteService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List(pm, csm);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total(pm, csm);
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);
            List<Client> clientList = new List<Client>(); ;
            using (ClientService clientService = new ClientService())
            {
                if (clientID > 0)
                {
                    Client chosenClient = clientService.GetById(clientID);
                    clientList.Add(chosenClient);
                }
                else
                {
                    clientList = clientService.GetClientsByPSP(pspId);
                }
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;

            return PartialView("_ManageSites", paging);
        }

        //
        // GET: /Client/SiteDetails/5
        public ActionResult SiteDetails(int id, string sourceView = "ManageSites", bool layout = true)
        {
            Site site;
            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            List<Region> regionOptions = new List<Region>();

            using (SiteService service = new SiteService())
            using (RegionService rservice = new RegionService())
            using (AddressService aservice = new AddressService())
            {
                site = service.GetById(id);
               // regionOptions = rservice.ListByColumnWhere("PSPId", pspId);

                if (site == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                Address address = aservice.Get(site.Id, "Site");


                bool unverified = (site.Status == (int)PSPClientStatus.Unverified);

                Site model = new Site()
                {
                    Id = site.Id,
                    Name = site.Name,
                    Description = site.Description,
                    XCord = site.XCord,
                    YCord = site.YCord,
                    Address = site.Address,
                    PostalCode = site.PostalCode,
                    ContactName = site.ContactName,
                    ContactNo = site.ContactNo,
                    PlanningPoint = site.PlanningPoint,
                    SiteType = (int)site.SiteType,
                    AccountCode = site.AccountCode,
                    Depot = site.Depot,
                    SiteCodeChep = site.SiteCodeChep,
                    Status = (int)site.Status,
                   // EditMode = true,
                    RegionId = site.RegionId,
                   // SourceView = sourceView,
                    //RegionOptions = regionOptions,
                    //FullAddress = new AddressViewModel()
                    //{
                    //    EditMode = true,
                    //    Town = address?.Town,
                    //    Id = address?.Id ?? 0,
                    //    PostCode = address?.PostalCode,
                    //    AddressLine1 = address?.Addressline1,
                    //    AddressLine2 = address?.Addressline2,
                    //    Province = (address != null) ? (Province)address.Province : Province.All,
                    //    AddressType = (address != null) ? (AddressType)address.Type : AddressType.Postal,
                    //}

                };
                if (layout)
                {
                    ViewBag.IncludeLayout = true;
                }
                ViewBag.Address = address;
               // ViewBag.RegionOptions = regionOptions;
                return View(model);
            }
        }

        // GET: Client/AddSite
        [Requires(PermissionTo.Create)]
        public ActionResult AddSite(string sourceView)
        {

            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            //regionOptions = Model.RegionOptions.Where(r => r.PSPId == pspId).ToList();
            SiteViewModel model = new SiteViewModel() { EditMode = true, SourceView = sourceView };//, RegionOptions = regionOptions
            List<Region> regionOptions = new List<Region>();
            using (RegionService rservice = new RegionService()) {
                regionOptions = rservice.ListByColumnWhere("PSPId", pspId);

            IEnumerable<SelectListItem> regionDDL = regionOptions.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Description

            });
            ViewBag.RegionOptions = regionDDL;

            //if (sourceView == "ManageSites")
            //{
            //    return RedirectToAction("ManageSites");
            //}
            //else
            //{
            //    return RedirectToAction("SubSites");
            //}
            return View(model);
        }
        }


        // POST: Client/Site
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddSite(SiteViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the Site was not created. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }
                //get clientId from session as there has to be one for the add button to have appeared in teh first place, grab it and assign sites to this until it changes
               
                string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
                int clientID = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);

                string sessSiteId = (Session["SiteId"] != null ? Session["SiteId"].ToString() : null);
                int SiteID = (!string.IsNullOrEmpty(sessSiteId) ? int.Parse(sessSiteId) : 0);

                using (SiteService siteService = new SiteService())
                using (ClientSiteService csService = new ClientSiteService())
                using (TransactionScope scope = new TransactionScope())
                using (AddressService aservice = new AddressService())
                {
                    #region Validation
                    if (!string.IsNullOrEmpty(model.AccountCode) && siteService.ExistByAccountCode(model.AccountCode.Trim()))
                    {
                        // Bank already exist!
                        Notify($"Sorry, a Site with the Account number \"{model.AccountCode}\" already exists!", NotificationType.Error);

                        return View(model);
                    }
                    #endregion

                    #region Create Site
                    Site site = new Site()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        XCord = model.XCord,
                        YCord = model.YCord,
                        Address = model.FullAddress.AddressLine1 + " " + model.FullAddress.Town + " " + model.FullAddress.PostCode, //model.Address,
                        PostalCode = model.FullAddress.PostCode,
                        ContactName = model.ContactName,
                        ContactNo = model.ContactNo,
                        PlanningPoint = model.PlanningPoint,
                        SiteType = (int)model.SiteType,
                        AccountCode = model.AccountCode,
                        Depot = model.Depot,
                        SiteCodeChep = model.SiteCodeChep,
                        Status = (int)model.Status,
                        RegionId = model.RegionId
                    };
                    if (SiteID > 0)
                    {
                        site.SiteId = SiteID;
                    }
                    site = siteService.Create(site);
                    #endregion

                    #region Create Address (s)

                    if (model.FullAddress != null)
                    {
                        Address address = new Address()
                        {
                            ObjectId = model.Id,
                            ObjectType = "Site",
                            Town = model.FullAddress.Town,
                            Status = (int)Status.Active,
                            PostalCode = model.FullAddress.PostCode,
                            Type = (int)model.FullAddress.AddressType,
                            Addressline1 = model.FullAddress.AddressLine1,
                            Addressline2 = model.FullAddress.AddressLine2,
                            Province = (int)model.FullAddress.Province,
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
                    

                    scope.Complete();
                }

                Notify("The Site was successfully created.", NotificationType.Success);
                if (model.SourceView == "ManageSites")
                {
                    return RedirectToAction("ManageSites");
                }
                else
                {
                    return RedirectToAction("SubSites");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

       

        // GET: Client/EditSite/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditSite(int id) //, string sourceView
        {
            Site site;
            //int pspId = Session[ "UserPSP" ];
            bool layout = true; //temporarily while i figure out why the table edit doesnt work
            using (SiteService service = new SiteService())
            using (RegionService rservice = new RegionService())
            using (AddressService aservice = new AddressService())
            {
                site = service.GetById(id);
                //regionOptions = rservice.ListByColumnWhere("PSPId", pspId);

                if (site == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                Address address = aservice.Get(site.Id, "Site");

                bool unverified = (site.Status == (int)PSPClientStatus.Unverified);

                SiteViewModel model = new SiteViewModel()
                {
                    Id = site.Id,
                    Name = site.Name,
                    Description = site.Description,
                    XCord = site.XCord,
                    YCord = site.YCord,
                    Address = site.Address,
                    PostalCode = site.PostalCode,
                    ContactName = site.ContactName,
                    ContactNo = site.ContactNo,
                    PlanningPoint = site.PlanningPoint,
                    SiteType = (int)site.SiteType,
                    AccountCode = site.AccountCode,
                    Depot = site.Depot,
                    SiteCodeChep = site.SiteCodeChep,
                    Status = (int)site.Status,
                    EditMode = true,
                    RegionId = site.RegionId,
                    //RegionOptions = regionOptions,
                    FullAddress = new AddressViewModel()
                    {
                        EditMode = true,
                        Town = address?.Town,
                        Id = address?.Id ?? 0,
                        PostCode = address?.PostalCode,
                        AddressLine1 = address?.Addressline1,
                        AddressLine2 = address?.Addressline2,
                        Province = (address != null) ? (Province)address.Province : Province.All,
                        AddressType = (address != null) ? (AddressType)address.Type : AddressType.Postal,
                    },
                    SourceView = "ManageSites",
                };

                //SiteViewModel model = new SiteViewModel() { EditMode = true, SourceView = sourceView };//, RegionOptions = regionOptions
                //int pspId = Session[ "UserPSP" ];
                int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
                List<Region> regionOptions = new List<Region>();

                regionOptions = rservice.ListByColumnWhere("PSPId", pspId);

                IEnumerable<SelectListItem> regionDDL = regionOptions.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description

                });
                ViewBag.RegionOptions = regionDDL;
                //if (sourceView == "ManageSites")
                //{
                //    return RedirectToAction("ManageSites");
                //}
                //else
                //{
                //    return RedirectToAction("SubSites");
                //}
                if (layout)
                {
                    ViewBag.IncludeLayout = true;
                }
                return View(model);
            }
        }

        // POST: Client/EditSite/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult EditSite(SiteViewModel model, PagingModel pm, bool isstructure = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the selected Site was not updated. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                Site site;

                using (SiteService service = new SiteService())
                using (AddressService aservice = new AddressService())
                using (TransactionScope scope = new TransactionScope())
                {
                    site = service.GetById(model.Id);
                    Address address = aservice.Get(model.Id, "Client");


                    #region Validations

                    //if (!string.IsNullOrEmpty(model.AccountCode) && service.ExistByAccountCode(model.AccountCode.Trim()))
                    //{
                    //    // Role already exist!
                    //    Notify($"Sorry, a Site with the Account Code \"{model.AccountCode} ({model.AccountCode})\" already exists!", NotificationType.Error);

                    //    return View(model);
                    //}

                    #endregion
                    #region Create Address (s)
                    string stringAddress = "";
                    if (model.FullAddress != null)
                    {
                        Address siteAddress = aservice.GetById(model.FullAddress.Id);

                        if (siteAddress == null)
                        {
                            siteAddress = new Address()
                            {
                                ObjectId = model.Id,
                                ObjectType = "Site",
                                Town = model.FullAddress.Town,
                                Status = (int)Status.Active,
                                PostalCode = model.FullAddress.PostCode,
                                Type = (int)model.FullAddress.AddressType,
                                Addressline1 = model.FullAddress.AddressLine1,
                                Addressline2 = model.FullAddress.AddressLine2,
                                Province = (int)model.FullAddress.Province,
                            };

                            aservice.Create(siteAddress);
                        }
                        else
                        {
                            siteAddress.Town = model.FullAddress.Town;
                            siteAddress.PostalCode = model.FullAddress.PostCode;
                            siteAddress.Type = (int)model.FullAddress.AddressType;
                            siteAddress.Addressline1 = model.FullAddress.AddressLine1;
                            siteAddress.Addressline2 = model.FullAddress.AddressLine2;
                            siteAddress.Province = (int)model.FullAddress.Province;

                            aservice.Update(siteAddress);
                        }
                        stringAddress = model.FullAddress.AddressLine1 + " " + model.FullAddress.Town + " " + model.FullAddress.PostCode;
                    }

                    #endregion
                    #region Update Site

                    // Update Site
                    site.Id = model.Id;
                    site.Name = model.Name;
                    site.Description = model.Description;
                    site.XCord = model.XCord;
                    site.YCord = model.YCord;
                    site.Address = stringAddress;
                    site.PostalCode = model.FullAddress.PostCode;
                    site.ContactName = model.ContactName;
                    site.ContactNo = model.ContactNo;
                    site.PlanningPoint = model.PlanningPoint;
                    site.SiteType = (int)model.SiteType;
                    site.AccountCode = model.AccountCode;
                    site.Depot = model.Depot;
                    site.SiteCodeChep = model.SiteCodeChep;
                    site.Status = (int)model.Status;
                    site.RegionId = model.RegionId;

                    service.Update(site);

                    #endregion
                   



                    scope.Complete();
                }

                Notify("The selected Site details were successfully updated.", NotificationType.Success);

                return RedirectToAction("ManageSites");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteSite/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeleteSite(SiteViewModel model)
        {
            Site site;
            try
            {

                using (SiteService service = new SiteService())
                using (TransactionScope scope = new TransactionScope())
                {
                    site = service.GetById(model.Id);

                    if (site == null)
                    {
                        Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                        return PartialView("_AccessDenied");
                    }

                    site.Status = (((Status)site.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;

                    service.Update(site);
                    scope.Complete();

                }
                Notify("The selected Client was successfully updated.", NotificationType.Success);
                return RedirectToAction("ManageSites");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetSitesForClient(string clientId)
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                Session["ClientId"] = clientId;//wroite to session to use for adds and updates
                List<Site> sites = new List<Site>();
                //int pspId = Session[ "UserPSP" ];
                //int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (SiteService service = new SiteService())
                using (RegionService regionservice = new RegionService())
                {
                    List<Region> regions = regionservice.List(new PagingModel(), new CustomSearchModel());
                    sites = service.GetSitesByClientIncluded(int.Parse(clientId)); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));
                    foreach (Site site in sites)
                    {
                        if (site.RegionId != null)
                        {
                            site.Region = regions.FirstOrDefault(r => r.Id == site.RegionId);
                        } else
                        {
                            site.Region = null;
                        }
                    }
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

        #region Sub Sites
        //
        // POST || GET: /Client/SubSites
        public ActionResult SubSites(Nullable<int> siteId)
        {

            ViewBag.ViewName = "_SubSites";

            int total = 0;

            List<Site> model = new List<Site>();
            List<Client> clientList = new List<Client>();
            List<Site> mainSiteList;

            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientID = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            //string sessSiteId = (Session["SiteId"] != null ? Session["SiteId"].ToString() : null);
            //int SiteID = (!string.IsNullOrEmpty(sessSiteId) ? int.Parse(sessSiteId) : 0);

            using (ClientService clientService = new ClientService())
            using (SiteService sitesService = new SiteService())
            {
                if (clientID > 0)
                {
                    Client chosenClient = clientService.GetById(clientID);
                    clientList.Add(chosenClient);
                }
                else
                {

                    //mainSiteList = sitesService.GetSitesByClientsOfPSP(pspId);
                    clientList = clientService.GetClientsByPSP(pspId);
                }
            }
            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;
            //IEnumerable<SelectListItem> siteListDDL = mainSiteList.Select(c => new SelectListItem
            //{
            //    Value = c.Id.ToString(),
            //    Text = c.Name

            //});
           // ViewBag.SiteList = siteListDDL;
            ViewBag.SelectedSiteId = siteId;


            return PartialView("_SubSites");
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetSiteListForClient(string clientId)
        {
            if (clientId != null && clientId != "")
            {
                Session["ClientId"] = clientId;
                List<Site> sites = null;

                //int pspId = Session[ "UserPSP" ];
                int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (SiteService service = new SiteService())
                using (ClientSiteService cservice = new ClientSiteService())
                {


                    sites = service.GetSitesByClients(int.Parse(clientId)); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

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

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetSitesForClientSiteIncluded(string clientId, string siteId)
        {
            if (clientId != null && clientId != "")
            {
                Session["ClientId"] = clientId;
                Session["SiteId"] = siteId;
                List<Site> sites = null;
                //int pspId = Session[ "UserPSP" ];
               // int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (SiteService service = new SiteService())
                {

                    sites = service.GetSitesByClientsIncluded(int.Parse(clientId), int.Parse(siteId)); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

                }
                //var jsonList = JsonConvert.SerializeObject(sites);
                return Json(sites, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetSitesForClientSiteExcluded(string clientId, string siteId)
        {
            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(siteId) )
            {
                Session["ClientId"] = clientId;
                Session["SiteId"] = siteId;
                List<Site> sites = null;
                //int pspId = Session[ "UserPSP" ];
                //int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (SiteService service = new SiteService())
                {

                    sites = service.GetSitesByClientsExcluded(int.Parse(clientId), int.Parse(siteId)); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

                }
                //var jsonList = JsonConvert.SerializeObject(sites);
                return Json(sites, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetSiteDetailsForMain(string siteId)
        {
            if (siteId != null && siteId != "")
            {
                //Session["ClientId"] = clientId;
                Session["SiteId"] = siteId;
                Site site = null;
                using (SiteService service = new SiteService())
                {
                    site = service.GetById(int.Parse(siteId));
                }
                return Json(site, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetSiteForClientSiteExcluded(string mainSiteId, string movedSiteId, string clientId)
        {
            if (!string.IsNullOrEmpty(mainSiteId) && !string.IsNullOrEmpty(movedSiteId) && !string.IsNullOrEmpty(clientId))
            {
                //Session["ClientId"] = clientId;
                Session["SiteId"] = mainSiteId;
                //using (GroupService service = new GroupService())
                //using (ClientSiteService clientservice = new ClientSiteService())
                using (SiteService sservice = new SiteService())
                using (TransactionScope scope = new TransactionScope())
                {
                    Site currentSite = sservice.GetById(int.Parse(movedSiteId));
                    currentSite.SiteId = null;
                    sservice.Update(currentSite);

                    //ClientSite site = new ClientSite()
                    //{
                    //    Site
                    //    ClientId = int.Parse(clientId),
                    //    Status = (int)Status.Active
                    //};
                    //clientgroupservice.Create(group);

                    scope.Complete();
                }


                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "False", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetSiteForClientSiteIncluded(string mainSiteId, string movedSiteId, string clientId)
        {
            if (!string.IsNullOrEmpty(mainSiteId) && !string.IsNullOrEmpty(movedSiteId) && !string.IsNullOrEmpty(clientId))
            {
                //Session["ClientId"] = clientId;
                Session["SiteId"] = mainSiteId;
                //using (GroupService service = new GroupService())
                //using (ClientSiteService clientservice = new ClientSiteService())
                using (SiteService sservice = new SiteService())
                using (TransactionScope scope = new TransactionScope())
                {
                    Site currentSite = sservice.GetById(int.Parse(movedSiteId));
                    currentSite.SiteId = int.Parse(mainSiteId);
                    sservice.Update(currentSite);

                    //ClientSite site = new ClientSite()
                    //{
                    //    Site
                    //    ClientId = int.Parse(clientId),
                    //    Status = (int)Status.Active
                    //};
                    //clientgroupservice.Create(group);

                    scope.Complete();
                }


                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "False", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetSiteBudgetList(string siteId)
        {
            if (siteId != null && siteId != "")
            {
                List<SiteBudget> load = null;

                using (SiteBudgetService bservice = new SiteBudgetService())
                {
                    load = bservice.ListByColumnWhere("SiteId", int.Parse(siteId));
                    return Json(load, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetSiteBudget(string Id, string SiteId, string BudgetYear, string January, string February, string March, string April, string May, string June, string July, string August, string September, string October, string November, string December)
        {
            if (Id != null)
            {
                using (SiteBudgetService bservice = new SiteBudgetService())
                using (TransactionScope scope = new TransactionScope())
                {
                    //Collection of budgets or singular?
                    if (int.Parse(Id) > 0)
                    {
                        SiteBudget budget = bservice.GetById(int.Parse(Id));

                        budget.SiteId = int.Parse(SiteId);
                        budget.Status = (int)Status.Active;
                        budget.BudgetYear = DateTime.Now.Year;
                        budget.January = int.Parse(January);
                        budget.February = int.Parse(February);
                        budget.March = int.Parse(March);
                        budget.April = int.Parse(April);
                        budget.May = int.Parse(May);
                        budget.June = int.Parse(June);
                        budget.July = int.Parse(July);
                        budget.August = int.Parse(August);
                        budget.September = int.Parse(September);
                        budget.October = int.Parse(October);
                        budget.November = int.Parse(November);
                        budget.December = int.Parse(December);



                        bservice.Update(budget);
                    }
                    else
                    {
                        SiteBudget budget = new SiteBudget();
                        budget.SiteId = int.Parse(SiteId);
                        budget.BudgetYear = DateTime.Now.Year;
                        budget.January = int.Parse(January);
                        budget.February = int.Parse(February);
                        budget.March = int.Parse(March);
                        budget.April = int.Parse(April);
                        budget.May = int.Parse(May);
                        budget.June = int.Parse(June);
                        budget.July = int.Parse(July);
                        budget.August = int.Parse(August);
                        budget.September = int.Parse(September);
                        budget.October = int.Parse(October);
                        budget.November = int.Parse(November);
                        budget.December = int.Parse(December);

                        bservice.Create(budget);
                    }
                    scope.Complete();
                }

                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Client Group
        //
        // GET: /Client/ClientGroups
        public ActionResult ClientGroups(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            ViewBag.ViewName = "ClientGroups";
            if (givecsm)
            {
                ViewBag.ViewName = "ClientGroups";

                return PartialView("_ClientGroupsCustomSearch", new CustomSearchModel("ClientGroups"));
            }
            int total = 0;

            List<Group> model = new List<Group>();
            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

            //get group list, and their associated clients. TRhis woill be extended with an api call to get clients included and excluded as the button is clicked, and as the groups are changed
            using (ClientService clientService = new ClientService())
            //using (ClientGroupService clientGroupService = new ClientGroupService())
            using (GroupService groupService = new GroupService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "Name";

                model = groupService.GetGroupsByPSP(pspId);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : groupService.Total();

                //get the specific list of clients that exists for the first group, to render the tables, will use an api call to change it accordingly after reselection
                Group clientGroup = groupService.GetGroupsByPSP(pspId).FirstOrDefault();
                //if (clientGroup != null)
                //{
                //    ViewBag.ClientListIncluded = clientService.GetClientsByPSPIncludedGroup(pspId, clientGroup.Id);
                //    ViewBag.ClientListExcluded = clientService.GetClientsByPSPExcludedGroup(pspId, clientGroup.Id);
                //    //ViewBag.GroupData = groupService.GetGroupsByPSP(pspId);
                //}
            }
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_ClientGroups", paging);
        }


        // GET: Client/AddGroup
        [Requires(PermissionTo.Create)]
        public ActionResult AddGroup()
        {
            GroupViewModel model = new GroupViewModel() { EditMode = true };
            return View(model);
        }


        // POST: Client/AddGroup
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddGroup(GroupViewModel model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the Site was not created. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                using (GroupService gService = new GroupService())
                using (TransactionScope scope = new TransactionScope())
                {
                    #region Create Group
                    Group group = new Group()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Status = (int)model.Status
                    };
                    group = gService.Create(group);
                    #endregion

                    scope.Complete();
                }

                Notify("The Group was successfully created.", NotificationType.Success);
                return RedirectToAction("ClientGroups");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        // GET: Client/EditGroupGet/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditGroupGet(int id)
        {
            Group group;

            using (GroupService service = new GroupService())
            {
                group = service.GetById(id);


                if (group == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                GroupViewModel model = new GroupViewModel()
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Status = (int)group.Status,
                    EditMode = true
                };
                return View("EditGroup", model);
            }
        }

        // POST: Client/EditGroup/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditGroup(GroupViewModel model, PagingModel pm, bool isstructure = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the selected Group was not updated. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                Group group;

                using (GroupService service = new GroupService())
                using (TransactionScope scope = new TransactionScope())
                {
                    group = service.GetById(model.Id);

                    #region Update Group

                    // Update Site
                    group.Id = model.Id;
                    group.Name = model.Name;
                    group.Description = model.Description;
                    group.Status = (int)model.Status;

                    service.Update(group);

                    #endregion


                    scope.Complete();
                }

                Notify("The selected Site details were successfully updated.", NotificationType.Success);

                return RedirectToAction("ClientGroups");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteGroup/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeleteGroup(GroupViewModel model)
        {
            Group group;
            ClientGroup clientGroup;
            try
            {

                using (GroupService service = new GroupService())
                // using (ClientGroupService clientgroupservice = new ClientGroupService())
                using (TransactionScope scope = new TransactionScope())
                {
                    group = service.GetById(model.Id);

                    if (group == null)
                    {
                        Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                        return PartialView("_AccessDenied");
                    }

                    group.Status = (((Status)group.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;

                    //clientGroup = clientgroupservice.GetById(model.Id);
                    //clientGroup.Status = (((Status)group.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;                    

                    service.Update(group);
                    // clientgroupservice.Update(clientGroup);
                    scope.Complete();

                }
                Notify("The selected Group was successfully updated.", NotificationType.Success);
                return RedirectToAction("ClientGroups");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }



        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetClientsForGroupIncluded(string groupId)
        {
            if (groupId != null && groupId != "")
            {
                List<Client> clients;
                //int pspId = Session[ "UserPSP" ];
                int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (ClientService clientService = new ClientService())
                {

                    clients = clientService.GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

                }
                return Json(clients, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetClientsForGroupExcluded(string groupId)
        {
            if (groupId != null && groupId != "")
            {
                List<Client> clients;
                //int pspId = Session[ "UserPSP" ];
                int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (ClientService clientService = new ClientService())
                {

                    clients = clientService.GetClientsByPSPExcludedGroup(pspId, int.Parse(groupId));

                }
                return Json(clients, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetClientForGroupExcluded(string groupId, string clientId)
        {
            if (!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(clientId)) {
                //using (GroupService service = new GroupService())
                using (ClientGroupService clientgroupservice = new ClientGroupService())
                using (TransactionScope scope = new TransactionScope())
                {
                    List<ClientGroup> group = new List<ClientGroup>();
                    group = clientgroupservice.GetClientGroupsByClientGroup(int.Parse(groupId), int.Parse(clientId));

                    if (group == null)
                    {
                        return Json(data: "False", behavior: JsonRequestBehavior.AllowGet);
                    }
                    foreach (ClientGroup g in group)
                    {
                        clientgroupservice.Delete(g);
                    }
                    scope.Complete();
                }


                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
            } else {
                return Json(data: "False", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetClientForGroupIncluded(string groupId, string clientId)
        {
            if (!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(clientId)) {
                //using (GroupService service = new GroupService())
                using (ClientGroupService clientgroupservice = new ClientGroupService())
                using (GroupService groupservice = new GroupService())
                using (PSPClientService pspclientservice = new PSPClientService())
                using (TransactionScope scope = new TransactionScope())
                {
                    List<PSPClient> pspClientList = pspclientservice.ListByColumnWhere("ClientId", int.Parse(clientId));
                    Group groupObj = groupservice.GetById(int.Parse(groupId));

                    ClientGroup group = new ClientGroup()
                    {
                        GroupId = int.Parse(groupId),
                        ClientId = int.Parse(clientId),
                        Status = (int)Status.Active,
                        //PSPClient = pspClientList.FirstOrDefault(),
                        //Group = groupObj
                    };
                    clientgroupservice.Create(group);
                    // clientgroupservice.Update(clientGroup);
                    scope.Complete();
                }


            }        
            return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Products
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SetClient(string ClientId)
        {
            if (ClientId != null)
            {
                Session["ClientId"] = ClientId;
                return Json(data: "True", behavior: JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }


        //
        // POST || GET: /Client/LinkProducts
        public ActionResult LinkProducts(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "Products";

                return PartialView("_LinkProductsCustomSearch", new CustomSearchModel("LinkProducts"));
            }

            int total = 0;

            List<Product> model = new List<Product>();
            List<Client> clientList = new List<Client>();
            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
            int clientId = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
            using (ProductService service = new ProductService())
            using (ClientService clientService = new ClientService())
            {
                model = service.List(pm, csm);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total1(pm, csm);
                if (clientId > 0)
                {
                    Client client = clientService.GetById(clientId);
                    clientList.Add(client);
                }
                else
                {
                    clientList = clientService.GetClientsByPSP(pspId);
                }
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;

            return PartialView("_LinkProducts", paging);
        }


        //
        // GET: /Client/ProductDetails/5
        public ActionResult ProductDetails(int id, bool layout = true)
        {
            using (ProductService pservice = new ProductService())
            using (DocumentService dservice = new DocumentService())
            {
                Product model = pservice.GetById(id);

                if (model == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return RedirectToAction("Index");
                }

                if (layout)
                {
                    ViewBag.IncludeLayout = true;
                }

                List<Document> documents = dservice.List(model.Id, "Product");

                if (documents != null)
                {
                    ViewBag.Documents = documents;
                }

                return View(model);
            }
        }

        //
        // GET: /Client/AddProduct/5
        [Requires(PermissionTo.Create)]
        public ActionResult AddProduct()
        {
            ProductViewModel model = new ProductViewModel() { EditMode = true, ProductPrices = new List<ProductPriceViewModel>() };

            foreach (int item in Enum.GetValues(typeof(ProductPriceType)))
            {
                ProductPriceType type = (ProductPriceType)item;

                model.ProductPrices.Add(new ProductPriceViewModel()
                {
                    Type = type,
                    Status = Status.Active
                });
            }

            return View(model);
        }

        //
        // POST: /Client/AddProduct/5
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Notify("Sorry, the Product was not created. Please correct all errors and try again.", NotificationType.Error);

                return View(model);
            }

            Product product = new Product();

            using (ProductService pservice = new ProductService())
            using (TransactionScope scope = new TransactionScope())
            using (DocumentService dservice = new DocumentService())
            using (ClientProductService pcservice = new ClientProductService())
            using (ProductPriceService ppservice = new ProductPriceService())
            {
                #region Validations

                if (pservice.Exist(model.Name))
                {
                    // Product already exist!
                    Notify($"Sorry, a Product with the Name \"{model.Name}\" already exists!", NotificationType.Error);

                    return View(model);
                }

                #endregion

                #region Product

                product.Name = model.Name;
                product.Status = (int)model.Status;
                product.Description = model.Description;

                product = pservice.Create(product);

                #endregion

                #region Product Prices

                if (model.ProductPrices.NullableAny())
                {
                    foreach (ProductPriceViewModel price in model.ProductPrices)
                    {
                        ProductPrice pp = new ProductPrice()
                        {
                            ProductId = product.Id,
                            Rate = price.Rate ?? 0,
                            Type = (int)price.Type,
                            RateUnit = price.RateUnit,
                            FromDate = price.StartDate,
                            Status = (int)price.Status,
                        };

                        ppservice.Create(pp);
                    }
                }

                #endregion

                #region Product to Client
                string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
                int clientID = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
                ClientProduct clientproduct = new ClientProduct()
                {
                    ClientId = clientID,
                    ProductId = product.Id,
                    ProductDescription = product.Description,
                    ActiveDate = product.CreatedOn,
                    HireRate = 0,
                    LostRate = 0,
                    IssueRate = 0,
                    PassonRate = 0,
                    PassonDays = 0,
                    Status = product.Status,

                };
                pcservice.Create(clientproduct);
                #endregion

                #region Any Files

                if (model.File != null)
                {
                    // Create folder
                    string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Product/{model.Name.Trim().Replace("/", "_").Replace("\\", "_")}/");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                    Document doc = new Document()
                    {
                        ObjectId = product.Id,
                        ObjectType = "Product",
                        Name = model.File.Name,
                        Category = model.File.Name,
                        Status = (int)Status.Active,
                        Title = model.File.File.FileName,
                        Size = model.File.File.ContentLength,
                        Description = model.File.Description,
                        Type = Path.GetExtension(model.File.File.FileName),
                        Location = $"Product/{model.Name.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{model.File.File.FileName}"
                    };

                    dservice.Create(doc);

                    string fullpath = Path.Combine(path, $"{now}-{model.File.File.FileName}");
                    model.File.File.SaveAs(fullpath);
                }

                #endregion

                scope.Complete();

                Notify("The Product was successfully created.", NotificationType.Success);
            }

            return RedirectToAction("LinkProducts");
        }

        //
        // GET: /Client/EditProduct/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditProduct(int id)
        {
            using (DocumentService dservice = new DocumentService())
            using (ProductService pservice = new ProductService())
            {
                Product product = pservice.GetById(id);

                if (product == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                List<Document> documents = dservice.List(product.Id, "Product");

                ProductViewModel model = new ProductViewModel()
                {
                    Id = product.Id,
                    EditMode = true,
                    Name = product.Name,
                    Description = product.Description,
                    Status = (Status)product.Status,
                    File = new FileViewModel()
                    {
                        Name = documents?.FirstOrDefault()?.Name,
                        Id = documents?.FirstOrDefault()?.Id ?? 0,
                        Extension = documents?.FirstOrDefault()?.Type,
                        Description = documents?.FirstOrDefault()?.Description,
                    },
                    ProductPrices = new List<ProductPriceViewModel>(),
                };

                foreach (ProductPrice p in product.ProductPrices)
                {
                    model.ProductPrices.Add(new ProductPriceViewModel()
                    {
                        Id = p.Id,
                        Rate = p.Rate,
                        RateUnit = p.RateUnit,
                        StartDate = p.FromDate,
                        ProductId = p.ProductId,
                        Status = (Status)p.Status,
                        Type = (ProductPriceType)p.Type
                    });
                }

                if (model.ProductPrices.Count < 3)
                {
                    foreach (int item in Enum.GetValues(typeof(ProductPriceType)))
                    {
                        ProductPriceType type = (ProductPriceType)item;

                        if (model.ProductPrices.Any(p => p.Type == type)) continue;

                        model.ProductPrices.Add(new ProductPriceViewModel()
                        {
                            Type = type,
                            Status = Status.Active
                        });
                    }
                }

                return View(model);
            }
        }

        //
        // POST: /Client/EditProduct/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Notify("Sorry, the selected Product was not updated. Please correct all errors and try again.", NotificationType.Error);

                return View(model);
            }

            using (ProductService pservice = new ProductService())
            using (TransactionScope scope = new TransactionScope())
            using (DocumentService dservice = new DocumentService())
            using (ProductPriceService ppservice = new ProductPriceService())
            {
                Product product = pservice.GetById(model.Id);

                #region Validations

                if (product == null)
                {
                    Notify("Sorry, that Product does not exist! Please specify a valid Product Id and try again.", NotificationType.Error);

                    return View(model);
                }

                if (product.Name != model.Name && pservice.Exist(model.Name))
                {
                    // Product already exist!
                    Notify($"Sorry, a Product with the Name \"{model.Name}\" already exists!", NotificationType.Error);

                    return View(model);
                }

                #endregion

                #region Product

                product.Name = model.Name;
                product.Status = (int)model.Status;
                product.Description = model.Description;

                product = pservice.Update(product);

                #endregion

                #region Product Prices

                if (model.ProductPrices.NullableAny())
                {
                    foreach (ProductPriceViewModel price in model.ProductPrices)
                    {
                        ProductPrice pp = ppservice.GetById(price.Id);

                        if (pp == null)
                        {
                            pp = new ProductPrice()
                            {
                                ProductId = product.Id,
                                Rate = price.Rate ?? 0,
                                Type = (int)price.Type,
                                RateUnit = price.RateUnit,
                                FromDate = price.StartDate,
                                Status = (int)price.Status,
                            };

                            ppservice.Create(pp);
                        }
                        else
                        {
                            pp.Rate = price.Rate ?? 0;
                            pp.RateUnit = price.RateUnit;
                            pp.FromDate = price.StartDate;
                            pp.Status = (int)price.Status;

                            ppservice.Update(pp);
                        }
                    }
                }

                #endregion

                #region Any Files

                if (model.File.File != null)
                {
                    // Create folder
                    string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Product/{model.Name.Trim().Replace("/", "_").Replace("\\", "_")}/");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                    Document doc = dservice.GetById(model.File.Id);

                    if (doc != null)
                    {
                        // Disable this file...
                        doc.Status = (int)Status.Inactive;

                        dservice.Update(doc);
                    }

                    doc = new Document()
                    {
                        ObjectId = product.Id,
                        ObjectType = "Product",
                        Name = model.File.Name,
                        Category = model.File.Name,
                        Status = (int)Status.Active,
                        Title = model.File.File.FileName,
                        Size = model.File.File.ContentLength,
                        Description = model.File.Description,
                        Type = Path.GetExtension(model.File.File.FileName),
                        Location = $"Product/{model.Name.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{model.File.File.FileName}"
                    };

                    dservice.Create(doc);

                    string fullpath = Path.Combine(path, $"{now}-{model.File.File.FileName}");
                    model.File.File.SaveAs(fullpath);
                }

                #endregion

                scope.Complete();

                Notify("The selected Product's details were successfully updated.", NotificationType.Success);
            }

            return RedirectToAction("LinkProducts");
        }

        //
        // POST: /Client/DeleteProduct/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeleteProduct(ProductViewModel model)
        {
            Product product;

            using (ProductService service = new ProductService())
            {
                product = service.GetById(model.Id);

                if (product == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                product.Status = (((Status)product.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;

                service.Update(product);

                Notify("The selected Product was successfully updated.", NotificationType.Success);
            }

            return RedirectToAction("LinkProducts");
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetProductsForClient(string clientId)
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                Session["ClientId"] = clientId;//wroite to session to use for adds and updates
                List<ProductCustomModel> products = null;
                //int pspId = Session[ "UserPSP" ];
                //int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (ProductService service = new ProductService())
                {
                    products = service.List1(new PagingModel(), new CustomSearchModel(), int.Parse(clientId));
                }
                //ProductPrice price = new ProductPrice();

                //using (ProductPriceService aservice = new ProductPriceService())
                //{
                //    foreach (ProductCustomModel prod in products)
                //    {
                //        string hrate = "";
                //        string irate = "";
                //        string lrate = "";
                //        string pocost = "";
                //        price = aservice.Get(prod.Id);
                //        if (price != null)
                //        {
                //            hrate = price.RateUnit.ToString() + ' ' + price.Rate.ToString();
                //            irate = price.RateUnit.ToString() + ' ' + price.Rate.ToString();
                //            lrate = price.RateUnit.ToString() + ' ' + price.Rate.ToString();
                //            pocost = price.RateUnit.ToString() + ' ' + price.Rate.ToString();

                //        }

                //        Status status = (Status)prod.Status;
                //        String active = status.Equals(Status.Active) ? "active" : "inactive";
                //        String enable = status.Equals(Status.Active) ? "Disable" : "Enable";


                //    }
                //}


                //var jsonList = JsonConvert.SerializeObject(sites);
                return Json(products, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Manage Transporters
        //
        // GET: /Client/ManageTransporters
        public ActionResult ManageTransporters(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {

            ViewBag.ViewName = "ManageTransporters";
            if (givecsm)
            {
                ViewBag.ViewName = "ManageTransporters";

                return PartialView("_ManageTransportersCustomSearch", new CustomSearchModel("ManageTransporters"));
            }
            int total = 0;

            List<Transporter> model = new List<Transporter>();
            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            using (TransporterService service = new TransporterService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List(pm, csm);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total(pm, csm);
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_ManageTransporters", paging);
        }

        // GET: Client/AddTransporter
        [Requires(PermissionTo.Create)]
        public ActionResult AddTransporter()
        {
            TransporterViewModel model = new TransporterViewModel() { EditMode = true };
            return View(model);
        }


        // POST: Client/Transporter
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddTransporter(TransporterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the Site was not created. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                using (TransporterService siteService = new TransporterService())
                using (TransactionScope scope = new TransactionScope())
                {
                    //#region Validation
                    //if (!string.IsNullOrEmpty(model.RegistrationNumber) && siteService.ExistByName(model.RegistrationNumber.Trim()))
                    //{
                    //    // Bank already exist!
                    //    Notify($"Sorry, a Site with the Account number \"{model.AccountCode}\" already exists!", NotificationType.Error);

                    //    return View(model);
                    //}
                    //#endregion
                    #region Create Transporter
                    Transporter site = new Transporter()
                    {
                        Name = model.Name,
                        TradingName = model.TradingName,
                        RegistrationNumber = model.RegistrationNumber,
                        Email = model.Email,
                        ContactNumber = model.ContactNumber,
                        Status = (int)Status.Active
                    };
                    site = siteService.Create(site);
                    #endregion

                    scope.Complete();
                }

                Notify("The Transporter was successfully created.", NotificationType.Success);
                return RedirectToAction("ManageTransporters");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

   

        // GET: Client/EditTransporter/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditTransporter(int id)
        {
            Transporter site;
            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);



            using (TransporterService service = new TransporterService())
            using (AddressService aservice = new AddressService())
            {
                site = service.GetById(id);            

                if (site == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                Address address = aservice.Get(site.Id, "Site");


                bool unverified = (site.Status == (int)PSPClientStatus.Unverified);

                TransporterViewModel model = new TransporterViewModel()
                {
                    Id = site.Id,
                    Name = site.Name,
                    TradingName = site.TradingName,
                    RegistrationNumber = site.RegistrationNumber,
                    Email = site.Email,
                    ContactNumber = site.ContactNumber,
                   // Status = (int)Status.Active
                    Status = (int)site.Status,
                    EditMode = true
                };
                return View(model);
            }
        }

        // POST: Client/EditTransporter/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult EditTransporter(TransporterViewModel model, PagingModel pm, bool isstructure = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the selected Transporter was not updated. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                Transporter site;

                using (TransporterService service = new TransporterService())
                using (TransactionScope scope = new TransactionScope())
                {
                    site = service.GetById(model.Id);


                    #region Validations

                    //if (!string.IsNullOrEmpty(model.AccountCode) && service.ExistByAccountCode(model.AccountCode.Trim()))
                    //{
                    //    // Role already exist!
                    //    Notify($"Sorry, a Site with the Account Code \"{model.AccountCode} ({model.AccountCode})\" already exists!", NotificationType.Error);

                    //    return View(model);
                    //}

                    #endregion
                    #region Update Transporter

                    // Update Transporter
                    site.Id = model.Id;
                    site.Name = model.Name;
                    site.RegistrationNumber = model.RegistrationNumber;
                    site.Email = model.Email;
                    site.ContactNumber = model.ContactNumber;
                    site.TradingName = model.TradingName;
                    site.Status = (int)model.Status;

                    service.Update(site);

                    #endregion




                    scope.Complete();
                }

                Notify("The selected Transporter details were successfully updated.", NotificationType.Success);

                return RedirectToAction("ManageTransporters");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: Client/DeleteTransporter/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeleteTransporter(SiteViewModel model)
        {
            Transporter site;
            try
            {

                using (TransporterService service = new TransporterService())
                using (TransactionScope scope = new TransactionScope())
                {
                    site = service.GetById(model.Id);

                    if (site == null)
                    {
                        Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                        return PartialView("_AccessDenied");
                    }

                    site.Status = (((Status)site.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;

                    service.Update(site);
                    scope.Complete();

                }
                Notify("The selected Transporter was successfully updated.", NotificationType.Success);
                return RedirectToAction("ManageSites");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }


        #endregion

        #region Awaiting Activation 

        //
        // POST || GET: /Client/AwaitingActivation
        public ActionResult AwaitingActivation(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "AwaitingActivation";

                return PartialView("_AwaitingActivationCustomSearch", new CustomSearchModel("AwaitingActivation"));
            }
            int total = 0;

            List<Client> model = new List<Client>();
            //int pspId = Session[ "UserPSP" ];
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            using (ClientService service = new ClientService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";
                if (CurrentUser.PSPs.Count > 0)
                {
                    model = service.GetClientsByPSPAwaitingActivation(pspId);
                }
                else
                {
                    model = null;
                }

                // var testModel = service.ListByColumn(null, "CompanyRegistrationNumber", "123456");
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);


            return PartialView("_AwaitingActivation", paging);
        }
        #endregion

        #region Exports

        //
        // GET: /Administration/Export
        public FileContentResult Export(PagingModel pm, CustomSearchModel csm, string type = "configurations")
        {
            string csv = "";
            string filename = string.Format("{0}-{1}.csv", type.ToUpperInvariant(), DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));

            pm.Skip = 0;
            pm.Take = int.MaxValue;

            switch (type)
            {
                case "clientlist":
                    #region ClientList
                    csv = String.Format("Id, Company Name, Reg #, Trading As, Vat Number, Contact Number, Contact Person, Email {0}", Environment.NewLine);

                    List<Client> clients = new List<Client>();

                    using (ClientService service = new ClientService())
                    {
                        clients = service.List1(pm, csm);
                    }

                    if (clients != null && clients.Any())
                    {
                        foreach (Client item in clients)
                        {
                            csv = String.Format("{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                                csv,
                                                item.Id,
                                                item.CompanyName,
                                                item.CompanyRegistrationNumber,
                                                item.TradingAs,
                                                item.VATNumber,
                                                item.ContactNumber,
                                                item.ContactPerson,
                                                item.Email,
                                                (Status)(int)item.Status,
                                                Environment.NewLine);
                        }
                    }


                    #endregion

                    break;
                case "awaitingactivation":
                    #region AwaitingActivation
                    csv = String.Format("Id, Company Name, Reg #, Trading As, Vat Number, Contact Number, Contact Person, Email {0}", Environment.NewLine);

                    List<Client> inactiveclients = new List<Client>();

                    using (ClientService service = new ClientService())
                    {
                        inactiveclients = service.List(pm, csm);
                    }

                    if (inactiveclients != null && inactiveclients.Any())
                    {
                        foreach (Client item in inactiveclients)
                        {
                            csv = String.Format("{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                                csv,
                                                item.Id,
                                                item.CompanyName,
                                                item.CompanyRegistrationNumber,
                                                item.TradingAs,
                                                item.VATNumber,
                                                item.ContactNumber,
                                                item.ContactPerson,
                                                item.Email,
                                                (Status)(int)item.Status,
                                                Environment.NewLine);
                        }
                    }


                    #endregion

                    break;
                //case "linkproducts":
                //    #region linkproducts
                //    csv = String.Format("Id, Company Name, Reg #, Trading As, Vat Number, Contact Number, Contact Person, Email {0}", Environment.NewLine);

                //    List<Product> product = new List<Product>();

                //    using (ProductService service = new ProductService())
                //    {
                //        clients = service.List(pm, csm);
                //    }

                //    if (clients != null && clients.Any())
                //    {
                //        foreach (Client item in clients)
                //        {
                //            csv = String.Format("{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10} {11}",
                //                                csv,
                //                                item.Id,
                //                                item.CompanyName,
                //                                item.CompanyRegistrationNumber,
                //                                item.TradingAs,
                //                                item.VATNumber,
                //                                item.ContactNumber,
                //                                item.ContactPerson,
                //                                item.Email,
                //                                Environment.NewLine);
                //        }
                //    }


                //    #endregion

                //    break;

                //case "managesites":
                //    #region ClientList
                //    csv = String.Format("Id, Company Name, Reg #, Trading As, Vat Number, Contact Number, Contact Person, Email {0}", Environment.NewLine);

                //    List<Client> clients = new List<Client>();

                //    using (ClientService service = new ClientService())
                //    {
                //        clients = service.List(pm, csm);
                //    }

                //    if (clients != null && clients.Any())
                //    {
                //        foreach (Client item in clients)
                //        {
                //            csv = String.Format("{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10} {11}",
                //                                csv,
                //                                item.Id,
                //                                item.CompanyName,
                //                                item.CompanyRegistrationNumber,
                //                                item.TradingAs,
                //                                item.VATNumber,
                //                                item.ContactNumber,
                //                                item.ContactPerson,
                //                                item.Email,
                //                                Environment.NewLine);
                //        }
                //    }


                //    #endregion

                //    break;
                //case "managetransporters":
                //    #region ClientList
                //    csv = String.Format("Id, Company Name, Reg #, Trading As, Vat Number, Contact Number, Contact Person, Email {0}", Environment.NewLine);

                //    List<Client> clients = new List<Client>();

                //    using (ClientService service = new ClientService())
                //    {
                //        clients = service.List(pm, csm);
                //    }

                //    if (clients != null && clients.Any())
                //    {
                //        foreach (Client item in clients)
                //        {
                //            csv = String.Format("{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10} {11}",
                //                                csv,
                //                                item.Id,
                //                                item.CompanyName,
                //                                item.CompanyRegistrationNumber,
                //                                item.TradingAs,
                //                                item.VATNumber,
                //                                item.ContactNumber,
                //                                item.ContactPerson,
                //                                item.Email,
                //                                Environment.NewLine);
                //        }
                //    }


                //    #endregion

                //    break;

            }

            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", filename);
        }

        #endregion

        #region General

        [HttpPost]
        // GET: /Client/ImportSites
        public ActionResult ImportSubSites(ImportFileModel model)
        {

            return View() ;
        }

        [HttpPost]
        // GET: /Client/ImportSites
        public ActionResult ImportSites(HttpPostedFileBase postedFile)
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

                try { 
                    string sessClientId = (Session["ClientId"] != null ? Session["ClientId"].ToString() : null);
                    int clientID = (!string.IsNullOrEmpty(sessClientId) ? int.Parse(sessClientId) : 0);
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

                            #region Validation
                            if (!string.IsNullOrEmpty(rows[13].ToString()) && !siteService.ExistByAccountCode(rows[13].ToString().Trim()))
                            {

                                int regionId = 0;
                                if (!string.IsNullOrEmpty(rows[16].ToString()))
                                {
                                    regionId = int.Parse(rows[16].ToString());
                                }
                                int provinceId = 0;
                                if (!string.IsNullOrEmpty(rows[8].ToString()))
                                {
                                    provinceId = int.Parse(rows[8].ToString());
                                }
                                #region Create Site
                                Site site = new Site()
                                {
                                    Name = rows[0].ToString(),
                                    
                                    Description = rows[1].ToString(),
                                    XCord = rows[1].ToString(),
                                    YCord = rows[2].ToString(),
                                    Address = rows[3].ToString() + " " + rows[4].ToString() + " " + rows[5].ToString() + " " + rows[6].ToString() + " " + rows[7].ToString() + " " + rows[8].ToString(),
                                    PostalCode = rows[7].ToString(),
                                    ContactName = rows[10].ToString(),
                                    ContactNo = rows[9].ToString(),
                                    PlanningPoint = rows[11].ToString(),
                                    SiteType = 1,//(int).parse(rows[12].ToString()),
                                    AccountCode = rows[13].ToString(),
                                    Depot = rows[14].ToString(),
                                    SiteCodeChep = rows[15].ToString(),
                                    Status = (int)Status.Pending,
                                    RegionId = regionId
                                };
                                //For Subsites
                                if (SiteID > 0) {
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
                                        Town = rows[5].ToString(),
                                        Status = (int)Status.Active,
                                        PostalCode = rows[6].ToString(),
                                        Type = (int)AddressType.Postal,
                                        Addressline1 = rows[3].ToString(),
                                        Addressline2 = rows[4].ToString(),
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
                    }
                        scope.Complete();
                    }

   
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            } else
            {
               
            }
            return RedirectToAction("Index", "Client");
        }


        //private bool UploadProductFile(HttpPostedFileBase FileUpload)
        //{
        //    // get the file stream in a readable way
        //    StreamReader reader = new StreamReader(FileUpload.InputStream);

        //    // get a DataTable representing the passed string
        //    System.Data.DataTable dt = ProcessCSV(reader.ReadToEnd());

        //    // for each row, compose the statement
        //    bool success = true;
        //    foreach (System.Data.DataRow row in dt.Rows)
        //        success = db.InsertProdutInfo(row);

        //    return success;
        //}

        #endregion

    }
}
