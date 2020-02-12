
namespace ACT.Core.Models.Custom
{
    public partial class RoleCustomModel
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public bool DashBoard { get; set; }
        public bool Administration { get; set; }
        public bool Finance { get; set; }
        public bool Client { get; set; }
        public bool Customer { get; set; }
        public bool Product { get; set; }
        public bool Pallet { get; set; }
    }
}
