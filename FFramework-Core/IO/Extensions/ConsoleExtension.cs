/*
    C# Framework with a lot of useful functions and classes
    Copyright (C) 2019/2020 Bruno Fištrek
    
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
using System.Runtime.InteropServices;

namespace FFramework_Core.IO.Extensions
{
    public static class ConsoleExtension
    {
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void InitConsole()
        {
            try
            {
                IntPtr hw = GetConsoleWindow();
                if (hw == IntPtr.Zero)
                    AllocConsole();

                Console.Clear();
                ShowWindow(hw, 8);
            }
            catch
            {
            }
        }

        public static void CloseConsole()
        {
            try
            {
                IntPtr hw = GetConsoleWindow();
                if (hw != IntPtr.Zero)
                    ShowWindow(hw, 0);
            }
            catch
            {
            }
        }

        public static void WriteColoredLine(string str)
        {
            if (!str.Contains("{") && !str.Contains("}"))
                Console.WriteLine(str);

            str += "{";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '{')
                {
                    int index = str.IndexOf('}', i);
                    int next;
                    if (index != -1)
                        next = str.IndexOf('{', index);
                    else
                        break;

                    string text = null;
                    for (int j = index + 1; j <= next - 1; j++)
                        text += str[j];

                    string color = null;
                    for (int k = i + 1; k <= index - 1; k++)
                        color += str[k];

                    switch (color)
                    {
                        case "white":
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case "blue":
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case "red":
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case "yellow":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case "green":
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case "cyan":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case "black":
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case "gray":
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case "magenta":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                    }
                    Console.Write(text);
                }
            }
            Console.WriteLine();
        }
    }
}