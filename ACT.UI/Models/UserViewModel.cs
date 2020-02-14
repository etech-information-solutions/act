using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class UserViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Required]
        [Display( Name = "Role" )]
        public int RoleId { get; set; }

        [Display( Name = "PSP" )]
        public int PSPId { get; set; }

        [Display( Name = "Client" )]
        public int ClientId { get; set; }

        public Role Role { get; set; }

        [Required]
        [Display( Name = "Name" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Name { get; set; }

        [Required]
        [Display( Name = "Surname" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Surname { get; set; }

        [Required]
        [Display( Name = "Email Address" )]
        [DataType( DataType.EmailAddress )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Email { get; set; }

        [Required]
        [Display( Name = "Cellphone Number" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Cell { get; set; }

        [Display( Name = "Password" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Password { get; set; }

        [Display( Name = "Confirm Password" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string ConfirmPassword { get; set; }



        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public RoleType RoleType { get; set; }

        public bool EditMode { get; set; }

        #endregion

        #region Model Options 

        public List<Role> RoleOptions
        {
            get
            {
                using ( RoleService service = new RoleService() )
                {
                    return service.List();
                }
            }
        }

        public Dictionary<int, string> PSPOptions
        {
            get
            {
                using ( PSPService pservice = new PSPService() )
                {
                    return pservice.List( true );
                }
            }
        }
        public Dictionary<int, string> ClientOptions
        {
            get
            {
                using ( ClientService cservice = new ClientService() )
                {
                    return cservice.List( true );
                }
            }
        }

        #endregion
    }
}