using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AttendancesServices
{
    class AttendanceTask
    {
        // private readonly DateTime LocalDayDummy = DateTime.Now.AddDays(-10);

        private readonly DateTime LocalDay = DateTime.Now;//.AddDays(-3);
        private readonly DateTime LocalYesterday = DateTime.Now.AddDays(-1);
        // private readonly DateTime LocalSingleDate = DateTime.Now.AddMonths(-1);
        // private readonly DateTime LocalMonthDate = DateTime.Now.AddMonths(-2);
        private readonly DateTime LocalMonthDate = DateTime.Now.AddDays(-45);

        private Thread AmThread, KpiThread, LeaveThread, GazThread; //OffThread, 

        /* HttpClient client = new HttpClient();*/
        // String strHostName = string.Empty;
        //Services()
        // {
        //IPAddress[] addresslist = Dns.GetHostAddresses(Dns.GetHostName());
        // }
        public void RunRemoveAbsent()
        {
            Absent Absenties = new (LocalMonthDate, LocalYesterday);
            try
            {
                Absenties.RemoveAbsenties();
            }
            finally
            {
                Absenties.Dispose();
            }
        }
        public void RunMachineAttendance()
        {
            AttendanceMachine Attendmachine = new (LocalMonthDate, LocalDay);
            try
            {
                Attendmachine.GetMachineAttendance();
            }
            finally
            {
                Attendmachine.Dispose();
            }
        }
        public void RunMachineAttendanceDaily()
        {
            AttendanceMachineDaily Attendmachinedaily = new (LocalYesterday, LocalDay);
            try
            {
                Attendmachinedaily.GetMachineAttendance();
            }
            finally
            {
                Attendmachinedaily.Dispose();
            }
        }
        public void RunKpiAttendance()
        {
            AttendanceKpi kpiAttend = new (LocalMonthDate, LocalYesterday);
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
        public void RunKpiAttendanceDaily()
        {
            AttendanceKpiDaily kpiAttendDaily = new (LocalYesterday, LocalDay);
            try
            {
                kpiAttendDaily.GetKpiAttendance();
            }
            finally
            {
                kpiAttendDaily.Dispose();
            }
        }
        public void RunEmployeeLogMapping()
        {
            EmployeeLogMapping EmployeeLogMap = new (LocalMonthDate, LocalDay);
            try
            {
                EmployeeLogMap.GetEmployeeLogMappaing();
            }
            finally
            {
                EmployeeLogMap.Dispose();
            }
        }
        public void RunLeaveApplications()
        {
            LeaveApplication LeaveApp = new (LocalMonthDate, LocalDay);
            try
            {
                LeaveApp.GetLeaveApplications();
            }
            finally
            {
                LeaveApp.Dispose();
            }
        }
        public void RunLeaveApplicationsDaily()
        {
            LeaveApplicationDaily LeaveAppDaily = new (LocalYesterday, LocalDay);
            try
            {
                LeaveAppDaily.GetLeaveApplications();
            }
            finally
            {
                LeaveAppDaily.Dispose();
            }
        }
        public void RunDaily()
        {
            AttendanceStatus daily = new (LocalMonthDate, LocalYesterday);
            try
            {
                daily.GetDailyAttendance();
            }
            finally
            {
                daily.Dispose();
            }
        }
        public void RunAttendanceStatusDaily()
        {
            AttendanceStatusDaily daily = new (LocalYesterday, LocalDay);
            try
            {
                daily.GetDailyAttendance();
            }
            finally
            {
                daily.Dispose();
            }
        }
        public void RunGazetted()
        {
            LeaveGazetted GazettedOff = new (LocalMonthDate, LocalYesterday);
            try
            {
                GazettedOff.GetGazetted();
            }
            finally
            {
                GazettedOff.Dispose();
            }
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
        public void RunAbsent()
        {
            Absent Absenties = new (LocalMonthDate, LocalYesterday);
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


            AttendanceMonthly MonthlyAttendance = new ();
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
            Attendance21To20 Monthly21To20Attendance = new ();
            try
            {
                Monthly21To20Attendance.GetAttendance21To20();
            }
            finally
            {
                Monthly21To20Attendance.Dispose();
            }
        }
        public void AttendanceTaskServicesInitials()
        {
            Thread AbsentiesRemoveThread = new (RunRemoveAbsent);
            AbsentiesRemoveThread.Start();
            AbsentiesRemoveThread.Join();


            Thread DailyThread = new (RunDaily);
            DailyThread.Start();
            DailyThread.Join();

            KpiThread = new (RunKpiAttendance);
            KpiThread.Start();
            KpiThread.Join();

            AmThread = new (RunMachineAttendance);
            AmThread.Start();
            AmThread.Join();
            
            LeaveThread = new Thread(RunLeaveApplications);
            LeaveThread.Start();
            LeaveThread.Join();                  

            /*OffThread = new Thread(RunWeeklyOff); // offweeks.GetOffDays();
            OffThread.Start();
            OffThread.Join();*/

            GazThread = new (RunGazetted); // GazettedOff.GetGazetted();
            GazThread.Start();
            GazThread.Join();

            Thread EmployeeLogMappingThread = new (RunEmployeeLogMapping);
            EmployeeLogMappingThread.Start();
            EmployeeLogMappingThread.Join();

           /* 
            *   Thread AbsentiesThread = new Thread(RunAbsent);
                AbsentiesThread.Start();
                AbsentiesThread.Join();
            */

            Thread MonthlyAttendanceThread = new (Monthly); // GazettedOff.GetGazetted();
            MonthlyAttendanceThread.Start();
            MonthlyAttendanceThread.Join();

            Thread Monthly21To20AttendanceThread = new (Monthly21To20); // GazettedOff.GetGazetted();
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
        public void AttendanceTaskServices()
        {
            DateTime localDate = DateTime.Now;

            TimeSpan start1 = TimeSpan.Parse("01:00");
            TimeSpan start3 = TimeSpan.Parse("03:00");
            TimeSpan start4 = TimeSpan.Parse("04:00");
            TimeSpan start5 = TimeSpan.Parse("05:00");
            TimeSpan start7 = TimeSpan.Parse("07:00");

            TimeSpan end = TimeSpan.Parse("06:00"); // 4 AM
            TimeSpan now = DateTime.Now.TimeOfDay;

            Thread DailyThread = new (RunAttendanceStatusDaily);
            DailyThread.Start();
            DailyThread.Join();

            LeaveThread = new Thread(RunLeaveApplicationsDaily);
            LeaveThread.Start();
            LeaveThread.Join();

            KpiThread = new Thread(RunKpiAttendanceDaily);
            KpiThread.Start();
            KpiThread.Join();

            AmThread = new (RunMachineAttendanceDaily);
            AmThread.Start();
            AmThread.Join();

            if (now <= start3)
            {
                Thread EmployeeLogMappingThread = new (RunEmployeeLogMapping);
                EmployeeLogMappingThread.Start();
                EmployeeLogMappingThread.Join();
            }
            if (now <= start7 && now >= start5) //localDate.Day == 25 &&
            {
                Thread MonthlyAttendanceThread = new (Monthly); // GazettedOff.GetGazetted();
                MonthlyAttendanceThread.Start();
                MonthlyAttendanceThread.Join();

                Thread Monthly21To20AttendanceThread = new (Monthly21To20); // GazettedOff.GetGazetted();
                Monthly21To20AttendanceThread.Start();
                Monthly21To20AttendanceThread.Join();
            }
        }
        public void AttendanceTaskServicesTimeWise()
        {
            
            DateTime localDate = DateTime.Now;

            TimeSpan start1 = TimeSpan.Parse("01:00");
            TimeSpan start3 = TimeSpan.Parse("03:00");
            TimeSpan start4 = TimeSpan.Parse("04:00");
            TimeSpan start5 = TimeSpan.Parse("05:00");
            TimeSpan start7 = TimeSpan.Parse("07:00");

            TimeSpan end = TimeSpan.Parse("06:00"); // 4 AM
            TimeSpan now = DateTime.Now.TimeOfDay;

            // MachineAttendance Attendmachine = new MachineAttendance();
            // AmThread = new Thread(Attendmachine.GetMachineAttendance); // Attendmachine.GetMachineAttendance();
            /* Task.Run(() =>
            {
                AmThread.Join();
                Console.WriteLine("Run after thread finished");
            });*/

            if (now >= start7)
            {
                AmThread = new Thread(RunMachineAttendance);
                AmThread.Start();
                AmThread.Join();

               
            }
            else if (now <= start3)
            {
                KpiThread = new (RunKpiAttendance);
                KpiThread.Start();
                KpiThread.Join();

                Thread EmployeeLogMappingThread = new (RunEmployeeLogMapping);
                EmployeeLogMappingThread.Start();
                EmployeeLogMappingThread.Join();

            }
            else if (now <= start5 && now >= start3)
            {
                Thread AbsentiesRemoveThread = new (RunRemoveAbsent);
                AbsentiesRemoveThread.Start();
                AbsentiesRemoveThread.Join();

                Thread DailyThread = new (RunDaily);
                DailyThread.Start();
                DailyThread.Join();

                LeaveThread = new (RunLeaveApplications);
                LeaveThread.Start();
                LeaveThread.Join();

                /*OffThread = new Thread(RunWeeklyOff); // offweeks.GetOffDays();
                OffThread.Start();
                OffThread.Join();*/

                GazThread = new Thread(RunGazetted); // GazettedOff.GetGazetted();
                GazThread.Start();
                GazThread.Join();
                
              /*  Thread AbsentiesThread = new Thread(RunAbsent);
                AbsentiesThread.Start();
                AbsentiesThread.Join();*/

            }

           /* if (localDate.Day == 24 && now >= start5 && now <= start7)
            {
                Thread AbsentiesThread = new Thread(RunAbsent);
                AbsentiesThread.Start();
                AbsentiesThread.Join();
            }*/
            if (now <= start7 && now >= start5) //localDate.Day == 25 &&
            {
                Thread MonthlyAttendanceThread = new (Monthly); // GazettedOff.GetGazetted();
                MonthlyAttendanceThread.Start();
                MonthlyAttendanceThread.Join();

                Thread Monthly21To20AttendanceThread = new (Monthly21To20); // GazettedOff.GetGazetted();
                Monthly21To20AttendanceThread.Start();
                Monthly21To20AttendanceThread.Join();
            }

            /*if (localDate.Day == 25 && now >= start && now <= end)
            {
                OffThread = new Thread(RunWeeklyOff); // offweeks.GetOffDays();
                OffThread.Start();
                OffThread.Join();

                Thread AbsentiesThread = new Thread(RunAbsent); // GazettedOff.GetGazetted();
                AbsentiesThread.Start();
                AbsentiesThread.Join();


                using (LeaveApproval ApprovedLeave = new LeaveApproval())
                {
                    Thread LeaveApprovalThread = new Thread(ApprovedLeave.GetLeaveApproval); // GazettedOff.GetGazetted();
                    LeaveApprovalThread.Start();
                    LeaveApprovalThread.Join();
                    // ApprovedLeave.Dispose();
                }
                using (AttendanceMonthly MonthlyAttendance = new AttendanceMonthly())
                {
                    Thread MonthlyAttendanceThread = new Thread(MonthlyAttendance.GetAttendanceApproval); // GazettedOff.GetGazetted();
                    MonthlyAttendanceThread.Start();
                    MonthlyAttendanceThread.Join();
                    // MonthlyAttendance.Dispose();
                }
            }*/



            //if (now <= start)//|| now <= end
            //{
            /*using (KpiAttendance kpiAttend = new KpiAttendance())
            {
                KpiThread = new Thread(kpiAttend.GetKpiAttendance); //  kpiAttendance.GetKpiAttendance();
                KpiThread.Start();
                KpiThread.Join();
                // kpiAttend.Dispose();
            }*/
            /* using (Daily daily = new Daily())
             {
                 Thread DailyThread = new Thread(daily.GetDailyAttendance); //  kpiAttendance.GetKpiAttendance();
                 DailyThread.Start();
                 DailyThread.Join();
                 // kpiAttend.Dispose();
             }

             using (EmployeeLogMapping EmployeeLogMapping = new EmployeeLogMapping())
             {
                 Thread EmployeeLogMappingThread = new Thread(EmployeeLogMapping.GetEmployeeLogMappaing); // GazettedOff.GetGazetted();
                 EmployeeLogMappingThread.Start();
                 EmployeeLogMappingThread.Join();
                 // EmployeeLogMapping.Dispose();
             }
             //}
             using (LeaveApplication LeaveApp = new LeaveApplication())
             {
                 LeaveThread = new Thread(LeaveApp.GetLeaveApplications); // LeaveApp.GetLeaveApplications();
                 LeaveThread.Start();
                 LeaveThread.Join();
                 // LeaveApp.Dispose();
             }

             if (localDate.Day == 25 && now >= start && now <= end)
             {
                 using (Gazetted GazettedOff = new Gazetted())
                 {
                     GazThread = new Thread(GazettedOff.GetGazetted); // GazettedOff.GetGazetted();
                     GazThread.Start();
                     GazThread.Join();
                     // GazettedOff.Dispose();
                 }
                 using (WeeklyOff offweeks = new WeeklyOff())
                 {
                     OffThread = new Thread(offweeks.GetOffDays); // offweeks.GetOffDays();
                     OffThread.Start();
                     OffThread.Join();
                     // offweeks.Dispose();
                 }
                 using (Absent Absenties = new Absent())
                 {
                     Thread AbsentiesThread = new Thread(Absenties.GetAbsenties); // GazettedOff.GetGazetted();
                     AbsentiesThread.Start();
                     AbsentiesThread.Join();
                     // EmployeeLogMapping.Dispose();
                 }
                 using (LeaveApproval ApprovedLeave = new LeaveApproval())
                 {
                     Thread LeaveApprovalThread = new Thread(ApprovedLeave.GetLeaveApproval); // GazettedOff.GetGazetted();
                     LeaveApprovalThread.Start();
                     LeaveApprovalThread.Join();
                     // ApprovedLeave.Dispose();
                 }
                 using (AttendanceMonthly MonthlyAttendance = new AttendanceMonthly())
                 {
                     Thread MonthlyAttendanceThread = new Thread(MonthlyAttendance.GetAttendanceApproval); // GazettedOff.GetGazetted();
                     MonthlyAttendanceThread.Start();
                     MonthlyAttendanceThread.Join();
                     // MonthlyAttendance.Dispose();
                 }
             }*/





            // private void MyMethod(string param1, int param2)
            // {
            //      do stuff
            // }
            // Thread myNewThread = new Thread(() => MyMethod("param1", 5));
            // myNewThread.Start();

            /*AmThread.Start();
            AmThread.Start();
            AmThread.Start();
            AmThread.Start();*/

            // SyncAttendancesServices();
            /*if (now <= start)//|| now <= end
            {
                SyncKpiAttendance(); // current time is between start and stop
            }
            SyncLeaveApplications();
            SyncOffDays();
            SyncGazetted();*/


            /*  oThread = new Thread(SyncLeaveApplications);
            iThread = new Thread(SyncLeaveApplications);

            // SendDialCallThreads(campaignDS);
            // Thread oThread = new Thread(() => SendDialCallThreads(campaignDS));

            oThread.Start();
            oThread.Join();*/
        }
    }
}
