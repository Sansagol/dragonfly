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
    
    public partial class User_Access
    {
        public decimal ID_User_Access { get; set; }
        public string Access_Token { get; set; }
        public System.DateTime Date_Creation { get; set; }
        public System.DateTime Expiration_Date { get; set; }
        public decimal ID_User { get; set; }
    
        public virtual User User { get; set; }
    }
}
