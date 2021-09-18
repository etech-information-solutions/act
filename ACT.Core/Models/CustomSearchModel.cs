using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
using System.Reflection;
using ACT.Core.Models.Simple;
using System.Data;

namespace ACT.Core.Models
{
    public class CustomSearchModel
    {
        public CustomSearchModel()
        {
            SetDefaults();

            using ( ClientService cservice = new ClientService() )
            {
                if ( cservice.SelectedClient != null && ClientId == 0 )
                {
                    ClientId = cservice.SelectedClient.Id;
                }
            }
        }



        #region Properties

        /// <summary>
        /// An entity's Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifies a Chepload
        /// </summary>
        public int ChepLoadId { get; set; }

        /// <summary>
        /// Can be used as a selected user 
        /// </summary>
        [Display( Name = "User" )]
        public int UserId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Role 
        /// </summary>
        [Display( Name = "Role" )]
        public int RoleId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Client 
        /// </summary>
        [Display( Name = "Client" )]
        public int ClientId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Client Group 
        /// </summary>
        [Display( Name = "Client Group" )]
        public int GroupId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Transporter
        /// </summary>
        [Display( Name = "Transporter" )]
        public int TransporterId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected POD Comment
        /// </summary>
        [Display( Name = "POD Comment" )]
        public int PODCommentId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as selected Clients
        /// </summary>
        [Display( Name = "Client Ids" )]
        public List<int> ClientIds
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Site 
        /// </summary>
        [Display( Name = "Supplier From" )]
        public int SiteId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected To Site 
        /// </summary>
        [Display( Name = "Customer To" )]
        public int ToSiteId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Product 
        /// </summary>
        [Display( Name = "Product" )]
        public int ProductId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected PSP Product 
        /// </summary>
        [Display( Name = "PSP Product" )]
        public int PSPProductId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Region 
        /// </summary>
        [Display( Name = "Region" )]
        public int RegionId
        {
            get; set;
        }

        /// <summary>
        /// Can be used for any entity requiring bank filter
        /// </summary>
        [Display( Name = "Bank" )]
        public int BankId
        {
            get; set;
        }

        /// <summary>
        /// Can be used for any entity requiring PSP filter
        /// </summary>
        [Display( Name = "PSP" )]
        public int PSPId { get; set; }

        /// <summary>
        /// Can be used for any entity requiring Vehicle filter
        /// </summary>
        [Display( Name = "Vehicle" )]
        public int VehicleId { get; set; }

        /// <summary>
        /// Can be used for any entity requiring Client Site filter
        /// </summary>
        [Display( Name = "Client Site" )]
        public int ClientSiteId { get; set; }

        /// <summary>
        /// Can be used for any entity requiring Outstanding Reason filter
        /// </summary>
        [Display( Name = "Outstanding Reason" )]
        public int OutstandingReasonId { get; set; }

        /// <summary>
        /// Can be used as a selected Dispute Reason 
        /// </summary>
        [Display( Name = "Dispute Reason" )]
        public int DisputeReasonId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Ticket
        /// </summary>
        [Display( Name = "Ticket" )]
        public int TicketId { get; set; }

        /// <summary>
        /// Can be used for a Start date range
        /// </summary>
        [Display( Name = "From Date" )]
        public DateTime? FromDate
        {
            get; set;
        }

        /// <summary>
        /// Can be used for a End date range
        /// </summary>
        [Display( Name = "To" )]
        public DateTime? ToDate
        {
            get; set;
        }

        /// <summary>
        /// Can be used for any Document Type
        /// </summary>
        [Display( Name = "Document Type" )]
        public DocumentType DocumentType
        {
            get; set;
        }

        /// <summary>
        /// Can be used to indicate Activity Type
        /// </summary>
        [Display( Name = "Activity" )]
        public ActivityTypes ActivityType
        {
            get; set;
        }

        /// <summary>
        /// Can be used to indicate Role Type
        /// </summary>
        [Display( Name = "Role Type" )]
        public RoleType RoleType
        {
            get; set;
        }

        /// <summary>
        /// Can be used to indicate Province
        /// </summary>
        [Display( Name = "Province" )]
        public int ProvinceId
        {
            get; set;
        }

        /// <summary>
        /// A custom search query
        /// </summary>
        [Display( Name = "Search Text" )]
        public string Query
        {
            get; set;
        }

        /// <summary>
        /// A custom Generic Name Search
        /// </summary>
        [Display( Name = "Generic Name Search" )]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// A custom Generic Description Search
        /// </summary>
        [Display( Name = "Generic Description Search" )]
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// A custom Generic Description Search
        /// </summary>
        [Display( Name = "Generic Reference Number Search" )]
        public string ReferenceNumber
        {
            get; set;
        }

        /// <summary>
        /// A custom Generic Description Search
        /// </summary>
        [Display( Name = "Generic Other Reference Number Search" )]
        public string ReferenceNumberOther
        {
            get; set;
        }

        /// <summary>
        /// A custom Generic Description Search
        /// </summary>
        [Display( Name = "Generic Contact Name Search" )]
        public string ContactName
        {
            get; set;
        }

        /// <summary>
        /// A custom Generic Description Search
        /// </summary>
        [Display( Name = "Generic Contact Number  Search" )]
        public string ContactNumber
        {
            get; set;
        }

        /// <summary>
        /// A custom Generic Description Search
        /// </summary>
        [Display( Name = "SitePlanningPoint Search" )]
        public string SitePlanningPoint
        {
            get; set;
        }

        /// <summary>
        /// Status
        /// </summary>
        [Display( Name = "Status" )]
        public Status Status
        {
            get; set;
        }

        [Display( Name = "Invoice Status" )]
        public InvoiceStatus InvoiceStatus
        {
            get;
            set;
        }

        [Display( Name = "Status" )]
        public PSPClientStatus PSPClientStatus
        {
            get;
            set;
        }

        [Display( Name = "Status" )]
        public Status ClientStatus
        {
            get;
            set;
        }

        [Display( Name = "Reconciliation Status" )]
        public ReconciliationStatus ReconciliationStatus
        {
            get;
            set;
        }

        [Display( Name = "Balance Status" )]
        public BalanceStatus BalanceStatus
        {
            get;
            set;
        }

        [Display( Name = "Dispute Status" )]
        public DisputeStatus DisputeStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Can be used for a Filter date range
        /// </summary>
        [Display( Name = "Filter Date" )]
        public DateTime? FilterDate
        {
            get; set;
        }


        /// <summary>
        /// Can be used for a Filter date range
        /// </summary>
        [Display( Name = "Filter Additional Date" )]
        public DateTime? Filter2Date
        {
            get; set;
        }

        public decimal? Amount { get; set; }

        /// <summary>
        /// Is Outstanding Pallets
        /// </summary>
        public bool IsOP { get; set; }

        public bool IsExchange { get; set; }

        public bool IsPSPPickUp { get; set; }

        public bool IsPODOutstanding { get; set; }

        public bool IsTransporterLiable { get; set; }

        [Display( Name = "Object Id" )]
        public int ObjectId
        {
            get; set;
        }

        [Display( Name = "Target Table" )]
        public string TableName
        {
            get; set;
        }

        [Display( Name = "Controller Name" )]
        public string ControllerName
        {
            get; set;
        }

        /// <summary>
        /// !SYSTEM: This is automatically set depending on page you're currently viewing
        /// </summary>
        public string ReturnView
        {
            get; set;
        }

        /// <summary>
        /// !SYSTEM: This is automatically set depending on page you're currently viewing
        /// </summary>
        public string Controller
        {
            get; set;
        }

        /// <summary>
        /// Some unique identifier
        /// </summary>
        public string ManuallyMatchedUID { get; set; }

        public bool HasAuthorisationCode { get; set; }
        public string DocketNumber { get; set; }
        public string OriginalDocketNumber { get; set; }
        public bool IncludeUserBroadCasts { get; set; }

        public string KPIReportFilterType { get; set; }

        #endregion



        #region Model Options

        public Dictionary<int, string> UserOptions
        {
            get
            {
                using ( UserService uservice = new UserService() )
                {
                    RoleType role = uservice.CurrentUser.RoleType;
                    role = role == RoleType.PSP ? role : RoleType.All;

                    return uservice.List( true, role );
                }
            }
        }

        public Dictionary<int, string> User1Options { get; set; }

        public Dictionary<int, string> ProvinceOptions
        {
            get
            {
                using ( ProvinceService pservice = new ProvinceService() )
                {
                    return pservice.List( true );
                }
            }
        }

        public List<Bank> BankOptions
        {
            get
            {
                using ( BankService service = new BankService() )
                {
                    return service.List();
                }
            }
            set
            {
            }
        }

        public Dictionary<int, string> SiteOptions { get; set; }

        public Dictionary<int, string> ClientOptions { get; set; }

        public Dictionary<int, string> ProductOptions { get; set; }

        public Dictionary<int, string> PSPOptions { get; set; }

        public Dictionary<int, string> PSPProductOptions { get; set; }

        public Dictionary<int, string> RegionOptions { get; set; }

        public Dictionary<int, string> TransporterOptions { get; set; }

        public Dictionary<int, string> VehicleOptions { get; set; }

        public Dictionary<int, string> ClientSiteOptions { get; set; }

        public Dictionary<int, string> PODCommentOptions { get; set; }

        public Dictionary<int, string> OutstandingReasonOptions { get; set; }

        public List<DisputeReason> DisputeReasonOptions { get; set; }

        public List<string> TableNameOptions
        {
            get
            {
                return Assembly.Load( "ACT.Data" )
                               .GetTypes()
                               .Where( a => string.Equals( a.Namespace, "ACT.Data.Models", StringComparison.Ordinal ) )
                               .Select( s => s.Name )
                               .ToList();
            }
        }

        public List<string> ControllerNameOptions
        {
            get
            {
                return MvcHelper.GetControllerNames();
            }
        }

        #endregion



        /// <summary>
        /// There's a common use for Branch, DirectorateProject and DepartmentSubProject  etc
        /// This function will help generically retrieve a unique list of the above 3 from a specified table
        /// CAUTION!! Only use this if the table you're trying to query has the above 3 "string" columns and are spelled exactly as above!
        /// If the table doesn't have, then using this function will break your request, if not spelled the same, kindly make it spelled as
        /// above to enjoy me!
        /// LOOK at /Views/PaymentRequisition/_List and then /Views/Shared/_PRCustomSearch for usage
        /// </summary>
        /// <param name="listType"></param>
        public CustomSearchModel( string listType )
        {
            SetDefaults();

            switch ( listType )
            {
                case "Users":



                    break;


                case "PSPs":
                case "LinkProducts":

                    using ( ClientService cservice = new ClientService() )
                    using ( ProductService pservice = new ProductService() )
                    {
                        ClientOptions = cservice.List( true );
                        ProductOptions = pservice.List( true );
                    }

                    break;


                case "Regions":

                    using ( PSPService pservice = new PSPService() )
                    {
                        PSPOptions = pservice.List( true );
                    }

                    break;


                case "Clients":

                    using ( PSPService pservice = new PSPService() )
                    using ( ClientService cservice = new ClientService() )
                    {
                        PSPOptions = pservice.List( true );
                        ClientOptions = cservice.List( true );
                    }

                    break;


                case "SetClient":
                case "ClientKPI":
                case "ReconcileLoads":
                case "MovementReport":
                case "ReconcileInvoice":
                case "ManageTransporters":

                    using ( ClientService cservice = new ClientService() )
                    {
                        ClientOptions = cservice.List( true );
                    }

                    break;


                case "Disputes":
                case "Exceptions":
                case "DeliveryNotes":

                    using ( SiteService sservice = new SiteService() )
                    using ( ClientService cservice = new ClientService() )
                    using ( DisputeReasonService drervice = new DisputeReasonService() )
                    {
                        SiteOptions = sservice.List( true );
                        ClientOptions = cservice.List( true );
                        DisputeReasonOptions = drervice.List();
                    }

                    break;


                case "ChepAudit":
                case "ClientAudit":

                    using ( SiteService sservice = new SiteService() )
                    {
                        SiteOptions = sservice.List( true );
                    }

                    break;


                case "DashBoard":

                    using ( SiteService sservice = new SiteService() )
                    using ( RegionService rservice = new RegionService() )
                    using ( ClientService cservice = new ClientService() )
                    using ( ProductService pservice = new ProductService() )
                    {
                        SiteOptions = sservice.List( true );
                        RegionOptions = rservice.List( true );
                        ClientOptions = cservice.List( true );
                        ProductOptions = pservice.List( true );
                    }

                    break;


                case "AuthorisationCodes":

                    using ( SiteService sservice = new SiteService() )
                    using ( ClientService cservice = new ClientService() )
                    using ( TransporterService tservice = new TransporterService() )
                    {
                        SiteOptions = sservice.List( true );
                        ClientOptions = cservice.List( true );
                        TransporterOptions = tservice.List( true );
                    }

                    break;


                case "Billing":

                    using ( PSPService pservice = new PSPService() )
                    using ( PSPProductService p1service = new PSPProductService() )
                    {
                        PSPOptions = pservice.List( true );
                        PSPProductOptions = p1service.List( true );
                    }

                    break;


                case "ManageSites":

                    using ( SiteService sservice = new SiteService() )
                    using ( ClientService cservice = new ClientService() )
                    using ( RegionService rservice = new RegionService() )
                    {
                        SiteOptions = sservice.List( true );
                        ClientOptions = cservice.List( true );
                        RegionOptions = rservice.List( true );
                    }

                    break;


                case "ClientData":

                    using ( ClientService cservice = new ClientService() )
                    using ( VehicleService vservice = new VehicleService() )
                    //using ( ClientSiteService csservice = new ClientSiteService() )
                    using ( TransporterService tservice = new TransporterService() )
                    using ( OutstandingReasonService urservice = new OutstandingReasonService() )
                    {
                        ClientOptions = cservice.List( true );
                        VehicleOptions = vservice.List( true );
                        //ClientSiteOptions = csservice.List( true );
                        TransporterOptions = tservice.List( true );
                        OutstandingReasonOptions = urservice.List( true );
                    }

                    break;

                case "PODOutstanding":
                case "PoolingAgentData":
                case "OutstandingPallets":
                case "TopOustandingCustomers":
                case "TransporterLiableReport":

                    using ( ClientService cservice = new ClientService() )
                    using ( OutstandingReasonService urservice = new OutstandingReasonService() )
                    {
                        ClientOptions = cservice.List( true );
                        OutstandingReasonOptions = urservice.List( true );
                    }

                    break;

                case "SiteAudits":

                    using ( SiteService sservice = new SiteService() )
                    using ( ClientService cservice = new ClientService() )
                    {
                        SiteOptions = sservice.List( true );
                        ClientOptions = cservice.List( true );
                    }

                    break;

                case "PODPerUser":
                case "PODUploadLog":

                    using ( UserService uservice = new UserService() )
                    {
                        User1Options = uservice.List( true, RoleType.All );
                    }

                    break;

                case "AuthorizationCodeAudit":

                    using ( UserService uservice = new UserService() )
                    using ( TransporterService tservice = new TransporterService() )
                    {
                        TransporterOptions = tservice.List( true );
                        User1Options = uservice.List( true, RoleType.All );
                    }

                    break;
                case "AuditLogPerUser":

                    using ( UserService uservice = new UserService() )
                    using ( ClientService cservice = new ClientService() )
                    using ( VehicleService vservice = new VehicleService() )
                    using ( PODCommentService pcservice = new PODCommentService() )
                    using ( TransporterService tservice = new TransporterService() )
                    {
                        ClientOptions = cservice.List( true );
                        VehicleOptions = vservice.List( true );
                        PODCommentOptions = pcservice.List( true );
                        TransporterOptions = tservice.List( true );
                        User1Options = uservice.List( true, RoleType.All );
                    }

                    break;
            }

            using ( ClientService cservice = new ClientService() )
            {
                if ( cservice.SelectedClient != null && ClientId == 0 )
                {
                    ClientId = cservice.SelectedClient.Id;
                }
            }
        }



        private void SetDefaults()
        {
            this.Status = Status.All;
            this.RoleType = RoleType.All;
            this.ClientStatus = Status.Active;
            this.DocumentType = DocumentType.None;
            this.ActivityType = ActivityTypes.All;
            this.InvoiceStatus = InvoiceStatus.All;
            this.DisputeStatus = DisputeStatus.All;
            this.BalanceStatus = BalanceStatus.None;
            this.PSPClientStatus = PSPClientStatus.All;
            this.ReconciliationStatus = ReconciliationStatus.All;
        }
    }
}
