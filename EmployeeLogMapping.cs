using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace AttendancesServices
{
    class EmployeeLogMapping : Common, IDisposable
    {
        // private DateTime Start, End, Start1, End1;

        private readonly DateTime localDate;
        private readonly DateTime localMonthDate;
        private readonly MySqlConnection conn;

        public EmployeeLogMapping(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;

            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Employee Log Mapping Connection working.");
            }
            else
            {
                Console.WriteLine("Employee Log Mapping Please check connection string");
            }
            // DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine("............................. In Employee Log Mapping ............................");
        }
        public void GetEmployeeLogMappaing()
        {
            /* 
             * Start1 = new DateTime(localDate.AddMonths(-2).Year, localDate.AddMonths(-2).Month, 21);
             * End1 = new DateTime(localDate.AddMonths(-1).Year, localDate.AddMonths(-1).Month, 20);
             * Start = new DateTime(localDate.AddMonths(-1).Year, localDate.AddMonths(-1).Month, 21);
             * End = new DateTime(localDate.Year, localDate.Month, 20);
             */
            long counting = 1;
            DataSet MachineAttendanceDataSet = GetMachineAttendance();
            DataTable EmployeeLogs;
            foreach (DataRow MachineAttendanceRow in MachineAttendanceDataSet.Tables["Attendance"].Rows)//campaignDS.Tables["Customers"].Rows
            {
                Console.Write(" {0}\n", MachineAttendanceRow["asm_id"]);
                Console.Write(" {0}\n", MachineAttendanceRow["employee_id"]);
                Console.Write(" {0}\n", MachineAttendanceRow["date"]);
                Console.Write(" {0}\n", counting);

                if (MachineAttendanceRow.IsNull("employee_id"))
                {
                    Console.Write("Employee not found {0} \n", MachineAttendanceRow["asm_id"]);
                    continue;
                }
                string InsertUpdateQry, updateColumns;
                EmployeeLogs = GetEmployeeLogs(MachineAttendanceRow["employee_id"].ToString(), MachineAttendanceRow["date"].ToString());
                foreach (DataRow EmployeeLogsRow in EmployeeLogs.Rows)
                {
                    string campaign_id = EmployeeLogsRow["campaign_id"].ToString();
                    string sub_campaign_id = EmployeeLogsRow["sub_campaign_id"].ToString();

                    updateColumns = " department_id = '" + EmployeeLogsRow["department_id"] + "' ";
                    updateColumns += ", designation_id = '" + EmployeeLogsRow["designation_id"] + "' ";
                    updateColumns += ", center_id = '" + EmployeeLogsRow["center_id"] + "' ";
                    updateColumns += ", project_id = '" + EmployeeLogsRow["project_id"] + "' ";
                    updateColumns += ", employment_type_id = '" + EmployeeLogsRow["employment_type_id"] + "' ";


                    string reporingTo = EmployeeLogsRow["reporting_to"].ToString();
                    if (string.IsNullOrEmpty(reporingTo))
                    {
                        reporingTo = "1";
                    }

                    updateColumns += " , reporting_to = '" + reporingTo + "' ";
                    string reporting_to_too = EmployeeLogsRow["reporting_to_too"].ToString();
                    if (string.IsNullOrEmpty(reporting_to_too))
                    {
                        updateColumns += " , reporting_to_too = '" + reporingTo + "' ";
                    }
                    else
                    {
                        updateColumns += " , reporting_to_too = '" + reporting_to_too + "' ";
                    }

                    /* updateColumns += ", reporting_to = '" + EmployeeLogsRow["reporting_to"] + "' ";

                     updateColumns += ", reporting_to_too = '" + EmployeeLogsRow["reporting_to_too"] + "' ";*/


                    if (string.IsNullOrEmpty(campaign_id))
                    {
                        updateColumns += " , campaign_id = '" + EmployeeLogsRow["project_id"] + "' ";
                    }
                    else
                    {
                        updateColumns += " , campaign_id = '" + campaign_id + "' ";
                    }

                    if (string.IsNullOrEmpty(sub_campaign_id))
                    {
                        updateColumns += " , sub_campaign_id = '" + EmployeeLogsRow["project_id"] + "' ";
                    }
                    else
                    {
                        updateColumns += " , sub_campaign_id = '" + sub_campaign_id + "' ";
                    }

                    InsertUpdateQry = " UPDATE tbl_attendances_machine set " + updateColumns;
                    InsertUpdateQry += " where asm_id = " + MachineAttendanceRow["asm_id"] + " and date = '" + Convert.ToDateTime(MachineAttendanceRow["date"]).ToString("yyyy-MM-dd") + "'"; // + " and sap_code = " + item.Key + "";

                    Console.Write(" {0}\n", InsertUpdateQry);


                    using (MySqlCommand command = new(InsertUpdateQry, conn))
                    {
                        if (command.ExecuteNonQuery() != 1)
                        {
                            //'handled as needed, //' but this snippet will throw an exception to force a rollback
                            throw new InvalidProgramException();
                        }
                    }

                }


                counting++;
            }
            // return;
        }
        private DataSet GetMachineAttendance()
        {
            string AttendanceMachineQry = "select * from tbl_attendances_machine where date >= '" + localMonthDate.ToString("yyyy-MM-dd") + "'  and date <= '" + localDate.ToString("yyyy-MM-dd") + "' order by date ASC, asm_id ASC ";
            Console.Write(" {0}\n", AttendanceMachineQry);
            DataSet AttendanceMachineDS = new();

            // DataTable attenUsrEmpDT = new DataTable();
            // MySqlDataReader attenUsrEmpRdr;
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
                using (MySqlCommand AttendanceMachineEmp = new(AttendanceMachineQry, conn))
                {
                    using (MySqlDataAdapter AttendanceMachineDA = new(AttendanceMachineEmp))
                    {
                        AttendanceMachineDA.SelectCommand.CommandType = CommandType.Text;
                        AttendanceMachineDA.Fill(AttendanceMachineDS, "Attendance");
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

            return AttendanceMachineDS;

        }

        private DataTable GetEmployeeLogs(string employee_id, string day)
        {
            string SelectEmployeeLogs = "select * from tbl_employee_logs " +
                "where state = 1 and project_id is not null " +
                "and employee_id = " + employee_id + " " +
                "and date <= '" + Convert.ToDateTime(day).ToString("yyyy-MM-dd") + "' " +
                "order by date desc limit 1";
            DataTable EmployeeLogsDT = new();

            Console.Write(" {0}\n", SelectEmployeeLogs);
            try
            {
                using (MySqlCommand EmployeeLogsCmd = new(SelectEmployeeLogs, conn))
                {
                    using (MySqlDataAdapter EmployeeLogsDA = new(EmployeeLogsCmd))
                    {
                        EmployeeLogsDA.SelectCommand.CommandType = CommandType.Text;
                        EmployeeLogsDA.Fill(EmployeeLogsDT);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }


            return EmployeeLogsDT;
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
        ~EmployeeLogMapping()
        {
            conn.Close();
            conn.Dispose();
            Console.WriteLine("............................. Out Employee Log Mapping ............................");
        }
    }
}
