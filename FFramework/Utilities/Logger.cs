/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2016 Bruno Fištrek

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
