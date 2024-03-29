﻿using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace AttendancesServices
{
    sealed class Attendance21To20 : Common, IDisposable
    {
        private DateTime Start, End, Start1, End1;
        private int dayz1, dayz;
        private readonly DateTime localDate;// = DateTime.Now;
        // private readonly DateTime localMonthDate = DateTime.Now.AddMonths(-2);
        private readonly MySqlConnection conn;
        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public Attendance21To20(DateTime To)
        {
            localDate = To;
            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Monthly 21To20 Attendance Connection working.");
            }
            else
            {
                Console.WriteLine("Monthly 21To20 Attendance Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In Monthly 21To20 Attendance..........................");
        }
        public void GetAttendance21To20()
        {
            /*DateTime Start4 = new DateTime(localDate.AddMonths(-4).Year, localDate.AddMonths(-4).Month, 21);
            DateTime End4 = new DateTime(localDate.AddMonths(-3).Year, localDate.AddMonths(-3).Month, 20);

            DateTime Start3 = new DateTime(localDate.AddMonths(-3).Year, localDate.AddMonths(-3).Month, 21);
            DateTime End3 = new DateTime(localDate.AddMonths(-2).Year, localDate.AddMonths(-2).Month, 20);

            LinkageMonthAttendance(Start4, End4);
            LinkageMonthAttendance(Start3, End3);*/


            Start1 = new DateTime(localDate.AddMonths(-2).Year, localDate.AddMonths(-2).Month, 21);
            End1 = new DateTime(localDate.AddMonths(-1).Year, localDate.AddMonths(-1).Month, 20);
            dayz1 = DateTime.DaysInMonth(Start1.Year, Start1.Month);
            LinkageMonthAttendance(Start1, End1, dayz1);

            Start = new DateTime(localDate.AddMonths(-1).Year, localDate.AddMonths(-1).Month, 21);
            End = new DateTime(localDate.Year, localDate.Month, 20);            
            dayz = DateTime.DaysInMonth(Start.Year, Start.Month);
            LinkageMonthAttendance(Start, End, dayz);
            
            if (localDate.Day > 21)
            {
                Start = new DateTime(localDate.Year, localDate.Month, 21);
                End = new DateTime(localDate.AddMonths(1).Year, localDate.AddMonths(1).Month, 20);
                dayz = DateTime.DaysInMonth(Start.Year, Start.Month);
                LinkageMonthAttendance(Start, End, dayz);
            }

            return;
        }
        private void LinkageMonthAttendance(DateTime M1, DateTime M2, int dyz)
        {
            DataSet attendanceDS = GetMonthWiseAgentsAttendance(M1, M2);

            foreach (DataRow campaignRow in attendanceDS.Tables["AttendanceMonth"].Rows)//campaignDS.Tables["Customers"].Rows
            {
                MonthlyInsertAndUpdate(campaignRow, M1, M2, dyz);
            }
        }
        private DataSet GetMonthWiseAgentsAttendance(DateTime M1, DateTime M2)//
        {
            // DateTime cardTime = Convert.ToDateTime(dateValue);
            // Dictionary<string, string> employeeUserDict = new ();
            string AttendanceMonthlyQry = " select asm_id, name " +
                ", count(*) cunt " +
                ", sum(case when type = 'P' then 1 else 0 end) as pres" +
                ", sum(case when type = 'A' then 1 else 0 end) as abse" +
                ", sum(case when type in ('L', 'HL', 'SL', 'CL', 'AL', 'ML', 'PL') then 1 else 0 end) as leve " +
                ", sum(case when type in ('O', 'WO') then 1 else 0 end) as off " +
                ", sum(case when type in ('T', 'TO') then 1 else 0 end) as tran " +
                ", sum(case when type in ('G', 'VO') then 1 else 0 end) as gazz " +
                ", sum(case when (type = 'P' and lesstime > 0) then 1 else 0 end) as lesstime " +
                ", sum(case when (type = 'P' and latearrival > 0) then 1 else 0 end) as latearrival " +
                ", sum(case when (type = 'P' and missed > 0) then 1 else 0 end) as missed " +
                " from tbl_attendances_machine " +
                " where date >= '" + M1.ToString("yyyy-MM-dd") + "' " +
                " and date <= '" + M2.ToString("yyyy-MM-dd") + "' " +
                " group by asm_id";

            Console.Write(" {0}\n", AttendanceMonthlyQry);
            DataSet attenUsrEmpDS = new();
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
                using (MySqlCommand attenUsrEmp = new(AttendanceMonthlyQry, conn))
                {
                    using (MySqlDataAdapter attenUsrEmpDA = new(attenUsrEmp))
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
        }
        private long MonthlyInsertAndUpdate(DataRow attenUsrEmpRdr, DateTime M1, DateTime M2, int dyz)
        {
            string InsertUpdateQry;
            string SelectCheck = "select count(*) from tbl_attendances_months " +
                " where asm_id =" + attenUsrEmpRdr["asm_id"] + " " +
                " and from_date = '" + M1.ToString("yyyy-MM-dd") + "' " +
                " and to_date = '" + M2.ToString("yyyy-MM-dd") + "'  " +
                " and dayz = 'sal' ";

            Console.Write(" {0}\n", SelectCheck);
            long count = 0;

            using (MySqlCommand SelectCheckCommand = new(SelectCheck, conn))
            {
                count = (long)SelectCheckCommand.ExecuteScalar();
            }
            if (count <= 0)
            {

                InsertUpdateQry = " INSERT INTO tbl_attendances_months ( asm_id, name, from_date, to_date, cunt, pres, abse, leve, off, tran, gazz, mon, yr, dayz, lesstime, latearrival, missed, total ) ";
                InsertUpdateQry += " values ( '" + attenUsrEmpRdr["asm_id"].ToString() + "' , '" + attenUsrEmpRdr["name"].ToString() + "' ,'" + M1.ToString("yyyy-MM-dd") + "' " +
                    ", '" + M2.ToString("yyyy-MM-dd") + "' " +
                    ", '" + attenUsrEmpRdr["cunt"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["pres"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["abse"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["leve"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["off"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["tran"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["gazz"].ToString() + "' " +
                    ", '" + M2.Month + "'" +
                    ", '" + M2.Year + "'" +
                    ", 'sal' " +
                    ", '" + attenUsrEmpRdr["lesstime"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["latearrival"].ToString() + "' " +
                    ", '" + attenUsrEmpRdr["missed"].ToString() + "'" +
                    ", '" + dyz + "' )";
            }
            else
            {
                /*employeeUserDict["asm_id"] = attenUsrEmpRdr["asm_id"].ToString();
                employeeUserDict["yr"] = attenUsrEmpRdr["yr"].ToString();
                employeeUserDict["mon"] = attenUsrEmpRdr["mon"].ToString()*/
                ;
                /*employeeUserDict["cunt"] = attenUsrEmpRdr["cunt"].ToString();
                employeeUserDict["pres"] = attenUsrEmpRdr["pres"].ToString();
                employeeUserDict["abse"] = attenUsrEmpRdr["abse"].ToString();
                employeeUserDict["leve"] = attenUsrEmpRdr["leve"].ToString();
                employeeUserDict["off"] = attenUsrEmpRdr["off"].ToString();
                employeeUserDict["tran"] = attenUsrEmpRdr["tran"].ToString();
                employeeUserDict["gazz"] = attenUsrEmpRdr["gazz"].ToString();*/

                InsertUpdateQry = " UPDATE tbl_attendances_months set " +
                    " name = '" + attenUsrEmpRdr["name"].ToString() + "' " +
                    ", cunt = '" + attenUsrEmpRdr["cunt"].ToString() + "' " +
                    ", pres = '" + attenUsrEmpRdr["pres"].ToString() + "' " +
                    ", abse = '" + attenUsrEmpRdr["abse"].ToString() + "' " +
                    ", leve = '" + attenUsrEmpRdr["leve"].ToString() + "' " +
                    ", off = '" + attenUsrEmpRdr["off"].ToString() + "' " +
                    ", tran = '" + attenUsrEmpRdr["tran"].ToString() + "' " +
                    ", gazz = '" + attenUsrEmpRdr["gazz"].ToString() + "' " +
                    ", mon = '" + M2.Month + "' " +
                    ", yr = '" + M2.Year + "' " +
                    ", total = '" + dyz + "' " +
                    ", lesstime = '" + attenUsrEmpRdr["lesstime"].ToString() + "' " +
                    ", latearrival = '" + attenUsrEmpRdr["latearrival"].ToString() + "' " +
                    ", missed = '" + attenUsrEmpRdr["missed"].ToString() + "' ";

                InsertUpdateQry += " where asm_id = " + attenUsrEmpRdr["asm_id"] +
                    " and from_date = '" + M1.ToString("yyyy-MM-dd") + "' " +
                    " and to_date = '" + M2.ToString("yyyy-MM-dd") + "' " +
                    " and dayz = 'sal' "; // + " and sap_code = " + item.Key + "";

            }

            Console.Write(" {0}\n", InsertUpdateQry);

            using (MySqlCommand command = new(InsertUpdateQry, conn))
            {
                count = command.ExecuteNonQuery();
            }
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
        ~Attendance21To20()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Monthly 21To20 Attendance ..........................");
        }
    }
}
