using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    sealed class AttendancesShifts : Common, IDisposable
    {
        private readonly DateTime localDate;
        private readonly DateTime localMonthDate;
        private readonly MySqlConnection conn;
        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;

        public AttendancesShifts(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;

            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Daily Connection working.");
            }
            else
            {
                Console.WriteLine("Daily Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................. In Attendance Shift ................................");
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
        ~AttendancesShifts()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine("............................. Out Attendance Shift ...............................");
        }

    }
}
