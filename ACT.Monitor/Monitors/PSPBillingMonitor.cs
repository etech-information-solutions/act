using ACT.Core.Enums;
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
    class PSPBillingMonitor : BaseMonitor
    {

        // get pspclient
        //pass psp to pspproduct
        // get bank details
        //process psp billing records
        static StreamWriter logWriter;

        /// <summary>
        /// Indicates if this Monitor is Running
        /// </summary>
        public static bool IsRunning
        {
            get; set;
        }

        /// <summary>
        /// Number of item successfully processed
        /// </summary>
        public static int LastCount
        {
            get; set;
        }

        private static List<string> ErrorLines = new List<string>();

        /// <summary>
        /// Runs all the operations of this monitor 
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool Run(StreamWriter writer)
        {
            if (IsRunning || !ConfigSettings.SystemRules.PSPBillingMonitorEnabled) return false;
            logWriter = writer;

            LastCount = 0;
            IsRunning = true;

            // 1. Excute Process
            Info(writer, ":: PSPBillingMonitor :: Process() :: Executed");

            bool success = Process(writer);

            #region Update Last Run

            using (SystemConfigService service = new SystemConfigService())
            {
                SystemConfig rules = service.GetById(ConfigSettings.SystemRules.Id);

                if (rules != null)
                {
                    rules.LastPSPBillingMonitorRun = DateTime.Now;

                    service.Update(rules);
                }
            }

            #endregion

            IsRunning = false;

            return true;
        }       


        /// <summary>
        /// Processes all Files in the queue that are still Pending
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static bool Process(StreamWriter writer)
        {
            try
            {
                using (var psps = new PSPClientService())
                using (var product = new PSPProductService())
                using (var bank = new BankDetailService())
                using (var config = new PSPConfigService())
                using (var bills = new PSPBillingService())
                {
                    var items = psps.PSPClientList();
                    PSPBilling pSPBilling = new PSPBilling();
                    foreach (var psp in items)
                    {
                        var annualList = product.AnnualPSPProductList(psp.PSPId);
                        var monthList = product.MonthlyPSPProductList(psp.PSPId);
                        var transList = product.TransactionalPSPProductList(psp.PSPId);
                        var cc = config.GetByPsp(psp.Id);
                        DateTime today = DateTime.Today;
                        var month = today.Month;
                        DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                        var bankDetail = bank.CheckAccount();

                        Info(writer, $"    BEGIN:: Processing psp billings @ {DateTime.Now}");

                        Info(writer, $"    FOUND:: {items.Count()} psp clients bills to process");
                        if (monthList != null)
                        {
                            foreach (PSPProduct pp in monthList)
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
                                pb.ModifiedBy = cc.ModifiedBy;
                                pb.ModifiedOn = DateTime.Now;
                                pb.PSPId = psp.PSPId;
                                pb.ClientId = psp.ClientId;
                                pb.PSPProductId = pp.ProductId;
                                pb.ReferenceNumber = bills.GenerateReference(pp.PSPId);

                                bool verifyRecord = bills.CheckPSPBilling(pb);
                                if (verifyRecord == true)
                                {
                                    continue;
                                }
                                else
                                {
                                    PSPBilling pbill = bills.Create(pb);
                                    if (pbill != null)
                                    {
                                        pbill.StatementNumber = pbill.Id;
                                        bills.Update(pbill);
                                    }
                                    else { continue; }
                                }
                            }
                            if (transList != null)
                            {
                                foreach (PSPProduct pp in transList)
                                {

                                    List<PSPProduct> products = product.GetPSPProductList(pp.PSPId, pp.ProductId);
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
                                        pb.ModifiedBy = cc.ModifiedBy;
                                        pb.ModifiedOn = DateTime.Now;
                                        pb.Units = num;
                                        pb.PSPId = psp.PSPId;
                                        pb.ClientId = psp.ClientId;
                                        pb.PSPProductId = pp.ProductId;
                                        pb.ReferenceNumber = bills.GenerateReference(pp.PSPId);

                                        bool verifyRecord = bills.CheckPSPBilling(pb);
                                        if (verifyRecord == true)
                                        {
                                            // Notify($"There is a Active PSP Billing already in the table. Try a different PSP Billing.", NotificationType.Error);
                                            // return RedirectToAction("PSPBilling");
                                            continue;
                                        }
                                        else
                                        {
                                            PSPBilling pbill = bills.Create(pb);
                                            if (pbill != null)
                                            {
                                                pbill.StatementNumber = pbill.Id;
                                                bills.Update(pbill);
                                            }
                                            else { continue; }

                                        }
                                    }
                                }
                            }
                            if (annualList != null && (month == 1 || month == 6))
                            {
                                foreach (PSPProduct pp in annualList)
                                {

                                    List<PSPProduct> products = product.GetPSPProductList(pp.PSPId, pp.ProductId);
                                    decimal rr = 0;
                                    PSPBilling pb = new PSPBilling();

                                    rr += (int)pp.Rate / 2;
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
                                    pb.ModifiedBy = cc.ModifiedBy;
                                    pb.ModifiedOn = DateTime.Now;

                                    pb.PSPId = psp.PSPId;
                                    pb.ClientId = psp.ClientId;
                                    pb.PSPProductId = pp.ProductId;
                                    pb.ReferenceNumber = bills.GenerateReference(pp.PSPId);

                                    bool verifyRecord = bills.CheckPSPBilling(pb);
                                    if (verifyRecord == true)
                                    {
                                        // Notify($"There is a Active PSP Billing already in the table. Try a different PSP Billing.", NotificationType.Error);
                                        // return RedirectToAction("PSPBilling");
                                        continue;
                                    }
                                    else
                                    {
                                        PSPBilling pbill = bills.Create(pb);
                                        if (pbill != null)
                                        {
                                            pbill.StatementNumber = pbill.Id;
                                            bills.Update(pbill);
                                        }
                                        else { continue; }

                                    }
                                }
                            }
                        }
                    }
                        
                    }
               
                return true;
            }
            catch (Exception ex)
            {
                Error(logWriter, ex, "Process");

                return false;
            }
        }


    }
}
