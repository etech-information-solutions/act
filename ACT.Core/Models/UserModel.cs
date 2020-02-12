using System;
using System.Collections.Generic;
using ACT.Core.Enums;
using ACT.Core.Interfaces;
using ACT.Data.Models;

namespace ACT.Core.Models
{
    public class UserModel : IUser
    {
        #region Properties

        public int Id { get; set; }

        public int Code { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime NiceCreatedOn { get; set; }

        public string Number { get; set; }

        public  string Message { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string IdNumber { get; set; }

        public string TaxNumber { get; set; }

        public bool IsSAId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Cell { get; set; }

        public Status Status { get; set; }

        public string JobTitle { get; set; }

        public string EmployeeNo { get; set; }

        public bool IsAdmin { get; set; }

        public string SelfieUrl { get; set; }

        public string IdPassportUrl { get; set; }

        public string PayRewardsTo { get; set; }


        public Role Role { get; set; }

        public RoleType RoleType { get; set; }

        public RoleModel RoleModel { get; set; }

        public List<Role> Roles { get; set; }

        #endregion

        #region IUser Members

        public string Username
        {
            get
            {
                return this.Email;
            }
        }

        IRole IUser.Role
        {
            get
            {
                return this.RoleModel;
            }
        }

        public string[] GetAspNetRoles()
        {

            if ( this.RoleModel == null )
            {
                throw new Exception( "UserModel.Role must be populated in order to implement IUser" );
            }

            return ( RoleModel as IRole ).GetAspNetRoles();

        }

        #endregion
    }
}
