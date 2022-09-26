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
        public static int numQuerries;

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
            double ukupnoVreme = 0;
            double brojUpita = 0;

            while (flag)
            {
                string connString = (string)con;
                try
                {
                    using (var conn = new SqlConnection(connString))
                    {
                        Random a = new Random();
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        conn.Open();
                        stopwatch.Stop();
                        ukupnoVreme += stopwatch.ElapsedMilliseconds;
                        brojUpita++;
                        
                        for (int i = 1; i <= numQuerries; i++)
                        {
                            var cmd = conn.CreateCommand();
                            cmd.CommandText = "select random_col from testTable where Id=" + a.Next(1, 140000).ToString();
                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {

                                }
                            }
                        }

                        conn.Close();
                        //conn.Dispose();
                        broj++;
                    }
                }
                catch (Exception xcp)
                {
                    Console.WriteLine(xcp);
                }
                // Interlocked.Increment(ref ctr);
            }
            //Console.WriteLine(ukupnoVreme / brojUpita);
            Add(ref proseci, ukupnoVreme / brojUpita);
            Add(ref ctr, broj);


        }

        static void Main(string[] args)
        {
            proseci = 0;
            int time;
            time = Int32.Parse(args[1]);
            string server = args[0];
            int brojNiti = Int32.Parse(args[2]);
            string pooling = args[3];
            numQuerries = Int32.Parse(args[4]);
            int numPooling = Int32.Parse(args[5]);
            //var connectionString = "Server=testcl.vnetOnebox.markonikolic-95.onebox.xdb.mscds.com,1433;Database=master;TrustServerCertificate=True;User Id=sa;Password=Yukon900!Katmai100";
            //var connectionString = "Server=" + server + ",1433;Database=master;Encrypt=true;User Id=sa;Password=Yukon900Katmai100;Authentication=Sql Password;Pooling=" + pooling;
            var connectionString = "Server="+server+",1433;Database=master;Encrypt=strict;User Id=sa;Password=Yukon900Katmai100;Authentication=Sql Password;Pooling="+pooling + ";max pool size=" + numPooling;
            Thread[] tredovi = new Thread[brojNiti];
            //MDCSSQL-FILCUB1.europe.corp.microsoft.com,1433
            //Thread t = new Thread(new ParameterizedThreadStart(execQuerry));
            //t.Start(connectionString);
            //t.Join(); 

            for (int i = 0; i < brojNiti; i++)
            {
                tredovi[i] = new Thread(new ParameterizedThreadStart(execQuerry));
                tredovi[i].Start(connectionString);
            }
            Thread.Sleep(time * 60000);
            flag = false;
            //Console.WriteLine(ctr);
            for (int i = 0; i < brojNiti; i++)
            {
                tredovi[i].Join();
            }
            Console.WriteLine(proseci / (double)brojNiti + "ms");
            Console.WriteLine(ctr);

        }
    }
}
