using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ACT.Core.Models;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class InvoiceViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Restaurant" )]
        public int RestaurantId { get; set; }

        [Display( Name = "Campaign Purchase" )]
        public int CampaignPurchaseId { get; set; }

        public string ReturnView { get; set; }

        #endregion

        #region Model Options


        #endregion
    }
}