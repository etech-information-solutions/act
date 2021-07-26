using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
namespace ACT.UI.Models
{
    public class TransporterViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Client" )]
        public int? ClientId { get; set; }

        [Required]
        [Display( Name = "Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Name { get; set; }

        [Required]
        [Display( Name = "Trading Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string TradingName { get; set; }

        [Display( Name = "Contact Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactName { get; set; }

        [Display( Name = "Contact Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ContactNumber { get; set; }

        [Required]
        [Display( Name = "Email" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string Email { get; set; }

        [Required]
        [Display( Name = "Registration Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string RegistrationNumber { get; set; }



        [Display( Name = "Supplier Code" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string SupplierCode { get; set; }

        [Display( Name = "Transporter Code" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ClientTransporterCode { get; set; }

        [Display( Name = "Chep Transporter Code" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1 )]
        public string ChepClientTransporterCode { get; set; }



        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        [Display( Name = "Contacts" )]
        public List<Contact> Contacts { get; set; }

        [Display( Name = "Vehicles" )]
        public List<Vehicle> Vehicles { get; set; }

        [Display( Name = "Import Transporters" )]
        public HttpPostedFileBase File { get; set; }

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

        #endregion
    }
}