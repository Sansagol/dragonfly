//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Notificztion_Task
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Notificztion_Task()
        {
            this.E_mail_Notification = new HashSet<E_mail_Notification>();
        }
    
        public decimal ID_Notificztion_Task { get; set; }
        public Nullable<decimal> Notification_Before_Month { get; set; }
        public Nullable<decimal> Notification_Before_Week { get; set; }
        public Nullable<decimal> Notification_Before_Day { get; set; }
        public Nullable<decimal> Notification_Before_Hour { get; set; }
        public string Notification_Text { get; set; }
        public decimal ID_Notification_Event { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<E_mail_Notification> E_mail_Notification { get; set; }
        public virtual Notification_Event Notification_Event { get; set; }
    }
}