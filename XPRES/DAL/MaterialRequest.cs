//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XPRES.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MaterialRequest
    {
        public int ID { get; set; }
        public string RequestNum { get; set; }
        public string FacilityCode { get; set; }
        public string ProdLine { get; set; }
        public string SubAssem { get; set; }
        public string PartNum { get; set; }
        public Nullable<System.DateTime> SubTimestamp { get; set; }
        public Nullable<System.DateTime> AckTimestamp { get; set; }
        public Nullable<System.DateTime> FillTimestamp { get; set; }
        public Nullable<System.DateTime> DelvrTimestamp { get; set; }
        public string ReqStatus { get; set; }
        public string Comments { get; set; }
        public string InvStatus { get; set; }
        public string ReqQty { get; set; }
    }
}
