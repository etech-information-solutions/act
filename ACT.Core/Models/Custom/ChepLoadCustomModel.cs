
namespace ACT.Core.Models.Custom
{
    using System;
    using System.Collections.Generic;
    using ACT.Data.Models;
    public partial class ChepLoadCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModfiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ChepStatus { get; set; }
        public string TransactionType { get; set; }
        public string DocketNumber { get; set; }
        public string OriginalDocketNumber { get; set; }
        public string UMI { get; set; }
        public string LocationId { get; set; }
        public string Location { get; set; }
        public string OtherPartyId { get; set; }
        public string OtherParty { get; set; }
        public string OtherPartyCountry { get; set; }
        public string EquipmentCode { get; set; }
        public string Equipment { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Ref { get; set; }
        public string OtherRef { get; set; }
        public string BatchRef { get; set; }
        public Nullable<System.DateTime> ShipmentDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public string InvoiceNumber { get; set; }
        public string Reason { get; set; }
        public string DataSource { get; set; }
        public Nullable<int> BalanceStatus { get; set; }
        public int Status { get; set; }
        public int PostingType { get; set; }

        public virtual Client Client { get; set; }
        public int? DocumentCount { get; set; }
        public List<Document> Documents { get; set; }

        public string ClientName { get; set; }
    }
}
