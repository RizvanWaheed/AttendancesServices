using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancesServices
{
    sealed class KpiAttendance: CommonQueries, IDisposable
    {
        private DateTime localDate;
        private DateTime localMonthDate;
        private MySqlConnection conn;
        // private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;
        public KpiAttendance(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;

            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("KPI Connection working.");
            }
            else
            {
                Console.WriteLine("KPI Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In KPI Attendance..........................");
        }
        public void GetKpiAttendance()
        {
            DataSet KpiAttendanceDS = GetKpiAttendanceQuery();
            try
            {
                // OpenConection();
                Dictionary<string, string> IUvalues = new Dictionary<string, string>();
                foreach (DataRow KpiAttendanceDR in KpiAttendanceDS.Tables["KpiAttendance"].Rows)
                {
                    long count = 0;
                    count = AttendanceAsmidAndDateWiseExist(conn, KpiAttendanceDR["asm_id"].ToString(), KpiAttendanceDR["date"].ToString(), count);
                   
                    TimeSpan actualHours = new TimeSpan(0, 8, 0, 0);
                    int result = TimeSpan.Compare(actualHours, (TimeSpan)KpiAttendanceDR["title"]);

                    if (result > 0)
                    {
                        IUvalues["color"] = "#301ae3";
                        IUvalues["lesstime"] = "1";
                    }
                    else
                    {
                        IUvalues["color"] = "#02bbf0";
                        IUvalues["lesstime"] = "0";
                    }
                    
                    IUvalues["name"] =  KpiAttendanceDR["name"].ToString();
                    IUvalues["employee_id"] = KpiAttendanceDR["employee_id"].ToString();
                    IUvalues["title"] = KpiAttendanceDR["title"].ToString();
                    IUvalues["applied"] = "0";
                    IUvalues["proc"] = "None";
                    IUvalues["span"] = "0";
                    IUvalues["type"] = "P";                    
                    IUvalues["missed"] = "0";
                    IUvalues["latearrival"] = "0";
                    IUvalues["Intimefound"] = "0";
                    IUvalues["Outtimefound"] = "0";
                    IUvalues["Shifttimefound"] = "0";
                    IUvalues["difference"] = KpiAttendanceDR["title"].ToString();
                    IUvalues["ShiftStart"] = Convert.ToDateTime(KpiAttendanceDR["date"]).ToString("yyyy-MM-dd");
                    IUvalues["ShiftEnd"] = Convert.ToDateTime(KpiAttendanceDR["date"]).ToString("yyyy-MM-dd");
                    IUvalues["InDatetime"] = Convert.ToDateTime(KpiAttendanceDR["date"]).ToString("yyyy-MM-dd");
                    IUvalues["OutDatetime"] = Convert.ToDateTime(KpiAttendanceDR["date"]).ToString("yyyy-MM-dd");
                    IUvalues["Outtime"] = "00:00:00";
                    IUvalues["Intime"] = "00:00:00";
                    IUvalues["shifttime"] = "00:00:00";
                    IUvalues["ShiftDatetime"] = Convert.ToDateTime(KpiAttendanceDR["date"]).ToString("yyyy-MM-dd");
                    IUvalues["asm_id"] = KpiAttendanceDR["asm_id"].ToString();
                    IUvalues["sap_code"] = KpiAttendanceDR["asm_id"].ToString();
                    IUvalues["date"] = KpiAttendanceDR["date"].ToString();


                    if (count <= 0)
                    {
                        AttendaceInsert(conn, IUvalues, count);
                    }
                    else
                    {
                        AttendaceUpdate(conn, IUvalues, count);
                    }

                   
                    /* Console.Write(" {0}\n", KpiAttendanceDR["date"]);
                     Console.Write(" {0}\n", KpiAttendanceDR["sip"]);
                     Console.Write(" {0}\n", KpiAttendanceDR["asm_id"]);
                     Console.Write(" {0}\n", KpiAttendanceDR["title"]);
                     Console.Write(" {0}\n", KpiAttendanceDR["login"]);
                     Console.Write(" {0}\n", KpiAttendanceDR["logout"]);*/
                }

                //CloseConnection();
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
        private DataSet GetKpiAttendanceQuery()
        {
            string KpiSelectQry = "select `date`, `sip`, `asm_id`, SEC_TO_TIME( SUM( TIME_TO_SEC(`agent_time`))) As title" +
                " , `tbl_attendances_kpi`.`shift_time`" +
                " , `tbl_attendances_kpi`.`login`, `tbl_attendances_kpi`.`logout`, `tbl_users`.`employee_id`, `tbl_users`.`name` " +
                " from tbl_attendances_kpi join tbl_users on (`tbl_attendances_kpi`.`asm_id` = `tbl_users`.`login`) " +
                " where `tbl_attendances_kpi`.`date` >= '" + localMonthDate.ToString("yyyy-MM-dd") + "' " +
                " and `tbl_attendances_kpi`.`date` <= '" + localDate.ToString("yyyy-MM-dd") + "' " +
                " group by `date`, `asm_id` , `tbl_attendances_kpi`.`login`" +
                ", `tbl_attendances_kpi`.`logout`,	`tbl_users`.`employee_id`,	`tbl_users`.`name` " +
                ", `sip`, `tbl_attendances_kpi`.`shift_time`" +
                "order by asm_id asc, date asc";//and asm_id = '7049'

            Console.Write(" {0}\n", KpiSelectQry);

            // OpenConection();
            // MySqlCommand attenUsrEmp = new MySqlCommand(attenUsrEmpQry, conn); 
            DataSet KpiSelectDS = new DataSet();
            // DataTable KpiSelectDT = new DataTable();
            // MySqlDataReader attenUsrEmpRdr;


            try
            {
                // Way 1
                // MySqlCommand KpiSelectCmd = new MySqlCommand(KpiSelectQry, conn);
                // KpiSelectCmd.CommandType = CommandType.Text;
                // MySqlDataReader KpiSelectRdr = KpiSelectCmd.ExecuteReader();

                // Way 2
                // MySqlDataAdapter KpiSelectDA = new MySqlDataAdapter(KpiSelectQry, conn);
                // KpiSelectDA.SelectCommand.CommandType = CommandType.Text;
                // KpiSelectDA.Fill(KpiSelectDS, "KpiAttendance");

                // Way 3
                using(MySqlCommand KpiSelectCmd = new MySqlCommand(KpiSelectQry, conn))
                {
                    using (MySqlDataAdapter KpiSelectDA = new MySqlDataAdapter(KpiSelectCmd))
                    {
                        KpiSelectDA.SelectCommand.CommandType = CommandType.Text;
                        // attenUsrEmpDA.Fill(KpiSelectDT);
                        KpiSelectDA.Fill(KpiSelectDS, "KpiAttendance");
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
            return KpiSelectDS;
            // return KpiSelectDT;
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
        ~KpiAttendance()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out KPI Attendance..........................");
        }
    }
}
