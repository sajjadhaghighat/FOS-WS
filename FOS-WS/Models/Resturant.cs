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
    
    public partial class Resturant
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Resturant()
        {
            this.Foods = new HashSet<Food>();
        }
    
        public int RID { get; set; }
        public string Rname { get; set; }
        public string Rstatus { get; set; }
        public Nullable<int> UID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Food> Foods { get; set; }
        public virtual User User { get; set; }
    }
}
