using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
namespace ACT.UI.Models
{
    public class SiteViewModel
    {
        #region Properties

        public int Id { get; set; }
        public Nullable<int> SiteId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        [Required]
        [Display(Name = "Site Name")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 3)]
        public string Description { get; set; }
        [Required]
        [Display(Name = "X Coordinate Longitude")]
        [StringLength(10, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string XCord { get; set; }
        [Required]
        [Display(Name = "Y Coordinate Latitude")]
        [StringLength(10, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string YCord { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        [Required]
        [Display(Name = "Contact Number")]
        [StringLength(13, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string ContactNo { get; set; }
        [Required]
        [Display(Name = "Contact Name")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string ContactName { get; set; }
        public string PlanningPoint { get; set; }
        [Required]
        public int SiteType { get; set; }
        public string AccountCode { get; set; }
        [Required]
        public string Depot { get; set; }
        public string SiteCodeChep { get; set; }
        [Required]
        public int Status { get; set; }
        [Display(Name = "Finance Contact Number")]
        [StringLength(13, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string FinanceContactNo { get; set; }
        [Display(Name = "Finance Contact Name")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string FinanceContact { get; set; }
        [Display(Name = "Receiving Contact Number")]
        [StringLength(13, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string ReceivingContactNo { get; set; }
        [Display(Name = "Receiving Contact Name")]
        [StringLength(50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 1)]
        public string ReceivingContact { get; set; }

        public bool EditMode { get; set; }
        public bool ContextualMode { get; set; }
        public string SourceView { get; set; }

        public AddressViewModel FullAddress { get; set; }


        #endregion


        #region Model Options

        //public List<Region> RegionOptions
        //{
        //    get
        //    {
        //        using (RegionService service = new RegionService())
        //        {
        //            return service.List();
        //        }
        //    }
        //    set
        //    { }
        //}
        //public List<SiteType> TypeOptions
        //{
        //    get
        //    {
        //        using (SiteTypeService service = new SiteTypeService())
        //        {
        //            return service.List();
        //        }
        //    }
        //}


        #endregion
    }
}