using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACT.UI.Models
{
    public class DynamicListModel
    {
        [Required]
        [Display( Name = "Branch" )]
        public string Branch { get; set; }

        [Required]
        [Display( Name = "Directorate/Project" )]
        public string DirectorateProject { get; set; }
         
        [Required]
        [Display( Name = "Department/Sub-Project" )]
        public string DepartmentSubProject { get; set; }

        public Dictionary<string, string> BranchOptions { get; set; }

        public Dictionary<string, string> DirectorateProjectOptions { get; set; }

        public Dictionary<string, string> DepartmentSubProjectOptions { get; set; }
    }
}