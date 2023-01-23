using Microsoft.Data.SqlClient;
using System;



namespace TDSSDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // TDS8.1
            //var connectionString = "Server=localhost,1433;Database=master;TrustServerCertificate=True;User Id=sa;Password=YUKON900!KATMAI100;Encrypt=strict;Connection Timeout=3000";

            // TDS8.1 AAD
            var connectionString = "Server=localhost,1433;Database=master;TrustServerCertificate=True;Authentication=Active Directory Integrated;Encrypt=strict;Connection Timeout=3000;Pooling=False";
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            long totalTime = 0, iterCount = 20;
            for (long i = 0; i < iterCount; i++)
            {
                watch.Reset();
                watch.Start();
                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        watch.Stop();
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
                Console.WriteLine("iteration " + i.ToString() + ":" + watch.ElapsedMilliseconds + "ms");
                totalTime += watch.ElapsedMilliseconds;
            }
            Console.WriteLine("avg time: " + totalTime / iterCount + "ms");
            Console.ReadLine();
        }
    }
}
