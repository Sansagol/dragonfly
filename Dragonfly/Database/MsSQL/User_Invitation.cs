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
    
    public partial class User_Invitation
    {
        public decimal ID_User_Invitation { get; set; }
        public string E_mail { get; set; }
        public System.DateTime Date_Of_Invitation { get; set; }
        public decimal ID_User_Referrer { get; set; }
        public bool IsAccepted { get; set; }
    
        public virtual User User { get; set; }
    }
}
