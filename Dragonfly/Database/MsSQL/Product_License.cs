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
    
    public partial class Product_License
    {
        public decimal ID_Product_License { get; set; }
        public decimal ID_Project { get; set; }
        public decimal ID_Client { get; set; }
        public System.DateTime Date_Begin { get; set; }
        public System.DateTime Date_End { get; set; }
        public decimal ID_License_Type { get; set; }
        public decimal License_Count { get; set; }
        public string Details { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual License_Type License_Type { get; set; }
        public virtual Project Project { get; set; }
    }
}
