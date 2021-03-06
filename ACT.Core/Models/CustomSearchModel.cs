﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using ACT.Core.Enums;
using ACT.Core.Services;
using ACT.Data.Models;
using System.Reflection;
using ACT.Core.Models.Simple;

namespace ACT.Core.Models
{
    public class CustomSearchModel
    {
        public CustomSearchModel()
        {
            SetDefaults();
        }



        #region Properties

        /// <summary>
        /// Can be used as a selected user 
        /// </summary>
        [Display( Name = "User" )]
        public int UserId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Role 
        /// </summary>
        [Display( Name = "Role" )]
        public int RoleId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Client 
        /// </summary>
        [Display( Name = "Client" )]
        public int ClientId
        {
            get; set;
        }

        /// <summary>
        /// Can be used as a selected Product 
        /// </summary>
        [Display( Name = "Product" )]
        public int ProductId
        {
            get; set;
        }

        /// <summary>
        /// Can be used for any entity requiring bank filter
        /// </summary>
        [Display( Name = "Bank" )]
        public int BankId
        {
            get; set;
        }

        /// <summary>
        /// Can be used for a Start date range
        /// </summary>
        [Display( Name = "From Date" )]
        public DateTime? FromDate
        {
            get; set;
        }

        /// <summary>
        /// Can be used for a End date range
        /// </summary>
        [Display( Name = "To" )]
        public DateTime? ToDate
        {
            get; set;
        }

        /// <summary>
        /// Can be used for any Document Type
        /// </summary>
        [Display( Name = "Document Type" )]
        public DocumentType DocumentType
        {
            get; set;
        }

        /// <summary>
        /// Can be used to indicate Activity Type
        /// </summary>
        [Display( Name = "Activity" )]
        public ActivityTypes ActivityType
        {
            get; set;
        }

        /// <summary>
        /// Can be used to indicate Role Type
        /// </summary>
        [Display( Name = "Role Type" )]
        public RoleType RoleType
        {
            get; set;
        }

        /// <summary>
        /// Can be used to indicate Province
        /// </summary>
        [Display( Name = "Province" )]
        public Province Province
        {
            get; set;
        }

        /// <summary>
        /// A custom search query
        /// </summary>
        [Display( Name = "Search Text" )]
        public string Query
        {
            get; set;
        }

        /// <summary>
        /// Status
        /// </summary>
        [Display( Name = "Status" )]
        public Status Status
        {
            get; set;
        }

        [Display( Name = "Invoice Status" )]
        public InvoiceStatus InvoiceStatus
        {
            get;
            set;
        }

        [Display( Name = "Status" )]
        public PSPClientStatus PSPClientStatus
        {
            get;
            set;
        }

        public decimal? Amount { get; set; }

        [Display( Name = "Object Id" )]
        public int ObjectId
        {
            get; set;
        }

        [Display( Name = "Target Table" )]
        public string TableName
        {
            get; set;
        }

        [Display( Name = "Controller Name" )]
        public string ControllerName
        {
            get; set;
        }

        /// <summary>
        /// !SYSTEM: This is automatically set depending on page you're currently viewing
        /// </summary>
        public string ReturnView
        {
            get; set;
        }

        /// <summary>
        /// !SYSTEM: This is automatically set depending on page you're currently viewing
        /// </summary>
        public string Controller
        {
            get; set;
        }

        #endregion



        #region Model Options

        public List<SimpleUserModel> UserOptions
        {
            get
            {
                using ( UserService service = new UserService() )
                {
                    return service.List( true );
                }
            }
        }

        public List<Bank> BankOptions
        {
            get
            {
                using ( BankService service = new BankService() )
                {
                    return service.List();
                }
            }
            set
            {
            }
        }

        public Dictionary<int, string> ClientOptions { get; set; }

        public Dictionary<int, string> ProductOptions { get; set; }

        public List<string> TableNameOptions
        {
            get
            {
                return Assembly.Load( "ACT.Data" )
                               .GetTypes()
                               .Where( a => string.Equals( a.Namespace, "ACT.Data.Models", StringComparison.Ordinal ) )
                               .Select( s => s.Name )
                               .ToList();
            }
        }

        public List<string> ControllerNameOptions
        {
            get
            {
                return MvcHelper.GetControllerNames();
            }
        }

        #endregion



        /// <summary>
        /// There's a common use for Branch, DirectorateProject and DepartmentSubProject  etc
        /// This function will help generically retrieve a unique list of the above 3 from a specified table
        /// CAUTION!! Only use this if the table you're trying to query has the above 3 "string" columns and are spelled exactly as above!
        /// If the table doesn't have, then using this function will break your request, if not spelled the same, kindly make it spelled as
        /// above to enjoy me!
        /// LOOK at /Views/PaymentRequisition/_List and then /Views/Shared/_PRCustomSearch for usage
        /// </summary>
        /// <param name="listType"></param>
        public CustomSearchModel( string listType )
        {
            SetDefaults();

            switch ( listType )
            {
                case "Users":

                    

                    break;

                case "PSPs":

                    using ( ClientService cservice = new ClientService() )
                    using ( ProductService pservice = new ProductService() )
                    {
                        ClientOptions = cservice.List( true );
                        ProductOptions = pservice.List( true );
                    }

                    break;
            }
        }



        private void SetDefaults()
        {
            this.Status = Status.All;
            this.Province = Province.All;
            this.RoleType = RoleType.All;
            this.DocumentType = DocumentType.All;
            this.ActivityType = ActivityTypes.All;
            this.InvoiceStatus = InvoiceStatus.All;
            this.PSPClientStatus = PSPClientStatus.All;
        }
    }
}
