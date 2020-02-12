using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;

namespace ACT.UI.Models
{
    public class RoleViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Name { get; set; }
        
        [Display( Name = "Type" )]
        public RoleType Type { get; set; }

        [Display( Name = "Dashboard" )]
        public bool DashBoard { get; set; }

        [Display( Name = "Clients" )]
        public bool Client { get; set; }

        [Display( Name = "Customers" )]
        public bool Customer { get; set; }

        [Display( Name = "Product" )]
        public bool Product { get; set; }

        [Display( Name = "Pallet" )]
        public bool Pallet { get; set; }

        [Display( Name = "Finance" )]
        public bool Finance { get; set; }

        [Display( Name = "Administration" )]
        public bool Administration { get; set; }

        public bool EditMode { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}