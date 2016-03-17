using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace FFramework.Utilities
{
    public class Logger
    {
        ///<summary>Write console text in color, save log lines in file, etc.</summary>
        ///<param name="caller">A function/part of application you're calling this function from</param>
        ///<param name="caller_color">A console display color for that caller</param>
        ///<param name="message">Message you want to display as actual log line</param>
        ///<param name="caller_color">A console display color for that message</param>
        ///<param name="use_timestamps">If true, the timestamps will appear at the start of each console line in gray color</param>
        ///<param name="save_in_file">If true, all log lines will be saved to a log file named in parameter "file_name"</param>
        ///<param name="file_name">A file you want to save your log lines if, it can be a path or a name, by default it's set to the directory where your application is and log file is named 'defaultname.log'</param>
        ///<returns>(nothing)</returns>
        public static void Write(string caller, ConsoleColor caller_color, string message, ConsoleColor message_color, bool use_timestamps = true, bool save_in_file = true, string file_name = "defaultname.log")
        {
            Mutex mutex = new Mutex();
            try
            {
                mutex.WaitOne();
                string text = null;
                Console.BufferHeight = Console.WindowWidth - 20;
                if (use_timestamps == true)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
                    Console.Write(text);
                }
                Console.ForegroundColor = caller_color;
                Console.Write(caller + ": ");
                Console.ForegroundColor = message_color;
                Console.Write(message + '\n');

                StreamWriter file = System.IO.File.AppendText(file_name);
                if(use_timestamps == true) file.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + caller + ": " + message);
                else file.WriteLine(caller + ": " + message);
                file.Close();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
