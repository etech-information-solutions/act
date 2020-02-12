using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ACT.UI.Models
{
    public class FileViewModel
    {
        #region Properties
        
        public int Id { get; set; }

        public decimal Size { get; set; }

        public string Extension { get; set; }

        [Display( Name = "Name" )]
        public string Name { get; set; }
        
        [Display( Name = "Description" )]
        public string Description { get; set; }
        
        public string Location { get; set; }

        public string ExternalUrl { get; set; }

        [Display( Name = "File" )]
        public HttpPostedFileBase File { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}