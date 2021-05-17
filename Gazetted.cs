using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    sealed class Gazetted: CommonQueries, IDisposable
    {
        private DateTime localDate; // = DateTime.Now.AddDays(-1);
        private DateTime localMonthDate; // = DateTime.Now.AddMonths(-2);
        private MySqlConnection conn;
        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public Gazetted(DateTime From, DateTime To)
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
            // string SelectCheckedIn = string.Empty;
            string SelectCheckedIn = "select date, type from tbl_leave_gazetted where status = 1 and year(date) = '" + localDate.ToString("yyyy") + "' and date <= '" + localDate.ToString("yyyy-MM-dd") + "' and date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' ";
            // string SelectCheck = "select count(*) cnt, asm_id from tbl_attendances_machine where date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' and date <= '" + localDate.ToString("yyyy-MM-dd") + "' group by asm_id";
            Console.Write(" {0}\n", SelectCheckedIn);

            //OpenConection();
            DataTable SelectCheckedInDT = new DataTable();
            Dictionary<string, string> employeeUserDict;

            using (MySqlCommand SelectCheckedInCmd = new MySqlCommand(SelectCheckedIn, conn))
            {
                using (MySqlDataAdapter attenUsrEmpDA = new MySqlDataAdapter(SelectCheckedInCmd))  //using (MySqlDataReader SelectCheckedInRdr = SelectCheckedInCmd.ExecuteReader())
                {
                    attenUsrEmpDA.SelectCommand.CommandType = CommandType.Text;
                    attenUsrEmpDA.Fill(SelectCheckedInDT);

                    //string SelectChecked = string.Empty;

                    Dictionary<string, string> IUvalues = new Dictionary<string, string>();
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
                                employeeUserDict = GetUserEmployeeGazetted(SelectCheckRdr["asm_id"].ToString());//localDate.ToString("yyyy-MM-dd"),
                                if (!employeeUserDict.ContainsKey("asm_id"))
                                {
                                    continue;
                                }
                                IUvalues["employee_id"] = employeeUserDict["employee_id"];
                                IUvalues["name"] = employeeUserDict["full_name"];
                                IUvalues["asm_id"] = SelectCheckRdr["asm_id"].ToString();
                                IUvalues["sap_code"] = SelectCheckRdr["asm_id"].ToString();
                                IUvalues["color"] = "#07d4a1";
                                IUvalues["applied"] = "0";
                                IUvalues["span"] = "0";
                                IUvalues["proc"] = "Service";
                                IUvalues["type"] = "G";
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
            DataTable SelectCheckDt = new DataTable();

            using (MySqlCommand SelectCheckCmd = new MySqlCommand(SelectCheck, conn))
            {
                SelectCheckCmd.CommandType = CommandType.Text;
                MySqlDataReader SelectCheckRdr = SelectCheckCmd.ExecuteReader();
                SelectCheckDt.Load(SelectCheckRdr);
            }
            return SelectCheckDt;
        }
      
        private Dictionary<string, string> GetUserEmployeeGazetted(string code)//
        {
            // DateTime cardTime = Convert.ToDateTime(dateValue);
            Dictionary<string, string> employeeUserDict = new Dictionary<string, string>();

            string employyeeUserQry = "SELECT  `tbl_employees`.`id`, `tbl_users`.`role_id`, `tbl_users`.`login`, `tbl_employees`.`sap_code`, `tbl_employees`.`full_name`, `tbl_setups`.`slug` ";
            employyeeUserQry += " from tbl_employees ";
            employyeeUserQry += " INNER JOIN `tbl_users` ON `tbl_users`.`employee_id` = `tbl_employees`.`id` ";
            employyeeUserQry += " INNER JOIN `tbl_setups` ON `tbl_setups`.`id` = `tbl_users`.`role_id` ";
            employyeeUserQry += " where tbl_employees.status = 1 " +
                " and (tbl_employees.sap_code = " + code + " or tbl_users.login = " + code + " )" +
                " ORDER BY tbl_employees.created desc limit 1 ";

            Console.Write(" {0}\n", employyeeUserQry);


            using (MySqlCommand employeeUserCmd = new MySqlCommand(employyeeUserQry, conn))
            {
                employeeUserCmd.CommandType = CommandType.Text;
                using (MySqlDataReader attenUsrEmpRdr = employeeUserCmd.ExecuteReader())
                {
                    if (attenUsrEmpRdr.HasRows)
                    {
                        while (attenUsrEmpRdr.Read())
                        {
                            employeeUserDict["employee_id"] = attenUsrEmpRdr["id"].ToString();
                            employeeUserDict["role_id"] = attenUsrEmpRdr["role_id"].ToString();
                            employeeUserDict["asm_id"] = attenUsrEmpRdr["login"].ToString();
                            employeeUserDict["sap_code"] = attenUsrEmpRdr["sap_code"].ToString();
                            employeeUserDict["slug"] = attenUsrEmpRdr["slug"].ToString();
                            employeeUserDict["full_name"] = attenUsrEmpRdr["full_name"].ToString();

                            String slug = attenUsrEmpRdr["slug"].ToString();
                            if (slug.Contains("ALTER"))
                            {
                                employeeUserDict["weeklyOff"] = "alternative";
                                employeeUserDict["working_hour"] = "08:30";
                                employeeUserDict["working_hour_number"] = "8.5";
                            }
                            else if (slug.Contains("DUAL"))
                            {
                                employeeUserDict["weeklyOff"] = "dual";
                                employeeUserDict["working_hour"] = "09:00";
                                employeeUserDict["working_hour_number"] = "9.0";
                            }
                            else
                            {
                                employeeUserDict["weeklyOff"] = "single";
                                employeeUserDict["working_hour"] = "08:00";
                                employeeUserDict["working_hour_number"] = "8.0";
                            }
                        }
                    }
                    /*else
                    {
                        employeeUserDict["weeklyOff"] = "single";
                        employeeUserDict["working_hour"] = "08:00";
                        employeeUserDict["working_hour_number"] = "8.0";
                    }*/
                }

            }
            // CloseConnection();
            return employeeUserDict;
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
        ~Gazetted()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Gazetted Attendance..........................");
        }
    }
}
