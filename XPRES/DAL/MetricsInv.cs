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
    
    public partial class MetricsInv
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<double> TotalUnits { get; set; }
        public Nullable<double> UnitsSub { get; set; }
        public Nullable<double> UnitsAdd { get; set; }
        public Nullable<double> UnitsNet { get; set; }
        public Nullable<double> UnitsAbs { get; set; }
        public Nullable<double> TotalValue { get; set; }
        public Nullable<double> ValueSub { get; set; }
        public Nullable<double> ValueAdd { get; set; }
        public Nullable<double> ValueNet { get; set; }
        public Nullable<double> ValueAbs { get; set; }
        public Nullable<double> TotalLocations { get; set; }
        public Nullable<double> FirstPassLocations { get; set; }
        public Nullable<double> SecondPassLocations { get; set; }
        public Nullable<double> GCNIL { get; set; }
        public Nullable<double> ExpectedZones { get; set; }
        public Nullable<double> CountedZones { get; set; }
        public string CountArea { get; set; }
        public Nullable<int> CountYear { get; set; }
        public Nullable<int> Qtr { get; set; }
        public Nullable<int> CountMonth { get; set; }
    }
}
