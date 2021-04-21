using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

using ACT.Core.Enums;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class ClientLoadViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Client" )]
        public int ClientId { get; set; }

        [Required]
        [Display( Name = "Vehicle" )]
        public int? VehicleId { get; set; }

        [Display( Name = "Client Site" )]
        public int? ClientSiteId { get; set; }

        [Display( Name = "Transporter" )]
        public int? TransporterId { get; set; }

        [Display( Name = "Outstanding Reason" )]
        public int? OutstandingReasonId { get; set; }

        [Display( Name = "POD Comment" )]
        public int? PODCommentId { get; set; }

        [Display( Name = "Load Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string LoadNumber { get; set; }

        [Required]
        [Display( Name = "Load Date" )]
        public DateTime? LoadDate { get; set; }

        [Required]
        [Display( Name = "Effective Date" )]
        public DateTime? EffectiveDate { get; set; }

        [Required]
        [Display( Name = "Notify Date" )]
        public DateTime? NotifyDate { get; set; }

        [Required]
        [Display( Name = "Account Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string AccountNumber { get; set; }

        [Required]
        [Display( Name = "Client Description" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ClientDescription { get; set; }

        [Required]
        [Display( Name = "Delivery Note" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DeliveryNote { get; set; }

        [Required]
        [Display( Name = "Reference Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ReferenceNumber { get; set; }

        [Required]
        [Display( Name = "Receiver Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ReceiverNumber { get; set; }

        [Display( Name = "Equipment" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Equipment { get; set; }

        [Required]
        [Display( Name = "Original Quantity" )]
        public decimal? OriginalQuantity { get; set; }

        [Display( Name = "New Quantity" )]
        public decimal? NewQuantity { get; set; }

        [Display( Name = "Reconcile Invoice" )]
        public YesNo ReconcileInvoice { get; set; }

        [Display( Name = "Reconcile Date" )]
        public DateTime? ReconcileDate { get; set; }

        [Display( Name = "POD" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PODNumber { get; set; }

        [Display( Name = "PCN" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PCNNumber { get; set; }

        [Display( Name = "PRN" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PRNNumber { get; set; }

        [Display( Name = "Reconciliation Status" )]
        public ReconciliationStatus Status { get; set; }

        [Display( Name = "THAN" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string THAN { get; set; }

        [Display( Name = "Return Quantity" )]
        public decimal? ReturnQty { get; set; }

        [Display( Name = "Outstanding Quantity" )]
        public decimal? OutstandingQty { get; set; }

        [Display( Name = "Debrief Quantity" )]
        public decimal? DebriefQty { get; set; }

        [Display( Name = "Admin Movement" )]
        public decimal? AdminMovement { get; set; }

        [Display( Name = "Chep Invoice #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ChepInvoiceNo { get; set; }

        [Display( Name = "Chep Compensation #" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ChepCompensationNo { get; set; }

        [Display( Name = "Cancelled Reason" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CancelledReason { get; set; }

        [Display( Name = "PCN Comments" )]
        public string PCNComments { get; set; }

        [Display( Name = "PRN Comments" )]
        public string PRNComments { get; set; }



        [Display( Name = "Import Loads" )]
        public HttpPostedFileBase File { get; set; }

        public List<FileViewModel> Files { get; set; }

        public bool EditMode { get; set; }

        #endregion


        #region Model Options

        public Dictionary<int, string> ClientOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( ClientService cservice = new ClientService() )
                {
                    if ( cservice.SelectedClient != null )
                    {
                        ClientId = cservice.SelectedClient.Id;
                    }

                    return cservice.List( true );
                }
            }
        }

        public Dictionary<int, string> VehicleOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( VehicleService service = new VehicleService() )
                {
                    return service.List( true );
                }
            }
        }

        public Dictionary<int, string> ClientSiteOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( ClientSiteService service = new ClientSiteService() )
                {
                    return service.List( true );
                }
            }
        }

        public Dictionary<int, string> TransporterOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( TransporterService service = new TransporterService() )
                {
                    return service.List( true );
                }
            }
        }

        public Dictionary<int, string> OutstandingReasonOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( OutstandingReasonService service = new OutstandingReasonService() )
                {
                    return service.List( true );
                }
            }
        }

        public Dictionary<int, string> PODCommentOptions
        {
            get
            {
                if ( !EditMode ) return null;

                using ( PODCommentService service = new PODCommentService() )
                {
                    return service.List( true );
                }
            }
        }

        #endregion
    }
}