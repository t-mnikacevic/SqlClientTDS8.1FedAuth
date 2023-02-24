using Microsoft.Data.SqlClient;
using System;
using System.Threading;
using System.Diagnostics;
namespace TDSSDemoApp
{
    class Program
    {
        public static double ctr = 0;
        public static bool flag = true;
        public static double proseci = 0;
        public static int numQueries = 1;

        public static double Add(ref double location1, double value)
        {
            double newCurrentValue = location1; // non-volatile read, so may be stale
            while (true)
            {
                double currentValue = newCurrentValue;
                double newValue = currentValue + value;
                newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
                if (newCurrentValue.Equals(currentValue)) // see "Update" below
                    return newValue;
            }
        }

        public static void execQuerry(object con)
        {
            double broj = 0;
            string connString = (string)con;
            var conn = new SqlConnection(connString);
            while (flag)
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    conn.Open();
                    for (int i = 2; i <= numQueries; i++)
                    {
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = "select * from sys.tables"; // the query we are executing in each connection
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                            }
                        }
                    }

                    conn.Close();
                    broj++;
                }
                catch (Exception xcp)
                {
                    Console.WriteLine(xcp);
                }
            }
            Add(ref ctr, broj);
        }

        static void Main(string[] args)
        {
            // new client
            int time = 1 * 10000; // 10 seconds
            int brojNiti = 5; // 5 threads attempting connection
            var connectionString = "Server=localhost,1433;Database=master;TrustServerCertificate=True;" +
                "Authentication=Active Directory Integrated;Encrypt=strict;Connection Timeout=300000;Pooling=false";
            Thread[] tredovi = new Thread[brojNiti];

            for (int i = 0; i < brojNiti; i++)
            {
                tredovi[i] = new Thread(new ParameterizedThreadStart(execQuerry));
                tredovi[i].Start(connectionString);
            }
            Thread.Sleep(time);
            flag = false;
            for (int i = 0; i < brojNiti; i++)
            {
                tredovi[i].Join();
            }
            Console.WriteLine("Avg time: " + time / (ctr / brojNiti) + "ms");
            Console.WriteLine("Number of connections: " + ctr);
            Console.WriteLine("Number of connections per thread: " + ctr / brojNiti);
        }
    }
}
/*
using Microsoft.Data.SqlClient;
using System;



namespace TDSSDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TDS8.1
            var connectionString = "Server=localhost,1433;Database=master;TrustServerCertificate=True;User Id=sa;Password=YUKON900!KATMAI100;Encrypt=strict;Connection Timeout=3000;Pooling=False";

            TDS8.1 AAD
           var connectionString = "Server=localhost,1433;Database=master;TrustServerCertificate=True;Authentication=Active Directory Integrated;Encrypt=strict;Connection Timeout=3000;Pooling=False";

            Console.WriteLine("TDS8.1, Pooling = false");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            long totalTime = 0, iterCount = 1000;
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

*/
