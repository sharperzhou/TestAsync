using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;


namespace Test2
{
    class Program
    {
        static void Main(string[] args)
        {
            
            using (var srcStream = new FileStream(
                @"C:\Users\admin\Desktop\TinySTL.zip", FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(srcStream, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(@"C:\Users\admin\Desktop\Dest\");
                }

                using (var stream = new FileStream(
                    @"C:\Users\admin\Desktop\test.zip", FileMode.Create, FileAccess.Write))
                {
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                    {
                        FileInfo fileInfo = new FileInfo(@"C:\Users\admin\Desktop\Dest\TinySTL.sln");
                        archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                        fileInfo = new FileInfo(@"C:\Users\admin\Desktop\Dest\README.md");
                        archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                    }
                }
            }
        }
    }

    
}
