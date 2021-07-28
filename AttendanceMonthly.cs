using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    sealed class AttendanceMonthly : Common, IDisposable
    {
        private DateTime Start, End, Start1, End1, Start2, End2;
        private readonly DateTime localDate = DateTime.Now;
        private readonly DateTime localMonthDate = DateTime.Now.AddMonths(-2);
        private MySqlConnection conn;
        // private DateTime start = new DateTime((int)localDate.Year, (int)LocalDate.Months, 1);

        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public AttendanceMonthly()
        {
            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Monthly Attendance Connection working.");
            }
            else
            {
                Console.WriteLine("Monthly Attendance, Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In Monthly Attendance..........................");
        }
        public void GetAttendanceMonthApproval()
        {
            /*DateTime Start4 = new DateTime(localDate.AddMonths(-4).Year, localDate.AddMonths(-4).Month, 1);
            DateTime End4 = new DateTime(localDate.AddMonths(-4).Year, localDate.AddMonths(-4).Month, DateTime.DaysInMonth(localDate.AddMonths(-4).Year, localDate.AddMonths(-4).Month));
            LinkageMonthAttendance(Start4, End4);

            DateTime Start3 = new DateTime(localDate.AddMonths(-3).Year, localDate.AddMonths(-3).Month, 1);
            DateTime End3 = new DateTime(localDate.AddMonths(-3).Year, localDate.AddMonths(-3).Month, DateTime.DaysInMonth(localDate.AddMonths(-3).Year, localDate.AddMonths(-3).Month));
            LinkageMonthAttendance(Start3, End3);*/

            Start2 = new DateTime(localDate.AddMonths(-2).Year, localDate.AddMonths(-2).Month, 1);
            End2 = new DateTime(localDate.AddMonths(-2).Year, localDate.AddMonths(-2).Month, DateTime.DaysInMonth(localDate.AddMonths(-2).Year, localDate.AddMonths(-2).Month));
            LinkageMonthAttendance(Start2, End2);

            Start1 = new DateTime(localDate.AddMonths(-1).Year, localDate.AddMonths(-1).Month, 1);
            End1 = new DateTime(localDate.AddMonths(-1).Year, localDate.AddMonths(-1).Month, DateTime.DaysInMonth(localDate.AddMonths(-1).Year, localDate.AddMonths(-1).Month));
            LinkageMonthAttendance(Start1, End1);

            Start = new DateTime(localDate.Year, localDate.Month, 1);
            End = DateTime.Now;
            LinkageMonthAttendance(Start, End);
            // return;
        }
        private void LinkageMonthAttendance(DateTime M1, DateTime M2)
        {
            DataSet attendanceDS =  GetMonthWiseAgentsAttendance(M1, M2);
            
            foreach (DataRow campaignRow in attendanceDS.Tables["AttendanceMonth"].Rows)//campaignDS.Tables["Customers"].Rows
            {
                MonthlyInsertAndUpdate(campaignRow, M1, M2);
            }
        }
        private DataSet GetMonthWiseAgentsAttendance(DateTime M1, DateTime M2)//
        {
            // DateTime cardTime = Convert.ToDateTime(dateValue);
            Dictionary<string, string> employeeUserDict = new Dictionary<string, string>();
            string AttendanceMonthlyQry = " select asm_id, year(date) as yr, month(date) as mon" +
                ", count(*) cunt, name " +
                ", sum(case when type = 'P' then 1 else 0 end) as pres " +
                ", sum(case when type = 'A' then 1 else 0 end) as abse " +
                ", sum(case when type in ('L', 'HL', 'SL', 'CL', 'AL', 'ML', 'PL') then 1 else 0 end) as leve " +
                ", sum(case when type in ('O', 'WO') then 1 else 0 end) as off " +
                ", sum(case when type in ('T', 'TO') then 1 else 0 end) as tran " +
                ", sum(case when type in ('G', 'VO') then 1 else 0 end) as gazz " +
                ", sum(case when (type = 'P' and lesstime > 0) then 1 else 0 end) as lesstime " +
                ", sum(case when (type = 'P' and latearrival > 0) then 1 else 0 end) as latearrival " +
                ", sum(case when (type = 'P' and missed > 0) then 1 else 0 end) as missed " +
                " from tbl_attendances_machine " +
                " where date >= '" + M1.ToString("yyyy-MM-dd") + "' and date <= '" + M2.ToString("yyyy-MM-dd") + "' " +
                " group by asm_id, year(date), month(date)";

            Console.Write(" {0}\n", AttendanceMonthlyQry);
            DataSet attenUsrEmpDS = new DataSet();
            try
            {
                // Way 1
                // MySqlCommand attenUsrEmp = new MySqlCommand(attenUsrEmpQry, conn);
                // attenUsrEmp.CommandType = CommandType.Text;
                // MySqlDataReader attenUsrEmpRdr = attenUsrEmp.ExecuteReader();

                // Way 2
                // MySqlDataAdapter attenUsrEmpDA = new MySqlDataAdapter(attenUsrEmpQry, conn);
                // attenUsrEmpDA.SelectCommand.CommandType = CommandType.Text;
                // attenUsrEmpDA.Fill(attenUsrEmpDS, "Attendance");

                // Way 3
                using (MySqlCommand attenUsrEmp = new MySqlCommand(AttendanceMonthlyQry, conn))
                {
                    using (MySqlDataAdapter attenUsrEmpDA = new MySqlDataAdapter(attenUsrEmp))
                    {
                        attenUsrEmpDA.SelectCommand.CommandType = CommandType.Text;
                        attenUsrEmpDA.Fill(attenUsrEmpDS, "AttendanceMonth");
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }
            
            return attenUsrEmpDS;


          /*  using (MySqlCommand employeeUserCmd = new MySqlCommand(AttendanceMonthlyQry, conn))
            {
                employeeUserCmd.CommandType = CommandType.Text;
                using (MySqlDataReader attenUsrEmpRdr = employeeUserCmd.ExecuteReader())
                {
                    if (attenUsrEmpRdr.HasRows)
                    {
                        while (attenUsrEmpRdr.Read())
                        {
                            
                           *//* employeeUserDict["asm_id"] = attenUsrEmpRdr["asm_id"].ToString();
                            employeeUserDict["yr"] = attenUsrEmpRdr["yr"].ToString();
                            employeeUserDict["mon"] = attenUsrEmpRdr["mon"].ToString();
                            employeeUserDict["cunt"] = attenUsrEmpRdr["cunt"].ToString();
                            employeeUserDict["pres"] = attenUsrEmpRdr["pres"].ToString();
                            employeeUserDict["abse"] = attenUsrEmpRdr["abse"].ToString();
                            employeeUserDict["leve"] = attenUsrEmpRdr["leve"].ToString();
                            employeeUserDict["off"] = attenUsrEmpRdr["off"].ToString();
                            employeeUserDict["tran"] = attenUsrEmpRdr["tran"].ToString();
                            employeeUserDict["gazz"] = attenUsrEmpRdr["gazz"].ToString();*//*

                            MonthlyInsertAndUpdate(attenUsrEmpRdr);

                            *//*String slug = attenUsrEmpRdr["slug"].ToString();
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
                            }*//*
                        }
                    }
                    *//*else
                    {
                        employeeUserDict["weeklyOff"] = "single";
                        employeeUserDict["working_hour"] = "08:00";
                        employeeUserDict["working_hour_number"] = "8.0";
                    }*//*
                }

            }
            // CloseConnection();
            return employeeUserDict;*/
        }
        private long MonthlyInsertAndUpdate(DataRow attenUsrEmpRdr, DateTime M1, DateTime M2)
        {
            string InsertUpdateQry;
            string SelectCheck = "select count(*) from tbl_attendances_months where asm_id =" + attenUsrEmpRdr["asm_id"] + " and yr = " + attenUsrEmpRdr["yr"] + " and mon = " + attenUsrEmpRdr["mon"] + " and dayz = 'kpi' ";

            Console.Write(" {0}\n", SelectCheck);
            long count = 0;

            using (MySqlCommand SelectCheckCommand = new MySqlCommand(SelectCheck, conn))
            {
                count = (long)SelectCheckCommand.ExecuteScalar();
            }
            if (count <= 0)
            {

                InsertUpdateQry = " INSERT INTO tbl_attendances_months ( asm_id, name, yr, mon, cunt, pres, abse, leve, off, tran, gazz, from_date, to_date, dayz, lesstime, latearrival, missed ) ";
                InsertUpdateQry += " values ( '"+ attenUsrEmpRdr["asm_id"].ToString() + "' , '" + attenUsrEmpRdr["name"].ToString() + "' ,'" + attenUsrEmpRdr["yr"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["mon"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["cunt"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["pres"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["abse"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["leve"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["off"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["tran"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["gazz"].ToString() + "' " +
                    ", '" + M1.ToString("yyyy-MM-dd") + "' "+
                    ", '" + M2.ToString("yyyy-MM-dd") + "' " +
                    ", 'kpi' "+
                    ", '" + attenUsrEmpRdr["lesstime"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["latearrival"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["missed"].ToString() + "' )";
            }
            else
            {
                /*employeeUserDict["asm_id"] = attenUsrEmpRdr["asm_id"].ToString();
                employeeUserDict["yr"] = attenUsrEmpRdr["yr"].ToString();
                employeeUserDict["mon"] = attenUsrEmpRdr["mon"].ToString()*/;
                /*employeeUserDict["cunt"] = attenUsrEmpRdr["cunt"].ToString();
                employeeUserDict["pres"] = attenUsrEmpRdr["pres"].ToString();
                employeeUserDict["abse"] = attenUsrEmpRdr["abse"].ToString();
                employeeUserDict["leve"] = attenUsrEmpRdr["leve"].ToString();
                employeeUserDict["off"] = attenUsrEmpRdr["off"].ToString();
                employeeUserDict["tran"] = attenUsrEmpRdr["tran"].ToString();
                employeeUserDict["gazz"] = attenUsrEmpRdr["gazz"].ToString();*/

                InsertUpdateQry = " UPDATE tbl_attendances_months set " +
                    " name = '" + attenUsrEmpRdr["name"].ToString()+"' " +
                    ", cunt = '" + attenUsrEmpRdr["cunt"].ToString() + "' " +
                    ", pres = '" + attenUsrEmpRdr["pres"].ToString()+"' " +
                    ", abse = '" + attenUsrEmpRdr["abse"].ToString() + "' " +
                    ", leve = '" + attenUsrEmpRdr["leve"].ToString() + "' " +
                    ", off = '" + attenUsrEmpRdr["off"].ToString() + "' " +
                    ", tran = '" + attenUsrEmpRdr["tran"].ToString() + "' " +
                    ", gazz = '" + attenUsrEmpRdr["gazz"].ToString() + "' " +
                    ", from_date = '" + M1.ToString("yyyy-MM-dd") + "' " +
                    ", to_date = '" + M2.ToString("yyyy-MM-dd") + "' " +
                    ", lesstime = '" + attenUsrEmpRdr["lesstime"].ToString() + "' " +
                    ", latearrival = '" + attenUsrEmpRdr["latearrival"].ToString() + "' " +
                    ", missed = '" + attenUsrEmpRdr["missed"].ToString() + "' ";

                InsertUpdateQry += " where asm_id = " + attenUsrEmpRdr["asm_id"] + 
                    " and yr = '" + attenUsrEmpRdr["yr"] + "' " +
                    " and mon = '"+ attenUsrEmpRdr["mon"] +"' " +
                    " and dayz = 'kpi' "; // + " and sap_code = " + item.Key + "";

            }
            
            Console.Write(" {0}\n", InsertUpdateQry);

            using (MySqlCommand command = new MySqlCommand(InsertUpdateQry, conn))
            {
                count = command.ExecuteNonQuery();
            }
            return count;
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
        ~AttendanceMonthly()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Monthly Attendance ..........................");
        }
    }
}
