using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMechProjectManager_HW.Utils;

namespace Test3
{
    class Program
    {
        static void Main(string[] args)
        {
            var get = HttpClientHelper.GetString("http://192.168.2.105:8080/site/getTest",
                new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("project_code","111"),
                    new KeyValuePair<string, string>("project_id","111"),
                    new KeyValuePair<string, string>("user_info","111"),
                });

            var post = HttpClientHelper.PostString("http://192.168.2.105:8080/site/postTest",
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("account","111"),
                });

            using (var srcStream = new FileStream(@"C:\Users\admin\Desktop\TinySTL.zip", FileMode.Open))
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
