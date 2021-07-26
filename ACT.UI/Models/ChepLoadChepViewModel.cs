using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACT.Core.Services;
using ACT.Data.Models;

namespace ACT.Core.Models.Custom
{
    public class ChepLoadChepViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Chep Load" )]
        public int ChepLoadId { get; set; }

        [Display( Name = "PCN" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PCN { get; set; }

        [Display( Name = "PRN" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string PRN { get; set; }

        [Display( Name = "Docket #" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string DocketNumber { get; set; }

        [Display( Name = "Reference" )]
        [StringLength( 150, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Reference { get; set; }

        [Display( Name = "Quantity" )]
        public int Quantity { get; set; }

        [Display( Name = "Effective Date" )]
        public DateTime? EffectiveDate { get; set; }

        public List<ChepLoadChepViewModel> ChepLoadAllocations { get; set; }

        #endregion



        #region Options

        public List<ChepLoadChep> ChepLoadAllocationOptions
        {
            get
            {
                if ( ChepLoadId == 0 ) return new List<ChepLoadChep>();

                using ( ChepLoadChepService clcservice = new ChepLoadChepService() )
                {
                    return clcservice.ListByChepLoadId( ChepLoadId );
                }
            }
        }

        #endregion
    }
}
