using System;
using System.Collections.Generic;
using System.Text;

namespace AttendancesServices.Models
{
    public partial class AttendancesMachine
    {
        public string AsmId { get; set; }
        public string Date { get; set; }
        public string SapCode { get; set; }
        public string Title { get; set; }
        public string InDatetime { get; set; }
        public string Intime { get; set; }
        public string OutDatetime { get; set; }
        public string Outtime { get; set; }
        public string Shifttime { get; set; }
        public string Lesstime { get; set; }
        public string Latearrival { get; set; }
        public string Missed { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string Color { get; set; }
        public string Intimefound { get; set; }
        public string Outtimefound { get; set; }
        public string Shifttimefound { get; set; }
        public string ShiftDatetime { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public string Difference { get; set; }
        public string From { get; set; }

    }
}
