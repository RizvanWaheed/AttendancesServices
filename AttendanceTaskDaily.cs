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

        /* public void RunWeeklyOff()
        {
           WeeklyOff OffWeeks = new WeeklyOff(LocalMonthDate, LocalYesterday);
            try
            {
                OffWeeks.GetOffDays();
            }
            finally
            {
                OffWeeks.Dispose();
            }
        }*/

        public void DailyServicesAttendance()
        {
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

            Thread MonthlyAttendanceThread = new(Monthly); // GazettedOff.GetGazetted();
            MonthlyAttendanceThread.Start();
            MonthlyAttendanceThread.Join();

            Thread Monthly21To20AttendanceThread = new(Monthly21To20); // GazettedOff.GetGazetted();
            Monthly21To20AttendanceThread.Start();
            Monthly21To20AttendanceThread.Join();

            /*if (localDate.Day == 24 && now >= start5 && now <= start7)
            {
                Thread AbsentiesThread = new Thread(RunAbsent);
                AbsentiesThread.Start();
                AbsentiesThread.Join();
            }
            if (localDate.Day == 25 && now <= start7)
            {
            */
            /* 
             * AttendanceMonthly MonthlyAttendance = new AttendanceMonthly();
             Thread MonthlyAttendanceThread = new Thread(MonthlyAttendance.GetAttendanceApproval); // GazettedOff.GetGazetted();
             MonthlyAttendanceThread.Start();
             MonthlyAttendanceThread.Join();*/
            //}
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
            //using (KpiAttendance kpiAttend = new KpiAttendance())
            /*  // {
              KpiThread = new Thread(kpiAttend.GetKpiAttendance); //  kpiAttendance.GetKpiAttendance();
              KpiThread.Start();
              // KpiThread.Join();

              //}
              Task.Run(() =>
              {
                  KpiThread.Join();
                  kpiAttend.Dispose();
              });*/

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

        public void Monthly21To20()
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
            Console.WriteLine(".............................Out Attendance Task..........................");
        }
    }
}
