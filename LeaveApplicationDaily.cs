using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AttendancesServices
{
    sealed class LeaveApplicationDaily : CommonQueries, IDisposable
    {
        private readonly DateTime localDate;
        private readonly DateTime localMonthDate;
        private readonly MySqlConnection conn;
        Dictionary<string, string> IUvalues;

        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public LeaveApplicationDaily(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;

            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Leave Application Daily Connection working.");
            }
            else
            {
                Console.WriteLine("Leave Application Daily Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In Leave Application Daily Attendance..........................");
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
            string leaveUsrEmpQry = " select asm_id, start, end, total, type, tbl_leave_applications.status, tbl_leave_applications.approve" +
                " , span, tbl_leave_applications.role_id, tbl_users.name, tbl_users.employee_id " +
                " from tbl_leave_applications inner join tbl_users on (tbl_users.login = tbl_leave_applications.asm_id) " +
                " where  ((`tbl_leave_applications`.`created_at` >= '" + localMonthDate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                " and `tbl_leave_applications`.`created_at` <= '" + localDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ) " +
                " or (`tbl_leave_applications`.`updated_at` >= '" + localMonthDate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                " and `tbl_leave_applications`.`updated_at` <= '" + localDate.ToString("yyyy-MM-dd HH:mm:ss") + "'))  " +
                " and tbl_leave_applications.status not in (0, 3) ";

            Console.Write(" {0}\n", leaveUsrEmpQry);

            DataSet leaveUsrEmpDS = new ();
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
                using (MySqlCommand leaveUsrEmp = new (leaveUsrEmpQry, conn))
                {
                    using (MySqlDataAdapter leaveUsrEmpDA = new (leaveUsrEmp))
                    {
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
            
            IUvalues = new Dictionary<string, string>();
            long count = 0;
            count = AttendanceAsmidAndDateWiseExist(conn, leaveRow["asm_id"].ToString(), date.ToString(), count);

            Dictionary<string, string> leave = GetLeaveEntitle(leaveRow["type"].ToString());
            
            IUvalues["color"] = leave["color"];
            IUvalues["name"] = leaveRow["name"].ToString();
            IUvalues["employee_id"] = leaveRow["employee_id"].ToString();
            IUvalues["sub_title"] = leave["title"];
            IUvalues["applied"] = "1";
            IUvalues["proc"] = "Service";
            IUvalues["span"] = leaveRow["span"].ToString();
            IUvalues["type"] = leave["type"]; // "L";            
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
            conn.Close();
            conn.Dispose();
            GC.SuppressFinalize(this);
        }
        ~LeaveApplicationDaily()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Leave Application Attendance..........................");
        }
    }
}
