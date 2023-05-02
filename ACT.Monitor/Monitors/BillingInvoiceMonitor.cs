using ACT.Core.Services;
using ACT.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Monitor.Monitors
{
    public class BillingInvoiceMonitor : BaseMonitor
    {
        static StreamWriter logWriter;

        /// <summary>
        /// Indicates if this Monitor is Running
        /// </summary>
        public static bool IsRunning
        {
            get; set;
        }

        public static int LastRunCount { get; set; }


        // <summary>
        // Runs all the operations of the EQ monitor
        // </summary>
        /// <param name = "writer" ></ param >
        // < returns ></ returns >
        public static bool Run(StreamWriter writer)
        {
            if (IsRunning || !ConfigSettings.SystemRules.BillingInvoiceMonitorEnabled) return false;
            {
                return true;
            }
            logWriter = writer;

            IsRunning = true;

            //.Detect
            Info(writer, "  :: BillingInvoiceMonitor :: Process() :: Executed");

            bool success = Process(writer);

            using (SystemConfigService cservice = new SystemConfigService())
            {
                SystemConfig configuration = cservice.GetById(ConfigSettings.SystemRules.Id);

                if (configuration != null)
                {
                    configuration.LastBillingInvoiceMonitorRun = DateTime.Now;

                    cservice.Update(configuration);
                }
            }

            IsRunning = false;

            return success;
        }

        // <summary>
        // Processes all emails in the queue that are still unsent
        // </summary>
        /// <param name = "writer" ></ param >
        /// < param name="from"></param>
        ///<param name = "to" ></ param >

        private static bool Process(StreamWriter writer)
        {
            var commissions = new List<BillingInvoice>();
            try
            {
                using (var a = new AddressService())
                using (var bds = new BankDetailService())
                using (var bs = new BankService())
                using (var bils = new BillingInvoiceLineItemService())
                using (var bis = new BillingInvoiceService())
                using (var pbs = new PSPBillingService())
                using (var pr = new ProvinceService())
                using (var cs = new ClientService())
                using (var ps = new PSPService())
                using (var config = new PSPConfigService())
                {
                    Info(writer, $"    BEGIN:: Processing billing invoices @ {DateTime.Now}");

                    var items = pbs.List().Where(x => x.PaymentAmount == null && x.PaymentDate == null && x.CreatedOn <= DateTime.Now.AddDays(-30));
                    Info(writer, $"    FOUND:: {items.Count()} items to process");

                    foreach (var item in items)
                    {
                        var address = a.GetAddress(item.PSPId);
                        var pspDetails = ps.GetById(item.PSPId);
                        var billingDetails = pbs.GetPSPBillingList(item.PSPId);
                        var pspD = pbs.GetPSPDetails(item.PSPId);
                        var bank = bds.CheckAccount();
                        var cc = config.GetByPsp(item.Id);
                        Province provinceName;
                        Bank bankName;
                        if (address == null)
                        {
                            Info(writer, $"No address captured for this psp {item.PSPId}");
                            continue;
                        }
                        else
                        {
                            provinceName = pr.GetProvinceName(address.ProvinceId);
                        }
                        if (bank == null)
                        {
                            Info(writer, $"No bank details captured for this psp {item.PSPId}");
                            continue;
                        }
                        else
                        {
                            bankName = bs.GetById(bank.BankId);
                        }

                        BillingInvoice billingInvoice = new BillingInvoice();
                        billingInvoice.AddressLine1 = address.Addressline1;
                        billingInvoice.AddressLine2 = address.Addressline2;
                        billingInvoice.Town = address.Town;
                        billingInvoice.Province = provinceName.Name;
                        billingInvoice.PostalCode = address.PostalCode;
                        billingInvoice.AccountNumber = bank.Account;
                        billingInvoice.Bank = bankName.Name;
                        billingInvoice.AccountType = bank.AccountType;
                        billingInvoice.Branch = bank.Branch;
                        billingInvoice.ContactNumber = pspDetails.ContactNumber;
                        billingInvoice.VatNumber = pspDetails.VATNumber;
                        billingInvoice.ContactPerson = pspDetails.ContactPerson;
                        billingInvoice.ContactEmail = pspDetails.FinPersonEmail;
                        billingInvoice.CompanyName = pspDetails.CompanyName;
                        billingInvoice.PSPName = pspDetails.TradingAs;
                        billingInvoice.InvoiceNumber = pspD.ReferenceNumber;
                        billingInvoice.InvoiceAmount = Math.Round((Decimal)pspD.InvoiceAmount, 2);
                        billingInvoice.CreatedOn = pspD.CreatedOn;
                        billingInvoice.ClientId = pspD.ClientId;
                        billingInvoice.PSPId = pspD.PSPId;
                        billingInvoice.StatementDate = pspD.StatementDate;
                        billingInvoice.DueDate = pspD.StatementDate;
                        decimal total = 0.0m;
                        foreach (PSPBilling pb in billingDetails)
                        {
                            billingInvoice.ClientId = pb.ClientId;
                            billingInvoice.InvoiceAmount = pb.InvoiceAmount;
                            billingInvoice.InvoiceNumber = pb.ReferenceNumber;
                            billingInvoice.StatementDate = pb.StatementDate;
                            billingInvoice.CreatedOn = pb.CreatedOn;
                            total += pb.InvoiceAmount;
                        }
                        var bill = bis.Create(billingInvoice);
                        if (bill != null)
                        {
                            BillingInvoiceLineItem bi = new BillingInvoiceLineItem();
                            bi.BillingInvoiceId = bill.Id;
                            bi.InvoiceAmount = billingInvoice.InvoiceAmount;
                            bi.InvoiceNumber = billingInvoice.InvoiceNumber;
                            bi.DueDate = billingInvoice.DueDate;
                            bi.CreatedOn = DateTime.Now;
                            bi.ClientId = billingInvoice.ClientId;
                            bi.PSPId = billingInvoice.PSPId;
                            bi.StatementDate = billingInvoice.StatementDate;
                            bi.ModifiedBy = cc.SystemEmail;
                            bi.ModifiedOn = DateTime.Now;
                            bils.Create(bi);

                        }

                    }
                    // print the invoice here
                }




                var controlReportBuilder = new StringBuilder();
                controlReportBuilder.AppendLine(@"<!DOCTYPE html><html> <head>
                <style>
                    .tab {
                        display: inline-block;
                        margin-left: 40px;
                    }
                </style>
            </head><body>");
                controlReportBuilder.AppendLine("Commission control report");
                controlReportBuilder.AppendLine($"<p>Run date {DateTime.Now.ToShortDateString()}</p>");
                controlReportBuilder.AppendLine($@"			</tbody>			</table>");
                controlReportBuilder.AppendLine(@"	<table border=""1"" cellpadding=""1"" cellspacing=""1"" style=""width:750.6px"">
            				<thead>
            					<tr>
            						<th scope=""col"" style=""width:250px"">Broker</th>
            						<th scope=""col"" style=""width:250px;"">Product</th>
            						<th scope=""col"" style=""width:200px;"">Policy cnt</th>
            						<th scope=""col"" style=""width:50px"">Commission</th>
            					</tr>
            				</thead>
            				<tbody>");


                controlReportBuilder.AppendLine(@"</tbody>
            			</table></body></html>");
                //using (SystemConfigService service = new SystemConfigService())
                //{
                //    SystemConfig rules = service.GetById(ConfigSettings.SystemRules.Id);
                //    EmailInfo to = new EmailInfo(rules.FinancialEmail, rules.FinancialEmail);
                //    SendControlReport(to, controlReportBuilder);
                //}
                //scope.Complete();

                Info(writer, $"    FINISH:: Processing commissions @ {DateTime.Now}");

                return true;
            }

            catch (Exception ex)
            {
                Error(writer, ex, "Process");

                return false;
            }
        }

    }
}




