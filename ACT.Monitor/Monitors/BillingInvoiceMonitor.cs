//using ACT.Core.Services;
//using ACT.Data.Models;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ACT.Monitor.Monitors
//{
//    public class BillingInvoiceMonitor : BaseMonitor
//    {
//        static StreamWriter logWriter;

//        /// <summary>
//        /// Indicates if this Monitor is Running
//        /// </summary>
//        public static bool IsRunning
//        {
//            get; set;
//        }

//        public static int LastRunCount { get; set; }


//        /// <summary>
//        /// Runs all the operations of the EQ monitor
//        /// </summary>
//        /// <param name="writer"></param>
//        /// <returns></returns>
//        public static bool Run(StreamWriter writer)
//        {
//            if (IsRunning || !ConfigSettings.SystemRules.BillingInvoiceMonitorEnabled) return false;
//            {
//                return true;
//            }
//            logWriter = writer;

//            IsRunning = true;

//            // 1. Detect
//            Info(writer, "  :: BillingInvoiceMonitor :: Process() :: Executed");

//            bool success = Process(writer);

//            using (SystemConfigService cservice = new SystemConfigService())
//            {
//                SystemConfig configuration = cservice.GetById(ConfigSettings.SystemRules.Id);

//                if (configuration != null)
//                {
//                    configuration.LastBillingInvoiceMonitorRun = DateTime.Now;

//                    cservice.Update(configuration);
//                }
//            }

//            IsRunning = false;

//            return success;
//        }

//        /// <summary>
//        /// Processes all emails in the queue that are still unsent
//        /// </summary>
//        /// <param name="writer"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        private static bool Process(StreamWriter writer)
//        {
//            var commissions = new List<BillingInvoice>();
//            try
//            {
//                using (var a = new AddressService())
//                using (var bds = new BankDetailService())
//                using (var bs = new BankService())
//                using (var bils = new BillingInvoiceLineItemService())
//                using (var bis = new BillingInvoiceService())
//                using (var pbs = new PSPBillingService())
//                using (var pr = new ProvinceService())
//                using (var cs = new ClientService())
//                using (var ps = new PSPService())

//                {
//                    Info(writer, $"    BEGIN:: Processing billing invoices @ {DateTime.Now}");

//                    var items = pbs.List().Where(x => x.PaymentAmount == null && x.PaymentDate == null && x.CreatedOn <= DateTime.Now.AddDays(-30));
//                    Info(writer, $"    FOUND:: {items.Count()} items to process");

//                    // BrokerProductCommission brokerProductCommission;

//                    foreach (var item in items)
//                    {
//                        var address = a.GetAddress(item.PSPId);
//                        var pspDetails = ps.GetById(item.PSPId);
//                        var billingDetails = pbs.GetPSPBillingList(item.PSPId);
//                        var pspD = pbs.GetPSPDetails(item.PSPId);
//                        var bank = bds.CheckAccount();
//                        Province provinceName;
//                        Bank bankName;
//                        if (address == null)
//                        {
//                            Info(writer, $"No address captured for this psp {item.PSPId}");
//                            continue;
//                        }
//                        else
//                        {
//                            provinceName = pr.GetProvinceName(address.ProvinceId);
//                        }
//                        if (bank == null)
//                        {
//                            Info(writer, $"No bank details captured for this psp {item.PSPId}");
//                            continue;
//                        }
//                        else
//                        {
//                            bankName = bs.GetById(bank.BankId);
//                        }

//                        BillingInvoice billingInvoice = new BillingInvoice();
//                        billingInvoice.AddressLine1 = address.Addressline1;
//                        billingInvoice.AddressLine2 = address.Addressline2;
//                        billingInvoice.Town = address.Town;
//                        billingInvoice.Province = provinceName.Name;
//                        billingInvoice.PostalCode = address.PostalCode;
//                        billingInvoice.AccountNumber = bank.Account;
//                        billingInvoice.Bank = bankName.Name;
//                        billingInvoice.AccountType = bank.AccountType;
//                        billingInvoice.Branch = bank.Branch;
//                        billingInvoice.ContactNumber = pspDetails.ContactNumber;
//                        billingInvoice.VatNumber = pspDetails.VATNumber;
//                        billingInvoice.ContactPerson = pspDetails.ContactPerson;
//                        billingInvoice.ContactEmail = pspDetails.FinPersonEmail;
//                        billingInvoice.CompanyName = pspDetails.CompanyName;
//                        billingInvoice.PSPName = pspDetails.TradingAs;
//                        billingInvoice.InvoiceNumber = pspD.ReferenceNumber;
//                        billingInvoice.InvoiceAmount = Math.Round((Decimal)pspD.InvoiceAmount, 2);
//                        billingInvoice.CreatedOn = pspD.CreatedOn;
//                        billingInvoice.ClientId = pspD.ClientId;
//                        billingInvoice.PSPId = pspD.PSPId;
//                        billingInvoice.StatementDate = pspD.StatementDate;
//                        billingInvoice.DueDate = pspD.StatementDate;


//                        //decimal? commission = GetCommission(brokerProductCommission, cnt);

//                        //FSPCommission fSPCommission = new FSPCommission();
//                        //fSPCommission.CommissionAmt = commission;
//                        //fSPCommission.CommissionDate = DateTime.Now;
//                        //fSPCommission.FSPId = fspId;
//                        //fSPCommission.BrokerId = broker.Id;
//                        //fSPCommission.MemberCollectionId = item.Id;
//                        //fSPCommission.MemberProductId = memberproduct.Id;
//                        //fSPCommission.UserId = userBroker.UserId;

//                        //fspcs.Create(fSPCommission);
//                        // commissions.Add(fSPCommission);

//                    }
//                }
//            }


//            //                    var controlReportBuilder = new StringBuilder();
//            //                    controlReportBuilder.AppendLine(@"<!DOCTYPE html><html> <head>
//            //    <style>
//            //        .tab {
//            //            display: inline-block;
//            //            margin-left: 40px;
//            //        }
//            //    </style>
//            //</head><body>");
//            //                    controlReportBuilder.AppendLine("Commission control report");
//            //                    controlReportBuilder.AppendLine($"<p>Run date {DateTime.Now.ToShortDateString()}</p>");
//            //                    controlReportBuilder.AppendLine($@"			</tbody>			</table>");
//            //                    controlReportBuilder.AppendLine(@"	<table border=""1"" cellpadding=""1"" cellspacing=""1"" style=""width:750.6px"">
//            //				<thead>
//            //					<tr>
//            //						<th scope=""col"" style=""width:250px"">Broker</th>
//            //						<th scope=""col"" style=""width:250px;"">Product</th>
//            //						<th scope=""col"" style=""width:200px;"">Policy cnt</th>
//            //						<th scope=""col"" style=""width:50px"">Commission</th>
//            //					</tr>
//            //				</thead>
//            //				<tbody>");

//            //                    Dictionary<int, List<FSPCommission>> commissionsPerBroker = SplitCommissions(commissions);

//            //                    foreach (var commission in commissionsPerBroker)
//            //                    {
//            //                        var statementNo = GenerateCommissionAdvice(commission.Key, commission.Value, DateTime.Now.AddDays(-30).ToShortDateString(), DateTime.Now.ToShortDateString(), controlReportBuilder);
//            //                        foreach (var item in commission.Value)
//            //                        {
//            //                            var memberProduct = mp.GetById(item.MemberProductId);
//            //                            brokerProductCommission = bpcs.GetByBrokerAndProduct(commission.Key, memberProduct.ProductId);

//            //                            var bc = new BrokerCommssion
//            //                            {
//            //                                BrokerId = commission.Key,
//            //                                BrokerProductCommissionId = brokerProductCommission.Id,
//            //                                CreatedOn = DateTime.Now,
//            //                                ModifiedOn = DateTime.Now,
//            //                                ModifiedBy = "System",
//            //                                Status = 1,
//            //                                StatementNo = statementNo,
//            //                                FSPCommissionId = item.Id

//            //                            };
//            //                            bcs.Create(bc);
//            //                        }
//            //                    }
//            //                    controlReportBuilder.AppendLine(@"</tbody>
//            //			</table></body></html>");
//            //                    using (SystemConfigService service = new SystemConfigService())
//            //                    {
//            //                        SystemConfig rules = service.GetById(ConfigSettings.SystemRules.Id);
//            //                        EmailInfo to = new EmailInfo(rules.FinancialEmail, rules.FinancialEmail);
//            //                        SendControlReport(to, controlReportBuilder);
//            //                    }
//            //                    scope.Complete();

//            //                    Info(writer, $"    FINISH:: Processing commissions @ {DateTime.Now}");

//            //                    return true;
//            //                }
//            //        }
//            catch (Exception ex)
//            {
//                Error(writer, ex, "Process");

//                return false;
//            }
//        }
       
//    }
    
//}


////        private static void SendControlReport(EmailInfo to, StringBuilder sb)
////        {
////            using (EmailSenderService service = new EmailSenderService())
////            {
////                Info(logWriter, $"BEGIN:: SendControlReport @{DateTime.Now}");
////                List<EmailInfo> tos = new List<EmailInfo>
////                {
////                    to
////                };

////                EmailMessageModel message = new EmailMessageModel(tos, "Commissions Control Account", ConfigSettings.MailChimp.FromName, ConfigSettings.MailChimp.FromEmail)
////                {
////                    Html = sb.ToString(),
////                    useTemplate = false
////                };

////                service.Send(tos, message);

////                Info(logWriter, $"FINISH:: SendControlReport @{DateTime.Now}");
////            }
////        }

////        private static string GenerateCommissionAdvice(int brokerId, List<FSPCommission> commissions, string periodFrom, string periodTo, StringBuilder controlReportDetails)
////        {
////            StringBuilder sb = new StringBuilder();
////            using (var mcs = new MemberCollectionService())
////            using (var bpcs = new BrokerProductCommissionService())
////            using (var fspcs = new FSPCommissionService())
////            using (var bfsp = new BrokerFSPService())
////            using (var bm = new BrokerMemberService())
////            using (var mps = new MemberProductService())
////            using (var ps = new ProductService())
////            using (var pps = new ProductPriceService())
////            using (var ms = new MemberService())
////            using (var bs = new BrokerService())
////            {
////                var broker = bs.GetById(brokerId);
////                var territory = string.Empty;

////                string begin = $@"
////<table border=""0"" cellpadding=""1"" cellspacing=""1"" style=""height:339px; width:752px"">
////	<tbody>
////		<tr>
////			<td style=""text-align:center""><strong>COMMISSION SUMMARY</strong></td>
////		</tr>
////		<tr>
////			<td>
////			<table border=""1"" cellpadding=""1"" cellspacing=""1"" style=""height:104px; width:745px"">
////				<tbody>
////					<tr>
////						<th scope=""row"" style=""text-align:left; width:123px""><strong>Period From:</strong></th>
////						<td style=""width:365px"">{periodFrom}</td>
////					</tr>
////					<tr>
////						<th scope=""row"" style=""text-align:left; width:123px""><strong>Period To:</strong></th>
////						<td style=""width:365px"">{periodTo}</td>
////					</tr>
////					<tr>
////						<th scope=""row"" style=""text-align:left; width:123px""><strong>Salesperson name:</strong></th>
////						<td style=""width:365px"">{broker.Name}</td>
////					</tr>
////					<tr>
////						<th scope=""row"" style=""text-align:left; width:123px""><strong>Territory:</strong></th>
////						<td style=""width:365px"">{territory}</td>
////					</tr>
////				</tbody>
////			</table>
////			</td>
////		</tr>
////		<tr>
////			<td>
////			<table border=""1"" cellpadding=""1"" cellspacing=""1"" style=""width:750.6px"">
////				<thead>
////					<tr>
////						<th scope=""col"" style=""width:74px"">Date</th>
////						<th scope=""col"" style=""width: 79px;"">Order #</th>
////						<th scope=""col"" style=""width: 292px;"">Client</th>
////						<th scope=""col"" style=""width:65px"">Extended</th>
////						<th scope=""col"" style=""width:112px"">Commission %</th>
////						<th scope=""col"" style=""width:100px"">Amount</th>
////					</tr>
////				</thead>
////				<tbody>

////";
////                sb.Append(begin);
////                Dictionary<string, Tuple<int, decimal>> productPolicies = new Dictionary<string, Tuple<int, decimal>>();
////                decimal grossCommission = 0;
////                decimal totalInvoiced = 0;
////                decimal advance = 0;
////                decimal deductions = 0;
////                foreach (var commission in commissions)
////                {
////                    var mp = mps.GetById(commission.MemberProductId);
////                    var product = ps.GetById(mp.ProductId);
////                    var member = ms.GetById(mp.MemberId);
////                    var productPrice = pps.GetByProduct(mp.ProductId);
////                    string line = $@"					<tr>
////						<td style=""width:74px"">{commission.CommissionDate}</td>
////						<td style=""width:79px"">{product.Code}</td>
////						<td style=""width:292px"">{member.MembershipNo}</td>
////						<td style=""width:65px"">0</td>
////						<td style=""width:112px"">0</td>
////						<td style=""text-align:right; width:100px"">{commission.CommissionAmt.Value.ToString("F")}</td>
////					</tr>";
////                    sb.Append(line);
////                    grossCommission += commission.CommissionAmt.Value;
////                    totalInvoiced += productPrice.Premium;
////                    if (!productPolicies.ContainsKey(product.Code))
////                    {
////                        productPolicies[product.Code] = new Tuple<int, decimal>(0, 0);
////                    }
////                    var cnt = productPolicies[product.Code].Item1;
////                    var productCommissions = productPolicies[product.Code].Item2;


////                    productPolicies[product.Code] = new Tuple<int, decimal>(++cnt, productCommissions + commission.CommissionAmt.Value);


////                }
////                foreach (var productPolicy in productPolicies.Keys)
////                {
////                    controlReportDetails.AppendLine($@"					<tr>
////						<td style=""width:250px"">{broker.Name}</td>
////						<td style=""width:250px"">{productPolicy}</td>
////						<td style=""width:200px; text-align: right"">{productPolicies[productPolicy].Item1}</td>
////						<td style=""width:50px; text-align: right"">{productPolicies[productPolicy].Item2.ToString("F")}</td>
////					</tr>");
////                    //controlReportDetails.AppendLine($"<p>{broker.Name}<span class=\"tab\"></span>{productPolicy}<span class=\"tab\"></span>{productPolicies[productPolicy].Item1}<span class=\"tab\"></span>{productPolicies[productPolicy].Item2}</p>");
////                }

////                controlReportDetails.AppendLine($@"					<tr>
////						<td style=""width:250px""></td>
////						<td style=""width:250px""></td>
////						<td style=""width:200px""><b>Total for broker</b></td>
////                        <td style=""width:50px; text-align: right""><b>{grossCommission.ToString("F")}</b></td>
////					</tr>");
////                //  controlReportDetails.AppendLine($"<p>Total for broker {grossCommission}</p>");

////                decimal payable = grossCommission - advance - deductions;

////                string end = $@"			</tbody>
////			</table>

////			<table align=""right"" border=""1"" cellpadding=""1"" cellspacing=""1"" style=""height:130px; width:293px"">
////				<tbody>
////					<tr>
////						<th scope=""row"" style=""text-align:right; width:180px"">Total Invoiced</th>
////						<td style=""text-align:right; width:100px"">{totalInvoiced.ToString("F")}</td>
////					</tr>
////					<tr>
////						<th scope=""row"" style=""text-align:right; width:180px"">Gross Commissions Earned</th>
////						<td style=""text-align:right; width:100px"">{grossCommission.ToString("F")}</td>
////					</tr>
////					<tr>
////						<th scope=""row"" style=""text-align:right; width:180px"">Less Advance</th>
////						<td style=""text-align:right; width:100px"">{advance.ToString("F")}</td>
////					</tr>
////					<tr>
////						<th scope=""row"" style=""text-align:right; width:180px"">Other Deductions</th>
////						<td style=""text-align:right; width:100px"">{deductions.ToString("F")}</td>
////					</tr>
////					<tr>
////						<th scope=""row"" style=""text-align:right; width:180px"">AmountPayable</th>
////						<td style=""text-align:right; width:100px"">{payable.ToString("F")}</td>
////					</tr>
////				</tbody>
////			</table>

////			<p>&nbsp;</p>
////			</td>
////		</tr>
////		<tr>
////			<td>&nbsp;</td>
////		</tr>
////	</tbody>
////</table>


////<p>&nbsp;</p>";

////                sb.Append(end);
////            }
////            var statementNo = $@"st{brokerId}_{periodFrom.Replace("/", "")}_{periodTo.Replace("/", "")}";
////            var fileName = $"{ConfigSettings.SystemRules.CommissionFilePath}\\st{brokerId}_{periodFrom.Replace("/", "")}_{periodTo.Replace("/", "")}.pdf";
////            StringReader sr = new StringReader(sb.ToString());
////            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
////            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
////            using (MemoryStream memoryStream = new MemoryStream())
////            {
////                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
////                pdfDoc.Open();

////                htmlparser.Parse(sr);
////                pdfDoc.Close();

////                byte[] bytes = memoryStream.ToArray();
////                memoryStream.Close();
////                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
////                {
////                    fs.Write(bytes, 0, bytes.Length);
////                }
////            }

////            return statementNo;
////        }

////        private static Dictionary<int, List<FSPCommission>> SplitCommissions(List<FSPCommission> commissions)
////        {
////            Dictionary<int, List<FSPCommission>> splits = new Dictionary<int, List<FSPCommission>>();

////            foreach (FSPCommission commission in commissions)
////            {
////                if (splits.ContainsKey(commission.BrokerId.Value))
////                {
////                    splits[commission.BrokerId.Value].Add(commission);
////                }
////                else
////                {
////                    splits[commission.BrokerId.Value] = new List<FSPCommission>() { commission };
////                }
////            }

////            return splits;
////        }

////        private static decimal? GetCommission(BrokerProductCommission brokerProductCommission, int cnt)
////        {
////            if (cnt <= brokerProductCommission.SaleQty1)
////            {
////                if (cnt == 0)
////                {
////                    return brokerProductCommission.Commission1P1;
////                }
////                else if (cnt == 1)
////                {
////                    return brokerProductCommission.Commission1P2;
////                }
////                else
////                {
////                    return brokerProductCommission.Commission1P3;
////                }
////            }
////            else if (cnt <= brokerProductCommission.SaleQty2)
////            {
////                cnt = cnt - (int)brokerProductCommission.SaleQty1 - 1;
////                if (cnt == 0)
////                {
////                    return brokerProductCommission.Commission2P1;
////                }
////                else if (cnt == 1)
////                {
////                    return brokerProductCommission.Commission2P2;
////                }
////                else
////                {
////                    return brokerProductCommission.Commission2P3;
////                }
////            }
////            else if (cnt <= brokerProductCommission.SaleQty3)
////            {
////                cnt = cnt - (int)brokerProductCommission.SaleQty2 - 1;
////                if (cnt == 0)
////                {
////                    return brokerProductCommission.Commission3P1;
////                }
////                else if (cnt == 1)
////                {
////                    return brokerProductCommission.Commission3P2;
////                }
////                else
////                {
////                    return brokerProductCommission.Commission3P3;
////                }
////            }
////            else if (cnt <= brokerProductCommission.SaleQty4)
////            {
////                cnt = cnt - (int)brokerProductCommission.SaleQty3 - 1;
////                if (cnt == 0)
////                {
////                    return brokerProductCommission.Commission4P1;
////                }
////                else if (cnt == 1)
////                {
////                    return brokerProductCommission.Commission4P2;
////                }
////                else
////                {
////                    return brokerProductCommission.Commission4P3;
////                }
////            }
////            else if (cnt <= brokerProductCommission.SaleQty5)
////            {
////                cnt = cnt - (int)brokerProductCommission.SaleQty4 - 1;
////                if (cnt == 0)
////                {

////                    return brokerProductCommission.Commission5P1;
////                }
////                else if (cnt == 1)
////                {
////                    return brokerProductCommission.Commission5P2;
////                }
////                else
////                {
////                    return brokerProductCommission.Commission5P3;
////                }
////            }

////            return 0;
////        }





////    }
////}