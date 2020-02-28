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

                model = service.GetClientsByPSP(CurrentUser.PSPs.FirstOrDefault().Id);

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
            {
                model = service.GetById(id);
                if (model == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return RedirectToAction("Index");
                }
                Address address = aservice.Get(model.Id, "PSP");

                List<Document> documents = dservice.List(model.Id, "PSP");

                if (address != null)
                {
                    ViewBag.Address = address;
                }
                if (documents != null)
                {
                    ViewBag.Documents = documents;
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
                using (AddressService aservice = new AddressService())
                using (TransactionScope scope = new TransactionScope())
                using (DocumentService dservice = new DocumentService())
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
                        AdminEmail = model.AdminEmail,
                        CompanyName = model.CompanyName,
                        Description = model.Description,
                        ContactPerson = model.ContactPerson,
                        ContactNumber = model.ContactNumber,
                        Status = (int)model.Status,
                        //ServiceRequired = (int)model.ServiceRequired,
                        CompanyRegistrationNumber = model.CompanyRegistrationNumber
                    };
                    client = service.Create(client);
                    #endregion

                    #region Create Client Budget

                    if (model.ClientBudget != null)
                    {
                        ClientBudget budget = new ClientBudget()
                        {
                            ClientId = model.Id,
                            Status = (int)Status.Active,
                            BudgetYear = DateTime.Now.Year,
                            January = model.ClientBudget.January,
                            February = model.ClientBudget.February,
                            March = model.ClientBudget.March,
                            April = model.ClientBudget.April,
                            May = model.ClientBudget.May,
                            June = model.ClientBudget.June,
                            July = model.ClientBudget.July,
                            August = model.ClientBudget.August,
                            September = model.ClientBudget.September,
                            October = model.ClientBudget.October,
                            November = model.ClientBudget.November,
                            December = model.ClientBudget.December,

                        };

                        budget = bservice.Create(budget);
                    }

                    #endregion

                    #region Create Address (s)

                    if (model.Address != null)
                    {
                        Address address = new Address()
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

                        aservice.Create(address);
                    }

                    #endregion

                    #region Any Uploads
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
                                ObjectId = model.Id,
                                ObjectType = "Client",
                                Status = (int)Status.Active,
                                Name =file.Name,
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

                Address address = aservice.Get(client.Id, "PSP");

                List<Document> documents = dservice.List(client.Id, "PSP");

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
                    ClientBudget = new EstimatedLoadViewModel()
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
                    },
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
                //foreach (FileViewModel file in client.CompanyFile)
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

        // POST: Client/Edit/5
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
                    Address address = aservice.Get(client.Id, "Client");

                    List<Document> documents = dservice.List(client.Id, "Client");

                    if (client == null)
                    {
                        Notify("Sorry, that Client does not exist! Please specify a valid Role Id and try again.", NotificationType.Error);

                        return View(model);
                    }

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
                    client.FinancialPerson = model.FinancialPerson;
                    client.Email = model.Email;
                    client.AdminEmail = model.AdminEmail;
                    client.DeclinedReason = model.DeclinedReason;
                    //client.ServiceRequired = model.ServiceRequired;
                    //Status = (Status)model.Status;
                    client.Status = model.Status;

                    service.Update(client);

                    #endregion

                    #region Update PSP Budget

                    if (model != null)
                    {
                        ClientBudget budget = bservice.GetById(model.ClientBudget.Id);

                        if (budget == null)
                        {
                            budget = new ClientBudget()
                            {
                                ClientId = model.Id,
                                Status = (int)Status.Active,
                                BudgetYear = DateTime.Now.Year,
                                January = model.ClientBudget.January,
                                February = model.ClientBudget.February,
                                March = model.ClientBudget.March,
                                April = model.ClientBudget.April,
                                May = model.ClientBudget.May,
                                June = model.ClientBudget.June,
                                July = model.ClientBudget.July,
                                August = model.ClientBudget.August,
                                September = model.ClientBudget.September,
                                October = model.ClientBudget.October,
                                November = model.ClientBudget.November,
                                December = model.ClientBudget.December,

                            };

                            bservice.Create(budget);
                        }
                        else
                        {
                            budget.BudgetYear = DateTime.Now.Year;
                            budget.January = model.ClientBudget.January;
                            budget.February = model.ClientBudget.February;
                            budget.March = model.ClientBudget.March;
                            budget.April = model.ClientBudget.April;
                            budget.May = model.ClientBudget.May;
                            budget.June = model.ClientBudget.June;
                            budget.July = model.ClientBudget.July;
                            budget.August = model.ClientBudget.August;
                            budget.September = model.ClientBudget.September;
                            budget.October = model.ClientBudget.October;
                            budget.November = model.ClientBudget.November;
                            budget.December = model.ClientBudget.December;

                            bservice.Update(budget);
                        }
                    }

                    #endregion

                    #region Create Address (s)

                    if (model.Address != null)
                    {
                        //    Address address = aservice.GetById(model.Address.Id);

                        //    if (address == null)
                        //    {
                        //        address = new Address()
                        //        {
                        //            ObjectId = model.Id,
                        //            ObjectType = "Client",
                        //            Town = model.Address.Town,
                        //            Status = (int)Status.Active,
                        //            PostalCode = model.Address.PostCode,
                        //            Type = (int)model.Address.AddressType,
                        //            Addressline1 = model.Address.AddressLine1,
                        //            Addressline2 = model.Address.AddressLine2,
                        //            Province = (int)model.Address.Province,
                        //        };

                        //        aservice.Create(address);
                        //    }
                        //    else
                        //    {
                        //        address.Town = model.Address.Town;
                        //        address.PostalCode = model.Address.PostCode;
                        //        address.Type = (int)model.Address.AddressType;
                        //        address.Addressline1 = model.Address.AddressLine1;
                        //        address.Addressline2 = model.Address.AddressLine2;
                        //        address.Province = (int)model.Address.Province;

                        //        aservice.Update(address);
                        //    }
                        //}

                        #endregion

                        #region Any Uploads

                        foreach (FileViewModel file in model.CompanyFile)
                        {
                            if (file.Name != null)
                            {
                                // Create folder
                                string path = Server.MapPath($"~/{VariableExtension.SystemRules.DocumentsLocation}/PSP/{model.CompanyName.Trim()}-{model.CompanyRegistrationNumber.Trim().Replace("/", "_").Replace("\\", "_")}/");

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

                        #endregion

                        scope.Complete();
                    }

                    Notify("The selected Client details were successfully updated.", NotificationType.Success);

                    return RedirectToAction("Client");
                }
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

           //ViewBag.ViewName = "ManageSites";

            int total = 0;

            List<Site> model = new List<Site>();
            int pspId = CurrentUser.PSPs.FirstOrDefault().Id;
            using (SiteService service = new SiteService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List(pm, csm);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total(pm, csm);
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            using(ClientService clientService = new ClientService())
            {
                ViewBag.ClientList = clientService.GetClientsByPSP(pspId);
            }

            return PartialView("_ManageSites", paging);
        }

        #endregion

        #region Sub Sites
        //
        // POST || GET: /Client/SubSites
        public ActionResult SubSites(PagingModel pm, CustomSearchModel csm)
        {

            ViewBag.ViewName = "SubSites";

            int total = 0;

            List<Site> model = new List<Site>();

            using (SiteService service = new SiteService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List();
                total = model.Count;
            }
            int pspId = CurrentUser.PSPs.FirstOrDefault().Id;

            using (ClientService clientService = new ClientService())
            using (SiteService sitesService = new SiteService())
            {
                //ViewBag.ClientListIncluded = clientService.GetClientsByPSPIncludedGroup(pspId, clientGroup.Id);
                ViewBag.ClientList = clientService.GetClientsByPSP(pspId);
                List<Site> mainSiteList = sitesService.GetSitesByClientsOfPSP(pspId);
                ViewBag.MainSiteList = mainSiteList;
                Site firstSite = mainSiteList.FirstOrDefault();
                if (mainSiteList != null) {
                    ViewBag.SubSiteListIncluded = sitesService.GetSitesByClientsOfPSPIncluded(pspId, firstSite.Id);
                    ViewBag.SubbSiteListExcluded = sitesService.GetSitesByClientsOfPSPExcluded(pspId, firstSite.Id);
                } else
                {
                    ViewBag.SubSiteListIncluded = null;
                    ViewBag.SubbSiteListExcluded = null;
                }
            }

            return PartialView("_SubSites");
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
            int pspId = CurrentUser.PSPs.FirstOrDefault().Id;

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
            int pspId = CurrentUser.PSPs.FirstOrDefault().Id;

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
            int pspId = CurrentUser.PSPs.FirstOrDefault().Id;

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

    }
}
