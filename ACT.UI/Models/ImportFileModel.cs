using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ACT.Core.Models;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class ImportFileModel
    {
        #region Properties

        [Display( Name = "Client Id" )]
        Client client { get; set; }

        [Display(Name = "Site Id")]
        Site site { get; set; }

        [Display( Name = "File" )]
        public HttpPostedFileBase File { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}