
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Core.Services;
using ACT.Data.Models;
using ACT.UI.Models;
using ACT.UI.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

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
                case "PSPBilling":

                    #region Billing

                    csv = string.Format( "Statement Date,Statement Number,PSP Name,Product Name,Statement Amount,Invoice Amount,Tax Amount,Payment Date,Reference Number,Nominated Account {0}", Environment.NewLine );

                    using ( PSPBillingService bservice = new PSPBillingService() )
                    {
                        List<PSPBillingCustomModel> billing = bservice.List1( pm, csm );

                        if ( billing.NullableAny() )
                        {
                            foreach ( PSPBillingCustomModel item in billing )
                            {
                                csv = string.Format( "{0} {1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} {12}",
                                                    csv,
                                                    "\"" + item.StatementDate + "\"",
                                                    "\"" + item.StatementNumber + "\"",
                                                    "\"" + item.CreatedOn + "\"",
                                                    "\"" + item.PSPName + "\"",
                                                    "\"" + item.ProductName + "\"",
                                                    "\"" + item.PaymentAmount + "\"",
                                                    "\"" + item.InvoiceAmount + "\"",
                                                    "\"" + item.TaxAmount + "\"",
                                                    "\"" + item.PaymentDate + "\"",
                                                    "\"" + item.ReferenceNumber + "\"",
                                                    "\"" + item.NominatedAccount + "\"",
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
                    ViewBag.ViewName = "PSPBilling";

                    return PartialView( "_PSPBillingCustomSearch", new CustomSearchModel( "PSPBilling" ) );
                }

                List<PSPBillingCustomModel> model = bservice.List1( pm, csm );

                int total = ( model.Count < pm.Take && pm.Skip == 0 ) ? model.Count : bservice.Total1( pm, csm );

                PagingExtension paging = PagingExtension.Create( model, total, pm.Skip, pm.Take, pm.Page );

                return PartialView( "_PSPBilling", paging );
            }
        }

        //
        // POST || GET: /Finance/PSPBilling
        public ActionResult PSPBilling(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "PSPBilling";

                return PartialView("_PSPBillingCustomSearch", new CustomSearchModel("PSPBilling"));
            }

            using (PSPBillingService service = new PSPBillingService())
            {
                List<PSPBillingCustomModel> model = service.List1(pm, csm);
                int total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total1(pm, csm);

                PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);

                return PartialView("_PSPBilling", paging);
            }
        }


        //
        // POST || GET: /Finance/BillingInvoice
        public ActionResult BillingInvoice(PagingModel pm, CustomSearchModel csm, bool givecsm = false)
        {
            if (givecsm)
            {
                ViewBag.ViewName = "BillingInvoice";

                return PartialView("_BillingInvoiceCustomSearch", new CustomSearchModel("BillingInvoice"));
            }

            using (BillingInvoiceService service = new BillingInvoiceService())
            {
                pm.Sort = pm.Sort ?? "ASC";
                pm.SortBy = pm.SortBy ?? "c.ReferenceNumber";

                List<BillingInvoiceCustomModel> model = service.List1(pm, csm);

                int total = (model.Count < pm.Take && pm.Skip == 0) ? model.Count : service.Total1(pm, csm);

                PagingExtension paging = PagingExtension.Create(model, total, pm.Skip, pm.Take, pm.Page);


                return PartialView("_BillingInvoice", paging);
            }
        }


        #endregion

        #region Link PSP Billing

        //
        // GET: /Finance/AddPSPBilling
        [Requires(PermissionTo.Create)]
        public ActionResult ProcessAllPSPBilling()
        {
            using (PSPClientService pcservice = new PSPClientService())
            using (PSPProductService pservice = new PSPProductService())
            using (BankDetailService bservice = new BankDetailService())
            using (PSPBillingService cpservice = new PSPBillingService())
            {
                BankDetail bankDetail = bservice.CheckAccount();
                List<PSPProduct> pSPProducts = new List<PSPProduct>();
                var list = pservice.PSPProductList();

                var pspclient = pcservice.PSPClientList();
                DateTime today = DateTime.Today;
                var month = today.Month;
                DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                foreach (PSPClient pc in pspclient)
                {
                    var trans = pservice.TransactionalPSPProductList(pc.PSPId);
                    var monthly = pservice.MonthlyPSPProductList(pc.PSPId);
                    var annually = pservice.AnnualPSPProductList(pc.PSPId);
                    if (monthly == null)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (PSPProduct pp in monthly)
                        {
                            PSPBilling pb = new PSPBilling();
                            pb.Units = 1;
                            pb.Rate = pp.Rate;
                            pb.StatementDate = endOfMonth;
                            pb.CreatedOn = DateTime.Now;
                            pb.NominatedAccount = bankDetail.Account;
                            pb.InvoiceAmount = (int)pp.Rate;
                            pb.TaxAmount = (int)pp.Rate * 0.15M;
                            pb.PaymentAmount = null;
                            pb.Status = (int)Status.Active;
                            pb.PaymentDate = null;
                            pb.ModifiedBy = CurrentUser.Email;
                            pb.ModifiedOn = DateTime.Now;
                            pb.PSPId = pc.PSPId;
                            pb.ClientId = pc.ClientId;
                            pb.PSPProductId = pp.ProductId;
                            pb.ReferenceNumber = cpservice.GenerateReference(pp.PSPId);

                            bool verifyRecord = cpservice.CheckPSPBilling(pb);
                            if (verifyRecord == true)
                            {
                               continue;
                            }
                            else
                            {
                                PSPBilling pbill = cpservice.Create(pb);
                                if (pbill != null)
                                {
                                    pbill.StatementNumber = pbill.Id;
                                    cpservice.Update(pbill);
                                }
                                else { continue; }
                            }
                        }
                        if (trans != null)
                        {
                            foreach (PSPProduct pp in trans)
                            {

                                List<PSPProduct> products = pservice.GetPSPProductList(pp.PSPId, pp.ProductId);
                                int rr = 0;
                                int num = 0;
                                if (products.Count() > 0)
                                {
                                    PSPBilling pb = new PSPBilling();
                                    foreach (var item in products)
                                    {
                                        rr += (int)item.Rate;
                                        num++;
                                    }

                                    pb.Units = num;
                                    pb.Rate = pp.Rate;
                                    pb.StatementDate = endOfMonth;
                                    pb.CreatedOn = DateTime.Now;
                                    pb.NominatedAccount = bankDetail.Account;
                                    pb.InvoiceAmount = rr;
                                    pb.TaxAmount = rr * 0.15M;
                                    pb.PaymentAmount = null;
                                    pb.Status = (int)Status.Active;
                                    pb.PaymentDate = null;
                                    pb.ModifiedBy = CurrentUser.Email;
                                    pb.ModifiedOn = DateTime.Now;
                                    pb.Units = num;
                                    pb.PSPId = pc.PSPId;
                                    pb.ClientId = pc.ClientId;
                                    pb.PSPProductId = pp.ProductId;
                                    pb.ReferenceNumber = cpservice.GenerateReference(pp.PSPId);

                                    bool verifyRecord = cpservice.CheckPSPBilling(pb);
                                    if (verifyRecord == true)
                                    {
                                        // Notify($"There is a Active PSP Billing already in the table. Try a different PSP Billing.", NotificationType.Error);
                                        // return RedirectToAction("PSPBilling");
                                        continue;
                                    }
                                    else
                                    {
                                        PSPBilling pbill = cpservice.Create(pb);
                                        if (pbill != null)
                                        {
                                            pbill.StatementNumber = pbill.Id;
                                            cpservice.Update(pbill);
                                        }
                                        else { continue; }

                                    }
                                }
                            }
                        }
                        if (annually != null && (month==1 || month ==6) )
                        {
                            foreach (PSPProduct pp in annually)
                            {

                                List<PSPProduct> products = pservice.GetPSPProductList(pp.PSPId, pp.ProductId);
                                decimal rr = 0;
                                    PSPBilling pb = new PSPBilling();
                                   
                                        rr += (int)pp.Rate/2;                                     
                                    pb.Units = 1;
                                    pb.Rate = pp.Rate;
                                    pb.StatementDate = endOfMonth;
                                    pb.CreatedOn = DateTime.Now;
                                    pb.NominatedAccount = bankDetail.Account;
                                    pb.InvoiceAmount = rr;
                                    pb.TaxAmount = rr * 0.15M;
                                    pb.PaymentAmount = null;
                                    pb.Status = (int)Status.Active;
                                    pb.PaymentDate = null;
                                    pb.ModifiedBy = CurrentUser.Email;
                                    pb.ModifiedOn = DateTime.Now;
                                    
                                    pb.PSPId = pc.PSPId;
                                    pb.ClientId = pc.ClientId;
                                    pb.PSPProductId = pp.ProductId;
                                    pb.ReferenceNumber = cpservice.GenerateReference(pp.PSPId);

                                    bool verifyRecord = cpservice.CheckPSPBilling(pb);
                                    if (verifyRecord == true)
                                    {
                                        // Notify($"There is a Active PSP Billing already in the table. Try a different PSP Billing.", NotificationType.Error);
                                        // return RedirectToAction("PSPBilling");
                                        continue;
                                    }
                                    else
                                    {
                                        PSPBilling pbill = cpservice.Create(pb);
                                        if (pbill != null)
                                        {
                                            pbill.StatementNumber = pbill.Id;
                                            cpservice.Update(pbill);
                                        }
                                        else { continue; }

                                    }
                                }
                            }
                        }
                    }
                }          

            return PSPBilling(new PagingModel(), new CustomSearchModel());          
        }
        //
        // GET: /Finance/AddPSPBilling
        [Requires(PermissionTo.Create)]
        public ActionResult AddPSPBilling()
        {
            PSPBillingViewModel model = new PSPBillingViewModel() { LinkMode = true };

            return View(model);
        }

        //
        // POST: /Finance/PSPBilling
        [HttpPost]
        [Requires(PermissionTo.Create)]
        public ActionResult AddPSPBilling(PSPBillingViewModel model)
        {
            using (ClientService cservice = new ClientService())
            using (PSPProductService pservice = new PSPProductService())
            using (BankDetailService bservice = new BankDetailService())
            using (PSPBillingService cpservice = new PSPBillingService())
            {
                Client c = cservice.GetById(model.ClientId);

                if (c == null)
                {
                    Notify("Sorry, the selected Client was not found. Please select a valid client and try again.", NotificationType.Error);

                    return View(model);
                }

                if (!c.PSPClients.Any())
                {
                    Notify("Sorry, the selected Client is not linked to any PSP. Please select a valid client and try again or contact us for further assistance.", NotificationType.Error);

                    return View(model);
                }
                model.ProductId = model.PSPProductId;
                PSPProduct p = pservice.CheckPSPProduct(model.PSPId, model.ProductId);
                if (p == null)
                {
                    Notify("You need an active PSPProduct for this PSP and Product to bill on", NotificationType.Error);
                    return View(model);
                }
                if (p.RateUnit == 1 || p.RateUnit ==0)
                { model.Units = 1; }

                BankDetail bankDetail = bservice.CheckAccount();
                PSPBilling pb = new PSPBilling();

                pb.Rate = p.Rate;
                pb.StatementDate = DateTime.Now;
                pb.CreatedOn = DateTime.Now;
                pb.NominatedAccount = bankDetail.Account;
                pb.InvoiceAmount = (decimal)(p.Rate * model.Units);
                pb.TaxAmount = pb.InvoiceAmount * 0.15M;
                pb.PaymentAmount = model.PaymentAmount;
                pb.Status = (int)Status.Active;
                pb.PaymentDate = model.PaymentDate;
                pb.ModifiedBy = CurrentUser.Email;
                pb.ModifiedOn = DateTime.Now;
                pb.StatementNumber = model.StatementNumber;
                pb.Units = model.Units;
                pb.PSPId = p.PSPId;
                pb.ClientId = model.ClientId;
                pb.PSPProductId = p.ProductId;
                pb.ReferenceNumber = cpservice.GenerateReference(model.PSPId);

                bool verifyRecord = cpservice.CheckPSPBilling(pb);
                if (verifyRecord == true)
                {
                    Notify($"There is a Active PSP Billing already in the table. Try a different PSP Billing.", NotificationType.Error);
                    return RedirectToAction("PSPBilling");
                }
                else
                {
                PSPBilling pbill =   cpservice.Create(pb);
                    if(pbill != null)
                    {
                        pbill.StatementNumber = pbill.Id;
                        cpservice.Update(pbill);
                    }                    

                    Notify("The selected Product was successfully linked to the selected client.", NotificationType.Success);
                }
                return PSPBilling(new PagingModel(), new CustomSearchModel());
            }
        }
        #endregion


        //
        // GET: /Finance/PSPBillingDetails/5
        public ActionResult PSPBillingDetails(int id, bool layout = true)
        {
            PSPBilling model;

            using (PSPBillingService service = new PSPBillingService())
            {
                model = service.GetById(id);
            }

            if (model == null)
            {
                Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                return RedirectToAction("Index");
            }

            if (layout)
            {
                ViewBag.IncludeLayout = true;
            }

            return View(model);
        }

        //
        //GET: Finance/ProcessPSP
        public ActionResult ProcessPSP()
        {
            using (PSPBillingService pservice = new PSPBillingService())
            {
                List<int> plist = pservice.GetDistinctPSPBillingList();
              
                    foreach (int item in plist)
                    {
                        List<PSPBilling> list = pservice.GetPSPBillingList(item);
                        if (list == null)
                        {
                            continue;
                        }
                        else
                        { //get client id.
                            GetInvoice(item);

                            foreach (PSPBilling c in list)
                            {
                                var clientpsp = pservice.GetPSPClientDetails(item, c.ClientId);
                                if (clientpsp.Count() == 0)
                                {
                                    continue;
                                }
                                else 
                                { 
                                    GetClientInvoice(c.ClientId);
                                }
                            }
                        }
                    }
                }

            // }

            return BillingInvoice(new PagingModel(), new CustomSearchModel());
        }
        //
        // GET: /Finance/GenerateInvoice/5
        public ActionResult GetInvoice(int id) 
        {
            PSPBilling model;
           
            BillingInvoice bi = new BillingInvoice();
            using (PSPBillingService service = new PSPBillingService())
            {
                model = service.GetById(id);
            }

            if (model == null)
            {
                Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);
               // continue;
                // return RedirectToAction("Index");
            }
            else { 
            int psp = model.PSPId;
            DateTime dat = DateTime.Today;
                using (AddressService aservice = new AddressService())
                using (BankDetailService bservice = new BankDetailService())
                using (BankService bankservice = new BankService())
                using (PSPService pservice = new PSPService())
                using (BillingInvoiceLineItemService bilcservice = new BillingInvoiceLineItemService())
                using (BillingInvoiceService biservice = new BillingInvoiceService())
                using (ProvinceService psservice = new ProvinceService())
                using (PSPBillingService service = new PSPBillingService())
                {
                    var address = aservice.GetAddress(psp);
                    var bank = bservice.CheckAccount();
                    var pspDetails = pservice.GetById(psp);
                    var billingDetails = service.GetPSPBillingList(psp);
                    var unique = service.GetDistinctPSPBillingList();
                    var pspD = service.GetPSPDetails(psp);
                   
                    List<PSPBilling> pSPBillings = new List<PSPBilling>();                  

                    if (address != null)
                    {
                        var provinceName = psservice.GetProvinceName(address.ProvinceId);
                        bi.AddressLine1 = address.Addressline1;
                        bi.AddressLine2 = address.Addressline2;
                        bi.PostalCode = address.PostalCode;
                        if (provinceName != null)
                        { bi.Province = provinceName.Name; }
                        bi.Town = address.Town;
                    }
                    if (bank != null)
                    {
                        var bankName = bankservice.GetById(bank.BankId);
                        bi.AccountNumber = bank.Account;
                        bi.Bank = bankName.Name;
                        bi.AccountType = bank.AccountType;
                        bi.Branch = bank.Branch;
                    }
                    if (pspDetails != null)
                    {
                        bi.ContactNumber = pspDetails.ContactNumber;
                        bi.VatNumber = pspDetails.VATNumber;
                        bi.ContactPerson = pspDetails.ContactPerson;
                        bi.ContactEmail = pspDetails.FinPersonEmail;
                        bi.CompanyName = pspDetails.CompanyName;
                        bi.PSPName = pspDetails.TradingAs;
                    }
                    if (pspD != null)
                    { //one billing
                        bi.InvoiceNumber = pspD.ReferenceNumber;
                        bi.InvoiceAmount = Math.Round((Decimal)pspD.InvoiceAmount, 2);
                        bi.CreatedOn = pspD.CreatedOn;
                        bi.ClientId = pspD.ClientId;
                        bi.PSPId = pspD.PSPId;
                        bi.StatementDate = pspD.StatementDate;
                        bi.DueDate = pspD.StatementDate;

                    }
                    if (billingDetails.Count() > 0)
                    {
                        decimal total = 0;
                        foreach (PSPBilling pb in billingDetails.ToList())
                        {
                            PSPBilling ps = new PSPBilling();
                            ps.InvoiceAmount = Math.Round((Decimal)pb.InvoiceAmount, 2);
                            ps.ReferenceNumber = pb.ReferenceNumber;
                            bi.InvoiceNumber = pb.ReferenceNumber;
                            ps.StatementDate = pb.StatementDate;
                            ps.CreatedOn = pb.CreatedOn;
                            ps.ClientId = pb.ClientId;
                            total += ps.InvoiceAmount;
                            pSPBillings.Add(pb);

                            DateTime dt = (DateTime)ps.StatementDate;
                            int result = DateTime.Compare(dat, dt);
                            if (result != 0)
                            {
                                dat = dt;
                            }
                        }
                        bi.StatementDate = dat;
                        bi.DueDate = bi.StatementDate;
                        bi.Total = total;
                        bi.Status = (int)Status.Active;
                        bi.InvoiceType = (int)InvoiceType.PSP;
                        bi.ModifiedBy = CurrentUser.Email;
                        bi.ModifiedOn = DateTime.Now;
                        bi.PSPBillingId = model.Id;
                    }
                    bool verifyRecord = biservice.CheckBillingInvoice(bi);
                    if (verifyRecord == true)
                    {
                        Notify($"There is a Active Billing invoice already in the table. Try a different PSP Billing.", NotificationType.Error);
                        //return RedirectToAction("PSPBilling");
                       // continue;
                    }
                    else
                    {

                        BillingInvoice pbill = biservice.Create(bi);
                        if (pbill != null)
                        {
                            foreach (var item in billingDetails.ToList())
                            {
                                BillingInvoiceLineItem line = new BillingInvoiceLineItem();
                                line.InvoiceAmount = item.InvoiceAmount;
                                line.InvoiceNumber = item.ReferenceNumber;
                                line.PSPId = item.PSPId;
                                line.ClientId = item.ClientId;
                                line.BillingInvoiceId = pbill.Id;
                                line.StatementDate = item.StatementDate;
                                line.DueDate = item.StatementDate;
                                line.CreatedOn = pbill.CreatedOn;
                                line.ModifiedBy = CurrentUser.Email;
                                line.ModifiedOn = pbill.ModifiedOn;
                                bilcservice.Create(line);
                            }
                            var resulted = PrintPSPInvoice(pbill.Id);

                            if (resulted != null)
                            {
                                return View(bi);
                            }
                        }

                    }
                    return View(bi);
                }                
            }
            return View(bi);
        }

        // GET: /Finance/GenerateClientInvoice/5
        public ActionResult GetClientInvoice(int id) 
        {
            PSPBilling model;

            using (PSPBillingService service = new PSPBillingService())
            {
                model = service.GetById(id);
            }


            if (model == null)
            {
                Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                return RedirectToAction("Index");
            }
            int client = model.ClientId;
            DateTime dat = DateTime.Today;
            using (AddressService aservice = new AddressService())
            using (BankDetailService bservice = new BankDetailService())
            using (BankService bankservice = new BankService())
            using (PSPService pservice = new PSPService())
            using (ProvinceService psservice = new ProvinceService()) 
            using (BillingInvoiceService biservice = new BillingInvoiceService())
            using (ClientService cservice = new ClientService())
           using (BillingInvoiceLineItemService  bilcservice = new BillingInvoiceLineItemService())
            using (PSPBillingService service = new PSPBillingService())
            {
                var address = aservice.GetAddress(client);
                var bank = bservice.CheckAccount();
                var pspDetails = pservice.GetById(model.PSPId);
                var unique = service.GetDistinctPSPBillingList();
                var cli = cservice.GetClientById(client);
                var billingDetails = service.GetClientBillingList(client, model.PSPId);
                var clientD = service.GetClientDetails(client);
               
                List<PSPBilling> pSPBillings = new List<PSPBilling>();
                BillingInvoice bi = new BillingInvoice();

                if (address != null)
                {
                    var provinceName = psservice.GetProvinceName(address.ProvinceId);
                    bi.AddressLine1 = address.Addressline1;
                    bi.AddressLine2 = address.Addressline2;
                    bi.PostalCode = address.PostalCode;
                    if (provinceName != null)
                    { bi.Province = provinceName.Name; }
                    bi.Town = address.Town;

                }
                if (bank != null)
                {
                    var bankName = bankservice.GetById(bank.BankId);
                    bi.AccountNumber = bank.Account;
                    bi.Bank = bankName.Name;
                    bi.Branch = bank.Branch;
                    bi.AccountType =  bank.AccountType;
                }
                if (cli != null)
                {
                    bi.ContactNumber = cli.ContactNumber;
                    bi.VatNumber = cli.VATNumber;
                    bi.ContactPerson = cli.ContactPerson;
                    bi.ContactEmail = cli.FinPersonEmail;
                    bi.CompanyName = cli.CompanyName;
                    bi.ClientName = cli.TradingAs;
                }
                if (clientD != null && pspDetails != null)
                { //one billing
                    bi.InvoiceNumber = clientD.ReferenceNumber;
                    bi.InvoiceAmount = Math.Round((Decimal)clientD.InvoiceAmount, 2);
                    bi.CreatedOn = clientD.CreatedOn; 
                    bi.ClientId = clientD.ClientId;
                    bi.PSPId = clientD.PSPId;
                    bi.PSPName = pspDetails.TradingAs;
                    bi.StatementDate = clientD.StatementDate; 
                    bi.DueDate = clientD.StatementDate; 

                }
                if (billingDetails.Count() > 0)
                {
                    decimal total = 0;
                    foreach (PSPBilling pb in billingDetails.ToList())
                    {
                        PSPBilling ps = new PSPBilling();
                        ps.InvoiceAmount = Math.Round((Decimal)pb.InvoiceAmount, 2);
                        ps.ReferenceNumber = pb.ReferenceNumber;
                        bi.InvoiceNumber = pb.ReferenceNumber;
                        ps.StatementDate = pb.StatementDate; 
                        ps.CreatedOn = pb.CreatedOn; 
                        ps.PSPId = pb.PSPId;
                        total += ps.InvoiceAmount;
                        pSPBillings.Add(pb);
                        DateTime dt = (DateTime)ps.StatementDate;
                        int result = DateTime.Compare(dat, dt);
                        if (result!= 0)
                        {
                            dat = dt;
                        }                       
                    }
                    bi.StatementDate = dat; 
                    bi.DueDate = bi.StatementDate;
                    bi.Total = total;
                    bi.Status = (int)Status.Active;
                    bi.InvoiceType = (int)InvoiceType.Client;
                    bi.ModifiedBy = CurrentUser.Email;
                    bi.ModifiedOn = DateTime.Now;
                    bi.PSPBillingId = model.Id;
                }
                bool verifyRecord = biservice.CheckBillingInvoice(bi);
                if (verifyRecord == true)
                {
                    Notify($"There is a Active Billing invoice already in the table. Try a different PSP Billing.", NotificationType.Error);
                    //return RedirectToAction("PSPBilling");
                }
                else
                {
                    BillingInvoice pbill = biservice.Create(bi);
                    if (pbill != null)
                    {
                        foreach (var item in billingDetails.ToList())
                        {
                            BillingInvoiceLineItem line = new BillingInvoiceLineItem();
                            line.InvoiceAmount = item.InvoiceAmount;
                            line.InvoiceNumber = item.ReferenceNumber;
                            line.PSPId = item.PSPId;
                            line.ClientId = item.ClientId;
                            line.BillingInvoiceId = pbill.Id;
                            line.StatementDate = item.StatementDate;
                            line.DueDate = item.StatementDate;
                            line.CreatedOn = pbill.CreatedOn;
                            line.ModifiedBy = CurrentUser.Email;
                            line.ModifiedOn = pbill.ModifiedOn;
                            bilcservice.Create(line);
                        }

                        var resulted = PrintClientInvoice(pbill.Id);

                        if (resulted != null)
                        {
                            return View(bi);
                        }
                    }
                }
                return View(bi);
            }
        }

        
        //
        // GET: /Administration/EditPSPProduct/5
        [Requires(PermissionTo.Edit)]
        public ActionResult EditPSPBilling(int id)
        {
            using (PSPBillingService service = new PSPBillingService())
            {
                PSPBilling billing = service.GetById(id);

                if (billing == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                PSPBillingViewModel model = new PSPBillingViewModel()
                {

                    EditMode = true,
                    Status = (Status)billing.Status,
                    ClientId = (int)billing.ClientId,
                    PSPId = (int)billing.PSPId,
                    NominatedAccount = billing.NominatedAccount,
                    PaymentAmount = billing.PaymentAmount,
                    PaymentDate = billing.PaymentDate,
                    StatementDate =  billing.StatementDate, //.ToString("dd/MM/yyyy"),
                    StatementNumber= billing.StatementNumber,
                    ReferenceNumber= billing.ReferenceNumber,
                    TaxAmount= billing.TaxAmount,
                    Rate= billing.Rate,
                    Units= billing.Units,
                    CreatedOn = billing.CreatedOn, 
                    ModifiedOn = DateTime.Now,
                    InvoiceAmount = (decimal) billing.InvoiceAmount
                };

                return View(model);
            }
        }

        //
        // POST: /Administration/EditPSPProduct/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult EditPSPBilling(PSPBillingViewModel model, PagingModel pm)
        {
            if (!ModelState.IsValid)
            {
                Notify("Sorry, the selected User was not updated. Please correct all errors and try again.", NotificationType.Error);

                return View(model);
            }

            using (PSPBillingService prodservice = new PSPBillingService())
            using (RoleService rservice = new RoleService())
            {
                PSPBilling prod = prodservice.GetById(model.Id);

                if (prod == null)
                {
                    Notify("Sorry, that psp billing does not exist! Please specify a valid psp link Id and try again.", NotificationType.Error);

                    return View(model);
                }

                #region Validations

                #endregion
                #region Update psp billing



                // Update psp billing

                prod.ModifiedBy = CurrentUser.Email;
                prod.InvoiceAmount = model.InvoiceAmount;
                prod.Rate = model.Rate;
                prod.NominatedAccount = model.NominatedAccount;
                prod.CreatedOn = Convert.ToDateTime(model.CreatedOn);
                prod.PaymentAmount = model.PaymentAmount;
                prod.ModifiedOn = DateTime.Now;
                prod.PaymentAmount = model.PaymentAmount;
                prod.PaymentDate = model.PaymentDate;
                prod.PSPId = model.PSPId;
                prod.ReferenceNumber = model.ReferenceNumber;
                prod.StatementDate = Convert.ToDateTime(model.StatementDate);
                prod.StatementNumber = model.StatementNumber;
                prod.Status = (int)model.Status;
                prod.TaxAmount = model.TaxAmount;
                prod.Units = model.Units;
                prod = prodservice.Update(prod);

                #endregion

                Notify("The selected psp billing's details were successfully updated.", NotificationType.Success);

                return RedirectToAction("PSPBilling");
            }
        }

        //
        // GET: /Administration/ProcessPSPProduct/5
        [Requires(PermissionTo.Edit)]
        public ActionResult ProcessPSPBilling(int id)
        {
            using (PSPBillingService service = new PSPBillingService())
            {
                PSPBilling billing = service.GetById(id);

                if (billing == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                PSPBillingViewModel model = new PSPBillingViewModel()
                {

                    EditMode = true,
                    Status = (Status)billing.Status,
                    ClientId = (int)billing.ClientId,
                    PSPId = (int)billing.PSPId,
                    NominatedAccount = billing.NominatedAccount,
                    PaymentAmount = billing.PaymentAmount,
                    PaymentDate = billing.PaymentDate,
                    StatementDate = billing.StatementDate, 
                    StatementNumber = billing.Id,
                    ReferenceNumber = billing.ReferenceNumber,
                    TaxAmount = billing.TaxAmount,
                    Rate = billing.Rate,
                    Units = billing.Units,
                    CreatedOn = billing.CreatedOn, 
                    ModifiedOn = DateTime.Now,
                    InvoiceAmount = (decimal)billing.InvoiceAmount
                };

                return View(model);
            }
        }

        //
        // POST: /Administration/ProcessPSPProduct/5
        [HttpPost]
        [Requires(PermissionTo.Edit)]
        public ActionResult ProcessPSPBilling(PSPBillingViewModel model, PagingModel pm)
        {
            if (!ModelState.IsValid)
            {
                Notify("Sorry, the selected User was not updated. Please correct all errors and try again.", NotificationType.Error);

                return View(model);
            }

            using (PSPBillingService prodservice = new PSPBillingService())
            using (RoleService rservice = new RoleService())
            {
                PSPBilling prod = prodservice.GetById(model.Id);

                if (prod == null)
                {
                    Notify("Sorry, that psp billing does not exist! Please specify a valid psp link Id and try again.", NotificationType.Error);

                    return View(model);
                }

                #region Validations

                #endregion
                #region Update psp billing

                // Update psp billing

                prod.ModifiedBy = CurrentUser.Email;
                prod.InvoiceAmount = model.InvoiceAmount;
                prod.Rate = model.Rate;
                prod.NominatedAccount = model.NominatedAccount;
                prod.CreatedOn = Convert.ToDateTime(model.CreatedOn);
                prod.PaymentAmount = model.PaymentAmount;
                prod.ModifiedOn = DateTime.Now;
                prod.PaymentAmount = model.PaymentAmount;
                prod.PaymentDate = model.PaymentDate;
                prod.PSPId = model.PSPId;
                prod.ReferenceNumber = model.ReferenceNumber;
                prod.StatementDate = Convert.ToDateTime(model.StatementDate);
                prod.StatementNumber = model.StatementNumber;
                prod.Status = (int)Status.Active;
                prod.TaxAmount = model.TaxAmount;
                prod.Units = model.Units;
                prod = prodservice.Update(prod);

                #endregion

                Notify("The selected psp billing's details were successfully updated.", NotificationType.Success);

                return RedirectToAction("PSPBilling");
            }
        }
        //
        // POST: /Finance/PrintClientInvoice
        public ActionResult PrintClientInvoice(int id)
        {
            BillingInvoice model;
            using (BillingInvoiceService service = new BillingInvoiceService())
            {
                model = service.GetBillingInvoice(id);
            }

            if (model == null)
                return PartialView("_AccessDenied");

            string body = RenderViewToString(ControllerContext, "~/Views/Shared/_ClientInvoice.cshtml", model, true);

            MemoryStream stream = new MemoryStream();

            using (iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10, 10, 10, 10))
            {
               // PdfWriter pWriter = PdfWriter.GetInstance(document, stream);
                PdfWriter pWriter = PdfWriter.GetInstance(document, new FileStream(@"C:\inetpub\wwwroot\client\" + model.InvoiceNumber +".pdf", FileMode.Create));

                PdfAction action = new PdfAction(PdfAction.PRINTDIALOG);
                pWriter.SetOpenAction(action);

                document.Open();

                XMLWorkerHelper.GetInstance().ParseXHtml(pWriter, document, new StringReader(body));
                // XMLWorkerHelper.GetInstance().ParseXHtml(pWriter1, document, new StringReader(body));
            }

            // return File( stream.ToArray(), "application/pdf" );
            return null;// File( new MemoryStream(), "application/pdf");
        }

        //
        // POST: /Finance/PrintPSPInvoice
        public ActionResult PrintPSPInvoice(int id)
        {
            BillingInvoice model;
        using (BillingInvoiceService service = new BillingInvoiceService())
        {
            model = service.GetById(id);
        }
            
        if (model == null)
                    return PartialView("_AccessDenied");

                string body = RenderViewToString(ControllerContext, "~/Views/Shared/_PSPInvoice.cshtml", model, true);

                MemoryStream stream = new MemoryStream();

                using (iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10, 10, 10, 10))
                {
                    //PdfWriter pWriter = PdfWriter.GetInstance(document, stream);
                 PdfWriter pWriter = PdfWriter.GetInstance(document, new FileStream(@"C:\inetpub\wwwroot\psp\" + model.InvoiceNumber +".pdf", FileMode.Create));

                PdfAction action = new PdfAction(PdfAction.PRINTDIALOG);
                    pWriter.SetOpenAction(action);

                    document.Open();

                    XMLWorkerHelper.GetInstance().ParseXHtml(pWriter, document, new StringReader(body));
                   //  XMLWorkerHelper.GetInstance().ParseXHtml(pWriter1, document, new StringReader(body));
                }

                // return File( stream.ToArray(), "application/pdf" );
                return null;// File( new MemoryStream(), "application/pdf");
           // }
        }

        //
        // POST: /Administration/DeletePSPBilling/5
        [HttpPost]
        [Requires(PermissionTo.Delete)]
        public ActionResult DeletePSPBilling(PSPBillingViewModel model, PagingModel pm)
        {
            using (PSPBillingService service = new PSPBillingService())
            {
                PSPBilling prod = service.GetById
                    (model.Id);

                if (prod == null)
                {
                    Notify("Sorry, the requested resource could not be found. Please try again", NotificationType.Error);

                    return PartialView("_AccessDenied");
                }

                prod.Status = (((Status)prod.Status) == Status.Active) ? (int)Status.Inactive : (int)Status.Active;

                service.Update(prod);

                Notify("The selected psp product link was successfully updated.", NotificationType.Success);

                return RedirectToAction("PSPBilling");
            }
        }


        // #endregion

       
    }
}
