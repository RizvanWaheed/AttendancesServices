using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AttendancesServices
{
    sealed class AttendanceStatusDaily : CommonQueries, IDisposable
    {
        private readonly DateTime localDate;
        private readonly DateTime localMonthDate;
        private readonly MySqlConnection conn;
        public AttendanceStatusDaily(DateTime From, DateTime To)
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
            Console.WriteLine(".............................In Daily Machine Attendance..........................");
        }
        public void GetDailyAttendance()
        {
            string SelectCheck;
            DataSet DailyAttendanceDS = GetDailyAttendanceQuery();
            try
            {
                // OpenConection();
                foreach (DataRow DailyAttendanceDR in DailyAttendanceDS.Tables["DailyAttendance"].Rows)
                {
                    if (DailyAttendanceDR.IsNull("employee_id"))
                    {
                        continue;
                    }
                    SelectCheck = "select count(*) from tbl_attendances_machine where asm_id = " + DailyAttendanceDR["asm_id"] + " and date = '" + Convert.ToDateTime(DailyAttendanceDR["date"]).ToString("yyyy-MM-dd") + "'";
                    Console.Write(" {0}\n", SelectCheck);
                    MySqlCommand SelectCheckCommand = new(SelectCheck, conn);

                    /*SelectCheckCommand.Parameters.AddWithValue("@asm_id", dictInner["asm_id"]);
                    SelectCheckCommand.Parameters.AddWithValue("@date", "'"+itemL2.Key+"'");
                    SelectCheckCommand.Parameters.AddWithValue("@sap_code", "'" + item.Key + "'");*/

                    string InsertUpdateQry = string.Empty;
                    string insertColumns = string.Empty;
                    string insertValues = string.Empty;
                    // string updateColumns = " proc = 'None', applied = 0, span=0 ";

                    Int64 count = (Int64)SelectCheckCommand.ExecuteScalar();
                    SelectCheckCommand.Dispose();
                    TimeSpan actualHours = new(0, 8, 0, 0);
                    //int result = TimeSpan.Compare(actualHours, (TimeSpan)DailyAttendanceDR["title"]);
                    if (count <= 0)
                    {
                        string type = DailyAttendanceDR["title"].ToString().ToLower();
                        Dictionary<string, string> leave = GetLeaveEntitle(DailyAttendanceDR["title"].ToString());
                        insertColumns += ", type ";
                        insertValues += ", '" + leave["type"] + "' ";

                        insertColumns += ", color ";
                        insertValues += ", '" + leave["color"] + "' ";

                        insertColumns += ", name ";
                        insertValues += ", '" + DailyAttendanceDR["name"] + "' ";

                        insertColumns += ", employee_id ";
                        insertValues += ", '" + DailyAttendanceDR["employee_id"] + "' ";

                        insertColumns += ", sub_title ";
                        insertValues += ", '" + leave["title"] + "' ";

                        insertColumns += ", title ";
                        insertValues += ", '" + leave["title"] + "' ";

                        insertColumns += ", missed ";
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
                        insertValues += ", '" + Convert.ToDateTime(DailyAttendanceDR["date"]).ToString("yyyy-MM-dd") + "' ";

                        insertColumns += ", ShiftEnd ";
                        insertValues += ", '" + Convert.ToDateTime(DailyAttendanceDR["date"]).ToString("yyyy-MM-dd") + "' ";

                        insertColumns += ", InDatetime ";
                        insertValues += ", '" + Convert.ToDateTime(DailyAttendanceDR["date"]).ToString("yyyy-MM-dd") + "' ";

                        insertColumns += ", Intime ";
                        insertValues += ", '00:00:00' ";

                        insertColumns += ", OutDatetime ";
                        insertValues += ", '" + Convert.ToDateTime(DailyAttendanceDR["date"]).ToString("yyyy-MM-dd") + "' ";

                        insertColumns += ", Outtime ";
                        insertValues += ", '00:00:00' ";

                        insertColumns += ", shifttime ";
                        insertValues += ", '" + DailyAttendanceDR["shift_time"] + "'  ";

                        insertColumns += ", ShiftDatetime ";
                        insertValues += ", '" + Convert.ToDateTime(DailyAttendanceDR["date"]).ToString("yyyy-MM-dd") + "' ";

                        InsertUpdateQry = " INSERT INTO tbl_attendances_machine ( asm_id, sap_code, date " + insertColumns + ", proc, applied, span ) ";
                        InsertUpdateQry += " values ( " + DailyAttendanceDR["asm_id"] + ", " + DailyAttendanceDR["asm_id"] + ", '" + Convert.ToDateTime(DailyAttendanceDR["date"]).ToString("yyyy-MM-dd") + "' " + insertValues + ", 'None', 0, 0 )";

                        Console.Write(" {0}\n", InsertUpdateQry);


                        using (MySqlCommand command = new(InsertUpdateQry, conn))
                        {
                            if (command.ExecuteNonQuery() != 1)
                            {
                                // 'handled as needed, //' but this snippet will throw an exception to force a rollback
                                // throw new InvalidProgramException();
                            }
                        }
                    }



                    /* Console.Write(" {0}\n", DailyAttendanceDR["date"]);
                     Console.Write(" {0}\n", DailyAttendanceDR["sip"]);
                     Console.Write(" {0}\n", DailyAttendanceDR["asm_id"]);
                     Console.Write(" {0}\n", DailyAttendanceDR["title"]);
                     Console.Write(" {0}\n", DailyAttendanceDR["login"]);
                     Console.Write(" {0}\n", DailyAttendanceDR["logout"]);*/
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
                // oTransaction.Rollback();
                // throw;
            }
            conn.Close();
            return;

        }
        private DataSet GetDailyAttendanceQuery()
        {
            string DailySelectQry = "select `date`, `asm_id`,	`tbl_attendances_daily`.`status` AS title, IFNULL(`tbl_attendances_daily`.`shift`,'09:00') as shift_time" +
                ", `tbl_users`.`employee_id`, `tbl_users`.`name` " +
                " from tbl_attendances_daily join tbl_users on (`tbl_attendances_daily`.`asm_id` = `tbl_users`.`login`) " +
                " where `tbl_attendances_daily`.`date` >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' " +
                " and `tbl_attendances_daily`.`date` <= '" + localDate.ToString("yyyy-MM-dd") + "' " +
                " and ( LOWER(`tbl_attendances_daily`.`status`) = 'off' " +
                "   or LOWER(`tbl_attendances_daily`.`status`) = 'ta' " +
                "   or LOWER(`tbl_attendances_daily`.`status`) = 'tp' " +
                "   or LOWER(`tbl_attendances_daily`.`status`) = 'cv' " +
                "   or LOWER(`tbl_attendances_daily`.`status`) = 'ad' " +
                "   or LOWER(`tbl_attendances_daily`.`status`) = 'os' " +
                "   or LOWER(`tbl_attendances_daily`.`status`) = 'vl' " +
                "   or LOWER(`tbl_attendances_daily`.`status`) = 'oc' " +
                "   or LOWER(`tbl_attendances_daily`.`status`) = 'a' " +
                " ) ";

            Console.Write(" {0}\n", DailySelectQry);
            DataSet DailySelectDS = new();
            try
            {
                // Way 1
                // MySqlCommand DailySelectCmd = new MySqlCommand(DailySelectQry, conn);
                // DailySelectCmd.CommandType = CommandType.Text;
                // MySqlDataReader DailySelectRdr = DailySelectCmd.ExecuteReader();

                // Way 2
                // MySqlDataAdapter DailySelectDA = new MySqlDataAdapter(DailySelectQry, conn);
                // DailySelectDA.SelectCommand.CommandType = CommandType.Text;
                // DailySelectDA.Fill(DailySelectDS, "DailyAttendance");
                // Way 3

                using (MySqlCommand DailySelectCmd = new(DailySelectQry, conn))
                {
                    using (MySqlDataAdapter DailySelectDA = new(DailySelectCmd))
                    {
                        DailySelectDA.SelectCommand.CommandType = CommandType.Text;
                        // attenUsrEmpDA.Fill(DailySelectDT);
                        DailySelectDA.Fill(DailySelectDS, "DailyAttendance");
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
            }*/
            //CloseConnection();
            return DailySelectDS;
            // return DailySelectDT;
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
        ~AttendanceStatusDaily()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Daily Attendance..........................");
        }
    }
}
