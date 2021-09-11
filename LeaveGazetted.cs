using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AttendancesServices
{
    sealed class LeaveGazetted : CommonQueries, IDisposable
    {
        private readonly DateTime localDate; // = DateTime.Now.AddDays(-1);
        private readonly DateTime localMonthDate; // = DateTime.Now.AddMonths(-2);
        private readonly MySqlConnection conn;
        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public LeaveGazetted(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;
            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Gazetted Connection working.");
            }
            else
            {
                Console.WriteLine("Gazetted Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In Gazetted Machine Attendance..........................");
        }
        public void GetGazetted()
        {
            // string SelectCheckedIn = string.Empty; " and year(date) = '" + localDate.ToString("yyyy") + "' " +
            string SelectCheckedIn = " select date, type, form " +
                                     " from tbl_leave_gazetted " +
                                     " where status = 1 " +
                                     " and ( " +
                                     " ( date <= '" + localDate.ToString("yyyy-MM-dd") + "' " +
                                     "      AND date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' ) " +
                                     " or ( date(created) <= '" + localDate.ToString("yyyy-MM-dd") + "' " +
                                     "      AND date(created) >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' ) " +
                                     " ) ";


            // string SelectCheck = "select count(*) cnt, asm_id from tbl_attendances_machine where date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' and date <= '" + localDate.ToString("yyyy-MM-dd") + "' group by asm_id";
            Console.Write(" {0}\n", SelectCheckedIn);

            //OpenConection();
            DataTable SelectCheckedInDT = new();
            Dictionary<string, string> employeeUserDict;

            using (MySqlCommand SelectCheckedInCmd = new(SelectCheckedIn, conn))
            {
                using (MySqlDataAdapter attenUsrEmpDA = new(SelectCheckedInCmd))  //using (MySqlDataReader SelectCheckedInRdr = SelectCheckedInCmd.ExecuteReader())
                {
                    attenUsrEmpDA.SelectCommand.CommandType = CommandType.Text;
                    attenUsrEmpDA.Fill(SelectCheckedInDT);

                    //string SelectChecked = string.Empty;

                    Dictionary<string, string> IUvalues = new();
                    // Dictionary<string, string> EmployeeUserDict = new Dictionary<string, string>();

                    foreach (DataRow SelectCheckedInRdr in SelectCheckedInDT.Rows) //if (SelectCheckedInRdr.HasRows)
                    {
                        //while (SelectCheckedInRdr.Read())
                        //{
                        DataTable SelectCheckDt = UsersList();
                        foreach (DataRow SelectCheckRdr in SelectCheckDt.Rows)
                        {
                            long count = 0;
                            count = AttendanceAsmidAndDateWiseExist(conn, SelectCheckRdr["asm_id"].ToString(), SelectCheckedInRdr["date"].ToString(), count);
                            if (count <= 0)
                            {
                                employeeUserDict = GetUserEmployee(SelectCheckRdr["asm_id"].ToString(), conn);//localDate.ToString("yyyy-MM-dd"),
                                if (!employeeUserDict.ContainsKey("asm_id"))
                                {
                                    continue;
                                }
                                string form = SelectCheckedInRdr["form"].ToString();
                                if ((form.Contains("weekly_off") && form.Contains(employeeUserDict["weeklyOff"])) || form.Contains("volunteer"))
                                {
                                    Dictionary<string, string> leave = GetLeaveEntitle(form);

                                    IUvalues["type"] = leave["type"];
                                    IUvalues["color"] = leave["color"];
                                    /* IUvalues["title"] = leave["title"];
                                    IUvalues["color"] = leave["color"];*/

                                    IUvalues["employee_id"] = employeeUserDict["employee_id"];
                                    IUvalues["name"] = employeeUserDict["name"];
                                    IUvalues["asm_id"] = SelectCheckRdr["asm_id"].ToString();
                                    IUvalues["sap_code"] = SelectCheckRdr["asm_id"].ToString();
                                    IUvalues["applied"] = "0";
                                    IUvalues["span"] = "0";
                                    IUvalues["proc"] = "Service";
                                    IUvalues["sub_title"] = IUvalues["title"] = SelectCheckedInRdr["type"].ToString();
                                    IUvalues["date"] = Convert.ToDateTime(SelectCheckedInRdr["date"]).ToString("yyyy-MM-dd");
                                    IUvalues["missed"] = "0";
                                    IUvalues["latearrival"] = "0";
                                    IUvalues["Intimefound"] = "0";
                                    IUvalues["Outtimefound"] = "0";
                                    IUvalues["Shifttimefound"] = "0";
                                    IUvalues["difference"] = "00:00:00";
                                    IUvalues["ShiftStart"] = Convert.ToDateTime(SelectCheckedInRdr["date"]).ToString("yyyy-MM-dd");
                                    IUvalues["ShiftEnd"] = Convert.ToDateTime(SelectCheckedInRdr["date"]).ToString("yyyy-MM-dd");
                                    IUvalues["InDatetime"] = Convert.ToDateTime(SelectCheckedInRdr["date"]).ToString("yyyy-MM-dd");
                                    IUvalues["OutDatetime"] = Convert.ToDateTime(SelectCheckedInRdr["date"]).ToString("yyyy-MM-dd");
                                    IUvalues["Outtime"] = "00:00:00";
                                    IUvalues["Intime"] = "00:00:00";
                                    IUvalues["shifttime"] = "00:00:00";
                                    IUvalues["ShiftDatetime"] = Convert.ToDateTime(SelectCheckedInRdr["date"]).ToString("yyyy-MM-dd");

                                    AttendaceInsert(conn, IUvalues, count);
                                }
                                //SetWeeklyOffData(WeeklyData, Convert.ToDateTime(SelectCheckedInRdr["date"]));
                            }


                        }
                        //}
                        SelectCheckDt.Dispose();
                    }

                }
            }

            conn.Close();
            return;
        }
        private DataTable UsersList()
        {
            string SelectCheck = "select count(*) cnt, asm_id from tbl_attendances_machine where date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' and date <= '" + localDate.ToString("yyyy-MM-dd") + "' group by asm_id";
            Console.Write(" {0}\n", SelectCheck);
            DataTable SelectCheckDt = new();

            using (MySqlCommand SelectCheckCmd = new(SelectCheck, conn))
            {
                SelectCheckCmd.CommandType = CommandType.Text;
                MySqlDataReader SelectCheckRdr = SelectCheckCmd.ExecuteReader();
                SelectCheckDt.Load(SelectCheckRdr);
            }
            return SelectCheckDt;
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
        ~LeaveGazetted()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Gazetted Attendance..........................");
        }
    }
}
