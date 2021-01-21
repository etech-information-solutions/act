
namespace ACT.Core.Models.Custom
{
    using System;
    
    public partial class UserCustomModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int? Type { get; set; }
        public string IdNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public int Status { get; set; }
        public string JobTitle { get; set; }
        public int Province { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Password { get; set; }
        public DateTime? PasswordDate { get; set; }

        public int? RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
