
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;

    public partial class ClientLoadCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int VehicleId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string LoadNumber { get; set; }
        public Nullable<System.DateTime> LoadDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> NotifyeDate { get; set; }
        public string AccountNumber { get; set; }
        public string ClientDescription { get; set; }
        public string DeliveryNote { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReceiverNumber { get; set; }
        public string Equipment { get; set; }
        public Nullable<decimal> OriginalQuantity { get; set; }
        public Nullable<decimal> NewQuantity { get; set; }
        public Nullable<int> ReconcileInvoice { get; set; }
        public Nullable<System.DateTime> ReconcileDate { get; set; }
        public string PODNumber { get; set; }
        public string PCNNumber { get; set; }
        public string PRNNumber { get; set; }
        public int Status { get; set; }
        public int? DocumentCount { get; set; }
        public List<Document> Documents { get; set; }

        public int? PostingType { get; set; }

        public string DocketNumber { get; set; }
    }
}
