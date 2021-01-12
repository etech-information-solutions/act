﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACT.Data.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ACTEntities : DbContext
    {
        public ACTEntities()
            : base("name=ACTEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<BankDetail> BankDetails { get; set; }
        public virtual DbSet<Broadcast> Broadcasts { get; set; }
        public virtual DbSet<ChepAudit> ChepAudits { get; set; }
        public virtual DbSet<ChepClient> ChepClients { get; set; }
        public virtual DbSet<ChepLoad> ChepLoads { get; set; }
        public virtual DbSet<ChepLoadChep> ChepLoadCheps { get; set; }
        public virtual DbSet<ChepLoadOld> ChepLoadOlds { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientAuthorisation> ClientAuthorisations { get; set; }
        public virtual DbSet<ClientBudget> ClientBudgets { get; set; }
        public virtual DbSet<ClientCustomer> ClientCustomers { get; set; }
        public virtual DbSet<ClientGroup> ClientGroups { get; set; }
        public virtual DbSet<ClientInvoice> ClientInvoices { get; set; }
        public virtual DbSet<ClientKPI> ClientKPIs { get; set; }
        public virtual DbSet<ClientLoad> ClientLoads { get; set; }
        public virtual DbSet<ClientProduct> ClientProducts { get; set; }
        public virtual DbSet<ClientProductMonthly> ClientProductMonthlies { get; set; }
        public virtual DbSet<ClientSite> ClientSites { get; set; }
        public virtual DbSet<ClientUser> ClientUsers { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<DeclineReason> DeclineReasons { get; set; }
        public virtual DbSet<DeliveryNote> DeliveryNotes { get; set; }
        public virtual DbSet<DeliveryNoteLine> DeliveryNoteLines { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<EstimatedLoad> EstimatedLoads { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<OutstandingReason> OutstandingReasons { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductPrice> ProductPrices { get; set; }
        public virtual DbSet<PSP> PSPs { get; set; }
        public virtual DbSet<PSPBilling> PSPBillings { get; set; }
        public virtual DbSet<PSPBudget> PSPBudgets { get; set; }
        public virtual DbSet<PSPClient> PSPClients { get; set; }
        public virtual DbSet<PSPConfig> PSPConfigs { get; set; }
        public virtual DbSet<PSPProduct> PSPProducts { get; set; }
        public virtual DbSet<PSPUser> PSPUsers { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<SiteAudit> SiteAudits { get; set; }
        public virtual DbSet<SiteBilling> SiteBillings { get; set; }
        public virtual DbSet<SiteBudget> SiteBudgets { get; set; }
        public virtual DbSet<SystemConfig> SystemConfigs { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<Transporter> Transporters { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserBroadcast> UserBroadcasts { get; set; }
        public virtual DbSet<UserDocument> UserDocuments { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserTask> UserTasks { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<PSPPeriod> PSPPeriods { get; set; }
        public virtual DbSet<Dispute> Disputes { get; set; }
    }
}
