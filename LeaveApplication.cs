using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    sealed class LeaveApplication :  CommonQueries, IDisposable
    {
        private DateTime localDate;
        private DateTime localMonthDate;
        private MySqlConnection conn;
        Dictionary<string, string> IUvalues;

        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public LeaveApplication(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;
            
            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Leave Application Connection working.");
            }
            else
            {
                Console.WriteLine("Leave Application Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In Leave Application Attendance..........................");
        }
        public void GetLeaveApplications()
        {
            Console.WriteLine("  Local Date: {0}", localDate);
            Console.WriteLine("  Local Two Month Date: {0}", localMonthDate);

            DataSet leavedanceDS = GetLeavesEmployeeUsers();
            foreach (DataRow leaveRow in leavedanceDS.Tables["Leaves"].Rows)//campaignDS.Tables["Customers"].Rows
            {
                int total = (int)leaveRow["total"];

                if (total > 1)
                {
                    foreach (DateTime day in EachDay((DateTime)leaveRow["start"], (DateTime)leaveRow["end"]))
                    {
                        Console.Write("Bla Bla Bla {0}-------------\n", day);
                        SetLeavesData(day, leaveRow);
                    }
                }
                else
                {
                    SetLeavesData(Convert.ToDateTime(leaveRow["start"]), leaveRow);

                }
                Console.Write(" {0}-------------\n", leaveRow["asm_id"]);
                Console.Write(" {0}-------------\n", leaveRow["start"]);
                Console.Write(" {0}-------------\n", leaveRow["end"]);
                Console.Write(" {0}-------------\n", leaveRow["total"]);
                Console.Write(" {0}-------------\n", leaveRow["type"]);
                Console.Write(" {0}-------------\n", leaveRow["status"]);
                // am_approve, span, role_id
            }
            conn.Close();
            return;
        }

        private DataSet GetLeavesEmployeeUsers()
        {
            /*
             * string leaveUsrEmpQry = " SELECT `kqz_employee`.`EmployeeCode`, `kqz_card`.`CardTime` ";
            leaveUsrEmpQry += " FROM `kqz_employee`	INNER JOIN `kqz_card` ON `kqz_employee`.`EmployeeID` = `kqz_card`.`EmployeeID` ";
            leaveUsrEmpQry += " WHERE `kqz_card`.`CardTime` >= '" + localMonthDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            leaveUsrEmpQry += " AND `kqz_card`.`CardTime` <= '" + localDate.ToString("yyyy-MM-dd HH:mm:ss") + "'  ";
            leaveUsrEmpQry += " ORDER BY `EmployeeCode` ASC, `CardTime` ASC";*/

            string leaveUsrEmpQry = " select asm_id, start, end, total, type, tbl_leave_applications.status, am_approve" +
                " , span, tbl_leave_applications.role_id, tbl_users.name, tbl_users.employee_id " +
                " from tbl_leave_applications inner join tbl_users on (tbl_users.login = tbl_leave_applications.asm_id) " +
                " where  start >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' " +
                " and end <= '" + localDate.ToString("yyyy-MM-dd") + "' " +
                " and tbl_leave_applications.status not in (0, 3) ";

            Console.Write(" {0}\n", leaveUsrEmpQry);

            // OpenConection();
            // MySqlCommand leaveUsrEmp = new MySqlCommand(leaveUsrEmpQry, conn); 
            DataSet leaveUsrEmpDS = new DataSet();
            // DataTable leaveUsrEmpDT = new DataTable();
            // MySqlDataReader leaveUsrEmpRdr;

            try
            {
                // Way 1
                // MySqlCommand leaveUsrEmp = new MySqlCommand(leaveUsrEmpQry, conn);
                // leaveUsrEmp.CommandType = CommandType.Text;
                // MySqlDataReader leaveUsrEmpRdr = leaveUsrEmp.ExecuteReader();

                // Way 2
                // MySqlDataAdapter leaveUsrEmpDA = new MySqlDataAdapter(leaveUsrEmpQry, conn);
                // leaveUsrEmpDA.SelectCommand.CommandType = CommandType.Text;
                // leaveUsrEmpDA.Fill(leaveUsrEmpDS, "Attendance");

                // Way 3
                using (MySqlCommand leaveUsrEmp = new MySqlCommand(leaveUsrEmpQry, conn))
                {
                    using (MySqlDataAdapter leaveUsrEmpDA = new MySqlDataAdapter(leaveUsrEmp)) {
                        leaveUsrEmpDA.SelectCommand.CommandType = CommandType.Text;
                        // leaveUsrEmpDA.Fill(leaveUsrEmpDT);
                        leaveUsrEmpDA.Fill(leaveUsrEmpDS, "Leaves");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }
            /*finally
            {
                conn.Dispose(); // return connection to the pool
            }
            CloseConnection();*/
            return leaveUsrEmpDS;
            // return attenUsrEmpDT;
        }
        private long SetLeavesData(DateTime date, DataRow leaveRow)
        {
          /*  string InsertUpdateQry;
            string insertColumns = string.Empty;
            string insertValues = string.Empty;
            string updateColumns = " proc = 'Service' ";
            string type = leaveRow["type"].ToString();*/

            IUvalues = new Dictionary<string, string>();
            long count = 0;
            count = AttendanceAsmidAndDateWiseExist(conn, leaveRow["asm_id"].ToString(), date.ToString(), count);

            Dictionary<string, string> leave = GetLeaveEntitle(leaveRow["type"].ToString());
            /* shiftTimeDict["leave"] = attenShiftSlr; */

            IUvalues["color"] = leave["color"];
            IUvalues["name"] = leaveRow["name"].ToString();
            IUvalues["employee_id"] = leaveRow["employee_id"].ToString();
            IUvalues["sub_title"] = leave["title"];
            IUvalues["applied"] = "1";
            IUvalues["proc"] = "Service";
            IUvalues["span"] = leaveRow["span"].ToString();
            IUvalues["type"] = "L";
            
            IUvalues["date"] = date.ToString();
            IUvalues["asm_id"] = leaveRow["asm_id"].ToString();

            if (count <= 0)
            {
                IUvalues["sap_code"] = leaveRow["asm_id"].ToString();
                IUvalues["title"] = leave["title"];
                IUvalues["missed"] = "0";
                IUvalues["latearrival"] = "0";
                IUvalues["Intimefound"] = "0";
                IUvalues["Outtimefound"] = "0";
                IUvalues["Shifttimefound"] = "0";
                IUvalues["difference"] = "00:00:00";
                IUvalues["ShiftStart"] = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                IUvalues["ShiftEnd"] = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                IUvalues["InDatetime"] = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                IUvalues["OutDatetime"] = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                IUvalues["Outtime"] = "00:00:00";
                IUvalues["Intime"] = "00:00:00";
                IUvalues["shifttime"] = "00:00:00";
                IUvalues["ShiftDatetime"] = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                

                AttendaceInsert(conn, IUvalues, count);
            }
            else
            {
                AttendaceUpdate(conn, IUvalues, count);
            }
    
            // CloseConnection();
            return count;
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
        ~LeaveApplication()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Leave Application Attendance..........................");
        }
    }
}
