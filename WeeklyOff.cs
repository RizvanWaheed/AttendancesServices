using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AttendancesServices
{
    sealed class WeeklyOff : CommonQueries, IDisposable
    {
        private readonly DateTime localDate;
        private readonly DateTime localMonthDate;
        private readonly MySqlConnection conn;

        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public WeeklyOff(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;

            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Weekly Off Connection working.");
            }
            else
            {
                Console.WriteLine("Weekly Off Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In Weekly Off Attendance..........................");
        }


        public void GetOffDays()
        {
            // string SelectChecked; // = string.Empty;
            /*
             * string SelectCheck = "select count(*) cnt, asm_id from tbl_attendances_machine where date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' and date <= '" + localDate.ToString("yyyy-MM-dd")+ "' group by asm_id";
             * Console.Write(" {0}\n", SelectCheck);
             * MySqlCommand SelectCheckCmd = new MySqlCommand(SelectCheck, conn);
             * SelectCheckCmd.CommandType = CommandType.Text;
             * MySqlDataReader SelectCheckRdr = SelectCheckCmd.ExecuteReader();
            */

            DataTable SelectCheckDt = UsersList();

            Dictionary<string, string> employeeUserDict;// = new Dictionary<string, string>();

            foreach (DataRow SelectCheckRdr in SelectCheckDt.Rows)
            {
                // while (SelectCheckRdr.Read())  // 	WEEKDAY("2021-01-26") in (5,6) Saturday and Sunday;
                // {
                Console.Write(" {0}\n", SelectCheckRdr["asm_id"]);
                employeeUserDict = GetUserEmployeeOffDays(SelectCheckRdr["asm_id"].ToString());//localDate.ToString("yyyy-MM-dd"),
                if (!employeeUserDict.ContainsKey("asm_id"))
                {
                    continue;
                }
                //long role = (long) employeeUserDict["role_id"].ToString();

                if (String.Compare(employeeUserDict["role_id"], "317") == 0)//(employeeUserDict["role_id"].Contains("317"))
                {
                    continue;
                }
                Console.Write(" {0}\n", employeeUserDict["weeklyOff"]);
                for (DateTime todayDate = localMonthDate; todayDate < localDate; todayDate = todayDate.AddDays(1))
                {
                    if ((todayDate.DayOfWeek == DayOfWeek.Sunday) || (todayDate.DayOfWeek == DayOfWeek.Saturday))
                    {
                        Dictionary<string, string> WeeklyData = new();

                        WeeklyData["asm_id"] = SelectCheckRdr["asm_id"].ToString();
                        WeeklyData["color"] = "#07d4a1";
                        WeeklyData["title"] = "Weekly Off";
                        WeeklyData["employee_id"] = employeeUserDict["employee_id"];
                        WeeklyData["name"] = employeeUserDict["full_name"];
                        long count = 0;

                        count = AttendanceAsmidAndDateWiseExist(conn, SelectCheckRdr["asm_id"].ToString(), todayDate.ToString("yyyy-MM-dd"), count);

                        // int weeker = GetWeekNumberOfMonth(todayDate);
                        int MonthCount = NumberOfParticularDaysInMonth(todayDate.Year, todayDate.Month, DayOfWeek.Saturday);
                        double HalfMonthCount = (double)MonthCount / 2;
                        int CutOff = (int)Math.Ceiling(HalfMonthCount);



                        if (count <= 0)
                        {
                            // Console.Write(" {0}\n", weeker);
                            if (todayDate.DayOfWeek == DayOfWeek.Sunday)
                            {
                                SetWeeklyOffData(WeeklyData, todayDate);
                            }
                            else if (todayDate.DayOfWeek == DayOfWeek.Saturday && String.Equals(employeeUserDict["weeklyOff"], "dual"))
                            {
                                SetWeeklyOffData(WeeklyData, todayDate);
                            }
                            else if (todayDate.DayOfWeek == DayOfWeek.Saturday && String.Equals(employeeUserDict["weeklyOff"], "alternative")) // && (weeker % 2 == 0)
                            {
                                long OffDaturdaysCount = 0;
                                OffDaturdaysCount = AttendanceAsmidAndYearNadMonthAndDayWiseExist(conn, SelectCheckRdr["asm_id"].ToString(), todayDate.Year, todayDate.Month, "O", 5, OffDaturdaysCount);

                                Console.Write(" MonthCount {0}\n", MonthCount);
                                Console.Write(" HalfMonthCount  {0}\n", HalfMonthCount);
                                Console.Write(" CutOff  {0}\n", CutOff);
                                Console.Write(" OffDaturdaysCount {0}\n", OffDaturdaysCount);

                                if (OffDaturdaysCount <= CutOff)
                                {
                                    SetWeeklyOffData(WeeklyData, todayDate);
                                }
                            }

                        }
                        /*else
                        {
                            //OpenConection();
                            SelectChecked = "select count(*) from tbl_attendances_machine where asm_id = " + SelectCheckRdr["asm_id"] + " and date = '" + todayDate.ToString("yyyy-MM-dd") + "' and type = 'A'"; //" and sap_code = " + item.Key +
                            Console.Write(" {0}\n", SelectChecked);
                            using (MySqlCommand SelectCheckedCommand = new MySqlCommand(SelectChecked, conn))
                            {
                                Int64 counting = (Int64)SelectCheckedCommand.ExecuteScalar();
                                RemoveAbsenties(SelectCheckRdr["asm_id"].ToString(), todayDate);
                                Console.Write(" {0}\n", weeker);
                                if (counting > 0 && todayDate.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    SetWeeklyOffData(WeeklyData, todayDate);
                                }
                                else if (counting > 0 && todayDate.DayOfWeek == DayOfWeek.Saturday && String.Equals(employeeUserDict["weeklyOff"], "dual"))
                                {
                                    SetWeeklyOffData(WeeklyData, todayDate);
                                }
                                else if ((counting > 0) && todayDate.DayOfWeek == DayOfWeek.Saturday && String.Equals(employeeUserDict["weeklyOff"], "alternative") && (weeker % 2 == 0))
                                {
                                    SetWeeklyOffData(WeeklyData, todayDate);
                                }

                            }
                        }*/

                    }
                }
            }
            SelectCheckDt.Dispose();
            conn.Close();
            return;
        }
        /* public void RemoveAbsenties(string asm_id, DateTime todayDate)
         {
             string DeleteQry = " Delete from tbl_attendances_machine ";
             DeleteQry += " where type = 'A' and asm_id = " + asm_id + " and date = '" + todayDate.ToString("yyyy-MM-dd") + "'";

             // OpenConection();
             Console.Write(" {0}\n", DeleteQry);
             MySqlCommand command = new MySqlCommand(DeleteQry, conn);
             if (command.ExecuteNonQuery() != 1)
             {
                 //'handled as needed, //' but this snippet will throw an exception to force a rollback
                 //throw new InvalidProgramException();
             }
             command.Dispose();
             return;
         }*/
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
                // SelectCheckCmd.Dispose();
            }
            return SelectCheckDt;
        }
        private Dictionary<string, string> GetUserEmployeeOffDays(string code)//
        {
            // DateTime cardTime = Convert.ToDateTime(dateValue);
            Dictionary<string, string> employeeUserDict = new();

            string employyeeUserQry = "SELECT  `tbl_employees`.`id`, `tbl_users`.`role_id`, `tbl_users`.`login`, `tbl_employees`.`sap_code`, `tbl_employees`.`full_name`, `tbl_setups`.`slug` ";
            employyeeUserQry += " from tbl_employees ";
            employyeeUserQry += " INNER JOIN `tbl_users` ON `tbl_users`.`employee_id` = `tbl_employees`.`id` and `tbl_users`.`role_id` != '317' ";
            employyeeUserQry += " INNER JOIN `tbl_setups` ON `tbl_setups`.`id` = `tbl_users`.`role_id` ";
            employyeeUserQry += " where tbl_employees.status = 1 " +
                " and (tbl_employees.sap_code = " + code + " or tbl_users.login = " + code + " )" +
                " ORDER BY tbl_employees.created desc limit 1 ";

            // Console.Write(" {0}\n", employyeeUserQry);


            using (MySqlCommand employeeUserCmd = new(employyeeUserQry, conn))
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
            InsertUpdateQry += " values ( " + data["asm_id"] + ", '" + todayDate.ToString("yyyy-MM-dd") + "' ,'" + data["asm_id"] + "' " + insertValues + ", 'Service', 'O' )";

            // OpenConection();
            Console.Write(" {0}\n", InsertUpdateQry);
            MySqlCommand command = new(InsertUpdateQry, conn);
            if (command.ExecuteNonQuery() != 1)
            {
                //'handled as needed, //' but this snippet will throw an exception to force a rollback
                throw new InvalidProgramException();
            }
            command.Dispose();
            // CloseConnection();
            return 1; ;


        }
        public void Dispose()
        {

            // Using the dispose pattern
            // Dispose(true);
            // release unmanaged resources here

            conn.Close();
            conn.Dispose();
            GC.SuppressFinalize(this);

        }
        ~WeeklyOff()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Weekly Attendance..........................");
        }
    }
}
