using System;
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

            Timer timer = new();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 6000000; //number in milisecinds  
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
            DateTime FromDate = DateTime.Now.AddDays(-33);
            DateTime ToDate = DateTime.Now.AddDays(-1);

            AttendanceTask svc = new(FromDate, ToDate);
            try
            {
                svc.AttendanceTaskServices();
            }
            catch
            {
                Console.WriteLine("Exception in main...\n");
            }
            finally
            {
                svc.Dispose();
            }



           /* DateTime FromDate1 = DateTime.Now.AddDays(-300);
            DateTime ToDate1 = DateTime.Now.AddDays(-270);

            DateTime FromDate2 = DateTime.Now.AddDays(-270);
            DateTime ToDate2 = DateTime.Now.AddDays(-240);

            DateTime FromDate3 = DateTime.Now.AddDays(-240);
            DateTime ToDate3 = DateTime.Now.AddDays(-210);

            DateTime FromDate4 = DateTime.Now.AddDays(-210);
            DateTime ToDate4 = DateTime.Now.AddDays(-180);

            DateTime FromDate5 = DateTime.Now.AddDays(-180);
            DateTime ToDate5 = DateTime.Now.AddDays(-150);


            AttendanceTask svc1 = new(FromDate1, ToDate1);
            try
            {
                svc1.AttendanceTaskServices();

            }
            finally
            {
                svc1.Dispose();
                AttendanceTask svc2 = new(FromDate2, ToDate2);
                try
                {
                    svc2.AttendanceTaskServices();

                }
                finally
                {
                    svc2.Dispose();
                    AttendanceTask svc3 = new(FromDate3, ToDate3);
                    try
                    {
                        svc3.AttendanceTaskServices();

                    }
                    finally
                    {
                        svc3.Dispose();
                        AttendanceTask svc4 = new(FromDate4, ToDate4);
                        try
                        {
                            svc4.AttendanceTaskServices();

                        }
                        finally
                        {
                            svc4.Dispose();
                            AttendanceTask svc5 = new(FromDate5, ToDate5);
                            try
                            {
                                svc5.AttendanceTaskServices();

                            }
                            finally
                            {
                                svc5.Dispose();
                            }
                        }
                    }
                }
            }
*/
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
            catch
            {
                Console.WriteLine("Exception in daily...\n");
            }
            finally
            {
                svcd.Dispose();
            }
        }
    }
}
