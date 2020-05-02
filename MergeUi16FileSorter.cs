using System.IO;

namespace FileSorting
{
    public class MergeUi16FileSorter: Ui16FileSorter
    {
        private const long MIN_FILE_SIZE = 1024*1024;
        private const long MIN2 = 8192*1024;

        public override string GetSortingMethodName()
        {
            return "Merge sorting";
        }

        public override void SortUi16NumbersInFile(string fileName)
        {   
            long fileLength = getFileLength(fileName);
            Stream mainFs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 8192);
            SplitMergeFile(0, fileLength/2, fileName, mainFs);
            mainFs.Close();
        }
        
        private void SplitMergeFile( long begin, long end, string fileName, Stream stream, bool stIsMemSt=false, long stOffset=0) 
        {
            if (end - begin < 2)
                return;
            long middle = begin + (end - begin) / 2;

            if (!stIsMemSt && (end - begin) < MIN2)
            {//use MemoryStream instead of FileStream
                Stream memSt = new MemoryStream(new byte[(end - begin)*2]);
                //копируем кусок [begin end) из stream в memSt
                stream.Position = begin * 2;
                for (long i = begin; i < end;i++)
                    WriteUshortToStream(memSt, ReadUshortFromStream(stream));

                SplitMergeFile(begin, middle, fileName, memSt, true, begin);
                SplitMergeFile(middle, end, fileName, memSt, true, begin);
                MergeFile( begin, middle, end, fileName, memSt, begin);
                //копировать обратно в stream
                stream.Position = begin * 2;
                memSt.Position = 0;
                for (long i = begin; i < end;i++)
                    WriteUshortToStream(stream, ReadUshortFromStream(memSt));
                memSt.Close();
                return;
            }
            
            
            SplitMergeFile(begin, middle, fileName, stream, stIsMemSt, stOffset);
            SplitMergeFile(middle, end, fileName, stream, stIsMemSt, stOffset);
            MergeFile( begin, middle, end, fileName, stream, stOffset);
        }
        
        private void MergeFile( long begin, long middle, long end, string fileName, Stream stream, long stOffset=0) {
            long fst = begin;
            long snd = middle;

            Stream stPart1;
            string part1FileName = "";
            long part1SizeInBytes = (middle - begin) * 2; 
            if (part1SizeInBytes < MIN_FILE_SIZE)
                stPart1 = new MemoryStream(new byte[part1SizeInBytes]);

            else
            {
                part1FileName = fileName + ".merge1p." + begin + "." + middle;
                stPart1 = new FileStream(part1FileName, FileMode.Create,
                    FileAccess.ReadWrite);
            }

            Stream stPart2;
            string part2FileName = "";
            long part2SizeInBytes = (end - middle) * 2;
            if (part2SizeInBytes < MIN_FILE_SIZE)
                stPart2 = new MemoryStream(new byte[part2SizeInBytes]);
            else
            {
                part2FileName = fileName + ".merge2p." + middle + "." + end;
                stPart2 = new FileStream(part2FileName, FileMode.Create,
                    FileAccess.ReadWrite);
            }

            stream.Seek((begin * 2)-(stOffset * 2),SeekOrigin.Begin);
            for (long i = begin; i < middle; i++)
                WriteUshortToStream(stPart1, ReadUshortFromStream(stream));
            
            for (long i = middle; i < end; i++)
                WriteUshortToStream(stPart2, ReadUshortFromStream(stream));
            
            //
            stream.Seek((begin * 2)-(stOffset * 2),SeekOrigin.Begin);
            stPart1.Position = 0;
            stPart2.Position = 0;
            ushort srcv1 = ReadUshortFromStream(stPart1);
            ushort srcv2 = ReadUshortFromStream(stPart2);
            
            while( fst < middle && snd < end) 
            {
                
                if( srcv1 < srcv2 )
                {
                    WriteUshortToStream(stream, srcv1);
                    //dest[k] = src[fst];
                    fst++;
                    if (fst < middle)
                        srcv1 = ReadUshortFromStream(stPart1);
                    
                }
                else {
                    WriteUshortToStream(stream, srcv2);
                    //dest[k] = src[snd];
                    snd++;
                    if (snd < end)
                        srcv2 = ReadUshortFromStream(stPart2);
                    
                }
			
            }

            while (fst < middle)
            {
                WriteUshortToStream(stream, srcv1);
                fst++;
                if (fst < middle)
                    srcv1 = ReadUshortFromStream(stPart1);
                
            }

            while (snd < end)
            {
                WriteUshortToStream(stream, srcv2);
                snd++;
                if (snd < end)
                    srcv2 = ReadUshortFromStream(stPart2);
                
            }
            
            stPart1.Close();
            stPart2.Close();
            
            if (stPart1.GetType() == typeof(FileStream))
                File.Delete(part1FileName);
            
            if (stPart2.GetType() == typeof(FileStream))
                File.Delete(part2FileName);
            
        }

    }//public class MergeUI16FileSorter
}