using System;
using System.IO;
using System.Text;

namespace FFramework.Utilities
{
    public class CSV
    {
        public static ThreadSafeList<String[]> GetFileContent(string csv_file)
        {
            ThreadSafeList<String[]> all = new ThreadSafeList<String[]>();
            try
            {
                if (!System.IO.File.Exists(csv_file)) return null;
                StreamReader f = new StreamReader(csv_file, Encoding.Default);
                string line;
                while ((line = f.ReadLine()) != null) all.Add(line.Split(new char[] { ';' }));
            }
            catch
            {
                return null;
            }
            return all;
        }
    }
}
