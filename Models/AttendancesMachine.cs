using System;
using System.Collections.Generic;
using System.Text;

namespace AttendancesServices.Models
{
    public partial class AttendancesMachine
    {
        public string asm_id { get; set; }
        public string date { get; set; }
        public string sap_code { get; set; }
        public string title { get; set; }
        public string InDatetime { get; set; }
        public string Intime { get; set; }
        public string OutDatetime { get; set; }
        public string Outtime { get; set; }
        public string shifttime { get; set; }
        public string lesstime { get; set; }
        public string latearrival { get; set; }
        public string missed { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string color { get; set; }
        public string Intimefound { get; set; }
        public string Outtimefound { get; set; }
        public string Shifttimefound { get; set; }
        public string ShiftDatetime { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public string difference { get; set; }
        public string from { get; set; }

    }
}
