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
        public ActionResult ClientList(PagingModel pm, CustomSearchModel csm)
        {
            int total = 0;

            List<Client> model = new List<Client>();

            using (ClientService service = new ClientService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";
                if (CurrentUser.PSPs.Count > 0)
                {
                    model = service.GetClientsByPSP(CurrentUser.PSPs.FirstOrDefault().Id);
                } else
                {
                    model = null;
                }

                // var testModel = service.ListByColumn(null, "CompanyRegistrationNumber", "123456");
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);


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
                Address address = aservice.Get(model.Id, "PSP");

                List<Document> documents = dservice.List(model.Id, "Client");
                List<Document> logo = dservice.List(model.Id, "ClientLogo");

                if (address != null)
                {
                    ViewBag.Address = address;
                }
                if (documents != null)
                {
                    ViewBag.Documents = documents;
                    ViewBag.Logo = logo;
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
                using (PSPClientService pspservice = new PSPClientService())
                using (AddressService aservice = new AddressService())
                using (TransactionScope scope = new TransactionScope())
                using (DocumentService dservice = new DocumentService())
                using (ClientKPIService kservice = new ClientKPIService())
                using (ClientBudgetService bservice = new ClientBudgetService())
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
                    int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
                    PSPClient pClient = new PSPClient()
                    {
                        PSPId = pspId,
                        ClientId = client.Id,
                        Status = (int)Status.Active
                    };
                    #endregion

                    #region Create Client Budget

                    if (model.ClientBudget != null)
                    {
                        foreach (ClientBudget budgetList in model.ClientBudget)
                        {
                            ClientBudget budget = new ClientBudget()
                            {
                                ClientId = client.Id,
                                Status = (int)Status.Active,
                                BudgetYear = DateTime.Now.Year,
                                January = budgetList.January,
                                February = budgetList.February,
                                March = budgetList.March,
                                April = budgetList.April,
                                May = budgetList.May,
                                June = budgetList.June,
                                July = budgetList.July,
                                August = budgetList.August,
                                September = budgetList.September,
                                October = budgetList.October,
                                November = budgetList.November,
                                December = budgetList.December,
                            };

                            budget = bservice.Create(budget);
                        }
                    }

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
                    if (model.CompanyFile != null)
                    {
                        foreach (FileViewModel file in model.CompanyFile)
                        {
                            if (file.Name != null)
                            {
                                // Create folder
                                string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

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
                                    Name = file.Name,
                                    Category = file.Name,
                                    Title = file.File.FileName,
                                    Size = file.File.ContentLength,
                                    Description = file.File.FileName,
                                    Type = Path.GetExtension(file.File.FileName),
                                    Location = $"Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{file.File.FileName}"
                                };

                                dservice.Create(doc);

                                string fullpath = Path.Combine(path, $"{now}-{file.File.FileName}");
                                file.File.SaveAs(fullpath);
                            }
                        }
                    }

                    #endregion

                    #region Any Logo Uploads
                    if (model.Logo != null)
                    {
                        foreach (FileViewModel logo in model.Logo)
                        {
                            if (logo.Name != null)
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
                                    ObjectType = "ClientLogo",
                                    Status = (int)Status.Active,
                                    Name = logo.Name,
                                    Category = logo.Name,
                                    Title = logo.File.FileName,
                                    Size = logo.File.ContentLength,
                                    Description = logo.File.FileName,
                                    Type = Path.GetExtension(logo.File.FileName),
                                    Location = $"Client/Logo/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{logo.File.FileName}"
                                };

                                dservice.Create(doc);

                                string fullpath = Path.Combine(path, $"{now}-{logo.File.FileName}");
                                logo.File.SaveAs(fullpath);
                            }
                        }
                    }

                    #endregion

                    #region Create Client Budget

                    //if (model.ClientKPI != null)
                    //{
                    //    foreach (ClientKPI kpi in model.ClientKPI)
                    //    {
                    //    }
                    //}

                    #endregion

                    scope.Complete();
                }
                Notify("The Client was successfully created.", NotificationType.Success);
                return RedirectToAction("Client");
            }
            catch
            {
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
            {
                client = service.GetById(id);


                if (client == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                Address address = aservice.Get(client.Id, "Client");

                List<Document> documents = dservice.List(client.Id, "Client");

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
                    //ClientBudget = new EstimatedLoadViewModel()
                    //{
                    //    Id = (unverified) ? 0 : client.ClientBudgets?.FirstOrDefault()?.Id ?? 0,
                    //    January = (unverified) ? load.January : client.ClientBudgets?.FirstOrDefault()?.January,
                    //    February = (unverified) ? load.February : client.ClientBudgets?.FirstOrDefault()?.February,
                    //    March = (unverified) ? load.March : client.ClientBudgets?.FirstOrDefault()?.March,
                    //    April = (unverified) ? load.April : client.ClientBudgets?.FirstOrDefault()?.April,
                    //    May = (unverified) ? load.May : client.ClientBudgets?.FirstOrDefault()?.May,
                    //    June = (unverified) ? load.June : client.ClientBudgets?.FirstOrDefault()?.June,
                    //    July = (unverified) ? load.July : client.ClientBudgets?.FirstOrDefault()?.July,
                    //    August = (unverified) ? load.August : client.ClientBudgets?.FirstOrDefault()?.August,
                    //    September = (unverified) ? load.September : client.ClientBudgets?.FirstOrDefault()?.September,
                    //    October = (unverified) ? load.October : client.ClientBudgets?.FirstOrDefault()?.October,
                    //    November = (unverified) ? load.November : client.ClientBudgets?.FirstOrDefault()?.November,
                    //    December = (unverified) ? load.December : client.ClientBudgets?.FirstOrDefault()?.December,
                    //},
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
                    }
                };
                if (client.ClientBudgets != null && client.ClientBudgets.Count > 0)
                {
                    foreach(ClientBudget budget in client.ClientBudgets)
                    {
                        ClientBudget cb = new ClientBudget()
                        {
                            Id = (unverified) ? 0 : client.ClientBudgets?.FirstOrDefault()?.Id ?? 0,
                            January = (unverified) ? load.January : client.ClientBudgets?.FirstOrDefault()?.January,
                            February = (unverified) ? load.February : client.ClientBudgets?.FirstOrDefault()?.February,
                            March = (unverified) ? load.March : client.ClientBudgets?.FirstOrDefault()?.March,
                            April = (unverified) ? load.April : client.ClientBudgets?.FirstOrDefault()?.April,
                            May = (unverified) ? load.May : client.ClientBudgets?.FirstOrDefault()?.May,
                            June = (unverified) ? load.June : client.ClientBudgets?.FirstOrDefault()?.June,
                            July = (unverified) ? load.July : client.ClientBudgets?.FirstOrDefault()?.July,
                            August = (unverified) ? load.August : client.ClientBudgets?.FirstOrDefault()?.August,
                            September = (unverified) ? load.September : client.ClientBudgets?.FirstOrDefault()?.September,
                            October = (unverified) ? load.October : client.ClientBudgets?.FirstOrDefault()?.October,
                            November = (unverified) ? load.November : client.ClientBudgets?.FirstOrDefault()?.November,
                            December = (unverified) ? load.December : client.ClientBudgets?.FirstOrDefault()?.December,
                        };
                        client.ClientBudgets.Add(cb);
                    }
                };
                //if (client != null && client.ClientBudgets.Count > 0)
                //{
                //    foreach (FileViewModel file in client.CompanyFile)
                //    CompanyFile = new FileViewModel()
                //    {
                //        Name = documents?.FirstOrDefault()?.Name,
                //        Id = documents?.FirstOrDefault()?.Id ?? 0,
                //        Extension = documents?.FirstOrDefault()?.Type,
                //        Description = documents?.FirstOrDefault()?.Description,
                //    },

                //}
                //if (client.Logo != null && client.ClientBudgets.Count > 0)
                //{
                //    foreach (FileViewModel file in client.Logo)
                //    CompanyFile = new FileViewModel()
                //    {
                //        Name = documents?.FirstOrDefault()?.Name,
                //        Id = documents?.FirstOrDefault()?.Id ?? 0,
                //        Extension = documents?.FirstOrDefault()?.Type,
                //        Description = documents?.FirstOrDefault()?.Description,
                //    },

                //}


                return View(model);
            }
           }

        // POST: Client/EditSite/5
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
                using (ClientBudgetService bservice = new ClientBudgetService())
                {
                    client = service.GetById(model.Id);

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
                    //Status = (Status)model.Status;
                    client.Status = model.Status;

                    service.Update(client);

                    #endregion

                    #region Update Client Budget
                    //Collection of budgets or singular?
                    //if (model != null)
                    //{
                    //    ClientBudget budget = bservice.GetById(model.ClientBudget.Id);

                    //    if (budget == null)
                    //    {
                    //        budget = new ClientBudget()
                    //        {
                    //            ClientId = model.Id,
                    //            Status = (int)Status.Active,
                    //            BudgetYear = DateTime.Now.Year,
                    //            January = model.ClientBudget.January,
                    //            February = model.ClientBudget.February,
                    //            March = model.ClientBudget.March,
                    //            April = model.ClientBudget.April,
                    //            May = model.ClientBudget.May,
                    //            June = model.ClientBudget.June,
                    //            July = model.ClientBudget.July,
                    //            August = model.ClientBudget.August,
                    //            September = model.ClientBudget.September,
                    //            October = model.ClientBudget.October,
                    //            November = model.ClientBudget.November,
                    //            December = model.ClientBudget.December,

                    //        };

                    //        bservice.Create(budget);
                    //    }
                    //    else
                    //    {
                    //        budget.BudgetYear = DateTime.Now.Year;
                    //        budget.January = model.ClientBudget.January;
                    //        budget.February = model.ClientBudget.February;
                    //        budget.March = model.ClientBudget.March;
                    //        budget.April = model.ClientBudget.April;
                    //        budget.May = model.ClientBudget.May;
                    //        budget.June = model.ClientBudget.June;
                    //        budget.July = model.ClientBudget.July;
                    //        budget.August = model.ClientBudget.August;
                    //        budget.September = model.ClientBudget.September;
                    //        budget.October = model.ClientBudget.October;
                    //        budget.November = model.ClientBudget.November;
                    //        budget.December = model.ClientBudget.December;

                    //        bservice.Update(budget);
                    //    }
                    //}

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
                                ObjectType = "CLient",
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
                    if (model.CompanyFile != null)
                    {
                        foreach (FileViewModel file in model.CompanyFile)
                        {
                            if (file.Name != null)
                            {
                                // Create folder
                                string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }

                                string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                                Document doc = dservice.GetById(file.Id);

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
                                    Name = file.Name,
                                    Category = file.Name,
                                    Title = file.File.FileName,
                                    Size = file.File.ContentLength,
                                    Description = file.File.FileName,
                                    Type = Path.GetExtension(file.File.FileName),
                                    Location = $"Client/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{file.File.FileName}"

                                };

                                dservice.Create(doc);

                                string fullpath = Path.Combine(path, $"{now}-{file.File.FileName}");
                                file.File.SaveAs(fullpath);
                            }
                        }
                    }

                    #endregion



                    #region Any Logos
                    if (model.Logo != null)
                    {
                        foreach (FileViewModel logo in model.Logo)
                        {
                            if (logo.Name != null)
                            {
                                // Create folder
                                string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/Client/Logo/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }

                                string now = DateTime.Now.ToString("yyyyMMddHHmmss");

                                Document doc = dservice.GetById(logo.Id);

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
                                    Name = logo.Name,
                                    Category = logo.Name,
                                    Title = logo.File.FileName,
                                    Size = logo.File.ContentLength,
                                    Description = "Logo",
                                    Type = Path.GetExtension(logo.File.FileName),
                                    Location = $"Client/Logo/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/{now}-{logo.File.FileName}"

                                };

                                dservice.Create(doc);

                                string fullpath = Path.Combine(path, $"{now}-{logo.File.FileName}");
                                logo.File.SaveAs(fullpath);
                            }
                        }
                    }

                        #endregion

                        scope.Complete();
                    }

                    Notify("The selected Client details were successfully updated.", NotificationType.Success);

                    return RedirectToAction("Client");                
            }
            catch
            {
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
                return RedirectToAction("Client");
            }
            catch
            {
                return View();
            }
        }

   
        #endregion

        #region Manage Sites
        //
        // GET: /Client/ManageSites
        public ActionResult ManageSites(PagingModel pm, CustomSearchModel csm)
        {

           ViewBag.ViewName = "ManageSites";

            int total = 0;

            List<Site> model = new List<Site>();
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            using (SiteService service = new SiteService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List(pm, csm);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total(pm, csm);
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);
            List<Client> clientList;
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.GetClientsByPSP(pspId);                
            }

            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;

            return PartialView("_ManageSites", paging);
        }

        // GET: Client/AddSite
        [Requires(PermissionTo.Create)]
        public ActionResult AddSite()
        {
            SiteViewModel model = new SiteViewModel() { EditMode = true };
            return View(model);
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
                        Address = model.Address,
                        PostalCode = model.PostalCode,
                        ContactName = model.ContactName,
                        ContactNo = model.ContactNo,
                        PlanningPoint = model.PlanningPoint,
                        SiteType = model.SiteType,
                        AccountCode = model.AccountCode,
                        Depot = model.Depot,
                        SiteCodeChep = model.SiteCodeChep,
                        Status = (int)model.Status
                    };
                    site = siteService.Create(site);
                    #endregion
                    #region Add ClientSite
                    //ClientSite csSite = new ClientSite()
                    //{

                    //}
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

                    scope.Complete();
                }

                Notify("The Site was successfully created.", NotificationType.Success);
                    return RedirectToAction("Client");
                }
                    catch
                    {
                        return View();
            }
        }

        // POST: Client/ImportSites
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult ImportSites(SiteViewImportModel file)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    Notify("Sorry, the Site was not created. Please correct all errors and try again.", NotificationType.Error);

                //    return View(model);
                //}

                //using (SiteService siteService = new SiteService())
                //using (ClientSiteService csService = new ClientSiteService())
                //using (TransactionScope scope = new TransactionScope())
                //using (AddressService aservice = new AddressService())
                //{
                //    #region Validation
                //    if (!string.IsNullOrEmpty(model.AccountCode) && siteService.ExistByAccountCode(model.AccountCode.Trim()))
                //    {
                //        // Bank already exist!
                //        Notify($"Sorry, a Site with the Account number \"{model.AccountCode}\" already exists!", NotificationType.Error);

                //        return View(model);
                //    }
                //    #endregion
                //    #region Create Site
                //    Site site = new Site()
                //    {
                //        Name = model.Name,
                //        Description = model.Description,
                //        XCord = model.XCord,
                //        YCord = model.YCord,
                //        Address = model.Address,
                //        PostalCode = model.PostalCode,
                //        ContactName = model.ContactName,
                //        ContactNo = model.ContactNo,
                //        PlanningPoint = model.PlanningPoint,
                //        SiteType = model.SiteType,
                //        AccountCode = model.AccountCode,
                //        Depot = model.Depot,
                //        SiteCodeChep = model.SiteCodeChep,
                //        Status = (int)model.Status
                //    };
                //    site = siteService.Create(site);
                //    #endregion
                //    #region Add ClientSite
                //    //ClientSite csSite = new ClientSite()
                //    //{

                //    //}
                //    #endregion
                //    #region Create Address (s)

                //    if (model.FullAddress != null)
                //    {
                //        Address address = new Address()
                //        {
                //            ObjectId = model.Id,
                //            ObjectType = "Site",
                //            Town = model.FullAddress.Town,
                //            Status = (int)Status.Active,
                //            PostalCode = model.FullAddress.PostCode,
                //            Type = (int)model.FullAddress.AddressType,
                //            Addressline1 = model.FullAddress.AddressLine1,
                //            Addressline2 = model.FullAddress.AddressLine2,
                //            Province = (int)model.FullAddress.Province,
                //        };

                //        aservice.Create(address);
                //    }

                //    #endregion

                //    scope.Complete();
                //}

                Notify("The Site was successfully created.", NotificationType.Success);
                return RedirectToAction("Client");
            }
            catch
            {
                return View();
            }
        }



        // GET: Client/EditSite/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditSite(int id)
        {
            Site site;

            using (SiteService service = new SiteService())
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
                    }
                };
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

                    if (!string.IsNullOrEmpty(model.AccountCode) && service.ExistByAccountCode(model.AccountCode.Trim()))
                    {
                        // Role already exist!
                        Notify($"Sorry, a Site with the Account Code \"{model.AccountCode} ({model.AccountCode})\" already exists!", NotificationType.Error);

                        return View(model);
                    }

                    #endregion
                    #region Update Site

                    // Update Site
                    site.Id = model.Id;
                    site.Name = model.Name;
                    site.Description = model.Description;
                    site.XCord = model.XCord;
                    site.YCord = model.YCord;
                    site.Address = model.Address;
                    site.PostalCode = model.PostalCode;
                    site.ContactName = model.ContactName;
                    site.ContactNo = model.ContactNo;
                    site.PlanningPoint = model.PlanningPoint;
                    site.SiteType = (int)model.SiteType;
                    site.AccountCode = model.AccountCode;
                    site.Depot = model.Depot;
                    site.SiteCodeChep = model.SiteCodeChep;
                    site.Status = (int)model.Status;

                    service.Update(site);

                    #endregion
                    #region Create Address (s)

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
                    }

                    #endregion



                    scope.Complete();
                }

                Notify("The selected Site details were successfully updated.", NotificationType.Success);

                return RedirectToAction("Client");
            }
            catch
            {
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
                return RedirectToAction("Client");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public string SetSiteStatus(string siteId, string status)
        {
            switch (status)
            {
                case "Terminate":

                    break;
                case "Activate":

                    break;
                default:
                    break;
            }

            return "true";
        }

        #endregion

        #region Sub Sites
        //
        // POST || GET: /Client/SubSites
        public ActionResult SubSites(PagingModel pm, CustomSearchModel csm)
        {

            ViewBag.ViewName = "_SubSites";

            int total = 0;

            List<Site> model = new List<Site>();
            List<Client> clientList;
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);
            List<Site> mainSiteList;

            int pspId = (CurrentUser != null? CurrentUser.PSPs.FirstOrDefault().Id:0);

            using (ClientService clientService = new ClientService())
            using (SiteService sitesService = new SiteService())
            {                
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = sitesService.List();
                total = model.Count;

                mainSiteList = sitesService.GetSitesByClientsOfPSP(pspId);
                clientList = clientService.GetClientsByPSP(pspId);

                //Site firstSite = mainSiteList.FirstOrDefault();
                //if (mainSiteList != null) {
                //    ViewBag.SubSiteListIncluded = sitesService.GetSitesByClientsOfPSPIncluded(pspId, firstSite.Id);
                //    ViewBag.SubbSiteListExcluded = sitesService.GetSitesByClientsOfPSPExcluded(pspId, firstSite.Id);
                //} else
                //{
                //    ViewBag.SubSiteListIncluded = null;
                //    ViewBag.SubbSiteListExcluded = null;
                //}
            }
            IEnumerable<SelectListItem> clientDDL = clientList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName

            });
            ViewBag.ClientList = clientDDL;
            IEnumerable<SelectListItem> siteListDDL = mainSiteList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name

            });
            ViewBag.SiteList = siteListDDL;


            return PartialView("_SubSites", paging);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetSitesForClientSiteIncluded(string clientId, string siteId)
        {
            if (clientId != null && clientId != "")
            {
                List<Site> sites = null;
                int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (SiteService service = new SiteService())
                {

                    sites = service.GetSitesByClientsOfPSPIncluded(pspId, int.Parse(clientId), int.Parse(siteId)); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

                }
                //var jsonList = JsonConvert.SerializeObject(sites);
                return Json(sites, JsonRequestBehavior.AllowGet);
            } else
            {
                return Json(data: "Error", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetSitesForClientSiteExcluded(string clientId, string siteId)
        {
            if (clientId != null && clientId != "")
            {
                List<Site> sites = null;
                int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

                using (SiteService service = new SiteService())
                {

                    sites = service.GetSitesByClientsOfPSPExcluded(pspId, int.Parse(clientId), int.Parse(siteId)); //GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

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
        public JsonResult GetSiteDetailsForMain( string siteId)
        {
            if (siteId != null && siteId != "")
            {
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
            

        [HttpPost]
        public string SetSiteForClientSiteExcluded(string siteId,  string clientId)
        {


            return "true";
        }

        [HttpPost]
        public string SetSiteForClientSiteIncluded(string groupId, string clientId)
        {


            return "true";
        }

        #endregion

        #region Client Group
        //
        // GET: /Client/ClientGroups
        public ActionResult ClientGroups(PagingModel pm, CustomSearchModel csm)
        {
            ViewBag.ViewName = "Group Clients";

            int total = 0;

            List<Group> model = new List<Group>();
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
                if (clientGroup != null)
                {
                    ViewBag.ClientListIncluded = clientService.GetClientsByPSPIncludedGroup(pspId, clientGroup.Id);
                    ViewBag.ClientListExcluded = clientService.GetClientsByPSPExcludedGroup(pspId, clientGroup.Id);
                    //ViewBag.GroupData = groupService.GetGroupsByPSP(pspId);
                }
            }
            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_ClientGroups", paging);
        }

        [HttpPost]
        public string GetClientForGroupIncluded(string groupId)
        {
            List<Client> clients;
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

            using (ClientService clientService = new ClientService())
            {
                
                clients = clientService.GetClientsByPSPIncludedGroup(pspId, int.Parse(groupId));

            }
            var jsonList = JsonConvert.SerializeObject(clients);
            return jsonList;
        }

        [HttpPost]
        public string GetClientForGroupExcluded(string groupId)
        {
            List<Client> clients;
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);

            using (ClientService clientService = new ClientService())
            {

                clients = clientService.GetClientsByPSPExcludedGroup(pspId, int.Parse(groupId));

            }
            var jsonList = JsonConvert.SerializeObject(clients);
            return jsonList;
        }

        [HttpPost]
        public string SetClientForGroupExcluded(string groupId, int clientId)
        {


            return "true";
        }

        [HttpPost]
        public string SetClientForGroupIncludedExcluded(string groupId, int clientId)
        {


            return "true";
        }

        [HttpPost]
        public string SetGroupStatus(string groupId, string status)
        {
            switch (status)
            {
                case "Terminate":

                    break;
                case "Activate":

                    break;
                default:
                    break;
            }

            return "true";
        }

        #endregion

        #region Products
        //
        // POST || GET: /Client/LinkProducts
        public ActionResult LinkProducts(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "Products";

                return PartialView("_LinkProducts", new CustomSearchModel("Products"));
            }

            int total = 0;

            List<Product> model = new List<Product>();
            int pspId = (CurrentUser != null ? CurrentUser.PSPs.FirstOrDefault().Id : 0);
            using (ProductService service = new ProductService())
            {
                model = service.List(pm, csm);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total1(pm, csm);
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);
            List<Client> clientList;
            using (ClientService clientService = new ClientService())
            {
                clientList = clientService.GetClientsByPSP(pspId);
            }

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

            return RedirectToAction("Products");
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

            return RedirectToAction("Products");
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

            return RedirectToAction("Products");
        }

        #endregion

        #region Manage Transporters
        //
        // GET: /Client/ManageTransporters
        public ActionResult ManageTransporters(PagingModel pm, CustomSearchModel csm)
        {

            ViewBag.ViewName = "ManageTransporters";

            int total = 0;

            List<Transporter> model = new List<Transporter>();
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

        #endregion

        #region Awaiting Activation 

        //
        // POST || GET: /Client/AwaitingActivation
        public ActionResult AwaitingActivation(PagingModel pm, CustomSearchModel csm)
        {
            int total = 0;

            List<Client> model = new List<Client>();

            using (ClientService service = new ClientService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";
                if (CurrentUser.PSPs.Count > 0)
                {
                    model = service.GetClientsByPSP(CurrentUser.PSPs.FirstOrDefault().Id);
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

    }
}
