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
    
    public partial class CountSchedule
    {
        public int ID { get; set; }
        public Nullable<int> Zone { get; set; }
        public string CountArea { get; set; }
        public string CountRange { get; set; }
        public string GoalYear { get; set; }
        public Nullable<int> GoalQuarter { get; set; }
        public Nullable<int> GoalMonth { get; set; }
        public Nullable<int> GoalWeek { get; set; }
        public Nullable<System.DateTime> GoalDate { get; set; }
        public string ActualYear { get; set; }
        public Nullable<int> ActualQuarter { get; set; }
        public Nullable<int> ActualMonth { get; set; }
        public Nullable<int> ActualWeek { get; set; }
        public Nullable<System.DateTime> ActualDate { get; set; }
        public Nullable<System.DateTime> SecondPassDate { get; set; }
        public string FirstCount { get; set; }
        public string SecondCount { get; set; }
        public string CountStatus { get; set; }
        public Nullable<int> CountID { get; set; }
    }
}
