using System;
using System.IO;

namespace FileSorting
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Program pr = new Program();
            pr.Run();
        }
        
        private void Run()
        {
            //TestSorting( new MergeUi16FileSorter());
            TestSorting( new CountingUi16FileSorter());
        }

        private void TestSorting(Ui16FileSorter sorter)
        {
            string fileName = "testfile.data";
            TestSorting(sorter, fileName, 100000L);//10^5
            TestSorting(sorter, fileName, 1*1000000L);//10^6
            TestSorting(sorter, fileName, 10*1000000L);//10^7
            TestSorting(sorter, fileName, 100*1000000L);//10^8
            TestSorting(sorter, fileName, 1000*1000000L);//10^9
        }

        private void TestSorting(Ui16FileSorter sorter, string fileName, long N)
        {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Creating a file with " + N + " 16-bit numbers...");
            GenerateFileWNumbers(fileName, N);
            Console.WriteLine("file created");

            ulong cs1 = GetSumOfNumbers(fileName, N);
            Console.WriteLine("CS: " + cs1);
            
            Console.WriteLine(sorter.GetSortingMethodName() + "...");
            DateTime startDt = DateTime.Now;
            sorter.SortUi16NumbersInFile(fileName);
            TimeSpan timeSpan = DateTime.Now - startDt;
            Console.WriteLine("Sorted for " + timeSpan.TotalSeconds + " seconds" );
            ulong cs2 = GetSumOfNumbers(fileName, N);
            Console.WriteLine(cs1 == cs2? "CS is OK":"ERROR: CS doesn't match (was " + cs1 + ", now " + cs2 + ")" );
        }

        private void GenerateFileWNumbers(string fileName, long n) {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            byte[] buf = new byte[2];
            Random random = new Random();
            for (long i = 0; i < n; i++)
            {
                /*int v = random.Next(65535);
                buf[0] = (byte)(v & 0xFF);
                buf[1] = (byte)( (v >> 8) & 0xFF);*/
                random.NextBytes(buf);
                fs.Write(buf, 0, 2);
            }
            fs.Close();
        }
        
        private ulong GetSumOfNumbers(string fileName, long n)
        {
            ulong cs = 0;
            FileStream fs = new FileStream(fileName,FileMode.Open, FileAccess.Read);
            for (long i = 0; i < n; i++)
            {
                cs += ReadUshortFromStream(fs);
            }
            fs.Close();
            return cs;
        }
        
        private ushort ReadUshortFromStream(Stream s)
        {
            byte[] buf = new byte[2];
            s.Read(buf, 0, 2);
            ushort res = (ushort) ((buf[1] << 8) | buf[0]);
            return res;
        }
        
        
        
        
        
        
    }
}