using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AttendancesServices
{
    class AttendanceTaskDaily : IDisposable
    {
        private readonly DateTime FromDate;
        private readonly DateTime ToDate;
        private readonly DateTime AbsentFromDate;
        private readonly DateTime AbsentToDate;
        private Thread AmThread, KpiThread, LeaveThread, GazThread, RunShiftDailyThread, AbsentiesRemoveThread, DailyThread, EmployeeLogMappingThread, AbsentiesThread; //OffThread, 

        public AttendanceTaskDaily(DateTime fromDate, DateTime toDate)
        {
            ToDate = toDate;
            FromDate = fromDate;
            AbsentFromDate = toDate.AddDays(-8);
            AbsentToDate = toDate.AddDays(-2);

            Console.WriteLine(".............................In Attendance Task ..........................");
        }

        public void DailyServicesAttendance()
        {

            DateTime localDate = DateTime.Now;

            TimeSpan start1 = TimeSpan.Parse("01:00");
            TimeSpan start3 = TimeSpan.Parse("03:00");
            TimeSpan start4 = TimeSpan.Parse("04:00");
            TimeSpan start5 = TimeSpan.Parse("05:00");
            TimeSpan start7 = TimeSpan.Parse("07:00");

            TimeSpan end = TimeSpan.Parse("06:00"); // 4 AM
            TimeSpan now = DateTime.Now.TimeOfDay;

            AbsentiesRemoveThread = new(RunRemoveAbsent);
            AbsentiesRemoveThread.Start();
            AbsentiesRemoveThread.Join();

            RunShiftDailyThread = new(RunShiftDaily);
            RunShiftDailyThread.Start();
            RunShiftDailyThread.Join();

            DailyThread = new(RunDaily);
            DailyThread.Start();
            DailyThread.Join();

            KpiThread = new(RunKpiAttendance);
            KpiThread.Start();
            KpiThread.Join();

            AmThread = new(RunMachineAttendance);
            AmThread.Start();
            AmThread.Join();

            LeaveThread = new(RunLeaveApplications);
            LeaveThread.Start();
            LeaveThread.Join();

            GazThread = new(RunGazetted); // GazettedOff.GetGazetted();
            GazThread.Start();
            GazThread.Join();

            EmployeeLogMappingThread = new(RunEmployeeLogMapping);
            EmployeeLogMappingThread.Start();
            EmployeeLogMappingThread.Join();

            AbsentiesThread = new(RunAbsent);
            AbsentiesThread.Start();
            AbsentiesThread.Join();


            if (now <= start7 && now >= start5) //localDate.Day == 25 &&
            {
                Thread MonthlyAttendanceThread = new(Monthly); // GazettedOff.GetGazetted();
                MonthlyAttendanceThread.Start();
                MonthlyAttendanceThread.Join();

                Thread Monthly21To20AttendanceThread = new(Monthly21To20); // GazettedOff.GetGazetted();
                Monthly21To20AttendanceThread.Start();
                Monthly21To20AttendanceThread.Join();
            }
          
        }
        public void RunRemoveAbsent()
        {
            Absent Absenties = new(AbsentFromDate, AbsentToDate);
            try
            {
                Absenties.RemoveAbsenties();
            }
            finally
            {
                Absenties.Dispose();
            }
        }
        public void RunShiftDaily()
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
        public void RunDaily()
        {
            AttendanceStatus daily = new(FromDate, ToDate);
            try
            {
                daily.GetDailyAttendance();
            }
            finally
            {
                daily.Dispose();
            }
        }

        public void RunKpiAttendance()
        {
            AttendanceKpi kpiAttend = new(FromDate, ToDate);
            try
            {
                kpiAttend.GetKpiAttendance();
            }
            finally
            {
                kpiAttend.Dispose();
            }
         
        }

        public void RunMachineAttendance()
        {
            AttendanceMachine Attendmachine = new(FromDate, ToDate);
            try
            {
                Attendmachine.GetMachineAttendance();
            }
            finally
            {
                Attendmachine.Dispose();
            }
        }

        public void RunLeaveApplications()
        {
            LeaveApplication LeaveApp = new(FromDate, ToDate);
            try
            {
                LeaveApp.GetLeaveApplications();
            }
            finally
            {
                LeaveApp.Dispose();
            }
        }

        public void RunGazetted()
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

        public void RunEmployeeLogMapping()
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

        public void RunAbsent()
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

        public void Monthly()
        {
            AttendanceMonthly MonthlyAttendance = new(ToDate);
            try
            {
                MonthlyAttendance.GetAttendanceMonthApproval();
            }
            finally
            {
                MonthlyAttendance.Dispose();
            }
        }

        public void Monthly21To20()
        {
            Attendance21To20 Monthly21To20Attendance = new(ToDate);
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
            Console.WriteLine(".............................Out Attendance Task..........................");
        }
    }
}
