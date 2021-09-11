using System;
using System.Threading;

namespace AttendancesServices
{
    class AttendanceTaskDaily : IDisposable
    {
        private readonly DateTime FromDate;
        private readonly DateTime ToDate;
        private readonly DateTime AbsentFromDate;
        private readonly DateTime AbsentToDate;
        private Thread AmThread, KpiThread, LeaveThread, GazThread, AbsentiesRemoveThread, DailyThread, RunShiftDailyThread, EmployeeLogMappingThread, AbsentiesThread; //OffThread, 

        /* HttpClient client = new HttpClient();*/
        // String strHostName = string.Empty;
        //Services()
        // {
        //IPAddress[] addresslist = Dns.GetHostAddresses(Dns.GetHostName());
        // }
        public AttendanceTaskDaily(DateTime fromDate, DateTime toDate)
        {
            ToDate = toDate;
            FromDate = fromDate;
            AbsentFromDate = fromDate.AddDays(-7);
            AbsentToDate = fromDate.AddDays(-1);
            Console.WriteLine(".............................In Attendance Daily Task ..........................");
        }
        public void RemoveAbsent()
        {
            Absent Absenties = new(AbsentFromDate, AbsentToDate);
            try
            {
                Absenties.RemoveAbsentiesDaily();
            }
            finally
            {
                Absenties.Dispose();
            }
        }
        public void AttendanceStatus()
        {
            AttendanceStatusDaily daily = new(FromDate, ToDate);
            try
            {
                daily.GetDailyAttendance();
            }
            finally
            {
                daily.Dispose();
            }
        }
        public void LeaveApplicationsDaily()
        {
            LeaveApplicationDaily LeaveAppDaily = new(FromDate, ToDate);
            try
            {
                LeaveAppDaily.GetLeaveApplications();
            }
            finally
            {
                LeaveAppDaily.Dispose();
            }
        }
        public void KpiAttendanceDaily()
        {
            AttendanceKpiDaily kpiAttendDaily = new(FromDate, ToDate);
            try
            {
                kpiAttendDaily.GetKpiAttendance();
            }
            finally
            {
                kpiAttendDaily.Dispose();
            }
        }
        public void MachineAttendanceDaily()
        {
            AttendanceMachineDaily Attendmachinedaily = new(FromDate, ToDate);
            try
            {
                Attendmachinedaily.GetMachineAttendance();
            }
            finally
            {
                Attendmachinedaily.Dispose();
            }
        }
        public void ShiftDaily()
        {
            AttendancesShifts AttendShift = new(FromDate, ToDate);
            try
            {
                AttendShift.GetAttendanceShiftWise();
            }
            finally
            {
                AttendShift.Dispose();
            }
        }
        public void GazettedDaily()
        {
            LeaveGazetted GazettedOff = new(FromDate, ToDate);
            try
            {
                GazettedOff.GetGazetted();
            }
            finally
            {
                GazettedOff.Dispose();
            }
        }
        public void EmployeeLogMapping()
        {
            EmployeeLogMapping EmployeeLogMap = new(FromDate, ToDate);
            try
            {
                EmployeeLogMap.GetEmployeeLogMappaing();
            }
            finally
            {
                EmployeeLogMap.Dispose();
            }
        }
        public void AbsentDaily()
        {
            Absent Absenties = new(AbsentFromDate, AbsentToDate);
            try
            {
                Absenties.GetAbsenties();
            }
            finally
            {
                Absenties.Dispose();
            }
        }
        public void DailyServicesAttendance()
        {

            TimeSpan start1 = TimeSpan.Parse("01:00");
            TimeSpan start3 = TimeSpan.Parse("03:00");
            TimeSpan start4 = TimeSpan.Parse("04:00");
            TimeSpan start5 = TimeSpan.Parse("05:00");
            TimeSpan start7 = TimeSpan.Parse("07:00");

            TimeSpan end = TimeSpan.Parse("06:00"); // 4 AM
            TimeSpan now = DateTime.Now.TimeOfDay;

            AbsentiesRemoveThread = new(RemoveAbsent);
            AbsentiesRemoveThread.Start();
            AbsentiesRemoveThread.Join();

            DailyThread = new(AttendanceStatus);
            DailyThread.Start();
            DailyThread.Join();

            LeaveThread = new(LeaveApplicationsDaily);
            LeaveThread.Start();
            LeaveThread.Join();

            KpiThread = new(KpiAttendanceDaily);
            KpiThread.Start();
            KpiThread.Join();

            AmThread = new(MachineAttendanceDaily);
            AmThread.Start();
            AmThread.Join();

            RunShiftDailyThread = new(ShiftDaily);
            RunShiftDailyThread.Start();
            RunShiftDailyThread.Join();

            // if (now <= start5)
            // {
            GazThread = new(GazettedDaily); // GazettedOff.GetGazetted();
            GazThread.Start();
            GazThread.Join();

            EmployeeLogMappingThread = new(EmployeeLogMapping);
            EmployeeLogMappingThread.Start();
            EmployeeLogMappingThread.Join();

            AbsentiesThread = new(AbsentDaily);
            AbsentiesThread.Start();
            AbsentiesThread.Join();

            // }
            if (now <= start7 && now > start3) //localDate.Day == 25 &&
            {
                Thread MonthlyAttendanceThread = new(MonthlyDaily); // GazettedOff.GetGazetted();
                MonthlyAttendanceThread.Start();
                MonthlyAttendanceThread.Join();

                Thread Monthly21To20AttendanceThread = new(Monthly21To20Daily); // GazettedOff.GetGazetted();
                Monthly21To20AttendanceThread.Start();
                Monthly21To20AttendanceThread.Join();
            }
        }
        public void MonthlyDaily()
        {

            /*DateTime Start2 = new DateTime(LocalDay.AddMonths(-2).Year, LocalDay.AddMonths(-2).Month, 1);
            DateTime End2 = new DateTime(LocalDay.AddMonths(-2).Year, LocalDay.AddMonths(-2).Month, DateTime.DaysInMonth(LocalDay.AddMonths(-2).Year, LocalDay.AddMonths(-2).Month));
            await LinkageMonthAttendance(Start2, End2);

            DateTime Start1 = new DateTime(LocalDay.AddMonths(-1).Year, LocalDay.AddMonths(-1).Month, 1);
            DateTime End1 = new DateTime(LocalDay.AddMonths(-1).Year, LocalDay.AddMonths(-1).Month, DateTime.DaysInMonth(LocalDay.AddMonths(-1).Year, LocalDay.AddMonths(-1).Month));
            await LinkageMonthAttendance(Start1, End1);

            DateTime Start = new DateTime(LocalDay.Year, LocalDay.Month, 1);
            DateTime End = DateTime.Now;
            await LinkageMonthAttendance(Start, End);*/


            AttendanceMonthly MonthlyAttendance = new();
            try
            {
                MonthlyAttendance.GetAttendanceMonthApproval();
            }
            finally
            {
                MonthlyAttendance.Dispose();
            }
        }
        public void Monthly21To20Daily()
        {
            Attendance21To20 Monthly21To20Attendance = new();
            try
            {
                Monthly21To20Attendance.GetAttendance21To20();
            }
            finally
            {
                Monthly21To20Attendance.Dispose();
            }
        }

        public void Dispose()

        {
            // Using the dispose pattern
            // Dispose(true);
            // … release unmanaged resources here
            GC.SuppressFinalize(this);
        }
        ~AttendanceTaskDaily()
        {
            Console.WriteLine(".............................Out Attendance Task Daily..........................");
        }

    }
}
