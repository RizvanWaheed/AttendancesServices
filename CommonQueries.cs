using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AttendancesServices
{
    class CommonQueries : Common
    {
        string SelectCheck;
        public long AttendanceAsmidAndYearNadMonthAndDayWiseExist(MySqlConnection conn, string AsmID, int year, int month, string type, int dayofweek, long count)
        {
            // string subQry;


            SelectCheck = " select count(*) from tbl_attendances_machine " +
                " where asm_id = " + AsmID + " " +
                " and YEAR(date) = '" + year + "' " +
                " and MONTH(date) = '" + month + "' " +
                " and type = '" + type + "' " +
                " and WEEKDAY(date) = '" + dayofweek + "'";

            Console.Write(" {0}\n", SelectCheck);
            using (MySqlCommand SelectCheckCommand = new(SelectCheck, conn))
            {
                count = (long)SelectCheckCommand.ExecuteScalar();
            }
            return count;
        }
        public long AttendanceAsmidAndDateWiseExist(MySqlConnection conn, string AsmID, string DateA, long count)
        {
            SelectCheck = "select count(*) from tbl_attendances_machine where asm_id = " + AsmID + " and date = '" + Convert.ToDateTime(DateA).ToString("yyyy-MM-dd") + "'";

            Console.Write(" {0}\n", SelectCheck);
            using (MySqlCommand SelectCheckCommand = new(SelectCheck, conn))
            {
                count = (long)SelectCheckCommand.ExecuteScalar();
            }
            return count;
        }
        public static long AttendaceInsertOrUpdate(MySqlConnection conn, string Qry, long count)
        {

            using (MySqlCommand command = new(Qry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
        }
        public static long AttendaceInsertUpdate(MySqlConnection conn, string Qry, long count)
        {
            using (MySqlCommand command = new(Qry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
        }
        public static long AttendaceInsert(MySqlConnection conn, Dictionary<string, string> dictInner, long count)
        {
            string InsertUpdateQry;
            string insertColumns = " asm_id, date  ";
            string insertValues = " " + dictInner["asm_id"] + ", '" + Convert.ToDateTime(dictInner["date"]).ToString("yyyy-MM-dd") + "' ";

            if (!string.IsNullOrEmpty(dictInner["sap_code"]))
            {
                insertColumns += ", sap_code ";
                insertValues += ", '" + dictInner["sap_code"] + "' ";
            }
            if (!string.IsNullOrEmpty(dictInner["employee_id"]))
            {
                insertColumns += ", employee_id ";
                insertValues += ", '" + dictInner["employee_id"] + "' ";
            }
            if (!string.IsNullOrEmpty(dictInner["name"]))
            {
                insertColumns += ", name ";
                insertValues += ", '" + dictInner["name"] + "' ";
            }
            if (!string.IsNullOrEmpty(dictInner["title"]))
            {
                insertColumns += ", title ";
                insertValues += ", '" + dictInner["title"] + "' ";
            }
            if (dictInner.ContainsKey("sub_title") && !string.IsNullOrEmpty(dictInner["sub_title"]))
            {
                insertColumns += ", sub_title ";
                insertValues += ", '" + dictInner["sub_title"] + "' ";
            }
            if (dictInner.ContainsKey("InDatetime") && !string.IsNullOrEmpty(dictInner["InDatetime"]))
            {
                insertColumns += ", InDatetime ";
                insertValues += ", '" + dictInner["InDatetime"] + "' ";
            }
            if (dictInner.ContainsKey("Intime") && !string.IsNullOrEmpty(dictInner["Intime"]))
            {
                insertColumns += ", Intime ";
                insertValues += ", '" + dictInner["Intime"] + "' ";
            }
            if (dictInner.ContainsKey("OutDatetime") && !string.IsNullOrEmpty(dictInner["OutDatetime"]))
            {
                insertColumns += ", OutDatetime ";
                insertValues += ", '" + dictInner["OutDatetime"] + "' ";
            }
            if (dictInner.ContainsKey("Outtime") && !string.IsNullOrEmpty(dictInner["Outtime"]))
            {
                insertColumns += ", Outtime ";
                insertValues += ", '" + dictInner["Outtime"] + "' ";
            }
            if (dictInner.ContainsKey("Shifttime") && !string.IsNullOrEmpty(dictInner["Shifttime"]))
            {
                insertColumns += ", shifttime ";
                insertValues += ", '" + dictInner["Shifttime"] + "' ";
            }
            if (dictInner.ContainsKey("lesstime") && !string.IsNullOrEmpty(dictInner["lesstime"]))
            {
                insertColumns += ", lesstime ";
                insertValues += ", '" + dictInner["lesstime"] + "' ";
            }
            if (dictInner.ContainsKey("latearrival") && !string.IsNullOrEmpty(dictInner["latearrival"]))
            {
                insertColumns += ", latearrival ";
                insertValues += ", '" + dictInner["latearrival"] + "' ";
            }
            if (dictInner.ContainsKey("missed") && !string.IsNullOrEmpty(dictInner["missed"]))
            {
                insertColumns += ", missed ";
                insertValues += ", '" + dictInner["missed"] + "' ";
            }
            if (dictInner.ContainsKey("color") && !string.IsNullOrEmpty(dictInner["color"]))
            {
                insertColumns += ", color ";
                insertValues += ", '" + dictInner["color"] + "' ";
            }
            if (dictInner.ContainsKey("Intimefound") && !string.IsNullOrEmpty(dictInner["Intimefound"]))
            {
                insertColumns += ", Intimefound ";
                insertValues += ", '" + dictInner["Intimefound"] + "' ";
            }
            if (dictInner.ContainsKey("Outtimefound") && !string.IsNullOrEmpty(dictInner["Outtimefound"]))
            {
                insertColumns += ", Outtimefound ";
                insertValues += ", '" + dictInner["Outtimefound"] + "' ";
            }
            if (dictInner.ContainsKey("Shifttimefound") && !string.IsNullOrEmpty(dictInner["Shifttimefound"]))
            {
                insertColumns += ", Shifttimefound ";
                insertValues += ", '" + dictInner["Shifttimefound"] + "' ";
            }
            if (dictInner.ContainsKey("ShiftDatetime") && !string.IsNullOrEmpty(dictInner["ShiftDatetime"]))
            {
                insertColumns += ", ShiftDatetime ";
                insertValues += ", '" + dictInner["ShiftDatetime"] + "' ";
            }
            if (dictInner.ContainsKey("ShiftStart") && !string.IsNullOrEmpty(dictInner["ShiftStart"]))
            {
                insertColumns += ", ShiftStart ";
                insertValues += ", '" + dictInner["ShiftStart"] + "' ";
            }
            if (dictInner.ContainsKey("ShiftEnd") && !string.IsNullOrEmpty(dictInner["ShiftEnd"]))
            {
                insertColumns += ", ShiftEnd ";
                insertValues += ", '" + dictInner["ShiftEnd"] + "' ";
            }
            if (dictInner.ContainsKey("difference") && !string.IsNullOrEmpty(dictInner["difference"]))
            {
                insertColumns += ", difference ";
                insertValues += ", '" + dictInner["difference"] + "' ";
            }

            if (dictInner.ContainsKey("proc") && !string.IsNullOrEmpty(dictInner["proc"]))
            {
                insertColumns += ", proc ";
                insertValues += ", '" + dictInner["proc"] + "' ";
            }
            if (dictInner.ContainsKey("applied") && !string.IsNullOrEmpty(dictInner["applied"]))
            {
                insertColumns += ", applied ";
                insertValues += ", '" + dictInner["applied"] + "' ";
            }
            if (dictInner.ContainsKey("span") && !string.IsNullOrEmpty(dictInner["span"]))
            {
                insertColumns += ", span ";
                insertValues += ", '" + dictInner["span"] + "' ";
            }
            if (dictInner.ContainsKey("type") && !string.IsNullOrEmpty(dictInner["type"]))
            {
                insertColumns += ", type ";
                insertValues += ", '" + dictInner["type"] + "' ";
            }

            InsertUpdateQry = " INSERT INTO tbl_attendances_machine ( " + insertColumns + " ) ";
            InsertUpdateQry += " values ( " + insertValues + " )";

            Console.Write(" {0}\n", InsertUpdateQry);

            using (MySqlCommand command = new(InsertUpdateQry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
        }
        public static long AttendaceUpdate(MySqlConnection conn, Dictionary<string, string> dictInner, long count)
        {
            string updateColumns = " type = '" + dictInner["type"] + "' ";
            string InsertUpdateQry;

            if (!string.IsNullOrEmpty(dictInner["proc"]))
            {
                updateColumns += ", proc = '" + dictInner["proc"] + "' ";
            }
            if (!string.IsNullOrEmpty(dictInner["name"]))
            {
                updateColumns += ", name = '" + dictInner["name"] + "' ";
            }
            if (dictInner.ContainsKey("sub_title") && !string.IsNullOrEmpty(dictInner["sub_title"]))
            {
                updateColumns += ", sub_title = '" + dictInner["sub_title"] + "' ";
            }
            if (!string.IsNullOrEmpty(dictInner["applied"]))
            {
                updateColumns += ", applied = '" + dictInner["applied"] + "' ";
            }
            if (!string.IsNullOrEmpty(dictInner["span"]))
            {
                updateColumns += ", span = '" + dictInner["span"] + "' ";
            }
            if (!string.IsNullOrEmpty(dictInner["employee_id"]))
            {
                updateColumns += ", employee_id = '" + dictInner["employee_id"] + "' ";
            }
            if (dictInner.ContainsKey("title") && !string.IsNullOrEmpty(dictInner["title"]))
            {
                updateColumns += ", title = '" + dictInner["title"] + "' ";
            }
            if (dictInner.ContainsKey("InDatetime") && !string.IsNullOrEmpty(dictInner["InDatetime"]))
            {
                updateColumns += ", InDatetime = '" + dictInner["InDatetime"] + "' ";
            }
            if (dictInner.ContainsKey("Intime") && !string.IsNullOrEmpty(dictInner["Intime"]))
            {
                updateColumns += ", Intime = '" + dictInner["Intime"] + "' ";
            }
            if (dictInner.ContainsKey("OutDatetime") && !string.IsNullOrEmpty(dictInner["OutDatetime"]))
            {
                updateColumns += ", OutDatetime = '" + dictInner["OutDatetime"] + "' ";
            }
            if (dictInner.ContainsKey("Outtime") && !string.IsNullOrEmpty(dictInner["Outtime"]))
            {
                updateColumns += ", Outtime = '" + dictInner["Outtime"] + "' ";
            }
            if (dictInner.ContainsKey("Shifttime") && !string.IsNullOrEmpty(dictInner["Shifttime"]))
            {
                updateColumns += ", shifttime = '" + dictInner["Shifttime"] + "' ";
            }
            if (dictInner.ContainsKey("lesstime") && !string.IsNullOrEmpty(dictInner["lesstime"]))
            {
                updateColumns += ", lesstime = '" + dictInner["lesstime"] + "' ";
            }
            if (dictInner.ContainsKey("latearrival") && !string.IsNullOrEmpty(dictInner["latearrival"]))
            {
                updateColumns += ", latearrival = '" + dictInner["latearrival"] + "' ";
            }
            if (dictInner.ContainsKey("missed") && !string.IsNullOrEmpty(dictInner["missed"]))
            {
                updateColumns += ", missed = '" + dictInner["missed"] + "' ";
            }
            if (dictInner.ContainsKey("color") && !string.IsNullOrEmpty(dictInner["color"]))
            {
                updateColumns += ", color = '" + dictInner["color"] + "' ";
            }
            if (dictInner.ContainsKey("Intimefound") && !string.IsNullOrEmpty(dictInner["Intimefound"]))
            {
                updateColumns += ", Intimefound = '" + dictInner["Intimefound"] + "' ";
            }
            if (dictInner.ContainsKey("Outtimefound") && !string.IsNullOrEmpty(dictInner["Outtimefound"]))
            {
                updateColumns += ", Outtimefound = '" + dictInner["Outtimefound"] + "' ";
            }
            if (dictInner.ContainsKey("Shifttimefound") && !string.IsNullOrEmpty(dictInner["Shifttimefound"]))
            {
                updateColumns += ", Shifttimefound = '" + dictInner["Shifttimefound"] + "' ";
            }
            if (dictInner.ContainsKey("ShiftDatetime") && !string.IsNullOrEmpty(dictInner["ShiftDatetime"]))
            {
                updateColumns += ", ShiftDatetime = '" + dictInner["ShiftDatetime"] + "' ";
            }
            if (dictInner.ContainsKey("ShiftStart") && !string.IsNullOrEmpty(dictInner["ShiftStart"]))
            {
                updateColumns += ", ShiftStart = '" + dictInner["ShiftStart"] + "' ";
            }
            if (dictInner.ContainsKey("ShiftEnd") && !string.IsNullOrEmpty(dictInner["ShiftEnd"]))
            {
                updateColumns += ", ShiftEnd = '" + dictInner["ShiftEnd"] + "' ";
            }
            if (dictInner.ContainsKey("difference") && !string.IsNullOrEmpty(dictInner["difference"]))
            {
                updateColumns += ", difference = '" + dictInner["difference"] + "' ";
            }
            InsertUpdateQry = " UPDATE tbl_attendances_machine set " + updateColumns;
            InsertUpdateQry += " where asm_id = " + dictInner["asm_id"] + " and date = '" + Convert.ToDateTime(dictInner["date"]).ToString("yyyy-MM-dd") + "'"; // + " and sap_code = " + item.Key + "";

            Console.Write(" {0}\n", InsertUpdateQry);

            using (MySqlCommand command = new(InsertUpdateQry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
        }



        protected static Dictionary<string, string> GetUserEmployee(string code, MySqlConnection conn)
        {
            // DateTime cardTime = Convert.ToDateTime(dateValue);

            string employyeeUserQry = "SELECT `tbl_users`.`role_id`, `tbl_users`.`login`, `tbl_employees`.`id`, `tbl_employees`.`sap_code`, `tbl_employees`.`full_name`" +
                " , `tbl_employees`.`last_working_date`, GROUP_CONCAT(`tbl_setup_setups`.`sub_setup_id`) as slugid, GROUP_CONCAT(`tbl_setups`.`slug` SEPARATOR '') as slug ";
            employyeeUserQry += " from tbl_employees ";
            employyeeUserQry += " INNER JOIN `tbl_users` ON `tbl_users`.`employee_id` = `tbl_employees`.`id`";
            employyeeUserQry += " LEFT JOIN `tbl_setup_setups` ON (`tbl_setup_setups`.`setup_id` = `tbl_users`.`role_id` and `tbl_setup_setups`.`type` =  'permissions_roles') ";
            employyeeUserQry += " LEFT JOIN `tbl_setups` ON (`tbl_setups`.`id` = `tbl_setup_setups`.`sub_setup_id`) ";
            employyeeUserQry += " where tbl_employees.status = 1 " +
                " and (tbl_employees.sap_code = " + code + " or tbl_users.login = " + code + " )" +
                " ORDER BY tbl_users.id desc, tbl_employees.id desc limit 1 ";


            /*employyeeUserQry += " INNER JOIN `tbl_setup_settings` ON (`tbl_setup_settings`.`setup_id` = `tbl_employees`.`employment_type_id` " +
                                                              " and `tbl_setup_settings`.`sub_setup_id` in (`tbl_employees`.`employment_type_id`) " +
                                                              " and `tbl_setup_settings`.`type` = 'contract_campaigns'" +
                                                              " and `tbl_setup_settings`.`field` = 'business_year' )";*/



            Console.Write(" {0}\n", employyeeUserQry);
            Dictionary<string, string> employeeUserDict = new();
            // OpenConection();
            using (MySqlCommand employeeUserCmd = new(employyeeUserQry, conn))
            {
                // MySqlCommand employeeUserCmd = new MySqlCommand(employyeeUserQry, conn);
                employeeUserCmd.CommandType = CommandType.Text;
                using (MySqlDataReader attenUsrEmpRdr = employeeUserCmd.ExecuteReader())
                {
                    if (attenUsrEmpRdr.HasRows)
                    {
                        while (attenUsrEmpRdr.Read())
                        {
                            if (string.IsNullOrEmpty(attenUsrEmpRdr["login"].ToString()))
                            {
                                continue;
                            }
                            Console.WriteLine(string.Format("role_id = {0}", attenUsrEmpRdr["role_id"].ToString()));
                            Console.WriteLine(string.Format("login = {0}", attenUsrEmpRdr["login"].ToString()));
                            Console.WriteLine(string.Format("sap_code = {0}", attenUsrEmpRdr["sap_code"].ToString()));
                            Console.WriteLine(string.Format("slug = {0}", attenUsrEmpRdr["slug"].ToString()));
                            Console.WriteLine(string.Format("full_name = {0}", attenUsrEmpRdr["full_name"].ToString()));

                            employeeUserDict["employee_id"] = attenUsrEmpRdr["id"].ToString();
                            employeeUserDict["role_id"] = attenUsrEmpRdr["role_id"].ToString();
                            employeeUserDict["asm_id"] = attenUsrEmpRdr["login"].ToString();
                            employeeUserDict["sap_code"] = attenUsrEmpRdr["sap_code"].ToString();
                            employeeUserDict["slug"] = attenUsrEmpRdr["slug"].ToString();
                            employeeUserDict["name"] = attenUsrEmpRdr["full_name"].ToString();
                            employeeUserDict["last_working_date"] = attenUsrEmpRdr["last_working_date"].ToString();

                            String slug = attenUsrEmpRdr["slug"].ToString();
                            if (slug.Contains("DUAL"))
                            {
                                employeeUserDict["weeklyOff"] = "dual";
                                employeeUserDict["working_hour"] = "09:00";
                                employeeUserDict["working_hour_number"] = "9.0";
                            }
                            else if (slug.Contains("ALTER"))
                            {
                                employeeUserDict["weeklyOff"] = "alter";
                                employeeUserDict["working_hour"] = "08:30";
                                employeeUserDict["working_hour_number"] = "8.5";
                            }
                            else
                            {
                                employeeUserDict["weeklyOff"] = "single";
                                employeeUserDict["working_hour"] = "08:00";
                                employeeUserDict["working_hour_number"] = "8.0";
                            }
                        }
                    }

                }
                // CloseConnection();
            }
            return employeeUserDict;

        }

    }
}
