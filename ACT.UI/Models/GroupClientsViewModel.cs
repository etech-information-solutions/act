using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
namespace ACT.UI.Models
{
    public class GroupClientsViewModel
    {
        #region Properties

        public int Id { get; set; }

        List<Group> clientIncluded { get; set; }
        List<Client> ClientListIncluded { get; set; }
        List<Client> ClientListExcluded { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
      
        [Display(Name = "Status")]
        public int Status { get; set; }

        #endregion


        #region Model Options


        #endregion
    }
}