using System.IO;

namespace FileSorting
{
    public class CountingUi16FileSorter: Ui16FileSorter
    {
        public override string GetSortingMethodName()
        {
            return "Counting sorting";
        }
        public override void SortUi16NumbersInFile(string fileName)
        {
            long fileLength = getFileLength(fileName);
            long n = fileLength / 2;
            int k = 65536;
            long[] C = new long[k];
            Stream readFs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            for (long i = 0; i < n; i++)
                C[ReadUshortFromStream(readFs)]++;
            readFs.Close();
            
            Stream writeFs = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            for(int x = 0; x < k; x++)
            for (long i = 0; i < C[x]; i++)
                WriteUshortToStream(writeFs, (ushort)x);
            writeFs.Close();
        }

    }
}