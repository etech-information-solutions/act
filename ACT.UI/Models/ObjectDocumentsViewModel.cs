using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Collections.Generic;
using ACT.Data.Models;

namespace ACT.UI.Models
{
    public class ObjectDocumentsViewModel
    {
        #region Properties
        
        public List<Document> objDocuments { get; set; }

        public string objType { get; set; }
        public int objId { get; set; }

        #endregion

        #region Model Options



        #endregion
    }
}