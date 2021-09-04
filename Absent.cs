using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    sealed class Absent : CommonQueries, IDisposable
    {
        private readonly DateTime localDate; // = DateTime.Now.AddMonths(-1);
        private readonly DateTime localMonthDate; // = DateTime.Now.AddMonths(-4);
        private readonly MySqlConnection conn;
        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public Absent(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;
            
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
            Console.WriteLine(".........................  In Monthly Attendance  ..........................");
        }
        public void RemoveAbsenties()
        {
            string DeleteQry = " Delete from tbl_attendances_machine ";
            DeleteQry += " where type not in ('U','L','R','A','M','O','T','G','C','HL','SL','CL','AL','ML','PL','CVO','ADO','WO','VO','OCO','OSO','TO') " +
                        " and date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' " +
                        " and date <= '" + localDate.ToString("yyyy-MM-dd") + "' ";

            // OpenConection();
            Console.Write(" {0}\n", DeleteQry);
            MySqlCommand command = new (DeleteQry, conn);
            if (command.ExecuteNonQuery() != 1){

                //'handled as needed, //' but this snippet will throw an exception to force a rollback
                //throw new InvalidProgramException();
            
            }
            command.Dispose();
            return;
        }
        public void GetAbsenties() { 

            string SelectChecked; // = string.Empty;
            /*
             * string SelectCheck = "select count(*) cnt, asm_id from tbl_attendances_machine where date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' and date <= '" + localDate.ToString("yyyy-MM-dd")+ "' group by asm_id";
             * Console.Write(" {0}\n", SelectCheck);
             * MySqlCommand SelectCheckCmd = new MySqlCommand(SelectCheck, conn);
             * SelectCheckCmd.CommandType = CommandType.Text;
             * MySqlDataReader SelectCheckRdr = SelectCheckCmd.ExecuteReader();
            */
            DataTable SelectCheckDt = UsersList();
            Dictionary<string, string> WeeklyData = new ();
            Dictionary<string, string> employeeUserDict;// = new Dictionary<string, string>();

            foreach (DataRow SelectCheckRdr in SelectCheckDt.Rows)
            {
                // while (SelectCheckRdr.Read())  // 	WEEKDAY("2021-01-26") in (5,6) Saturday and Sunday;
                // {
                Console.Write(" {0}\n", SelectCheckRdr["asm_id"]);
                employeeUserDict = GetUserEmployee(SelectCheckRdr["asm_id"].ToString(), conn);//localDate.ToString("yyyy-MM-dd"),
                if (!employeeUserDict.ContainsKey("asm_id"))
                {
                    continue;
                }

                Console.Write(" {0}\n", employeeUserDict["weeklyOff"]);

                for (DateTime todayDate = localMonthDate; todayDate < localDate; todayDate = todayDate.AddDays(1))
                {
                    // if ((todayDate.DayOfWeek == DayOfWeek.Sunday) || (todayDate.DayOfWeek == DayOfWeek.Saturday))
                    // {
                    // OpenConection();
                    SelectChecked = "select count(*) from tbl_attendances_machine where asm_id = " + SelectCheckRdr["asm_id"] + " and date = '" + todayDate.ToString("yyyy-MM-dd") + "'"; //" and sap_code = " + item.Key +
                    Console.Write(" {0}\n", SelectChecked);
                    using (MySqlCommand SelectCheckedCommand = new (SelectChecked, conn))
                    {
                        Int64 count = (Int64)SelectCheckedCommand.ExecuteScalar();
                        WeeklyData["asm_id"] = SelectCheckRdr["asm_id"].ToString();
                        WeeklyData["color"] = "#e83e8c";
                        WeeklyData["title"] = "Absent";
                        WeeklyData["employee_id"] = employeeUserDict["employee_id"];
                        WeeklyData["name"] = employeeUserDict["name"];

                        // WeeklyData["color"] = "#07d4a1";
                        // WeeklyData["title"] = "Weekly Off";

                        int weeker = GetWeekNumberOfMonth(todayDate);

                        if (count <= 0)
                        {
                            SetWeeklyOffData(WeeklyData, todayDate);
                        }
                    }
                  //  }
                }
            }
            SelectCheckDt.Dispose();
            conn.Close();
            return;

            //return;
        }
        private DataTable UsersList()
        {
            string SelectCheck = "select count(*) cnt, asm_id from tbl_attendances_machine " +
                                   " where date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' " +
                                    "  and date <= '" + localDate.ToString("yyyy-MM-dd") + "' " +
                                    " group by asm_id";
            Console.Write(" {0}\n", SelectCheck);
            DataTable SelectCheckDt = new ();

            using (MySqlCommand SelectCheckCmd = new (SelectCheck, conn))
            {
                SelectCheckCmd.CommandType = CommandType.Text;
                MySqlDataReader SelectCheckRdr = SelectCheckCmd.ExecuteReader();
                SelectCheckDt.Load(SelectCheckRdr);
                // SelectCheckCmd.Dispose();
            }
            return SelectCheckDt;
        }
        private long SetWeeklyOffData(Dictionary<string, string> data, DateTime todayDate)
        {
            string InsertUpdateQry; // = string.Empty;
            string insertColumns = string.Empty;
            string insertValues = string.Empty;

            insertColumns += ", title ";
            insertValues += ", '" + data["title"] + "' ";

            insertColumns += ", sub_title ";
            insertValues += ", '" + data["title"] + "' ";

            insertColumns += ", employee_id ";
            insertValues += ", '" + data["employee_id"] + "' ";

            insertColumns += ", name ";
            insertValues += ", '" + data["name"] + "' ";

            insertColumns += ", span ";
            insertValues += ", '0' ";

            insertColumns += ", color ";
            insertValues += ", '" + data["color"] + "' ";

            insertColumns += ", applied ";
            insertValues += ", '0' ";

            insertColumns += ", missed ";
            insertValues += ", '0' ";

            insertColumns += ", lesstime ";
            insertValues += ", '0' ";

            insertColumns += ", latearrival ";
            insertValues += ", '0' ";

            insertColumns += ", Intimefound ";
            insertValues += ", '0' ";

            insertColumns += ", Outtimefound ";
            insertValues += ", '0' ";

            insertColumns += ", Shifttimefound ";
            insertValues += ", '0' ";

            insertColumns += ", difference ";
            insertValues += ", '00:00:00' ";

            insertColumns += ", ShiftStart ";
            insertValues += ", '" + todayDate.ToString("yyyy-MM-dd") + "' ";

            insertColumns += ", ShiftEnd ";
            insertValues += ", '" + todayDate.ToString("yyyy-MM-dd") + "' ";

            insertColumns += ", InDatetime ";
            insertValues += ", '" + todayDate.ToString("yyyy-MM-dd") + "' ";

            insertColumns += ", Intime ";
            insertValues += ", '00:00:00' ";

            insertColumns += ", OutDatetime ";
            insertValues += ", '" + todayDate.ToString("yyyy-MM-dd") + "' ";

            insertColumns += ", Outtime ";
            insertValues += ", '00:00:00' ";

            insertColumns += ", shifttime ";
            insertValues += ", '00:00:00' ";

            insertColumns += ", ShiftDatetime ";
            insertValues += ", '" + todayDate.ToString("yyyy-MM-dd") + "' ";

            InsertUpdateQry = " INSERT INTO tbl_attendances_machine ( asm_id, date, sap_code" + insertColumns + ", proc, type ) ";
            InsertUpdateQry += " values ( " + data["asm_id"] + ", '" + todayDate.ToString("yyyy-MM-dd") + "' ,'" + data["asm_id"] + "' " + insertValues + ", 'Service', 'A' )";

            // OpenConection();
            Console.Write(" {0}\n", InsertUpdateQry);
            MySqlCommand command = new (InsertUpdateQry, conn);
            if (command.ExecuteNonQuery() != 1)
            {
                //'handled as needed, //' but this snippet will throw an exception to force a rollback
                throw new InvalidProgramException();
            }
            command.Dispose();
            // CloseConnection();
            return 1; 


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
        ~Absent()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Monthly Attendance ..........................");
        }
    }
}
