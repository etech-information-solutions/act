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

namespace ACT.UI.Controllers
    {
        public class ClientController : BaseController
    {
        // GET: Client
        public ActionResult Index()
        {
                  return View();
        }

        // GET: Client/Create
        [Requires(PermissionTo.Create)]
        public ActionResult Create()
        {
            //PSPClient model = new PSPClient();
            ClientViewModel model = new ClientViewModel();
            return View(model);
        }

        // POST: Client/Create
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult Create(ClientViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Sorry, the Client was not created. Please correct all errors and try again.", NotificationType.Error);

                    return View(model);
                }

                Client client = new Client();

                using (ClientService service = new ClientService())
                {
                    if (service.ExistByCompanyRegistrationNumber(model.CompanyRegistrationNumber))
                    {
                        // Bank already exist!
                        Notify($"Sorry, a Client with the Registration number \"{model.CompanyRegistrationNumber}\" already exists!", NotificationType.Error);

                        return View(model);
                    }

                    client.CompanyName = model.CompanyName;
                    client.CompanyRegistrationNumber = model.CompanyRegistrationNumber;
                    client.Description = model.Description;
                    client.Status = (int)model.Status;
                    client.ContactNumber = model.ContactNumber;

                    client = service.Create(client);

                    Notify("The Bank was successfully created.", NotificationType.Success);
                }

                return RedirectToAction("Client");
            }
            catch
            {
                return View();
            }
        }

        // GET: Client/Edit/5
        [Requires(PermissionTo.Edit)]
        public ActionResult Edit(int id)
        {
            Client client;

            using (ClientService service = new ClientService())
            {
                client = null;// service.GetById(id);
            }

            if (client == null)
            {
                Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                return PartialView("_AccessDenied");
            }

            ClientViewModel model = new ClientViewModel()
            {
                //Id = bank.Id,
                //Name = bank.Name,
                //Description = bank.Description,
                //Code = bank.Code,
                //Status = (Status)bank.Status,
                EditMode = true
            };

            return View(model);
        }

        // POST: Client/Edit/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult Edit(ClientViewModel model)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Client/Delete/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult Delete(ClientViewModel model)
        {
            Client client;
            try
            {

                using (ClientService service = new ClientService())
                {
                    client = service.GetById(model.Id);

                    if (client == null)
                    {
                        Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                        return PartialView("_AccessDenied");
                    }

                    client.Status = (((Status)client.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;

                    service.Update(client);

                    Notify("The selected Client was successfully updated.", NotificationType.Success);
                }

                return RedirectToAction("Client");
            }
            catch
            {
                return View();
            }
        }


        //
        // POST || GET: /Client/Clients
        public ActionResult Clients(PagingModel pm, CustomSearchModel csm)
        {
            int total = 0;

            List<Role> model = new List<Role>();

            using (RoleService service = new RoleService())
            {
                pm.Sort = pm.Sort ?? "DESC";
                pm.SortBy = pm.SortBy ?? "CreatedOn";

                model = service.List(pm, csm);
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total(pm, csm);
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            return PartialView("_Roles", paging);
        }

        #region ClientList
        //
        // POST || GET: /Client/ClientList
        public ActionResult ClientList(PagingModel pm)
        {

            ViewBag.ViewName = "ClientList";

            int total = 0;

            List<Client> model = new List<Client>();

            using (ClientService service = new ClientService())
            {
                model = service.List();
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);


            return PartialView("_ClientList", paging);
        }
        #endregion

        #region Manage Sites
        //
        // POST || GET: /Client/ManageSites
        public ActionResult ManageSites(PagingModel pm)
        {

           ViewBag.ViewName = "ManageSites";

            int total = 0;

            List<Site> model = new List<Site>();

            using (SiteService service = new SiteService())
            {
                model = service.List();
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            using(ClientService clients = new ClientService())
            {
                var clientList = new SelectList(clients.OldObject.ToDictionary(), "Id", "CompanyName");
                ViewData["ClientList"] = clientList;
            }

            return PartialView("_ManageSites", paging);
        }

        #endregion

        #region Sub Sites
        //
        // POST || GET: /Client/SubSites
        public ActionResult SubSites(PagingModel pm)
        {

            ViewBag.ViewName = "SubSites";

            int total = 0;

            List<Site> model = new List<Site>();

            using (SiteService service = new SiteService())
            {
                model = service.List();
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

            using (ClientService clients = new ClientService())
            {
                var clientList = new SelectList(clients.OldObject.ToDictionary(), "Id", "CompanyName");
                ViewData["ClientList"] = clientList;
            }
            using (SiteService sites = new SiteService())
            {
                var siteList = new SelectList(sites.OldObject.ToDictionary(), "Id", "Name");
                ViewData["MainSiteList"] = siteList;
            }

            return PartialView("_SubSites", paging);
        }

        #endregion

        #region Client Group
        //
        // POST || GET: /Client/ClientGroup
        public ActionResult ClientGroup(PagingModel pm)
        {

            ViewBag.ViewName = "ClientGroup";

            int total = 0;

            List<ClientGroup> model = new List<ClientGroup>();

            using (ClientGroupService service = new ClientGroupService())
            {
                model = service.List();
                total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total();
            }

            PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);


            return PartialView("_ClientGroup", paging);
        }

        #endregion

    }
}
