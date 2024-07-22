using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class ClientCustomerViewModel
    {
        #region Properties
        public int Id { get; set; }

        [Display( Name = "Customer Name" )]
        [Required( ErrorMessage = "Customer Name is required" )]
        public int ClientId { get; set; }

        [Display( Name = "Customer Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CustomerNumber { get; set; }

        [Display( Name = "Customer Name" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CustomerName { get; set; }

        [Display( Name = "Contact Number" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string CustomerContact { get; set; }

        [Display( Name = "Key Account Manager" )]
        public string KeyAccountManager { get; set; }

        [Display( Name = "Contacts" )]
        public List<Contact> Contacts { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public bool EditMode { get; set; }

        public AddressViewModel Address { get; set; }

        public int? CustomerUserId { get; set; }
        #endregion

        #region Model Options
        public Dictionary<int, string> ClientOptions
        {
            get
            {
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