﻿using System;
using System.Timers;

namespace AttendancesServices
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo cki;
            Console.Clear();
            // Establish an event handler to process key press events.
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleCustomeHandler);
            // Console.WriteLine("Hello World!");
            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            OnTimedEventFirst();
            System.Timers.Timer timer = new();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 4800000; //number in milisecinds  
            timer.Enabled = true;

            /*Task task = new Task(() =>
            {
                AttendanceTask svc = new AttendanceTask();
                while (true)
                {
                    svc.AttendanceTaskServices();
                    Thread.Sleep(2400000);
                }
            });
            task.Start();*/


            cki = Console.ReadKey(true);
            var cki2 = Console.Read();
            var cki3 = Console.ReadLine();
        }
        protected static void ConsoleCustomeHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("\nThe read operation has been interrupted.");
            Console.WriteLine("  Key pressed: {0}", args.SpecialKey);
            Console.WriteLine("  Cancel property: {0}", args.Cancel);

            // Set the Cancel property to true to prevent the process from terminating.
            Console.WriteLine("Setting the Cancel property to true...");
            args.Cancel = true;

            // Announce the new value of the Cancel property.
            Console.WriteLine("  Cancel property: {0}", args.Cancel);
            Console.WriteLine("The read operation will resume...\n");
        }
        private static void OnTimedEventFirst()
        {
            /* AttendanceTask svc = new();
             svc.AttendanceTaskServicesInitials();*/
            DateTime FromDate = DateTime.Now.AddDays(-150);
            DateTime ToDate = DateTime.Now.AddDays(-1);

            AttendanceTask svc = new(FromDate, ToDate);
            try
            {
                svc.AttendanceTaskServices();
            }
            finally
            {
                svc.Dispose();
            }


        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("I am in time lapsed.");

            DateTime FromDate = DateTime.Now.AddDays(-1);
            DateTime ToDate = DateTime.Now;

            AttendanceTaskDaily svcd = new(FromDate, ToDate);
            try
            {
                svcd.DailyServicesAttendance();
            }
            finally
            {
                svcd.Dispose();
            }
        }
    }
}
