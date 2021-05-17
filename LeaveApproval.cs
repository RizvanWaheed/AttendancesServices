using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    class LeaveApproval : Common, IDisposable
    {
        private readonly DateTime localDate = DateTime.Now;
        private readonly DateTime localMonthDate = DateTime.Now.AddMonths(-4);
        private MySqlConnection conn;
        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public LeaveApproval()
        {
            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Monthly Attendance Connection working.");
            }
            else
            {
                Console.WriteLine("Monthly Attendance Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In Monthly Attendance..........................");
        }
        public void GetLeaveApproval()
        {



            //return;
        }
        public void Dispose()
        {
            // Using the dispose pattern
            // Dispose(true);
            // … release unmanaged resources here
            conn.Close();
            conn.Dispose();
            GC.SuppressFinalize(this);
        }
        ~LeaveApproval()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Monthly Attendance ..........................");
        }
    }
}
