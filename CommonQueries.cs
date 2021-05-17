using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    class CommonQueries: Common
    {
        string SelectCheck;
        public long AttendanceAsmidAndYearNadMonthAndDayWiseExist(MySqlConnection conn, string AsmID, int year, int month, string type, int dayofweek, long count)
        {
            // string subQry;


            SelectCheck = " select count(*) from tbl_attendances_machine " +
                " where asm_id = " + AsmID + " " +
                " and YEAR(date) = '"+ year + "' " +
                " and MONTH(date) = '" + month + "' " +
                " and type = '"+ type +"' " +
                " and WEEKDAY(date) = '"+ dayofweek + "'";

            Console.Write(" {0}\n", SelectCheck);
            using (MySqlCommand SelectCheckCommand = new MySqlCommand(SelectCheck, conn))
            {
                count = (long)SelectCheckCommand.ExecuteScalar();
            }
            return count;
        }
        public long AttendanceAsmidAndDateWiseExist(MySqlConnection conn, string AsmID, string DateA, long count)
        {
            SelectCheck = "select count(*) from tbl_attendances_machine where asm_id = " + AsmID + " and date = '" + Convert.ToDateTime(DateA).ToString("yyyy-MM-dd") + "'";

            Console.Write(" {0}\n", SelectCheck);
            using (MySqlCommand SelectCheckCommand = new MySqlCommand(SelectCheck, conn))
            {
                count = (long)SelectCheckCommand.ExecuteScalar();
            }
            return count;
        }
        public long AttendaceInsertOrUpdate(MySqlConnection conn, string Qry, long count)
        {

            using (MySqlCommand command = new MySqlCommand(Qry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
        }
        public long AttendaceInsertUpdate(MySqlConnection conn, string Qry, long count)
        {
            using (MySqlCommand command = new MySqlCommand(Qry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
        }
        public long AttendaceInsert(MySqlConnection conn, Dictionary<string,string> dictInner, long count)
        {
            string InsertUpdateQry = string.Empty;
            string insertColumns = " asm_id, date  ";
            string insertValues = " "+ dictInner["asm_id"] + ", '" + Convert.ToDateTime(dictInner["date"]).ToString("yyyy-MM-dd") + "' ";

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
            if (dictInner.ContainsKey("sub_title") &&  !string.IsNullOrEmpty(dictInner["sub_title"]))
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

            using (MySqlCommand command = new MySqlCommand(InsertUpdateQry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
        }
        public long AttendaceUpdate(MySqlConnection conn, Dictionary<string, string> dictInner, long count)
        {
            string updateColumns = " type = '" + dictInner["type"] + "' ";
            string InsertUpdateQry = string.Empty;

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
            if (dictInner.ContainsKey("title") &&  !string.IsNullOrEmpty(dictInner["title"]))
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

            using (MySqlCommand command = new MySqlCommand(InsertUpdateQry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
        }

    }
}
