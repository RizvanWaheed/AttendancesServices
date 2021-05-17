using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    class Common
    {
        protected IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        protected int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }
        protected int NumberOfParticularDaysInMonth(int year, int month, DayOfWeek dayOfWeek)
        {
            DateTime startDate = new DateTime(year, month, 1);
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
        protected Dictionary<string, string> GetLeaveEntitle(string attenShiftSlr)
        {
            Dictionary<string, string> shiftTimeDict = new Dictionary<string, string>();

            //if (!string.IsNullOrEmpty(attenShiftSlr))
            //{
            if (attenShiftSlr == "annual")
            {
                shiftTimeDict["title"] = "Annual Leave";
                shiftTimeDict["color"] = "#28a745";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr == "sl")
            {
                shiftTimeDict["title"] = "Sick Leave";
                shiftTimeDict["color"] = "#17a2b8";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr == "cl")
            {
                shiftTimeDict["title"] = "Casual Leave";
                shiftTimeDict["color"] = "#a011a5";
                shiftTimeDict["leave"] = attenShiftSlr;
            }

            else if (attenShiftSlr == "oc")
            {
                shiftTimeDict["title"] = "Off Call";
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr == "vl")
            {
                shiftTimeDict["title"] = "Volunteer";
                shiftTimeDict["color"] = "#dc3545";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr.ToLower() == "off")
            {
                shiftTimeDict["title"] = "Weekly Off";
                shiftTimeDict["color"] = "#07d4a1";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr.ToLower() == "a")
            {
                shiftTimeDict["title"] = "Absent";
                shiftTimeDict["color"] = "#e83e8c";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr.ToLower() == "p")
            {
                shiftTimeDict["title"] = "Present";
                shiftTimeDict["color"] = "#01ff70";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr == "pl")
            {
                shiftTimeDict["title"] = "Paternity";
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr == "ml")
            {
                shiftTimeDict["title"] = "Maternity";
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr == "hold")
            {
                shiftTimeDict["title"] = "On Hold";
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = attenShiftSlr;
                // }else if($leave == 'os'){
                //     return array('title' => 'Out Station', 'color' => '#b10000', 'leave' => $leave);
            }
            else if (attenShiftSlr == "od")
            {
                shiftTimeDict["title"] = "Official Day Out";
                shiftTimeDict["color"] = "#b10000";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            else if (attenShiftSlr == "mp")
            {
                shiftTimeDict["title"] = "Missed";
                shiftTimeDict["color"] = "#b10000";
                shiftTimeDict["leave"] = attenShiftSlr;
                // }else if($leave == 'Pen'){
                //     return array('title' => 'Present', 'color' => '#01ff70', 'leave' => $leave);
            }
            else
            {
                shiftTimeDict["title"] = attenShiftSlr;
                shiftTimeDict["color"] = "#ffc107";
                shiftTimeDict["leave"] = attenShiftSlr;
            }
            return shiftTimeDict;
        }


    }
}
