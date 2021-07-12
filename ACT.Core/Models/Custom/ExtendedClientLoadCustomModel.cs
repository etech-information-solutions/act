using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Models.Custom
{
    public class ExtendedClientLoadCustomModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Nullable<int> ClientSiteId { get; set; }
        public int ClientLoadId { get; set; }
        public Nullable<int> OutstandingReasonId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
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
        public bool IsExchange { get; set; }
        public bool IsPSPPickup { get; set; }
        public bool TransporterLiable { get; set; }
        public bool ManuallyMatchedLoad { get; set; }
        public string ManuallyMatchedUID { get; set; }
        public string CorrectedRef { get; set; }
        public string CorrectedOtherRef { get; set; }
        public bool IsExtra { get; set; }
        public string UID { get; set; }
        public int DocumentType { get; set; }
        public string PalletReturnSlipNo { get; set; }
        public string ChepCustomerThanDocNo { get; set; }
        public string WarehouseTransferDocNo { get; set; }
        public Nullable<System.DateTime> PalletReturnDate { get; set; }
    }
}
