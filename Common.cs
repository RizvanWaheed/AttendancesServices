using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    class Common
    {
        protected static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        protected static int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new (date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }
        protected static int NumberOfParticularDaysInMonth(int year, int month, DayOfWeek dayOfWeek)
        {
            DateTime startDate = new (year, month, 1);
            int totalDays = startDate.AddMonths(1).Subtract(startDate).Days;

            int answer = Enumerable.Range(1, totalDays)
                .Select(item => new DateTime(year, month, item))
                .Where(date => date.DayOfWeek == dayOfWeek)
                .Count();

            return answer;
        }
        /*protected int NumberOfSaturdays(DateTime start, DateTime end)
        {
            return start.GetDaysInBetween(end, inclusive: true).Count(d => d.DayOfWeek == DayOfWeek.Monday);

        }*/
        protected static Dictionary<string, string> GetLeaveEntitle(string attenShiftSlr)
        {
            Dictionary<string, string> shiftTimeDict = new ();
            

            //if (!string.IsNullOrEmpty(attenShiftSlr))
            //{
            if (attenShiftSlr.CompareTo("annual") == 0) //(attenShiftSlr == "annual")
            {
                shiftTimeDict["title"] = "Annual Leave";
                shiftTimeDict["color"] = "#28a745";
                shiftTimeDict["leave"] = "annual"; // attenShiftSlr;
                shiftTimeDict["type"] = "AL";
            }
            else if (attenShiftSlr.CompareTo("sick") == 0 || attenShiftSlr.CompareTo("sl") == 0) // (attenShiftSlr == "sl")
            {
                shiftTimeDict["title"] = "Sick Leave";
                shiftTimeDict["color"] = "#17a2b8";
                shiftTimeDict["leave"] = "sick";
                shiftTimeDict["type"] = "SL";
            }
            else if (attenShiftSlr.CompareTo("casual") == 0 || attenShiftSlr.CompareTo("cl") == 0) // (attenShiftSlr == "cl")
            {
                shiftTimeDict["title"] = "Casual Leave";
                shiftTimeDict["color"] = "#a011a5";
                shiftTimeDict["leave"] = "casual";
                shiftTimeDict["type"] = "CL";
            }
            else if (attenShiftSlr.CompareTo("off_call") == 0 || attenShiftSlr.CompareTo("oc") == 0) //(attenShiftSlr == "oc")
            {
                shiftTimeDict["title"] = "Off Call";
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = "off_call";
                shiftTimeDict["type"]  = "OCO";
            }
            else if (attenShiftSlr.CompareTo("volunteer") == 0 || attenShiftSlr.Contains("volunteer")  || attenShiftSlr.CompareTo("vl") == 0) //(attenShiftSlr == "vl")
            {
                shiftTimeDict["title"] = "Volunteer";
                shiftTimeDict["color"] = "#dc3545";
                shiftTimeDict["leave"] = "volunteer";
                shiftTimeDict["type"]  = "VO";
            }
            else if (attenShiftSlr.CompareTo("weekly_off") == 0 || attenShiftSlr.Contains("weekly_off") || attenShiftSlr.ToLower() == "off") // ()
            {
                shiftTimeDict["title"] = "Weekly Off";
                shiftTimeDict["color"] = "#07d4a1";
                shiftTimeDict["leave"] = "weekly_off";
                shiftTimeDict["type"]  = "WO";
            }
            else if (attenShiftSlr.CompareTo("absent") == 0 || attenShiftSlr.ToLower() == "a" || attenShiftSlr.ToLower() == "ta") // ()
            {
                shiftTimeDict["title"] = "Absent";
                shiftTimeDict["color"] = "#e83e8c";
                shiftTimeDict["leave"] = "absent";
                shiftTimeDict["type"]  = "A";
            }
            else if (attenShiftSlr.CompareTo("present") == 0 || attenShiftSlr.ToLower() == "p") //()
            {
                shiftTimeDict["title"] = "Present";
                shiftTimeDict["color"] = "#01ff70";
                shiftTimeDict["leave"] = "present";
                shiftTimeDict["type"]  = "P";
            }
            else if (attenShiftSlr.CompareTo("paternity") == 0 || attenShiftSlr.ToLower() == "pl")
            {
                shiftTimeDict["title"] = "Paternity";
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = "paternity";
                shiftTimeDict["type"] = "PL";
            }
            else if (attenShiftSlr.CompareTo("meternity") == 0 || attenShiftSlr.ToLower() == "ml")
            {
                shiftTimeDict["title"] = "Maternity";
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = "meternity";
                shiftTimeDict["type"]  = "ML";
            }
            else if (attenShiftSlr == "hold")
            {
                shiftTimeDict["title"] = "On Hold";
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = "hold";
                shiftTimeDict["type"]  = "HL";
                // }else if($leave == 'os'){
                //     return array('title' => 'Out Station', 'color' => '#b10000', 'leave' => $leave);
            }
            else if (attenShiftSlr.CompareTo("out_station") == 0 || attenShiftSlr == "od" || attenShiftSlr == "os")
            {
                shiftTimeDict["title"] = "Official Day Out";
                shiftTimeDict["color"] = "#b10000";
                shiftTimeDict["leave"] = "out_station";
                shiftTimeDict["type"]  = "OSO";
            }
            else if (attenShiftSlr.CompareTo("away_day") == 0 || attenShiftSlr == "ad" )
            {
                shiftTimeDict["title"] = "Away Day";
                shiftTimeDict["color"] = "#b10000";
                shiftTimeDict["leave"] = "away_day";
                shiftTimeDict["type"] = "ADO";
            }
            else if (attenShiftSlr.CompareTo("client_visit") == 0 || attenShiftSlr == "cv")
            {
                shiftTimeDict["title"] = "Client Visit";
                shiftTimeDict["color"] = "#b10000";
                shiftTimeDict["leave"] = "client_visit";
                shiftTimeDict["type"] = "CVO";
            }
            else if (String.Equals(attenShiftSlr, "tp") || String.Equals(attenShiftSlr, "training"))
            {
                shiftTimeDict["title"] = "Training";
                shiftTimeDict["color"] = "#b10000";
                shiftTimeDict["leave"] = "trainig";
                shiftTimeDict["type"]  = "TO";
            }
            else if (attenShiftSlr.CompareTo("missed") == 0 || attenShiftSlr == "mp")
            {
                shiftTimeDict["title"] = "Missed";
                shiftTimeDict["color"] = "#b10000";
                shiftTimeDict["leave"] = "missed";
                shiftTimeDict["type"] = "M";
                // }else if($leave == 'Pen'){
                //     return array('title' => 'Present', 'color' => '#01ff70', 'leave' => $leave);
            }
            else
            {
                shiftTimeDict["title"] = attenShiftSlr;
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = attenShiftSlr;
                shiftTimeDict["type"] = "U";
            }
            return shiftTimeDict;
        }


    }
}
