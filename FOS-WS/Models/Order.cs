//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FOS_WS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public int OID { get; set; }
        public int Oqty { get; set; }
        public string Phone { get; set; }
        public string Timestamp { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
        public Nullable<int> UID { get; set; }
        public Nullable<int> FID { get; set; }
    
        public virtual Food Food { get; set; }
        public virtual User User { get; set; }
    }
}
