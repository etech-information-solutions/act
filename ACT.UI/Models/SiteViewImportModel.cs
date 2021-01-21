using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
namespace ACT.UI.Models
{
    public class SiteViewImportModel
    {
        #region Properties

        public int Id { get; set; }
        public int? ClientId { get; set; }
        
        FileViewModel importFile { get; set; }

        #endregion


        #region Model Options


        #endregion
    }
}