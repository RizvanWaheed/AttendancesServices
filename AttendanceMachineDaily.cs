using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AttendancesServices
{
    sealed class AttendanceMachineDaily : CommonQueries, IDisposable //,  // : IDisposable
    {
        private readonly DateTime localDate;
        private readonly DateTime localMonthDate;

        private readonly MySqlConnection conn;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DictData;

        public AttendanceMachineDaily(DateTime From, DateTime To)
        {
            localDate = To;
            localMonthDate = From;

            conn = DatabaseConnection.GetDBConnection();
            conn.Open();

            var temp = conn.State.ToString();
            if (temp == "Open")//sqlCon.State == ConnectionState.Open && 
            {
                Console.WriteLine("Machine Connection working.");
            }
            else
            {
                Console.WriteLine("Machine Please check connection string.");
            }
            DictData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Console.WriteLine(".............................In Machine Attendance..........................");
        }
        /*public MachineAttendance(string color) : this()
        {
            this.color = color;
            Console.WriteLine("Constructor with color parameter called!");
        }
        public MachineAttendance(string param1, string param2) : this(param1)
        {

        }*/
        public void GetMachineAttendance()
        {
            Console.WriteLine("  Local Date: {0}", localDate);
            Console.WriteLine("  Local Two Month Date: {0}", localMonthDate);

            DataSet attendanceDS = GetAttendanceEmployeeUsers();
            var actual_shifttime_ends_next = new Dictionary<string, DateTime>();

            DateTime checkOlder = new(2015, 12, 31);
            Dictionary<string, string> employeeUserDict = new();
            foreach (DataRow campaignRow in attendanceDS.Tables["Attendance"].Rows)//campaignDS.Tables["Customers"].Rows
            {

                DateTime CardTime = Convert.ToDateTime(campaignRow["CardTime"]);
                string NewDate = Convert.ToDateTime(campaignRow["CardTime"]).ToString("yyyy-MM-dd");
                string NextDate = Convert.ToDateTime(campaignRow["CardTime"]).AddDays(1).ToString("yyyy-MM-dd");  // Carbon::parse($value['CardTime'])->addDay()->toDateString();
                string PastDate = Convert.ToDateTime(campaignRow["CardTime"]).AddDays(-1).ToString("yyyy-MM-dd");  // Carbon::parse($value['CardTime'])->subDay()->toDateString();
                string NewTime = Convert.ToDateTime(campaignRow["CardTime"]).ToString("HH:mm:ss");  // Carbon::parse($value['CardTime'])->toTimeString();
                string sap_code = campaignRow["EmployeeCode"].ToString();

                if (DictData.ContainsKey(sap_code))
                {
                    if (!DictData[sap_code].ContainsKey(NewDate))
                    {
                        DictData[sap_code].Add(NewDate, new Dictionary<string, string>()); //{ { "sap_code", sap_code } }
                    }
                }
                else
                {
                    DictData.Add(sap_code, new Dictionary<string, Dictionary<string, string>>() { { NewDate, new Dictionary<string, string>() } }); // { { "sap_code", sap_code } }
                }
                if (DictData.ContainsKey(sap_code))
                {
                    if (!DictData[sap_code].ContainsKey(NextDate))
                    {
                        DictData[sap_code].Add(NextDate, new Dictionary<string, string>()); //{ { "sap_code", sap_code } }
                    }
                }
                else
                {
                    DictData.Add(sap_code, new Dictionary<string, Dictionary<string, string>>() { { NextDate, new Dictionary<string, string>() } }); // { { "sap_code", sap_code } }
                }
                if (!DictData[sap_code][NewDate].ContainsKey("asm_id"))
                {
                    employeeUserDict = GetUserEmployee(sap_code, conn); //NewDate,
                    if (employeeUserDict.Count() < 1)
                    {
                        continue;
                    }
                }
                DictData[sap_code][NextDate]["name"] = DictData[sap_code][NewDate]["name"] = employeeUserDict["name"];
                DictData[sap_code][NextDate]["employee_id"] = DictData[sap_code][NewDate]["employee_id"] = employeeUserDict["employee_id"];
                DictData[sap_code][NextDate]["asm_id"] = DictData[sap_code][NewDate]["asm_id"] = employeeUserDict["asm_id"];
                DictData[sap_code][NextDate]["weeklyOff"] = DictData[sap_code][NewDate]["weeklyOff"] = employeeUserDict["weeklyOff"];
                DictData[sap_code][NextDate]["working_hour"] = DictData[sap_code][NewDate]["working_hour"] = employeeUserDict["working_hour"];
                DictData[sap_code][NextDate]["working_hour_number"] = DictData[sap_code][NewDate]["working_hour_number"] = employeeUserDict["working_hour_number"];

                Dictionary<string, string> shiftDict = GetAttendanceShift(NewDate, DictData[sap_code][NewDate]["asm_id"]);

                DictData[sap_code][NextDate]["Shifttime"] = DictData[sap_code][NewDate]["Shifttime"] = shiftDict["Shifttime"].ToString();
                DictData[sap_code][NextDate]["Shifttimefound"] = DictData[sap_code][NewDate]["Shifttimefound"] = shiftDict["Shifttimefound"].ToString();
                DictData[sap_code][NextDate]["Shifttimenumber"] = DictData[sap_code][NewDate]["Shifttimenumber"] = shiftDict["Shifttimenumber"].ToString();


                if (!actual_shifttime_ends_next.ContainsKey(sap_code))
                {
                    actual_shifttime_ends_next.Add(sap_code, checkOlder); //{ { "sap_code", sap_code } }
                }
                if (!DictData[sap_code][NewDate].ContainsKey("Intimefound"))
                {
                    DictData[sap_code][NewDate]["Intime"] = string.Empty;
                    DictData[sap_code][NewDate]["InDatetime"] = string.Empty;
                    DictData[sap_code][NewDate]["Intimefound"] = string.Empty;
                }
                if (!DictData[sap_code][NewDate].ContainsKey("Outtimefound"))
                {
                    DictData[sap_code][NewDate]["Outtimefound"] = string.Empty;
                    DictData[sap_code][NewDate]["Outtime"] = string.Empty;
                    DictData[sap_code][NewDate]["OutDatetime"] = string.Empty;
                }

                String actual_shifttime = DictData[sap_code][NewDate]["ShiftDatetime"] = Convert.ToDateTime(NewDate + " " + shiftDict["Shifttime"]).ToString("yyyy-MM-dd HH:mm:ss");
                String actual_shifttime_start = DictData[sap_code][NewDate]["ShiftStart"] = Convert.ToDateTime(NewDate + " " + shiftDict["Shifttime"]).AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss");
                String actual_shifttime_ends = DictData[sap_code][NewDate]["ShiftEnd"] = Convert.ToDateTime(NewDate + " " + shiftDict["Shifttime"]).AddHours(20).ToString("yyyy-MM-dd HH:mm:ss");
                String actual_shifttime_start_max = Convert.ToDateTime(NewDate + " " + shiftDict["Shifttime"]).AddHours(6).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime actual_shifttime_margin = Convert.ToDateTime(NewDate + " " + shiftDict["Shifttime"]).AddMinutes(15);

                /*
                 * < 0 − If date1 is earlier than date2. date1 before date2
                0 − If date1 is the same as date2. date1 equals date2
                > 0 − If date1 is later than date2. date1 after date2
                */
                int intialCase = DateTime.Compare(actual_shifttime_ends_next[sap_code], checkOlder); // result != 0
                // actual_shifttime_ends_next[sap_code]->notEqualTo(actual_shifttime_ends)
                int resultOut = DateTime.Compare(actual_shifttime_ends_next[sap_code], actual_shifttime_margin); // result != 0
                //actual_shifttime_ends_next["sap_code"]->notEqualTo(actual_shifttime_ends) 
                int resultOut2 = DateTime.Compare(actual_shifttime_ends_next[sap_code], CardTime); // result2 > 0 || date1 > date2
                //$CardTime->lt($actual_shifttime_ends_next) actual_shifttime_ends_next > caredTime
                int resultOut3 = DateTime.Compare(Convert.ToDateTime(actual_shifttime_start), CardTime); // result2 > 0 || date1 > date2 
                // $CardTime->lt($actual_shifttime_start)
                int resultIn = DateTime.Compare(Convert.ToDateTime(actual_shifttime_start), CardTime); // result2 < 0 || date1 < date2
                // $CardTime->gt($actual_shifttime_start);
                int resultIn2 = DateTime.Compare(Convert.ToDateTime(actual_shifttime_start_max), CardTime); // resultIn2 > 0 || date1 > date2
                // $CardTime->lt($actual_shifttime_start_max);
                int resultOut4 = DateTime.Compare(Convert.ToDateTime(actual_shifttime_ends), CardTime); // resultOut4 > 0 || date1 > date2
                // $CardTime->lt($actual_shifttime_ends)
                int resultIn3 = DateTime.Compare(Convert.ToDateTime(actual_shifttime_ends), CardTime); // resultIn3 > 0 || date1 > date2
                                                                                                       //$CardTime->gt($actual_shifttime_ends)

                /*Console.Write(" {0}\n", actual_shifttime_ends_next[sap_code]);
                Console.Write(" {0}\n", checkOlder);
                Console.Write(" {0}\n", intialCase);

                Console.Write(" {0}\n", actual_shifttime_ends_next[sap_code]);
                Console.Write(" {0}\n", actual_shifttime_margin);
                Console.Write(" {0}\n", resultOut);

                Console.Write(" {0}\n", actual_shifttime_ends_next[sap_code]);
                Console.Write(" {0}\n", CardTime);
                Console.Write(" {0}\n", resultOut2);

                Console.Write(" {0}\n", actual_shifttime_start);
                Console.Write(" {0}\n", CardTime);
                Console.Write(" {0}\n", resultOut3);

                Console.Write(" {0}\n", actual_shifttime_start);
                Console.Write(" {0}\n", CardTime);
                Console.Write(" {0}\n", resultIn);

                Console.Write(" {0}\n", actual_shifttime_start_max);
                Console.Write(" {0}\n", CardTime);
                Console.Write(" {0}\n", resultIn2);

                Console.Write(" {0}\n", actual_shifttime_ends);
                Console.Write(" {0}\n", CardTime);
                Console.Write(" {0}\n", resultOut4);

                Console.Write(" {0}\n", actual_shifttime_ends);
                Console.Write(" {0}\n", CardTime);
                Console.Write(" {0}\n", resultIn3);*/

                TimeSpan ReslShift = TimeSpan.Parse("00:00");
                TimeSpan ReslShift2 = TimeSpan.Parse("-06:00");
                if (DictData[sap_code].ContainsKey(PastDate))
                {
                    if (DictData[sap_code][PastDate].ContainsKey("Shifttime"))
                    {
                        TimeSpan PastShift = TimeSpan.Parse(DictData[sap_code][PastDate]["Shifttime"]);
                        TimeSpan CurrShift = TimeSpan.Parse(DictData[sap_code][NewDate]["Shifttime"]);
                        ReslShift = CurrShift.Subtract(PastShift);

                        Console.Write(" Shift Difference {0}\n", ReslShift);
                        Console.Write(" Difference Result {0}\n", ReslShift >= ReslShift2);
                        Console.Write(" Difference Result2 {0}\n", ReslShift < ReslShift2);

                    }
                }



                if (intialCase != 0 && resultOut != 0 && resultOut2 > 0 && resultOut3 > 0) //((resultOut3 > 0 && ReslShift >= ReslShift2) || (resultOut3 < 0 && ReslShift < ReslShift2)))
                {
                    Console.Write(" {0}\n", "First IF");
                    if (!DictData[sap_code].ContainsKey(PastDate))
                    {
                        DictData[sap_code].Add(PastDate, new Dictionary<string, string>()); //{ { "sap_code", sap_code } }
                    }
                    DictData[sap_code][PastDate]["Outtime"] = NewTime;
                    DictData[sap_code][PastDate]["Outtimefound"] = "1";
                    DictData[sap_code][PastDate]["OutDatetime"] = Convert.ToDateTime(campaignRow["CardTime"]).ToString("yyyy-MM-dd HH:mm:ss");

                }
                else if (resultIn < 0 && resultIn2 > 0 && String.IsNullOrEmpty(DictData[sap_code][NewDate]["Intimefound"]))
                {
                    Console.Write(" {0}\n", "Second IF");
                    DictData[sap_code][NewDate]["InDatetime"] = Convert.ToDateTime(campaignRow["CardTime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    DictData[sap_code][NewDate]["Intime"] = NewTime;
                    DictData[sap_code][NewDate]["Intimefound"] = "1";
                }
                else if (resultOut4 > 0 && String.IsNullOrEmpty(DictData[sap_code][NewDate]["Outtimefound"]))
                {
                    Console.Write(" {0}\n", "Third IF");
                    DictData[sap_code][NewDate]["Outtime"] = NewTime;
                    DictData[sap_code][NewDate]["Outtimefound"] = "1";
                    DictData[sap_code][NewDate]["OutDatetime"] = Convert.ToDateTime(campaignRow["CardTime"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (resultIn3 < 0)
                {
                    Console.Write(" {0}\n", "Fourth IF");
                    DictData[sap_code][NextDate]["Intime"] = NewTime; //$all_dates[$NewDate]['Outtime'].'-'.
                    DictData[sap_code][NextDate]["InDatetime"] = Convert.ToDateTime(campaignRow["CardTime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    DictData[sap_code][NextDate]["Intimefound"] = "1";
                }
                else
                {
                    Console.Write(" {0}\n", "Fifth IF");
                    DictData[sap_code][NewDate]["Outtime"] = NewTime; //$all_dates[$NewDate]['Outtime'].'-'.
                    DictData[sap_code][NewDate]["OutDatetime"] = Convert.ToDateTime(campaignRow["CardTime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    // $actual_shifttime_ends_prev = Carbon::parse($NewDate.' '.$shift_time)->addHours(18);
                }
                actual_shifttime_ends_next[sap_code] = Convert.ToDateTime(actual_shifttime_ends);




            }
            foreach (var item in DictData)
            {
                // foo(item.Key);
                // bar(item.Value);
                foreach (var itemL2 in item.Value.ToList())
                {
                    // itemL2.Key;
                    Dictionary<string, string> dictInner = itemL2.Value;
                    if (!dictInner.ContainsKey("asm_id"))
                    {
                        DictData[item.Key].Remove(itemL2.Key);
                        continue;
                    }
                    // Console.Write(dictInner["asm_id"]);
                    DateTime todayDate = Convert.ToDateTime(itemL2.Key);
                    if (!dictInner.ContainsKey("Outtimefound"))
                    {
                        dictInner["Outtimefound"] = string.Empty;
                    }
                    if (!dictInner.ContainsKey("Intimefound"))
                    {
                        dictInner["Intimefound"] = string.Empty;
                    }
                    DictData[item.Key][itemL2.Key]["latearrival"] = "0";
                    DictData[item.Key][itemL2.Key]["lesstime"] = "0";
                    DictData[item.Key][itemL2.Key]["missed"] = "0";
                    DictData[item.Key][itemL2.Key]["difference"] = "0";
                    DictData[item.Key][itemL2.Key]["color"] = "#000";

                    if (String.IsNullOrEmpty(dictInner["Intimefound"]) && (!String.IsNullOrEmpty(dictInner["Outtimefound"])))
                    {
                        // $all_dates[$value['date']]['startEditable'] = false;
                        DictData[item.Key][itemL2.Key]["color"] = "#01ff70";
                        DictData[item.Key][itemL2.Key]["title"] = "In: Missing Out: " + dictInner["Outtime"];
                        DictData[item.Key][itemL2.Key]["missed"] = "1";

                        string missingin = GetMissingAttendance(itemL2.Key, dictInner["asm_id"], "In");
                        if (!String.IsNullOrEmpty(missingin))
                        {
                            DictData[item.Key][itemL2.Key]["color"] = "#02bbf0";
                            DictData[item.Key][itemL2.Key]["title"] = "In: " + missingin + " Out: " + dictInner["Outtime"];
                            DictData[item.Key][itemL2.Key]["missed"] = "0";
                            DictData[item.Key][itemL2.Key]["Intime"] = missingin;
                            DictData[item.Key][itemL2.Key]["Intimefound"] = "1";
                            DictData[item.Key][itemL2.Key]["InDatetime"] = Convert.ToDateTime(Convert.ToDateTime(dictInner["ShiftDatetime"]).ToString("yyyy-MM-dd") + " " + missingin).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    else if (String.IsNullOrEmpty(dictInner["Outtimefound"]) && !String.IsNullOrEmpty(dictInner["Intimefound"]))
                    {
                        DictData[item.Key][itemL2.Key]["color"] = "#01ff70";
                        DictData[item.Key][itemL2.Key]["title"] = "In: " + dictInner["Intime"] + " Out: Missing";
                        DictData[item.Key][itemL2.Key]["missed"] = "1";

                        string missingout = GetMissingAttendance(itemL2.Key, dictInner["asm_id"], "Out");
                        if (!String.IsNullOrEmpty(missingout))
                        {
                            DictData[item.Key][itemL2.Key]["color"] = "#02bbf0";
                            DictData[item.Key][itemL2.Key]["title"] = "In: " + dictInner["Intime"] + " Out: " + missingout;
                            DictData[item.Key][itemL2.Key]["missed"] = "0";
                            DictData[item.Key][itemL2.Key]["Outtime"] = missingout;
                            DictData[item.Key][itemL2.Key]["Outtimefound"] = "1";
                            DictData[item.Key][itemL2.Key]["OutDatetime"] = Convert.ToDateTime(Convert.ToDateTime(dictInner["ShiftDatetime"]).ToString("yyyy-MM-dd") + " " + missingout).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    if (!String.IsNullOrEmpty(dictInner["Outtimefound"]) && !String.IsNullOrEmpty(dictInner["Intimefound"]))
                    {
                        DateTime Margin = Convert.ToDateTime(dictInner["ShiftDatetime"]).AddMinutes(15);

                        DateTime fromtiming = Convert.ToDateTime(dictInner["InDatetime"]);
                        DateTime totiming = Convert.ToDateTime(dictInner["OutDatetime"]);
                        TimeSpan span = totiming.Subtract(fromtiming);
                        TimeSpan span2 = TimeSpan.Parse(dictInner["working_hour"]);
                        int tresult = TimeSpan.Compare(span, span2);

                        int hours = span.Hours;
                        int minutes = span.Minutes;
                        char spliter = ':';
                        string[] multiArray = dictInner["working_hour"].ToString().Split(spliter);//, StringSplitOptions.None
                        // $diftim = $totiming->diffInHours($fromtiming);
                        // $diftim = $totiming->diff($fromtiming)->format('%H:%i:%s');

                        DictData[item.Key][itemL2.Key]["difference"] = hours + ":" + minutes;
                        int resultMargin = DateTime.Compare(fromtiming, Margin);

                        Console.Write(" {0}\n", "-------------------------------------------------------------------");

                        //Console.Write(" {0}\n", DictData[item.Key][itemL2.Key]["Shifttime"]);
                        //Console.Write(" {0}\n", fromtiming);
                        //Console.Write(" {0}\n", Margin);

                        Console.Write(" {0}\n", span);
                        Console.Write(" {0}\n", dictInner["working_hour"]);
                        Console.Write(" {0}\n", tresult);
                        Console.Write(" {0}\n", int.Parse(multiArray[0]));
                        Console.Write(" {0}\n", int.Parse(multiArray[1]));
                        Console.Write(" {0}\n", hours);
                        Console.Write(" {0}\n", minutes);


                        /*
                        < 0 − If fromtiming is earlier than Margin. fromtiming before Margin
                        = 0 − If date1 is the same as date2. date1 equals date2
                        > 0 − If date1 is later than date2. date1 after date2
                        */
                        // if(strtotime($value['InDatetime']) > strtotime($actual_shifttime_margin->toDateTimeString())){
                        Console.Write(" {0}\n", "-------------------------------------------------------------------");
                        if (resultMargin > 0)
                        {
                            DictData[item.Key][itemL2.Key]["color"] = "#0c679b";
                            DictData[item.Key][itemL2.Key]["latearrival"] = "1";
                        }
                        else if (tresult < 1)
                        {
                            // else if (int.Parse(multiArray[0]) < hours || int.Parse(multiArray[1]) < minutes)
                            DictData[item.Key][itemL2.Key]["color"] = "#301ae3";
                            DictData[item.Key][itemL2.Key]["lesstime"] = "1";
                        }
                        else
                        {
                            DictData[item.Key][itemL2.Key]["color"] = "#02bbf0";
                        }
                        if (tresult < 1) //(int.Parse(multiArray[0]) < hours || int.Parse(multiArray[1]) < minutes)
                        {
                            DictData[item.Key][itemL2.Key]["lesstime"] = "1";
                        }

                        // $all_dates[$value['date']]['color'] = '#02bbf0';
                        // 0c679b Carbon::parse($fromtiming)->lt(Carbon::parse($actual_shifttime_margin)

                        DictData[item.Key][itemL2.Key]["title"] = "In:" + dictInner["Intime"] + " Out: " + dictInner["Outtime"];

                        // $all_dates[$value['date']]['startEditable'] = false;
                        // $all_dates2[$value['date']]['title'] = ' Out: '.$value['Outtime'];
                        // $all_dates[$value['date']]['title'] = $diftim.' In:'.$value['Intime'] .' Out: '.$value['Outtime'];
                        // $all_dates[$value['date']]['title'] = Carbon::parse($fromtiming)->gt(Carbon::parse($actual_shifttime_margin)).' In:'.$value['InDatetime'] .' Out: '.$actual_shifttime_margin;
                        // $all_dates[$value['date']]['title'] = $diftim.' In:'.$diftim .' Out: '.$working_hour;
                        // $all_dates[$value['date']]['title'] = Carbon::parse('first saturday of '.$monthYear).'-'.Carbon::parse('first saturday of '.$monthYear)->eq(Carbon::parse($value['Intime']));
                        // 'Out: '.Carbon::parse($value['Outtime'])->weekOfMonth.'-'.Carbon::parse('third saturday of '.$   ->month.'2020').'-'.Carbon::parse('fourth saturday of '.$request->month.'2020');//Carbon::parse($value['Outtime'])->isSaturday();//Carbon::parse($value['Intime'])->weeksInMonth.
                    }
                }
            }

            SetAttendanceData(DictData);
            conn.Close();


            /*attenUsrEmpDT.Load(attenUsrEmpQryCmd.ExecuteReader());*/
            /*  Console.Write(" {0}\n", attenUsrEmpDS);*/

        }
        private DataSet GetAttendanceEmployeeUsers()
        {
            string attenUsrEmpQry = " SELECT `kqz_employee`.`EmployeeCode`, `kqz_card`.`CardTime` ";
            attenUsrEmpQry += " FROM `kqz_employee`	INNER JOIN `kqz_card` ON `kqz_employee`.`EmployeeID` = `kqz_card`.`EmployeeID` ";
            attenUsrEmpQry += " WHERE `kqz_card`.`CardTime` >= '" + localMonthDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            attenUsrEmpQry += " AND `kqz_card`.`CardTime` <= '" + localDate.ToString("yyyy-MM-dd HH:mm:ss") + "'  ";// AND `kqz_employee`.`EmployeeCode` = '30011105'  AND `kqz_employee`.`EmployeeCode` = '30000061' //  AND `kqz_employee`.`EmployeeCode` = '30000061'
            attenUsrEmpQry += " ORDER BY `EmployeeCode` ASC, `CardTime` ASC";

            Console.Write(" {0}\n", attenUsrEmpQry);
            // DatabaseConnection.getDBConnection();

            // MySqlCommand attenUsrEmp = new MySqlCommand(attenUsrEmpQry, conn); 
            DataSet attenUsrEmpDS = new();
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
                using (MySqlCommand attenUsrEmp = new(attenUsrEmpQry, conn))
                {
                    using (MySqlDataAdapter attenUsrEmpDA = new(attenUsrEmp))
                    {
                        attenUsrEmpDA.SelectCommand.CommandType = CommandType.Text;
                        attenUsrEmpDA.Fill(attenUsrEmpDS, "Attendance");
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
            // CloseConnection();
            return attenUsrEmpDS;
            // return attenUsrEmpDT;
        }

        private Dictionary<string, string> GetAttendanceShift(string dateValue, string asmId)
        {
            // string cardTime = Convert.ToDateTime(dateValue).ToString("yyyy-MM-dd"); 

            string attenShiftQry = "select `times` from `tbl_attendances_shifts` ";
            attenShiftQry += " where `asm_id` = " + asmId + " and `status` = 1 and date(`dates`) <= '" + dateValue + "' ";
            attenShiftQry += " order by `dates` desc limit 1 ";

            Console.Write(" {0}\n", attenShiftQry);

            //OpenConection();
            string attenShiftSlr = string.Empty;
            Dictionary<string, string> shiftTimeDict = new();
            using (MySqlCommand attenShiftCmd = new(attenShiftQry, conn))
            {
                var tsAttendShift = attenShiftCmd.ExecuteScalar();
                if (tsAttendShift != null)
                {
                    attenShiftSlr = tsAttendShift.ToString();
                }
            }
            // Execute the query and obtain the value of the first column of the first row
            //CloseConnection();
            if (string.IsNullOrEmpty(attenShiftSlr))
            {
                shiftTimeDict["Shifttimefound"] = "0";
                shiftTimeDict["Shifttime"] = "09:00:00";
                shiftTimeDict["Shifttimenumber"] = "09";
            }
            else
            {
                shiftTimeDict["Shifttimefound"] = "1";
                shiftTimeDict["Shifttime"] = attenShiftSlr;
                shiftTimeDict["Shifttimenumber"] = attenShiftSlr.Substring(0, 1);
            }
            return shiftTimeDict;

        }
        private String GetMissingAttendance(string dateValue, string asmId, string missing)
        {
            // DateTime cardTime = Convert.ToDateTime(dateValue);

            string attenShiftQry = "select `time` from `tbl_attendances_missings` ";
            attenShiftQry += " where `asm_id` = " + asmId + " and `active` = 1 and `date` = '" + dateValue + "'  and `missing` = '" + missing + "' ";
            attenShiftQry += " ORDER BY created desc limit 1 ";

            Console.Write(" {0}\n", attenShiftQry);

            //OpenConection();
            String attenShiftSlr;

            using (MySqlCommand attenShiftCmd = new(attenShiftQry, conn))
            {
                var attenShiftSlr1 = attenShiftCmd.ExecuteScalar();
                attenShiftSlr = (attenShiftSlr1 == null) ? string.Empty : attenShiftSlr1.ToString();
            }
            // Execute the query and obtain the value of the first column of the first row

            // CloseConnection();
            return attenShiftSlr;

            /*Dictionary<string, string> shiftTimeDict = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(attenShiftSlr))
            {
                shiftTimeDict["Shifttimefound"] = "false";
                shiftTimeDict["Shifttime"] = "09:00:00";
                shiftTimeDict["Shifttimenumber"] = "09";
            }
            else
            {
                shiftTimeDict["Shifttimefound"] = "true";
                shiftTimeDict["Shifttime"] = attenShiftSlr;
                shiftTimeDict["Shifttimenumber"] = attenShiftSlr.Substring(0, 1);
            }
            return shiftTimeDict;*/

        }
        private void SetAttendanceData(Dictionary<string, Dictionary<string, Dictionary<string, string>>> attendances)
        {
            // OpenConection();
            // string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {

                foreach (var item in attendances)
                {
                    // foo(item.Key);
                    // bar(item.Value);
                    foreach (var itemL2 in item.Value)
                    {

                        Dictionary<string, string> dictInner = itemL2.Value;

                        DateTime todayDate = Convert.ToDateTime(itemL2.Key);
                        // campaignRow["CardTime"])
                        // int resultent = DateTime.Compare(Convert.ToDateTime(localMonthDate.ToString("yyyy-MM-dd")), todayDate);//|| resultent == 0
                        if (!dictInner.ContainsKey("title") || dictInner.Count < 15 || localMonthDate.Date == todayDate.Date)
                        {
                            continue;
                        }
                        // itemL2.Key;
                        // Console.Write(dictInner["asm_id"]);
                        long count = 0;
                        count = AttendanceAsmidAndDateWiseExist(conn, dictInner["asm_id"].ToString(), itemL2.Key, count);

                        dictInner["proc"] = "Service";
                        dictInner["applied"] = "0";
                        dictInner["span"] = "0";
                        dictInner["type"] = "P";
                        dictInner["date"] = itemL2.Key;
                        dictInner["sap_code"] = item.Key;

                        if (count <= 0)
                        {
                            AttendaceInsert(conn, dictInner, count);
                        }
                        else
                        {
                            AttendaceUpdate(conn, dictInner, count);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
                // oTransaction.Rollback();
                // throw;
            }
            finally
            {

            }
            // Define a query returning a single row result set
            /*string updateQry = "INSERT INTO campaign_dials (asm_id, date, sap_code, title, InDatetime, Intime, OutDatetime, Outtime, shifttime, lesstime, latearrival, missed, color, Intimefound, Outtimefound, Shifttimefound, ShiftDatetime, ShiftStart, ShiftEnd, difference )";
            updateQry += " values (" + insert["campaign_id"] + ", " + insert["file_id"] + ", " + insert["company_id"] + ", '" + insert["phonefrom"] + "', '" + insert["transferto"] + "', '" + insert["ivrAudio"] + "', '" + insert["vmaudio"] + "', '" + insert["campaigntype"] + "', '" + insert["isvmdrop"] + "', '" + insert["dnclistkeypressnumber"] + "', '" + insert["transfertokeypressnumber"] + "', '" + insert["phoneto"] + "', '" + insert["dropid"] + "', '" + insert["uuid"] + "', '" + insert["provider"] + "', " + insert["created_by"] + ", '" + dt + "' )";
            MySqlCommand command = new MySqlCommand(updateQry, conn);
            // Console.Write(" {0}\n", updateQry);
            // Execute the query and obtain the value of the first column of the first row
            int cnt = command.ExecuteNonQuery();
            // Console.Write("Saved Rown Campaigns are {0}\n", cnt);*/
            // CloseConnection();
            // return null;
        }
        public void Dispose()

        {

            // Using the dispose pattern

            // Dispose(true);

            // … release unmanaged resources here

            GC.SuppressFinalize(this);

        }
        ~AttendanceMachineDaily()
        {
            DictData = null;
            conn.Close();
            conn.Dispose();
            Console.WriteLine(".............................Out Machine Attendance..........................");
        }
    }
}
