
using System.Collections.Generic;

using ACT.Data.Models;

namespace ACT.Core.Models.Custom
{
    public partial class TransporterCustomModel
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string TradingName { get; set; }
        public string RegistrationNumber { get; set; }
        public int Status { get; set; }

        public int ContactCount { get; set; }

        public int VehicleCount { get; set; }

        public List<Contact> Contacts { get; set; }

        public List<Vehicle> Vehicles { get; set; }
    }
}
