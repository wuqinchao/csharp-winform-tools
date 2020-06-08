using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cloudthinking.BuildTools.ReVersion
{
    class Program
    {
        static string filePath = "";
        static string version = "";
        static string startDate = "";
        static int Main(string[] args)
        {
            if (!parseCmd(args))
            {
                print();
                return 1;
            }
              
            if (File.Exists(filePath))
            {
                Encoding defaultEncoding = TxtFileEncoder.GetEncoding(filePath);
                string txt = File.ReadAllText(filePath);
                string org_v = SearchVersion(txt);
                string new_v = "";
                if (string.IsNullOrEmpty(version))
                {
                    new_v = UpgradeVersion(org_v);
                }
                else
                {
                    new_v = version;
                }
                txt = txt.Replace(org_v, new_v);
                using (StreamWriter writer = new StreamWriter(filePath, false, defaultEncoding))
                {
                    writer.Write(txt);
                }
            }
            else
            {
                return 2;
            }
            return 0;
        }

        static void print()
        {
            Console.WriteLine("-f <file> [-s <date>] [-v <version>]");
            Console.WriteLine("该程序用来修改C#程序中定义的程序版本号和文件版本");
            Console.WriteLine("参数说明");
            Console.WriteLine("--参数之间用空格分割,参数中有空格需用双引号");
            Console.WriteLine("-f <file> 指定需要修改的包含版本号的.cs文件");
            Console.WriteLine("-s <date> BUILD号起始日期, 如2015-01-01, 不指定则默认为2015-01-01");
            Console.WriteLine("          如已指定-v, 则此参数会无效");
            Console.WriteLine("-v <version> 指定目标版本号, 如不指定则自动生成");

            Console.WriteLine("返回值说明");
            Console.WriteLine("0 成功");
            Console.WriteLine("1 参数错误");
            Console.WriteLine("2 文件不存在");
        }

        static bool parseCmd(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false;
            }
            List<string> arg_list = new List<string>(args);
            while (arg_list.Count > 0)
            {
                if (arg_list[0] == "-f")
                {
                    filePath = arg_list[1];
                    arg_list.RemoveRange(0, 2);
                }
                else if (arg_list[0] == "-s")
                {
                    startDate = arg_list[1];
                    arg_list.RemoveRange(0, 2);
                }
                else if (arg_list[0] == "-v")
                {
                    version = arg_list[1];
                    arg_list.RemoveRange(0, 2);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        static string SearchVersion(string input)
        {
            string version = "";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"\[assembly: AssemblyVersion\(""(?<version>\d+\.\d+\.\d+\.\d+?)""\)\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var m = reg.Match(input);
            if (m.Success && m.Groups["version"].Success)
            {
                version = m.Groups["version"].Value;
            }
            return version;
        }

        static string UpgradeVersion(string version)
        {
            Version org_version = new Version(version);
            Version new_version = new Version(org_version.Major, org_version.Minor, getBuild(), getReversion());
            return new_version.ToString();
        }
        static int getBuild()
        {
            DateTime dt_start;
            DateTime dt_target;
            if (!DateTime.TryParse(startDate, out dt_start))
            {
                dt_start = new DateTime(2015, 1, 1);
            }
            dt_target = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            int days = (dt_target - dt_start).Days;
            return days;
        }
        static int getReversion()
        {
            DateTime b = DateTime.Now;
            TimeSpan s = (b - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0));

            return Convert.ToInt32(s.TotalMinutes);
        }
    }
}
