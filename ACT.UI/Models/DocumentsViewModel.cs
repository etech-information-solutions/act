using System;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using System.Web;
using System.Collections.Generic;
using ACT.Core.Services;

namespace ACT.UI.Models
{
    public class DocumentsViewModel
    {
        #region Properties

        public int Id { get; set; }

        [Display( Name = "Select Category" )]
        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Category { get; set; }

        [StringLength( 50, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Category1 { get; set; }

        [Required]
        [Display( Name = "Title" )]
        [StringLength( 250, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Title { get; set; }
        
        [Display( Name = "External URL" )]
        [StringLength( 500, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Url { get; set; }
        
        [Required]
        [Display( Name = "Description" )]
        [StringLength( 1000, ErrorMessage = "Only {1} characters are allowed for this field.", MinimumLength = 0 )]
        public string Description { get; set; }

        [Required]
        [Display( Name = "Status" )]
        public Status Status { get; set; }

        public string Type { get; set; }

        public long Size { get; set; }

        public bool EditMode { get; set; }

        [Display( Name = "File" )]
        public HttpPostedFileBase File { get; set; }

        #endregion

        #region Model Options

        public List<string> CategoryOptions
        {
            get
            {
                if ( !EditMode ) return new List<string>() { };

                using ( DocumentService service = new DocumentService() )
                {
                    return service.ListByColumn( "Category" );
                }
            }
        }

        #endregion
    }
}