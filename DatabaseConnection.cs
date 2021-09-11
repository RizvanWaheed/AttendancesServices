using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace AttendancesServices
{
    public static class DatabaseConnection
    {
        static MySqlConnection databaseConnection = null;
        public static MySqlConnection GetDBConnection()
        {
            if (databaseConnection == null)
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                    databaseConnection = new MySqlConnection(connectionString);

                }
                catch (MySqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 0:
                            Console.WriteLine("Cannot connect to server.  Contact administrator");
                            break;
                        case 1045:
                            Console.WriteLine("Invalid username/password, please try again");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().Message);
                }

            }
            return databaseConnection;
        }

        /* public static void Disconnect()
         {
             databaseConnection.Close();
             databaseConnection.Dispose();
             databaseConnection = null;
         }*/
    }
}
