using Microsoft.Data.SqlClient;
using System;



namespace TDSSDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost,1433;Database=master;TrustServerCertificate=True;User Id=sa;Password=Yukon900!Katmai100";
            //var connectionString = "Server=testcl.public.vnetOnebox.MDCSSQL-MANIKO1.onebox.xdb.mscds.com,3342;Database=master;Encrypt=true;User Id=CloudSA;Password=uE5vL5aP8jO7;";



            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "select @@servername";
                    Console.WriteLine(cmd.ExecuteScalar());



                    Console.WriteLine();



                    cmd.CommandText = "select * from sys.tables";
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Console.WriteLine(rdr.GetString(0));
                        }
                    }
                }
            }
            catch (Exception xcp)
            {
                Console.WriteLine(xcp);
            }
            Console.ReadLine();
        }
    }
}
