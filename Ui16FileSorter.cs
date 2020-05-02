using System.IO;

namespace FileSorting
{
    public abstract class Ui16FileSorter
    {
        public abstract string GetSortingMethodName();
        public abstract void SortUi16NumbersInFile(string fileName);
        
        protected void WriteUshortToStream(Stream s, ushort value)
        {
            byte[] buf = new byte[]
            {
                (byte)(value & 0xFF),
                (byte)(value >> 8)
            };
            s.Write(buf, 0, 2);
        }

        protected ushort ReadUshortFromStream(Stream s)
        {
            byte[] buf = new byte[2];
            s.Read(buf, 0, 2);
            ushort res = (ushort) ((buf[1] << 8) | buf[0]);
            return res;
        }
        
        protected long getFileLength(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return fi.Length;
        }
        
        
    }
}