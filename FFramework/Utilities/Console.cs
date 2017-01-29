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

namespace FFramework.Utilities
{
    public class Console
    {
        public static void WriteColoredLine(string str)
        {
            if (!str.Contains("{") && !str.Contains("}")) System.Console.WriteLine(str);
            str += "{";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '{')
                {
                    int index = str.IndexOf('}', i);
                    int next;
                    if (index != -1) next = str.IndexOf('{', index);
                    else break;
                    string text = null;
                    for (int j = index + 1; j <= next - 1; j++) text += str[j];
                    string color = null;
                    for (int k = i + 1; k <= index - 1; k++) color += str[k];
                    switch (color)
                    {
                        case "white":
                            System.Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case "blue":
                            System.Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case "red":
                            System.Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case "yellow":
                            System.Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case "green":
                            System.Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case "cyan":
                            System.Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case "black":
                            System.Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case "gray":
                            System.Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case "magenta":
                            System.Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                    }
                    System.Console.Write(text);
                }
            }
            System.Console.WriteLine();
        }

        public static void WriteStructMembers(object o)
        {
            foreach (var field in o.GetType().GetFields())
            {
                var value = field.GetValue(o);
                if (field.FieldType.IsArray)
                {
                    Array a = (Array)value;
                    string s = null;
                    for (int i = 0; i < a.Length; i++) s += Convert.ToString(a.GetValue(i));
                    System.Console.WriteLine(field.Name + " (" + field.FieldType.ToString() + "): " + s);
                }
                else System.Console.WriteLine(field.Name + " (" + field.FieldType.ToString() + "): " + value);
            }
        }
    }
}
