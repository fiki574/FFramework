using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FFramework.FUtilities
{
    public class ProcessEx
    {
        ///<summary>Check if "process_name" is started</summary>
        ///<param name="process_name">Process you want to check if it's running</param>
        ///<returns>true if the process is running, false if it's not</returns>
        public static bool IsProcessActive(string process_name)
        {
            Process[] pname = Process.GetProcessesByName(process_name);
            if (pname.Length == 0) return false;
            else return true;
        }

        ///<summary>Retrieve the proper "process_name" process for further use</summary>
        ///<param name="process_name">Process you want to check and retrieve if possible</param>
        ///<returns>Correct process if found, if not then null</returns>
        public static Process GetProcess(string process_name)
        {
            Process[] pname = Process.GetProcessesByName(process_name);
            if (pname.Length == 0) return null;
            else return pname[0];
        }
    }
}
