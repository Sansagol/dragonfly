﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dragonfly.Database.MsSQL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DragonflyEntities : DbContext
    {
        public DragonflyEntities()
            : base("name=DragonflyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Access_Function> Access_Function { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Client_Type> Client_Type { get; set; }
        public virtual DbSet<E_mail> E_mail { get; set; }
        public virtual DbSet<E_mail_Notification> E_mail_Notification { get; set; }
        public virtual DbSet<Global_User_Role> Global_User_Role { get; set; }
        public virtual DbSet<Invitation_User_Settings> Invitation_User_Settings { get; set; }
        public virtual DbSet<LDAP_Settings> LDAP_Settings { get; set; }
        public virtual DbSet<Notification_Event> Notification_Event { get; set; }
        public virtual DbSet<Notification_Morpheme> Notification_Morpheme { get; set; }
        public virtual DbSet<Notificztion_Task> Notificztion_Task { get; set; }
        public virtual DbSet<Phone> Phone { get; set; }
        public virtual DbSet<Support_Entitlement> Support_Entitlement { get; set; }
        public virtual DbSet<User_Invitation> User_Invitation { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<User_Access> User_Access { get; set; }
        public virtual DbSet<User_Project> User_Project { get; set; }
        public virtual DbSet<Project_Access_Function> Project_Access_Function { get; set; }
        public virtual DbSet<Project_Role> Project_Role { get; set; }
        public virtual DbSet<License_Type> License_Type { get; set; }
        public virtual DbSet<Product_License> Product_License { get; set; }
    
        public virtual int Product_License_Add_New(ObjectParameter entId)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Product_License_Add_New", entId);
        }
    }
}
